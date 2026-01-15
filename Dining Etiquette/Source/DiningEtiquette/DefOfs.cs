using RimWorld;
using Verse;

namespace DiningEtiquette
{
    [DefOf]
    public static class DE_DefOf
    {
        public static TimeAssignmentDef DiningEtiquette_MealTime;
        public static ThingDef DE_HotDrink;
        public static ThingDef DE_ColdDrink;
        public static ThingDef DE_SnackSimple;
        public static ThingDef DE_SnackFine;
        public static ThingDef DE_SnackLavish;

        static DE_DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DE_DefOf));
        }
    }
}