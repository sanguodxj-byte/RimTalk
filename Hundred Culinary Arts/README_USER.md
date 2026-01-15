# 厨间百艺 (Culinary Arts 100)

> 让每一餐都独一无二的程序化菜名系统

[English](#english-version) | [中文](#中文版本)

---

## 中文版本

### 📖 简介

**厨间百艺**是一个创新的RimWorld模组，通过程序化算法为游戏中的每道菜肴生成独特的名称。告别千篇一律的"精致食物"，享受沉浸式的美食体验！

### ✨ 核心特性

#### 1. 无限菜名组合
- **100+种技法词汇**：从"乱炖"到"佛跳墙"
- **30+种食材映射**：智能转换为烹饪形态
- **中西双风格**：同时支持中式和西式菜系
- **数千种组合**：理论上可生成数千种独特菜名

#### 2. 技能驱动系统
| 技能等级 | 层级 | 典型菜名示例 |
|---------|------|------------|
| 0-5级 | 生存本能 | 煮肉块、烤土豆 |
| 6-12级 | 烟火家常 | 红烧肉排配土豆丝 |
| 13-17级 | 珍馐美馔 | 主厨的慢煨狮子头 |
| 18-20级 | 登峰造极 | 绝世的佛跳墙海鲜饭 |

#### 3. 时间一致性
- 同一厨师用相同食材，在**6小时内**产出相同菜名
- 模拟真实厨师的烹饪习惯

#### 4. 智能显示
- **仓库堆叠时**：显示原始名称（例："精致食物 x20"）
- **单品/食用时**：显示自定义名称（例："绝世的红烧狮子头"）
- 保持仓库整洁，食用时沉浸感满满

#### 5. 心情加成系统
| 前缀品质 | 心情效果 | 出现概率（高技能） |
|---------|---------|------------------|
| 负面（烧焦的、过咸的） | -3 | 0% (技能13+) |
| 无前缀 | 0 | 50-80% |
| 正面（美味的、主厨的） | +3 | 10-30% |
| 传说（绝世的、发光的） | +8 | 0-20% (技能18+) |

---

### 🚀 安装方法

#### 方法一：Steam创意工坊（推荐）
1. 在Steam创意工坊搜索"Culinary Arts 100"或"厨间百艺"
2. 点击订阅
3. 启动游戏，在Mod管理器中启用本mod

#### 方法二：手动安装
1. 下载最新版本的mod文件
2. 解压到RimWorld安装目录的`Mods`文件夹
   - 例：`C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\Culinary Arts 100`
3. 启动游戏，在Mod管理器中启用本mod

#### 依赖项
- **Harmony** (必需，游戏会自动提示安装)

---

### 🎮 使用指南

#### 基础使用
1. 像往常一样让小人制作食物
2. 查看制作完成的食物，会自动获得独特的名称
3. 小人食用时会根据菜品质量获得心情加成

#### 高级技巧
- **培养高技能厨师**：技能越高，产出好菜的概率越大
- **固定菜谱**：6小时内重复制作可获得相同菜名
- **查看详情**：选中单个食物可在检查栏看到菜名、风格、心情等信息

#### 调试模式
按`F12`打开开发者模式，选中食物可查看：
- 生成种子值
- 菜系风格
- 心情加成值
- 厨师技能等级

---

### 🔧 配置选项

目前版本暂无可配置选项，所有功能自动运行。

未来版本计划添加：
- [ ] 菜名生成频率开关
- [ ] 心情效果倍率调整
- [ ] Mod兼容性开关

---

### 🤝 兼容性

#### 已知兼容
- ✅ 原版游戏所有食物
- ✅ Royalty DLC
- ✅ Ideology DLC
- ✅ Biotech DLC

#### 应该兼容（未测试）
- ⚠️ Vanilla Cooking Expanded
- ⚠️ Gastronomy
- ⚠️ 其他添加新食材的mod

#### 已知不兼容
- ❌ 目前无

**问题反馈**：如果发现不兼容问题，请在创意工坊页面留言。

---

### 📝 更新日志

#### v1.0.0 (2025-12-26)
- ✨ 首次发布
- ✨ 实现核心菜名生成系统
- ✨ 支持中西双风格
- ✨ 116种技法词汇
- ✨ 30+食材映射
- ✨ 时间种子一致性系统
- ✨ 智能堆叠显示
- ✨ 心情效果系统

---

### ❓ 常见问题

**Q: 为什么仓库里的食物还是显示"精致食物"？**
A: 这是正常的！仓库堆叠时显示原名以保持整洁。选中单个食物或小人拿着/食用时会显示自定义名称。

**Q: 同样的厨师和食材为什么做出了不同的菜？**
A: 可能是超过了6小时的时间窗口。系统每6小时会更新一次，模拟厨师换菜谱的行为。

**Q: 能否添加更多食材映射？**
A: 当然可以！欢迎在创意工坊留言建议，或者查看mod源代码自行修改。

**Q: 存档兼容性如何？**
A: 可以随时添加或移除本mod，不会破坏存档。移除后，自定义菜名会消失，食物恢复原始名称。

**Q: 性能影响如何？**
A: 极小。菜名生成仅在食物制作时触发一次，不会持续消耗资源。

---

### 🙏 致谢

- **RimWorld Mod社区**：提供了丰富的学习资源
- **Harmony框架**：让mod开发变得可能
- **所有测试玩家**：感谢你们的反馈

---

### 📧 联系方式

- **GitHub**: [项目地址]
- **Steam创意工坊**: [工坊页面]
- **Discord**: [Discord服务器]

---

## English Version

### 📖 Introduction

**Culinary Arts 100** is an innovative RimWorld mod that generates unique names for every dish using procedural algorithms. Say goodbye to monotonous "fine meals" and enjoy an immersive culinary experience!

### ✨ Core Features

#### 1. Infinite Name Combinations
- **100+ technique terms**: From "Charred" to "Sous-vide"
- **30+ ingredient mappings**: Smart transformation to cooking forms
- **Dual cuisine styles**: Chinese and Western
- **Thousands of combinations**: Theoretically generates thousands of unique dish names

#### 2. Skill-Driven System
| Skill Level | Tier | Example Dishes |
|------------|------|----------------|
| 0-5 | Survival | Boiled Meat, Roasted Potato |
| 6-12 | Home Cooking | Sautéed Pork with Potatoes |
| 13-17 | Gourmet | Chef's Special Braised Beef |
| 18-20 | Legendary | Legendary Sous-vide Steak with Truffle Risotto |

#### 3. Time Consistency
- Same chef + same ingredients = same dish name within **6 hours**
- Simulates real chef cooking habits

#### 4. Smart Display
- **In storage (stacked)**: Shows base name (e.g., "Fine meal x20")
- **Single item/eating**: Shows custom name (e.g., "Legendary Braised Beef")
- Keeps storage clean while providing immersion when eating

#### 5. Mood Bonus System
| Prefix Quality | Mood Effect | Chance (High Skill) |
|---------------|-------------|---------------------|
| Terrible (Burnt, Oversalted) | -3 | 0% (skill 13+) |
| None | 0 | 50-80% |
| Good (Delicious, Chef's Special) | +3 | 10-30% |
| Legendary (Glowing, Divine) | +8 | 0-20% (skill 18+) |

---

### 🚀 Installation

#### Method 1: Steam Workshop (Recommended)
1. Search for "Culinary Arts 100" in Steam Workshop
2. Click Subscribe
3. Enable the mod in the game's Mod Manager

#### Method 2: Manual Installation
1. Download the latest mod files
2. Extract to RimWorld's `Mods` folder
   - Example: `C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\Culinary Arts 100`
3. Enable in game's Mod Manager

#### Dependencies
- **Harmony** (Required, game will prompt installation)

---

### 🎮 Usage Guide

#### Basic Usage
1. Have pawns cook meals as usual
2. Completed meals will automatically receive unique names
3. Pawns gain mood bonuses based on dish quality when eating

#### Advanced Tips
- **Train high-skill chefs**: Higher skill = better chance of good dishes
- **Fixed recipes**: Repeat cooking within 6 hours for same dish name
- **View details**: Select individual food items to see name, style, mood in inspect pane

#### Debug Mode
Press `F12` for dev mode, select food to see:
- Generation seed
- Cuisine style
- Mood bonus
- Chef skill level

---

### 🔧 Configuration

Currently no config options, all features run automatically.

Planned for future versions:
- [ ] Name generation toggle
- [ ] Mood effect multiplier
- [ ] Mod compatibility switches

---

### 🤝 Compatibility

#### Known Compatible
- ✅ All vanilla foods
- ✅ Royalty DLC
- ✅ Ideology DLC
- ✅ Biotech DLC

#### Should Be Compatible (Untested)
- ⚠️ Vanilla Cooking Expanded
- ⚠️ Gastronomy
- ⚠️ Other mods adding new ingredients

#### Known Incompatible
- ❌ None currently

**Report Issues**: Please comment on the Workshop page if you find incompatibilities.

---

### 📝 Changelog

#### v1.0.0 (2025-12-26)
- ✨ Initial release
- ✨ Core name generation system
- ✨ Chinese & Western dual styles
- ✨ 116 technique terms
- ✨ 30+ ingredient mappings
- ✨ Time-seed consistency system
- ✨ Smart stack display
- ✨ Mood bonus system

---

### ❓ FAQ

**Q: Why do meals in storage still show "Fine meal"?**
A: This is normal! Stacked items show base names for cleanliness. Select individual items or watch pawns carry/eat them to see custom names.

**Q: Why did same chef and ingredients make different dishes?**
A: The 6-hour time window may have expired. The system updates every 6 hours, simulating chefs changing recipes.

**Q: Can you add more ingredient mappings?**
A: Absolutely! Please suggest on the Workshop page, or modify the source code yourself.

**Q: Is it save-compatible?**
A: Yes! You can add or remove this mod anytime without breaking saves. Custom names will disappear if removed, food reverts to base names.

**Q: Performance impact?**
A: Minimal. Name generation only triggers once when food is cooked, no continuous resource consumption.

---

### 🙏 Credits

- **RimWorld Mod Community**: Provided rich learning resources
- **Harmony Framework**: Made modding possible
- **All testers**: Thank you for your feedback

---

### 📧 Contact

- **GitHub**: [Project Link]
- **Steam Workshop**: [Workshop Page]
- **Discord**: [Discord Server]

---

**享受你的美食之旅！ / Enjoy your culinary journey!**