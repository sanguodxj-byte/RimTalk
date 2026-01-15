using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace ListenToMe
{
    /// <summary>
    /// Gizmo - 为选中的小人添加指令按钮
    /// </summary>
    public class Command_TextInput : Command_Action
    {
        private Pawn pawn;

        public Command_TextInput(Pawn pawn)
        {
            this.pawn = pawn;
            this.defaultLabel = "文本指令";
            this.defaultDesc = "通过文本输入控制小人执行各种任务";
            
            // 尝试加载自定义图标，如果失败则使用默认图标
            try
            {
                this.icon = ContentFinder<Texture2D>.Get("UI/Commands/ListenToMe", false);
            }
            catch
            {
                this.icon = null;
            }
            
            // 如果没有自定义图标，使用游戏内置图标
            if (this.icon == null)
            {
                this.icon = TexCommand.Draft; // 使用征召图标作为默认
            }

            this.action = () => OpenCommandDialog();
        }

        private void OpenCommandDialog()
        {
            if (pawn != null && !pawn.Dead)
            {
                Find.WindowStack.Add(new Dialog_TextCommand(pawn));
            }
        }
    }

    /// <summary>
    /// 为小人添加Gizmo的补丁类
    /// </summary>
    [StaticConstructorOnStartup]
    public static class PawnGizmoAdder
    {
        static PawnGizmoAdder()
        {
            // 使用Harmony注入
            var harmony = new HarmonyLib.Harmony("listenToMe.mod");
            harmony.PatchAll();
        }
    }
}
