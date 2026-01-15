using RimWorld;
using Verse;

namespace CulinaryArts
{
    /// <summary>
    /// 自定义Thought_Memory，用于存储具体的菜名和动态心情值
    /// 这样RimTalk等模组就能读取到小人吃了什么
    /// </summary>
    public class Thought_Memory_NamedMeal : Thought_Memory
    {
        public string dishName;
        public int customMoodOffset;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref dishName, "dishName");
            Scribe_Values.Look(ref customMoodOffset, "customMoodOffset", 0);
        }

        /// <summary>
        /// 覆盖心情效果，使用自定义值（如果设置了）
        /// </summary>
        public override float MoodOffset()
        {
            if (customMoodOffset != 0)
            {
                return customMoodOffset;
            }
            return base.MoodOffset();
        }

        /// <summary>
        /// 确保只有菜名相同时才合并堆叠
        /// </summary>
        public override bool GroupsWith(Thought other)
        {
            var otherNamed = other as Thought_Memory_NamedMeal;
            if (otherNamed == null) return base.GroupsWith(other);
            
            return base.GroupsWith(other) && this.dishName == otherNamed.dishName;
        }

        // 重写 Label 以包含菜名
        // 格式：吃了美味的食物 (红烧肉)
        public override string LabelCap
        {
            get
            {
                if (!string.IsNullOrEmpty(dishName))
                {
                    return $"{base.LabelCap} ({dishName})";
                }
                return base.LabelCap;
            }
        }

        // 重写 Description 以包含菜名
        public override string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(dishName))
                {
                    return $"{base.Description}\n\n{("CulinaryArts.DishWas".Translate(dishName))}";
                }
                return base.Description;
            }
        }
    }
}