using System.Collections.Generic;
using Verse;

namespace CulinaryArts
{
    /// <summary>
    /// 食材形态映射定义
    /// 用于在XML中配置食材的中西式烹饪形态
    /// </summary>
    public class IngredientMappingDef : Def
    {
        /// <summary>
        /// 对应的食材DefName
        /// </summary>
        public string ingredientDefName;

        /// <summary>
        /// 中式形态列表
        /// </summary>
        public List<BilingualStringXml> chineseForms = new List<BilingualStringXml>();

        /// <summary>
        /// 西式形态列表
        /// </summary>
        public List<BilingualStringXml> westernForms = new List<BilingualStringXml>();

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (var error in base.ConfigErrors())
            {
                yield return error;
            }

            if (string.IsNullOrEmpty(ingredientDefName))
            {
                yield return "ingredientDefName cannot be null or empty";
            }
        }
    }

    /// <summary>
    /// 用于XML序列化的双语字符串辅助类
    /// </summary>
    public class BilingualStringXml
    {
        public string cn;
        public string en;

        public void LoadDataFromXmlCustom(System.Xml.XmlNode xmlRoot)
        {
            if (xmlRoot.ChildNodes.Count == 1 && xmlRoot.FirstChild.NodeType == System.Xml.XmlNodeType.Text)
            {
                cn = xmlRoot.InnerText;
                en = xmlRoot.InnerText;
            }
            else
            {
                foreach (System.Xml.XmlNode node in xmlRoot.ChildNodes)
                {
                    if (node.Name == "cn") cn = node.InnerText;
                    else if (node.Name == "en") en = node.InnerText;
                }
            }
        }
    }
}