using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using Verse;
using RimWorld;
using VoiceTuner.Models;
using VoiceTuner.Providers;
using VoiceTuner.Services;

namespace VoiceTuner.UI
{
    /// <summary>
    /// VoiceTuner 主窗口
    /// </summary>
    public class Window_VoiceTuner : Window
    {
        // 窗口尺寸
        public override Vector2 InitialSize => new Vector2(900f, 650f);
        
        // 人格列表
        private List<PersonaInfo> personas = new List<PersonaInfo>();
        private int selectedPersonaIndex = 0;
        
        // 当前编辑的配置
        private PersonaTTSConfig? currentConfig;
        
        // 测试文本
        private string testText = "";
        
        // UI状态
        private Vector2 personaListScrollPos;
        private Vector2 configScrollPos;
        private bool isSynthesizing = false;
        private string statusMessage = "";
        private float statusMessageTime = 0f;
        
        // 音频播放
        private AudioSource? audioSource;
        private byte[]? lastAudioData;
        
        public Window_VoiceTuner()
        {
            doCloseButton = true;
            doCloseX = true;
            absorbInputAroundWindow = true;
            forcePause = false;
            
            // 加载设置
            testText = VoiceTunerMod.Settings.DefaultTestText;
            
            // 加载人格列表
            LoadPersonas();
            
            // 选择第一个人格
            if (personas.Count > 0)
            {
                SelectPersona(0);
            }
        }
        
        /// <summary>
        /// 加载可用的人格列表
        /// </summary>
        private void LoadPersonas()
        {
            personas.Clear();
            
            // 尝试从The Second Seat加载人格
            try
            {
                // 使用反射检查是否存在NarratorPersonaDef
                var defDatabaseType = typeof(DefDatabase<>);
                var narratorPersonaDefType = GenTypes.GetTypeInAnyAssembly("TheSecondSeat.PersonaGeneration.NarratorPersonaDef");
                
                if (narratorPersonaDefType != null)
                {
                    var genericDefDatabase = defDatabaseType.MakeGenericType(narratorPersonaDefType);
                    var allDefsProperty = genericDefDatabase.GetProperty("AllDefsListForReading");
                    
                    if (allDefsProperty != null)
                    {
                        var allDefs = allDefsProperty.GetValue(null) as System.Collections.IList;
                        if (allDefs != null)
                        {
                            foreach (var def in allDefs)
                            {
                                var defNameField = narratorPersonaDefType.GetField("defName");
                                var narratorNameField = narratorPersonaDefType.GetField("narratorName");
                                
                                string defName = defNameField?.GetValue(def)?.ToString() ?? "Unknown";
                                string displayName = narratorNameField?.GetValue(def)?.ToString() ?? defName;
                                
                                personas.Add(new PersonaInfo
                                {
                                    DefName = defName,
                                    DisplayName = displayName,
                                    Source = "The Second Seat"
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"[VoiceTuner] Failed to load personas from The Second Seat: {ex.Message}");
            }
            
            // 如果没有找到任何人格，添加默认示例
            if (personas.Count == 0)
            {
                personas.Add(new PersonaInfo
                {
                    DefName = "Sideria_Default",
                    DisplayName = "Sideria",
                    Source = "示例"
                });
                
                personas.Add(new PersonaInfo
                {
                    DefName = "Custom_Persona",
                    DisplayName = "自定义人格",
                    Source = "用户"
                });
            }
        }
        
        /// <summary>
        /// 选择人格
        /// </summary>
        private void SelectPersona(int index)
        {
            if (index < 0 || index >= personas.Count) return;
            
            selectedPersonaIndex = index;
            var persona = personas[index];
            
            // 获取或创建配置
            currentConfig = VoiceTunerMod.Settings.GetOrCreateConfig(persona.DefName, persona.DisplayName);
        }
        
        /// <summary>
        /// 绘制窗口内容
        /// </summary>
        public override void DoWindowContents(Rect inRect)
        {
            // 标题
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0, 0, inRect.width, 30f), "🎤 Voice Tuner - TTS语音调节器");
            Text.Font = GameFont.Small;
            
            // 主内容区域
            Rect mainRect = new Rect(0, 40f, inRect.width, inRect.height - 120f);
            
            // 左侧：人格列表
            Rect leftRect = new Rect(mainRect.x, mainRect.y, 200f, mainRect.height);
            DrawPersonaList(leftRect);
            
            // 右侧：配置面板
            Rect rightRect = new Rect(leftRect.xMax + 10f, mainRect.y, mainRect.width - leftRect.width - 10f, mainRect.height);
            DrawConfigPanel(rightRect);
            
            // 底部：测试和操作按钮
            Rect bottomRect = new Rect(0, inRect.height - 70f, inRect.width, 60f);
            DrawBottomPanel(bottomRect);
            
            // 状态消息
            if (!string.IsNullOrEmpty(statusMessage) && Time.time - statusMessageTime < 5f)
            {
                Rect statusRect = new Rect(0, inRect.height - 25f, inRect.width, 20f);
                GUI.color = new Color(0.7f, 0.9f, 0.7f);
                Widgets.Label(statusRect, statusMessage);
                GUI.color = Color.white;
            }
        }
        
        /// <summary>
        /// 绘制人格列表
        /// </summary>
        private void DrawPersonaList(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            
            Rect headerRect = new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, 25f);
            Widgets.Label(headerRect, "人格列表");
            
            Rect listRect = new Rect(rect.x + 5f, rect.y + 35f, rect.width - 10f, rect.height - 45f);
            Rect viewRect = new Rect(0, 0, listRect.width - 16f, personas.Count * 35f);
            
            Widgets.BeginScrollView(listRect, ref personaListScrollPos, viewRect);
            
            for (int i = 0; i < personas.Count; i++)
            {
                Rect itemRect = new Rect(0, i * 35f, viewRect.width, 32f);
                
                bool isSelected = i == selectedPersonaIndex;
                if (isSelected)
                {
                    Widgets.DrawHighlightSelected(itemRect);
                }
                else if (Mouse.IsOver(itemRect))
                {
                    Widgets.DrawHighlight(itemRect);
                }
                
                // 人格名称
                Widgets.Label(new Rect(itemRect.x + 5f, itemRect.y + 2f, itemRect.width - 10f, 20f), personas[i].DisplayName);
                
                // 来源标签
                GUI.color = Color.gray;
                Text.Font = GameFont.Tiny;
                Widgets.Label(new Rect(itemRect.x + 5f, itemRect.y + 16f, itemRect.width - 10f, 14f), personas[i].Source);
                Text.Font = GameFont.Small;
                GUI.color = Color.white;
                
                if (Widgets.ButtonInvisible(itemRect))
                {
                    SelectPersona(i);
                }
            }
            
            Widgets.EndScrollView();
        }
        
        /// <summary>
        /// 绘制配置面板
        /// </summary>
        private void DrawConfigPanel(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            
            if (currentConfig == null)
            {
                Widgets.Label(rect, "请选择一个人格");
                return;
            }
            
            Rect contentRect = new Rect(rect.x + 10f, rect.y + 10f, rect.width - 20f, rect.height - 20f);
            Rect viewRect = new Rect(0, 0, contentRect.width - 16f, 600f);
            
            Widgets.BeginScrollView(contentRect, ref configScrollPos, viewRect);
            
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(new Rect(0, 0, viewRect.width, viewRect.height));
            
            // 提供商选择
            listing.Label("TTS 提供商:");
            string providerDisplay = currentConfig.ProviderConfig.GetProviderDisplayName();
            if (listing.ButtonText(providerDisplay))
            {
                ShowProviderSelectionMenu();
            }
            listing.Gap(12f);
            
            // 分隔线
            listing.GapLine(12f);
            
            // 根据提供商类型绘制配置面板
            var provider = TTSProviderManager.GetProvider(currentConfig.ProviderConfig.ProviderType);
            if (provider != null)
            {
                Rect providerRect = listing.GetRect(400f);
                provider.DrawConfigPanel(providerRect, currentConfig.ProviderConfig);
            }
            
            listing.End();
            Widgets.EndScrollView();
        }
        
        /// <summary>
        /// 绘制底部面板
        /// </summary>
        private void DrawBottomPanel(Rect rect)
        {
            // 测试文本输入
            Rect textLabelRect = new Rect(rect.x, rect.y, 80f, 25f);
            Widgets.Label(textLabelRect, "测试文本:");
            
            Rect textInputRect = new Rect(textLabelRect.xMax + 5f, rect.y, rect.width - textLabelRect.width - 400f, 25f);
            testText = Widgets.TextField(textInputRect, testText);
            
            // 按钮区域
            float buttonY = rect.y;
            float buttonX = rect.xMax - 380f;
            float buttonWidth = 90f;
            float buttonGap = 5f;
            
            // 播放测试按钮
            Rect playRect = new Rect(buttonX, buttonY, buttonWidth, 25f);
            GUI.enabled = !isSynthesizing && currentConfig != null;
            if (Widgets.ButtonText(playRect, isSynthesizing ? "合成中..." : "🔊 播放测试"))
            {
                _ = SynthesizeAndPlayAsync();
            }
            GUI.enabled = true;
            
            // 保存配置按钮
            Rect saveRect = new Rect(playRect.xMax + buttonGap, buttonY, buttonWidth, 25f);
            if (Widgets.ButtonText(saveRect, "💾 保存配置"))
            {
                SaveCurrentConfig();
            }
            
            // 导出XML按钮
            Rect exportRect = new Rect(saveRect.xMax + buttonGap, buttonY, buttonWidth, 25f);
            if (Widgets.ButtonText(exportRect, "📤 导出XML"))
            {
                ExportToXml();
            }
            
            // 注入Mod按钮
            Rect injectRect = new Rect(exportRect.xMax + buttonGap, buttonY, buttonWidth, 25f);
            if (Widgets.ButtonText(injectRect, "📥 注入Mod"))
            {
                InjectToMod();
            }
        }
        
        /// <summary>
        /// 显示提供商选择菜单
        /// </summary>
        private void ShowProviderSelectionMenu()
        {
            if (currentConfig == null) return;
            
            var options = new List<FloatMenuOption>();
            
            foreach (var providerType in TTSProviderManager.GetProviderTypes())
            {
                TTSProviderType type = providerType.Type;
                options.Add(new FloatMenuOption(providerType.DisplayName, () =>
                {
                    currentConfig.ProviderConfig.ProviderType = type;
                    
                    // 应用默认配置
                    var defaultConfig = TTSProviderConfig.CreateDefault(type);
                    currentConfig.ProviderConfig.ApiEndpoint = defaultConfig.ApiEndpoint;
                    
                    // 保留API密钥
                    // currentConfig.ProviderConfig.ApiKey 保持不变
                }));
            }
            
            Find.WindowStack.Add(new FloatMenu(options));
        }
        
        /// <summary>
        /// 合成并播放测试音频
        /// </summary>
        private async Task SynthesizeAndPlayAsync()
        {
            if (currentConfig == null || isSynthesizing) return;
            
            isSynthesizing = true;
            ShowStatus("正在合成语音...");
            
            try
            {
                var provider = TTSProviderManager.GetProvider(currentConfig.ProviderConfig.ProviderType);
                if (provider == null)
                {
                    ShowStatus("❌ 未找到TTS提供商");
                    return;
                }
                
                byte[]? audioData = await provider.SynthesizeAsync(testText, currentConfig.ProviderConfig);
                
                if (audioData == null || audioData.Length == 0)
                {
                    ShowStatus("❌ 语音合成失败");
                    return;
                }
                
                lastAudioData = audioData;
                ShowStatus($"✅ 合成成功 ({audioData.Length} 字节)");
                
                // 自动播放
                if (VoiceTunerMod.Settings.AutoPlayTest)
                {
                    PlayAudio(audioData);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"❌ 错误: {ex.Message}");
                Log.Error($"[VoiceTuner] Synthesis failed: {ex}");
            }
            finally
            {
                isSynthesizing = false;
            }
        }
        
        /// <summary>
        /// 播放音频
        /// </summary>
        private void PlayAudio(byte[] audioData)
        {
            try
            {
                // 保存到临时文件
                string tempPath = Path.Combine(Path.GetTempPath(), $"voicetuner_test_{DateTime.Now:yyyyMMddHHmmss}.wav");
                File.WriteAllBytes(tempPath, audioData);
                
                // 使用系统默认播放器播放
                System.Diagnostics.Process.Start(tempPath);
                
                ShowStatus("🔊 正在播放...");
            }
            catch (Exception ex)
            {
                ShowStatus($"❌ 播放失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 保存当前配置
        /// </summary>
        private void SaveCurrentConfig()
        {
            if (currentConfig == null) return;
            
            VoiceTunerMod.Settings.SaveConfig(currentConfig);
            ShowStatus("✅ 配置已保存");
        }
        
        /// <summary>
        /// 导出XML
        /// </summary>
        private void ExportToXml()
        {
            if (currentConfig == null) return;
            
            try
            {
                string xml = ConfigExportService.ExportToXml(currentConfig);
                
                // 保存到桌面
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"VoiceTuner_{currentConfig.PersonaDefName}_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
                string filePath = Path.Combine(desktopPath, fileName);
                
                File.WriteAllText(filePath, xml);
                
                ShowStatus($"✅ 已导出到桌面: {fileName}");
            }
            catch (Exception ex)
            {
                ShowStatus($"❌ 导出失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 注入到Mod
        /// </summary>
        private void InjectToMod()
        {
            if (currentConfig == null) return;
            
            // 查找Sideria mod路径
            string? sideriaPath = FindSideriaModPath();
            
            if (string.IsNullOrEmpty(sideriaPath))
            {
                ShowStatus("❌ 未找到 The Second Seat - Sideria mod");
                return;
            }
            
            try
            {
                ConfigExportService.InjectToMod(currentConfig, sideriaPath, VoiceTunerMod.Settings.BackupBeforeExport);
                ShowStatus("✅ 已注入到 Sideria mod");
            }
            catch (Exception ex)
            {
                ShowStatus($"❌ 注入失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 查找Sideria mod路径
        /// </summary>
        private string? FindSideriaModPath()
        {
            // 在当前工作目录的父目录中查找
            string currentDir = Directory.GetCurrentDirectory();
            string parentDir = Directory.GetParent(currentDir)?.FullName ?? "";
            
            if (string.IsNullOrEmpty(parentDir)) return null;
            
            // 查找 "The Second Seat - Sideria" 目录
            string[] possibleNames = new[]
            {
                "The Second Seat - Sideria",
                "TheSecondSeat-Sideria",
                "Sideria"
            };
            
            foreach (string name in possibleNames)
            {
                string path = Path.Combine(parentDir, name);
                if (Directory.Exists(path))
                {
                    return path;
                }
            }
            
            // 在RimWorld Mods目录中查找
            foreach (var modPack in LoadedModManager.RunningModsListForReading)
            {
                if (modPack.Name.Contains("Sideria") || modPack.PackageId.Contains("sideria"))
                {
                    return modPack.RootDir;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 显示状态消息
        /// </summary>
        private void ShowStatus(string message)
        {
            statusMessage = message;
            statusMessageTime = Time.time;
        }
    }
    
    /// <summary>
    /// 人格信息
    /// </summary>
    public class PersonaInfo
    {
        public string DefName = "";
        public string DisplayName = "";
        public string Source = "";
    }
}