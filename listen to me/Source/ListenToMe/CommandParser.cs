using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace ListenToMe
{
    /// <summary>
    /// 文本指令解析器 - 支持关键词匹配和智能分析
    /// 指令格式: [动词] + [位置] + [动作] + [对象]
    /// 例如: "去厨房做饭"、"到仓库拿木材"、"前往医疗室治疗伤员"
    /// </summary>
    public static class CommandParser
    {
        // 动作类型枚举
        public enum CommandType
        {
            Move,           // 移动
            Work,           // 工作
            Fight,          // 战斗
            Wait,           // 等待
            Construct,      // 建造
            Craft,          // 制作
            Haul,           // 搬运
            Hunt,           // 狩猎
            Gather,         // 采集
            Mine,           // 挖矿
            Tend,           // 医疗
            Social,         // 社交
            Research,       // 研究
            Clean,          // 清洁
            Extinguish,     // 灭火
            Repair,         // 修理
            Unknown         // 未知
        }

        // 移动动词 - "去/来/到/前往"
        private static readonly List<string> MovementVerbs = new List<string>
        {
            "去", "来", "到", "前往", "走到", "移动到", "过去",
            "go", "goto", "move to", "head to", "walk to"
        };

        // 位置关键词
        private static readonly Dictionary<string, List<string>> LocationKeywords = new Dictionary<string, List<string>>
        {
            { "厨房", new List<string> { "厨房", "炉灶", "kitchen", "stove" } },
            { "仓库", new List<string> { "仓库", "储藏室", "storage", "warehouse" } },
            { "医疗室", new List<string> { "医疗室", "病房", "hospital", "medical" } },
            { "工作台", new List<string> { "工作台", "工作间", "workshop", "workbench" } },
            { "裁缝台", new List<string> { "裁缝台", "缝纫台", "tailor", "tailoring" } },
            { "锻造台", new List<string> { "锻造台", "铁匠", "forge", "smithing" } },
            { "研究台", new List<string> { "研究台", "研究室", "research", "lab" } },
            { "卧室", new List<string> { "卧室", "寝室", "bedroom", "sleeping" } },
            { "外面", new List<string> { "外面", "室外", "野外", "outside" } }
        };

        // 动作关键词 - "做/干/制作/建造"等
        private static readonly Dictionary<CommandType, List<string>> ActionKeywords = new Dictionary<CommandType, List<string>>
        {
            { CommandType.Work, new List<string> { 
                "做", "干", "工作", "干活", "做事", "劳动", 
                "do", "work", "labor", "perform" 
            }},
            { CommandType.Craft, new List<string> { 
                "制作", "做", "生产", "制造", "加工", "合成", "造",
                "craft", "make", "produce", "manufacture", "create" 
            }},
            { CommandType.Fight, new List<string> { 
                "攻击", "战斗", "打", "杀", "消灭", "击杀", "战",
                "attack", "fight", "kill", "combat", "battle", "destroy" 
            }},
            { CommandType.Construct, new List<string> { 
                "建造", "建", "修建", "盖", "建设", "搭建",
                "build", "construct", "create", "erect" 
            }},
            { CommandType.Haul, new List<string> { 
                "搬运", "搬", "运", "拿", "取", "搬走", "运送",
                "haul", "carry", "transport", "move", "fetch" 
            }},
            { CommandType.Hunt, new List<string> { 
                "狩猎", "猎", "打猎", "猎杀",
                "hunt", "hunting" 
            }},
            { CommandType.Gather, new List<string> { 
                "采集", "采", "收集", "收获", "采摘",
                "gather", "collect", "harvest", "pick" 
            }},
            { CommandType.Mine, new List<string> { 
                "挖矿", "挖", "开采", "采矿", "挖掘",
                "mine", "mining", "dig", "excavate" 
            }},
            { CommandType.Tend, new List<string> { 
                "治疗", "医疗", "照顾", "救治", "看病", "医治",
                "tend", "heal", "doctor", "medical", "treat", "cure" 
            }},
            { CommandType.Clean, new List<string> { 
                "清洁", "打扫", "清理", "扫", "清扫",
                "clean", "sweep", "clear", "tidy" 
            }},
            { CommandType.Repair, new List<string> { 
                "修理", "维修", "修复", "修",
                "repair", "fix", "maintain", "mend" 
            }},
            { CommandType.Wait, new List<string> { 
                "等待", "等", "停", "待命", "休息", "待着",
                "wait", "stop", "rest", "stay", "standby" 
            }}
        };

        // 对象关键词（用于识别目标物体）
        private static readonly Dictionary<string, List<string>> ObjectKeywords = new Dictionary<string, List<string>>
        {
            { "敌人", new List<string> { "敌人", "敌", "入侵者", "袭击者", "enemy", "hostile", "invader" } },
            { "伤员", new List<string> { "伤员", "伤者", "病人", "wounded", "injured", "patient" } },
            { "动物", new List<string> { "动物", "野兽", "猎物", "animal", "beast", "prey" } },
            { "植物", new List<string> { "植物", "作物", "庄稼", "plant", "crop" } },
            { "矿石", new List<string> { "矿石", "矿", "石头", "ore", "rock", "stone" } },
            { "物资", new List<string> { "物资", "物品", "东西", "材料", "item", "material", "stuff" } }
        };

        // 工作台类型识别
        private static readonly Dictionary<string, Type> WorkbenchTypes = new Dictionary<string, Type>
        {
            { "裁缝", typeof(Building_WorkTable) },
            { "tailor", typeof(Building_WorkTable) },
            { "锻造", typeof(Building_WorkTable) },
            { "forge", typeof(Building_WorkTable) },
            { "炉", typeof(Building_WorkTable) },
            { "smithing", typeof(Building_WorkTable) },
            { "厨房", typeof(Building_WorkTable) },
            { "kitchen", typeof(Building_WorkTable) },
            { "炉灶", typeof(Building_WorkTable) },
            { "stove", typeof(Building_WorkTable) },
            { "工作台", typeof(Building_WorkTable) },
            { "bench", typeof(Building_WorkTable) },
            { "桌", typeof(Building_WorkTable) },
            { "table", typeof(Building_WorkTable) }
        };

        /// <summary>
        /// 解析指令文本
        /// 格式: [移动动词] + [位置] + [动作] + [对象]
        /// 例如: "去厨房做饭"、"到仓库拿木材"、"前往医疗室治疗伤员"
        /// </summary>
        public static ParsedCommand ParseCommand(string input, Pawn pawn)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ParsedCommand { Type = CommandType.Unknown };
            }

            input = input.ToLower().Trim();
            
            var command = new ParsedCommand
            {
                OriginalText = input
            };
            
            // 1. 检查是否包含移动动词
            bool hasMovementVerb = MovementVerbs.Any(v => input.StartsWith(v));
            
            // 2. 提取位置信息
            string location = ExtractLocation(input);
            
            // 3. 提取动作类型
            command.Type = ExtractAction(input);
            
            // 4. 提取对象信息
            string targetObject = ExtractObject(input);
            
            // 5. 根据格式智能判断指令
            if (hasMovementVerb && !string.IsNullOrEmpty(location))
            {
                // 格式: "去[位置]" 或 "去[位置][做什么]"
                command.TargetLocation = FindLocationOnMap(location, pawn);
                
                if (command.Type == CommandType.Unknown)
                {
                    // 纯移动指令
                    command.Type = CommandType.Move;
                }
                // 否则是组合指令（移动+动作），保持动作类型
            }
            else if (command.Type != CommandType.Unknown)
            {
                // 直接动作指令，如 "制作防尘大衣"
                command.Target = ExtractTarget(input, pawn);
            }
            else
            {
                // 尝试关键词匹配
                command.Type = MatchKeyword(input);
                command.Target = ExtractTarget(input, pawn);
            }
            
            // 6. 提取要制作的物品
            if (command.Type == CommandType.Craft)
            {
                command.ItemToCraft = ExtractCraftItem(input);
            }
            
            // 7. 提取数量
            command.Count = ExtractCount(input);
            
            // 8. AI分析增强（如果还是不明确)
            if (command.Type == CommandType.Unknown)
            {
                command.Type = AIAnalyzeCommand(input);
            }
            
            return command;
        }

        /// <summary>
        /// 提取位置关键词
        /// </summary>
        private static string ExtractLocation(string input)
        {
            foreach (var kvp in LocationKeywords)
            {
                if (kvp.Value.Any(keyword => input.Contains(keyword)))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// 在地图上查找位置
        /// </summary>
        private static IntVec3 FindLocationOnMap(string location, Pawn pawn)
        {
            if (pawn?.Map == null || string.IsNullOrEmpty(location))
                return IntVec3.Invalid;

            // 查找房间
            var rooms = pawn.Map.regionGrid.allRooms;
            foreach (var room in rooms)
            {
                if (room.Role?.label != null)
                {
                    string roomLabel = room.Role.label.ToLower();
                    if (LocationKeywords.TryGetValue(location, out var keywords))
                    {
                        if (keywords.Any(k => roomLabel.Contains(k) || k.Contains(roomLabel)))
                        {
                            return room.Cells.RandomElement();
                        }
                    }
                }
            }

            // 查找建筑（如工作台）
            foreach (var kvp in WorkbenchTypes)
            {
                if (LocationKeywords.TryGetValue(location, out var keywords))
                {
                    if (keywords.Contains(kvp.Key))
                    {
                        var building = pawn.Map.listerBuildings.allBuildingsColonist
                            .Where(b => b.def.label.ToLower().Contains(kvp.Key.ToLower()))
                            .OrderBy(b => b.Position.DistanceTo(pawn.Position))
                            .FirstOrDefault();
                        
                        if (building != null)
                            return building.Position;
                    }
                }
            }

            return IntVec3.Invalid;
        }

        /// <summary>
        /// 提取动作类型
        /// </summary>
        private static CommandType ExtractAction(string input)
        {
            foreach (var kvp in ActionKeywords)
            {
                if (kvp.Value.Any(keyword => input.Contains(keyword)))
                {
                    return kvp.Key;
                }
            }
            return CommandType.Unknown;
        }

        /// <summary>
        /// 提取对象关键词
        /// </summary>
        private static string ExtractObject(string input)
        {
            foreach (var kvp in ObjectKeywords)
            {
                if (kvp.Value.Any(keyword => input.Contains(keyword)))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// 关键词匹配（备用方法）
        /// </summary>
        private static CommandType MatchKeyword(string input)
        {
            // 先检查动作关键词
            foreach (var kvp in ActionKeywords)
            {
                if (kvp.Value.Any(keyword => input.Contains(keyword)))
                {
                    return kvp.Key;
                }
            }
            
            return CommandType.Unknown;
        }

        /// <summary>
        /// 提取目标对象
        /// </summary>
        private static Thing ExtractTarget(string input, Pawn pawn)
        {
            if (pawn == null || pawn.Map == null) return null;

            // 查找地图上的建筑和物体
            var allThings = pawn.Map.listerThings.AllThings;
            
            // 尝试匹配工作台
            foreach (var kvp in WorkbenchTypes)
            {
                if (input.Contains(kvp.Key))
                {
                    var workbench = allThings
                        .OfType<Building_WorkTable>()
                        .Where(b => b.def.label.ToLower().Contains(kvp.Key) || 
                                   b.def.defName.ToLower().Contains(kvp.Key))
                        .OrderBy(b => b.Position.DistanceTo(pawn.Position))
                        .FirstOrDefault();
                    
                    if (workbench != null) return workbench;
                }
            }
            
            // 尝试匹配其他物体
            var targetThing = allThings
                .Where(t => t.def.label.ToLower().Contains(input) || 
                           input.Contains(t.def.label.ToLower()))
                .OrderBy(t => t.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();
            
            return targetThing;
        }

        /// <summary>
        /// 提取要制作的物品
        /// </summary>
        private static ThingDef ExtractCraftItem(string input)
        {
            // 搜索所有可制作的物品定义
            var allDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            
            // 移除动词和位置词，只保留物品名称
            string itemName = input;
            foreach (var verb in MovementVerbs)
            {
                itemName = itemName.Replace(verb, "");
            }
            foreach (var kvp in ActionKeywords)
            {
                foreach (var action in kvp.Value)
                {
                    itemName = itemName.Replace(action, "");
                }
            }
            foreach (var kvp in LocationKeywords)
            {
                foreach (var loc in kvp.Value)
                {
                    itemName = itemName.Replace(loc, "");
                }
            }
            itemName = itemName.Trim();
            
            // 尝试精确匹配
            var exactMatch = allDefs.FirstOrDefault(def => 
                itemName.Contains(def.label.ToLower()) || 
                def.label.ToLower().Contains(itemName));
            
            if (exactMatch != null) return exactMatch;
            
            // 尝试模糊匹配
            var fuzzyMatch = allDefs.FirstOrDefault(def =>
            {
                var label = def.label.ToLower();
                var defName = def.defName.ToLower();
                return itemName.Split(' ', '，', ',').Any(word => 
                    word.Length > 1 && (label.Contains(word) || defName.Contains(word)));
            });
            
            return fuzzyMatch;
        }

        /// <summary>
        /// 提取数量
        /// </summary>
        private static int ExtractCount(string input)
        {
            // 查找数字
            var words = input.Split(' ', '，', ',', '个', '件', '只', '条', '张');
            foreach (var word in words)
            {
                if (int.TryParse(word, out int count))
                {
                    return Math.Max(1, count);
                }
            }
            
            // 查找中文数字
            var chineseNumbers = new Dictionary<string, int>
            {
                {"一", 1}, {"二", 2}, {"三", 3}, {"四", 4}, {"五", 5},
                {"六", 6}, {"七", 7}, {"八", 8}, {"九", 9}, {"十", 10}
            };
            
            foreach (var kvp in chineseNumbers)
            {
                if (input.Contains(kvp.Key))
                {
                    return kvp.Value;
                }
            }
            
            return 1;
        }

        /// <summary>
        /// AI智能分析指令（简单的规则引擎）
        /// </summary>
        private static CommandType AIAnalyzeCommand(string input)
        {
            // 基于上下文的智能判断
            
            // 包含位置词 -> 移动
            if (ContainsPositionWords(input))
                return CommandType.Move;
            
            // 包含敌对词 -> 战斗
            if (ContainsHostileWords(input))
                return CommandType.Fight;
            
            // 包含物品制作词 -> 制作
            if (ContainsCraftWords(input))
                return CommandType.Craft;
            
            // 默认为工作
            return CommandType.Work;
        }

        private static bool ContainsPositionWords(string input)
        {
            string[] posWords = { "那里", "这里", "房间", "外面", "里面", "旁边", "附近", 
                                 "here", "there", "room", "outside", "inside", "near" };
            return posWords.Any(w => input.Contains(w));
        }

        private static bool ContainsHostileWords(string input)
        {
            return ObjectKeywords.TryGetValue("敌人", out var keywords) && 
                   keywords.Any(w => input.Contains(w));
        }

        private static bool ContainsCraftWords(string input)
        {
            string[] craftWords = { "衣服", "武器", "装备", "药", "食物", "家具", "墙", "门",
                                   "cloth", "weapon", "armor", "medicine", "food", "furniture" };
            return craftWords.Any(w => input.Contains(w));
        }
    }

    /// <summary>
    /// 解析后的指令数据
    /// </summary>
    public class ParsedCommand
    {
        public CommandParser.CommandType Type { get; set; }
        public Thing Target { get; set; }
        public ThingDef ItemToCraft { get; set; }
        public int Count { get; set; } = 1;
        public string OriginalText { get; set; }
        public IntVec3 TargetLocation { get; set; }
    }
}
