using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Verse;
using RimWorld;
using HarmonyLib;

namespace SuperbRecruitment
{
    /// <summary>
    /// RimTalk 对话内容注入器
    /// 类似 RimTalkExpandMemory 的实现，动态注入自定义对话节点
    /// </summary>
    [StaticConstructorOnStartup]
    public static class RimTalkDialogueInjector
    {
        private static bool injectionAttempted = false;
        private static Type dialogueNodeType = null;
        private static Type dialogueTreeType = null;

        static RimTalkDialogueInjector()
        {
            // 在游戏启动时自动尝试注入
            LongEventHandler.ExecuteWhenFinished(InjectDialogueContent);
        }

        /// <summary>
        /// 注入自定义对话内容到 RimTalk
        /// </summary>
        public static void InjectDialogueContent()
        {
            if (injectionAttempted)
                return;

            injectionAttempted = true;

            try
            {
                Log.Message("[Superb Recruitment] Attempting to inject dialogue content into RimTalk...");

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
                    Log.Message("[Superb Recruitment] RimTalk not found, injection skipped");
                    return;
                }

                // 获取关键类型
                dialogueNodeType = rimTalkAssembly.GetType("RimTalk.DialogueNode");
                dialogueTreeType = rimTalkAssembly.GetType("RimTalk.DialogueTree");
                var dialogueDatabaseType = rimTalkAssembly.GetType("RimTalk.DialogueDatabase");

                if (dialogueNodeType == null || dialogueTreeType == null || dialogueDatabaseType == null)
                {
                    Log.Warning("[Superb Recruitment] RimTalk types not found, using reflection fallback");
                    return;
                }

                // 创建说服对话树
                var persuasionTree = CreatePersuasionDialogueTree();

                // 注入到 RimTalk 数据库
                var addTreeMethod = dialogueDatabaseType.GetMethod(
                    "AddDialogueTree",
                    BindingFlags.Public | BindingFlags.Static
                );

                if (addTreeMethod != null && persuasionTree != null)
                {
                    addTreeMethod.Invoke(null, new object[] { persuasionTree });
                    Log.Message("[Superb Recruitment] Successfully injected persuasion dialogue tree into RimTalk");
                }
                else
                {
                    Log.Warning("[Superb Recruitment] Could not inject dialogue tree - method not found");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error injecting dialogue content: {ex}");
            }
        }

        /// <summary>
        /// 创建说服对话树
        /// </summary>
        private static object CreatePersuasionDialogueTree()
        {
            try
            {
                if (dialogueTreeType == null)
                    return null;

                // 使用反射创建对话树实例
                var tree = Activator.CreateInstance(dialogueTreeType);

                // 设置基本属性
                SetProperty(tree, "TreeId", "SuperbRecruitment_Persuasion");
                SetProperty(tree, "Title", "Persuade Visitor");
                SetProperty(tree, "Description", "Attempt to convince a visitor to join your colony");

                // 创建对话节点
                var nodes = CreatePersuasionNodes();
                SetProperty(tree, "Nodes", nodes);

                // 设置触发条件
                var conditions = CreateTriggerConditions();
                SetProperty(tree, "Conditions", conditions);

                return tree;
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error creating dialogue tree: {ex}");
                return null;
            }
        }

        /// <summary>
        /// 创建说服对话节点
        /// </summary>
        private static List<object> CreatePersuasionNodes()
        {
            var nodes = new List<object>();

            try
            {
                if (dialogueNodeType == null)
                    return nodes;

                // 根节点 - 开场白
                var rootNode = CreateNode(
                    "start",
                    "你好，我想和你谈谈关于加入我们殖民地的事。",
                    new List<string> { "friendly", "opportunity", "emotional" }
                );
                nodes.Add(rootNode);

                // 友好路线节点
                var friendlyNode = CreateNode(
                    "friendly",
                    "我们殖民地的人都很友善，你会喜欢这里的生活。",
                    new List<string> { "friendly_response" }
                );
                nodes.Add(friendlyNode);

                // 机会路线节点
                var opportunityNode = CreateNode(
                    "opportunity",
                    "加入我们可以获得更好的发展机会和资源。",
                    new List<string> { "opportunity_response" }
                );
                nodes.Add(opportunityNode);

                // 情感路线节点
                var emotionalNode = CreateNode(
                    "emotional",
                    "在这个危险的边缘世界，我们需要互相扶持才能生存。",
                    new List<string> { "emotional_response" }
                );
                nodes.Add(emotionalNode);

                // 回应节点（由访客回复）
                AddResponseNodes(nodes);
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error creating nodes: {ex}");
            }

            return nodes;
        }

        /// <summary>
        /// 创建单个对话节点
        /// </summary>
        private static object CreateNode(string id, string text, List<string> nextNodeIds)
        {
            try
            {
                if (dialogueNodeType == null)
                    return null;

                var node = Activator.CreateInstance(dialogueNodeType);
                
                SetProperty(node, "NodeId", id);
                SetProperty(node, "Text", text);
                SetProperty(node, "NextNodes", nextNodeIds);
                SetProperty(node, "SpeakerType", "Initiator"); // 发起者（殖民者）

                return node;
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error creating node {id}: {ex}");
                return null;
            }
        }

        /// <summary>
        /// 添加访客回应节点
        /// </summary>
        private static void AddResponseNodes(List<object> nodes)
        {
            try
            {
                // 友好回应
                var friendlyResponse = CreateVisitorResponseNode(
                    "friendly_response",
                    "嗯，听起来不错。",
                    0.10f
                );
                nodes.Add(friendlyResponse);

                // 机会回应
                var opportunityResponse = CreateVisitorResponseNode(
                    "opportunity_response",
                    "确实是个值得考虑的机会。",
                    0.15f
                );
                nodes.Add(opportunityResponse);

                // 情感回应
                var emotionalResponse = CreateVisitorResponseNode(
                    "emotional_response",
                    "你说得有道理...让我想想。",
                    0.12f
                );
                nodes.Add(emotionalResponse);
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error adding response nodes: {ex}");
            }
        }

        /// <summary>
        /// 创建访客回应节点（带说服值变化）
        /// </summary>
        private static object CreateVisitorResponseNode(string id, string text, float persuasionDelta)
        {
            try
            {
                if (dialogueNodeType == null)
                    return null;

                var node = Activator.CreateInstance(dialogueNodeType);
                
                SetProperty(node, "NodeId", id);
                SetProperty(node, "Text", text);
                SetProperty(node, "SpeakerType", "Target"); // 目标（访客）
                SetProperty(node, "IsEndNode", true);

                // 添加自定义数据 - 说服值变化
                var customData = new Dictionary<string, object>
                {
                    { "persuasionDelta", persuasionDelta },
                    { "modId", "SuperbRecruitment" }
                };
                SetProperty(node, "CustomData", customData);

                return node;
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error creating response node {id}: {ex}");
                return null;
            }
        }

        /// <summary>
        /// 创建触发条件
        /// </summary>
        private static List<object> CreateTriggerConditions()
        {
            var conditions = new List<object>();

            try
            {
                // 条件1: 目标必须是访客
                conditions.Add(new Dictionary<string, object>
                {
                    { "Type", "PawnStatus" },
                    { "Status", "Guest" }
                });

                // 条件2: 目标必须是非敌对阵营
                conditions.Add(new Dictionary<string, object>
                {
                    { "Type", "FactionRelation" },
                    { "Relation", "Neutral|Ally" }
                });

                // 条件3: 目标必须有说服追踪 Hediff
                conditions.Add(new Dictionary<string, object>
                {
                    { "Type", "HasHediff" },
                    { "HediffDef", "SuperbRecruitment_PersuasionTracking" }
                });
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error creating conditions: {ex}");
            }

            return conditions;
        }

        /// <summary>
        /// 辅助方法：设置对象属性
        /// </summary>
        private static void SetProperty(object obj, string propertyName, object value)
        {
            if (obj == null)
                return;

            try
            {
                var type = obj.GetType();
                var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                
                if (property != null && property.CanWrite)
                {
                    property.SetValue(obj, value);
                }
                else
                {
                    // 尝试字段
                    var field = type.GetField(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    if (field != null)
                    {
                        field.SetValue(obj, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"[Superb Recruitment] Could not set property {propertyName}: {ex.Message}");
            }
        }

        /// <summary>
        /// 为访客启用自定义对话
        /// </summary>
        public static void EnableCustomDialogueForVisitor(Pawn visitor)
        {
            if (visitor == null || !visitor.IsColonist && visitor.guest?.GuestStatus != GuestStatus.Guest)
                return;

            try
            {
                // 添加标记，表示这个访客可以进行说服对话
                var comp = visitor.TryGetComp<CompDialogueEnabled>();
                if (comp == null)
                {
                    // 如果没有 Comp，通过 Hediff 标记
                    var hediff = visitor.health?.hediffSet?.GetFirstHediffOfDef(
                        DefDatabase<HediffDef>.GetNamed("SuperbRecruitment_PersuasionTracking", false)
                    );

                    if (hediff != null)
                    {
                        Log.Message($"[Superb Recruitment] Enabled custom dialogue for visitor: {visitor.LabelShort}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error enabling dialogue for visitor: {ex}");
            }
        }
    }

    /// <summary>
    /// 对话启用组件（可选）
    /// </summary>
    public class CompDialogueEnabled : ThingComp
    {
        public bool customDialogueEnabled = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref customDialogueEnabled, "customDialogueEnabled", false);
        }
    }
}
