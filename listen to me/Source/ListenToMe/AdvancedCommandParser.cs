using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ListenToMe
{
    /// <summary>
    /// 高级指令解析器 - 扩展功能
    /// </summary>
    public static class AdvancedCommandParser
    {
        /// <summary>
        /// 解析复合指令（例如：去厨房然后做饭）
        /// </summary>
        public static List<ParsedCommand> ParseCompoundCommand(string input, Pawn pawn)
        {
            var commands = new List<ParsedCommand>();
            
            // 分割连续指令的关键词
            string[] separators = { "然后", "接着", "再", "之后", "后", "and then", "then", "after" };
            
            var parts = SplitByKeywords(input, separators);
            
            foreach (var part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part))
                {
                    var command = CommandParser.ParseCommand(part.Trim(), pawn);
                    if (command.Type != CommandParser.CommandType.Unknown)
                    {
                        commands.Add(command);
                    }
                }
            }
            
            return commands;
        }

        /// <summary>
        /// 提取位置信息（坐标或区域名称）
        /// </summary>
        public static IntVec3 ExtractLocation(string input, Pawn pawn)
        {
            if (pawn?.Map == null)
                return IntVec3.Invalid;

            // 尝试查找房间
            var rooms = pawn.Map.regionGrid.allRooms;
            foreach (var room in rooms)
            {
                if (room.Role?.label != null && input.Contains(room.Role.label.ToLower()))
                {
                    return room.Cells.RandomElement();
                }
            }

            // 尝试查找地区
            var areas = pawn.Map.areaManager.AllAreas;
            foreach (var area in areas)
            {
                if (area.Label != null && input.Contains(area.Label.ToLower()))
                {
                    var cells = area.ActiveCells.ToList();
                    if (cells.Any())
                    {
                        return cells.RandomElement();
                    }
                }
            }

            return IntVec3.Invalid;
        }

        /// <summary>
        /// 识别物品材质和品质要求
        /// </summary>
        public static void ParseItemQuality(string input, out QualityCategory quality, out ThingDef stuff)
        {
            quality = QualityCategory.Normal;
            stuff = null;

            // 品质关键词
            var qualityKeywords = new Dictionary<string, QualityCategory>
            {
                { "传说", QualityCategory.Legendary },
                { "legend", QualityCategory.Legendary },
                { "杰作", QualityCategory.Masterwork },
                { "master", QualityCategory.Masterwork },
                { "优秀", QualityCategory.Excellent },
                { "excellent", QualityCategory.Excellent },
                { "良好", QualityCategory.Good },
                { "good", QualityCategory.Good },
                { "普通", QualityCategory.Normal },
                { "normal", QualityCategory.Normal },
                { "差", QualityCategory.Poor },
                { "poor", QualityCategory.Poor },
                { "劣质", QualityCategory.Awful },
                { "awful", QualityCategory.Awful }
            };

            foreach (var kvp in qualityKeywords)
            {
                if (input.Contains(kvp.Key))
                {
                    quality = kvp.Value;
                    break;
                }
            }

            // 材质关键词
            var stuffKeywords = new Dictionary<string, string>
            {
                { "钢", "Steel" },
                { "steel", "Steel" },
                { "木", "WoodLog" },
                { "wood", "WoodLog" },
                { "石", "BlocksGranite" },
                { "stone", "BlocksGranite" },
                { "银", "Silver" },
                { "silver", "Silver" },
                { "金", "Gold" },
                { "gold", "Gold" },
                { "布", "Cloth" },
                { "cloth", "Cloth" },
                { "皮", "Leather_Plain" },
                { "leather", "Leather_Plain" }
            };

            foreach (var kvp in stuffKeywords)
            {
                if (input.Contains(kvp.Key))
                {
                    stuff = DefDatabase<ThingDef>.GetNamedSilentFail(kvp.Value);
                    if (stuff != null)
                        break;
                }
            }
        }

        /// <summary>
        /// 解析条件指令（例如：如果有敌人就攻击）
        /// </summary>
        public static bool ParseConditionalCommand(string input, Pawn pawn, out string condition, out ParsedCommand command)
        {
            condition = "";
            command = null;

            string[] conditionKeywords = { "如果", "当", "若", "if", "when" };
            string[] actionKeywords = { "就", "则", "那么", "then" };

            bool hasCondition = false;
            foreach (var keyword in conditionKeywords)
            {
                if (input.Contains(keyword))
                {
                    hasCondition = true;
                    break;
                }
            }

            if (!hasCondition)
                return false;

            // 分割条件和动作
            foreach (var actionKeyword in actionKeywords)
            {
                int index = input.IndexOf(actionKeyword);
                if (index > 0)
                {
                    condition = input.Substring(0, index).Trim();
                    string action = input.Substring(index + actionKeyword.Length).Trim();
                    command = CommandParser.ParseCommand(action, pawn);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 评估条件是否满足
        /// </summary>
        public static bool EvaluateCondition(string condition, Pawn pawn)
        {
            condition = condition.ToLower();

            // 敌人相关
            if (condition.Contains("敌人") || condition.Contains("enemy"))
            {
                return pawn.Map?.mapPawns.AllPawns.Any(p => p.HostileTo(pawn.Faction) && !p.Dead) ?? false;
            }

            // 受伤相关
            if (condition.Contains("受伤") || condition.Contains("injured") || condition.Contains("wounded"))
            {
                return pawn.Map?.mapPawns.AllPawns.Any(p => p.health.HasHediffsNeedingTend()) ?? false;
            }

            // 饥饿相关
            if (condition.Contains("饿") || condition.Contains("hungry") || condition.Contains("饥饿"))
            {
                return pawn.needs?.food?.CurLevel < 0.3f;
            }

            // 疲劳相关
            if (condition.Contains("累") || condition.Contains("tired") || condition.Contains("疲劳"))
            {
                return pawn.needs?.rest?.CurLevel < 0.3f;
            }

            // 默认返回true
            return true;
        }

        /// <summary>
        /// 按关键词分割字符串
        /// </summary>
        private static string[] SplitByKeywords(string input, string[] keywords)
        {
            var result = new List<string>();
            var current = input;

            foreach (var keyword in keywords)
            {
                var parts = current.Split(new[] { keyword }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    result.AddRange(parts);
                    break;
                }
            }

            if (result.Count == 0)
            {
                result.Add(input);
            }

            return result.ToArray();
        }

        /// <summary>
        /// 智能纠错（处理常见的拼写错误）
        /// </summary>
        public static string AutoCorrect(string input)
        {
            var corrections = new Dictionary<string, string>
            {
                { "去厨房做法", "去厨房做饭" },
                { "公鸡", "攻击" },
                { "治聊", "治疗" },
                { "狩来", "狩猎" }
            };

            foreach (var kvp in corrections)
            {
                if (input.Contains(kvp.Key))
                {
                    input = input.Replace(kvp.Key, kvp.Value);
                }
            }

            return input;
        }

        /// <summary>
        /// 生成指令建议（当指令不清楚时）
        /// </summary>
        public static List<string> GenerateCommandSuggestions(string input, Pawn pawn)
        {
            var suggestions = new List<string>();

            // 基于输入的部分匹配提供建议
            if (input.Length < 2)
                return suggestions;

            var allCommands = new List<string>
            {
                "去厨房", "去仓库", "去医疗室",
                "做饭", "清洁", "工作",
                "攻击敌人", "狩猎", "采集",
                "制作防尘大衣", "制作木墙", "制作药品",
                "治疗伤员", "等待", "休息"
            };

            foreach (var cmd in allCommands)
            {
                if (cmd.Contains(input) || input.Contains(cmd.Substring(0, Math.Min(2, cmd.Length))))
                {
                    suggestions.Add(cmd);
                }
            }

            return suggestions.Take(5).ToList();
        }
    }
}
