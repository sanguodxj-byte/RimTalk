using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace SuperbRecruitment
{
    /// <summary>
    /// 访客说服命令Gizmo
    /// </summary>
    public class Command_PersuadeVisitor : Command_Action
    {
        private Pawn visitor;

        public Command_PersuadeVisitor(Pawn visitor)
        {
            this.visitor = visitor;
            defaultLabel = "SuperbRecruitment_PersuadeLabel".Translate();
            defaultDesc = "SuperbRecruitment_PersuadeDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Commands/Persuade", true);
            
            // 如果找不到图标，使用默认社交图标
            if (icon == null)
            {
                icon = TexCommand.GatherSpotActive;
            }

            action = delegate
            {
                ShowPersuaderSelectionMenu();
            };
        }

        /// <summary>
        /// 显示说服者选择菜单
        /// </summary>
        private void ShowPersuaderSelectionMenu()
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();

            // 获取或创建说服追踪Hediff
            Hediff_PersuasionTracking persuasionHediff = GetOrCreatePersuasionHediff();

            if (persuasionHediff == null)
            {
                Messages.Message("SuperbRecruitment_CannotPersuade".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            // 选项1: 玩家自己说服
            if (persuasionHediff.PlayerAttemptsRemaining > 0)
            {
                FloatMenuOption playerOption = new FloatMenuOption(
                    "SuperbRecruitment_PlayerPersuade".Translate(persuasionHediff.PlayerAttemptsRemaining),
                    delegate
                    {
                        PersuasionDialogueManager.Instance.StartDialogue(visitor, null, true);
                    }
                );
                options.Add(playerOption);
            }
            else
            {
                FloatMenuOption playerOptionDisabled = new FloatMenuOption(
                    "SuperbRecruitment_PlayerNoAttempts".Translate(),
                    null
                );
                playerOptionDisabled.Disabled = true;
                options.Add(playerOptionDisabled);
            }

            // 选项2: 选择殖民者说服
            List<Pawn> availableColonists = GetAvailableColonists(persuasionHediff);

            if (availableColonists.Count > 0)
            {
                foreach (Pawn colonist in availableColonists)
                {
                    Pawn localColonist = colonist; // 避免闭包问题
                    
                    string label = "SuperbRecruitment_ColonistPersuade".Translate(
                        localColonist.LabelShort,
                        GetColonistPersuasionBonus(localColonist)
                    );

                    FloatMenuOption colonistOption = new FloatMenuOption(
                        label,
                        delegate
                        {
                            PersuasionDialogueManager.Instance.StartDialogue(visitor, localColonist, false);
                        }
                    );
                    options.Add(colonistOption);
                }
            }
            else
            {
                FloatMenuOption noColonistsOption = new FloatMenuOption(
                    "SuperbRecruitment_NoColonistsAvailable".Translate(),
                    null
                );
                noColonistsOption.Disabled = true;
                options.Add(noColonistsOption);
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }

        /// <summary>
        /// 获取或创建说服追踪Hediff
        /// </summary>
        private Hediff_PersuasionTracking GetOrCreatePersuasionHediff()
        {
            HediffDef hediffDef = DefDatabase<HediffDef>.GetNamed("SuperbRecruitment_PersuasionTracking", false);
            if (hediffDef == null)
            {
                Log.Error("[Superb Recruitment] 找不到 SuperbRecruitment_PersuasionTracking HediffDef");
                return null;
            }

            Hediff_PersuasionTracking hediff = visitor.health.hediffSet.GetFirstHediffOfDef(hediffDef) as Hediff_PersuasionTracking;

            if (hediff == null)
            {
                hediff = (Hediff_PersuasionTracking)HediffMaker.MakeHediff(hediffDef, visitor);
                visitor.health.AddHediff(hediff);
            }

            return hediff;
        }

        /// <summary>
        /// 获取可用的殖民者列表
        /// </summary>
        private List<Pawn> GetAvailableColonists(Hediff_PersuasionTracking persuasionHediff)
        {
            return visitor.Map.mapPawns.FreeColonistsSpawned
                .Where(p => 
                    p.RaceProps.Humanlike &&
                    !p.Dead &&
                    !p.Downed &&
                    p.Awake() &&
                    !persuasionHediff.pawn.HostileTo(p) &&
                    persuasionHediff.CanColonistTryPersuade(p.thingIDNumber)
                )
                .OrderByDescending(p => GetColonistPersuasionBonus(p))
                .ToList();
        }

        /// <summary>
        /// 计算殖民者的说服加成
        /// </summary>
        private float GetColonistPersuasionBonus(Pawn colonist)
        {
            float bonus = 1.0f;

            // 社交技能加成
            if (colonist.skills != null)
            {
                int socialSkill = colonist.skills.GetSkill(SkillDefOf.Social).Level;
                bonus += socialSkill * 0.02f; // 每级+2%
            }

            // 特性加成
            if (colonist.story?.traits != null)
            {
                if (colonist.story.traits.HasTrait(TraitDefOf.Kind))
                    bonus += 0.15f;
                if (colonist.story.traits.HasTrait(TraitDefOf.Psychopath))
                    bonus -= 0.20f;
            }

            return bonus;
        }

        public override bool Visible
        {
            get
            {
                // 只对访客显示
                return visitor != null && 
                       visitor.Faction != null && 
                       visitor.Faction != Faction.OfPlayer &&
                       !visitor.Faction.HostileTo(Faction.OfPlayer) &&
                       visitor.guest != null &&
                       visitor.guest.GuestStatus == GuestStatus.Guest;
            }
        }
    }
}
