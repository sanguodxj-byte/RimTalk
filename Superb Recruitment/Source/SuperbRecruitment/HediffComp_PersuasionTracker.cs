using Verse;

namespace SuperbRecruitment
{
    /// <summary>
    /// 说服追踪器属性定义
    /// </summary>
    public class HediffCompProperties_PersuasionTracker : HediffCompProperties
    {
        public HediffCompProperties_PersuasionTracker()
        {
            compClass = typeof(HediffComp_PersuasionTracker);
        }
    }

    /// <summary>
    /// 说服追踪器组件
    /// </summary>
    public class HediffComp_PersuasionTracker : HediffComp
    {
        public HediffCompProperties_PersuasionTracker Props => (HediffCompProperties_PersuasionTracker)props;

        public Hediff_PersuasionTracking ParentHediff => parent as Hediff_PersuasionTracking;

        public override string CompLabelInBracketsExtra
        {
            get
            {
                if (ParentHediff != null)
                {
                    // 不显示具体数值，保持隐藏
                    return "?%";
                }
                return base.CompLabelInBracketsExtra;
            }
        }

        public override string CompTipStringExtra
        {
            get
            {
                if (ParentHediff != null)
                {
                    return "SuperbRecruitment_PersuasionTooltip".Translate(
                        ParentHediff.PlayerAttemptsRemaining,
                        ParentHediff.ColonistAttemptsRemaining
                    );
                }
                return base.CompTipStringExtra;
            }
        }
    }
}
