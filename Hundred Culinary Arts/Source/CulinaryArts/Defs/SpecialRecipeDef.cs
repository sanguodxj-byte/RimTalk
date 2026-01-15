using System.Collections.Generic;
using Verse;

namespace CulinaryArts
{
    /// <summary>
    /// 特殊食谱定义
    /// 用于在 XML 中配置特殊食谱（彩蛋菜名）
    /// </summary>
    public class SpecialRecipeDef : Def
    {
        /// <summary>
        /// 中文标签名称
        /// </summary>
        public string chineseLabel;

        /// <summary>
        /// 所需食材关键词列表
        /// 支持模糊匹配（例如 "Pork" 可匹配 "Meat_Pork"）
        /// </summary>
        public List<string> requiredIngredients;

        /// <summary>
        /// 是否需要精确匹配食材数量
        /// 如果为 true，实际食材数量必须与 requiredIngredients.Count 相等
        /// </summary>
        public bool exactMatch = false;
    }
}