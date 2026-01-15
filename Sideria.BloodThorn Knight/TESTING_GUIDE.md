# ?? 血棘骑士 Mod 快速测试指南

## ?? 测试目标

验证当前mod的核心功能是否可用，找出需要修复的问题。

---

## ?? 测试前准备

### 1. 确认文件结构

```
Sideria.BloodThorn Knight/
├── About/
│   ├── About.xml
│   ├── Preview.png
│   └── PublishedFileId.txt
├── Defs/
│   ├── ThingDefs_Races/
│   │   └── Races_Sideria.xml
│   ├── PawnKindDefs/
│   │   └── PawnKinds_Sideria.xml
│   ├── BackstoryDefs/
│   │   └── Backstories_Sideria.xml
│   ├── TraitDefs/
│   │   └── Traits_Sideria.xml
│   ├── ThoughtDefs/
│   │   └── Thoughts_Sideria.xml
│   ├── HediffDefs/
│   │   ├── Hediffs_Sideria.xml
│   │   └── Hediffs_Sideria_DualPath.xml
│   ├── AbilityDefs/
│   │   ├── Abilities_Sideria.xml
│   │   └── Abilities_Sideria_DualPath.xml
│   ├── ThingDefs_Weapons/
│   │   └── Weapons_Sideria.xml
│   ├── IncidentDefs/
│   │   └── Incidents_Sideria.xml
│   ├── TaleDefs/
│   │   └── Tales_Sideria.xml
│   └── ScenarioDefs/
│       └── Scenarios_Sideria.xml
├── Languages/
│   ├── English/
│   │   └── Keyed/
│   │       └── Keys.xml
│   └── ChineseSimplified/
│       └── Keyed/
│           └── Keys.xml
├── Patches/
│   └── FacialAnimation_Compatibility.xml
├── Assemblies/
│   └── (C# DLL files)
└── Textures/
    └── (贴图文件，目前可能缺失)
```

### 2. 检查依赖mod

**必需**：
- [x] Harmony
- [x] Humanoid Alien Races (HAR)

**可选**：
- [ ] Facial Animation - WIP (如果你做了FA表情)

---

## ?? 测试步骤

### 测试1：Mod加载测试 ?（最重要）

**目标**：确认mod能被游戏识别和加载

**步骤**：
```
1. 启动RimWorld
2. 进入Mod管理界面
3. 查找"Sideria: BloodThorn Knight"
4. 查看是否有红色错误提示
```

**预期结果**：
- ? Mod出现在列表中
- ? 没有红色错误图标
- ? 可以勾选启用

**如果失败**：
- ? Mod不出现 → 检查`About/About.xml`
- ? 红色错误 → 检查错误日志

---

### 测试2：种族定义测试

**目标**：确认血龙种能够生成

**步骤**：
```
1. 启动游戏并加载mod
2. 按 ` 键（波浪号）打开开发模式
3. 按 Shift+F12 打开Debug Actions
4. 搜索"Spawn pawn"
5. 选择"Spawn pawn"
6. 在列表中查找"Sideria_Dracovampir"
7. 点击生成
```

**预期结果**：
- ? 能找到"Sideria_Dracovampir"种族
- ? 点击后地图上生成角色
- ? 角色有正常的贴图（或粉红色占位符）

**如果失败**：
- ? 找不到种族 → `Races_Sideria.xml`配置错误
- ? 游戏崩溃 → 检查错误日志
- ? 生成失败 → 检查背景故事配置

---

### 测试3：角色属性测试

**目标**：检查生成的角色属性是否正确

**步骤**：
```
1. 选中生成的血龙种角色
2. 按 i 键打开角色信息
3. 检查以下内容：
   - Health Tab > 查看Hediffs
   - Bio Tab > 查看背景故事
   - Stats Tab > 查看属性
   - Gear Tab > 查看装备
```

**预期结果**：
```
? Health Tab:
   - 有"龙魂 (Dragon Soul)"hediff
   - 有"血骸 (Blood Curse)"hediff
   
? Bio Tab:
   - 有背景故事（不是"无"）
   - 年龄500+岁
   
? Stats Tab:
   - 移动速度: 4.9 c/s
   - 健康倍数: 1.4
   - 舒适温度: -60°C ~ 60°C
   
? Gear Tab:
   - 可能有装备（如果是BloodThornKnight）
```

**如果失败**：
- ? 缺少hediff → C#代码问题或XML配置错误
- ? 无背景故事 → `Backstories_Sideria.xml`错误
- ? 属性异常 → `Races_Sideria.xml`错误

---

### 测试4：特质测试

**目标**：确认特质正常工作

**步骤**：
```
1. 在角色信息的Character Tab中
2. 查看Traits（特质）列表
3. 应该看到：
   - 龙裔血统 (Draconic Bloodline)
   - 吸血鬼始祖 (Vampiric Progenitor)
   - 骑士荣誉 (Knightly Honor)
   - 血棘精通 (BloodThorn Mastery)
```

**预期结果**：
- ? 所有特质都存在
- ? 鼠标悬停有描述文字
- ? 特质效果生效（检查Stats Tab的buff/debuff）

**如果失败**：
- ? 特质缺失 → `Traits_Sideria.xml`或`PawnKinds_Sideria.xml`错误
- ? 无描述 → 语言文件缺失

---

### 测试5：思想系统测试

**目标**：检查自定义思想是否工作

**步骤**：
```
1. 在Needs Tab中查看Mood
2. 点击查看Thoughts（思想）
3. 尝试触发自定义思想：
   - 让角色战斗 → 应该出现"渴望战斗"思想
   - 长时间和平 → 应该出现"战斗渴望"debuff
```

**预期结果**：
- ? Thoughts列表正常显示
- ? 自定义思想会出现
- ? 心情值受影响

**如果失败**：
- ? 思想不出现 → `Thoughts_Sideria.xml`配置错误
- ? 游戏崩溃 → C#代码问题

---

### 测试6：武器测试（阿茨冈德）

**目标**：确认专属武器可以生成和装备

**步骤**：
```
1. Debug Actions > "Spawn thing"
2. 搜索"Atzgand"或"Sideria_Atzgand"
3. 生成武器
4. 让角色装备
5. 检查战斗效果
```

**预期结果**：
- ? 武器能生成
- ? 有正常贴图（或占位符）
- ? 角色能装备
- ? 攻击能造成伤害
- ? 有特殊效果（如果配置了）

**如果失败**：
- ? 武器不存在 → `Weapons_Sideria.xml`错误
- ? 无法装备 → 武器标签配置错误
- ? 无伤害 → 武器属性配置错误

---

### 测试7：事件测试（落魄骑士）

**目标**：检查"流浪骑士求助"事件

**步骤**：
```
1. Debug Actions > "Execute incident"
2. 搜索"Sideria"相关事件
3. 找到"Sideria_WandererKnightPlea"
4. 执行事件
```

**预期结果**：
- ? 事件能找到
- ? 执行后地图上出现访客
- ? 访客是血龙种
- ? 收到信件/对话
- ? 有交互选项

**如果失败**：
- ? 事件不存在 → `Incidents_Sideria.xml`错误
- ? 执行崩溃 → C#代码或事件逻辑错误
- ? 无交互 → 事件worker未实现

---

### 测试8：双路线系统测试

**目标**：检查龙魂/血骸系统

**步骤**：
```
1. 生成血龙种角色
2. 检查Health Tab是否有：
   - Dragon Soul hediff (龙魂)
   - Blood Curse hediff (血骸)
3. 让角色战斗
4. 观察龙魂/血骸数值变化
5. 检查是否有相关技能解锁
```

**预期结果**：
- ? 初始有龙魂hediff（10%）
- ? 初始有血骸hediff（0%）
- ? 战斗后龙魂增加
- ? 长时间不战斗血骸增加
- ? 达到阈值解锁技能

**如果失败**：
- ? Hediff缺失 → C#代码问题
- ? 数值不变化 → `DualPathManager.cs`未工作
- ? 技能不解锁 → 技能条件配置错误

---

### 测试9：C#组件测试

**目标**：确认C#代码正常加载和运行

**步骤**：
```
1. 启动游戏并加载mod
2. 按F12打开日志
3. 搜索"Sideria"相关日志
4. 查看是否有：
   - [Sideria] Mod initialized
   - [Sideria] 相关的info日志
5. 检查是否有error或exception
```

**预期结果**：
- ? 有初始化日志
- ? 没有红色error
- ? 没有exception堆栈

**如果失败**：
- ? 无日志 → C#未编译或未加载
- ? 有error → 检查错误信息
- ? 有exception → 代码bug

---

### 测试10：场景测试

**目标**：测试自定义开局场景

**步骤**：
```
1. 新游戏 > 选择场景
2. 查找"Sideria"相关场景
3. 选择并开始游戏
4. 检查初始设置
```

**预期结果**：
- ? 能找到自定义场景
- ? 场景描述正确显示
- ? 开局有希德莉亚角色
- ? 角色属性符合设定

**如果失败**：
- ? 场景不存在 → `Scenarios_Sideria.xml`错误
- ? 开局异常 → 场景配置错误

---

## ?? 错误日志查看

### 查看位置

**Windows**:
```
C:\Users\[用户名]\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log
```

**或游戏内**:
```
按 Ctrl+F12 打开日志窗口
```

### 关键错误类型

**红色错误（Error）**：
```
[Error] Could not resolve cross-reference...
→ 某个引用的def不存在

[Error] Exception during...
→ C#代码崩溃

[Error] Could not load file or assembly...
→ DLL缺失或版本不对
```

**橙色警告（Warning）**：
```
[Warning] Backstory XXX not found
→ 背景故事配置问题（可能不影响使用）
```

---

## ?? 测试记录表

### 快速测试清单

```
□ 测试1: Mod加载
□ 测试2: 种族生成
□ 测试3: 角色属性
□ 测试4: 特质系统
□ 测试5: 思想系统
□ 测试6: 武器系统
□ 测试7: 事件系统
□ 测试8: 双路线系统
□ 测试9: C#组件
□ 测试10: 场景系统
```

### 问题记录

```
问题1: ____________________
位置: ____________________
错误信息: ____________________
优先级: □高 □中 □低

问题2: ____________________
...
```

---

## ?? 常见问题快速修复

### 问题1: Mod不加载

**症状**: Mod列表中看不到mod

**检查**:
```xml
<!-- About/About.xml -->
<ModMetaData>
  <name>Sideria: BloodThorn Knight</name>
  <packageId>Sideria.BloodThornKnight</packageId>
  <supportedVersions>
    <li>1.5</li> <!-- 确认版本正确 -->
  </supportedVersions>
</ModMetaData>
```

### 问题2: 种族不生成

**症状**: Debug spawn中找不到种族

**检查**:
```xml
<!-- Races_Sideria.xml -->
<AlienRace.ThingDef_AlienRace ParentName="BasePawn">
  <defName>Sideria_Dracovampir</defName> <!-- 确认defName -->
  ...
</AlienRace.ThingDef_AlienRace>
```

### 问题3: 背景故事错误

**症状**: 角色没有背景故事

**检查**:
```xml
<!-- Backstories_Sideria.xml -->
<!-- 确认backstoryCategories存在 -->
<BackstoryDef>
  <defName>Sideria_BloodThornBackstory</defName>
  ...
</BackstoryDef>

<!-- PawnKinds_Sideria.xml -->
<backstoryCategories>
  <li>Sideria_BloodThornBackstory</li> <!-- 名字匹配 -->
</backstoryCategories>
```

### 问题4: Hediff缺失

**症状**: 角色没有龙魂/血骸

**检查**:
1. `Hediffs_Sideria_DualPath.xml`中hediff是否定义
2. C#代码是否正常加载
3. GameComponent是否注册

### 问题5: 语言文件缺失

**症状**: 游戏中显示defName而非翻译

**检查**:
```xml
<!-- Languages/English/Keyed/Keys.xml -->
<LanguageData>
  <Sideria_Dracovampir_Label>Dracovampir</Sideria_Dracovampir_Label>
  ...
</LanguageData>
```

---

## ?? 最小可用版本(MVP)

如果想快速测试，只需确保以下核心功能：

```
? 必须工作：
1. Mod能加载
2. 种族能生成
3. 角色有基础属性
4. 角色能移动和战斗

? 可以暂缺：
5. 双路线系统
6. 自定义事件
7. 专属武器
8. 特殊技能
9. Facial Animation
10. 完整的思想系统
```

---

## ?? 测试优先级

### 第1优先级（核心功能）
```
P1: Mod加载测试
P1: 种族生成测试
P1: 基础属性测试
```

### 第2优先级（玩法功能）
```
P2: 特质系统
P2: 武器系统
P2: 思想系统
```

### 第3优先级（高级功能）
```
P3: 双路线系统
P3: 事件系统
P3: C#组件
```

### 第4优先级（锦上添花）
```
P4: 场景系统
P4: Facial Animation
P4: 多语言
```

---

## ?? 快速测试流程（5分钟）

```
1分钟: 启动游戏，检查mod是否在列表中
1分钟: 开发模式生成血龙种角色
2分钟: 检查角色属性、特质、hediff
1分钟: 查看日志有无error

总计：5分钟知道mod是否基本可用
```

---

## ?? 下一步

### 如果测试通过
```
→ 继续开发高级功能
→ 添加贴图
→ 优化平衡性
→ 准备发布
```

### 如果测试失败
```
→ 记录所有错误
→ 按优先级修复
→ 重新测试
→ 告诉我具体问题，我帮你修复
```

---

**现在就开始测试吧！** ??

**完成测试后告诉我**：
1. 哪些测试通过了 ?
2. 哪些测试失败了 ?
3. 有什么错误信息 ??

我会帮你逐一修复问题！
