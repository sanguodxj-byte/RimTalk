using System.Collections.Generic;
using VoiceTuner.Models;

namespace VoiceTuner.Providers
{
    /// <summary>
    /// TTS提供商管理器 - 管理所有TTS提供商实例
    /// </summary>
    public static class TTSProviderManager
    {
        private static readonly Dictionary<TTSProviderType, ITTSProvider> providers = new Dictionary<TTSProviderType, ITTSProvider>();
        
        /// <summary>
        /// 初始化所有提供商
        /// </summary>
        public static void Initialize()
        {
            providers.Clear();
            providers[TTSProviderType.Azure] = new AzureTTSProvider();
            providers[TTSProviderType.FishAudio] = new FishAudioProvider();
            providers[TTSProviderType.IndexTTS] = new IndexTTSProvider();
        }
        
        /// <summary>
        /// 获取指定类型的提供商
        /// </summary>
        public static ITTSProvider? GetProvider(TTSProviderType type)
        {
            if (providers.Count == 0)
            {
                Initialize();
            }
            
            return providers.TryGetValue(type, out var provider) ? provider : null;
        }
        
        /// <summary>
        /// 获取所有提供商
        /// </summary>
        public static IEnumerable<ITTSProvider> GetAllProviders()
        {
            if (providers.Count == 0)
            {
                Initialize();
            }
            
            return providers.Values;
        }
        
        /// <summary>
        /// 获取提供商类型列表
        /// </summary>
        public static List<(TTSProviderType Type, string DisplayName)> GetProviderTypes()
        {
            return new List<(TTSProviderType, string)>
            {
                (TTSProviderType.Azure, "Azure TTS（微软认知服务）"),
                (TTSProviderType.FishAudio, "Fish Audio"),
                (TTSProviderType.IndexTTS, "IndexTTS-2 (语音克隆)")
            };
        }
    }
}