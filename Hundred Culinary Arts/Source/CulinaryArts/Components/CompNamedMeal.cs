using Verse;

namespace CulinaryArts
{
    /// <summary>
    /// 为食物存储自定义名称和相关数据的组件
    /// v3.0 简化版：纯数据存储，无复杂堆叠逻辑
    /// </summary>
    public class CompNamedMeal : ThingComp
    {
        private string customName;
        private int moodOffset;
        private int generationSeed;
        private string cuisineStyle;

        /// <summary>
        /// 自定义菜名
        /// </summary>
        public string CustomName => customName;

        /// <summary>
        /// 心情加成值 (-3, 0, 3, 8)
        /// </summary>
        public int MoodOffset => moodOffset;

        /// <summary>
        /// 生成时使用的种子（用于调试）
        /// </summary>
        public int GenerationSeed => generationSeed;

        /// <summary>
        /// 菜系风格（Chinese/Western）
        /// </summary>
        public string CuisineStyle => cuisineStyle;

        /// <summary>
        /// 设置所有数据
        /// </summary>
        public void SetData(string name, int mood, int seed, CulinaryArts.CuisineStyle style)
        {
            this.customName = name;
            this.moodOffset = mood;
            this.generationSeed = seed;
            this.cuisineStyle = style.ToString();
        }

        /// <summary>
        /// 存档保存/加载
        /// </summary>
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref customName, "customName");
            Scribe_Values.Look(ref moodOffset, "moodOffset", 0);
            Scribe_Values.Look(ref generationSeed, "generationSeed", 0);
            Scribe_Values.Look(ref cuisineStyle, "cuisineStyle", "Chinese");
        }

        /// <summary>
        /// 拆分堆叠时复制数据
        /// </summary>
        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            if (piece is ThingWithComps pieceWithComps)
            {
                var otherComp = pieceWithComps.GetComp<CompNamedMeal>();
                if (otherComp != null)
                {
                    CulinaryArts.CuisineStyle style = ParseStyle(this.cuisineStyle);
                    otherComp.SetData(customName, moodOffset, generationSeed, style);
                }
            }
        }

        /// <summary>
        /// 堆叠合并前的处理
        /// 如果当前没有名字，从被吸收的堆叠中复制名称
        /// </summary>
        public override void PreAbsorbStack(Thing otherStack, int count)
        {
            base.PreAbsorbStack(otherStack, count);

            if (string.IsNullOrEmpty(customName) && otherStack is ThingWithComps otherWithComps)
            {
                var otherComp = otherWithComps.GetComp<CompNamedMeal>();
                if (otherComp != null && !string.IsNullOrEmpty(otherComp.CustomName))
                {
                    CulinaryArts.CuisineStyle style = ParseStyle(otherComp.CuisineStyle);
                    SetData(otherComp.CustomName, otherComp.MoodOffset, otherComp.GenerationSeed, style);
                }
            }
        }

        /// <summary>
        /// 调试信息（仅在开发者模式下显示）
        /// </summary>
        public override string CompInspectStringExtra()
        {
            if (Prefs.DevMode && !string.IsNullOrEmpty(customName))
            {
                return $"[Debug] {cuisineStyle} | Mood: {moodOffset}";
            }
            return base.CompInspectStringExtra();
        }

        /// <summary>
        /// 解析风格枚举
        /// </summary>
        private CulinaryArts.CuisineStyle ParseStyle(string styleStr)
        {
            if (string.IsNullOrEmpty(styleStr)) return CulinaryArts.CuisineStyle.Chinese;
            try
            {
                return (CulinaryArts.CuisineStyle)System.Enum.Parse(typeof(CulinaryArts.CuisineStyle), styleStr);
            }
            catch
            {
                return CulinaryArts.CuisineStyle.Chinese;
            }
        }
    }
}