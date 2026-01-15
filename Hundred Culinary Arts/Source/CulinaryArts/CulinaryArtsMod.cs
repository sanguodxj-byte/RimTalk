using Verse;
using HarmonyLib;

namespace CulinaryArts
{
    /// <summary>
    /// 模组主类，负责初始化Harmony补丁
    /// </summary>
    [StaticConstructorOnStartup]
    public static class CulinaryArtsMod
    {
        static CulinaryArtsMod()
        {
            try
            {
                // 创建Harmony实例并应用所有补丁
                var harmony = new HarmonyLib.Harmony("culinaryarts.hundredarts");
                harmony.PatchAll();

                // 联动 Dining Etiquette
                if (ModLister.GetActiveModWithIdentifier("DiningEtiquette.Mod") != null)
                {
                    Log.Message("[厨间百艺] 检测到 Dining Etiquette，正在应用联动补丁...");
                    ApplyDiningEtiquetteIntegration();
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"[厨间百艺] 初始化失败: {ex}");
            }
        }

        private static void ApplyDiningEtiquetteIntegration()
        {
            // 在游戏加载完毕后，使用 Harmony Patch Thing.Label 和 Description 可能更合适，
            // 但这里直接修改 Def 也是可行的，只要在生成 Def 之后。
            // 为了支持动态原料命名，我们需要 Patch 物品生成时的 Label。
            
            // 此处保留基础重命名，动态命名逻辑将由 DiningEtiquetteMod 内部处理（如果需要），
            // 或者我们在这里实现一个 Comp 或 Patch。
            // 鉴于复杂性，我们先实现静态的基础重命名，确保“玄米茶”等概念存在。
            
            RenameThing("DE_ColdDrink", "特调冰饮", "一杯加了冰块的清爽饮料。它可能是冰镇玄米茶、鲜榨果汁或冰可可，具体取决于所用的素食原料。");
            RenameThing("DE_HotDrink", "特调热饮", "一杯热气腾腾的饮料。它可能是热玄米茶、热可可或热咖啡，具体取决于所用的素食原料。");
            RenameThing("DE_SnackSimple", "传统糕点", "朴实无华的传统糕点。虽然外表粗糙，但用料扎实，能带来那种令人怀念的家乡味道。");
            RenameThing("DE_SnackFine", "果酱塔", "精致的小点心，酥脆的塔皮包裹着酸甜适口的果酱。是下午茶时间的完美伴侣。");
            RenameThing("DE_SnackLavish", "宫廷御点", "极其奢华的宫廷式点心，制作工艺繁复，造型美轮美奂。每一口都是味蕾的极致享受。");
        }

        private static void RenameThing(string defName, string label, string desc)
        {
            ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
            if (def != null)
            {
                def.label = label;
                def.description = desc;
            }
        }
    }
}