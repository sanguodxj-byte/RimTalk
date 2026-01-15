namespace VoiceTuner.Models
{
    /// <summary>
    /// TTS提供商类型枚举
    /// </summary>
    public enum TTSProviderType
    {
        /// <summary>Azure TTS（微软认知服务）</summary>
        Azure,
        
        /// <summary>Fish Audio</summary>
        FishAudio,
        
        /// <summary>IndexTTS-2（硅基流动音色克隆）</summary>
        IndexTTS
    }
}