using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;

namespace SuperbRecruitment
{
    /// <summary>
    /// Harmony补丁：为访客添加说服按钮
    /// </summary>
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("SuperbRecruitment.Mod");
            harmony.PatchAll();
            
            Log.Message("[Superb Recruitment] Harmony补丁已应用");
            
            // 初始化 RimTalk 钩子
            try
            {
                RimTalkHooks.Initialize();
                
                // 应用 RimTalk 对话拦截补丁
                RimTalkDialogueInterceptor.ApplyPatches(harmony);
            }
            catch (Exception ex)
            {
                Log.Warning($"[Superb Recruitment] RimTalk initialization failed (this is OK if RimTalk is not installed): {ex.Message}");
            }
        }

        /// <summary>
        /// 为访客添加 Gizmo
        /// </summary>
        [HarmonyPatch(typeof(Pawn), "GetGizmos")]
        public static class Pawn_GetGizmos_Patch
        {
            static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
            {
                // 只对访客显示说服按钮
                if (__instance == null || 
                    __instance.Faction == null || 
                    __instance.Faction == Faction.OfPlayer ||
                    __instance.Faction.HostileTo(Faction.OfPlayer) ||
                    __instance.guest == null ||
                    __instance.guest.GuestStatus != GuestStatus.Guest)
                {
                    return;
                }

                var gizmos = __result.ToList();

                // 添加说服按钮（仅在没有 RimTalk 或需要手动触发时显示）
                // 如果有 RimTalk，玩家可以直接右键对话
                var persuadeCommand = new Command_PersuadeVisitor(__instance);
                gizmos.Add(persuadeCommand);

                // 检查是否有说服追踪 Hediff
                HediffDef hediffDef = DefDatabase<HediffDef>.GetNamed("SuperbRecruitment_PersuasionTracking", false);
                if (hediffDef != null)
                {
                    var hediff = __instance.health.hediffSet.GetFirstHediffOfDef(hediffDef) as Hediff_PersuasionTracking;
                    
                    if (hediff != null)
                    {
                        // 如果说服值足够高，显示招募按钮
                        float successChance = hediff.CalculateSuccessChance();
                        if (successChance > 0.15f) // 成功率超过15%才显示招募按钮
                        {
                            var recruitCommand = new Command_RecruitVisitor(__instance, hediff);
                            gizmos.Add(recruitCommand);
                        }
                        
                        // 启用 RimTalk 自定义对话
                        RimTalkDialogueInjector.EnableCustomDialogueForVisitor(__instance);
                    }
                }

                __result = gizmos;
            }
        }

        /// <summary>
        /// 在访客第一次生成时初始化说服 Hediff
        /// </summary>
        [HarmonyPatch(typeof(Pawn), "SpawnSetup")]
        public static class Pawn_SpawnSetup_Patch
        {
            static void Postfix(Pawn __instance)
            {
                try
                {
                    // 检查是否是访客
                    if (__instance == null ||
                        __instance.Faction == null ||
                        __instance.Faction == Faction.OfPlayer ||
                        __instance.Faction.HostileTo(Faction.OfPlayer) ||
                        __instance.guest == null ||
                        __instance.guest.GuestStatus != GuestStatus.Guest)
                    {
                        return;
                    }

                    // 检查是否已有说服 Hediff
                    HediffDef hediffDef = DefDatabase<HediffDef>.GetNamed("SuperbRecruitment_PersuasionTracking", false);
                    if (hediffDef == null)
                        return;

                    var existingHediff = __instance.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                    if (existingHediff == null)
                    {
                        // 自动添加说服追踪 Hediff
                        var hediff = HediffMaker.MakeHediff(hediffDef, __instance);
                        __instance.health.AddHediff(hediff);
                        
                        Log.Message($"[Superb Recruitment] Auto-initialized persuasion tracking for visitor: {__instance.LabelShort}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"[Superb Recruitment] Error in SpawnSetup patch: {ex}");
                }
            }
        }
    }
}
