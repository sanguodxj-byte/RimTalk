using System;
using System.Collections.Generic;
using Verse;

namespace VoiceTuner.Models
{
    /// <summary>
    /// TTS提供商配置 - 存储各TTS后端的配置参数
    /// </summary>
    public class TTSProviderConfig : IExposable
    {
        // ============================================
        // 通用配置
        // ============================================
        
        /// <summary>TTS提供商类型</summary>
        public TTSProviderType ProviderType = TTSProviderType.Azure;
        
        /// <summary>API端点URL</summary>
        public string ApiEndpoint = "";
        
        /// <summary>API密钥</summary>
        public string ApiKey = "";
        
        /// <summary>语音ID/名称</summary>
        public string VoiceId = "zh-CN-XiaoxiaoNeural";
        
        /// <summary>语速（0.5-2.0）</summary>
        public float Speed = 1.0f;
        
        /// <summary>音调（0.5-2.0）</summary>
        public float Pitch = 1.0f;
        
        /// <summary>音量（0.0-1.0）</summary>
        public float Volume = 1.0f;
        
        // ============================================
        // Azure TTS 专用配置
        // ============================================
        
        /// <summary>Azure区域（如eastus, westeurope）</summary>
        public string AzureRegion = "eastus";
        
        /// <summary>Azure情感风格（如cheerful, sad, angry）</summary>
        public string AzureStyle = "";
        
        /// <summary>Azure情感强度（0.01-2.0）</summary>
        public float AzureStyleDegree = 1.0f;
        
        /// <summary>Azure角色扮演（如Boy, Girl, OlderAdultMale）</summary>
        public string AzureRole = "";
        
        // ============================================
        // Fish Audio 专用配置
        // ============================================
        
        /// <summary>Fish Audio参考音色ID</summary>
        public string FishReferenceId = "";
        
        /// <summary>Fish Audio延迟模式（normal/balanced）</summary>
        public string FishLatency = "normal";
        
        /// <summary>Fish Audio输出格式（wav/mp3/opus）</summary>
        public string FishFormat = "wav";
        
        // ============================================
        // IndexTTS 专用配置
        // ============================================
        
        /// <summary>IndexTTS说话人ID</summary>
        public string IndexSpeakerId = "";
        
        /// <summary>IndexTTS输出格式</summary>
        public string IndexFormat = "wav";

        /// <summary>IndexTTS音色克隆URL</summary>
        public string IndexCloneUrl = "";
        
        /// <summary>IndexTTS-2 情感</summary>
        public string IndexEmotion = "";
        
        /// <summary>
        /// 获取默认配置
        /// </summary>
        public static TTSProviderConfig CreateDefault(TTSProviderType type)
        {
            var config = new TTSProviderConfig { ProviderType = type };
            
            switch (type)
            {
                case TTSProviderType.Azure:
                    config.ApiEndpoint = "https://eastus.tts.speech.microsoft.com/cognitiveservices/v1";
                    config.VoiceId = "zh-CN-XiaoxiaoNeural";
                    break;
                    
                case TTSProviderType.FishAudio:
                    config.ApiEndpoint = "https://api.fish.audio/v1/tts";
                    break;
                    
                case TTSProviderType.IndexTTS:
                    config.ApiEndpoint = "https://api.siliconflow.cn/v1/uploads/audio/voice"; // 上传端点
                    break;
            }
            
            return config;
        }
        
        /// <summary>
        /// 克隆配置
        /// </summary>
        public TTSProviderConfig Clone()
        {
            return new TTSProviderConfig
            {
                ProviderType = this.ProviderType,
                ApiEndpoint = this.ApiEndpoint,
                ApiKey = this.ApiKey,
                VoiceId = this.VoiceId,
                Speed = this.Speed,
                Pitch = this.Pitch,
                Volume = this.Volume,
                AzureRegion = this.AzureRegion,
                AzureStyle = this.AzureStyle,
                AzureStyleDegree = this.AzureStyleDegree,
                AzureRole = this.AzureRole,
                FishReferenceId = this.FishReferenceId,
                FishLatency = this.FishLatency,
                FishFormat = this.FishFormat,
                IndexSpeakerId = this.IndexSpeakerId,
                IndexFormat = this.IndexFormat,
                IndexCloneUrl = this.IndexCloneUrl,
                IndexEmotion = this.IndexEmotion
            };
        }
        
        /// <summary>
        /// 保存/加载配置
        /// </summary>
        public void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                Scribe_Values.Look(ref ProviderType, "providerType", TTSProviderType.Azure);
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                string providerTypeStr = "";
                Scribe_Values.Look(ref providerTypeStr, "providerType", "Azure");
                if (Enum.TryParse<TTSProviderType>(providerTypeStr, out var parsedType))
                {
                    ProviderType = parsedType;
                }
                else if (providerTypeStr.Equals("CosyVoice2", StringComparison.OrdinalIgnoreCase) || providerTypeStr.Equals("SiliconFlow", StringComparison.OrdinalIgnoreCase))
                {
                    // 向后兼容，将旧的CosyVoice2或SiliconFlow映射到IndexTTS
                    ProviderType = TTSProviderType.IndexTTS;
                }
                else
                {
                    // 未知类型，使用默认值
                    ProviderType = TTSProviderType.Azure;
                }
            }
            
            Scribe_Values.Look(ref ApiEndpoint, "apiEndpoint", "");
            Scribe_Values.Look(ref ApiKey, "apiKey", "");
            Scribe_Values.Look(ref VoiceId, "voiceId", "zh-CN-XiaoxiaoNeural");
            Scribe_Values.Look(ref Speed, "speed", 1.0f);
            Scribe_Values.Look(ref Pitch, "pitch", 1.0f);
            Scribe_Values.Look(ref Volume, "volume", 1.0f);
            
            // Azure
            Scribe_Values.Look(ref AzureRegion, "azureRegion", "eastus");
            Scribe_Values.Look(ref AzureStyle, "azureStyle", "");
            Scribe_Values.Look(ref AzureStyleDegree, "azureStyleDegree", 1.0f);
            Scribe_Values.Look(ref AzureRole, "azureRole", "");
            
            // Fish Audio
            Scribe_Values.Look(ref FishReferenceId, "fishReferenceId", "");
            Scribe_Values.Look(ref FishLatency, "fishLatency", "normal");
            Scribe_Values.Look(ref FishFormat, "fishFormat", "wav");
            
            // IndexTTS
            Scribe_Values.Look(ref IndexSpeakerId, "indexSpeakerId", "");
            Scribe_Values.Look(ref IndexFormat, "indexFormat", "wav");
            Scribe_Values.Look(ref IndexCloneUrl, "indexCloneUrl", "");
            Scribe_Values.Look(ref IndexEmotion, "indexEmotion", "");
        }
        
        /// <summary>
        /// 获取提供商显示名称
        /// </summary>
        public string GetProviderDisplayName()
        {
            return ProviderType switch
            {
                TTSProviderType.Azure => "Azure TTS",
                TTSProviderType.FishAudio => "Fish Audio",
                TTSProviderType.IndexTTS => "IndexTTS-2 (语音克隆)",
                _ => "Unknown"
            };
        }
    }
    
    /// <summary>
    /// Azure TTS 可用的情感风格列表
    /// </summary>
    public static class AzureTTSStyles
    {
        public static readonly List<string> Styles = new List<string>
        {
            "",  // 无情感
            "affectionate",
            "angry",
            "assistant",
            "calm",
            "chat",
            "cheerful",
            "customerservice",
            "depressed",
            "disgruntled",
            "documentary-narration",
            "embarrassed",
            "empathetic",
            "envious",
            "excited",
            "fearful",
            "friendly",
            "gentle",
            "hopeful",
            "lyrical",
            "narration-professional",
            "narration-relaxed",
            "newscast",
            "newscast-casual",
            "newscast-formal",
            "poetry-reading",
            "sad",
            "serious",
            "shouting",
            "sports_commentary",
            "sports_commentary_excited",
            "whispering",
            "terrified",
            "unfriendly"
        };
        
        public static readonly List<string> Roles = new List<string>
        {
            "",  // 无角色
            "Boy",
            "Girl",
            "OlderAdultFemale",
            "OlderAdultMale",
            "SeniorFemale",
            "SeniorMale",
            "YoungAdultFemale",
            "YoungAdultMale"
        };
    }
    
    /// <summary>
    /// Azure TTS 可用的语音列表
    /// </summary>
    public static class AzureTTSVoices
    {
        public static readonly List<(string Id, string DisplayName, string Language)> Voices = new List<(string, string, string)>
        {
            // 中文（普通话）
            ("zh-CN-XiaoxiaoNeural", "晓晓（女声，通用）", "zh-CN"),
            ("zh-CN-XiaoyiNeural", "晓伊（女声，儿童）", "zh-CN"),
            ("zh-CN-YunjianNeural", "云健（男声，体育）", "zh-CN"),
            ("zh-CN-YunxiNeural", "云希（男声，通用）", "zh-CN"),
            ("zh-CN-YunxiaNeural", "云夏（男声，儿童）", "zh-CN"),
            ("zh-CN-YunyangNeural", "云扬（男声，新闻）", "zh-CN"),
            ("zh-CN-XiaochenNeural", "晓辰（女声，客服）", "zh-CN"),
            ("zh-CN-XiaohanNeural", "晓涵（女声，自然）", "zh-CN"),
            ("zh-CN-XiaomengNeural", "晓梦（女声，活泼）", "zh-CN"),
            ("zh-CN-XiaomoNeural", "晓墨（女声，清晰）", "zh-CN"),
            ("zh-CN-XiaoqiuNeural", "晓秋（女声，温和）", "zh-CN"),
            ("zh-CN-XiaoruiNeural", "晓睿（女声，自信）", "zh-CN"),
            ("zh-CN-XiaoshuangNeural", "晓双（女声，可爱）", "zh-CN"),
            ("zh-CN-XiaoxuanNeural", "晓萱（女声，优雅）", "zh-CN"),
            ("zh-CN-XiaoyanNeural", "晓颜（女声，柔和）", "zh-CN"),
            ("zh-CN-XiaoyouNeural", "晓悠（女声，活泼）", "zh-CN"),
            ("zh-CN-YunfengNeural", "云枫（男声，沉稳）", "zh-CN"),
            ("zh-CN-YunhaoNeural", "云皓（男声，自然）", "zh-CN"),
            ("zh-CN-YunjieNeural", "云杰（男声，阳光）", "zh-CN"),
            ("zh-CN-YunzeNeural", "云泽（男声，成熟）", "zh-CN"),
            
            // 粤语
            ("zh-HK-HiuMaanNeural", "曉曼（女声，香港）", "zh-HK"),
            ("zh-HK-HiuGaaiNeural", "曉佳（女声，温柔）", "zh-HK"),
            ("zh-HK-WanLungNeural", "雲龍（男声，香港）", "zh-HK"),
            
            // 台湾国语
            ("zh-TW-HsiaoChenNeural", "曉臻（女声，台湾）", "zh-TW"),
            ("zh-TW-HsiaoYuNeural", "曉雨（女声，温柔）", "zh-TW"),
            ("zh-TW-YunJheNeural", "雲哲（男声，台湾）", "zh-TW"),
            
            // 英语
            ("en-US-JennyNeural", "Jenny（女声，美式）", "en-US"),
            ("en-US-GuyNeural", "Guy（男声，美式）", "en-US"),
            ("en-US-AriaNeural", "Aria（女声，新闻）", "en-US"),
            ("en-GB-SoniaNeural", "Sonia（女声，英式）", "en-GB"),
            ("en-GB-RyanNeural", "Ryan（男声，英式）", "en-GB"),
            
            // 日语
            ("ja-JP-NanamiNeural", "七海（女声，日本）", "ja-JP"),
            ("ja-JP-KeitaNeural", "圭太（男声，日本）", "ja-JP"),
            ("ja-JP-AoiNeural", "葵（女声，可爱）", "ja-JP"),
            ("ja-JP-MayuNeural", "真由（女声，温柔）", "ja-JP"),
            
            // 韩语
            ("ko-KR-SunHiNeural", "선희（女声，韩国）", "ko-KR"),
            ("ko-KR-InJoonNeural", "인준（男声，韩国）", "ko-KR")
        };
    }
}