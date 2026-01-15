using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;

namespace CulinaryArts
{
    /// <summary>
    /// Mod 设置类
    /// 存储和管理用户配置
    /// </summary>
    public class CulinaryArtsSettings : ModSettings
    {
        /// <summary>
        /// 灵感模式：启用后厨师会扫描周围5格内的食材来触发特殊食谱
        /// </summary>
        public static bool SimulateDiversity = true;

        /// <summary>
        /// 调试日志：启用后在控制台输出详细日志
        /// </summary>
        public static bool ShowDebugLog = false;

        /// <summary>
        /// 存档保存/加载
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref SimulateDiversity, "simulateDiversity", true);
            Scribe_Values.Look(ref ShowDebugLog, "showDebugLog", false);
        }
    }

    /// <summary>
    /// Mod 主类（带设置界面和食谱图鉴）
    /// </summary>
    public class CulinaryArtsMod_Settings : Mod
    {
        public static CulinaryArtsSettings Settings;
        private Vector2 scrollPosition = Vector2.zero;
        private string searchText = "";

        public CulinaryArtsMod_Settings(ModContentPack content) : base(content)
        {
            Settings = GetSettings<CulinaryArtsSettings>();
        }

        public override string SettingsCategory()
        {
            return "厨间百艺 Culinary Arts";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            bool isChinese = LanguageHelper.IsChinese();
            
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            // === 设置区域 ===
            listing.Label(isChinese ? "【游戏设置】" : "【Game Settings】");
            listing.GapLine();

            // 灵感模式开关
            string inspirationLabel = isChinese ? "启用灵感模式" : "Enable Inspiration Mode";
            string inspirationDesc = isChinese 
                ? "厨师会扫描周围5格内的食材来辅助触发特殊食谱（不消耗这些食材）" 
                : "Chefs will scan ingredients within 5 tiles to help trigger special recipes";
            listing.CheckboxLabeled(inspirationLabel, ref CulinaryArtsSettings.SimulateDiversity, inspirationDesc);

            // 调试日志开关
            string debugLabel = isChinese ? "显示调试日志" : "Show Debug Logs";
            listing.CheckboxLabeled(debugLabel, ref CulinaryArtsSettings.ShowDebugLog);

            listing.Gap(20);

            // === 图鉴区域 ===
            listing.Label(isChinese ? "【边缘食谈 · 食谱图鉴】" : "【Recipe Compendium】");
            listing.GapLine();

            // 获取图鉴数据
            CulinaryData data = null;
            if (Current.Game != null && Find.World != null)
            {
                data = Find.World.GetComponent<CulinaryData>();
            }

            int unlockedCount = data?.UnlockedCount ?? 0;
            int totalCount = SpecialRecipeDatabase.GetTotalCount();
            float triggerChance = Mathf.Min(0.10f + (unlockedCount * 0.01f), 0.80f);
            bool canImitate = unlockedCount >= 30;

            // 统计信息
            string statsText = isChinese 
                ? $"已解锁: {unlockedCount} / {totalCount}  |  当前触发率: {triggerChance:P0}  |  仿制能力: {(canImitate ? "已激活" : $"还差{30 - unlockedCount}个")}"
                : $"Unlocked: {unlockedCount} / {totalCount}  |  Trigger Rate: {triggerChance:P0}  |  Imitation: {(canImitate ? "Active" : $"{30 - unlockedCount} more needed")}";
            listing.Label(statsText);

            listing.Gap(10);

            // 搜索框
            string searchLabel = isChinese ? "搜索: " : "Search: ";
            searchText = listing.TextEntryLabeled(searchLabel, searchText);

            listing.Gap(10);

            // 图鉴列表区域
            float listingHeight = listing.CurHeight;
            listing.End();

            // 计算剩余空间用于滚动列表
            float remainingHeight = inRect.height - listingHeight - 10;
            Rect scrollOutRect = new Rect(inRect.x, inRect.y + listingHeight, inRect.width, remainingHeight);
            
            // 获取所有食谱并过滤
            var allRecipes = DefDatabase<SpecialRecipeDef>.AllDefsListForReading ?? new List<SpecialRecipeDef>();
            var filteredRecipes = allRecipes.Where(r => 
            {
                if (string.IsNullOrEmpty(searchText)) return true;
                string search = searchText.ToLower();
                string label = isChinese ? (r.chineseLabel ?? r.label) : r.label;
                return label.ToLower().Contains(search) || r.defName.ToLower().Contains(search);
            }).ToList();

            // 计算滚动内容高度
            float rowHeight = 28f;
            float scrollViewHeight = filteredRecipes.Count * rowHeight;
            Rect scrollViewRect = new Rect(0, 0, scrollOutRect.width - 20, scrollViewHeight);

            // 开始滚动区域
            Widgets.BeginScrollView(scrollOutRect, ref scrollPosition, scrollViewRect);

            float curY = 0;
            foreach (var recipe in filteredRecipes)
            {
                Rect rowRect = new Rect(0, curY, scrollViewRect.width, rowHeight - 2);
                
                bool isUnlocked = data?.IsUnlocked(recipe.defName) ?? false;
                string recipeName = isChinese ? (recipe.chineseLabel ?? recipe.label) : recipe.label;
                string ingredientsText = recipe.requiredIngredients != null 
                    ? string.Join(", ", recipe.requiredIngredients) 
                    : "";

                // 背景色
                if (curY % (rowHeight * 2) < rowHeight)
                {
                    Widgets.DrawBoxSolid(rowRect, new Color(0.1f, 0.1f, 0.1f, 0.3f));
                }

                // 状态图标和名称
                Rect iconRect = new Rect(rowRect.x + 5, rowRect.y + 4, 20, 20);
                Rect nameRect = new Rect(rowRect.x + 30, rowRect.y + 4, 200, rowHeight);
                Rect ingredientsRect = new Rect(rowRect.x + 240, rowRect.y + 4, scrollViewRect.width - 250, rowHeight);

                if (isUnlocked)
                {
                    // 已解锁 - 绿色勾
                    GUI.color = Color.green;
                    Widgets.Label(iconRect, "✓");
                    GUI.color = Color.white;
                    Widgets.Label(nameRect, recipeName);
                    GUI.color = new Color(0.7f, 0.7f, 0.7f);
                    Widgets.Label(ingredientsRect, ingredientsText);
                    GUI.color = Color.white;
                }
                else
                {
                    // 未解锁 - 灰色问号，名称显示为???
                    GUI.color = new Color(0.5f, 0.5f, 0.5f);
                    Widgets.Label(iconRect, "?");
                    string hiddenName = isChinese ? "???" : "???";
                    Widgets.Label(nameRect, hiddenName);
                    Widgets.Label(ingredientsRect, ingredientsText);
                    GUI.color = Color.white;
                }

                // 鼠标悬停提示
                if (Mouse.IsOver(rowRect))
                {
                    Widgets.DrawHighlight(rowRect);
                    if (isUnlocked)
                    {
                        string tip = isChinese 
                            ? $"{recipeName}\n需要: {ingredientsText}\n状态: 已解锁" 
                            : $"{recipeName}\nRequires: {ingredientsText}\nStatus: Unlocked";
                        TooltipHandler.TipRegion(rowRect, tip);
                    }
                    else
                    {
                        string tip = isChinese 
                            ? $"需要: {ingredientsText}\n状态: 未解锁\n提示: 使用这些食材烹饪来解锁！" 
                            : $"Requires: {ingredientsText}\nStatus: Locked\nHint: Cook with these ingredients to unlock!";
                        TooltipHandler.TipRegion(rowRect, tip);
                    }
                }

                curY += rowHeight;
            }

            Widgets.EndScrollView();
        }
    }
}