using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CulinaryArts
{
    /// <summary>
    /// 食谱图鉴窗口
    /// 允许玩家在游戏中查看已解锁的特殊食谱
    /// </summary>
    public class Window_RecipeCompendium : Window
    {
        private Vector2 scrollPosition = Vector2.zero;
        private string searchText = "";
        private int selectedTab = 0; // 0=全部, 1=已解锁, 2=未解锁

        public override Vector2 InitialSize => new Vector2(700f, 600f);

        public Window_RecipeCompendium()
        {
            this.doCloseButton = true;
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = false;
            this.forcePause = false;
            this.preventCameraMotion = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            bool isChinese = LanguageHelper.IsChinese();

            // 标题
            Text.Font = GameFont.Medium;
            string title = isChinese ? "边缘食谈 · 食谱图鉴" : "Recipe Compendium";
            Rect titleRect = new Rect(0, 0, inRect.width, 40);
            Widgets.Label(titleRect, title);
            Text.Font = GameFont.Small;

            float curY = 45f;

            // 获取图鉴数据
            CulinaryData data = null;
            if (Current.Game != null && Find.World != null)
            {
                data = Find.World.GetComponent<CulinaryData>();
            }

            int unlockedCount = data?.UnlockedCount ?? 0;
            int totalCount = SpecialRecipeDatabase.GetTotalCount();
            float completionRate = totalCount > 0 ? (float)unlockedCount / totalCount : 0f;
            float triggerChance = Mathf.Min(0.10f + (unlockedCount * 0.01f), 0.80f);
            bool canImitate = unlockedCount >= 30;

            // 进度条 - 使用简单的填充矩形
            Rect progressBarRect = new Rect(0, curY, inRect.width, 25);
            Widgets.DrawBoxSolid(progressBarRect, new Color(0.1f, 0.1f, 0.1f));
            Rect filledRect = new Rect(0, curY, inRect.width * completionRate, 25);
            Widgets.DrawBoxSolid(filledRect, new Color(0.2f, 0.6f, 0.2f));
            Widgets.DrawBox(progressBarRect);
            
            Text.Anchor = TextAnchor.MiddleCenter;
            string progressText = isChinese 
                ? $"图鉴进度: {unlockedCount} / {totalCount} ({completionRate:P0})" 
                : $"Progress: {unlockedCount} / {totalCount} ({completionRate:P0})";
            Widgets.Label(progressBarRect, progressText);
            Text.Anchor = TextAnchor.UpperLeft;
            curY += 30;

            // 统计信息
            Color prevColor = GUI.color;
            GUI.color = new Color(0.8f, 0.8f, 0.6f);
            string statsText = isChinese 
                ? $"当前触发率: {triggerChance:P0}  |  仿制能力: {(canImitate ? "已激活" : $"还差{30 - unlockedCount}个食谱解锁")}"
                : $"Trigger Rate: {triggerChance:P0}  |  Imitation: {(canImitate ? "Active" : $"Need {30 - unlockedCount} more to unlock")}";
            Rect statsRect = new Rect(0, curY, inRect.width, 22);
            Widgets.Label(statsRect, statsText);
            GUI.color = prevColor;
            curY += 28;

            // 标签页按钮
            float tabWidth = 100f;
            string[] tabLabels = isChinese 
                ? new[] { "全部", "已解锁", "未解锁" }
                : new[] { "All", "Unlocked", "Locked" };

            for (int i = 0; i < 3; i++)
            {
                Rect tabRect = new Rect(i * (tabWidth + 5), curY, tabWidth, 28);
                bool isSelected = selectedTab == i;
                
                if (isSelected)
                {
                    Widgets.DrawBoxSolid(tabRect, new Color(0.3f, 0.3f, 0.4f));
                }
                
                if (Widgets.ButtonText(tabRect, tabLabels[i], true, true, isSelected ? Color.yellow : Color.white))
                {
                    selectedTab = i;
                }
            }
            curY += 35;

            // 搜索框
            Rect searchLabelRect = new Rect(0, curY, 50, 25);
            Rect searchFieldRect = new Rect(55, curY, 200, 25);
            Widgets.Label(searchLabelRect, isChinese ? "搜索:" : "Search:");
            searchText = Widgets.TextField(searchFieldRect, searchText);
            curY += 32;

            // 分隔线
            Widgets.DrawLineHorizontal(0, curY, inRect.width);
            curY += 5;

            // 列表区域
            float listHeight = inRect.height - curY - 45; // 留出底部按钮空间
            Rect scrollOutRect = new Rect(0, curY, inRect.width, listHeight);

            // 获取并过滤食谱
            var allRecipes = DefDatabase<SpecialRecipeDef>.AllDefsListForReading ?? new List<SpecialRecipeDef>();
            var filteredRecipes = allRecipes.Where(r =>
            {
                // 标签页过滤
                bool isUnlocked = data?.IsUnlocked(r.defName) ?? false;
                if (selectedTab == 1 && !isUnlocked) return false;
                if (selectedTab == 2 && isUnlocked) return false;

                // 搜索过滤
                if (string.IsNullOrEmpty(searchText)) return true;
                string search = searchText.ToLower();
                string label = isChinese ? (r.chineseLabel ?? r.label) : r.label;
                return label.ToLower().Contains(search) || r.defName.ToLower().Contains(search);
            }).ToList();

            // 计算内容高度
            float rowHeight = 32f;
            float scrollViewHeight = filteredRecipes.Count * rowHeight;
            Rect scrollViewRect = new Rect(0, 0, scrollOutRect.width - 20, Mathf.Max(scrollViewHeight, listHeight));

            // 开始滚动区域
            Widgets.BeginScrollView(scrollOutRect, ref scrollPosition, scrollViewRect);

            float itemY = 0;
            int index = 0;
            foreach (var recipe in filteredRecipes)
            {
                Rect rowRect = new Rect(0, itemY, scrollViewRect.width, rowHeight - 2);
                bool isUnlocked = data?.IsUnlocked(recipe.defName) ?? false;

                // 交替背景色
                if (index % 2 == 0)
                {
                    Widgets.DrawBoxSolid(rowRect, new Color(0.15f, 0.15f, 0.15f, 0.5f));
                }
                else
                {
                    Widgets.DrawBoxSolid(rowRect, new Color(0.1f, 0.1f, 0.1f, 0.3f));
                }

                // 图标区域
                Rect iconRect = new Rect(rowRect.x + 8, rowRect.y + 6, 20, 20);
                
                // 名称区域
                Rect nameRect = new Rect(rowRect.x + 35, rowRect.y + 6, 220, rowHeight);
                
                // 食材区域
                Rect ingredientsRect = new Rect(rowRect.x + 260, rowRect.y + 6, scrollViewRect.width - 270, rowHeight);

                string recipeName = isChinese ? (recipe.chineseLabel ?? recipe.label) : recipe.label;
                string ingredientsText = recipe.requiredIngredients != null
                    ? string.Join(", ", recipe.requiredIngredients)
                    : "";

                if (isUnlocked)
                {
                    // 已解锁 - 显示绿色勾号和完整信息
                    prevColor = GUI.color;
                    GUI.color = new Color(0.4f, 1f, 0.4f);
                    Widgets.Label(iconRect, "✓");
                    GUI.color = Color.white;
                    Widgets.Label(nameRect, recipeName);
                    GUI.color = new Color(0.7f, 0.7f, 0.7f);
                    Widgets.Label(ingredientsRect, ingredientsText);
                    GUI.color = prevColor;
                }
                else
                {
                    // 未解锁 - 灰色显示
                    prevColor = GUI.color;
                    GUI.color = new Color(0.5f, 0.5f, 0.5f);
                    Widgets.Label(iconRect, "?");
                    string hiddenName = isChinese ? "???" : "???";
                    Widgets.Label(nameRect, hiddenName);
                    GUI.color = new Color(0.4f, 0.4f, 0.4f);
                    Widgets.Label(ingredientsRect, ingredientsText);
                    GUI.color = prevColor;
                }

                // 鼠标悬停高亮和提示
                if (Mouse.IsOver(rowRect))
                {
                    Widgets.DrawHighlight(rowRect);
                    
                    string tip;
                    if (isUnlocked)
                    {
                        tip = isChinese
                            ? $"【{recipeName}】\n需要食材: {ingredientsText}\n状态: ✓ 已解锁"
                            : $"【{recipeName}】\nIngredients: {ingredientsText}\nStatus: ✓ Unlocked";
                    }
                    else
                    {
                        tip = isChinese
                            ? $"需要食材: {ingredientsText}\n状态: 未解锁\n\n提示: 使用这些食材烹饪来解锁此食谱！"
                            : $"Ingredients: {ingredientsText}\nStatus: Locked\n\nHint: Cook with these ingredients to unlock!";
                    }
                    TooltipHandler.TipRegion(rowRect, tip);
                }

                itemY += rowHeight;
                index++;
            }

            Widgets.EndScrollView();

            // 底部信息
            float bottomY = inRect.height - 35;
            prevColor = GUI.color;
            GUI.color = new Color(0.6f, 0.6f, 0.6f);
            string footerText = isChinese
                ? $"显示 {filteredRecipes.Count} 个食谱"
                : $"Showing {filteredRecipes.Count} recipes";
            Widgets.Label(new Rect(0, bottomY, 200, 25), footerText);
            GUI.color = prevColor;
        }
    }
}