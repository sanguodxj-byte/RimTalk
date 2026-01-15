using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Newtonsoft.Json;
using VoiceTuner.Models;

namespace VoiceTuner.Providers
{
    /// <summary>
    /// Fish Audio TTS提供商
    /// </summary>
    public class FishAudioProvider : ITTSProvider
    {
        private readonly HttpClient httpClient;
        
        public string ProviderName => "Fish Audio";
        public string ProviderDescription => "Fish Audio TTS，支持参考音色克隆";
        public TTSProviderType ProviderType => TTSProviderType.FishAudio;
        
        public FishAudioProvider()
        {
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(60);
        }
        
        public async Task<byte[]?> SynthesizeAsync(string text, TTSProviderConfig config)
        {
            if (string.IsNullOrEmpty(config.ApiKey))
            {
                Log.Warning("[FishAudioProvider] API Key is not configured");
                return null;
            }
            
            if (string.IsNullOrEmpty(config.FishReferenceId))
            {
                Log.Warning("[FishAudioProvider] Reference ID is not configured");
                return null;
            }
            
            try
            {
                string endpoint = string.IsNullOrEmpty(config.ApiEndpoint) 
                    ? "https://api.fish.audio/v1/tts" 
                    : config.ApiEndpoint;
                
                var requestBody = new
                {
                    text = text,
                    reference_id = config.FishReferenceId,
                    format = config.FishFormat,
                    latency = config.FishLatency,
                    streaming = false,
                    normalize = true
                };
                
                string jsonBody = JsonConvert.SerializeObject(requestBody);
                
                if (Prefs.DevMode)
                {
                    Log.Message($"[FishAudioProvider] Endpoint: {endpoint}");
                    Log.Message($"[FishAudioProvider] Request: {jsonBody}");
                }
                
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Headers.Add("Authorization", $"Bearer {config.ApiKey}");
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                
                var response = await httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Log.Error($"[FishAudioProvider] Error: {response.StatusCode}");
                    Log.Error($"[FishAudioProvider] Details: {error}");
                    return null;
                }
                
                byte[] audioData = await response.Content.ReadAsByteArrayAsync();
                
                if (Prefs.DevMode)
                {
                    Log.Message($"[FishAudioProvider] Audio generated: {audioData.Length} bytes");
                }
                
                return audioData;
            }
            catch (Exception ex)
            {
                Log.Error($"[FishAudioProvider] Exception: {ex.Message}");
                return null;
            }
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
            return new List<(string, string)>
            {
                ("custom", "自定义参考音色（需输入Reference ID）")
            };
        }
        
        public void DrawConfigPanel(Rect rect, TTSProviderConfig config)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            
            listing.Label("API 端点:");
            config.ApiEndpoint = listing.TextEntry(config.ApiEndpoint);
            if (string.IsNullOrEmpty(config.ApiEndpoint))
            {
                config.ApiEndpoint = "https://api.fish.audio/v1/tts";
            }
            listing.Gap(6f);
            
            listing.Label("API 密钥:");
            config.ApiKey = listing.TextEntry(config.ApiKey);
            listing.Gap(6f);
            
            listing.Label("参考音色 ID:");
            config.FishReferenceId = listing.TextEntry(config.FishReferenceId);
            GUI.color = Color.gray;
            listing.Label("  (在Fish Audio网站获取音色ID)");
            GUI.color = Color.white;
            listing.Gap(6f);
            
            listing.Label("延迟模式:");
            if (listing.ButtonText(config.FishLatency))
            {
                var options = new List<FloatMenuOption>
                {
                    new FloatMenuOption("normal（标准）", () => config.FishLatency = "normal"),
                    new FloatMenuOption("balanced（平衡）", () => config.FishLatency = "balanced")
                };
                Find.WindowStack.Add(new FloatMenu(options));
            }
            listing.Gap(6f);
            
            listing.Label("输出格式:");
            if (listing.ButtonText(config.FishFormat))
            {
                var options = new List<FloatMenuOption>
                {
                    new FloatMenuOption("wav", () => config.FishFormat = "wav"),
                    new FloatMenuOption("mp3", () => config.FishFormat = "mp3"),
                    new FloatMenuOption("opus", () => config.FishFormat = "opus")
                };
                Find.WindowStack.Add(new FloatMenu(options));
            }
            
            listing.End();
        }

        public Task<string?> UploadVoiceAsync(string filePath, TTSProviderConfig config)
        {
            Log.Warning("Fish Audio provider does not support voice cloning via audio upload in this manner.");
            return Task.FromResult<string?>(null);
        }
    }
}