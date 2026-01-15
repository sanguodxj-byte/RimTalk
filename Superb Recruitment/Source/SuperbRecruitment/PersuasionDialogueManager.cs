using System;
using Verse;
using RimWorld;

namespace SuperbRecruitment
{
    /// <summary>
    /// 说服对话管理器（单例模式）
    /// 集成 RimTalk 支持 - 优先使用 RimTalk，只有在不可用时才使用选项菜单
    /// </summary>
    public class PersuasionDialogueManager
    {
        private static PersuasionDialogueManager instance;
        
        public static PersuasionDialogueManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PersuasionDialogueManager();
                }
                return instance;
            }
        }

        private Pawn currentVisitor;
        private Pawn currentPersuader;
        private bool isPlayerControlled;

        /// <summary>
        /// 开始说服对话
        /// 逻辑：优先使用 RimTalk 自定义对话，只有在 RimTalk 不可用时才显示选项菜单
        /// </summary>
        public void StartDialogue(Pawn visitor, Pawn persuader, bool isPlayerControlled)
        {
            if (visitor == null)
            {
                Log.Error("[Superb Recruitment] 无法开始对话：访客为空");
                return;
            }

            this.currentVisitor = visitor;
            this.currentPersuader = persuader;
            this.isPlayerControlled = isPlayerControlled;

            Log.Message($"[Superb Recruitment] 开始说服对话: 访客={visitor.LabelShort}, 说服者={persuader?.LabelShort ?? "玩家"}");

            // 确保访客有说服追踪 Hediff
            EnsurePersuasionHediff(visitor);

            // 优先检查并使用 RimTalk
            if (RimTalkHooks.IsRimTalkAvailable())
            {
                // 如果有 RimTalk，提示玩家使用右键对话
                if (isPlayerControlled)
                {
                    Messages.Message(
                        "SuperbRecruitment_UseRimTalk".Translate(visitor.LabelShort),
                        visitor,
                        MessageTypeDefOf.NeutralEvent
                    );
                    
                    Log.Message("[Superb Recruitment] RimTalk 可用，提示玩家使用右键对话");
                    return; // 不显示选项菜单，让玩家使用 RimTalk
                }
                else
                {
                    // 殖民者 NPC 说服时，尝试自动触发 RimTalk 对话
                    bool rimTalkStarted = RimTalkHooks.TryStartDialogue(
                        visitor, 
                        persuader, 
                        OnRimTalkDialogueComplete
                    );

                    if (rimTalkStarted)
                    {
                        Log.Message("[Superb Recruitment] 使用 RimTalk 对话系统（NPC）");
                        return;
                    }
                }
            }

            // RimTalk 不可用或启动失败，使用回退系统
            if (isPlayerControlled)
            {
                ShowSimpleDialogue();
            }
            else
            {
                PerformNPCPersuasion();
            }
        }

        /// <summary>
        /// 确保访客有说服追踪 Hediff
        /// </summary>
        private void EnsurePersuasionHediff(Pawn visitor)
        {
            HediffDef hediffDef = DefDatabase<HediffDef>.GetNamed("SuperbRecruitment_PersuasionTracking", false);
            if (hediffDef == null)
                return;

            var hediff = visitor.health.hediffSet.GetFirstHediffOfDef(hediffDef);
            if (hediff == null)
            {
                hediff = HediffMaker.MakeHediff(hediffDef, visitor);
                visitor.health.AddHediff(hediff);
                Log.Message($"[Superb Recruitment] 为访客 {visitor.LabelShort} 添加说服追踪");
            }
        }

        /// <summary>
        /// RimTalk 对话完成回调
        /// 注意：这个方法在有 RimTalk 时通常不会被调用，
        /// 因为 RimTalkDialogueInterceptor 会自动拦截并处理
        /// </summary>
        private void OnRimTalkDialogueComplete(float persuasionDelta)
        {
            Log.Message($"[Superb Recruitment] RimTalk 对话完成回调，说服值变化: {persuasionDelta:+0.00;-0.00}");
            
            ApplyPersuasionResult(persuasionDelta);
        }

        /// <summary>
        /// 显示简单对话窗口（回退方案）
        /// 只在 RimTalk 不可用时使用
        /// </summary>
        private void ShowSimpleDialogue()
        {
            // 获取说服追踪 Hediff
            HediffDef hediffDef = DefDatabase<HediffDef>.GetNamed("SuperbRecruitment_PersuasionTracking", false);
            if (hediffDef == null)
            {
                Log.Error("[Superb Recruitment] 找不到 SuperbRecruitment_PersuasionTracking HediffDef");
                return;
            }

            Hediff_PersuasionTracking hediff = currentVisitor.health.hediffSet.GetFirstHediffOfDef(hediffDef) as Hediff_PersuasionTracking;
            
            if (hediff == null)
            {
                hediff = (Hediff_PersuasionTracking)HediffMaker.MakeHediff(hediffDef, currentVisitor);
                currentVisitor.health.AddHediff(hediff);
            }

            Log.Message("[Superb Recruitment] 显示简单对话窗口（RimTalk 不可用）");

            var dialogue = new Dialog_SimplePersuasion(
                currentVisitor,
                hediff,
                OnDialogueOptionSelected
            );
            Find.WindowStack.Add(dialogue);
        }

        /// <summary>
        /// 对话选项选择回调（简单对话系统）
        /// </summary>
        public void OnDialogueOptionSelected(float persuasionDelta)
        {
            ApplyPersuasionResult(persuasionDelta);
        }

        /// <summary>
        /// 应用说服结果
        /// </summary>
        private void ApplyPersuasionResult(float persuasionDelta)
        {
            if (currentVisitor == null)
                return;

            // 获取或创建说服追踪 Hediff
            HediffDef hediffDef = DefDatabase<HediffDef>.GetNamed("SuperbRecruitment_PersuasionTracking", false);
            if (hediffDef == null)
            {
                Log.Error("[Superb Recruitment] 找不到 SuperbRecruitment_PersuasionTracking HediffDef");
                return;
            }

            Hediff_PersuasionTracking hediff = currentVisitor.health.hediffSet.GetFirstHediffOfDef(hediffDef) as Hediff_PersuasionTracking;
            
            if (hediff == null)
            {
                hediff = (Hediff_PersuasionTracking)HediffMaker.MakeHediff(hediffDef, currentVisitor);
                currentVisitor.health.AddHediff(hediff);
            }

            // 应用说服值变化
            hediff.AdjustPersuasion(persuasionDelta, currentPersuader, isPlayerControlled);

            // 显示结果消息
            ShowPersuasionResultMessage(persuasionDelta);
        }

        /// <summary>
        /// 显示说服结果消息
        /// </summary>
        private void ShowPersuasionResultMessage(float delta)
        {
            string messageKey;
            
            if (delta >= 0.15f)
                messageKey = "SuperbRecruitment_PersuasionExcellent";
            else if (delta >= 0.10f)
                messageKey = "SuperbRecruitment_PersuasionGood";
            else if (delta >= 0.05f)
                messageKey = "SuperbRecruitment_PersuasionAverage";
            else if (delta > 0f)
                messageKey = "SuperbRecruitment_PersuasionPoor";
            else
                messageKey = "SuperbRecruitment_PersuasionNegative";

            Messages.Message(
                messageKey.Translate(),
                currentVisitor,
                MessageTypeDefOf.NeutralEvent
            );
        }

        /// <summary>
        /// NPC 自动说服（殖民者）
        /// </summary>
        private void PerformNPCPersuasion()
        {
            if (currentPersuader == null || currentVisitor == null)
                return;

            Messages.Message(
                "SuperbRecruitment_NPCPersuasionAttempt".Translate(
                    currentPersuader.LabelShort, 
                    currentVisitor.LabelShort
                ),
                currentPersuader,
                MessageTypeDefOf.NeutralEvent
            );

            // 基础说服效果
            float baseEffect = 0.08f;

            // 社交技能加成
            if (currentPersuader.skills != null)
            {
                int socialSkill = currentPersuader.skills.GetSkill(SkillDefOf.Social).Level;
                baseEffect += socialSkill * 0.005f; // 每级+0.5%
            }

            // 特性加成
            if (currentPersuader.story?.traits != null)
            {
                if (currentPersuader.story.traits.HasTrait(TraitDefOf.Kind))
                    baseEffect += 0.15f;
                if (currentPersuader.story.traits.HasTrait(TraitDefOf.Psychopath))
                    baseEffect -= 0.20f;
            }

            // 添加随机变化
            float randomVariation = Rand.Range(0.8f, 1.2f);
            float finalEffect = baseEffect * randomVariation;

            ApplyPersuasionResult(finalEffect);
        }
    }
}
