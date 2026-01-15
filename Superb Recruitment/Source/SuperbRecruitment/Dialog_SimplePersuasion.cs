using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace SuperbRecruitment
{
    /// <summary>
    /// 简单说服对话窗口
    /// 当RimTalk不可用时使用
    /// </summary>
    public class Dialog_SimplePersuasion : Window
    {
        private Pawn visitor;
        private Hediff_PersuasionTracking persuasionHediff;
        private Action<float> onComplete;

        private List<PersuasionOption> options;
        private Vector2 scrollPosition = Vector2.zero;

        private const float WindowWidth = 600f;
        private const float WindowHeight = 500f;
        private const float OptionHeight = 80f;
        private const float Padding = 20f;

        public Dialog_SimplePersuasion(Pawn visitor, Hediff_PersuasionTracking hediff, Action<float> onComplete)
        {
            this.visitor = visitor;
            this.persuasionHediff = hediff;
            this.onComplete = onComplete;

            this.forcePause = true;
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = false;

            InitializeOptions();
        }

        public override Vector2 InitialSize => new Vector2(WindowWidth, WindowHeight);

        /// <summary>
        /// 初始化对话选项
        /// </summary>
        private void InitializeOptions()
        {
            options = new List<PersuasionOption>
            {
                new PersuasionOption
                {
                    label = "SuperbRecruitment_Option_FriendlyChat".Translate(),
                    description = "SuperbRecruitment_Option_FriendlyChat_Desc".Translate(),
                    minEffect = 0.05f,
                    maxEffect = 0.12f,
                    difficulty = 0.3f
                },
                new PersuasionOption
                {
                    label = "SuperbRecruitment_Option_ShareStory".Translate(),
                    description = "SuperbRecruitment_Option_ShareStory_Desc".Translate(),
                    minEffect = 0.08f,
                    maxEffect = 0.15f,
                    difficulty = 0.5f
                },
                new PersuasionOption
                {
                    label = "SuperbRecruitment_Option_OfferOpportunity".Translate(),
                    description = "SuperbRecruitment_Option_OfferOpportunity_Desc".Translate(),
                    minEffect = 0.10f,
                    maxEffect = 0.20f,
                    difficulty = 0.7f
                },
                new PersuasionOption
                {
                    label = "SuperbRecruitment_Option_EmotionalAppeal".Translate(),
                    description = "SuperbRecruitment_Option_EmotionalAppeal_Desc".Translate(),
                    minEffect = 0.00f,
                    maxEffect = 0.25f,
                    difficulty = 0.9f
                },
                new PersuasionOption
                {
                    label = "SuperbRecruitment_Option_SkipTalk".Translate(),
                    description = "SuperbRecruitment_Option_SkipTalk_Desc".Translate(),
                    minEffect = 0.02f,
                    maxEffect = 0.05f,
                    difficulty = 0.1f
                }
            };
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(inRect.x, inRect.y, inRect.width, 40f);
            Widgets.Label(titleRect, "SuperbRecruitment_PersuadeDialogTitle".Translate(visitor.LabelShort));

            Text.Font = GameFont.Small;
            
            // 访客信息
            Rect infoRect = new Rect(inRect.x, titleRect.yMax + 10f, inRect.width, 60f);
            DrawVisitorInfo(infoRect);

            // 对话选项
            Rect optionsRect = new Rect(
                inRect.x, 
                infoRect.yMax + 10f, 
                inRect.width, 
                inRect.height - infoRect.yMax - 70f
            );
            DrawOptions(optionsRect);

            // 底部按钮
            Rect bottomRect = new Rect(
                inRect.x, 
                inRect.yMax - 50f, 
                inRect.width, 
                40f
            );
            DrawBottomButtons(bottomRect);
        }

        /// <summary>
        /// 绘制访客信息
        /// </summary>
        private void DrawVisitorInfo(Rect rect)
        {
            Widgets.DrawBoxSolid(rect, new Color(0.2f, 0.2f, 0.2f, 0.5f));
            
            Rect textRect = rect.ContractedBy(10f);
            
            string info = "SuperbRecruitment_VisitorInfo".Translate(
                visitor.LabelShort,
                visitor.Faction?.Name ?? "Unknown",
                persuasionHediff.PlayerAttemptsRemaining
            );
            
            Widgets.Label(textRect, info);
        }

        /// <summary>
        /// 绘制对话选项
        /// </summary>
        private void DrawOptions(Rect rect)
        {
            Rect viewRect = new Rect(0f, 0f, rect.width - 20f, options.Count * (OptionHeight + 10f));
            
            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);

            float yOffset = 0f;
            foreach (var option in options)
            {
                Rect optionRect = new Rect(0f, yOffset, viewRect.width, OptionHeight);
                DrawOption(optionRect, option);
                yOffset += OptionHeight + 10f;
            }

            Widgets.EndScrollView();
        }

        /// <summary>
        /// 绘制单个选项
        /// </summary>
        private void DrawOption(Rect rect, PersuasionOption option)
        {
            // 背景
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            Widgets.DrawBoxSolid(rect, new Color(0.15f, 0.15f, 0.15f, 0.8f));

            Rect innerRect = rect.ContractedBy(8f);

            // 标签
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect labelRect = new Rect(innerRect.x, innerRect.y, innerRect.width, 25f);
            Widgets.Label(labelRect, option.label);

            // 描述
            Text.Font = GameFont.Tiny;
            Rect descRect = new Rect(innerRect.x, labelRect.yMax + 2f, innerRect.width, 30f);
            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            Widgets.Label(descRect, option.description);
            GUI.color = Color.white;

            // 难度指示器
            Text.Font = GameFont.Tiny;
            Rect difficultyRect = new Rect(innerRect.x, descRect.yMax + 2f, innerRect.width, 15f);
            string difficultyText = "SuperbRecruitment_Difficulty".Translate(GetDifficultyText(option.difficulty));
            Widgets.Label(difficultyRect, difficultyText);

            // 点击处理
            if (Widgets.ButtonInvisible(rect))
            {
                SelectOption(option);
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }

        /// <summary>
        /// 获取难度文本
        /// </summary>
        private string GetDifficultyText(float difficulty)
        {
            if (difficulty < 0.3f) return "SuperbRecruitment_DifficultyEasy".Translate();
            if (difficulty < 0.6f) return "SuperbRecruitment_DifficultyMedium".Translate();
            if (difficulty < 0.85f) return "SuperbRecruitment_DifficultyHard".Translate();
            return "SuperbRecruitment_DifficultyVeryHard".Translate();
        }

        /// <summary>
        /// 选择选项
        /// </summary>
        private void SelectOption(PersuasionOption option)
        {
            // 计算结果
            float successRoll = Rand.Value;
            float effectRoll = Rand.Value;

            float effect;
            if (successRoll > option.difficulty)
            {
                // 成功
                effect = Mathf.Lerp(option.minEffect, option.maxEffect, effectRoll);
            }
            else
            {
                // 失败
                effect = option.minEffect * effectRoll * 0.5f;
            }

            // 应用结果
            onComplete?.Invoke(effect);

            // 关闭窗口
            Close();
        }

        /// <summary>
        /// 绘制底部按钮
        /// </summary>
        private void DrawBottomButtons(Rect rect)
        {
            if (Widgets.ButtonText(rect, "SuperbRecruitment_CancelDialogue".Translate()))
            {
                Close();
            }
        }
    }

    /// <summary>
    /// 说服选项数据结构
    /// </summary>
    public class PersuasionOption
    {
        public string label;
        public string description;
        public float minEffect;
        public float maxEffect;
        public float difficulty; // 0.0 - 1.0
    }
}
