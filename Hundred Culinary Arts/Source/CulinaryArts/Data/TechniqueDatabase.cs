using System;
using System.Collections.Generic;
using Verse;

namespace CulinaryArts
{
    /// <summary>
    /// 技法数据库 v3.0
    /// 根据厨师技能等级提供不同层次的烹饪技法
    /// 已移除低质量词汇
    /// </summary>
    public static class TechniqueDatabase
    {
        private static Dictionary<TechniqueLevel, Dictionary<CuisineStyle, BilingualString[]>> database;

        static TechniqueDatabase()
        {
            InitializeDatabase();
        }

        /// <summary>
        /// 初始化技法数据库
        /// </summary>
        private static void InitializeDatabase()
        {
            database = new Dictionary<TechniqueLevel, Dictionary<CuisineStyle, BilingualString[]>>
            {
                // === Tier 1: 生存本能 (0-5级) - 基础烹饪 ===
                [TechniqueLevel.Survival] = new Dictionary<CuisineStyle, BilingualString[]>
                {
                    [CuisineStyle.Chinese] = new[]
                    {
                        new BilingualString("水煮", "Boiled"),
                        new BilingualString("炭烤", "Charcoal Grilled"),
                        new BilingualString("凉拌", "Cold Dressed"),
                        new BilingualString("炙烤", "Broiled"),
                        new BilingualString("杂烩", "Hodgepodge"),
                        new BilingualString("腌制", "Pickled"),
                        new BilingualString("烟熏", "Smoked"),
                        new BilingualString("糊", "Mushy"),
                        new BilingualString("乱炖", "Stewed"),
                        new BilingualString("生拌", "Raw Tossed")
                    },
                    [CuisineStyle.Western] = new[]
                    {
                        new BilingualString("烤", "Grilled"),
                        new BilingualString("煮", "Boiled"),
                        new BilingualString("烘焙", "Roasted"),
                        new BilingualString("炸", "Fried"),
                        new BilingualString("泥", "Mashed"),
                        new BilingualString("烟熏", "Smoked"),
                        new BilingualString("生", "Raw"),
                        new BilingualString("烤", "Toasted"),
                        new BilingualString("蒸", "Steamed"),
                        new BilingualString("切碎", "Chopped")
                    }
                },

                // === Tier 2: 烟火家常 (6-12级) - 常见家常菜式 ===
                [TechniqueLevel.HomeCooking] = new Dictionary<CuisineStyle, BilingualString[]>
                {
                    [CuisineStyle.Chinese] = new[]
                    {
                        new BilingualString("红烧", "Red Braised"),
                        new BilingualString("爆炒", "Stir-fried"),
                        new BilingualString("清蒸", "Steamed"),
                        new BilingualString("干煸", "Dry-fried"),
                        new BilingualString("糖醋", "Sweet and Sour"),
                        new BilingualString("酱爆", "Soy Sauce Fried"),
                        new BilingualString("油焖", "Braised in Oil"),
                        new BilingualString("蒜蓉", "Garlic"),
                        new BilingualString("葱烧", "Scallion Braised"),
                        new BilingualString("椒盐", "Salt and Pepper"),
                        new BilingualString("回锅", "Double-cooked"),
                        new BilingualString("粉蒸", "Steamed with Rice Flour"),
                        new BilingualString("溜", "Sautéed with Starch"),
                        new BilingualString("烩", "Braised"),
                        new BilingualString("焖", "Simmered"),
                        new BilingualString("炸", "Deep-fried"),
                        new BilingualString("煎", "Pan-fried")
                    },
                    [CuisineStyle.Western] = new[]
                    {
                        new BilingualString("嫩煎", "Sautéed"),
                        new BilingualString("烘焙", "Baked"),
                        new BilingualString("炖", "Stewed"),
                        new BilingualString("奶油", "Creamy"),
                        new BilingualString("脆皮", "Crispy"),
                        new BilingualString("挂霜", "Glazed"),
                        new BilingualString("焖烧", "Braised"),
                        new BilingualString("香煎", "Pan-seared"),
                        new BilingualString("油炸", "Deep-fried"),
                        new BilingualString("烧烤", "BBQ"),
                        new BilingualString("黄油", "Buttered"),
                        new BilingualString("芝士", "Cheesy"),
                        new BilingualString("煎封", "Pan-sealed"),
                        new BilingualString("炙烤", "Broiled")
                    }
                },

                // === Tier 3: 珍馐美馔 (13-17级) - 餐厅级/特色菜 ===
                [TechniqueLevel.Gourmet] = new Dictionary<CuisineStyle, BilingualString[]>
                {
                    [CuisineStyle.Chinese] = new[]
                    {
                        new BilingualString("鲍汁", "Abalone Sauce"),
                        new BilingualString("芙蓉", "Hibiscus"),
                        new BilingualString("避风塘", "Typhoon Shelter Style"),
                        new BilingualString("宫廷", "Imperial"),
                        new BilingualString("砂锅", "Casserole"),
                        new BilingualString("蜜汁", "Honey Glazed"),
                        new BilingualString("豉汁", "Black Bean Sauce"),
                        new BilingualString("鱼香", "Fish-flavored"),
                        new BilingualString("宫保", "Kung Pao"),
                        new BilingualString("拔丝", "Candied"),
                        new BilingualString("酥炸", "Crispy Fried"),
                        new BilingualString("软溜", "Soft Sautéed"),
                        new BilingualString("糟溜", "Wine Lees"),
                        new BilingualString("醉", "Drunken"),
                        new BilingualString("三杯", "Three Cup"),
                        new BilingualString("东坡", "Dongpo"),
                        new BilingualString("手撕", "Hand-shredded"),
                        new BilingualString("铁板", "Sizzling"),
                        new BilingualString("石锅", "Stone Pot"),
                        new BilingualString("茶香", "Tea Smoked"),
                        new BilingualString("果木", "Fruit Wood"),
                        new BilingualString("挂炉", "Oven Roasted")
                    },
                    [CuisineStyle.Western] = new[]
                    {
                        new BilingualString("水波", "Poached"),
                        new BilingualString("焦糖", "Caramelized"),
                        new BilingualString("油封", "Confit"),
                        new BilingualString("塔塔", "Tartare"),
                        new BilingualString("生薄片", "Carpaccio"),
                        new BilingualString("蓝带", "Cordon Bleu"),
                        new BilingualString("惠灵顿", "Wellington"),
                        new BilingualString("佛罗伦萨", "Florentine"),
                        new BilingualString("普罗旺斯", "Provençal"),
                        new BilingualString("荷兰酱", "Hollandaise"),
                        new BilingualString("贝加奈斯", "Béarnaise"),
                        new BilingualString("勃艮第", "Bourguignon"),
                        new BilingualString("玛萨拉", "Marsala"),
                        new BilingualString("柠檬", "Piccata"),
                        new BilingualString("焗", "Au Gratin"),
                        new BilingualString("浓缩", "Reduction"),
                        new BilingualString("浸渍", "Infused"),
                        new BilingualString("慢煮", "Slow-cooked")
                    }
                },

                // === Tier 4: 登峰造极 (18-20级) - 顶级/国宴级/分子料理 ===
                [TechniqueLevel.Legendary] = new Dictionary<CuisineStyle, BilingualString[]>
                {
                    [CuisineStyle.Chinese] = new[]
                    {
                        new BilingualString("文火慢炖", "Gentle Simmer"),
                        new BilingualString("高汤煨", "Rich Broth Stewed"),
                        new BilingualString("蜜炙", "Honey Roasted"),
                        new BilingualString("挂浆", "Battered"),
                        new BilingualString("过油拔丝", "Oil-passed Candied"),
                        new BilingualString("隔水蒸", "Double-boiled"),
                        new BilingualString("炖盅", "Stewed in Pot"),
                        new BilingualString("上浆", "Velveted"),
                        new BilingualString("勾芡", "Thickened"),
                        new BilingualString("油泡", "Oil-blanched"),
                        new BilingualString("汽蒸", "Steam-cooked"),
                        new BilingualString("煨", "Simmered"),
                        new BilingualString("焐", "Slow-braised"),
                        new BilingualString("煲", "Clay Pot Cooked"),
                        new BilingualString("旺火收汁", "High Heat Reduced")
                    },
                    [CuisineStyle.Western] = new[]
                    {
                        new BilingualString("低温慢煮", "Sous-vide"),
                        new BilingualString("分子料理", "Molecular"),
                        new BilingualString("解构重组", "Deconstructed"),
                        new BilingualString("熟成", "Aged"),
                        new BilingualString("球化", "Spherified"),
                        new BilingualString("液氮速冻", "Nitro-frozen"),
                        new BilingualString("真空低温", "Vacuum Low-temp"),
                        new BilingualString("烟熏慢烤", "Smoked and Slow-roasted"),
                        new BilingualString("浸渍萃取", "Infused"),
                        new BilingualString("焦糖化", "Caramelized"),
                        new BilingualString("胶化", "Gelified"),
                        new BilingualString("泡沫化", "Foamed"),
                        new BilingualString("晶球", "Spherification"),
                        new BilingualString("冷萃", "Cold-extracted"),
                        new BilingualString("精准控温", "Precision-tempered")
                    }
                }
            };
        }

        /// <summary>
        /// 根据技能等级获取技法
        /// </summary>
        /// <param name="skillLevel">烹饪技能等级 (0-20)</param>
        /// <param name="style">菜系风格</param>
        /// <param name="mealDef">食物类型定义</param>
        /// <param name="rand">随机数生成器</param>
        /// <returns>技法名称</returns>
        public static string GetTechnique(int skillLevel, CuisineStyle style, ThingDef mealDef, Random rand)
        {
            // 1. 获取厨师技能允许的最高Tier
            TechniqueLevel maxSkillTier = GetMaxTierForSkill(skillLevel);

            // 2. 获取食物类型偏好的Tier范围
            var (prefMin, prefMax) = GetPreferredTierRange(mealDef);

            // 3. 在技能限制下，尽量选择偏好范围内的Tier
            TechniqueLevel selectedTier = SelectTierWithPreference(maxSkillTier, prefMin, prefMax, rand);

            var techniques = database[selectedTier][style];
            return techniques[rand.Next(techniques.Length)].ToString();
        }

        /// <summary>
        /// 根据技能等级确定最高可用Tier
        /// </summary>
        private static TechniqueLevel GetMaxTierForSkill(int skill)
        {
            if (skill <= 5) return TechniqueLevel.Survival;
            if (skill <= 12) return TechniqueLevel.HomeCooking;
            if (skill <= 17) return TechniqueLevel.Gourmet;
            return TechniqueLevel.Legendary;
        }

        /// <summary>
        /// 获取食物类型的偏好Tier范围
        /// </summary>
        private static (TechniqueLevel min, TechniqueLevel max) GetPreferredTierRange(ThingDef mealDef)
        {
            if (mealDef == null) return (TechniqueLevel.Survival, TechniqueLevel.Legendary);

            string defName = mealDef.defName;

            // 奢侈食物 (Lavish): 偏好 Gourmet(2) - Legendary(3)
            if (defName.Contains("Lavish"))
            {
                return (TechniqueLevel.Gourmet, TechniqueLevel.Legendary);
            }

            // 精致食物 (Fine): 偏好 HomeCooking(1) - Gourmet(2)
            if (defName.Contains("Fine"))
            {
                return (TechniqueLevel.HomeCooking, TechniqueLevel.Gourmet);
            }

            // 简单食物 (Simple/Survival/Nutrient): 偏好 Survival(0) - HomeCooking(1)
            return (TechniqueLevel.Survival, TechniqueLevel.HomeCooking);
        }

        /// <summary>
        /// 在技能限制下选择Tier，优先考虑偏好范围
        /// </summary>
        private static TechniqueLevel SelectTierWithPreference(TechniqueLevel maxSkillTier, TechniqueLevel prefMin, TechniqueLevel prefMax, Random rand)
        {
            var candidates = new List<TechniqueLevel>();
            var preferredCandidates = new List<TechniqueLevel>();

            // 遍历所有厨师能掌握的Tier
            for (int i = 0; i <= (int)maxSkillTier; i++)
            {
                TechniqueLevel current = (TechniqueLevel)i;
                candidates.Add(current);

                // 如果在偏好范围内，加入偏好列表
                if (current >= prefMin && current <= prefMax)
                {
                    preferredCandidates.Add(current);
                }
            }

            // 如果有偏好范围内的可用Tier，给予更高权重 (80%概率)
            if (preferredCandidates.Count > 0)
            {
                if (rand.NextDouble() < 0.8)
                {
                    return preferredCandidates[rand.Next(preferredCandidates.Count)];
                }
            }

            // 否则（或20%概率）从所有可用Tier中随机选择
            return candidates[rand.Next(candidates.Count)];
        }

        /// <summary>
        /// 获取指定Tier的技法数量（用于调试）
        /// </summary>
        public static int GetTechniqueCount(TechniqueLevel tier, CuisineStyle style)
        {
            return database[tier][style].Length;
        }

        /// <summary>
        /// 获取所有技法总数（用于调试）
        /// </summary>
        public static int GetTotalTechniqueCount()
        {
            int count = 0;
            foreach (var tierData in database.Values)
            {
                foreach (var styleData in tierData.Values)
                {
                    count += styleData.Length;
                }
            }
            return count;
        }
    }
}