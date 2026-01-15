using System.Collections.Generic;
using Verse;
using VoiceTuner.Models;

namespace VoiceTuner
{
    /// <summary>
    /// VoiceTuner Mod 设置
    /// </summary>
    public class VoiceTunerSettings : ModSettings
    {
        // 快捷键设置
        public bool EnableHotkey = true;
        public string HotkeyModifier = "Ctrl";
        public string HotkeyKey = "T";
        
        // 默认测试文本
        public string DefaultTestText = "你好，我是AI叙事者，很高兴认识你。";
        
        // 音频设置
        public float MasterVolume = 1.0f;
        public bool AutoPlayTest = true;
        
        // 导出设置
        public bool BackupBeforeExport = true;
        
        // 人格TTS配置列表
        public List<PersonaTTSConfig> PersonaConfigs = new List<PersonaTTSConfig>();
        
        // 当前选中的人格索引
        public int SelectedPersonaIndex = 0;
        
        /// <summary>
        /// 保存/加载设置
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            
            Scribe_Values.Look(ref EnableHotkey, "enableHotkey", true);
            Scribe_Values.Look(ref HotkeyModifier, "hotkeyModifier", "Ctrl");
            Scribe_Values.Look(ref HotkeyKey, "hotkeyKey", "T");
            
            Scribe_Values.Look(ref DefaultTestText, "defaultTestText", "你好，我是AI叙事者，很高兴认识你。");
            
            Scribe_Values.Look(ref MasterVolume, "masterVolume", 1.0f);
            Scribe_Values.Look(ref AutoPlayTest, "autoPlayTest", true);
            
            Scribe_Values.Look(ref BackupBeforeExport, "backupBeforeExport", true);
            
            Scribe_Collections.Look(ref PersonaConfigs, "personaConfigs", LookMode.Deep);
            
            Scribe_Values.Look(ref SelectedPersonaIndex, "selectedPersonaIndex", 0);
            
            if (PersonaConfigs == null)
            {
                PersonaConfigs = new List<PersonaTTSConfig>();
            }
        }
        
        /// <summary>
        /// 获取或创建指定人格的配置
        /// </summary>
        public PersonaTTSConfig GetOrCreateConfig(string defName, string displayName)
        {
            var existing = PersonaConfigs.Find(c => c.PersonaDefName == defName);
            if (existing != null)
            {
                return existing;
            }
            
            var newConfig = PersonaTTSConfig.CreateDefault(defName, displayName);
            PersonaConfigs.Add(newConfig);
            return newConfig;
        }
        
        /// <summary>
        /// 保存人格配置
        /// </summary>
        public void SaveConfig(PersonaTTSConfig config)
        {
            int index = PersonaConfigs.FindIndex(c => c.PersonaDefName == config.PersonaDefName);
            if (index >= 0)
            {
                PersonaConfigs[index] = config;
            }
            else
            {
                PersonaConfigs.Add(config);
            }
            
            // 触发保存
            LoadedModManager.GetMod<VoiceTunerMod>()?.WriteSettings();
        }
    }
}