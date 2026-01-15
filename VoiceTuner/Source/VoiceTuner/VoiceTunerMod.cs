using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;
using VoiceTuner.Models;
using VoiceTuner.Providers;
using VoiceTuner.UI;

namespace VoiceTuner
{
    /// <summary>
    /// VoiceTuner Mod 主类
    /// </summary>
    public class VoiceTunerMod : Mod
    {
        public static VoiceTunerMod? Instance { get; private set; }
        public static VoiceTunerSettings Settings => Instance?.GetSettings<VoiceTunerSettings>() ?? new VoiceTunerSettings();
        
        private readonly Harmony harmony;
        
        public VoiceTunerMod(ModContentPack content) : base(content)
        {
            Instance = this;
            
            // 初始化Harmony
            harmony = new Harmony("VoiceTuner.Core");
            harmony.PatchAll();
            
            // 初始化提供商管理器
            TTSProviderManager.Initialize();
            
            // 获取设置
            GetSettings<VoiceTunerSettings>();
            
            Log.Message("[VoiceTuner] Mod initialized");
        }
        
        /// <summary>
        /// 设置窗口标题
        /// </summary>
        public override string SettingsCategory()
        {
            return "Voice Tuner";
        }
        
        /// <summary>
        /// 绘制设置界面
        /// </summary>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            var settings = GetSettings<VoiceTunerSettings>();
            
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            
            // 标题
            Text.Font = GameFont.Medium;
            listing.Label("🎤 Voice Tuner - TTS语音调节器");
            Text.Font = GameFont.Small;
            listing.Gap(12f);
            
            // 打开调节器窗口按钮
            if (listing.ButtonText("打开语音调节器窗口"))
            {
                Find.WindowStack.Add(new Window_VoiceTuner());
            }
            listing.Gap(6f);
            
            // 快捷键设置
            listing.Label("快捷键设置:");
            listing.Gap(6f);
            
            listing.CheckboxLabeled("启用快捷键", ref settings.EnableHotkey, "按下快捷键打开语音调节器");
            
            if (settings.EnableHotkey)
            {
                listing.Label($"当前快捷键: {settings.HotkeyModifier}+{settings.HotkeyKey}");
                
                if (listing.ButtonText("更改快捷键"))
                {
                    ShowHotkeySelectionMenu(settings);
                }
            }
            listing.Gap(12f);
            
            // 默认测试文本
            listing.Label("默认测试文本:");
            settings.DefaultTestText = listing.TextEntry(settings.DefaultTestText);
            listing.Gap(12f);
            
            // 音频设置
            listing.Label("音频设置:");
            listing.Gap(6f);
            
            listing.Label($"主音量: {(int)(settings.MasterVolume * 100)}%");
            settings.MasterVolume = listing.Slider(settings.MasterVolume, 0f, 1f);
            
            listing.CheckboxLabeled("自动播放测试音频", ref settings.AutoPlayTest, "合成完成后自动播放");
            listing.Gap(12f);
            
            // 导出设置
            listing.Label("导出设置:");
            listing.Gap(6f);
            
            listing.CheckboxLabeled("导出前备份原文件", ref settings.BackupBeforeExport, "修改mod文件前创建备份");
            
            listing.End();
        }
        
        /// <summary>
        /// 显示快捷键选择菜单
        /// </summary>
        private void ShowHotkeySelectionMenu(VoiceTunerSettings settings)
        {
            var modifierOptions = new List<FloatMenuOption>
            {
                new FloatMenuOption("Ctrl", () => {
                    settings.HotkeyModifier = "Ctrl";
                    ShowKeySelectionMenu(settings);
                }),
                new FloatMenuOption("Alt", () => {
                    settings.HotkeyModifier = "Alt";
                    ShowKeySelectionMenu(settings);
                }),
                new FloatMenuOption("Shift", () => {
                    settings.HotkeyModifier = "Shift";
                    ShowKeySelectionMenu(settings);
                })
            };
            
            Find.WindowStack.Add(new FloatMenu(modifierOptions));
        }
        
        /// <summary>
        /// 显示按键选择菜单
        /// </summary>
        private void ShowKeySelectionMenu(VoiceTunerSettings settings)
        {
            var keyOptions = new List<FloatMenuOption>();
            
            // 字母键
            for (char c = 'A'; c <= 'Z'; c++)
            {
                string key = c.ToString();
                keyOptions.Add(new FloatMenuOption(key, () => settings.HotkeyKey = key));
            }
            
            // 功能键
            for (int i = 1; i <= 12; i++)
            {
                string key = $"F{i}";
                keyOptions.Add(new FloatMenuOption(key, () => settings.HotkeyKey = key));
            }
            
            Find.WindowStack.Add(new FloatMenu(keyOptions));
        }
    }
    
    /// <summary>
    /// 快捷键检测补丁
    /// </summary>
    [HarmonyPatch(typeof(UIRoot_Play), "UIRootOnGUI")]
    public static class HotkeyPatch
    {
        public static void Postfix()
        {
            if (VoiceTunerMod.Instance == null) return;
            
            var settings = VoiceTunerMod.Settings;
            if (!settings.EnableHotkey) return;
            
            // 检测快捷键
            bool modifierPressed = settings.HotkeyModifier switch
            {
                "Ctrl" => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl),
                "Alt" => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt),
                "Shift" => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift),
                _ => false
            };
            
            if (!modifierPressed) return;
            
            // 解析按键
            KeyCode keyCode = KeyCode.None;
            if (settings.HotkeyKey.Length == 1)
            {
                keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), settings.HotkeyKey);
            }
            else if (settings.HotkeyKey.StartsWith("F"))
            {
                keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), settings.HotkeyKey);
            }
            
            if (keyCode != KeyCode.None && Input.GetKeyDown(keyCode))
            {
                // 检查是否已有窗口打开
                var existingWindow = Find.WindowStack.WindowOfType<Window_VoiceTuner>();
                if (existingWindow != null)
                {
                    existingWindow.Close();
                }
                else
                {
                    Find.WindowStack.Add(new Window_VoiceTuner());
                }
            }
        }
    }
}