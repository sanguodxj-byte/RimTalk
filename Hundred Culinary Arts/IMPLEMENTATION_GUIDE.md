
# 厨间百艺 - 开发实施指南

本文档提供分阶段的详细实施步骤，帮助开发者逐步构建整个mod。

---

## 开发环境准备

### 必需工具
- **Visual Studio 2022** (或 VS Code + C# 扩展)
- **.NET Framework 4.7.2** (RimWorld 1.4使用)
- **RimWorld 游戏本体** (用于测试)
- **Git** (版本控制)

### 目录设置

```powershell
# RimWorld安装路径 (示例)
$RimWorldPath = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld"

# Mod开发路径 (当前工作区)
$ModPath = "C:\Users\Administrator\Desktop\rim mod\Hundred Culinary Arts"

# Mods文件夹 (用于测试)
$TestModsPath = "$RimWorldPath\Mods"
```

---

## Phase 1: 项目骨架搭建

### Step 1.1: 创建目录结构

```
Hundred Culinary Arts/
├── About/
├── Assemblies/
├── Defs/
│   └── ThoughtDefs/
├── Languages/
│   ├── English/
│   │   └── Keyed/
│   └── ChineseSimplified/
│       └── Keyed/
└── Source/
    └── CulinaryArts/
        ├── Components/
        ├── Systems/
        ├── Harmony/
        ├── Data/
        └── Utilities/
```

### Step 1.2: 创建 About.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<ModMetaData>
  <name>厨间百艺 (Culinary Arts 100)</name>
  <author>Your Name</author>
  <packageId>yourname.culinaryarts</packageId>
  <supportedVersions>
    <li>1.4</li>
    <li>1.5</li>
  </supportedVersions>
  <description>程序化生成的动态菜名系统。让每一餐都独一无二！

核心特性:
- 100+种菜名组合
- 厨师技能决定菜品质量
- 智能堆叠显示
- 心情加成系统

A procedural food naming system that makes every meal unique!

Core Features:
- 100+ dish name combinations
- Chef skill determines quality
- Smart stack display
- Mood bonus system</description>
  <modDependencies>
    <li>
      <packageId>brrainz.harmony</packageId>
      <displayName>Harmony</displayName>
      <steamWorkshopUrl>steam://url/CommunityFilePage/2009463077</steamWorkshopUrl>
      <downloadUrl>https://github.com/pardeike/HarmonyRimWorld/releases/latest</downloadUrl>
    </li>
  </modDependencies>
  <loadAfter>
    <li>Ludeon.RimWorld</li>
    <li>Ludeon.RimWorld.Royalty</li>
    <li>Ludeon.RimWorld.Ideology</li>
    <li>Ludeon.RimWorld.Biotech</li>
    <li>brrainz.harmony</li>
  </loadAfter>
</ModMetaData>
```

### Step 1.3: 创建 C# 项目文件

**CulinaryArts.csproj**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
    <LangVersion>latest</LangVersion>
    <OutputPath>..\..\Assemblies</OutputPath>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
    <DebugType>portable</DebugType>
  </PropertyGroup>

  <ItemGroup>
    <!-- RimWorld核心引用 - 请根据实际路径修改 -->
    <Reference Include="Assembly-CSharp">
      <HintPath>C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="UnityEngine.CoreModule">
      <HintPath>C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.CoreModule.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="0Harmony">
      <HintPath>C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\0Harmony.dll</HintPath>
      <Private>False</Private>
    </Reference>
  </ItemGroup>
</Project>
```

---

## Phase 2: 核心组件实现

### Step 2.1: 定义基础枚举和常量

**Utilities/Enums.cs**:
```csharp
namespace CulinaryArts
{
    public enum CuisineStyle
    {
        Chinese = 0,
        Western = 1
    }

    public enum TechniqueLevel
    {
        Survival = 0,      // 0-5
        HomeCooking = 1,   // 6-12
        Gourmet = 2,       // 13-17
        Legendary = 3      // 18-20
    }

    public enum PrefixQuality
    {
        Terrible = -3,
        None = 0,
        Good = 3,
        Legendary = 8
    }

    public static class Constants
    {
        public const int TICKS_PER_WINDOW = 15000;  // 6小时
        public const int SEED_MULTIPLIER = 397;     // 质数，用于种子计算
    }
}
```

### Step 2.2: 实现 CompNamedMeal 组件

**Components/CompProperties_NamedMeal.cs**:
```csharp
using Verse;

namespace CulinaryArts
{
    public class CompProperties_NamedMeal : CompProperties
    {
        public CompProperties_NamedMeal()
        {
            compClass = typeof(CompNamedMeal);
        }
    }
}
```

**Components/CompNamedMeal.cs**:
```csharp
using Verse;

namespace CulinaryArts
{
    public class CompNamedMeal : ThingComp
    {
        private string customName;
        private int moodOffset;
        private int generationSeed;
        private string cuisineStyle;

        public string CustomName => customName;
        public int MoodOffset => moodOffset;

        public void SetData(string name, int mood, int seed, CuisineStyle style)
        {
            customName = name;
            moodOffset = mood;
            generationSeed = seed;
            cuisineStyle = style.ToString();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref customName, "customName");
            Scribe_Values.Look(ref moodOffset, "moodOffset", 0);
            Scribe_Values.Look(ref generationSeed, "generationSeed", 0);
            Scribe_Values.Look(ref cuisineStyle, "cuisineStyle", "Chinese");
        }
    }
}
```

### Step 2.3: 实现时间种子生成器

**Systems/TimeSeedGenerator.cs**:
```csharp
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CulinaryArts
{
    public static class TimeSeedGenerator
    {
        public static int GenerateSeed(Pawn chef, List<Thing> ingredients)
        {
            // 厨师ID部分
            int pawnHash = chef.thingIDNumber * Constants.SEED_MULTIPLIER;

            // 食材哈希 (排序后确保顺序无关)
            int ingredientHash = GetIngredientHash(ingredients);

            // 时间窗口 (6小时一个周期)
            int timeWindow = Find.TickManager.TicksGame / Constants.TICKS_PER_WINDOW;

            // 异或组合
            return pawnHash ^ ingredientHash ^ timeWindow;
        }

        private static int GetIngredientHash(List<Thing> ingredients)
        {
            int hash = 0;
            
            // 按defName排序确保一致性
            var sortedDefs = ingredients
                .Select(t => t.def.defName)
                .OrderBy(name => name)
                .ToList();

            foreach (var defName in sortedDefs)
            {
                hash ^= defName.GetHashCode();
            }

            return hash;
        }
    }
}
```

---

## Phase 3: 数据库实现

### Step 3.1: 食材形态数据库

**Data/IngredientDatabase.cs**:
```csharp
using System;
using System.Collections.Generic;

namespace CulinaryArts
{
    public class IngredientForms
    {
        public string[] Chinese { get; set; }
        public string[] Western { get; set; }
    }

    public static class IngredientDatabase
    {
        private static Dictionary<string, IngredientForms> database;

        static IngredientDatabase()
        {
            InitializeDatabase();
        }

        private static void InitializeDatabase()
        {
            database = new Dictionary<string, IngredientForms>
            {
                // 谷物类
                ["Rice"] = new IngredientForms
                {
                    Chinese = new[] { "饭", "粥", "糕", "粉", "肠粉" },
                    Western = new[] { "Risotto", "Paella", "Pudding", "Rice Bowl" }
                },
                ["Corn"] = new IngredientForms
                {
                    Chinese = new[] { "窝头", "糊", "棒子", "饼子" },
                    Western = new[] { "Polenta", "Tortilla", "Chowder", "Cornbread" }
                },

                // 肉类 (通用)
                ["Meat"] = new IngredientForms
                {
                    Chinese = new[] { "肉片", "肉丝", "肉排", "肉糜" },
                    Western = new[] { "Steak", "Fillet", "Bits", "Roast" }
                },

                // 蔬菜类
                ["RawPotatoes"] = new IngredientForms
                {
                    Chinese = new[] { "土豆丝", "土豆块", "薯泥", "土豆片" },
                    Western = new[] { "Mashed Potato", "Fries", "Wedges", "Purée" }
                },

                // 可以继续添加更多...
            };
        }

        public static string GetForm(string defName, CuisineStyle style, Random rand)
        {
            // 尝试直接匹配
            if (database.TryGetValue(defName, out var forms))
            {
                var pool = style == CuisineStyle.Chinese ? forms.Chinese : forms.Western;
                return pool[rand.Next(pool.Length)];
            }

            // 尝试模糊匹配 (例如 Meat_Muffalo -> Meat)
            foreach (var key in database.Keys)
            {
                if (defName.Contains(key))
                {
                    var forms2 = database[key];
                    var pool = style == CuisineStyle.Chinese ? forms2.Chinese : forms2.Western;
                    return pool[rand.Next(pool.Length)];
                }
            }

            // 回退: 返回原始名称
            return defName;
        }
    }
}
```

### Step 3.2: 技法数据库

**Data/TechniqueDatabase.cs**:
```csharp
using System;
using System.Collections.Generic;

namespace CulinaryArts
{
    public static class TechniqueDatabase
    {
        private static Dictionary<TechniqueLevel, Dictionary<CuisineStyle, string[]>> database;

        static TechniqueDatabase()
        {
            InitializeDatabase();
        }

        private static void InitializeDatabase()
        {
            database = new Dictionary<TechniqueLevel, Dictionary<CuisineStyle, string[]>>
            {
                [TechniqueLevel.Survival] = new Dictionary<CuisineStyle, string[]>
                {
                    [CuisineStyle.Chinese] = new[] { "煮", "烤", "乱炖", "拌", "糊", "蒸", "煎", "炒" },
                    [CuisineStyle.Western] = new[] { "Charred", "Boiled", "Basic", "Mashed", "Grilled", "Fried" }
                },

                [TechniqueLevel.HomeCooking] = new Dictionary<CuisineStyle, string[]>
                {
                    [CuisineStyle.Chinese] = new[] { "爆炒", "红烧", "清蒸", "干煎", "回锅", "糖醋", "宫保", "麻辣", "葱爆", "油焖", "酱烧", "干锅", "水煮", "白切", "卤" },
                    [CuisineStyle.Western] = new[] { "Sautéed", "Baked", "Glazed", "Creamy", "Crispy", "Smoked", "Roasted", "Grilled", "Braised", "Stewed", "Marinated", "Breaded", "Pan-seared", "Herb-crusted", "Garlic" }
                },

                [TechniqueLevel.Gourmet] = new Dictionary<CuisineStyle, string[]>
                {
                    [CuisineStyle.Chinese] = new[] { "慢煨", "白灼", "糟卤", "挂汁", "松鼠形", "砂锅", "拔丝", "琉璃", "脆皮", "荷叶", "锅巴", "龙井", "冰糖", "蜜汁", "脆炸" },
                    [CuisineStyle.Western] = new[] { "Poached", "Seared", "Reduction", "Caramelized", "Infused", "Wellington-style", "En Croûte", "Au Gratin", "Provençal", "Florentine" }
                },

                [TechniqueLevel.Legendary] = new Dictionary<CuisineStyle, string[]>
                {
                    [CuisineStyle.Chinese] = new[] { "乾坤", "锦绣", "龙凤", "金汤", "宫廷", "佛跳墙", "满汉", "仙府", "御膳", "九转" },
                    [CuisineStyle.Western] = new[] { "Sous-vide", "Confit", "Deconstructed", "Truffled", "Aged", "Molecular", "Spherified", "Foamed", "Torched" }
                }
            };
        }

        public static string GetTechnique(int skillLevel, CuisineStyle style, Random rand)
        {
            TechniqueLevel tier = GetTierForSkill(skillLevel);
            
            // 高技能有机会使用更高Tier（加权随机）
            tier = WeightedTierSelection(skillLevel, tier, rand);

            return database[tier][style][rand.Next(database[tier][style].Length)];
        }

        private static TechniqueLevel GetTierForSkill(int skill)
        {
            if (skill <= 5) return TechniqueLevel.Survival;
            if (skill <= 12) return TechniqueLevel.HomeCooking;
            if (skill <= 17) return TechniqueLevel.Gourmet;
            return TechniqueLevel.Legendary;
        }

        private static TechniqueLevel WeightedTierSelection(int skill, TechniqueLevel baseTier, Random rand)
        {
            // 简化版: 80%当前Tier, 20%更高Tier
            if (skill > 12 && baseTier < TechniqueLevel.Legendary && rand.NextDouble() < 0.2)
            {
                return baseTier + 1;
            }
            return baseTier;
        }
    }
}
```

### Step 3.3: 前缀数据库

**Data/PrefixDatabase.cs**:
```csharp
using System;
using System.Collections.Generic;

namespace CulinaryArts
{
    public static class PrefixDatabase
    {
        private static Dictionary<PrefixQuality, Dictionary<CuisineStyle, string[]>> database;

        static PrefixDatabase()
        {
            InitializeDatabase();
        }

        private static void InitializeDatabase()
        {
            database = new Dictionary<PrefixQuality, Dictionary<CuisineStyle, string[]>>
            {
                [PrefixQuality.Terrible] = new Dictionary<CuisineStyle, string[]>
                {
                    [CuisineStyle.Chinese] = new[] { "烧焦的", "过咸的", "没熟的", "油腻的" },
                    [CuisineStyle.Western] = new[] { "Burnt", "Oversalted", "Undercooked", "Greasy" }
                },

                [PrefixQuality.Good] = new Dictionary<CuisineStyle, string[]>
                {
                    [CuisineStyle.Chinese] = new[] { "美味的", "主厨的", "精心的", "香气四溢的" },
                    [CuisineStyle.Western] = new[] { "Delicious", "Chef's Special", "Exquisite", "Aromatic" }
                },

                [PrefixQuality.Legendary] = new Dictionary<CuisineStyle, string[]>
                {
                    [CuisineStyle.Chinese] = new[] { "绝世的", "发光的", "仙品", "神级", "传说中的" },
                    [CuisineStyle.Western] = new[] { "Legendary", "Glowing", "Divine", "Godlike", "Mythical" }
                }
            };
        }

        public static (string prefix, int mood) GeneratePrefix(int skillLevel, CuisineStyle style, Random rand)
        {
            float roll = (float)rand.NextDouble();
            PrefixQuality quality;

            // 根据技能等级决定概率
            if (skillLevel <= 5)
            {
                if (roll < 0.30f) quality = PrefixQuality.Terrible;
                else if (roll < 0.95f) return ("", 0);
                else quality = PrefixQuality.Good;
            }
            else if (skillLevel <= 12)
            {
                if (roll < 0.10f) quality = PrefixQuality.Terrible;
                else if (roll < 0.90f) return ("", 0);
                else quality = PrefixQuality.Good;
            }
            else if (skillLevel <= 17)
            {
                if (roll < 0.75f) return ("", 0);
                else if (roll < 0.95f) quality = PrefixQuality.Good;
                else quality = PrefixQuality.Legendary;
            }
            else // 18-20
            {
                if (roll < 0.50f) return ("", 0);
                else if (roll < 0.80f) quality = PrefixQuality.Good;
                else quality = PrefixQuality.Legendary;
            }

            string prefix = database[quality][style][rand.Next(database[quality][style].Length)];
            return (prefix, (int)quality);
        }
    }
}
```

---

## Phase 4: 核心命名系统

**Systems/NameGenerator.cs**:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace CulinaryArts
{
    public static class NameGenerator
    {
        public static void GenerateMealName(Thing meal, Pawn chef, List<Thing> ingredients)
        {
            // 1. 生成种子
            int seed = TimeSeedGenerator.GenerateSeed(chef, ingredients);
            Random rand = new Random(seed);

            // 2. 确定风格
            CuisineStyle style = (CuisineStyle)rand.Next(2);

            // 3. 获取技能等级
            int skillLevel = chef.skills.GetSkill(SkillDefOf.Cooking).Level;

            // 4. 生成前缀
            var (prefix, mood) = PrefixDatabase.GeneratePrefix(skillLevel, style, rand);

            // 5. 选择技法
            string technique = TechniqueDatabase.GetTechnique(skillLevel, style, rand);

            // 6. 转换食材形态
            List<string> forms = ingredients
                .Select(ing => IngredientDatabase.GetForm(ing.def.defName, style, rand))
                .Distinct()
                .Take(2)  // 最多2种食材形态
                .ToList();

            // 7. 组装名称
            string name = AssembleName(prefix, technique, forms, style);

            // 8. 写入组件
            var comp = meal.TryGetComp<CompNamedMeal>();
            if (comp != null)
            {
                comp.SetData(name, mood, seed, style);
            }
        }

        private static string AssembleName(string prefix, string technique, List<string> forms, CuisineStyle style)
        {
            if (forms.Count == 0)
                return style == CuisineStyle.Chinese ? "未知料理" : "Unknown Dish";

            string result;

            if (style == CuisineStyle.Chinese)
            {
                // 中式: [前缀][技法][主料][配辅料]
                if (forms.Count == 1)
                {
                    result = $"{technique}{forms[0]}";
                }
                else
                {
                    result = $"{technique}{forms[0]}配{forms[1]}";
                }

                // 添加前缀
                if (!string.IsNullOrEmpty(prefix))
                {
                    result = prefix + result;
                }
            }
            else
            {
                // 西式: [Prefix] [Technique] [Main] with [Side]
                if (forms.Count == 1)
                {
                    result = $"{technique} {forms[0]}";
                }
                else
                {
                    result = $"{technique} {forms[0]} with {forms[1]}";
                }

                // 添加前缀
                if (!string.IsNullOrEmpty(prefix))
                {
                    result = prefix + " " + result;
                }
            }

            return result;
        }
    }
}
```

---

## Phase 5: Harmony补丁实现

### Step 5.1: Mod入口和Harmony初始化

**CulinaryArtsMod.cs**:
```csharp
using Verse;
using HarmonyLib;

namespace CulinaryArts
{
    [StaticConstructorOnStartup]
    public static class CulinaryArtsMod
    {
        static CulinaryArtsMod()
        {
            var harmony = new Harmony("yourname.culinaryarts");
            harmony.PatchAll();
            
            Log.Message("[Culinary Arts] Mod initialized successfully!");
        }
    }
}
```

### Step 5.2: 食物生成补丁

**Harmony/Patch_GenRecipe.cs**:
```csharp
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CulinaryArts.Harmony
{
    [HarmonyPatch(typeof(GenRecipe), "MakeRecipeProducts")]
    public static class Patch_GenRecipe_MakeRecipeProducts
    {
        [HarmonyPostfix]
        public static void Postfix(
            RecipeDef recipeDef,
            Pawn worker,
            List<Thing> ingredients,
            ref IEnumerable<Thing> __result)
        {
            // 只处理食物配方
            if (recipeDef?.ProducedThingDef == null || !recipeDef.ProducedThingDef.IsIngestible)
                return;

            // 只处理有烹饪技能要求的
            if (worker == null || ingredients == null || ingredients.Count == 0)
                return;

            foreach (var product in __result)
            {
                // 检查是否有CompNamedMeal组件
                if (product.TryGetComp<CompNamedMeal>() != null)
                {
                    NameGenerator.GenerateMealName(product, worker, ingredients);
                }
            }
        }
    }
}
```

### Step 5.3: 标签显示补丁

**Harmony/Patch_Thing_Label.cs**:
```csharp
using HarmonyLib;
using Verse;

namespace CulinaryArts.Harmony
{
    [HarmonyPatch(typeof(Thing), "Label", MethodType.Getter)]
    public static class Patch_Thing_Label
    {
        [HarmonyPostfix]
        public static void Postfix(Thing __instance, ref string __result)
        {
            var comp = __instance.TryGetComp<CompNamedMeal>();
            if (comp == null || string.IsNullOrEmpty(comp.CustomName))
                return;

            // 堆叠时显示原名，单品时显示自定义名
            bool shouldShowCustomName = __instance.stackCount == 1 ||
                                       Find.Selector.SelectedObjects.Contains(__instance) ||
                                       __instance.ParentHolder is Pawn_InventoryTracker ||
                                       __instance.ParentHolder is Pawn_CarryTracker;

            if (shouldShowCustomName)
            {
                __result = comp.CustomName;
            }
        }
    }
}
```

### Step 5.4: 心情效果补丁

**Harmony/Patch_FoodUtility.cs**:
```csharp
using HarmonyLib;
using RimWorld;
using Verse;

namespace CulinaryArts.Harmony
{
    [HarmonyPatch(typeof(FoodUtility), "AddIngestThoughtsFromIngredient")]
    public static class Patch_FoodUtility_AddIngestThoughts
    {
        [HarmonyPostfix]
        public static void Postfix(ThingDef def, Pawn ingester, Thing ingredient)
        {
            if (ingredient == null || ingester == null)
                return;

            var comp = ingredient.TryGetComp<CompNamedMeal>();
            if (comp == null || comp.MoodOffset == 0)
                return;

            // 根据心情值添加对应的Thought
            ThoughtDef thoughtDef = comp.MoodOffset switch
            {
                -3 => ThoughtDefOf.CulinaryArts_Terrible,
                3 => ThoughtDefOf.CulinaryArts_Delicious,
                8 => ThoughtDefOf.CulinaryArts_Legendary,
                _ => null
            };

            if (thoughtDef != null)
            {
                ingester.needs.mood.thoughts.memories.TryGainMemory(thoughtDef);
            }
        }
    }
}
```

**Utilities/ThoughtDefOf.cs**:
```csharp
using RimWorld;

namespace CulinaryArts
{
    [DefOf]
    public static class ThoughtDefOf
    {
        public static ThoughtDef CulinaryArts_Terrible;
        public static ThoughtDef CulinaryArts_Delicious;
        public static ThoughtDef CulinaryArts_Legendary;

        static ThoughtDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThoughtDefOf));
        }
    }
}
```

---

## Phase 6: XML定义文件

### ThoughtDefs

**Defs/ThoughtDefs/Thoughts_Memory_CulinaryArts.xml**:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Defs>

  <ThoughtDef>
    <defName>CulinaryArts_Terrible</defName>
    <thoughtClass>Thought_Memory</thoughtClass>
    <durationDays>0.5</durationDays>
    <stackLimit>3</stackLimit>
    <stackLimitForSameOtherPawn>3</stackLimitForSameOtherPawn>
    <stages>
      <li>
        <label>难以下咽的食物</label>
        <description>这做的什么玩意儿...</description>
        <baseMoodEffect>-3</baseMoodEffect>
      </li>
    </stages>
  </ThoughtDef>

  <ThoughtDef>
    <defName>CulinaryArts_Delicious</defName>
    <thoughtClass>Thought_Memory</thoughtClass>
    <durationDays>1</durationDays>
    <stackLimit>5</stackLimit>
    <stackLimitForSameOtherPawn>5</stackLimitForSameOtherPawn>
    <stages>
      <li>
        <label>美味的料理</label>
        <description>这菜做得真不错！</description>
        <baseMoodEffect>3</baseMoodEffect>
      </li>
    </stages>
  </ThoughtDef>

  <ThoughtDef>
    <defName>CulinaryArts_Legendary</defName>
    <thoughtClass>Thought_Memory</thoughtClass>
    <durationDays>2</durationDays>
    <stackLimit>1</stackLimit>
    <stackLimitForSameOtherPawn>1</stackLimitForSameOtherPawn>
    <stages>
      