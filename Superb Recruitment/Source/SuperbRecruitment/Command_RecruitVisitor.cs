using UnityEngine;
using Verse;
using RimWorld;

namespace SuperbRecruitment
{
    /// <summary>
    /// 招募访客命令
    /// 在说服值足够高时显示
    /// </summary>
    public class Command_RecruitVisitor : Command_Action
    {
        private Pawn visitor;
        private Hediff_PersuasionTracking persuasionHediff;

        public Command_RecruitVisitor(Pawn visitor, Hediff_PersuasionTracking hediff)
        {
            this.visitor = visitor;
            this.persuasionHediff = hediff;
            
            defaultLabel = "SuperbRecruitment_RecruitLabel".Translate();
            defaultDesc = "SuperbRecruitment_RecruitDesc".Translate();
            icon = TexCommand.Draft;

            action = delegate
            {
                AttemptRecruitment();
            };
        }

        /// <summary>
        /// 尝试招募访客
        /// </summary>
        private void AttemptRecruitment()
        {
            if (visitor == null || persuasionHediff == null)
            {
                Log.Error("[Superb Recruitment] 招募失败：访客或Hediff为空");
                return;
            }

            float successChance = persuasionHediff.CalculateSuccessChance();

            // 显示确认对话框
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                "SuperbRecruitment_RecruitConfirm".Translate(
                    visitor.LabelShort,
                    (successChance * 100f).ToString("F0")
                ),
                delegate
                {
                    PerformRecruitment(successChance);
                },
                true,
                "SuperbRecruitment_RecruitConfirmTitle".Translate()
            ));
        }

        /// <summary>
        /// 执行招募
        /// </summary>
        private void PerformRecruitment(float successChance)
        {
            bool success = persuasionHediff.TryRecruit();

            if (success)
            {
                // 招募成功
                RecruitSuccess();
            }
            else
            {
                // 招募失败
                RecruitFailure();
            }
        }

        /// <summary>
        /// 招募成功处理
        /// </summary>
        private void RecruitSuccess()
        {
            // 添加正面想法
            visitor.needs?.mood?.thoughts?.memories?.TryGainMemory(
                DefDatabase<ThoughtDef>.GetNamed("SuperbRecruitment_ConvinvedToJoin")
            );

            // 加入玩家阵营
            if (visitor.Faction != Faction.OfPlayer)
            {
                Faction oldFaction = visitor.Faction;
                visitor.SetFaction(Faction.OfPlayer);

                // 如果访客在监狱中，释放
                if (visitor.guest != null)
                {
                    visitor.guest.SetGuestStatus(null);
                }

                // 发送成功信件
                Find.LetterStack.ReceiveLetter(
                    "SuperbRecruitment_RecruitSuccessLabel".Translate(),
                    "SuperbRecruitment_RecruitSuccessDesc".Translate(
                        visitor.LabelShort,
                        oldFaction?.Name ?? "Unknown"
                    ),
                    LetterDefOf.PositiveEvent,
                    visitor
                );

                // 播放音效
                // SoundDefOf.Quest_Accepted.PlayOneShot(SoundInfo.OnCamera());
            }

            // 移除说服追踪Hediff
            visitor.health.RemoveHediff(persuasionHediff);

            Log.Message($"[Superb Recruitment] 成功招募: {visitor.LabelShort}");
        }

        /// <summary>
        /// 招募失败处理
        /// </summary>
        private void RecruitFailure()
        {
            // 添加负面想法
            visitor.needs?.mood?.thoughts?.memories?.TryGainMemory(
                DefDatabase<ThoughtDef>.GetNamed("SuperbRecruitment_FailedPersuasion")
            );

            // 显示失败消息
            Messages.Message(
                "SuperbRecruitment_RecruitFailed".Translate(visitor.LabelShort),
                visitor,
                MessageTypeDefOf.NegativeEvent
            );

            // 降低说服值作为惩罚
            float penalty = 0.15f;
            float newValue = persuasionHediff.PersuasionValue - penalty;
            
            // 如果说服值过低，访客可能生气离开
            if (newValue < 0.1f)
            {
                VisitorLeavesAngry();
            }

            Log.Message($"[Superb Recruitment] 招募失败: {visitor.LabelShort}");
        }

        /// <summary>
        /// 访客生气离开
        /// </summary>
        private void VisitorLeavesAngry()
        {
            // 移除说服追踪
            visitor.health.RemoveHediff(persuasionHediff);

            // 强制访客离开
            if (visitor.guest != null)
            {
                visitor.guest.SetGuestStatus(null);
            }

            // 可能降低与该阵营的关系
            if (visitor.Faction != null && visitor.Faction != Faction.OfPlayer)
            {
                visitor.Faction.TryAffectGoodwillWith(
                    Faction.OfPlayer,
                    -5,
                    true,
                    true,
                    HistoryEventDefOf.UsedHarmfulItem
                );
            }

            Messages.Message(
                "SuperbRecruitment_VisitorLeftAngry".Translate(visitor.LabelShort),
                visitor,
                MessageTypeDefOf.NegativeEvent
            );
        }

        public override bool Visible
        {
            get
            {
                return visitor != null && 
                       persuasionHediff != null &&
                       visitor.Faction != null && 
                       visitor.Faction != Faction.OfPlayer;
            }
        }

        /// <summary>
        /// 显示成功率提示
        /// </summary>
        public override string Desc
        {
            get
            {
                if (persuasionHediff != null)
                {
                    float successChance = persuasionHediff.CalculateSuccessChance();
                    return base.Desc + "\n\n" + 
                           "SuperbRecruitment_RecruitChance".Translate((successChance * 100f).ToString("F0"));
                }
                return base.Desc;
            }
        }
    }
}
