using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ListenToMe
{
    /// <summary>
    /// 调试工具类
    /// </summary>
    public static class DebugTools
    {
        private static List<string> commandHistory = new List<string>();
        private static bool debugMode = false;

        /// <summary>
        /// 启用/禁用调试模式
        /// </summary>
        public static bool DebugMode
        {
            get => debugMode;
            set
            {
                debugMode = value;
                Log.Message($"[ListenToMe] Debug mode {(value ? "enabled" : "disabled")}");
            }
        }

        /// <summary>
        /// 记录指令到历史
        /// </summary>
        public static void LogCommand(string command, Pawn pawn, ParsedCommand parsed, bool success)
        {
            string log = $"[{DateTime.Now:HH:mm:ss}] Pawn: {pawn?.LabelShort ?? "null"}, " +
                        $"Input: '{command}', Type: {parsed?.Type ?? CommandParser.CommandType.Unknown}, " +
                        $"Success: {success}";
            
            commandHistory.Add(log);
            
            if (debugMode)
            {
                Log.Message($"[ListenToMe] {log}");
            }

            // 保持历史记录在合理大小
            if (commandHistory.Count > 100)
            {
                commandHistory.RemoveAt(0);
            }
        }

        /// <summary>
        /// 获取指令历史
        /// </summary>
        public static List<string> GetCommandHistory()
        {
            return new List<string>(commandHistory);
        }

        /// <summary>
        /// 清除历史
        /// </summary>
        public static void ClearHistory()
        {
            commandHistory.Clear();
            Log.Message("[ListenToMe] Command history cleared");
        }

        /// <summary>
        /// 生成诊断报告
        /// </summary>
        public static string GenerateDiagnosticReport(Pawn pawn)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Listen To Me Diagnostic Report ===");
            sb.AppendLine($"Time: {DateTime.Now}");
            sb.AppendLine();

            if (pawn != null)
            {
                sb.AppendLine("=== Pawn Information ===");
                sb.AppendLine($"Name: {pawn.LabelShort}");
                sb.AppendLine($"Faction: {pawn.Faction?.Name ?? "None"}");
                sb.AppendLine($"Status: Dead={pawn.Dead}, Downed={pawn.Downed}");
                
                if (pawn.jobs?.curJob != null)
                {
                    sb.AppendLine($"Current Job: {pawn.jobs.curJob.GetReport(pawn)}");
                }
                else
                {
                    sb.AppendLine("Current Job: None");
                }

                if (pawn.needs?.mood != null)
                {
                    sb.AppendLine($"Mood: {pawn.needs.mood.CurLevel:P0}");
                }

                sb.AppendLine();
            }

            sb.AppendLine("=== System Status ===");
            sb.AppendLine($"Debug Mode: {debugMode}");
            sb.AppendLine($"Active Dialogues: {DialogueSystem.IsInDialogue(pawn)}");
            sb.AppendLine($"Command History Count: {commandHistory.Count}");
            sb.AppendLine();

            sb.AppendLine("=== Recent Commands ===");
            var recentCommands = commandHistory.Count > 10 
                ? commandHistory.GetRange(commandHistory.Count - 10, 10) 
                : commandHistory;
            
            foreach (var cmd in recentCommands)
            {
                sb.AppendLine(cmd);
            }

            sb.AppendLine();
            sb.AppendLine("=== End of Report ===");

            return sb.ToString();
        }

        /// <summary>
        /// 测试指令解析
        /// </summary>
        public static void TestCommandParsing(string input, Pawn pawn)
        {
            Log.Message($"[ListenToMe] Testing command: '{input}'");
            
            var parsed = CommandParser.ParseCommand(input, pawn);
            
            Log.Message($"[ListenToMe] Parse Result:");
            Log.Message($"  Type: {parsed.Type}");
            Log.Message($"  Target: {parsed.Target?.Label ?? "None"}");
            Log.Message($"  ItemToCraft: {parsed.ItemToCraft?.label ?? "None"}");
            Log.Message($"  Count: {parsed.Count}");
            Log.Message($"  Location: {parsed.TargetLocation}");
        }

        /// <summary>
        /// 列出所有可用的工作台
        /// </summary>
        public static void ListAvailableWorkbenches(Map map)
        {
            if (map == null)
            {
                Log.Warning("[ListenToMe] No map provided");
                return;
            }

            Log.Message("[ListenToMe] Available Workbenches:");
            
            var workbenches = map.listerBuildings.allBuildingsColonist
                .OfType<Building_WorkTable>();

            foreach (var wb in workbenches)
            {
                Log.Message($"  - {wb.Label} ({wb.def.defName}) at {wb.Position}");
                
                if (wb.def.AllRecipes != null)
                {
                    Log.Message($"    Recipes: {wb.def.AllRecipes.Count}");
                }
            }
        }

        /// <summary>
        /// 列出所有可制作的物品
        /// </summary>
        public static void ListCraftableItems(string filter = "")
        {
            Log.Message("[ListenToMe] Craftable Items:");
            
            var recipes = DefDatabase<RecipeDef>.AllDefsListForReading
                .Where(r => r.products != null && r.products.Any());

            int count = 0;
            foreach (var recipe in recipes)
            {
                if (!string.IsNullOrEmpty(filter) && 
                    !recipe.label.ToLower().Contains(filter.ToLower()))
                {
                    continue;
                }

                var product = recipe.products.First();
                Log.Message($"  - {recipe.label}: {product.thingDef.label} x{product.count}");
                count++;
                
                if (count >= 20) // 限制输出数量
                {
                    Log.Message("  ... (truncated, too many results)");
                    break;
                }
            }
        }

        /// <summary>
        /// 性能统计
        /// </summary>
        public static class PerformanceStats
        {
            private static int totalCommands = 0;
            private static int successfulCommands = 0;
            private static int failedCommands = 0;
            private static float totalParseTime = 0f;

            public static void RecordCommand(bool success, float parseTime)
            {
                totalCommands++;
                if (success)
                    successfulCommands++;
                else
                    failedCommands++;
                totalParseTime += parseTime;
            }

            public static void PrintStats()
            {
                Log.Message("[ListenToMe] Performance Statistics:");
                Log.Message($"  Total Commands: {totalCommands}");
                Log.Message($"  Successful: {successfulCommands} ({(totalCommands > 0 ? (float)successfulCommands / totalCommands * 100 : 0):F1}%)");
                Log.Message($"  Failed: {failedCommands} ({(totalCommands > 0 ? (float)failedCommands / totalCommands * 100 : 0):F1}%)");
                Log.Message($"  Avg Parse Time: {(totalCommands > 0 ? totalParseTime / totalCommands * 1000 : 0):F2}ms");
            }

            public static void Reset()
            {
                totalCommands = 0;
                successfulCommands = 0;
                failedCommands = 0;
                totalParseTime = 0f;
            }
        }
    }

    /// <summary>
    /// 开发者控制台命令
    /// </summary>
    [StaticConstructorOnStartup]
    public static class DevConsoleCommands
    {
        static DevConsoleCommands()
        {
            // 注册开发者命令（如果需要）
            if (Prefs.DevMode)
            {
                Log.Message("[ListenToMe] Developer commands registered");
            }
        }
    }
}
