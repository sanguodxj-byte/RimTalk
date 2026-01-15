using RimWorld;

namespace CulinaryArts
{
    /// <summary>
    /// ThoughtDef引用工具类
    /// 用于方便地引用模组定义的ThoughtDef
    /// </summary>
    [DefOf]
    public static class ThoughtDefOf
    {
        public static ThoughtDef CulinaryArts_AteTerribleMeal;
        public static ThoughtDef CulinaryArts_AteGoodMeal;
        public static ThoughtDef CulinaryArts_AteLegendaryMeal;
        public static ThoughtDef CulinaryArts_AteNormalMeal;

        static ThoughtDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThoughtDefOf));
        }
    }
}