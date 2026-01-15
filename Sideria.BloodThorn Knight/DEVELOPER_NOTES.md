# 开发者笔记 - 落魄骑士事件系统

## ?? 设计思路

### 核心概念
创建一个情感驱动的招募系统，让玩家通过善意行为建立关系，而不是传统的购买或俘虏。

### 设计目标
1. **沉浸式叙事** - 希德莉亚不是商品，而是有故事的角色
2. **玩家选择** - 多条路线，不同投资和回报
3. **情感连接** - 通过日常互动建立关系
4. **游戏平衡** - 传奇剑昂贵但立即见效，喂养便宜但需耐心

---

## ?? 用户体验流程

### 理想游戏流程（30天路线）

```
Day 1-2: 正常游戏，建立基地
    ↓
Day 3: "咦？来了个访客！"
    → 玩家好奇点击
    → "她叫希德莉亚，看起来很疲惫..."
    ↓
Day 3-7: 尝试性互动
    → "给她点食物吧"
    → 希德莉亚："谢谢你的善意"
    → 玩家："+5心情，还不错"
    ↓
Day 8-15: 建立习惯
    → 每天都会想起喂她
    → 开始关心这个角色
    → 对话了解她的故事
    ↓
Day 16-30: 期待加入
    → "还有X天她就会加入了"
    → 玩家投入情感
    → 准备好迎接新成员
    ↓
Day 33: 感动时刻
    → "因为你的善意，希德莉亚决定加入！"
    → 玩家：成就感满满
    → 虽然她没有传奇剑，但这是自己培养的
```

### 快速路线（传奇剑）

```
Day 3: 希德莉亚出现
    ↓
玩家：
    选项A: "我有传奇剑！立即满足她的愿望"
        → 血之共鸣！
        → 力量觉醒！
        → 立即获得强大战士
        → 代价：失去昂贵的传奇剑
    
    选项B: "先养着，看看再说"
        → 转入30天路线
```

---

## ?? 技术实现细节

### 为什么选择GameComponent？

**优点：**
- ? 全局唯一实例
- ? 自动序列化支持
- ? 每Tick自动调用
- ? 生命周期管理完善

**替代方案对比：**
- ? WorldComponent：只适合世界级数据
- ? MapComponent：地图卸载会丢失
- ? 静态类：无法序列化

### 为什么用Harmony而不是继承？

**优点：**
- ? 不破坏原版类
- ? 模组间兼容性好
- ? 可以Postfix多个方法
- ? 运行时动态注入

**缺点（已克服）：**
- ?? 需要小心版本兼容
- ?? 调试稍困难（已添加日志）

### 为什么每天只检查一次？

**性能考虑：**
```csharp
// ? 不好：每Tick检查（每秒60次）
public override void GameComponentTick()
{
    if (GenDate.DaysPassed >= 3)
        TriggerEvent();
}

// ? 好：每天检查一次
public override void GameComponentTick()
{
    if (Find.TickManager.TicksGame % 60000 == 0)
        CheckDailyEvents();
}
```

**性能对比：**
- 每Tick：60次/秒 × 60秒 × 24小时 = 518万次/天
- 每天：1次/天
- 性能提升：**518万倍**

---

## ?? 已知陷阱和解决方案

### 陷阱1: Pawn引用丢失

**问题：**
```csharp
private Pawn sideria;  // 可能在加载存档后为null
```

**解决：**
```csharp
public override void ExposeData()
{
    Scribe_References.Look(ref sideria, "sideria");  // 正确序列化引用
}
```

### 陷阱2: 访客自动离开

**问题：**
原版访客有默认停留时间

**解决：**
```csharp
sideria.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Guest);
// Guest状态不会自动离开，与Visitor不同
```

### 陷阱3: 每天喂养多次

**问题：**
玩家可能一天内多次喂养刷计数

**解决：**
```csharp
if (lastFedDay != currentDay)
{
    fedCount++;
    lastFedDay = currentDay;
}
// 只有当天第一次喂养才计数
```

### 陷阱4: 传奇剑判定太严格

**问题：**
只判断defName包含"longsword"会漏掉很多剑

**当前方案：**
```csharp
if (thing.def.defName.ToLower().Contains("longsword") || 
    thing.def.defName.ToLower().Contains("sword"))
```

**可能改进：**
```csharp
// 使用武器标签判断
if (thing.def.weaponTags.Contains("LongSword") ||
    thing.def.weaponTags.Contains("MedievalSword"))
```

---

## ?? 平衡性考量

### 传奇剑路线 vs 喂养路线

| 方面 | 传奇剑路线 | 喂养路线 |
|------|-----------|----------|
| **投资** | 极高（传奇剑） | 低（食物） |
| **等待时间** | 0天 | 30天 |
| **获得特质** | 全部（4个） | 部分（2个） |
| **武器装备** | 阿茨冈德（强大） | 自行装备 |
| **情感体验** | 快速满足 | 渐进培养 |
| **适合玩家** | 后期富裕 | 早期节约 |

### 数值平衡

**喂养成本：**
- 简易餐 × 30 = ~150银（极低）
- 精致餐 × 30 = ~300银（仍然低）

**传奇剑价值：**
- 传奇长剑 = 2000-5000银（极高）

**回报差异：**
- 喂养路线：低投入，长等待，部分力量
- 传奇剑路线：高投入，立即获得，完全力量

**结论：** 平衡！两条路线各有利弊

---

## ?? 设计哲学

### 1. 玩家驱动的叙事

不强制玩家选择特定路线，让玩家自己决定如何对待希德莉亚：
- 慷慨喂养
- 投资传奇剑
- 或者忽视她

每种选择都有结果，无对错之分。

### 2. 情感投资机制

通过日常重复行为（喂养）建立情感连接：
- 每天想起她
- 关心她的状态
- 期待她的加入

这比简单的"购买"更有代入感。

### 3. 即时 vs 延迟满足

传奇剑提供即时满足（快乐），喂养提供延迟满足（成就感）。
两种心理需求都被满足。

### 4. 尊重玩家时间

30天不算太长（~1-2小时游戏时间），但足够建立关系。
太短：没有情感积累
太长：玩家失去耐心

---

## ?? 未来扩展思路

### 短期（易实现）

1. **食物品质影响**
```csharp
private int GetFoodQualityBonus(Thing food)
{
    if (food.def.defName.Contains("Lavish"))
        return 2;  // 豪华餐计数2次
    if (food.def.defName.Contains("Simple"))
        return 1;  // 简易餐计数1次
    return 0;  // 粗糙食物不计数
}
```

2. **关系值影响对话**
```csharp
private string GetDialogByRelationship()
{
    float opinion = sideria.relations.OpinionOf(giver);
    if (opinion > 80)
        return "warm_dialog";
    else if (opinion > 50)
        return "friendly_dialog";
    else
        return "neutral_dialog";
}
```

### 中期（需要工作）

1. **动态对话树**
   - 基于喂养次数解锁新对话
   - 揭示希德莉亚的背景故事
   - 玩家选择影响关系

2. **任务系统**
   - 希德莉亚请求帮助击败特定敌人
   - 完成任务提前加入
   - 任务难度影响奖励

3. **特殊事件**
   - 随机敌人袭击希德莉亚
   - 玩家保护她 → 关系大增
   - 忽视她 → 失望离开

### 长期（复杂系统）

1. **多阶段觉醒**
   - 第一阶段：喂养10天 → 部分觉醒
   - 第二阶段：喂养30天 → 进一步觉醒
   - 第三阶段：传奇剑 → 完全觉醒

2. **血之共鸣系统扩展**
   - 不同品质武器 → 不同形态阿茨冈德
   - 传奇：完整阿茨冈德
   - 杰作：弱化版
   - 优秀：基础版

3. **希德莉亚剧情线**
   - 过去的故人来访
   - 寻找失落的记忆
   - 面对血族真祖的责任

---

## ?? 代码优化建议

### 当前可改进

1. **缓存查找结果**
```csharp
// ? 每次都查找
private Thing FindLegendarySword()
{
    return GenClosest.ClosestThingReachable(...);
}

// ? 缓存结果
private Thing cachedLegendarySword;
private int lastSwordCheckTick;

private Thing FindLegendarySword()
{
    if (Find.TickManager.TicksGame - lastSwordCheckTick > 250)
    {
        cachedLegendarySword = GenClosest.ClosestThingReachable(...);
        lastSwordCheckTick = Find.TickManager.TicksGame;
    }
    return cachedLegendarySword;
}
```

2. **使用枚举替代多个bool**
```csharp
// ? 多个标志
private bool eventTriggered;
private bool hasJoined;
private bool joinedViaLegendarySword;

// ? 使用枚举
public enum SideriaState
{
    NotArrived,
    Visiting,
    JoinedByGratitude,
    JoinedBySword
}
private SideriaState state;
```

3. **事件系统解耦**
```csharp
// 定义事件
public static event Action<Pawn> OnSideriaArrived;
public static event Action<Pawn, int> OnSideriaFed;
public static event Action<Pawn> OnSideriaJoined;

// 触发事件
OnSideriaArrived?.Invoke(sideria);

// 其他模组可以监听
SideriaGameComponent.OnSideriaArrived += (pawn) =>
{
    Log.Message("My mod: Sideria arrived!");
};
```

---

## ?? 学到的经验

### RimWorld Modding经验

1. **GameComponent是核心**
   - 所有持久化数据放这里
   - 全局事件检查放这里
   - 跨地图状态放这里

2. **Harmony要谨慎**
   - 只补丁必须的方法
   - Postfix比Prefix安全
   - 添加充分的空值检查

3. **性能永远重要**
   - 能每天检查就不要每Tick
   - 能缓存就不要重复查找
   - 能局部就不要全局

4. **用户体验第一**
   - 清晰的反馈消息
   - 合理的等待时间
   - 多样的选择路径

### 游戏设计经验

1. **情感投资有效**
   - 日常互动建立连接
   - 玩家会关心角色
   - 比单纯数值更吸引人

2. **多路线增加重玩性**
   - 不同玩法风格
   - 不同资源投入
   - 不同游戏体验

3. **即时与延迟满足并存**
   - 传奇剑：即时满足
   - 喂养：延迟满足
   - 两者都重要

---

## ?? 推荐阅读

### RimWorld Modding

- **RimWorld Wiki Modding**
  https://rimworldwiki.com/wiki/Modding

- **Harmony文档**
  https://harmony.pardeike.net/

- **反编译Assembly-CSharp**
  使用dnSpy或ILSpy查看源码

### 游戏设计

- **《游戏设计艺术》** - Jesse Schell
  理解玩家心理和情感设计

- **《上瘾模型》** - Nir Eyal
  了解如何创造吸引人的游戏循环

- **《心流理论》** - Mihaly Csikszentmihalyi
  平衡挑战和奖励

---

## ?? 开发感想

这个事件系统展示了RimWorld modding的强大之处：

? **XML定义外观和属性**  
? **C#实现复杂逻辑**  
? **Harmony扩展原版功能**  
? **完美的模组生态**  

从零开始实现一个完整的招募系统，包括：
- 事件触发
- UI交互
- 数据持久化
- 物品转化
- 多路线设计

这不仅是技术练习，更是游戏设计的实践。

**希德莉亚的故事才刚刚开始...** ????

---

**笔记版本**: 1.0  
**作者**: AI助手  
**日期**: 2024  
**状态**: 开发完成，等待编译测试
