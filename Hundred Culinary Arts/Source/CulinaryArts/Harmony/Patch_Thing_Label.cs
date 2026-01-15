using HarmonyLib;
using Verse;

namespace CulinaryArts.Harmony
{
    /// <summary>
    /// 修改Thing.Label的显示逻辑
    /// 堆叠时显示原名，单品时显示自定义名
    /// </summary>
    [HarmonyPatch(typeof(Thing), "Label", MethodType.Getter)]
    public static class Patch_Thing_Label
    {
        [HarmonyPostfix]
        public static void Postfix(Thing __instance, ref string __result)
        {
            try
            {
                // 检查是否有CompNamedMeal组件
                var comp = __instance?.TryGetComp<CompNamedMeal>();
                if (comp == null || string.IsNullOrEmpty(comp.CustomName))
                    return;

                // 使用NameGenerator的智能显示逻辑
                __result = NameGenerator.GetDisplayLabel(__instance, __result);
            }
            catch (System.Exception ex)
            {
                Log.Error($"[CulinaryArts] Patch_Thing_Label错误: {ex}");
            }
        }
    }
}