using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace ListenToMe
{
    /// <summary>
    /// UI窗口 - 用于输入文本指令
    /// </summary>
    public class Dialog_TextCommand : Window
    {
        private string inputText = "";
        private Pawn targetPawn;
        private bool useDialogue = true;

        public override Vector2 InitialSize => new Vector2(700f, 500f);  // 增大窗口尺寸

        public Dialog_TextCommand(Pawn pawn)
        {
            this.targetPawn = pawn;
            this.doCloseButton = false;  // 禁用默认关闭按钮
            this.doCloseX = true;        // 保留右上角 X
            this.closeOnClickedOutside = false;  // 不要点外面就关闭
            this.absorbInputAroundWindow = true;
            this.forcePause = false;     // 不要暂停游戏
        }

        public override void DoWindowContents(Rect inRect)
        {
            // 首先检查按键事件（在任何 GUI 控件之前）
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
                {
                    Event.current.Use();
                    ExecuteCommand();
                    return;
                }
                else if (Event.current.keyCode == KeyCode.Escape)
                {
                    Event.current.Use();
                    Close();
                    return;
                }
            }

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            // 标题
            Text.Font = GameFont.Medium;
            Rect titleRect = listing.GetRect(Text.LineHeight);
            Widgets.Label(titleRect, $"给 {targetPawn.LabelShort} 下达指令");
            Text.Font = GameFont.Small;

            listing.Gap(12f);

            // 小人信息
            Rect pawnInfoRect = listing.GetRect(60f);
            DrawPawnInfo(pawnInfoRect);

            listing.Gap(12f);

            // 输入框标签
            Rect labelRect = listing.GetRect(Text.LineHeight);
            Widgets.Label(labelRect, "输入指令 (例如: 等待 / 去厨房 / 攻击敌人) - 输入后按 Enter ?");
            
            // 输入框
            Rect textEntryRect = listing.GetRect(30f);
            GUI.SetNextControlName("CommandInput");
            inputText = Widgets.TextField(textEntryRect, inputText);

            listing.Gap(6f);

            // 对话选项
            listing.CheckboxLabeled("使用对话模式 (小人会先回应再行动)", ref useDialogue);

            listing.Gap(12f);

            // 示例指令
            DrawExampleCommands(listing);

            listing.Gap(20f);

            // 按钮
            Rect buttonRect = listing.GetRect(35f);
            float buttonWidth = (buttonRect.width - 10f) / 2f;

            if (Widgets.ButtonText(new Rect(buttonRect.x, buttonRect.y, buttonWidth, buttonRect.height), "执行指令 (Enter)"))
            {
                ExecuteCommand();
            }

            if (Widgets.ButtonText(new Rect(buttonRect.x + buttonWidth + 10f, buttonRect.y, buttonWidth, buttonRect.height), "取消 (Esc)"))
            {
                Close();
            }

            listing.End();
        }

        private void DrawPawnInfo(Rect rect)
        {
            Widgets.DrawBoxSolid(rect, new Color(0.2f, 0.2f, 0.2f, 0.5f));
            
            Rect innerRect = rect.ContractedBy(4f);
            
            // 小人名字
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(innerRect.x, innerRect.y, innerRect.width, 20f), 
                $"<b>{targetPawn.LabelShort}</b>");
            
            // 当前活动
            string activity = targetPawn.jobs?.curJob != null 
                ? targetPawn.jobs.curJob.GetReport(targetPawn) 
                : "空闲";
            Widgets.Label(new Rect(innerRect.x, innerRect.y + 22f, innerRect.width, 18f), 
                $"当前: {activity}");
            
            // 心情
            if (targetPawn.needs?.mood != null)
            {
                float mood = targetPawn.needs.mood.CurLevel;
                string moodText = mood > 0.7f ? "?? 心情很好" : mood > 0.3f ? "?? 心情一般" : "?? 心情不好";
                Widgets.Label(new Rect(innerRect.x, innerRect.y + 40f, innerRect.width, 18f), moodText);
            }
        }

        private void DrawExampleCommands(Listing_Standard listing)
        {
            Rect exampleLabelRect = listing.GetRect(Text.LineHeight);
            Widgets.Label(exampleLabelRect, "<i>示例指令:</i>");
            
            Rect exampleRect = listing.GetRect(120f);
            Widgets.DrawBoxSolid(exampleRect, new Color(0.1f, 0.1f, 0.1f, 0.3f));
            
            Rect textRect = exampleRect.ContractedBy(6f);
            Text.Font = GameFont.Tiny;
            
            string examples = 
                "? 移动类: 去厨房 / 到仓库 / 前往医疗室\n" +
                "? 工作类: 在裁缝台工作 / 做饭 / 清洁房间\n" +
                "? 战斗类: 攻击敌人 / 消灭入侵者\n" +
                "? 制作类: 制作防尘大衣 / 做3个木墙 / 生产药品\n" +
                "? 其他类: 等待 / 狩猎 / 采集植物 / 挖矿 / 治疗伤员";
            
            Widgets.Label(textRect, examples);
            Text.Font = GameFont.Small;
        }

        private void ExecuteCommand()
        {
            Log.Message("[ListenToMe] ========== ExecuteCommand 开始 ==========");
            
            if (string.IsNullOrWhiteSpace(inputText))
            {
                Log.Warning("[ListenToMe] 输入为空");
                Messages.Message("请输入指令", MessageTypeDefOf.RejectInput);
                return;
            }

            Log.Message($"[ListenToMe] 输入文本: '{inputText}'");

            if (targetPawn == null || targetPawn.Dead)
            {
                Log.Warning("[ListenToMe] 目标小人不可用");
                Messages.Message("目标小人不可用", MessageTypeDefOf.RejectInput);
                Close();
                return;
            }

            Log.Message($"[ListenToMe] 目标小人: {targetPawn.LabelShort}");
            Log.Message($"[ListenToMe] 小人状态: Dead={targetPawn.Dead}, Downed={targetPawn.Downed}, Drafted={targetPawn.Drafted}");

            // 解析指令
            Log.Message("[ListenToMe] 开始解析指令...");
            ParsedCommand command = CommandParser.ParseCommand(inputText, targetPawn);
            Log.Message($"[ListenToMe] 解析结果: Type={command.Type}, Original='{command.OriginalText}'");

            if (command.Type == CommandParser.CommandType.Unknown)
            {
                Log.Warning("[ListenToMe] 指令类型未知");
                Messages.Message("无法识别指令，请重新输入", MessageTypeDefOf.RejectInput);
                return;
            }

            // 根据是否使用对话模式执行
            if (useDialogue)
            {
                Log.Message("[ListenToMe] 使用对话模式");
                // 使用对话系统
                DialogueSystem.StartDialogue(targetPawn, inputText, command);
                Messages.Message($"已向 {targetPawn.LabelShort} 下达指令: {inputText}", MessageTypeDefOf.NeutralEvent);
            }
            else
            {
                Log.Message("[ListenToMe] 使用直接执行模式");
                // 直接执行
                bool success = CommandExecutor.ExecuteCommand(command, targetPawn);
                Log.Message($"[ListenToMe] CommandExecutor.ExecuteCommand 返回: {success}");
                
                if (success)
                {
                    Messages.Message($"{targetPawn.LabelShort} 开始执行: {inputText}", MessageTypeDefOf.TaskCompletion);
                    Log.Message($"[ListenToMe] ? 指令执行成功");
                }
                else
                {
                    Log.Warning($"[ListenToMe] ? 指令执行失败");
                }
            }

            Log.Message("[ListenToMe] ========== ExecuteCommand 结束 ==========");
            Close();
        }
    }
}
