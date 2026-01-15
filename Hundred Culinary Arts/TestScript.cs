using System;
using System.Collections.Generic;
using CulinaryArts;
using Verse;
using RimWorld;

namespace CulinaryArts.Test
{
    public class MockThing : Thing
    {
        public ThingDef myDef;
        public override ThingDef def => myDef;
    }

    public class TestRunner
    {
        public static void RunTest()
        {
            Log.Message("=== 开始厨间百艺菜名生成测试 ===");

            // 1. 模拟环境
            Pawn chef = new Pawn();
            chef.Name = new NameTriple("Test", "Chef", "One");
            chef.skills = new Pawn_SkillTracker(chef);
            // 模拟10级烹饪
            // 注意：这里简化处理，实际需要更复杂的Mock，或者我们直接调用 NameGenerator 的内部逻辑（如果可访问）
            // 由于 NameGenerator 依赖很多 RimWorld 内部对象，直接运行可能会报错。
            // 我们主要测试 NameGenerator.AssembleDishName 和 IngredientDatabase.GetForm

            // 2. 准备数据
            string agaveDef = "RawAgave"; // 龙舌兰 (有映射)
            string snakeMeatDef = "Meat_Snake"; // 蛇肉 (无映射，应该回退)

            // 模拟 DefDatabase (这在非游戏环境下很难)
            // 所以我们只能在游戏内运行这个测试，或者模拟 IngredientDatabase 的行为。
            // 鉴于我们无法在当前环境运行游戏，我们将通过代码逻辑推演结果。
            
            Log.Message("测试场景：10级厨师，材料：龙舌兰 + 蛇肉，制作精致食物");
            
            // 模拟随机数种子
            int[] seeds = new int[] { 1001, 2002, 3003, 4004, 5005 };
            
            for (int i = 0; i < 5; i++)
            {
                int seed = seeds[i];
                Random rand = new Random(seed);
                
                // 模拟风格 (50% 中/西)
                CuisineStyle style = (CuisineStyle)rand.Next(2);
                
                // 模拟前缀 (10级厨师: 2%负面, 95%无, 3%正面)
                var (prefix, mood) = PrefixDatabase.GeneratePrefix(10, style, rand);
                
                // 模拟技法 (10级: HomeCooking(1), MealFine: 偏好 HomeCooking-Gourmet)
                // 10级最高 HomeCooking。MealFine 偏好 HomeCooking-Gourmet。
                // 交集是 HomeCooking。所以应该主要出 HomeCooking 技法。
                string technique = TechniqueDatabase.GetTechnique(10, style, null, rand); // MealFine 模拟 null 传入，或者我们需要模拟 ThingDef
                
                // 模拟食材形态
                // 龙舌兰 -> Agave Cubes / Agave Chunks
                // 蛇肉 -> 应该触发回退 -> "Snake Meat" (假设Label是这个)
                string form1 = IngredientDatabase.GetForm(agaveDef, style, rand);
                
                // 模拟蛇肉回退逻辑 (IngredientDatabase.GetForm 内部)
                // 假设数据库没有 Meat_Snake，也没有 Meat 模糊匹配 (其实有 Meat 模糊匹配)
                // Wait, "Meat_Snake" contains "Meat", so it might match "Meat" entry!
                // IngredientDatabase has ["Meat"] entry.
                // "Meat_Snake" contains "Meat". So it will use generic meat forms (Slices, Steak, etc.)
                // 除非用户希望它显示 "Snake Meat"。
                // 当前逻辑：模糊匹配优先于回退。
                string form2 = IngredientDatabase.GetForm(snakeMeatDef, style, rand);
                
                // 组装
                // 中文环境模拟
                string resultCN = SimulateAssemble(prefix, technique, new List<string>{form1, form2}, style, true);
                
                Log.Message($"[{i+1}] 风格: {style}, 前缀: {prefix}, 技法: {technique}, 食材1: {form1}, 食材2: {form2} => {resultCN}");
            }
        }
        
        static string SimulateAssemble(string prefix, string technique, List<string> forms, CuisineStyle style, bool isChinese)
        {
             // 简化的组装逻辑复制
             string dishName = "";
             if (style == CuisineStyle.Chinese)
             {
                 dishName = $"{technique}{forms[0]}配{forms[1]}";
                 if (!string.IsNullOrEmpty(prefix)) dishName = prefix + dishName;
             }
             else
             {
                 // 英文语序
                 dishName = $"{technique} {forms[0]} with {forms[1]}"; // 简化连接词
                 if (!string.IsNullOrEmpty(prefix)) dishName = $"{prefix} {dishName}";
             }
             return dishName;
        }
    }
}