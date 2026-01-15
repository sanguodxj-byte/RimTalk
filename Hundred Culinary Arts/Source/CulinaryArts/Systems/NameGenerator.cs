using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace CulinaryArts
{
    /// <summary>
    /// 核心菜名生成器 v3.2
    /// 整合环境灵感、图鉴解锁、仿制能力、变种菜名
    /// </summary>
    public static class NameGenerator
    {
        /// <summary>
        /// 为食物生成自定义名称
        /// </summary>
        public static void GenerateMealName(Thing meal, Pawn chef, List<Thing> ingredients)
        {
            try
            {
                if (meal == null || chef == null || ingredients == null || ingredients.Count == 0)
                    return;

                var comp = meal.TryGetComp<CompNamedMeal>();
                if (comp == null)
                    return;

                // 1. 生成时间种子和随机数
                int seed = TimeSeedGenerator.GenerateSeed(chef, meal.def, ingredients);
                Random rand = new Random(seed);

                // 2. 确定菜系风格
                CuisineStyle style = (CuisineStyle)rand.Next(2);

                // 3. 获取厨师的烹饪技能等级
                int skillLevel = chef.skills?.GetSkill(SkillDefOf.Cooking)?.Level ?? 0;

                // 4. 生成前缀和心情效果
                var (prefix, prefixMood) = PrefixDatabase.GeneratePrefix(skillLevel, style, rand);
                
                // 5. 计算基础心情加成（根据食物类型）
                int baseMood = GetBaseMoodFromMealType(meal.def.defName);
                int mood = baseMood + prefixMood;

                // 5. 获取图鉴数据
                var data = Find.World?.GetComponent<CulinaryData>();
                int unlockedCount = data?.UnlockedCount ?? 0;

                // 6. 计算特殊食谱触发概率
                // 基础10%，每解锁1个+1%，最高80%
                float triggerChance = Math.Min(0.10f + (unlockedCount * 0.01f), 0.80f);

                // 7. 判断是否有仿制能力（解锁30个配方后）
                bool canImitate = unlockedCount >= 30;

                // 8. 构建可用食材类别池（包括环境灵感）
                HashSet<string> ingredientCategories = new HashSet<string>();
                
                // 添加实际使用的食材
                foreach (var ing in ingredients)
                {
                    string category = SpecialRecipeDatabase.NormalizeToCategory(ing.def.defName);
                    ingredientCategories.Add(category);
                }

                // 环境灵感：扫描周围5格内的食材
                if (CulinaryArtsSettings.SimulateDiversity && chef.Map != null)
                {
                    var cells = GenRadial.RadialCellsAround(chef.Position, 5f, true);
                    foreach (var cell in cells)
                    {
                        if (!cell.InBounds(chef.Map)) continue;
                        var things = cell.GetThingList(chef.Map);
                        foreach (var t in things)
                        {
                            if (t.def.IsIngestible && !t.def.IsCorpse && t.HitPoints > 0)
                            {
                                string category = SpecialRecipeDatabase.NormalizeToCategory(t.def.defName);
                                ingredientCategories.Add(category);
                            }
                        }
                    }
                }

                string dishName;
                bool hitSpecial = false;
                BilingualString specialName = default;
                string recipeKey = "";
                int matchedCount = 0;

                // 构建实际使用的食材类别池（不包括环境灵感，用于解锁判断）
                HashSet<string> actualIngredientCategories = new HashSet<string>();
                foreach (var ing in ingredients)
                {
                    string category = SpecialRecipeDatabase.NormalizeToCategory(ing.def.defName);
                    actualIngredientCategories.Add(category);
                }

                // 9. 尝试触发特殊食谱
                if (rand.NextDouble() < triggerChance)
                {
                    // 使用包含环境灵感的食材池来触发菜名和解锁
                    // 环境灵感和仿制能力都可以用于解锁
                    if (SpecialRecipeDatabase.TryGetSpecialRecipeName(ingredientCategories, canImitate, out specialName, out recipeKey, out matchedCount))
                    {
                        hitSpecial = true;
                        
                        // 使用包含环境灵感的食材池进行解锁判断，支持仿制能力
                        bool canUnlock = SpecialRecipeDatabase.CheckExactMatch(ingredientCategories, recipeKey, canImitate);
                        
                        if (CulinaryArtsSettings.ShowDebugLog)
                        {
                            Log.Message($"[厨间百艺] 触发食谱: {recipeKey}, 可用食材(含环境灵感): [{string.Join(", ", ingredientCategories)}], 可解锁: {canUnlock}");
                        }
                        
                        if (canUnlock)
                        {
                            data?.Unlock(recipeKey);
                        }
                    }
                }

                // 10. 生成最终菜名
                string mealDef = meal.def.defName;
                if (hitSpecial)
                {
                    string finalDishName = specialName.ToString();

                    // 获取排序后的实际食材列表（不包括环境灵感）
                    var sortedIngredients = GetSortedIngredientDefs(ingredients);

                    // 检查是否有额外食材
                    if (sortedIngredients.Count > matchedCount)
                    {
                        var extraIngs = sortedIngredients.Skip(matchedCount).Take(1).ToList();

                        if (extraIngs.Count > 0)
                        {
                            string extraName = IngredientDatabase.GetForm(extraIngs[0].defName, style, rand);

                            if (LanguageHelper.IsChinese())
                            {
                                finalDishName = $"{finalDishName}配{extraName}";
                            }
                            else
                            {
                                finalDishName = $"{finalDishName} with {extraName}";
                            }
                        }
                    }

                    // 加上前缀
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        dishName = LanguageHelper.IsChinese() ? $"{prefix}{finalDishName}" : $"{prefix} {finalDishName}";
                    }
                    else
                    {
                        dishName = finalDishName;
                    }
                }
                else if (IsVanillaMeal(mealDef))
                {
                    // 原版餐点：程序化生成
                    string technique = TechniqueDatabase.GetTechnique(skillLevel, style, meal.def, rand);
                    List<string> forms = GetIngredientForms(ingredients, style, rand);
                    dishName = AssembleDishName(prefix, technique, forms, style);
                }
                else
                {
                    // 其他Mod食物：保留原名，仅添加前缀
                    string originalLabel = meal.def.label;
                    dishName = !string.IsNullOrEmpty(prefix)
                        ? (LanguageHelper.IsChinese() ? $"{prefix}{originalLabel}" : $"{prefix} {originalLabel}")
                        : originalLabel;
                }

                // 11. 保存到组件
                comp.SetData(dishName, mood, seed, style);
            }
            catch (Exception ex)
            {
                Log.Error($"[CulinaryArts] Error: {ex}");
            }
        }

        /// <summary>
        /// 判断是否是原版餐点
        /// </summary>
        private static bool IsVanillaMeal(string defName)
        {
            return defName == "MealSimple" || defName.StartsWith("MealSimple_") ||
                   defName == "MealFine" || defName.StartsWith("MealFine_") ||
                   defName == "MealLavish" || defName.StartsWith("MealLavish_") ||
                   defName == "MealSurvivalPack" || defName.StartsWith("MealSurvival");
        }

        /// <summary>
        /// 根据食物类型获取基础心情加成（等同原版）
        /// 简单餐 +5，精致餐 +10，奢侈餐 +12
        /// </summary>
        private static int GetBaseMoodFromMealType(string defName)
        {
            if (defName.Contains("Lavish")) return 12;  // 奢侈餐
            if (defName.Contains("Fine")) return 10;    // 精致餐
            if (defName.Contains("Simple")) return 5;   // 简单餐
            return 0; // 其他餐点
        }

        /// <summary>
        /// 获取排序后的食材定义列表（肉类优先）
        /// </summary>
        private static List<ThingDef> GetSortedIngredientDefs(List<Thing> ingredients)
        {
            return ingredients
                .OrderByDescending(ing =>
                {
                    if (ing.def.IsMeat) return 3;
                    if (ing.def.ingestible?.foodType.HasFlag(FoodTypeFlags.AnimalProduct) ?? false) return 2;
                    if (ing.def.ingestible?.foodType.HasFlag(FoodTypeFlags.VegetableOrFruit) ?? false) return 1;
                    return 0;
                })
                .Select(ing => ing.def)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// 获取食材的烹饪形态列表
        /// </summary>
        private static List<string> GetIngredientForms(List<Thing> ingredients, CuisineStyle style, Random rand)
        {
            var forms = new List<string>();

            var sortedDefs = GetSortedIngredientDefs(ingredients);
            int count = Math.Min(sortedDefs.Count, 2);

            for (int i = 0; i < count; i++)
            {
                string form = IngredientDatabase.GetForm(sortedDefs[i].defName, style, rand);
                forms.Add(form);
            }

            if (forms.Count == 0)
            {
                forms.Add(style == CuisineStyle.Chinese ? "混合料理" : "Mixed Dish");
            }

            return forms;
        }

        /// <summary>
        /// 组装完整菜名
        /// </summary>
        private static string AssembleDishName(string prefix, string technique, List<string> forms, CuisineStyle style)
        {
            if (forms.Count == 0)
            {
                return style == CuisineStyle.Chinese ? "未知料理" : "Unknown Dish";
            }

            string dishName;
            bool isChineseLang = LanguageHelper.IsChinese();

            if (style == CuisineStyle.Chinese)
            {
                if (isChineseLang)
                {
                    dishName = forms.Count == 1 ? $"{technique}{forms[0]}" : $"{technique}{forms[0]}配{forms[1]}";
                }
                else
                {
                    dishName = forms.Count == 1 ? $"{technique} {forms[0]}" : $"{technique} {forms[0]} with {forms[1]}";
                }

                if (!string.IsNullOrEmpty(prefix))
                {
                    dishName = isChineseLang ? prefix + dishName : $"{prefix} {dishName}";
                }
            }
            else
            {
                if (forms.Count == 1)
                {
                    dishName = isChineseLang ? $"{technique}{forms[0]}" : $"{technique} {forms[0]}";
                }
                else
                {
                    var connectors = new[]
                    {
                        new BilingualString("配", "with"),
                        new BilingualString("佐", "served with"),
                        new BilingualString("搭配", "paired with")
                    };
                    int hash = (prefix + technique + forms[0] + forms[1]).GetHashCode();
                    var connector = connectors[Math.Abs(hash) % connectors.Length];

                    if (isChineseLang)
                    {
                        dishName = $"{technique}{forms[0]}{connector.CN}{forms[1]}";
                    }
                    else
                    {
                        dishName = $"{technique} {forms[0]} {connector.EN} {forms[1]}";
                    }
                }

                if (!string.IsNullOrEmpty(prefix))
                {
                    dishName = isChineseLang ? prefix + dishName : $"{prefix} {dishName}";
                }
            }

            return dishName;
        }

        /// <summary>
        /// 获取食物的显示名称（考虑堆叠情况）
        /// </summary>
        public static string GetDisplayLabel(Thing meal, string baseLabel)
        {
            var comp = meal?.TryGetComp<CompNamedMeal>();
            if (comp == null || string.IsNullOrEmpty(comp.CustomName))
            {
                return baseLabel;
            }

            bool shouldShowCustomName = meal.stackCount == 1;

            return shouldShowCustomName ? comp.CustomName : baseLabel;
        }
    }
}