namespace CulinaryArts
{
    /// <summary>
    /// 菜系风格
    /// </summary>
    public enum CuisineStyle
    {
        Chinese = 0,
        Western = 1
    }

    /// <summary>
    /// 技法等级分层
    /// </summary>
    public enum TechniqueLevel
    {
        Survival = 0,      // 生存本能 0-5
        HomeCooking = 1,   // 烟火家常 6-12
        Gourmet = 2,       // 珍馐美馔 13-17
        Legendary = 3      // 登峰造极 18-20
    }

    /// <summary>
    /// 前缀品质等级
    /// </summary>
    public enum PrefixQuality
    {
        Terrible = -3,     // 负面
        None = 0,          // 无前缀
        Good = 3,          // 正面
        Legendary = 8      // 传说
    }

    /// <summary>
    /// 常量定义
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 时间窗口：6小时 = 15000 ticks
        /// </summary>
        public const int TICKS_PER_WINDOW = 15000;

        /// <summary>
        /// 种子生成用质数
        /// </summary>
        public const int SEED_MULTIPLIER = 397;
    }

    /// <summary>
    /// 双语字符串结构
    /// </summary>
    public struct BilingualString
    {
        public string CN;
        public string EN;

        public BilingualString(string cn, string en)
        {
            CN = cn;
            EN = en;
        }

        public override string ToString()
        {
            return LanguageHelper.IsChinese() ? CN : EN;
        }
    }

    /// <summary>
    /// 语言辅助类
    /// </summary>
    public static class LanguageHelper
    {
        /// <summary>
        /// 检测当前语言是否为中文（简体或繁体）
        /// </summary>
        public static bool IsChinese()
        {
            if (Verse.LanguageDatabase.activeLanguage == null) return false;
            string lang = Verse.LanguageDatabase.activeLanguage.LegacyFolderName;
            return lang == "ChineseSimplified" || lang == "ChineseTraditional";
        }
    }
}