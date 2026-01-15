using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using Verse;
using VoiceTuner.Models;

namespace VoiceTuner.Services
{
    /// <summary>
    /// 配置导出服务 - 导出XML和注入到Mod
    /// </summary>
    public static class ConfigExportService
    {
        /// <summary>
        /// 导出为独立XML配置文件
        /// </summary>
        public static string ExportToXml(PersonaTTSConfig config)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<!-- VoiceTuner TTS配置导出 -->");
            sb.AppendLine($"<!-- 生成时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss} -->");
            sb.AppendLine("<Defs>");
            sb.AppendLine();
            sb.AppendLine($"  <!-- {config.PersonaDisplayName} TTS配置 -->");
            sb.AppendLine("  <TheSecondSeat.PersonaGeneration.NarratorPersonaDef>");
            sb.AppendLine($"    <defName>{config.PersonaDefName}</defName>");
            sb.AppendLine();
            sb.AppendLine("    <!-- TTS语音配置 -->");
            
            var providerConfig = config.ProviderConfig;
            
            // 基础TTS参数（兼容The Second Seat）
            sb.AppendLine($"    <ttsVoiceName>{EscapeXml(providerConfig.VoiceId)}</ttsVoiceName>");
            sb.AppendLine($"    <ttsVoicePitch>{providerConfig.Pitch:F2}</ttsVoicePitch>");
            sb.AppendLine($"    <ttsVoiceRate>{providerConfig.Speed:F2}</ttsVoiceRate>");
            sb.AppendLine();
            
            // 扩展TTS配置
            sb.AppendLine("    <!-- 扩展TTS配置（VoiceTuner专用） -->");
            sb.AppendLine($"    <ttsProviderType>{providerConfig.ProviderType}</ttsProviderType>");
            sb.AppendLine($"    <ttsApiEndpoint>{EscapeXml(providerConfig.ApiEndpoint)}</ttsApiEndpoint>");
            sb.AppendLine($"    <ttsVolume>{providerConfig.Volume:F2}</ttsVolume>");
            
            // 根据提供商类型添加特定配置
            switch (providerConfig.ProviderType)
            {
                case TTSProviderType.Azure:
                    sb.AppendLine();
                    sb.AppendLine("    <!-- Azure TTS专用配置 -->");
                    sb.AppendLine($"    <ttsAzureRegion>{EscapeXml(providerConfig.AzureRegion)}</ttsAzureRegion>");
                    if (!string.IsNullOrEmpty(providerConfig.AzureStyle))
                    {
                        sb.AppendLine($"    <ttsAzureStyle>{EscapeXml(providerConfig.AzureStyle)}</ttsAzureStyle>");
                        sb.AppendLine($"    <ttsAzureStyleDegree>{providerConfig.AzureStyleDegree:F2}</ttsAzureStyleDegree>");
                    }
                    if (!string.IsNullOrEmpty(providerConfig.AzureRole))
                    {
                        sb.AppendLine($"    <ttsAzureRole>{EscapeXml(providerConfig.AzureRole)}</ttsAzureRole>");
                    }
                    break;
                    
                case TTSProviderType.FishAudio:
                    sb.AppendLine();
                    sb.AppendLine("    <!-- Fish Audio专用配置 -->");
                    sb.AppendLine($"    <ttsFishReferenceId>{EscapeXml(providerConfig.FishReferenceId)}</ttsFishReferenceId>");
                    sb.AppendLine($"    <ttsFishLatency>{EscapeXml(providerConfig.FishLatency)}</ttsFishLatency>");
                    sb.AppendLine($"    <ttsFishFormat>{EscapeXml(providerConfig.FishFormat)}</ttsFishFormat>");
                    break;
                    
            }
            
            sb.AppendLine();
            sb.AppendLine("  </TheSecondSeat.PersonaGeneration.NarratorPersonaDef>");
            sb.AppendLine();
            sb.AppendLine("</Defs>");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// 注入配置到Mod的XML文件
        /// </summary>
        public static void InjectToMod(PersonaTTSConfig config, string modPath, bool backup = true)
        {
            // 查找NarratorPersonaDefs XML文件
            string defsPath = Path.Combine(modPath, "Defs");
            if (!Directory.Exists(defsPath))
            {
                throw new DirectoryNotFoundException($"Defs目录不存在: {defsPath}");
            }
            
            // 查找包含该人格定义的XML文件
            string? targetFile = FindPersonaDefFile(defsPath, config.PersonaDefName);
            
            if (string.IsNullOrEmpty(targetFile))
            {
                throw new FileNotFoundException($"未找到包含 {config.PersonaDefName} 的XML文件");
            }
            
            // 备份原文件
            if (backup)
            {
                string backupPath = targetFile + $".backup_{DateTime.Now:yyyyMMdd_HHmmss}";
                File.Copy(targetFile, backupPath, true);
                Log.Message($"[VoiceTuner] Backup created: {backupPath}");
            }
            
            // 读取并修改XML
            string xmlContent = File.ReadAllText(targetFile, Encoding.UTF8);
            string modifiedXml = InjectTTSConfig(xmlContent, config);
            
            // 写回文件
            File.WriteAllText(targetFile, modifiedXml, Encoding.UTF8);
            
            Log.Message($"[VoiceTuner] Injected TTS config to: {targetFile}");
        }
        
        /// <summary>
        /// 查找包含指定人格定义的XML文件
        /// </summary>
        private static string? FindPersonaDefFile(string defsPath, string personaDefName)
        {
            foreach (string file in Directory.GetFiles(defsPath, "*.xml", SearchOption.AllDirectories))
            {
                string content = File.ReadAllText(file);
                if (content.Contains($"<defName>{personaDefName}</defName>"))
                {
                    return file;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 注入TTS配置到XML内容
        /// </summary>
        private static string InjectTTSConfig(string xmlContent, PersonaTTSConfig config)
        {
            var providerConfig = config.ProviderConfig;
            
            // 使用正则表达式查找并替换或添加TTS配置
            // 查找目标人格定义块
            string defPattern = $@"(<defName>{Regex.Escape(config.PersonaDefName)}</defName>.*?)(</TheSecondSeat\.PersonaGeneration\.NarratorPersonaDef>)";
            
            var match = Regex.Match(xmlContent, defPattern, RegexOptions.Singleline);
            if (!match.Success)
            {
                throw new InvalidOperationException($"无法在XML中找到 {config.PersonaDefName} 的定义块");
            }
            
            string defContent = match.Groups[1].Value;
            string closingTag = match.Groups[2].Value;
            
            // 移除现有的TTS配置行
            string[] ttsFields = new[]
            {
                "ttsVoiceName", "ttsVoicePitch", "ttsVoiceRate", "ttsVoiceSpeed",
                "ttsProviderType", "ttsApiEndpoint", "ttsVolume",
                "ttsAzureRegion", "ttsAzureStyle", "ttsAzureStyleDegree", "ttsAzureRole",
                "ttsFishReferenceId", "ttsFishLatency", "ttsFishFormat",
                "ttsIndexSpeakerId", "ttsIndexFormat"
            };
            
            foreach (string field in ttsFields)
            {
                defContent = Regex.Replace(defContent, $@"\s*<{field}>[^<]*</{field}>", "");
            }
            
            // 构建新的TTS配置
            StringBuilder newTTSConfig = new StringBuilder();
            newTTSConfig.AppendLine();
            newTTSConfig.AppendLine("    <!-- TTS语音配置（由VoiceTuner生成） -->");
            newTTSConfig.AppendLine($"    <ttsVoiceName>{EscapeXml(providerConfig.VoiceId)}</ttsVoiceName>");
            newTTSConfig.AppendLine($"    <ttsVoicePitch>{providerConfig.Pitch:F1}</ttsVoicePitch>");
            newTTSConfig.AppendLine($"    <ttsVoiceRate>{providerConfig.Speed:F1}</ttsVoiceRate>");
            
            // 添加提供商特定配置
            switch (providerConfig.ProviderType)
            {
                case TTSProviderType.Azure:
                    if (!string.IsNullOrEmpty(providerConfig.AzureStyle))
                    {
                        newTTSConfig.AppendLine($"    <ttsAzureStyle>{EscapeXml(providerConfig.AzureStyle)}</ttsAzureStyle>");
                        newTTSConfig.AppendLine($"    <ttsAzureStyleDegree>{providerConfig.AzureStyleDegree:F2}</ttsAzureStyleDegree>");
                    }
                    if (!string.IsNullOrEmpty(providerConfig.AzureRole))
                    {
                        newTTSConfig.AppendLine($"    <ttsAzureRole>{EscapeXml(providerConfig.AzureRole)}</ttsAzureRole>");
                    }
                    break;
            }
            
            newTTSConfig.Append("    ");
            
            // 组合新的定义内容
            string newDefContent = defContent.TrimEnd() + newTTSConfig.ToString() + "\n  " + closingTag;
            
            // 替换原内容
            return xmlContent.Substring(0, match.Index) + newDefContent + xmlContent.Substring(match.Index + match.Length);
        }
        
        /// <summary>
        /// 转义XML特殊字符
        /// </summary>
        private static string EscapeXml(string? text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            
            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
        
        /// <summary>
        /// 从XML导入配置
        /// </summary>
        public static PersonaTTSConfig? ImportFromXml(string xmlPath)
        {
            if (!File.Exists(xmlPath))
            {
                Log.Warning($"[VoiceTuner] XML file not found: {xmlPath}");
                return null;
            }
            
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);
                
                var defNode = doc.SelectSingleNode("//TheSecondSeat.PersonaGeneration.NarratorPersonaDef");
                if (defNode == null)
                {
                    Log.Warning("[VoiceTuner] NarratorPersonaDef not found in XML");
                    return null;
                }
                
                string defName = defNode.SelectSingleNode("defName")?.InnerText ?? "";
                string narratorName = defNode.SelectSingleNode("narratorName")?.InnerText ?? defName;
                
                var config = new PersonaTTSConfig
                {
                    PersonaDefName = defName,
                    PersonaDisplayName = narratorName
                };
                
                // 读取TTS配置
                config.ProviderConfig.VoiceId = defNode.SelectSingleNode("ttsVoiceName")?.InnerText ?? "";
                
                if (float.TryParse(defNode.SelectSingleNode("ttsVoicePitch")?.InnerText, out float pitch))
                {
                    config.ProviderConfig.Pitch = pitch;
                }
                
                if (float.TryParse(defNode.SelectSingleNode("ttsVoiceRate")?.InnerText, out float rate))
                {
                    config.ProviderConfig.Speed = rate;
                }
                
                // Azure特定配置
                string azureStyle = defNode.SelectSingleNode("ttsAzureStyle")?.InnerText ?? "";
                if (!string.IsNullOrEmpty(azureStyle))
                {
                    config.ProviderConfig.ProviderType = TTSProviderType.Azure;
                    config.ProviderConfig.AzureStyle = azureStyle;
                    
                    if (float.TryParse(defNode.SelectSingleNode("ttsAzureStyleDegree")?.InnerText, out float styleDegree))
                    {
                        config.ProviderConfig.AzureStyleDegree = styleDegree;
                    }
                }
                
                config.ProviderConfig.AzureRole = defNode.SelectSingleNode("ttsAzureRole")?.InnerText ?? "";
                
                return config;
            }
            catch (Exception ex)
            {
                Log.Error($"[VoiceTuner] Failed to import XML: {ex.Message}");
                return null;
            }
        }
    }
}