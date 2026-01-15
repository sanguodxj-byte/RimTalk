using System;
using Verse;
using RimWorld;

namespace SuperbRecruitment
{
    /// <summary>
    /// 说服追踪Hediff，用于存储访客的说服进度
    /// </summary>
    public class Hediff_PersuasionTracking : HediffWithComps
    {
        // 说服值 (0.0 - 1.0)，对应0-100%
        private float persuasionValue;
        
        // 玩家剩余影响次数
        private int playerAttemptsRemaining = 3;
        
        // 其他殖民者剩余影响次数（每个殖民者）
        private int colonistAttemptsRemaining = 1;
        
        // 已经尝试说服的殖民者列表
        private System.Collections.Generic.HashSet<int> colonistsWhoTried = new System.Collections.Generic.HashSet<int>();

        public float PersuasionValue => persuasionValue;
        public int PlayerAttemptsRemaining => playerAttemptsRemaining;
        public int ColonistAttemptsRemaining => colonistAttemptsRemaining;

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            InitializePersuasionValue();
        }

        /// <summary>
        /// 初始化说服值，使用正态分布，以50%为中心
        /// </summary>
        private void InitializePersuasionValue()
        {
            // 使用Box-Muller变换生成正态分布随机数
            // 均值=0.5, 标准差=0.15 (这样大约95%的值在0.2-0.8之间)
            float u1 = Rand.Value;
            float u2 = Rand.Value;
            
            float randStdNormal = (float)(Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2));
            float mean = 0.5f;
            float stdDev = 0.15f;
            
            persuasionValue = mean + stdDev * randStdNormal;
            
            // 限制在0-0.95之间（初始值不要太高，给玩家留空间）
            persuasionValue = Math.Max(0f, Math.Min(0.95f, persuasionValue));
            
            Log.Message($"[Superb Recruitment] 初始化访客 {pawn.LabelShort} 的说服值: {persuasionValue:P0}");
        }

        /// <summary>
        /// 检查殖民者是否可以尝试说服
        /// </summary>
        public bool CanColonistTryPersuade(int colonistId)
        {
            return !colonistsWhoTried.Contains(colonistId);
        }

        /// <summary>
        /// 调整说服值
        /// </summary>
        public void AdjustPersuasion(float delta, Pawn persuader, bool isPlayer)
        {
            if (isPlayer)
            {
                if (playerAttemptsRemaining <= 0)
                {
                    Messages.Message("SuperbRecruitment_PlayerNoMoreAttempts".Translate(), MessageTypeDefOf.RejectInput);
                    return;
                }
                playerAttemptsRemaining--;
            }
            else
            {
                int persuaderId = persuader.thingIDNumber;
                if (colonistsWhoTried.Contains(persuaderId))
                {
                    Messages.Message("SuperbRecruitment_ColonistAlreadyTried".Translate(persuader.LabelShort), MessageTypeDefOf.RejectInput);
                    return;
                }
                colonistsWhoTried.Add(persuaderId);
            }

            float oldValue = persuasionValue;
            persuasionValue += delta;
            persuasionValue = Math.Max(0f, Math.Min(1.0f, persuasionValue)); // 改为可以达到1.0
            
            Log.Message($"[Superb Recruitment] {persuader?.LabelShort ?? "Player"} 对 {pawn.LabelShort} 的说服效果: {delta:+0.00;-0.00} (从 {oldValue:P0} 到 {persuasionValue:P0})");
            
            // 检查是否达到100%，自动招募
            if (persuasionValue >= 1.0f && oldValue < 1.0f)
            {
                AutoRecruit(persuader);
            }
        }

        /// <summary>
        /// 自动招募（说服值达到100%）
        /// </summary>
        private void AutoRecruit(Pawn persuader)
        {
            if (pawn == null || pawn.Faction == Faction.OfPlayer)
                return;

            // 添加正面想法
            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(
                DefDatabase<ThoughtDef>.GetNamed("SuperbRecruitment_ConvinvedToJoin", false)
            );

            Faction oldFaction = pawn.Faction;
            pawn.SetFaction(Faction.OfPlayer);

            // 如果在监狱中，释放
            if (pawn.guest != null)
            {
                pawn.guest.SetGuestStatus(null);
            }

            // 发送成功信件
            Find.LetterStack.ReceiveLetter(
                "SuperbRecruitment_AutoRecruitSuccessLabel".Translate(),
                "SuperbRecruitment_AutoRecruitSuccessDesc".Translate(
                    pawn.LabelShort,
                    persuader?.LabelShort ?? "未知",
                    oldFaction?.Name ?? "Unknown"
                ),
                LetterDefOf.PositiveEvent,
                pawn
            );

            // 移除说服追踪Hediff
            pawn.health.RemoveHediff(this);

            Log.Message($"[Superb Recruitment] 自动招募成功: {pawn.LabelShort} (说服值达到100%)");
        }

        /// <summary>
        /// 计算招募成功概率
        /// 设计目标：10次招募，综合成功率30%
        /// 使用公式: 成功率 = persuasionValue^2.5
        /// 这样平均说服值50%时，10次尝试的总成功率约为30%
        /// </summary>
        public float CalculateSuccessChance()
        {
            // 使用幂函数使曲线更陡峭，低说服值时成功率很低，高说服值时成功率快速提升
            float successChance = (float)Math.Pow(persuasionValue, 2.5);
            return successChance;
        }

        /// <summary>
        /// 尝试招募
        /// </summary>
        public bool TryRecruit()
        {
            float successChance = CalculateSuccessChance();
            bool success = Rand.Chance(successChance);
            
            Log.Message($"[Superb Recruitment] 招募尝试 {pawn.LabelShort}: 说服值={persuasionValue:P0}, 成功率={successChance:P0}, 结果={success}");
            
            return success;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref persuasionValue, "persuasionValue", 0.5f);
            Scribe_Values.Look(ref playerAttemptsRemaining, "playerAttemptsRemaining", 3);
            Scribe_Values.Look(ref colonistAttemptsRemaining, "colonistAttemptsRemaining", 1);
            Scribe_Collections.Look(ref colonistsWhoTried, "colonistsWhoTried", LookMode.Value);
            
            if (Scribe.mode == LoadSaveMode.PostLoadInit && colonistsWhoTried == null)
            {
                colonistsWhoTried = new System.Collections.Generic.HashSet<int>();
            }
        }
    }
}
