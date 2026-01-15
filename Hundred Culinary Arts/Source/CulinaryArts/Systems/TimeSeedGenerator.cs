using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CulinaryArts
{
    /// <summary>
    /// 时间种子生成器
    /// 确保同厨师+同食材在6小时内产生相同的随机种子
    /// </summary>
    public static class TimeSeedGenerator
    {
        /// <summary>
        /// 生成基于厨师、食材、食物类型和时间窗口的种子
        /// </summary>
        /// <param name="chef">烹饪的小人</param>
        /// <param name="mealDef">生成的食物类型定义</param>
        /// <param name="ingredients">使用的食材列表</param>
        /// <returns>确定性种子值</returns>
        public static int GenerateSeed(Pawn chef, ThingDef mealDef, List<Thing> ingredients)
        {
            // 厨师ID哈希
            int pawnHash = chef.thingIDNumber * Constants.SEED_MULTIPLIER;

            // 食物类型哈希
            int defHash = mealDef?.defName.GetHashCode() ?? 0;

            // 食材哈希（排序后确保顺序无关）
            int ingredientHash = GetIngredientHash(ingredients);

            // 时间窗口（整除确保离散性）
            int timeWindow = Find.TickManager.TicksGame / Constants.TICKS_PER_WINDOW;

            // 异或组合（保证均匀分布）
            return pawnHash ^ defHash ^ ingredientHash ^ timeWindow;
        }

        /// <summary>
        /// 计算食材列表的哈希值
        /// 按DefName排序后计算，确保食材顺序不影响结果
        /// </summary>
        private static int GetIngredientHash(List<Thing> ingredients)
        {
            if (ingredients == null || ingredients.Count == 0)
                return 0;

            int hash = 0;

            // 按DefName排序确保一致性
            var sortedDefNames = ingredients
                .Select(t => t.def.defName)
                .OrderBy(name => name)
                .ToList();

            foreach (var defName in sortedDefNames)
            {
                hash ^= defName.GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// 获取当前时间窗口编号（用于调试）
        /// </summary>
        public static int GetCurrentTimeWindow()
        {
            return Find.TickManager.TicksGame / Constants.TICKS_PER_WINDOW;
        }

        /// <summary>
        /// 获取距离下一个时间窗口的剩余tick数（用于调试）
        /// </summary>
        public static int GetTicksUntilNextWindow()
        {
            int currentTicks = Find.TickManager.TicksGame;
            int nextWindowTick = (currentTicks / Constants.TICKS_PER_WINDOW + 1) * Constants.TICKS_PER_WINDOW;
            return nextWindowTick - currentTicks;
        }
    }
}