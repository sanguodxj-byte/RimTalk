# C# 落魄骑士事件系统 - 实现说明

## ?? 系统概述

完整实现了希德莉亚落魄骑士事件系统：
- ? 第3天自动触发希德莉亚到访
- ? 希德莉亚作为访客常驻殖民地
- ? 可以每天喂养她
- ? 30天后因感激自动加入（如果有喂养记录）
- ? 给予传奇长剑立即加入并觉醒全部力量
- ? 传奇长剑转化为阿茨冈德并生物编码绑定

---

## ?? 文件结构

```
Sideria.BloodThorn Knight/
├── Source/
│   ├── SideriaBloodThornKnight.csproj  # C#项目文件
│   ├── SideriaMod.cs                   # 模组入口和Harmony初始化
│   ├── SideriaGameComponent.cs         # 游戏数据管理组件
│   ├── SideriaInteractionUtility.cs    # 交互系统（右键菜单）
│   └── HarmonyPatches.cs               # Harmony补丁
├── Assemblies/
│   └── SideriaBloodThornKnight.dll     # 编译后的DLL（需编译）
├── Defs/
│   ├── IncidentDefs/Incidents_Sideria.xml
│   ├── ThoughtDefs/Thoughts_Sideria.xml  # 添加了"收到食物"思想
│   └── ...
└── Languages/
    ├── English/Keyed/Keys.xml           # 英文翻译（已更新）
    └── ChineseSimplified/Keyed/Keys.xml # 中文翻译（已更新）
```

---

## ?? 编译说明

### 前提条件

1. **Visual Studio 2019/2022** 或 **Rider**
2. **.NET Framework 4.7.2** SDK
3. **RimWorld 游戏** 安装路径

### 编译步骤

#### 方法1：使用Visual Studio

```powershell
# 1. 打开项目文件
cd "C:\Users\Administrator\Desktop\rim mod\Sideria.BloodThorn Knight\Source"
# 使用Visual Studio打开 SideriaBloodThornKnight.csproj

# 2. 修改RimWorld引用路径
# 在项目文件中，确认以下路径指向你的RimWorld安装目录：
# <HintPath>D:\steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll</HintPath>

# 3. 构建项目
# 在Visual Studio中：生成 → 生成解决方案 (Ctrl+Shift+B)
```

#### 方法2：使用命令行

```powershell
# 1. 进入Source目录
cd "C:\Users\Administrator\Desktop\rim mod\Sideria.BloodThorn Knight\Source"

# 2. 使用MSBuild编译
# 如果安装了Visual Studio：
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" SideriaBloodThornKnight.csproj /p:Configuration=Release

# 或者使用dotnet CLI（如果安装了.NET SDK）：
dotnet build SideriaBloodThornKnight.csproj -c Release
```

#### 方法3：使用Rider

```
1. 打开Rider
2. 文件 → 打开 → 选择 SideriaBloodThornKnight.csproj
3. 修改RimWorld引用路径（右键项目 → 属性 → 引用）
4. 生成 → 生成解决方案
```

### 编译输出

成功编译后，DLL文件将输出到：
```
Sideria.BloodThorn Knight/Assemblies/SideriaBloodThornKnight.dll
```

---

## ?? 系统工作流程

### 1. 事件触发 (第3天)

```
Day 3
  ↓
SideriaGameComponent.CheckDailyEvents()
  ↓
TriggerSideriaArrival()
  ↓
生成希德莉亚 Pawn
  - 种族: Sideria_Dracovampir
  - PawnKind: Sideria_WandererKnight
  - 派系: Visitor (访客)
  - 无武器
  - 破旧装备
  ↓
在地图边缘生成
  ↓
发送信件通知玩家
```

### 2. 玩家交互

```
右键点击希德莉亚
  ↓
FloatMenuMakerMap_AddHumanlikeOrders_Patch (Harmony)
  ↓
显示菜单选项:
  - 与希德莉亚对话
  - 给予食物
  - 给予传奇长剑
```

#### 选项A: 给予食物

```
选择"给予食物"
  ↓
SideriaInteractionUtility.GiveFood()
  ↓
殖民者拿取食物 → 交给希德莉亚
  ↓
SideriaGameComponent.RecordFeeding()
  - fedCount++
  - lastFedDay = 当前天数
  - 给予"收到食物"正面心情
  ↓
希德莉亚说："感谢这顿饭..."
```

#### 选项B: 给予传奇长剑

```
选择"给予传奇长剑"
  ↓
SideriaInteractionUtility.GiveLegendarySword()
  ↓
SideriaGameComponent.JoinColonyByLegendarySword()
  ↓
[血之共鸣]
  1. 销毁传奇长剑
  2. 生成阿茨冈德
  3. 生物编码绑定到希德莉亚
  4. 希德莉亚装备阿茨冈德
  5. 添加所有特质（力量觉醒）:
     - Sideria_BloodThornMastery
     - Sideria_VampiricProgenitor
  6. 改变派系 → Faction.OfPlayer
  ↓
发送"血之共鸣"信件
  ↓
希德莉亚加入殖民地！
```

### 3. 30天感激加入

```
每天检查 (CheckDailyEvents)
  ↓
if (daysSinceArrival >= 30 && fedCount > 0)
  ↓
JoinColonyByGratitude()
  - 改变派系 → Faction.OfPlayer
  - 保持现有装备（无阿茨冈德）
  - 保持部分特质（力量未完全觉醒）
  ↓
发送"骑士的感激"信件
  ↓
希德莉亚加入殖民地！
```

---

## ?? 关键代码解析

### SideriaGameComponent

```csharp
// 游戏主组件，追踪希德莉亚状态
public class SideriaGameComponent : GameComponent
{
    private Pawn sideria;           // 希德莉亚引用
    private int arrivalDay;         // 到达日期
    private int fedCount;           // 喂养次数
    private int lastFedDay;         // 最后喂养日期
    private bool eventTriggered;    // 是否已触发
    private bool hasJoined;         // 是否已加入
    
    // 每天检查
    public override void GameComponentTick() { ... }
    
    // 第3天触发
    private void TriggerSideriaArrival() { ... }
    
    // 记录喂养
    public void RecordFeeding() { ... }
    
    // 通过传奇剑加入
    public void JoinColonyByLegendarySword(Thing sword) { ... }
    
    // 通过感激加入
    private void JoinColonyByGratitude() { ... }
}
```

### Harmony补丁

```csharp
[HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
public static class FloatMenuMakerMap_AddHumanlikeOrders_Patch
{
    // 在右键菜单中添加希德莉亚选项
    [HarmonyPostfix]
    public static void Postfix(...) { ... }
}
```

---

## ?? 配置参数

可以在 `SideriaGameComponent.cs` 中调整的参数：

```csharp
// 常量定义
private const int EVENT_TRIGGER_DAY = 3;          // 触发天数（改为5则第5天触发）
private const int DAYS_FOR_GRATITUDE_JOIN = 30;   // 感激加入天数（改为20则20天）
```

### 自定义触发条件

如果想添加额外触发条件（如殖民地必须有传奇剑）：

```csharp
private void CheckDailyEvents()
{
    int currentDay = GenDate.DaysPassed;
    
    // 第3天且殖民地有传奇剑时触发
    if (!eventTriggered && currentDay >= EVENT_TRIGGER_DAY)
    {
        // 添加检查
        if (HasLegendarySwordInColony())
        {
            TriggerSideriaArrival();
        }
        return;
    }
    
    // ...
}

private bool HasLegendarySwordInColony()
{
    Map map = Find.Maps.FirstOrDefault(m => m.IsPlayerHome);
    if (map == null) return false;
    
    return map.listerThings.AllThings.Any(t => 
        IsLegendaryLongsword(t));
}
```

---

## ?? 调试技巧

### 启用日志

在 `SideriaMod.cs` 中添加详细日志：

```csharp
static SideriaMod()
{
    var harmony = new Harmony("com.sideria.bloodthornknight");
    harmony.PatchAll();
    
    Log.Message("[Sideria] Mod loaded successfully!");
    Log.Message("[Sideria] Event trigger day: 3");
    Log.Message("[Sideria] Gratitude join days: 30");
}
```

在关键方法中添加日志：

```csharp
private void TriggerSideriaArrival()
{
    Log.Message("[Sideria] Triggering arrival event...");
    // ...
    Log.Message($"[Sideria] Sideria spawned at {spawnPos}");
}
```

### 开发者模式测试

```
1. 开启开发模式（游戏选项）
2. F12打开调试菜单
3. 使用"时间加速"快速到第3天
4. 查看是否触发事件
5. 使用"生成传奇长剑"测试武器转化
```

### 常见问题

**问题1: DLL未加载**
- 检查 `Assemblies/` 目录中是否有 `SideriaBloodThornKnight.dll`
- 检查 `0Harmony.dll` 是否存在
- 查看游戏日志（按 `~` 键）

**问题2: 希德莉亚未在第3天出现**
- 检查是否有玩家主基地地图
- 查看日志中的触发消息
- 确认 `GameComponent` 是否正确注册

**问题3: 右键菜单不显示**
- 确认希德莉亚的派系为 `Visitor`
- 检查 `hasJoined` 标志是否为 `false`
- 查看Harmony补丁是否应用成功

**问题4: 传奇剑不转化**
- 确认是传奇品质（Legendary）
- 确认是长剑类武器
- 检查 `IsLegendaryLongsword` 方法逻辑

---

## ?? 测试清单

- [ ] **编译成功**
  - [ ] 无编译错误
  - [ ] DLL文件生成
  - [ ] Harmony正确引用

- [ ] **事件触发**
  - [ ] 第3天自动触发
  - [ ] 希德莉亚在地图边缘生成
  - [ ] 收到信件通知

- [ ] **希德莉亚状态**
  - [ ] 无武器
  - [ ] 破旧装备
  - [ ] 访客派系
  - [ ] 不会自动离开

- [ ] **交互系统**
  - [ ] 右键显示菜单
  - [ ] 对话功能
  - [ ] 给予食物功能
  - [ ] 给予传奇剑功能

- [ ] **喂养系统**
  - [ ] 每天只计数一次
  - [ ] 喂养次数正确累加
  - [ ] 获得"收到食物"心情

- [ ] **30天加入**
  - [ ] 第33天（3+30）自动加入
  - [ ] 收到感激信件
  - [ ] 派系改为玩家

- [ ] **传奇剑转化**
  - [ ] 原剑消失
  - [ ] 生成阿茨冈德
  - [ ] 生物编码绑定
  - [ ] 希德莉亚装备阿茨冈德
  - [ ] 特质完整（4个）
  - [ ] 立即加入殖民地

---

## ?? 未来扩展

### 可能的改进

1. **更丰富的对话系统**
   - 多个对话选项
   - 根据关系和喂养次数变化
   - 讲述背景故事

2. **任务系统**
   - 希德莉亚请求帮助击败特定敌人
   - 完成任务加速加入

3. **特殊事件**
   - 希德莉亚遇到危险
   - 玩家保护她 → 提前加入

4. **多种加入方式**
   - 给予其他高价值物品
   - 治疗她的伤病
   - 提供高质量房间

5. **动态关系系统**
   - 喂养质量影响关系
   - 忽视她会降低好感
   - 关系影响加入后的心情

---

## ?? 相关资源

- **RimWorld Modding Wiki**: https://rimworldwiki.com/wiki/Modding
- **Harmony文档**: https://harmony.pardeike.net/
- **C# API参考**: 反编译 `Assembly-CSharp.dll`

---

**文档版本**: 1.0  
**最后更新**: 2024  
**状态**: ? 完整实现

**编译完成后，希德莉亚的落魄骑士之旅即可开始！** ????
