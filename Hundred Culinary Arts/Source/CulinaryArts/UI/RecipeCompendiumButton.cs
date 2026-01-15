using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;

namespace CulinaryArts
{
    /// <summary>
    /// 图鉴按钮纹理
    /// </summary>
    [StaticConstructorOnStartup]
    public static class RecipeCompendiumTextures
    {
        public static readonly Texture2D ButtonIcon;

        static RecipeCompendiumTextures()
        {
            // 加载自定义图标
            ButtonIcon = ContentFinder<Texture2D>.Get("UI/Icons/RecipeBook", false);
            if (ButtonIcon == null)
            {
                Log.Warning("[厨间百艺] 无法加载按钮图标 UI/Icons/RecipeBook，使用备用图标");
                ButtonIcon = TexButton.ShowLearningHelper;
            }
        }
    }

    /// <summary>
    /// 在PlaySettings栏位（右下角美观度/污染度开关栏）添加食谱图鉴按钮
    /// </summary>
    [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
    public static class Patch_PlaySettings_AddCompendiumButton
    {
        [HarmonyPostfix]
        public static void Postfix(WidgetRow row, bool worldView)
        {
            if (worldView) return; // 不在世界视图显示
            if (row == null) return;

            // 添加图鉴按钮到PlaySettings栏位
            string tooltip = LanguageHelper.IsChinese() 
                ? "食谱图鉴\n点击查看已解锁的特殊食谱" 
                : "Recipe Book\nClick to view unlocked special recipes";
            
            if (row.ButtonIcon(RecipeCompendiumTextures.ButtonIcon, tooltip))
            {
                Find.WindowStack.Add(new Window_RecipeCompendium());
            }
        }
    }
}