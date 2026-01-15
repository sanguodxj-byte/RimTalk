using System;
using System.Collections.Generic;

namespace CulinaryArts
{
    /// <summary>
    /// 前缀数据库 v3.0
    /// 根据厨师技能等级提供不同品质的前缀和对应心情效果
    /// 已移除低质量词汇（家常、风味等）
    /// </summary>
    public static class PrefixDatabase
    {
        private static Dictionary<PrefixQuality, Dictionary<CuisineStyle, BilingualString[]>> database;

        static PrefixDatabase()
        {
            InitializeDatabase();
        }

        /// <summary>
        /// 初始化前缀数据库
        /// </summary>
        private static void InitializeDatabase()
        {
            database = new Dictionary<PrefixQuality, Dictionary<CuisineStyle, BilingualString[]>>
            {
                // === 负面前缀 (-3心情) ===
                [PrefixQuality.Terrible] = new Dictionary<CuisineStyle, BilingualString[]>
                {
                    [CuisineStyle.Chinese] = new[]
                    {
                        new BilingualString("焦糊的", "Burnt"),
                        new BilingualString("过咸的", "Oversalted"),
                        new BilingualString("夹生的", "Undercooked"),
                        new BilingualString("油腻的", "Greasy"),
                        new BilingualString("软烂的", "Mushy"),
                        new BilingualString("发苦的", "Bitter"),
                        new BilingualString("有异味的", "Smelly"),
                        new BilingualString("惨不忍睹的", "Awful")
                    },
                    [CuisineStyle.Western] = new[]
                    {
                        new BilingualString("焦糊的", "Burnt"),
                        new BilingualString("过咸的", "Oversalted"),
                        new BilingualString("夹生的", "Undercooked"),
                        new BilingualString("油腻的", "Greasy"),
                        new BilingualString("发苦的", "Bitter"),
                        new BilingualString("有异味的", "Smelly"),
                        new BilingualString("惨不忍睹的", "Awful"),
                        new BilingualString("变质的", "Spoiled")
                    }
                },

                // === 正面前缀 (+3心情) ===
                [PrefixQuality.Good] = new Dictionary<CuisineStyle, BilingualString[]>
                {
                    [CuisineStyle.Chinese] = new[]
                    {
                        new BilingualString("秘制", "Secret Recipe"),
                        new BilingualString("招牌", "Signature"),
                        new BilingualString("特色", "Specialty"),
                        new BilingualString("精选", "Select"),
                        new BilingualString("家传", "Heirloom"),
                        new BilingualString("风味", "Flavorful"),
                        new BilingualString("鲜香", "Savory"),
                        new BilingualString("可口", "Tasty")
                    },
                    [CuisineStyle.Western] = new[]
                    {
                        new BilingualString("招牌", "Signature"),
                        new BilingualString("经典", "Classic"),
                        new BilingualString("特制", "Special"),
                        new BilingualString("优质", "Fine"),
                        new BilingualString("美味", "Tasty"),
                        new BilingualString("家常", "Homemade"),
                        new BilingualString("乡村", "Rustic"),
                        new BilingualString("精选", "Select")
                    }
                },

                // === 传说前缀 (+8心情) ===
                [PrefixQuality.Legendary] = new Dictionary<CuisineStyle, BilingualString[]>
                {
                    [CuisineStyle.Chinese] = new[]
                    {
                        new BilingualString("御膳", "Imperial"),
                        new BilingualString("龙级", "Dragon-tier"),
                        new BilingualString("特级", "Grand"),
                        new BilingualString("至尊", "Supreme"),
                        new BilingualString("绝味", "Exquisite"),
                        new BilingualString("传世", "Heirloom"),
                        new BilingualString("神级", "Divine"),
                        new BilingualString("天香", "Heavenly")
                    },
                    [CuisineStyle.Western] = new[]
                    {
                        new BilingualString("盛大", "Grand"),
                        new BilingualString("皇家", "Royal"),
                        new BilingualString("至尊", "Supreme"),
                        new BilingualString("大师级", "Master"),
                        new BilingualString("终极", "Ultimate"),
                        new BilingualString("获奖", "Award-winning"),
                        new BilingualString("神圣", "Divine"),
                        new BilingualString("完美", "Perfect")
                    }
                }
            };
        }

        /// <summary>
        /// 根据技能等级生成前缀和心情效果
        /// </summary>
        /// <param name="skillLevel">烹饪技能等级 (0-20)</param>
        /// <param name="style">菜系风格</param>
        /// <param name="rand">随机数生成器</param>
        /// <returns>前缀字符串和心情值</returns>
        public static (string prefix, int mood) GeneratePrefix(int skillLevel, CuisineStyle style, Random rand)
        {
            float roll = (float)rand.NextDouble();
            PrefixQuality quality;

            // === 技能 0-5: 生存本能 ===
            // 5% 负面, 94% 无, 1% 正面 (总计 6% 有前缀)
            if (skillLevel <= 5)
            {
                if (roll < 0.05f)
                {
                    quality = PrefixQuality.Terrible;
                }
                else if (roll < 0.99f)
                {
                    return ("", 0); // 无前缀
                }
                else
                {
                    quality = PrefixQuality.Good;
                }
            }
            // === 技能 6-12: 烟火家常 ===
            // 2% 负面, 95% 无, 3% 正面 (总计 5% 有前缀)
            else if (skillLevel <= 12)
            {
                if (roll < 0.02f)
                {
                    quality = PrefixQuality.Terrible;
                }
                else if (roll < 0.97f)
                {
                    return ("", 0); // 无前缀
                }
                else
                {
                    quality = PrefixQuality.Good;
                }
            }
            // === 技能 13-17: 珍馐美馔 ===
            // 0% 负面, 92% 无, 7% 正面, 1% 传说 (总计 8% 有前缀)
            else if (skillLevel <= 17)
            {
                if (roll < 0.92f)
                {
                    return ("", 0); // 无前缀
                }
                else if (roll < 0.99f)
                {
                    quality = PrefixQuality.Good;
                }
                else
                {
                    quality = PrefixQuality.Legendary;
                }
            }
            // === 技能 18-20: 登峰造极 ===
            // 0% 负面, 85% 无, 10% 正面, 5% 传说 (总计 15% 有前缀)
            else
            {
                if (roll < 0.85f)
                {
                    return ("", 0); // 无前缀
                }
                else if (roll < 0.95f)
                {
                    quality = PrefixQuality.Good;
                }
                else
                {
                    quality = PrefixQuality.Legendary;
                }
            }

            // 从对应品质和风格的词库中随机选择
            string prefix = GetRandomPrefix(quality, style, rand);
            return (prefix, (int)quality);
        }

        /// <summary>
        /// 从指定品质和风格的词库中随机获取前缀
        /// </summary>
        private static string GetRandomPrefix(PrefixQuality quality, CuisineStyle style, Random rand)
        {
            var prefixes = database[quality][style];
            return prefixes[rand.Next(prefixes.Length)].ToString();
        }

        /// <summary>
        /// 获取指定品质和风格的前缀数量（用于调试）
        /// </summary>
        public static int GetPrefixCount(PrefixQuality quality, CuisineStyle style)
        {
            return database[quality][style].Length;
        }

        /// <summary>
        /// 获取所有前缀总数（用于调试）
        /// </summary>
        public static int GetTotalPrefixCount()
        {
            int count = 0;
            foreach (var qualityData in database.Values)
            {
                foreach (var styleData in qualityData.Values)
                {
                    count += styleData.Length;
                }
            }
            return count;
        }
    }
}