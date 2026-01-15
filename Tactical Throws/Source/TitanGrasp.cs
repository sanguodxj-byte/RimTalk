using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
using HarmonyLib;

namespace TitanGrasp
{
    // --- 1. Def 引用定义 ---
    [DefOf]
    public static class TitanDefOf
    {
        public static AbilityDef TitanGrasp_Ability;
        public static JobDef Titan_GrappleHold;     // 持有状态
        public static JobDef Titan_Throw;           // 投掷动作
        public static JobDef Titan_Slam;            // 抱摔动作
        public static ThingDef Titan_LivingProjectile; // 活体抛射物

        static TitanDefOf() { DefOfHelper.EnsureInitializedInCtor(typeof(TitanDefOf)); }
    }

    // --- 2. 自动习得 & 视觉动画补丁 ---
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("com.titangrasp.mod");
            harmony.PatchAll();
            
            // 注入 AI 逻辑到 ThinkTree
            InjectAI();
        }

        private static void InjectAI()
        {
            try
            {
                ThinkTreeDef humanlike = DefDatabase<ThinkTreeDef>.GetNamed("Humanlike");
                if (humanlike == null) return;

                // 寻找 Combat 节点下的 JobGiver_AIFightEnemies
                // 这里的遍历需要小心，我们寻找最合适的插入点
                var nodeToFind = typeof(JobGiver_AIFightEnemies);
                
                ThinkNode_Priority combatNode = null;
                int insertIndex = -1;

                // 深度优先搜索寻找包含 JobGiver_AIFightEnemies 的父节点
                FindNode(humanlike.thinkRoot, nodeToFind, ref combatNode, ref insertIndex);

                if (combatNode != null && insertIndex != -1)
                {
                    // 在 FightEnemies 之前插入我们的 JobGiver
                    combatNode.subNodes.Insert(insertIndex, new JobGiver_AIUseTitanGrasp());
                    Log.Message("[TitanGrasp] Successfully injected AI logic into Humanlike ThinkTree.");
                }
                else
                {
                    Log.Warning("[TitanGrasp] Could not find JobGiver_AIFightEnemies in Humanlike ThinkTree to inject AI.");
                }
            }
            catch (Exception e)
            {
                Log.Error($"[TitanGrasp] Error injecting AI: {e}");
            }
        }

        private static bool FindNode(ThinkNode node, Type typeToFind, ref ThinkNode_Priority parentNode, ref int index)
        {
            if (node.subNodes == null) return false;

            for (int i = 0; i < node.subNodes.Count; i++)
            {
                var sub = node.subNodes[i];
                if (typeToFind.IsAssignableFrom(sub.GetType()))
                {
                    if (node is ThinkNode_Priority priorityNode)
                    {
                        parentNode = priorityNode;
                        index = i;
                        return true;
                    }
                }

                if (FindNode(sub, typeToFind, ref parentNode, ref index)) return true;
            }
            return false;
        }
    }

    // 补丁：生成时根据格斗等级自动获得技能
    [HarmonyPatch(typeof(Pawn), "SpawnSetup")]
    public static class Patch_AddAbility
    {
        public static void Postfix(Pawn __instance)
        {
            if (__instance == null || !__instance.RaceProps.Humanlike || __instance.Dead) return;

            // 检查是否有技能系统
            if (__instance.abilities == null) return;

            // 检查是否已有技能
            if (__instance.abilities.GetAbility(TitanDefOf.TitanGrasp_Ability) != null) return;

            // 门槛：格斗等级 >= 10
            if (__instance.skills != null)
            {
                var meleeSkill = __instance.skills.GetSkill(SkillDefOf.Melee);
                if (meleeSkill != null && meleeSkill.Level >= 10)
                {
                    __instance.abilities.GainAbility(TitanDefOf.TitanGrasp_Ability);
                }
            }
        }
    }

    // 补丁：视觉渲染拦截 (60度倾斜 + 蓄力动画)
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnAt")]
    public static class Patch_RenderPawnAt
    {
        public static void Prefix(PawnRenderer __instance, ref Vector3 drawLoc)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn == null) return;
            
            if (pawn.ParentHolder is Pawn_CarryTracker tracker)
            {
                Pawn carrier = tracker.pawn;
                if (carrier == null) return;

                // 只有在使用本Mod的Job时才调整渲染
                if (carrier.CurJobDef == TitanDefOf.Titan_GrappleHold || 
                    carrier.CurJobDef == TitanDefOf.Titan_Throw || 
                    carrier.CurJobDef == TitanDefOf.Titan_Slam)
                {
                    // 1. 基础位移：模拟60度悬挂 (向南偏移z，视觉上在身前)
                    float baseOffset = -0.3f; 

                    // 2. 动画偏移
                    float animOffset = 0f;

                    if (carrier.CurJobDef == TitanDefOf.Titan_Throw)
                    {
                        // 投掷动画：下沉蓄力(30%) -> 猛烈上挑(70%)
                        float progress = 1f - ((float)carrier.jobs.curDriver.ticksLeftThisToil / 45f);
                        if (progress < 0.3f) 
                            animOffset = -0.2f * (progress / 0.3f); // 下沉
                        else 
                            animOffset = -0.2f + (0.7f * ((progress - 0.3f) / 0.7f)); // 上挑
                    }
                    else if (carrier.CurJobDef == TitanDefOf.Titan_Slam)
                    {
                        // 抱摔动画：举高 -> 砸地
                        float progress = 1f - ((float)carrier.jobs.curDriver.ticksLeftThisToil / 40f);
                        if (progress > 0.6f) animOffset = -0.6f; // 猛砸
                        else animOffset = 0.2f; // 举高
                    }

                    // 应用 Z 轴位移 (屏幕南北向)
                    drawLoc.z += (baseOffset + animOffset);
                    // 确保图层正确 (比地板高，不被墙遮挡)
                    drawLoc.y = AltitudeLayer.Pawn.AltitudeFor() + 0.04f;
                }
            }
        }
    }

    // --- 3. CompProperties 定义 ---
    public class CompProperties_Grapple : CompProperties_AbilityEffect
    {
        public CompProperties_Grapple()
        {
            this.compClass = typeof(CompAbilityEffect_Grapple);
        }
    }

    // --- 4. 自定义 Ability 类 (控制图标显隐) ---
    public class Ability_GrappleCheck : Ability
    {
        public Ability_GrappleCheck(Pawn pawn) : base(pawn) { }
        public Ability_GrappleCheck(Pawn pawn, AbilityDef def) : base(pawn, def) { }

        public override IEnumerable<Command> GetGizmos()
        {
            // 核心限制：非空手状态彻底隐藏图标
            if (pawn.equipment != null && pawn.equipment.Primary != null) yield break;

            // 如果已经抓着人，也隐藏"擒拿"图标 (避免逻辑冲突)
            if (pawn.carryTracker != null && pawn.carryTracker.CarriedThing != null) yield break;

            foreach (var cmd in base.GetGizmos())
            {
                yield return cmd;
            }
        }
    }

    // --- 5. 擒拿效果判定 (核心机制) ---
    public class CompAbilityEffect_Grapple : CompAbilityEffect
    {
        public new CompProperties_Grapple Props => (CompProperties_Grapple)props;

        // 确保非空手无法使用 (双重保险)
        public override bool GizmoDisabled(out string reason)
        {
            if (parent.pawn.equipment != null && parent.pawn.equipment.Primary != null)
            {
                reason = "必须空手 (仿生武器可用)";
                return true;
            }
            return base.GizmoDisabled(out reason);
        }

        // AI 可用性判断
        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            if (parent.pawn.equipment?.Primary != null) return false;
            if (parent.pawn.carryTracker?.CarriedThing != null) return false;
            
            Pawn targetPawn = target.Pawn;
            if (targetPawn == null) return false;
            if (targetPawn.Dead || targetPawn.Downed) return false;
            
            // 不对友方使用
            if (targetPawn.Faction == parent.pawn.Faction) return false;
            
            return true;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn caster = parent.pawn;
            Pawn victim = target.Pawn;

            if (victim == null) return;
            if (caster.equipment != null && caster.equipment.Primary != null) return; // 再次检查

            // --- 队友判定 (直接成功) ---
            bool isFriendly = (victim.Faction == caster.Faction);
            bool success = false;

            if (isFriendly)
            {
                success = true;
            }
            else
            {
                // --- 敌人判定公式 ---
                // 基础 0.3 + (等级*0.05) + (特性0.5)
                float baseChance = 0.3f;
                float levelBonus = caster.skills.GetSkill(SkillDefOf.Melee).Level * 0.05f;
                float traitBonus = (caster.story != null && caster.story.traits != null && caster.story.traits.HasTrait(TraitDefOf.Brawler)) ? 0.5f : 0f;

                // 体型惩罚
                float sizeFactor = caster.BodySize / Mathf.Max(0.1f, victim.BodySize);
                
                float finalChance = (baseChance + levelBonus + traitBonus) * sizeFactor;
                success = Rand.Value <= finalChance;
            }

            if (success)
            {
                Job job = JobMaker.MakeJob(TitanDefOf.Titan_GrappleHold, victim);
                job.count = 1;
                caster.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                
                string msg = isFriendly 
                    ? $"{caster.LabelShort} 抓起了队友 {victim.LabelShort}。" 
                    : $"{caster.LabelShort} 擒拿成功！";
                Messages.Message(msg, caster, MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                parent.StartCooldown(180); // 3s CD
                caster.stances.SetStance(new Stance_Cooldown(60, target.Thing, null)); // 1s 硬直
                MoteMaker.ThrowText(caster.DrawPos, caster.Map, "擒拿失败", Color.red);
            }
        }
    }

    // --- 6. 持有状态 Job (动态提供投掷/抱摔按钮) ---
    public class JobDriver_GrappleHold : JobDriver
    {
        private Pawn Victim => (Pawn)job.GetTarget(TargetIndex.A).Thing;
        private int holdTicks = 0;
        private const int AI_DECISION_DELAY = 30; // AI 决策延迟 (0.5秒)

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Victim, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            
            // 持续持有，等待玩家操作或 AI 自动决策
            Toil hold = new Toil();
            hold.defaultCompleteMode = ToilCompleteMode.Never;
            hold.tickAction = () =>
            {
                pawn.rotationTracker.FaceTarget(Victim);
                holdTicks++;
                
                // AI 自动决策 (非玩家控制的小人)
                if (!pawn.IsColonistPlayerControlled && holdTicks >= AI_DECISION_DELAY)
                {
                    AIDecideAction();
                }
            };
            yield return hold;
        }

        private void AIDecideAction()
        {
            Pawn victim = pawn.carryTracker?.CarriedThing as Pawn;
            if (victim == null) return;

            // 计算最佳目标：优先投掷向墙壁或其他敌人
            float range = 6f + (pawn.skills.GetSkill(SkillDefOf.Melee).Level * 0.5f);
            
            // 寻找最佳投掷目标
            LocalTargetInfo bestThrowTarget = FindBestThrowTarget(range);
            
            if (bestThrowTarget.IsValid)
            {
                // 投掷向最佳目标
                Job throwJob = JobMaker.MakeJob(TitanDefOf.Titan_Throw, bestThrowTarget);
                pawn.jobs.StartJob(throwJob, JobCondition.InterruptForced);
            }
            else
            {
                // 没有好目标，直接抱摔
                Job slamJob = JobMaker.MakeJob(TitanDefOf.Titan_Slam, pawn.Position);
                pawn.jobs.StartJob(slamJob, JobCondition.InterruptForced);
            }
        }

        private LocalTargetInfo FindBestThrowTarget(float range)
        {
            Map map = pawn.Map;
            IntVec3 pawnPos = pawn.Position;
            
            LocalTargetInfo bestTarget = LocalTargetInfo.Invalid;
            float bestScore = 0f;

            // 搜索范围内的目标
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(pawnPos, range, true))
            {
                if (!cell.InBounds(map)) continue;
                if (cell == pawnPos) continue;

                float score = 0f;

                // 检查该位置是否有墙壁
                Building building = cell.GetEdifice(map);
                if (building != null && building.def.passability == Traversability.Impassable)
                {
                    score += 50f; // 墙壁高分
                }

                // 检查该位置是否有敌人 (一石二鸟)
                Pawn targetPawn = cell.GetFirstPawn(map);
                if (targetPawn != null && targetPawn.HostileTo(pawn))
                {
                    score += 40f; // 敌人高分
                }

                // 距离惩罚 (更远的目标稍微低分，但不是主要因素)
                float dist = pawnPos.DistanceTo(cell);
                score -= dist * 0.5f;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = cell;
                }
            }

            // 如果没有找到好目标（分数低于阈值），返回无效
            if (bestScore < 10f)
            {
                return LocalTargetInfo.Invalid;
            }

            return bestTarget;
        }
    }

    // 补丁：在擒拿状态下添加投掷/抱摔按钮
    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    public static class Patch_Pawn_GetGizmos
    {
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
        {
            foreach (var gizmo in __result)
            {
                yield return gizmo;
            }

            // 只有在擒拿持有状态下才显示额外按钮
            if (__instance.CurJobDef != TitanDefOf.Titan_GrappleHold) yield break;
            if (__instance.carryTracker?.CarriedThing == null) yield break;

            // --- 动态射程：6 + 0.5 * 等级 ---
            float level = __instance.skills.GetSkill(SkillDefOf.Melee).Level;
            float range = 6f + (level * 0.5f);

            // 按钮：投掷
            Command_Target cmdThrow = new Command_Target();
            cmdThrow.defaultLabel = $"投掷 (射程:{range:F0})";
            cmdThrow.defaultDesc = "投掷目标。撞击墙壁/其他单位造成全额伤害。";
            cmdThrow.icon = ContentFinder<Texture2D>.Get("UI/Abilities/TitanThrow", false) ?? BaseContent.BadTex;
            cmdThrow.targetingParams = TargetingParameters.ForAttackAny();
            cmdThrow.action = (LocalTargetInfo target) =>
            {
                Job throwJob = JobMaker.MakeJob(TitanDefOf.Titan_Throw, target);
                __instance.jobs.TryTakeOrderedJob(throwJob, JobTag.Misc);
            };
            yield return cmdThrow;

            // 按钮：抱摔
            Command_Action cmdSlam = new Command_Action();
            cmdSlam.defaultLabel = "抱摔";
            cmdSlam.defaultDesc = "原地猛砸，造成高额伤害和眩晕。";
            cmdSlam.icon = ContentFinder<Texture2D>.Get("UI/Abilities/TitanSlam", false) ?? BaseContent.BadTex;
            cmdSlam.action = () =>
            {
                Job slamJob = JobMaker.MakeJob(TitanDefOf.Titan_Slam, __instance.Position);
                __instance.jobs.TryTakeOrderedJob(slamJob, JobTag.Misc);
            };
            yield return cmdSlam;
        }
    }

    // --- 7. 投掷执行 Job ---
    public class JobDriver_Throw : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // 蓄力前摇 (视觉由Harmony处理)
            yield return Toils_General.Wait(45); 

            // 发射
            Toil launch = new Toil();
            launch.initAction = () =>
            {
                Pawn victim = pawn.carryTracker.CarriedThing as Pawn;
                if (victim != null && pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Direct, out Thing dropped))
                {
                    var proj = (Projectile_LivingPawn)GenSpawn.Spawn(TitanDefOf.Titan_LivingProjectile, pawn.Position, pawn.Map);
                    proj.Launch(pawn, pawn.DrawPos, job.GetTarget(TargetIndex.A).Cell, job.GetTarget(TargetIndex.A).Cell, ProjectileHitFlags.All, false, victim);
                    
                    // 进入CD
                    pawn.abilities?.GetAbility(TitanDefOf.TitanGrasp_Ability)?.StartCooldown(180);
                }
            };
            yield return launch;
        }
    }

    // --- 8. 抱摔执行 Job ---
    public class JobDriver_Slam : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // 蓄力
            yield return Toils_General.Wait(40);

            // 执行伤害
            Toil slam = new Toil();
            slam.initAction = () =>
            {
                Pawn victim = pawn.carryTracker.CarriedThing as Pawn;
                if (victim != null && pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Direct, out Thing dropped))
                {
                    // 伤害公式：(30 + 等级*2) * 近战系数
                    float baseDmg = 30f;
                    float skillBonus = pawn.skills.GetSkill(SkillDefOf.Melee).Level * 2.0f;
                    float statFactor = pawn.GetStatValue(StatDefOf.MeleeDamageFactor);
                    float finalDmg = (baseDmg + skillBonus) * statFactor;

                    // 造成伤害
                    DamageInfo dinfo = new DamageInfo(DamageDefOf.Blunt, finalDmg, 0.8f, -1, pawn);
                    dinfo.SetBodyRegion(BodyPartHeight.Middle, BodyPartDepth.Inside);
                    victim.TakeDamage(dinfo);
                    
                    // 添加眩晕效果 (使用stance系统)
                    victim.stances.stunner.StunFor(120, pawn); // 2秒眩晕

                    // 特效
                    GenExplosion.DoExplosion(pawn.Position, pawn.Map, 1.9f, DamageDefOf.Smoke, null);
                    MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, $"暴击 {finalDmg:F0}", Color.red);
                    
                    pawn.abilities?.GetAbility(TitanDefOf.TitanGrasp_Ability)?.StartCooldown(180);
                }
            };
            yield return slam;
        }
    }

    // --- 9. 活体抛射物逻辑 (环境杀) ---
    public class Projectile_LivingPawn : Projectile
    {
        private Pawn flyingPawn;

        public void Launch(Thing launcher, Vector3 origin, IntVec3 usedTarget, IntVec3 intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire, Pawn pawnToThrow)
        {
            this.flyingPawn = pawnToThrow;
            base.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, null);
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = base.Map;
            IntVec3 pos = this.Position;
            base.Impact(hitThing, blockedByShield);
            
            if (flyingPawn == null) 
            { 
                this.Destroy(); 
                return; 
            }

            // 计算伤害数据
            Pawn caster = this.launcher as Pawn;
            float dmgFactor = (caster != null) ? caster.GetStatValue(StatDefOf.MeleeDamageFactor) : 1f;
            float skillBonus = (caster != null) ? caster.skills.GetSkill(SkillDefOf.Melee).Level * 2.0f : 0f;

            bool hardImpact = false;
            string txt = "落地";

            // 1. 撞到人 (一石二鸟)
            if (hitThing is Pawn targetPawn)
            {
                hardImpact = true;
                txt = "碰撞";
                float collisionDmg = (20f + skillBonus) * dmgFactor;
                targetPawn.TakeDamage(new DamageInfo(DamageDefOf.Blunt, collisionDmg, 0.6f, -1, this.launcher));
                
                targetPawn.stances.stunner.StunFor(120, this.launcher as Pawn); // 2秒眩晕
            }
            // 2. 撞到墙/石头
            else if (hitThing is Building || (pos.InBounds(map) && pos.GetTerrain(map).affordances.Contains(TerrainAffordanceDefOf.Heavy)))
            {
                hardImpact = true;
                txt = "撞墙";
            }

            // 重新把人生成出来
            if (pos.InBounds(map))
            {
                GenSpawn.Spawn(flyingPawn, pos, map);
            }

            // 3. 结算自身伤害 (队友撞墙也一样痛)
            float selfBase = hardImpact ? 25f : 10f; // 撞墙25，空地10
            float selfDmg = hardImpact ? ((selfBase + skillBonus) * dmgFactor) : 10f; // 空地不受加成

            flyingPawn.TakeDamage(new DamageInfo(DamageDefOf.Blunt, selfDmg, 0.8f, -1, this.launcher));
            
            if (hardImpact)
            {
                flyingPawn.stances.stunner.StunFor(120, this.launcher as Pawn); // 2秒眩晕
                
                GenExplosion.DoExplosion(pos, map, 1.5f, DamageDefOf.Smoke, null);
                MoteMaker.ThrowText(pos.ToVector3(), map, $"{txt} {selfDmg:F0}", Color.yellow);
            }

            this.Destroy();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref flyingPawn, "flyingPawn");
        }
    }

    // --- 10. AI 自动施法 JobGiver ---
    public class JobGiver_AIUseTitanGrasp : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            // 1. 基础状态检查
            if (pawn.Downed || pawn.Dead || pawn.IsColonistPlayerControlled) return null;
            if (pawn.equipment?.Primary != null) return null; // 必须空手

            // 2. 检查是否有技能且可用
            var ability = pawn.abilities?.GetAbility(TitanDefOf.TitanGrasp_Ability);
            if (ability == null || !ability.CanCast) return null;

            // 3. 获取当前攻击目标
            Pawn enemy = pawn.mindState.enemyTarget as Pawn;
            if (enemy == null || !enemy.Spawned || enemy.Downed) return null;

            // 4. 距离检查
            // 允许 AI 在一定范围内主动接近并尝试擒拿，而不是仅在贴脸时使用
            if (pawn.Position.DistanceTo(enemy.Position) > 25f) return null;

            // 5. 目标有效性检查 (使用 Ability 的判定逻辑)
            if (!ability.CanApplyOn(new LocalTargetInfo(enemy))) return null;

            // 6. 成功率预判 (可选：如果成功率太低就不尝试)
            // 这里我们让 AI 总是尝试，增加混乱度

            // 7. 返回施法 Job
            return ability.GetJob(enemy, enemy);
        }
    }
}