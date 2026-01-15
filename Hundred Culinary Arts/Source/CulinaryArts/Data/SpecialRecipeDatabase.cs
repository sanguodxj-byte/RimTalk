using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CulinaryArts
{
    /// <summary>
    /// 特殊配方数据库
    /// 从 XML 定义中读取特殊食谱，支持模糊匹配、环境灵感和仿制
    /// </summary>
    [StaticConstructorOnStartup]
    public static class SpecialRecipeDatabase
    {
        private static List<SpecialRecipeDef> allRecipes;

        static SpecialRecipeDatabase()
        {
            allRecipes = DefDatabase<SpecialRecipeDef>.AllDefsListForReading;
        }

        /// <summary>
        /// 获取所有特殊食谱的总数
        /// </summary>
        public static int GetTotalCount() => allRecipes?.Count ?? 0;

        /// <summary>
        /// 尝试获取特殊配方名称（支持环境灵感和仿制）
        /// </summary>
        /// <param name="ingredientCategories">所有可用食材类别（包括环境灵感扫描到的）</param>
        /// <param name="allowImitation">是否允许仿制（缺少1种食材也能匹配）</param>
        /// <param name="name">输出的双语名称</param>
        /// <param name="outKey">输出的食谱 DefName（用于解锁图鉴）</param>
        /// <param name="matchedCount">匹配的食材数量</param>
        /// <returns>是否找到匹配</returns>
        public static bool TryGetSpecialRecipeName(HashSet<string> ingredientCategories, bool allowImitation, out BilingualString name, out string outKey, out int matchedCount)
        {
            name = default;
            outKey = null;
            matchedCount = 0;

            if (ingredientCategories == null || ingredientCategories.Count == 0) return false;
            if (allRecipes == null || allRecipes.Count == 0) return false;

            // 遍历所有配方寻找匹配（优先完全匹配，其次仿制匹配）
            SpecialRecipeDef bestMatch = null;
            int bestMatchCount = 0;
            bool isBestImitation = false;

            foreach (var recipe in allRecipes)
            {
                if (recipe.requiredIngredients == null || recipe.requiredIngredients.Count == 0)
                    continue;

                // 检查配方的每个必需食材是否在可用列表中
                int foundCount = 0;
                foreach (var req in recipe.requiredIngredients)
                {
                    bool found = ingredientCategories.Contains(req);
                    if (found) foundCount++;
                }

                int requiredCount = recipe.requiredIngredients.Count;
                int missingCount = requiredCount - foundCount;

                // 完全匹配
                if (missingCount == 0)
                {
                    // 找到完全匹配，直接返回
                    name = new BilingualString(recipe.chineseLabel ?? recipe.label, recipe.label);
                    outKey = recipe.defName;
                    matchedCount = foundCount;
                    return true;
                }
                // 仿制匹配：缺少1种食材
                else if (allowImitation && missingCount == 1 && foundCount >= 1)
                {
                    // 记录仿制匹配，但继续搜索更好的匹配
                    if (bestMatch == null || foundCount > bestMatchCount)
                    {
                        bestMatch = recipe;
                        bestMatchCount = foundCount;
                        isBestImitation = true;
                    }
                }
            }

            // 返回仿制匹配结果
            if (bestMatch != null && isBestImitation)
            {
                name = new BilingualString(bestMatch.chineseLabel ?? bestMatch.label, bestMatch.label);
                outKey = bestMatch.defName;
                matchedCount = bestMatchCount;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 将RimWorld食材DefName归一化为食谱使用的类别名
        /// </summary>
        public static string NormalizeToCategory(string defName)
        {
            string lower = defName.ToLower();

            // === 肉类 ===
            if (lower.Contains("pig") || lower.Contains("boar") || lower.Contains("pork"))
                return "Pork";

            if (lower.Contains("muffalo") || lower.Contains("bison") || lower.Contains("cow") ||
                lower.Contains("yak") || lower.Contains("elk") || lower.Contains("caribou") ||
                lower.Contains("beef") || lower.Contains("boomalope") || lower.Contains("horse") ||
                lower.Contains("donkey") || lower.Contains("deer") || lower.Contains("alpaca") ||
                lower.Contains("dromedary") || lower.Contains("ibex") || lower.Contains("gazelle"))
                return "Beef";

            if (lower.Contains("chicken") || lower.Contains("turkey") || lower.Contains("ostrich") ||
                lower.Contains("emu") || lower.Contains("cassowary") || lower.Contains("duck") ||
                lower.Contains("goose") || lower.Contains("guinea"))
                return "Chicken";

            if (lower.Contains("fish") || lower.Contains("salmon") || lower.Contains("tuna") ||
                lower.Contains("crab") || lower.Contains("lobster") || lower.Contains("shrimp"))
                return "Fish";

            if (lower.Contains("megaspider") || lower.Contains("spelopede") || lower.Contains("megascarab") ||
                lower.Contains("insect"))
                return "Insect";

            if (lower.Contains("human"))
                return "Human";

            if (lower.StartsWith("meat_") || lower.Contains("meat"))
                return "Meat";

            // === 蛋类 ===
            if (lower.Contains("egg"))
                return "Egg";

            // === 奶类 ===
            if (lower.Contains("milk"))
                return "Milk";

            // === 蔬菜/植物类 ===
            if (lower.Contains("potato"))
                return "Potato";

            if (lower.Contains("berries") || lower.Contains("berry"))
                return "Berries";

            if (lower.Contains("rice"))
                return "Rice";

            if (lower.Contains("corn"))
                return "Corn";

            if (lower.Contains("agave"))
                return "Agave";

            if (lower.Contains("fungus") || lower.Contains("mushroom"))
                return "Fungus";

            if (lower.Contains("hay") || lower.Contains("grass"))
                return "Haygrass";

            if (lower.Contains("vegetable") || lower.Contains("cabbage") || lower.Contains("carrot") ||
                lower.Contains("onion") || lower.Contains("tomato") || lower.Contains("pepper") ||
                lower.Contains("pumpkin") || lower.Contains("healroot"))
                return "Vegetable";

            return defName;
        }

        /// <summary>
        /// 检查食材是否可以解锁食谱
        /// 支持环境灵感和仿制能力解锁
        /// </summary>
        /// <param name="availableIngredients">可用食材类别（包含环境灵感）</param>
        /// <param name="recipeKey">食谱的 DefName</param>
        /// <param name="canImitate">是否有仿制能力</param>
        /// <returns>是否可以解锁</returns>
        public static bool CheckExactMatch(HashSet<string> availableIngredients, string recipeKey, bool canImitate = false)
        {
            if (availableIngredients == null || string.IsNullOrEmpty(recipeKey)) return false;
            if (allRecipes == null || allRecipes.Count == 0) return false;

            var recipe = allRecipes.Find(r => r.defName == recipeKey);
            if (recipe == null || recipe.requiredIngredients == null) return false;

            int requiredCount = recipe.requiredIngredients.Count;
            int missingCount = 0;

            // 检查食谱的每个必需食材是否在可用食材中
            foreach (var req in recipe.requiredIngredients)
            {
                if (!availableIngredients.Contains(req))
                {
                    missingCount++;
                }
            }

            // 完全匹配：所有食材都有 → 可以解锁
            if (missingCount == 0)
            {
                if (CulinaryArtsSettings.ShowDebugLog)
                {
                    Log.Message($"[厨间百艺] 解锁成功（完全匹配）：{recipeKey}，食材: [{string.Join(", ", availableIngredients)}]");
                }
                return true;
            }

            // 仿制解锁：有仿制能力且只缺少1种食材 → 可以解锁
            if (canImitate && missingCount == 1)
            {
                if (CulinaryArtsSettings.ShowDebugLog)
                {
                    Log.Message($"[厨间百艺] 解锁成功（仿制能力）：{recipeKey}，缺少1种食材，食材: [{string.Join(", ", availableIngredients)}]");
                }
                return true;
            }

            if (CulinaryArtsSettings.ShowDebugLog)
            {
                Log.Message($"[厨间百艺] 解锁失败：{recipeKey} 缺少 {missingCount} 种食材，食材: [{string.Join(", ", availableIngredients)}]");
            }
            return false;
        }

        /// <summary>
        /// 重新加载配方列表
        /// </summary>
        public static void Reload()
        {
            allRecipes = DefDatabase<SpecialRecipeDef>.AllDefsListForReading;
        }
    }
}