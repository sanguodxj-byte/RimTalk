using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CulinaryArts.Harmony
{
    /// <summary>
    /// 拦截食物生成，为新制作的食物添加自定义名称
    /// </summary>
    [HarmonyPatch(typeof(GenRecipe), "MakeRecipeProducts")]
    public static class Patch_GenRecipe_MakeRecipeProducts
    {
        [HarmonyPostfix]
        public static void Postfix(
            RecipeDef recipeDef,
            Pawn worker,
            List<Thing> ingredients,
            Thing dominantIngredient,
            IBillGiver billGiver,
            ref IEnumerable<Thing> __result)
        {
            try
            {
                // 只处理食物配方
                if (recipeDef?.ProducedThingDef == null)
                    return;

                // 检查是否是可食用物品
                if (!recipeDef.ProducedThingDef.IsIngestible)
                    return;

                // 必须有工作的小人和食材
                if (worker == null || ingredients == null || ingredients.Count == 0)
                    return;

                // 关键修复：将懒加载的IEnumerable转换为List，避免迭代器陷阱
                var productList = __result.ToList();

                // 遍历List进行修改
                foreach (var product in productList)
                {
                    // 检查是否有CompNamedMeal组件
                    var comp = product?.TryGetComp<CompNamedMeal>();
                    if (comp != null)
                    {
                        // 生成自定义名称
                        NameGenerator.GenerateMealName(product, worker, ingredients);
                    }
                }

                // 重要：将修改后的List赋值回返回值
                __result = productList;
            }
            catch (System.Exception ex)
            {
                Log.Error($"[CulinaryArts] Patch_GenRecipe错误: {ex}");
            }
        }
    }
}