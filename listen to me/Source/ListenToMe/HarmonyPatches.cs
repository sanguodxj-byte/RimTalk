using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace ListenToMe
{
    /// <summary>
    /// Harmony补丁 - 为小人添加指令按钮
    /// </summary>
    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    public static class Patch_Pawn_GetGizmos
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            // 只为玩家派系的殖民者添加按钮
            if (__instance.Faction != null && 
                __instance.Faction.IsPlayer && 
                __instance.RaceProps.Humanlike &&
                !__instance.Dead)
            {
                var gizmos = new List<Gizmo>(__result ?? new List<Gizmo>());
                gizmos.Add(new Command_TextInput(__instance));
                __result = gizmos;
                
                // 添加调试日志
                if (DebugTools.DebugMode)
                {
                    Log.Message($"[ListenToMe] 为 {__instance.LabelShort} 添加了文本指令按钮");
                }
            }
        }
    }

    /// <summary>
    /// 添加右键菜单选项
    /// </summary>
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class Patch_FloatMenuMakerMap_AddHumanlikeOrders
    {
        [HarmonyPostfix]
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            // 确保是玩家派系的小人
            if (pawn == null || !pawn.IsColonist || pawn.Dead)
                return;

            // 添加"文本指令"选项到右键菜单
            opts.Add(new FloatMenuOption(
                "文本指令...",
                delegate
                {
                    Find.WindowStack.Add(new Dialog_TextCommand(pawn));
                },
                MenuOptionPriority.High
            ));
            
            Log.Message($"[ListenToMe] 为 {pawn.LabelShort} 添加了右键菜单选项");
        }
    }

    /// <summary>
    /// 在游戏每帧更新时调用对话系统更新
    /// </summary>
    [HarmonyPatch(typeof(TickManager), "DoSingleTick")]
    public static class Patch_TickManager_DoSingleTick
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            // 每10 tick更新一次对话系统
            if (Find.TickManager.TicksGame % 10 == 0)
            {
                DialogueSystem.Update();
            }
        }
    }

    /// <summary>
    /// Mod初始化 - 应用Harmony补丁
    /// </summary>
    [StaticConstructorOnStartup]
    public static class HarmonyInitializer
    {
        static HarmonyInitializer()
        {
            Log.Message("[ListenToMe] ========================================");
            Log.Message("[ListenToMe] 开始初始化 Listen To Me Mod");
            Log.Message("[ListenToMe] ========================================");
            
            try
            {
                // 创建Harmony实例
                var harmony = new Harmony("rimworld.listenToMe.mod");
                
                // 应用所有补丁
                harmony.PatchAll();
                
                Log.Message("[ListenToMe] ? Harmony 补丁应用成功！");
                Log.Message("[ListenToMe] 补丁列表:");
                Log.Message("[ListenToMe]   - Pawn.GetGizmos (添加文本指令按钮)");
                Log.Message("[ListenToMe]   - FloatMenuMakerMap.AddHumanlikeOrders (添加右键菜单)");
                Log.Message("[ListenToMe]   - TickManager.DoSingleTick (更新对话系统)");
                
                // 暂时禁用 RimTalk 集成（导致问题）
                // TODO: 稍后重新启用并修复
                /*
                Log.Message("[ListenToMe] 尝试集成 RimTalk 对话监听...");
                RimTalkDialogueListener.TryPatchRimTalk(harmony);
                RimTalkDialogueListener.TryPatchDialogInput(harmony);
                */
                
                Log.Message("[ListenToMe] ========================================");
                Log.Message("[ListenToMe] Mod 初始化完成！");
                Log.Message("[ListenToMe] ========================================");
                Log.Message("[ListenToMe] ");
                Log.Message("[ListenToMe] 使用方法:");
                Log.Message("[ListenToMe]   方式1: 选中小人 → 点击'文本指令'按钮");
                Log.Message("[ListenToMe]   方式2: 右键点击小人 → 选择'文本指令...'");
                Log.Message("[ListenToMe]   方式3: 选中小人 → 按 L 键");
                Log.Message("[ListenToMe] ");
                Log.Message("[ListenToMe] 提示: 启用开发者模式可以看到详细日志");
                Log.Message("[ListenToMe] ========================================");
            }
            catch (System.Exception ex)
            {
                Log.Error($"[ListenToMe] ? Harmony 补丁应用失败: {ex.Message}");
                Log.Error($"[ListenToMe] 堆栈跟踪: {ex.StackTrace}");
            }
        }
    }
}
