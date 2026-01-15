using System;
using Verse;

namespace VoiceTuner.Models
{
    /// <summary>
    /// 人格TTS配置 - 关联人格与TTS设置
    /// </summary>
    public class PersonaTTSConfig : IExposable
    {
        /// <summary>关联的人格DefName</summary>
        public string PersonaDefName = "";
        
        /// <summary>人格显示名称</summary>
        public string PersonaDisplayName = "";
        
        /// <summary>TTS提供商配置</summary>
        public TTSProviderConfig ProviderConfig = new TTSProviderConfig();
        
        /// <summary>预设名称（用于保存/加载）</summary>
        public string PresetName = "";
        
        /// <summary>上次测试时间</summary>
        public DateTime LastTestTime = DateTime.MinValue;
        
        /// <summary>是否已配置</summary>
        public bool IsConfigured => !string.IsNullOrEmpty(ProviderConfig.ApiKey) ||
                                    ProviderConfig.ProviderType == TTSProviderType.IndexTTS;
        
        /// <summary>
        /// 创建默认配置
        /// </summary>
        public static PersonaTTSConfig CreateDefault(string defName, string displayName)
        {
            return new PersonaTTSConfig
            {
                PersonaDefName = defName,
                PersonaDisplayName = displayName,
                ProviderConfig = TTSProviderConfig.CreateDefault(TTSProviderType.Azure),
                PresetName = $"{defName}_Default"
            };
        }
        
        /// <summary>
        /// 克隆配置
        /// </summary>
        public PersonaTTSConfig Clone()
        {
            return new PersonaTTSConfig
            {
                PersonaDefName = this.PersonaDefName,
                PersonaDisplayName = this.PersonaDisplayName,
                ProviderConfig = this.ProviderConfig.Clone(),
                PresetName = this.PresetName,
                LastTestTime = this.LastTestTime
            };
        }
        
        /// <summary>
        /// 保存/加载配置
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.Look(ref PersonaDefName, "personaDefName", "");
            Scribe_Values.Look(ref PersonaDisplayName, "personaDisplayName", "");
            Scribe_Values.Look(ref PresetName, "presetName", "");
            
            Scribe_Deep.Look(ref ProviderConfig, "providerConfig");
            
            if (ProviderConfig == null)
            {
                ProviderConfig = new TTSProviderConfig();
            }
        }
        
        /// <summary>
        /// 获取配置摘要
        /// </summary>
        public string GetSummary()
        {
            return $"{ProviderConfig.GetProviderDisplayName()} - {ProviderConfig.VoiceId}";
        }
    }
}