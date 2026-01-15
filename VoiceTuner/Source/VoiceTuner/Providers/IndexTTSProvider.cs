using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Newtonsoft.Json;
using VoiceTuner.Models;
using System.IO;

namespace VoiceTuner.Providers
{
    /// <summary>
    /// IndexTTS-2 提供商（硅基流动音色克隆）
    /// </summary>
    public class IndexTTSProvider : ITTSProvider
    {
        private readonly HttpClient httpClient;
        private string audioFilePathBuffer = ""; // 用于UI输入的缓冲区

        public string ProviderName => "IndexTTS-2 (语音克隆)";
        public string ProviderDescription => "通过上传音频克隆音色，由硅基流动驱动。";
        public TTSProviderType ProviderType => TTSProviderType.IndexTTS;

        public IndexTTSProvider()
        {
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(180); // 上传和处理可能需要更长时间
        }

        public async Task<byte[]?> SynthesizeAsync(string text, TTSProviderConfig config)
        {
            try
            {
                // 验证音色ID：允许 speech:custom: 开头的克隆音色，或者预设音色（不为空即可）
                if (string.IsNullOrEmpty(config.IndexSpeakerId))
                {
                    Log.Error("[IndexTTSProvider] 音色URI无效或为空，请先上传音频进行克隆或输入预设音色名。");
                    return null;
                }

                // 合成请求使用固定的端点
                string endpoint = "https://api.siliconflow.cn/v1/audio/speech";

                var requestBody = new
                {
                    model = "IndexTeam/IndexTTS-2",
                    input = text,
                    voice = config.IndexSpeakerId,
                    speed = config.Speed,
                    response_format = config.IndexFormat,
                    emotion = string.IsNullOrEmpty(config.IndexEmotion) ? null : config.IndexEmotion
                };

                string jsonBody = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (Prefs.DevMode)
                {
                    Log.Message($"[IndexTTSProvider] Synthesize Endpoint: {endpoint}");
                    Log.Message($"[IndexTTSProvider] Synthesize Request: {jsonBody}");
                }

                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Headers.Add("Authorization", $"Bearer {config.ApiKey}");
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Log.Error($"[IndexTTSProvider] Synthesize Error: {response.StatusCode}");
                    Log.Error($"[IndexTTSProvider] Details: {error}");
                    return null;
                }

                byte[] audioData = await response.Content.ReadAsByteArrayAsync();

                if (Prefs.DevMode)
                {
                    Log.Message($"[IndexTTSProvider] Audio generated: {audioData.Length} bytes");
                }

                return audioData;
            }
            catch (Exception ex)
            {
                Log.Error($"[IndexTTSProvider] Synthesize Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> TestConnectionAsync(TTSProviderConfig config)
        {
            // 对于这个提供商，测试连接等同于一次成功的合成
            try
            {
                byte[]? result = await SynthesizeAsync("连接测试", config);
                return result != null && result.Length > 0;
            }
            catch
            {
                return false;
            }
        }
        
        public List<(string Id, string DisplayName)> GetAvailableVoices()
        {
            // 返回一些预设音色，方便用户测试
            return new List<(string, string)>
            {
                ("alex", "Alex (预设)"),
                ("anna", "Anna (预设)"),
                ("bella", "Bella (预设)"),
                ("benjamin", "Benjamin (预设)"),
                ("charles", "Charles (预设)"),
                ("david", "David (预设)"),
                ("dinah", "Dinah (预设)"),
                ("sanjiu", "Sanjiu (预设)")
            };
        }

        public void DrawConfigPanel(Rect rect, TTSProviderConfig config)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);

            // API密钥
            listing.Label("硅基流动 API 密钥:");
            config.ApiKey = listing.TextEntry(config.ApiKey);
            listing.Gap(6f);

            // 音色选择
            listing.Label("选择预设音色:");
            string voiceDisplay = config.IndexSpeakerId;
            // 尝试找到友好的显示名称
            var voices = GetAvailableVoices();
            var currentVoice = voices.Find(v => v.Id == config.IndexSpeakerId);
            if (currentVoice.Id != null)
            {
                voiceDisplay = currentVoice.DisplayName;
            }
            else if (string.IsNullOrEmpty(voiceDisplay))
            {
                voiceDisplay = "(请选择或输入)";
            }

            if (listing.ButtonText(voiceDisplay))
            {
                var options = new List<FloatMenuOption>();
                foreach (var v in voices)
                {
                    options.Add(new FloatMenuOption(v.DisplayName, () => config.IndexSpeakerId = v.Id));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }
            listing.Gap(6f);

            // 音色 URI / 预设名称 (手动输入)
            listing.Label("音色 URI / 自定义名称 (手动输入):");
            config.IndexSpeakerId = listing.TextEntry(config.IndexSpeakerId, 2); //允许多行以显示完整的URI
            GUI.color = Color.gray;
            listing.Label(" (可填入克隆生成的URI，或直接输入 `sanjiu` 等预设音色名)");
            GUI.color = Color.white;
            listing.Gap(6f);

            // 音频文件路径输入
            listing.Label("要克隆的音频文件路径:");
            audioFilePathBuffer = listing.TextEntry(audioFilePathBuffer);
            GUI.color = Color.gray;
            listing.Label(" (例如: C:\\Users\\YourName\\Desktop\\voice.wav)");
            GUI.color = Color.white;
            
            // 上传按钮
            if (listing.ButtonText("📁 克隆音色(上传)"))
            {
                // 注意：RimWorld的UI不是为长时异步操作设计的，这里没有实现完美的UI阻塞
                // 但我们会通过日志和状态消息反馈
                _ = UploadVoiceAsync(audioFilePathBuffer, config);
            }
            listing.Gap(12f);

            listing.Label("情感 (可选):");
            config.IndexEmotion = listing.TextEntry(config.IndexEmotion);
            listing.Gap(6f);
            
            listing.Label($"语速: {config.Speed:F2}x");
            config.Speed = listing.Slider(config.Speed, 0.5f, 2.0f);
            listing.Gap(6f);
            
            listing.Label("输出格式:");
            if (listing.ButtonText(config.IndexFormat))
            {
                var options = new List<FloatMenuOption>
                {
                    new FloatMenuOption("wav", () => config.IndexFormat = "wav"),
                    new FloatMenuOption("mp3", () => config.IndexFormat = "mp3")
                };
                Find.WindowStack.Add(new FloatMenu(options));
            }
            listing.Gap(12f);

            GUI.color = new Color(0.7f, 0.85f, 1f);
            listing.Label("📌 IndexTTS-2 克隆说明:");
            GUI.color = Color.gray;
            listing.Label("  1. 粘贴 API 密钥。");
            listing.Label("  2. 粘贴 5-10 秒的音频文件路径。");
            listing.Label("  3. 点击“克隆音色”按钮，成功后上方会填入音色URI。");
            listing.Label("  4. 即可使用该音色进行语音合成。");
            GUI.color = Color.white;
            
            listing.End();
        }

        public async Task<string?> UploadVoiceAsync(string filePath, TTSProviderConfig config)
        {
            if (string.IsNullOrEmpty(config.ApiKey))
            {
                Log.Error("[IndexTTSProvider] API 密钥为空。");
                return null;
            }
            
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    Log.Error($"[IndexTTSProvider] 文件路径无效或文件不存在: {filePath}");
                    return null;
                }

                byte[] fileBytes = File.ReadAllBytes(filePath);
                string fileName = Path.GetFileName(filePath);
                
                // 处理 customName 以符合 API 要求 (仅字母、数字、_、-)
                string customName = Path.GetFileNameWithoutExtension(filePath);
                // 替换非法字符为 _
                customName = System.Text.RegularExpressions.Regex.Replace(customName, @"[^a-zA-Z0-9_\-]", "_");
                // 移除连续的下划线
                customName = System.Text.RegularExpressions.Regex.Replace(customName, @"_+", "_");
                // 移除开头和结尾的下划线/连字符
                customName = customName.Trim('_', '-');
                // 确保以字母开头（如果不是，添加前缀）
                if (string.IsNullOrEmpty(customName) || !char.IsLetter(customName[0]))
                {
                    customName = "voice" + (string.IsNullOrEmpty(customName) ? "" : "_" + customName);
                }
                // 截断到 50 个字符以留出空间添加随机后缀
                if (customName.Length > 50)
                {
                    customName = customName.Substring(0, 50);
                }
                // 再次去除末尾可能的下划线/连字符
                customName = customName.TrimEnd('_', '-');
                // 添加随机后缀以避免重名问题
                customName += $"_{DateTime.Now.Ticks % 10000}";

                if (Prefs.DevMode)
                {
                    Log.Message($"[IndexTTSProvider] Using custom_name: {customName}");
                }

                using var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(fileBytes), "file", fileName);
                content.Add(new StringContent(customName), "custom_name");

                string endpoint = "https://api.siliconflow.cn/v1/uploads/audio/voice";

                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Headers.Add("Authorization", $"Bearer {config.ApiKey}");
                request.Content = content;

                if (Prefs.DevMode)
                {
                    Log.Message($"[IndexTTSProvider] Uploading voice from: {filePath}");
                    Log.Message($"[IndexTTSProvider] Upload Endpoint: {endpoint}");
                }

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<UploadResult>(jsonResponse);
                    if (result != null && !string.IsNullOrEmpty(result.uri))
                    {
                        config.IndexSpeakerId = result.uri;
                        Log.Message($"[IndexTTSProvider] Voice cloned successfully. URI: {result.uri}");
                        return result.uri;
                    }
                    else
                    {
                        Log.Error($"[IndexTTSProvider] 解析上传结果失败: {jsonResponse}");
                        return null;
                    }
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Log.Error($"[IndexTTSProvider] Voice upload failed: {response.StatusCode}");
                    Log.Error($"[IndexTTSProvider] Details: {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[IndexTTSProvider] Upload exception: {ex.Message}");
                return null;
            }
        }

        // 用于解析上传结果的内部类
        private class UploadResult
        {
            public string uri { get; set; } = "";
        }
    }
}
