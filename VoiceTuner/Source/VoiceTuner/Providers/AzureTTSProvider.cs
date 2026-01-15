using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using VoiceTuner.Models;

namespace VoiceTuner.Providers
{
    /// <summary>
    /// Azure TTS提供商 - 支持完整SSML（情感、角色、语速、音调）
    /// </summary>
    public class AzureTTSProvider : ITTSProvider
    {
        private readonly HttpClient httpClient;
        
        public string ProviderName => "Azure TTS";
        public string ProviderDescription => "微软认知服务TTS，支持丰富的情感表达和角色扮演";
        public TTSProviderType ProviderType => TTSProviderType.Azure;
        
        public AzureTTSProvider()
        {
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        
        public async Task<byte[]?> SynthesizeAsync(string text, TTSProviderConfig config)
        {
            if (string.IsNullOrEmpty(config.ApiKey))
            {
                Log.Warning("[AzureTTSProvider] API Key is not configured");
                return null;
            }
            
            try
            {
                string endpoint = $"https://{config.AzureRegion}.tts.speech.microsoft.com/cognitiveservices/v1";
                string ssml = BuildSSML(text, config);
                
                if (Prefs.DevMode)
                {
                    Log.Message($"[AzureTTSProvider] Endpoint: {endpoint}");
                    Log.Message($"[AzureTTSProvider] SSML:\n{ssml}");
                }
                
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Headers.Add("Ocp-Apim-Subscription-Key", config.ApiKey);
                request.Headers.Add("X-Microsoft-OutputFormat", "riff-48khz-16bit-mono-pcm");
                request.Headers.Add("User-Agent", "VoiceTuner-RimWorld");
                request.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");
                
                var response = await httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Log.Error($"[AzureTTSProvider] Error: {response.StatusCode}");
                    Log.Error($"[AzureTTSProvider] Details: {error}");
                    return null;
                }
                
                byte[] audioData = await response.Content.ReadAsByteArrayAsync();
                
                if (Prefs.DevMode)
                {
                    Log.Message($"[AzureTTSProvider] Audio generated: {audioData.Length} bytes");
                }
                
                return audioData;
            }
            catch (Exception ex)
            {
                Log.Error($"[AzureTTSProvider] Exception: {ex.Message}");
                return null;
            }
        }
        
        private string BuildSSML(string text, TTSProviderConfig config)
        {
            string rateStr = FormatPercent(config.Speed);
            string pitchStr = FormatPercent(config.Pitch);
            string escapedText = System.Security.SecurityElement.Escape(text);
            bool hasStyle = !string.IsNullOrEmpty(config.AzureStyle) && config.VoiceId.Contains("Neural");
            bool hasRole = !string.IsNullOrEmpty(config.AzureRole);
            
            StringBuilder ssml = new StringBuilder();
            ssml.AppendLine("<?xml version='1.0' encoding='utf-8'?>");
            ssml.AppendLine("<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xmlns:mstts='https://www.w3.org/2001/mstts' xml:lang='zh-CN'>");
            ssml.AppendLine($"  <voice name='{config.VoiceId}'>");
            
            if (hasStyle || hasRole)
            {
                StringBuilder expressAs = new StringBuilder();
                expressAs.Append("    <mstts:express-as");
                
                if (hasStyle)
                {
                    expressAs.Append($" style='{config.AzureStyle}'");
                    expressAs.Append($" styledegree='{config.AzureStyleDegree:F2}'");
                }
                
                if (hasRole)
                {
                    expressAs.Append($" role='{config.AzureRole}'");
                }
                
                expressAs.AppendLine(">");
                ssml.Append(expressAs);
                
                ssml.AppendLine($"      <prosody rate='{rateStr}' pitch='{pitchStr}'>");
                ssml.AppendLine($"        {escapedText}");
                ssml.AppendLine("      </prosody>");
                ssml.AppendLine("    </mstts:express-as>");
            }
            else
            {
                ssml.AppendLine($"    <prosody rate='{rateStr}' pitch='{pitchStr}'>");
                ssml.AppendLine($"      {escapedText}");
                ssml.AppendLine("    </prosody>");
            }
            
            ssml.AppendLine("  </voice>");
            ssml.AppendLine("</speak>");
            
            return ssml.ToString();
        }
        
        private string FormatPercent(float value)
        {
            int percent = (int)((value - 1.0f) * 100);
            if (percent >= 0)
            {
                return $"+{percent}%";
            }
            return $"{percent}%";
        }
        
        public async Task<bool> TestConnectionAsync(TTSProviderConfig config)
        {
            try
            {
                byte[]? result = await SynthesizeAsync("测试", config);
                return result != null && result.Length > 0;
            }
            catch
            {
                return false;
            }
        }
        
        public List<(string Id, string DisplayName)> GetAvailableVoices()
        {
            var result = new List<(string, string)>();
            foreach (var voice in AzureTTSVoices.Voices)
            {
                result.Add((voice.Id, $"{voice.DisplayName} [{voice.Language}]"));
            }
            return result;
        }
        
        public void DrawConfigPanel(Rect rect, TTSProviderConfig config)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            
            listing.Label("Azure 区域:");
            config.AzureRegion = listing.TextEntry(config.AzureRegion);
            listing.Gap(6f);
            
            listing.Label("API 密钥:");
            config.ApiKey = listing.TextEntry(config.ApiKey);
            listing.Gap(6f);
            
            listing.Label("语音:");
            if (listing.ButtonText(config.VoiceId))
            {
                ShowVoiceSelectionMenu(config);
            }
            listing.Gap(6f);
            
            listing.Label($"语速: {config.Speed:F2}x");
            config.Speed = listing.Slider(config.Speed, 0.5f, 2.0f);
            listing.Gap(6f);
            
            listing.Label($"音调: {config.Pitch:F2}");
            config.Pitch = listing.Slider(config.Pitch, 0.5f, 2.0f);
            listing.Gap(6f);
            
            listing.Label("情感风格:");
            string styleDisplay = string.IsNullOrEmpty(config.AzureStyle) ? "(无)" : config.AzureStyle;
            if (listing.ButtonText(styleDisplay))
            {
                ShowStyleSelectionMenu(config);
            }
            listing.Gap(6f);
            
            if (!string.IsNullOrEmpty(config.AzureStyle))
            {
                listing.Label($"情感强度: {config.AzureStyleDegree:F2}");
                config.AzureStyleDegree = listing.Slider(config.AzureStyleDegree, 0.01f, 2.0f);
                listing.Gap(6f);
            }
            
            listing.Label("角色扮演:");
            string roleDisplay = string.IsNullOrEmpty(config.AzureRole) ? "(无)" : config.AzureRole;
            if (listing.ButtonText(roleDisplay))
            {
                ShowRoleSelectionMenu(config);
            }
            
            listing.End();
        }
        
        private void ShowVoiceSelectionMenu(TTSProviderConfig config)
        {
            var options = new List<FloatMenuOption>();
            
            foreach (var voice in AzureTTSVoices.Voices)
            {
                string voiceId = voice.Id;
                string display = $"{voice.DisplayName} [{voice.Language}]";
                options.Add(new FloatMenuOption(display, () => {
                    config.VoiceId = voiceId;
                }));
            }
            
            Find.WindowStack.Add(new FloatMenu(options));
        }
        
        private void ShowStyleSelectionMenu(TTSProviderConfig config)
        {
            var options = new List<FloatMenuOption>();
            
            foreach (var style in AzureTTSStyles.Styles)
            {
                string styleCopy = style;
                string display = string.IsNullOrEmpty(style) ? "(无)" : style;
                options.Add(new FloatMenuOption(display, () => {
                    config.AzureStyle = styleCopy;
                }));
            }
            
            Find.WindowStack.Add(new FloatMenu(options));
        }
        
        private void ShowRoleSelectionMenu(TTSProviderConfig config)
        {
            var options = new List<FloatMenuOption>();
            
            foreach (var role in AzureTTSStyles.Roles)
            {
                string roleCopy = role;
                string display = string.IsNullOrEmpty(role) ? "(无)" : role;
                options.Add(new FloatMenuOption(display, () => {
                    config.AzureRole = roleCopy;
                }));
            }
            
            Find.WindowStack.Add(new FloatMenu(options));
        }

        public Task<string?> UploadVoiceAsync(string filePath, TTSProviderConfig config)
        {
            Log.Warning("Azure TTS provider does not support voice cloning via audio upload.");
            return Task.FromResult<string?>(null);
        }
    }
}