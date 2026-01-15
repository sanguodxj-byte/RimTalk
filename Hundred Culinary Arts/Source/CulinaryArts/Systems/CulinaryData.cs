using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CulinaryArts
{
    /// <summary>
    /// 世界组件：存储已解锁的食谱图鉴数据
    /// 用于实现"Roguelike 收集"体验
    /// </summary>
    public class CulinaryData : WorldComponent
    {
        private HashSet<string> unlockedRecipes = new HashSet<string>();

        public CulinaryData(World world) : base(world) { }

        /// <summary>
        /// 获取图鉴完成率 (0.0 到 1.0)
        /// 用于驱动特殊食谱触发概率和模仿能力
        /// </summary>
        public float CompletionRate
        {
            get
            {
                int total = SpecialRecipeDatabase.GetTotalCount();
                return total == 0 ? 0f : (float)unlockedRecipes.Count / total;
            }
        }

        /// <summary>
        /// 已解锁的食谱数量
        /// </summary>
        public int UnlockedCount => unlockedRecipes.Count;

        /// <summary>
        /// 解锁一个新食谱
        /// </summary>
        /// <param name="recipeKey">食谱的 DefName</param>
        /// <returns>是否是新解锁的（true 表示首次解锁）</returns>
        public bool Unlock(string recipeKey)
        {
            if (!unlockedRecipes.Contains(recipeKey))
            {
                unlockedRecipes.Add(recipeKey);
                Messages.Message($"【厨艺大成】解锁新食谱图鉴：{recipeKey}", MessageTypeDefOf.PositiveEvent);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查食谱是否已解锁
        /// </summary>
        public bool IsUnlocked(string recipeKey) => unlockedRecipes.Contains(recipeKey);

        /// <summary>
        /// 存档保存/加载
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref unlockedRecipes, "unlockedRecipes", LookMode.Value);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && unlockedRecipes == null)
            {
                unlockedRecipes = new HashSet<string>();
            }
        }
    }
}