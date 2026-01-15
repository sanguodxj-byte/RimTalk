using UnityEngine;
using Verse;

namespace DiningEtiquette
{
    public class DiningEtiquetteSettings : ModSettings
    {
        public float highSaturationThreshold = 0.95f; // 阈值 A
        public float mealHungerThreshold = 0.5f;      // 阈值 B
        public bool allowSocializeAtHighSaturation = false; // 是否允许饱腹社交

        public override void ExposeData()
        {
            Scribe_Values.Look(ref highSaturationThreshold, "highSaturationThreshold", 0.95f);
            Scribe_Values.Look(ref mealHungerThreshold, "mealHungerThreshold", 0.5f);
            Scribe_Values.Look(ref allowSocializeAtHighSaturation, "allowSocializeAtHighSaturation", false);
        }
    }

    public class DiningEtiquetteMod : Mod
    {
        public static DiningEtiquetteSettings settings;

        public DiningEtiquetteMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<DiningEtiquetteSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            
            // 阈值 A：高饱腹度阈值 (默认为 95%)
            listingStandard.Label($"High Saturation Threshold (Threshold A): {settings.highSaturationThreshold:P0}");
            settings.highSaturationThreshold = listingStandard.Slider(settings.highSaturationThreshold, 0.5f, 1.0f);
            listingStandard.Label("Above this hunger level, pawns will skip meal time actions (unless 'Allow Socialize' is on).");
            
            listingStandard.Gap();

            // 阈值 B：正餐阈值 (默认为 50%)
            listingStandard.Label($"Meal Hunger Threshold (Threshold B): {settings.mealHungerThreshold:P0}");
            settings.mealHungerThreshold = listingStandard.Slider(settings.mealHungerThreshold, 0.1f, 0.8f);
            listingStandard.Label("Below this hunger level, pawns will prioritize eating a proper meal.");

            listingStandard.Gap();

            // 饱腹社交开关
            listingStandard.CheckboxLabeled("Allow Socialize at High Saturation", ref settings.allowSocializeAtHighSaturation, "If enabled, pawns above Threshold A will gather at tables to socialize instead of skipping action.");

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Dining Etiquette";
        }
    }
}