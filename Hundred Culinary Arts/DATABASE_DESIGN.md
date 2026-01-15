# 厨间百艺 - 数据词库设计

本文档详细定义了所有的食材映射、技法词库和前缀词库。

---

## 一、食材形态映射表

### 1.1 谷物类 (Grains)

| 原始DefName | 中式形态 | 西式形态 |
|------------|---------|---------|
| Rice | 饭、粥、糕、粉、肠粉、糍粑 | Risotto、Paella、Pudding、Rice Bowl |
| Wheat | 面、馒头、饼、饺子、包子、花卷 | Pasta、Bread、Toast、Pizza、Pie |
| Corn | 窝头、糊、棒子、饼子 | Polenta、Tortilla、Chowder、Cornbread |

### 1.2 肉类 (Meat)

| 原始DefName | 中式形态 | 西式形态 |
|------------|---------|---------|
| Meat_Muffalo | 肉片、肉丝、肉排、肉糜、大块肉 | Steak、Fillet、Bits、Roast、Chops |
| Meat_Human | 人肉片、人肉丝、人肉排 | Human Steak、Human Fillet、Human Bits |
| Meat_Chicken | 鸡肉、鸡翅、鸡腿、鸡胸 | Chicken Breast、Drumstick、Wings、Tender |
| Meat_Pig | 猪肉、五花肉、里脊、排骨 | Pork Belly、Tenderloin、Ribs、Chops |

### 1.3 蔬菜类 (Vegetables)

| 原始DefName | 中式形态 | 西式形态 |
|------------|---------|---------|
| RawPotatoes | 土豆丝、土豆块、薯泥、土豆片 | Mashed、Fries、Wedges、Purée |
| Corn | 玉米粒、玉米段 | Kernels、Cob |
| RawAgave | 龙舌兰丁、龙舌兰块 | Agave Chunks、Agave Pieces |

### 1.4 特殊食材 (Special)

| 原始DefName | 中式形态 | 西式形态 |
|------------|---------|---------|
| InsectJelly | 虫胶冻、虫胶糕 | Insect Jelly、Gel Cube |
| Milk | 奶、奶汤、奶油 | Milk、Cream、Dairy |
| Eggs | 蛋、蛋羹、蛋饼 | Egg、Omelette、Scramble |

---

## 二、技法词库系统

### 2.1 Tier 1: 生存本能 (Level 0-5)

#### 中式技法
```csharp
private static readonly string[] Tier1_Chinese = new[]
{
    "煮",           // Boiled
    "烤",           // Roasted
    "乱炖",         // Stewed
    "拌",           // Mixed
    "糊",           // Mush
    "蒸",           // Steamed (basic)
    "煎",           // Fried (basic)
    "炒"            // Stir-fried (basic)
};
```

#### 西式技法
```csharp
private static readonly string[] Tier1_Western = new[]
{
    "Charred",      // 烤焦的
    "Boiled",       // 水煮
    "Basic",        // 基础的
    "Mashed",       // 捣碎的
    "Grilled",      // 烤的
    "Fried",        // 炸的
    "Raw",          // 生的（勉强处理过）
    "Burnt"         // 烧焦的
};
```

### 2.2 Tier 2: 烟火家常 (Level 6-12)

#### 中式技法
```csharp
private static readonly string[] Tier2_Chinese = new[]
{
    "爆炒",         // Stir-fried
    "红烧",         // Braised in Soy Sauce
    "清蒸",         // Steamed
    "干煎",         // Pan-fried
    "回锅",         // Twice-cooked
    "糖醋",         // Sweet & Sour
    "宫保",         // Kung Pao Style
    "麻辣",         // Spicy & Numbing
    "葱爆",         // Scallion Blasted
    "油焖",         // Oil Braised
    "酱烧",         // Sauce Braised
    "干锅",         // Dry Pot
    "水煮",         // Poached in Chili Oil
    "白切",         // Plain Cut
    "卤"            // Braised in Spices
};
```

#### 西式技法
```csharp
private static readonly string[] Tier2_Western = new[]
{
    "Sautéed",      // 嫩煎
    "Baked",        // 烘焙
    "Glazed",       // 蜜汁
    "Creamy",       // 奶油
    "Crispy",       // 酥脆
    "Smoked",       // 烟熏
    "Roasted",      // 烘烤
    "Grilled",      // 炙烤
    "Braised",      // 炖煮
    "Stewed",       // 慢炖
    "Marinated",    // 腌制
    "Breaded",      // 裹粉
    "Pan-seared",   // 煎封
    "Herb-crusted", // 香草外皮
    "Garlic"        // 蒜香
};
```

### 2.3 Tier 3: 珍馐美馔 (Level 13-17)

#### 中式技法
```csharp
private static readonly string[] Tier3_Chinese = new[]
{
    "慢煨",         // Slow Simmered
    "白灼",         // Scalded
    "糟卤",         // Wine-lees Marinated
    "挂汁",         // Glazed Sauce
    "松鼠形",       // Squirrel-shaped
    "砂锅",         // Casserole
    "拔丝",         // Candied
    "琉璃",         // Glazed (premium)
    "脆皮",         // Crispy Skin
    "荷叶",         // Lotus Leaf Wrapped
    "锅巴",         // Crispy Rice Base
    "龙井",         // Longjing Tea Infused
    "冰糖",         // Rock Sugar
    "蜜汁",         // Honey Glazed
    "脆炸",         // Crispy Fried
    "软炸",         // Soft Fried
    "干烧",         // Dry Braised
    "鱼香",         // Fish-fragrant Style
    "怪味",         // Strange-flavor
    "陈皮",         // Tangerine Peel
    "五香",         // Five-spice
    "酒香",         // Wine-fragrant
    "茶熏",         // Tea-smoked
    "花雕",         // Huadiao Wine
    "秘制"          // Secret Recipe
};
```

#### 西式技法
```csharp
private static readonly string[] Tier3_Western = new[]
{
    "Poached",          // 低温水煮
    "Seared",           // 炙烤
    "Reduction",        // 浓缩汁
    "Caramelized",      // 焦糖化
    "Infused",          // 浸渍
    "Wellington-style", // 惠灵顿式
    "En Croûte",        // 酥皮包裹
    "Au Gratin",        // 奶酪焗
    "Provençal",        // 普罗旺斯风
    "Florentine",       // 佛罗伦萨风
    "Bourguignon",      // 勃艮第风
    "Milanese",         // 米兰风
    "Marsala",          // 马沙拉酒
    "Béarnaise",        // 贝亚恩酱
    "Hollandaise",      // 荷兰酱
    "Velouté",          // 天鹅绒汁
    "Demi-glace",       // 半釉汁
    "Meunière",         // 磨坊主式
    "Piccata",          // 柠檬黄油
    "Saltimbocca",      // 跳入口中
    "Osso Buco",        // 炖小腿
    "Ratatouille",      // 普罗旺斯杂烩
    "Cassoulet",        // 砂锅焖豆
    "Bouillabaisse",    // 马赛鱼汤
    "Coq au Vin"        // 红酒炖鸡
};
```

### 2.4 Tier 4: 登峰造极 (Level 18-20)

#### 中式技法
```csharp
private static readonly string[] Tier4_Chinese = new[]
{
    "乾坤",         // Heaven & Earth
    "锦绣",         // Brocade
    "龙凤",         // Dragon & Phoenix
    "金汤",         // Golden Broth
    "宫廷",         // Imperial Court
    "佛跳墙",       // Buddha Jumps Over Wall
    "满汉",         // Manchu-Han Imperial
    "仙府",         // Celestial Palace
    "御膳",         // Imperial Kitchen
    "九转"          // Nine Transformations
};
```

#### 西式技法
```csharp
private static readonly string[] Tier4_Western = new[]
{
    "Sous-vide",        // 舒肥/真空低温
    "Confit",           // 油封
    "Deconstructed",    // 解构
    "Truffled",         // 松露风味
    "Aged",             // 熟成
    "Molecular",        // 分子
    "Spherified",       // 球化
    "Foamed",           // 泡沫
    "Nitro-frozen",     // 液氮速冻
    "Torched"           // 火炬炙烤
};
```

---

## 三、前缀词库系统

### 3.1 负面前缀 (Mood: -3)

#### 中式负面
```csharp
private static readonly string[] NegativePrefix_Chinese = new[]
{
    "烧焦的",
    "过咸的",
    "没熟的",
    "油腻的",
    "糊了的",
    "发苦的",
    "半生的",
    "变质的"
};
```

#### 西式负面
```csharp
private static readonly string[] NegativePrefix_Western = new[]
{
    "Burnt",
    "Oversalted",
    "Undercooked",
    "Greasy",
    "Rubbery",
    "Soggy",
    "Bland",
    "Spoiled"
};
```

### 3.2 正面前缀 (Mood: +3)

#### 中式正面
```csharp
private static readonly string[] PositivePrefix_Chinese = new[]
{
    "美味的",
    "主厨的",
    "精心的",
    "香气四溢的",
    "色香味俱全的",
    "入口即化的",
    "鲜嫩的",
    "脆爽的"
};
```

#### 西式正面
```csharp
private static readonly string[] PositivePrefix_Western = new[]
{
    "Delicious",
    "Chef's Special",
    "Exquisite",
    "Aromatic",
    "Succulent",
    "Tender",
    "Crispy",
    "Flavorful"
};
```

### 3.3 传说前缀 (Mood: +8)

#### 中式传说
```csharp
private static readonly string[] LegendaryPrefix_Chinese = new[]
{
    "绝世的",
    "发光的",
    "仙品",
    "神级",
    "传说中的",
    "天工之作",
    "举世无双的",
    "登峰造极的"
};
```

#### 西式传说
```csharp
private static readonly string[] LegendaryPrefix_Western = new[]
{
    "Legendary",
    "Glowing",
    "Divine",
    "Godlike",
    "Mythical",
    "Masterpiece",
    "Transcendent",
    "Celestial"
};
```

---

## 四、名称组装规则

### 4.1 中式命名模板

```
结构: [前缀] + [技法] + [主料形态] + [配料形态]

示例:
- 绝世的 + 红烧 + 狮子头 + 配板栗土豆
- 美味的 + 糖醋 + 里脊 + 配时蔬
- 宫廷 + 佛跳墙 + 海参 + 配鲍鱼

特殊规则:
1. 如果只有一种食材: [前缀] + [技法] + [形态]
   例: 清蒸鲈鱼
2. 如果有多种食材: [前缀] + [技法] + [主料] + 配 + [辅料]
   例: 爆炒腰花配木耳
3. Tier 4技法可以单独作为菜名主体:
   例: 满汉全席之龙凤呈祥
```

### 4.2 西式命名模板

```
结构: [Prefix] + [Technique] + [Main Form] + with + [Side Form]

示例:
- Legendary Confit Beef with Potato Purée
- Delicious Seared Chicken with Garlic Bread
- Sous-vide Steak with Truffle Risotto

特殊规则:
1. 单一食材: [Prefix] + [Technique] + [Form]
   例: Glazed Salmon
2. 多食材: [Prefix] + [Technique] + [Main] + with + [Side]
   例: Braised Pork with Apple Sauce
3. Tier 4可以省略with:
   例: Molecular Deconstructed Beef
```

---

## 五、完整示例矩阵

### 5.1 技能等级对比

| 厨师 | 技能 | 食材 | 可能产出 | 心情 |
|------|-----|------|---------|------|
| 难民小李 | 2 | 肉+土豆 | 没熟的乱炖肉块配土豆 | -3 |
| 殖民者老王 | 8 | 肉+土豆 | 红烧肉排配土豆丝 | 0 |
| 大厨老张 | 12 | 肉+土豆 | 美味的糖醋里脊配薯泥 | +3 |
| 食神戈登 | 20 | 肉+土豆 | 绝世的油封牛排配松露土豆慕斯 | +8 |

### 5.2 时间一致性示例

```
时间: 8:00 AM (Tick 120000)
厨师: 老张 (ID: 12345, 技能: 12)
食材: Meat_Muffalo + RawPotatoes

Seed = (12345 * 397) ^ (Meat_Muffalo.Hash ^ RawPotatoes.Hash) ^ (120000 / 15000)
     = 4900965 ^ 23948723 ^ 8
     = 某固定值 (例: 19482734)

使用此Seed生成:
- 风格: 中式 (rand.Next(2) = 0)
- 前缀: 美味的 (roll = 0.12, 触发正面)
- 技法: 红烧 (Tier 2, 权重选择)
- 形态: 肉排 + 土豆丝

最终: "美味的红烧肉排配土豆丝"

在接下来的6小时内 (Tick 120000 - 135000):
老张用同样的食材制作，产出名称永远相同！

到了14:00 (Tick 135001):
时间窗口改变，Seed重新计算，可能产出:
"爆炒肉丝配土豆块"
```

---

## 六、扩展性设计

### 6.1 添加新食材

```csharp
// 在 IngredientDatabase.cs 中添加
formDatabase.Add("Meat_Bear", new IngredientForms
{
    Chinese = new[] { "熊掌", "熊肉", "熊排" },
    Western = new[] { "Bear Steak", "Bear Roast", "Bear Paw" }
});
```

### 6.2 添加新技法

```csharp
// 在 TechniqueDatabase.cs 中扩展
Tier2_Chinese = Tier2_Chinese.Concat(new[] { "盐焗", "豉汁" }).ToArray();
```

### 6.3 Mod兼容性

对于其他Mod添加的食材，如果未定义映射，系统将:
1. 使用原始label作为形态
2. 正常应用技法和前缀
3. 记录日志供玩家反馈

```csharp
if (!formDatabase.ContainsKey(ingredient.defName))
{
    Log.Warning($"[CulinaryArts] Unknown ingredient: {ingredient.defName}, using default form");
    return ingredient.label; // 回退策略
}
```

---

## 七、本地化支持

### 7.1 中文词库
所有中式技法、前缀、形态已在上方定义。

### 7.2 英文词库
所有西式技法、前缀、形态已在上方定义。

### 7.3 社区翻译接口

```xml
<!-- Languages/YourLanguage/Keyed/CulinaryArts_Techniques.xml -->
<LanguageData>
  <CA_Technique_Tier1_0>您的翻译</CA_Technique_Tier1_0>
  <CA_Technique_Tier1_1>您的翻译</CA_Technique_Tier1_1>
  <!-- ... -->
</LanguageData>
```

---

## 八、平衡性数据

### 8.1 前缀触发概率总览

| 技能区间 | 负面(-3) | 无(0) | 正面(+3) | 传说(+8) |
|---------|---------|------|---------|---------|
| 0-5 | 30% | 65% | 5% | 0% |
| 6-12 | 10% | 80% | 10% | 0% |
| 13-17 | 0% | 75% | 20% | 5% |
| 18-20 | 0% | 50% | 30% | 20% |

### 8.2 心情效果持续时间

| 效果等级 | 心情值 | 持续时间 | 堆叠上限 |
|---------|-------|---------|---------|
| 负面 | -3 | 0.5天 | 3 |
| 正面 | +3 | 1天 | 5 |
| 传说 | +8 | 2天 | 1 |

### 8.3 技法分布建议

- Tier 1: 8个 (简单易实现)
- Tier 2: 15个 (常见家常)
- Tier 3: 25个 (高级复杂)
- Tier 4: 10个 (稀有艺术)

**总计**: 58个技法 × 2风格 = 116种技法

---

## 九、测试数据集

### 9.1 回归测试用例

```csharp
[TestCase(Seed: 12345, Chef: "John", Skill: 5, Ingredients: "Rice", 
          Expected: "煮饭", MoodRange: "-3 to +3")]

[TestCase(Seed: 12345, Chef: "John", Skill: 15, Ingredients: "Meat+Potato",
          Expected: "慢煨肉排配土豆慕斯", MoodRange: "0 to +8")]

[TestCase(Seed: 12345, Chef: "John", Skill: 20, Ingredients: "Meat+Rice",
          Expected: "绝世的佛跳墙海鲜饭", MoodRange: "+3 to +8")]
```

### 9.2 边界条件

- 技能0级: 应只触发Tier 1
- 技能20级: 应有20%概率触发传说前缀
- 单一食材: 不应出现"配"字
- 未知食材: 应回退到原始label

---

**数据版本**: 1.0  
**词库总量**: 116技法 + 50+食材映射 + 32前缀  
**最后更新**: 2025-12-26