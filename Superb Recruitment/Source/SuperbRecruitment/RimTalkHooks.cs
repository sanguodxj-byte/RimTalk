using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using RimWorld;
using HarmonyLib;

namespace SuperbRecruitment
{
    /// <summary>
    /// RimTalk 集成钩子 - 参考 RimTalkExpandMemory 的实现方式
    /// 通过反射和 Harmony 注入自定义对话内容
    /// </summary>
    public static class RimTalkHooks
    {
        private static bool rimTalkDetected = false;
        private static Type dialogueManagerType = null;
        private static Type conversationNodeType = null;
        private static MethodInfo addDialogueMethod = null;

        /// <summary>
        /// 初始化 RimTalk 钩子
        /// </summary>
        public static void Initialize()
        {
            try
            {
                // 检测 RimTalk 是否存在
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name == "RimTalk")
                    {
                        Log.Message("[Superb Recruitment] RimTalk detected, initializing hooks...");
                        
                        // 查找 RimTalk 核心类型
                        dialogueManagerType = assembly.GetType("RimTalk.DialogueManager");
                        conversationNodeType = assembly.GetType("RimTalk.ConversationNode");
                        
                        if (dialogueManagerType != null)
                        {
                            // 获取添加对话的方法
                            addDialogueMethod = dialogueManagerType.GetMethod(
                                "RegisterDialogue",
                                BindingFlags.Public | BindingFlags.Static
                            );
                            
                            if (addDialogueMethod != null)
                            {
                                rimTalkDetected = true;
                                Log.Message("[Superb Recruitment] RimTalk hooks initialized successfully");
                                
                                // 注册自定义对话
                                RegisterCustomDialogues();
                            }
                        }
                        
                        break;
                    }
                }
                
                if (!rimTalkDetected)
                {
                    Log.Message("[Superb Recruitment] RimTalk not detected, using fallback dialogue system");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error initializing RimTalk hooks: {ex}");
                rimTalkDetected = false;
            }
        }

        /// <summary>
        /// 检查 RimTalk 是否可用
        /// </summary>
        public static bool IsRimTalkAvailable()
        {
            return rimTalkDetected;
        }

        /// <summary>
        /// 注册自定义对话内容
        /// </summary>
        private static void RegisterCustomDialogues()
        {
            try
            {
                // 创建说服对话数据
                var persuasionDialogue = CreatePersuasionDialogue();
                
                if (persuasionDialogue != null && addDialogueMethod != null)
                {
                    addDialogueMethod.Invoke(null, new object[] { "SuperbRecruitment_Persuade", persuasionDialogue });
                    Log.Message("[Superb Recruitment] Registered custom persuasion dialogue with RimTalk");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error registering dialogues: {ex}");
            }
        }

        /// <summary>
        /// 创建说服对话结构
        /// </summary>
        private static object CreatePersuasionDialogue()
        {
            try
            {
                // 使用反射创建对话数据结构
                // 这里需要根据 RimTalk 的实际 API 调整
                var dialogueData = new Dictionary<string, object>
                {
                    { "id", "SuperbRecruitment_Persuade" },
                    { "title", "Persuade Visitor" },
                    { "description", "Attempt to convince the visitor to join your colony" },
                    { "allowedPawns", new List<string> { "Visitor" } }
                };
                
                return dialogueData;
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error creating dialogue: {ex}");
                return null;
            }
        }

        /// <summary>
        /// 启动 RimTalk 对话（如果可用）
        /// </summary>
        public static bool TryStartDialogue(Pawn visitor, Pawn persuader, Action<float> onComplete)
        {
            if (!rimTalkDetected || dialogueManagerType == null)
                return false;

            try
            {
                // 通过反射调用 RimTalk 的对话启动方法
                var startDialogueMethod = dialogueManagerType.GetMethod(
                    "StartDialogue",
                    BindingFlags.Public | BindingFlags.Static
                );

                if (startDialogueMethod != null)
                {
                    var parameters = new object[]
                    {
                        visitor,
                        "SuperbRecruitment_Persuade",
                        new Dictionary<string, object>
                        {
                            { "persuader", persuader },
                            { "callback", onComplete }
                        }
                    };

                    startDialogueMethod.Invoke(null, parameters);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"[Superb Recruitment] Failed to start RimTalk dialogue: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// 提取对话结果中的说服值变化
        /// </summary>
        public static float ExtractPersuasionDelta(object dialogueResult)
        {
            try
            {
                if (dialogueResult == null)
                    return 0f;

                // 通过反射从对话结果中提取数据
                var resultType = dialogueResult.GetType();
                var variablesField = resultType.GetField("Variables", BindingFlags.Public | BindingFlags.Instance);
                
                if (variablesField != null)
                {
                    var variables = variablesField.GetValue(dialogueResult) as Dictionary<string, object>;
                    if (variables != null && variables.ContainsKey("persuasionDelta"))
                    {
                        return Convert.ToSingle(variables["persuasionDelta"]);
                    }
                }

                // 如果没有明确的 persuasionDelta，根据对话质量计算
                var qualityField = resultType.GetField("Quality", BindingFlags.Public | BindingFlags.Instance);
                if (qualityField != null)
                {
                    var quality = qualityField.GetValue(dialogueResult);
                    return CalculateDeltaFromQuality(quality);
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"[Superb Recruitment] Error extracting persuasion delta: {ex.Message}");
            }

            return 0.05f; // 默认值
        }

        /// <summary>
        /// 根据对话质量计算说服值变化
        /// </summary>
        private static float CalculateDeltaFromQuality(object quality)
        {
            if (quality == null)
                return 0.05f;

            string qualityStr = quality.ToString();
            
            switch (qualityStr)
            {
                case "Excellent":
                    return Rand.Range(0.15f, 0.20f);
                case "Good":
                    return Rand.Range(0.10f, 0.15f);
                case "Average":
                    return Rand.Range(0.05f, 0.10f);
                case "Poor":
                    return Rand.Range(0.00f, 0.05f);
                case "Terrible":
                    return Rand.Range(-0.05f, 0.00f);
                default:
                    return 0.05f;
            }
        }
    }
}
