using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ListenToMe
{
    /// <summary>
    /// 对话系统 - 小人先回应再执行指令
    /// </summary>
    public static class DialogueSystem
    {
        private static Dictionary<Pawn, DialogueState> activeDialogues = new Dictionary<Pawn, DialogueState>();

        /// <summary>
        /// 开始与小人对话 - 小人回应后执行指令
        /// </summary>
        public static void StartDialogue(Pawn pawn, string playerInput, ParsedCommand command)
        {
            if (pawn == null || pawn.Dead || pawn.Map == null)
                return;

            Log.Message($"[ListenToMe] 开始对话 - {pawn.LabelShort}: {playerInput}");

            // 创建对话状态
            var state = new DialogueState
            {
                Pawn = pawn,
                PlayerInput = playerInput,
                Command = command,
                Stage = DialogueStage.GeneratingResponse,
                StartTick = Find.TickManager.TicksGame
            };

            activeDialogues[pawn] = state;

            // 直接生成简单回应（不依赖 RimTalk）
            string response = GenerateFallbackResponse(pawn, playerInput);
            OnResponseReceived(pawn, response);
        }

        /// <summary>
        /// 当收到回应时
        /// </summary>
        private static void OnResponseReceived(Pawn pawn, string response)
        {
            if (!activeDialogues.TryGetValue(pawn, out var state))
                return;

            state.PawnResponse = response;
            state.Stage = DialogueStage.WaitingForResponse;
            state.ExecutionTick = Find.TickManager.TicksGame + GetResponseDelay(response);

            // 显示对话气泡
            ShowDialogueBubble(pawn, response);
            
            Log.Message($"[ListenToMe] {pawn.LabelShort} 回应: {response}");
        }

        /// <summary>
        /// 更新对话系统 - 检查是否该执行指令了
        /// </summary>
        public static void Update()
        {
            if (activeDialogues.Count == 0) return;

            var completedDialogues = new List<Pawn>();
            int currentTick = Find.TickManager.TicksGame;

            foreach (var kvp in activeDialogues)
            {
                var pawn = kvp.Key;
                var state = kvp.Value;

                // 跳过还在生成回应的对话
                if (state.Stage == DialogueStage.GeneratingResponse)
                    continue;

                // 检查是否到了执行指令的时间
                if (state.Stage == DialogueStage.WaitingForResponse && currentTick >= state.ExecutionTick)
                {
                    Log.Message($"[ListenToMe] {pawn.LabelShort} 开始执行指令（回应后）");
                    
                    // 执行指令
                    bool success = CommandExecutor.ExecuteCommand(state.Command, pawn);
                    
                    state.Stage = success ? DialogueStage.Completed : DialogueStage.Failed;
                    completedDialogues.Add(pawn);
                }
            }

            // 清理已完成的对话
            foreach (var pawn in completedDialogues)
            {
                activeDialogues.Remove(pawn);
            }
        }

        /// <summary>
        /// 显示对话气泡
        /// </summary>
        private static void ShowDialogueBubble(Pawn pawn, string text)
        {
            if (pawn?.Map == null) return;

            try
            {
                // 使用 RimWorld 原生 Mote 系统显示对话
                MoteMaker.ThrowText(
                    pawn.DrawPos + new UnityEngine.Vector3(0f, 0f, 0.5f),
                    pawn.Map,
                    text,
                    UnityEngine.Color.white,
                    5f  // 持续 5 秒
                );
            }
            catch (Exception ex)
            {
                Log.Warning($"[ListenToMe] 显示对话失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取回应延迟时间（基于文本长度）
        /// </summary>
        private static int GetResponseDelay(string response)
        {
            // 基础延迟 60 ticks (1秒) + 每个字 5 ticks
            return 60 + (response.Length * 5);
        }

        /// <summary>
        /// 生成预设回应
        /// </summary>
        private static string GenerateFallbackResponse(Pawn pawn, string command)
        {
            var responses = new List<string>
            {
                "好的", "明白了", "收到", "我这就去", "没问题", "了解"
            };

            // 根据心情调整
            if (pawn?.needs?.mood != null)
            {
                float mood = pawn.needs.mood.CurLevel;
                if (mood < 0.3f)
                {
                    responses.Clear();
                    responses.AddRange(new[] { "好吧...", "知道了", "唉..." });
                }
                else if (mood > 0.7f)
                {
                    responses.AddRange(new[] { "好嘞！", "没问题！", "马上！" });
                }
            }

            return responses.RandomElement();
        }

        /// <summary>
        /// 检查小人是否正在对话中
        /// </summary>
        public static bool IsInDialogue(Pawn pawn)
        {
            return activeDialogues.ContainsKey(pawn);
        }

        /// <summary>
        /// 取消对话
        /// </summary>
        public static void CancelDialogue(Pawn pawn)
        {
            activeDialogues.Remove(pawn);
        }
    }

    /// <summary>
    /// 对话状态
    /// </summary>
    public class DialogueState
    {
        public Pawn Pawn { get; set; }
        public string PlayerInput { get; set; }
        public ParsedCommand Command { get; set; }
        public string PawnResponse { get; set; }
        public DialogueStage Stage { get; set; }
        public int StartTick { get; set; }
        public int ExecutionTick { get; set; }
    }

    /// <summary>
    /// 对话阶段
    /// </summary>
    public enum DialogueStage
    {
        GeneratingResponse,  // 正在生成回应
        WaitingForResponse,  // 等待显示回应
        Executing,           // 执行中
        Completed,           // 已完成
        Failed               // 失败
    }
}
