using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace SuperbRecruitment
{
    /// <summary>
    /// AI 对话评估器
    /// 分析 RimTalk 对话内容并判断说服效果
    /// </summary>
    public static class AIDialogueEvaluator
    {
        // 正面关键词（说服相关）
        private static readonly HashSet<string> positiveKeywords = new HashSet<string>
        {
            // 中文
            "加入", "一起", "合作", "朋友", "帮助", "机会", "未来", "安全", "家", "归属",
            "信任", "希望", "梦想", "成功", "繁荣", "友善", "温暖", "欢迎", "需要", "重要",
            
            // 英文
            "join", "together", "cooperate", "friend", "help", "opportunity", "future", "safe", "home", "belong",
            "trust", "hope", "dream", "success", "prosper", "kind", "warm", "welcome", "need", "important"
        };

        // 负面关键词（可能降低说服效果）
        private static readonly HashSet<string> negativeKeywords = new HashSet<string>
        {
            // 中文
            "不", "拒绝", "离开", "危险", "害怕", "担心", "怀疑", "不信", "算了", "放弃",
            
            // 英文
            "no", "refuse", "leave", "danger", "fear", "worry", "doubt", "distrust", "nevermind", "give up"
        };

        /// <summary>
        /// 评估对话的说服效果
        /// </summary>
        public static float EvaluatePersuasion(DialogueContent content, Pawn persuader, Pawn target)
        {
            try
            {
                Log.Message($"[Superb Recruitment] Evaluating dialogue:\n{content}");

                float baseDelta = 0.08f;
                float multiplier = 1.0f;

                // 1. 分析玩家输入内容
                float inputScore = AnalyzeText(content.PlayerInput);
                multiplier += inputScore;

                // 2. 分析 AI 回应的情感倾向
                float responseScore = AnalyzeText(content.AIResponse);
                multiplier += responseScore * 0.5f; // AI 回应权重减半

                // 3. 使用 RimTalk 提供的质量评分
                float qualityBonus = GetQualityBonus(content.Quality);
                multiplier += qualityBonus;

                // 4. 使用情感分数
                if (content.Sentiment > 0)
                {
                    multiplier += content.Sentiment * 0.3f;
                }
                else if (content.Sentiment < 0)
                {
                    multiplier += content.Sentiment * 0.5f; // 负面情感影响更大
                }

                // 5. 说服者特性加成
                float traitBonus = GetPersuaderBonus(persuader);
                multiplier += traitBonus;

                // 6. 目标访客特性影响
                float targetModifier = GetTargetModifier(target);
                multiplier *= targetModifier;

                // 7. 对话长度加成（更长的对话更有说服力）
                float lengthBonus = GetLengthBonus(content.PlayerInput);
                multiplier += lengthBonus;

                // 计算最终结果
                float finalDelta = baseDelta * multiplier;

                // 限制范围在 -0.10 到 +0.25 之间
                finalDelta = Math.Max(-0.10f, Math.Min(0.25f, finalDelta));

                Log.Message($"[Superb Recruitment] Evaluation result: base={baseDelta}, multiplier={multiplier:F2}, final={finalDelta:F3}");

                return finalDelta;
            }
            catch (Exception ex)
            {
                Log.Error($"[Superb Recruitment] Error evaluating persuasion: {ex}");
                return 0.05f; // 默认值
            }
        }

        /// <summary>
        /// 分析文本内容
        /// </summary>
        private static float AnalyzeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0f;

            float score = 0f;
            string lowerText = text.ToLower();

            // 统计正面关键词
            int positiveCount = positiveKeywords.Count(keyword => lowerText.Contains(keyword));
            score += positiveCount * 0.15f;

            // 统计负面关键词
            int negativeCount = negativeKeywords.Count(keyword => lowerText.Contains(keyword));
            score -= negativeCount * 0.20f;

            // 文本长度影响（更详细的描述更有说服力）
            if (text.Length > 100)
                score += 0.1f;
            else if (text.Length > 50)
                score += 0.05f;

            // 问号表示询问，更友好
            if (text.Contains("?") || text.Contains("？"))
                score += 0.05f;

            // 感叹号表示热情
            int exclamationCount = text.Count(c => c == '!' || c == '！');
            score += Math.Min(exclamationCount * 0.03f, 0.1f);

            return score;
        }

        /// <summary>
        /// 根据 RimTalk 质量评分获取加成
        /// </summary>
        private static float GetQualityBonus(string quality)
        {
            if (string.IsNullOrEmpty(quality))
                return 0f;

            switch (quality.ToLower())
            {
                case "excellent":
                case "优秀":
                    return 0.5f;
                case "good":
                case "良好":
                    return 0.3f;
                case "average":
                case "一般":
                    return 0f;
                case "poor":
                case "较差":
                    return -0.2f;
                case "terrible":
                case "糟糕":
                    return -0.4f;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// 获取说服者加成
        /// </summary>
        private static float GetPersuaderBonus(Pawn persuader)
        {
            if (persuader == null)
                return 0f;

            float bonus = 0f;

            // 社交技能
            if (persuader.skills != null)
            {
                int socialSkill = persuader.skills.GetSkill(SkillDefOf.Social).Level;
                bonus += socialSkill * 0.02f; // 每级 +2%
            }

            // 特性
            if (persuader.story?.traits != null)
            {
                if (persuader.story.traits.HasTrait(TraitDefOf.Kind))
                    bonus += 0.2f;
                
                if (persuader.story.traits.HasTrait(TraitDefOf.Psychopath))
                    bonus -= 0.3f;
            }

            return bonus;
        }

        /// <summary>
        /// 获取目标修正
        /// </summary>
        private static float GetTargetModifier(Pawn target)
        {
            if (target == null)
                return 1.0f;

            float modifier = 1.0f;

            // 目标的心情影响接受度
            if (target.needs?.mood != null)
            {
                float moodLevel = target.needs.mood.CurLevel;
                if (moodLevel > 0.7f)
                    modifier += 0.2f; // 心情好更容易说服
                else if (moodLevel < 0.3f)
                    modifier -= 0.3f; // 心情差更难说服
            }

            // 目标特性
            if (target.story?.traits != null)
            {
                if (target.story.traits.HasTrait(TraitDefOf.Kind))
                    modifier += 0.15f; // 善良的人更容易说服
                
                if (target.story.traits.HasTrait(TraitDefOf.Greedy))
                    modifier -= 0.1f; // 贪婪的人更难说服（要求更多）
            }

            // 健康状况
            if (target.health != null)
            {
                float healthPercent = target.health.summaryHealth.SummaryHealthPercent;
                if (healthPercent < 0.5f)
                    modifier += 0.1f; // 受伤的人更需要帮助
            }

            return modifier;
        }

        /// <summary>
        /// 获取对话长度加成
        /// </summary>
        private static float GetLengthBonus(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0f;

            int length = text.Length;

            if (length > 200)
                return 0.15f;
            else if (length > 100)
                return 0.10f;
            else if (length > 50)
                return 0.05f;

            return 0f;
        }

        /// <summary>
        /// 分析对话历史的一致性
        /// </summary>
        public static float AnalyzeConsistency(List<string> history)
        {
            if (history == null || history.Count < 2)
                return 0f;

            // 如果对话历史一致（多次提到相同主题），加成
            // 这里可以实现更复杂的逻辑
            return 0.05f * Math.Min(history.Count - 1, 3); // 最多 3 轮对话加成
        }
    }
}
