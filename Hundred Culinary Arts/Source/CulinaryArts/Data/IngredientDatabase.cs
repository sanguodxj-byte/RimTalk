using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CulinaryArts
{
    /// <summary>
    /// 食材形态定义
    /// </summary>
    public class IngredientForms
    {
        public BilingualString[] Chinese { get; set; }
        public BilingualString[] Western { get; set; }
    }

    /// <summary>
    /// 食材形态映射数据库
    /// 将原始食材DefName映射为中西式烹饪形态
    /// 支持XML配置
    /// </summary>
    [StaticConstructorOnStartup]
    public static class IngredientDatabase
    {
        private static Dictionary<string, IngredientForms> database = new Dictionary<string, IngredientForms>();

        static IngredientDatabase()
        {
            // 在静态构造函数中无法访问 DefDatabase，因为此时 Def 可能还没加载完
            // 但 StaticConstructorOnStartup 保证在 Def 加载后执行
            InitializeDatabase();
        }

        /// <summary>
        /// 初始化食材映射数据库
        /// 从 XML Defs 中加载数据
        /// </summary>
        public static void InitializeDatabase()
        {
            try
            {
                database.Clear();

                // 检查 DefDatabase 是否可用
                if (DefDatabase<IngredientMappingDef>.AllDefsListForReading == null)
                {
                    return;
                }

                foreach (var def in DefDatabase<IngredientMappingDef>.AllDefsListForReading)
                {
                    if (string.IsNullOrEmpty(def.ingredientDefName)) continue;

                    var forms = new IngredientForms
                    {
                        Chinese = def.chineseForms?.Select(x => new BilingualString(x.cn, x.en)).ToArray() ?? new BilingualString[0],
                        Western = def.westernForms?.Select(x => new BilingualString(x.cn, x.en)).ToArray() ?? new BilingualString[0]
                    };

                    database[def.ingredientDefName] = forms;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[CulinaryArts] 初始化食材数据库时出错: {ex}");
            }
        }

        /// <summary>
        /// 获取食材的烹饪形态
        /// </summary>
        /// <param name="defName">食材DefName</param>
        /// <param name="style">菜系风格</param>
        /// <param name="rand">随机数生成器</param>
        /// <returns>食材形态名称</returns>
        public static string GetForm(string defName, CulinaryArts.CuisineStyle style, Random rand)
        {
            // 1. 直接匹配
            if (database.TryGetValue(defName, out var forms))
            {
                var pool = style == CulinaryArts.CuisineStyle.Chinese ? forms.Chinese : forms.Western;
                if (pool != null && pool.Length > 0)
                {
                    return pool[rand.Next(pool.Length)].ToString();
                }
            }

            // 2. 模糊匹配（例如 Meat_Muffalo -> Meat）
            // 优先匹配最长的键（最具体）
            string bestMatch = null;
            int maxLen = 0;

            foreach (var key in database.Keys)
            {
                if (defName.Contains(key) && key.Length > maxLen)
                {
                    bestMatch = key;
                    maxLen = key.Length;
                }
            }

            if (bestMatch != null)
            {
                var forms2 = database[bestMatch];
                var pool = style == CulinaryArts.CuisineStyle.Chinese ? forms2.Chinese : forms2.Western;
                if (pool != null && pool.Length > 0)
                {
                    return pool[rand.Next(pool.Length)].ToString();
                }
            }

            // 3. 回退机制：直接使用食材的 Label
            try
            {
                ThingDef def = DefDatabase<ThingDef>.GetNamed(defName, false);
                if (def != null)
                {
                    return def.LabelCap; // 自动处理当前语言的翻译
                }
            }
            catch
            {
                // 忽略错误
            }

            // 4. 最终回退：返回 DefName
            return defName;
        }

        /// <summary>
        /// 检查食材是否在数据库中
        /// </summary>
        public static bool HasMapping(string defName)
        {
            if (database.ContainsKey(defName))
                return true;

            foreach (var key in database.Keys)
            {
                if (defName.Contains(key))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 获取数据库中的食材数量（用于调试）
        /// </summary>
        public static int GetMappingCount()
        {
            return database.Count;
        }
    }
}