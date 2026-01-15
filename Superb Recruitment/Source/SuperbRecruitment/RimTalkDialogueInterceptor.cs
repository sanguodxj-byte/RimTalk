using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;

namespace SuperbRecruitment
{
    /// <summary>
    /// RimTalk 对话结果拦截器
    /// 监听 RimTalk 对话完成事件，提取对话内容并让 AI 判断说服效果
    /// </summary>
    public static class RimTalkDialogueInterceptor
    {
        private static bool patchApplied = false;
        private static Type dialogueResultType = null;

        /// <summary>
        /// 应用 Harmony 补丁到 RimTalk
        /// </summary>
        public static void ApplyPatches(Harmony harmony)
        {
            if (patchApplied)
                return;

            try
            {
                Log.Message("[Superb Recruitment] Attempting to patch RimTalk dialogue completion...");

                // 查找 RimTalk 程序集
                Assembly rimTalkAssembly = null;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name == "RimTalk")
                    {
                        rimTalkAssembly = assembly;
                        break;
                    }
                }

                if (rimTalkAssembly == null)
                {
                    Log.Message("[Superb Recruitment] RimTalk not found, dialogue interception skipped");
                    return;
                }

                // 查找对话完成方法
                var dialogueManagerType = rimTalkAssembly.GetType("RimTalk.DialogueManager");
                dialogueResultType = rimTalkAssembly.GetType("RimTalk.DialogueResult");

                if (dialogueManagerType != null)
                {
                    // 查找对话完成回调方法
                    var completeMethod = dialogueManagerType.GetMethod(
                        "OnDialogueComplete",
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic
                    );

                    if (completeMethod != null)
                    {
                        var postfix = typeof(RimTalkDialogueInterceptor).GetMethod(
                            nameof(OnDialogueComplete_Postfix),
                            BindingFlags.Public | BindingFlags.Static
                        );

                        harmony.Patch(completeMethod, postfix: new HarmonyMethod(postfix));
                        patchApplied = true;
                        Log.Message("[Superb Recruitment] Successfully patched RimTalk dialogue completion");
                    }
                    else
                    {
                        Log.Warning("[Superb Recruitment] Could not find RimTalk OnDialogueComplete method");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error patching RimTalk: {ex}");
            }
        }

        /// <summary>
        /// RimTalk 对话完成后缀补丁
        /// </summary>
        public static void OnDialogueComplete_Postfix(object __instance, object result)
        {
            try
            {
                if (result == null || dialogueResultType == null)
                    return;

                // 提取对话参与者
                var initiator = ExtractPawn(result, "Initiator");
                var target = ExtractPawn(result, "Target");

                if (initiator == null || target == null)
                    return;

                // 检查目标是否是我们要说服的访客
                if (!IsPersuasionTarget(target))
                    return;

                Log.Message($"[Superb Recruitment] Intercepted RimTalk dialogue: {initiator.LabelShort} -> {target.LabelShort}");

                // 提取对话内容
                var dialogueContent = ExtractDialogueContent(result);
                
                // 让 AI 判断说服效果
                float persuasionDelta = AIDialogueEvaluator.EvaluatePersuasion(
                    dialogueContent,
                    initiator,
                    target
                );

                Log.Message($"[Superb Recruitment] AI evaluated persuasion delta: {persuasionDelta:+0.00;-0.00}");

                // 应用说服值变化
                ApplyPersuasionDelta(target, initiator, persuasionDelta);
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error in dialogue completion postfix: {ex}");
            }
        }

        /// <summary>
        /// 从对话结果中提取 Pawn
        /// </summary>
        private static Pawn ExtractPawn(object result, string fieldName)
        {
            try
            {
                var field = dialogueResultType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(result) as Pawn;
                }

                var property = dialogueResultType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    return property.GetValue(result) as Pawn;
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"[Superb Recruitment] Error extracting pawn {fieldName}: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// 提取对话内容
        /// </summary>
        private static DialogueContent ExtractDialogueContent(object result)
        {
            var content = new DialogueContent();

            try
            {
                // 提取玩家输入的文本
                var playerInputField = dialogueResultType.GetField("PlayerInput", BindingFlags.Public | BindingFlags.Instance);
                if (playerInputField != null)
                {
                    content.PlayerInput = playerInputField.GetValue(result) as string ?? "";
                }

                // 提取 AI 回应
                var aiResponseField = dialogueResultType.GetField("AIResponse", BindingFlags.Public | BindingFlags.Instance);
                if (aiResponseField != null)
                {
                    content.AIResponse = aiResponseField.GetValue(result) as string ?? "";
                }

                // 提取对话历史
                var historyField = dialogueResultType.GetField("History", BindingFlags.Public | BindingFlags.Instance);
                if (historyField != null)
                {
                    var history = historyField.GetValue(result) as List<string>;
                    if (history != null)
                    {
                        content.History = new List<string>(history);
                    }
                }

                // 提取对话质量评分（如果 RimTalk 提供）
                var qualityField = dialogueResultType.GetField("Quality", BindingFlags.Public | BindingFlags.Instance);
                if (qualityField != null)
                {
                    var quality = qualityField.GetValue(result);
                    if (quality != null)
                    {
                        content.Quality = quality.ToString();
                    }
                }

                // 提取情感分数
                var sentimentField = dialogueResultType.GetField("Sentiment", BindingFlags.Public | BindingFlags.Instance);
                if (sentimentField != null)
                {
                    var sentiment = sentimentField.GetValue(result);
                    if (sentiment != null && float.TryParse(sentiment.ToString(), out float sentimentValue))
                    {
                        content.Sentiment = sentimentValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"[Superb Recruitment] Error extracting dialogue content: {ex.Message}");
            }

            return content;
        }

        /// <summary>
        /// 检查目标是否是说服对象
        /// </summary>
        private static bool IsPersuasionTarget(Pawn pawn)
        {
            if (pawn == null)
                return false;

            // 检查是否有说服追踪 Hediff
            var hediffDef = DefDatabase<HediffDef>.GetNamed("SuperbRecruitment_PersuasionTracking", false);
            if (hediffDef == null)
                return false;

            return pawn.health?.hediffSet?.GetFirstHediffOfDef(hediffDef) != null;
        }

        /// <summary>
        /// 应用说服值变化
        /// </summary>
        private static void ApplyPersuasionDelta(Pawn target, Pawn persuader, float delta)
        {
            try
            {
                var hediffDef = DefDatabase<HediffDef>.GetNamed("SuperbRecruitment_PersuasionTracking", false);
                if (hediffDef == null)
                    return;

                var hediff = target.health.hediffSet.GetFirstHediffOfDef(hediffDef) as Hediff_PersuasionTracking;
                if (hediff != null)
                {
                    // 判断是玩家还是殖民者说服
                    bool isPlayer = persuader == null || !persuader.IsColonist;
                    
                    hediff.AdjustPersuasion(delta, persuader, isPlayer);

                    // 显示结果消息
                    ShowResultMessage(target, delta);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error applying persuasion delta: {ex}");
            }
        }

        /// <summary>
        /// 显示结果消息
        /// </summary>
        private static void ShowResultMessage(Pawn target, float delta)
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
                target,
                MessageTypeDefOf.NeutralEvent
            );
        }
    }

    /// <summary>
    /// 对话内容数据结构
    /// </summary>
    public class DialogueContent
    {
        public string PlayerInput { get; set; } = "";
        public string AIResponse { get; set; } = "";
        public List<string> History { get; set; } = new List<string>();
        public string Quality { get; set; } = "Unknown";
        public float Sentiment { get; set; } = 0f;

        public override string ToString()
        {
            return $"Player: {PlayerInput}\nAI: {AIResponse}\nQuality: {Quality}\nSentiment: {Sentiment}";
        }
    }
}
