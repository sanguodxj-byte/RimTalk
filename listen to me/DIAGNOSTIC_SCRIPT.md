# Listen To Me - 诊断和测试脚本
# 在游戏开发者控制台中运行这些命令来诊断问题

## 步骤 1: 检查 Mod 是否加载
```csharp
Log.Message("=== ListenToMe 诊断 ===");
var assembly = System.AppDomain.CurrentDomain.GetAssemblies()
    .FirstOrDefault(a => a.GetName().Name == "ListenToMe");
if (assembly != null) {
    Log.Message("? ListenToMe DLL 已加载");
    Log.Message($"  版本: {assembly.GetName().Version}");
    Log.Message($"  位置: {assembly.Location}");
} else {
    Log.Error("? ListenToMe DLL 未加载！");
}
```

## 步骤 2: 检查 Harmony 补丁
```csharp
var harmony = HarmonyLib.Harmony.GetPatchInfo(
    typeof(Verse.Pawn).GetMethod("GetGizmos"));
if (harmony != null && harmony.Owners.Any(o => o.Contains("listenToMe"))) {
    Log.Message("? Harmony 补丁已应用到 Pawn.GetGizmos");
} else {
    Log.Error("? Harmony 补丁未应用！");
}
```

## 步骤 3: 启用详细日志
```csharp
ListenToMe.DebugTools.DebugMode = true;
Log.Message("? 已启用详细日志模式");
```

## 步骤 4: 测试指令解析
```csharp
var pawn = Find.Selector.SingleSelectedThing as Verse.Pawn;
if (pawn != null) {
    Log.Message($"选中的小人: {pawn.LabelShort}");
    var command = ListenToMe.CommandParser.ParseCommand("等待", pawn);
    Log.Message($"解析结果: Type={command.Type}, Original={command.OriginalText}");
} else {
    Log.Warning("请先选中一个小人");
}
```

## 步骤 5: 直接测试指令执行
```csharp
var pawn = Find.Selector.SingleSelectedThing as Verse.Pawn;
if (pawn != null) {
    var command = ListenToMe.CommandParser.ParseCommand("等待", pawn);
    bool success = ListenToMe.CommandExecutor.ExecuteCommand(command, pawn);
    Log.Message($"执行结果: {(success ? "成功" : "失败")}");
    
    // 检查小人的任务队列
    if (pawn.jobs?.curJob != null) {
        Log.Message($"当前任务: {pawn.jobs.curJob.def.defName}");
    } else {
        Log.Warning("小人没有当前任务");
    }
} else {
    Log.Warning("请先选中一个小人");
}
```

## 步骤 6: 检查小人状态
```csharp
var pawn = Find.Selector.SingleSelectedThing as Verse.Pawn;
if (pawn != null) {
    Log.Message($"=== {pawn.LabelShort} 状态 ===");
    Log.Message($"  已死亡: {pawn.Dead}");
    Log.Message($"  已倒下: {pawn.Downed}");
    Log.Message($"  派系: {pawn.Faction?.Name ?? "无"}");
    Log.Message($"  是玩家派系: {pawn.Faction?.IsPlayer}");
    Log.Message($"  地图: {pawn.Map != null}");
    Log.Message($"  任务系统: {pawn.jobs != null}");
    if (pawn.jobs != null) {
        Log.Message($"  当前任务: {pawn.jobs.curJob?.def.defName ?? "无"}");
        Log.Message($"  任务队列: {pawn.jobs.jobQueue.Count}");
    }
}
```

## 步骤 7: 测试 Job 创建
```csharp
var pawn = Find.Selector.SingleSelectedThing as Verse.Pawn;
if (pawn != null) {
    try {
        // 测试创建 Wait_Combat Job
        var job = Verse.AI.JobMaker.MakeJob(RimWorld.JobDefOf.Wait_Combat);
        job.expiryInterval = 2500;
        Log.Message($"? Job 创建成功: {job.def.defName}");
        
        // 测试分配 Job
        bool assigned = pawn.jobs.TryTakeOrderedJob(job, Verse.AI.JobTag.Misc);
        Log.Message($"Job 分配结果: {(assigned ? "成功" : "失败")}");
        
        if (assigned && pawn.jobs.curJob != null) {
            Log.Message($"? 小人现在的任务: {pawn.jobs.curJob.def.defName}");
        }
    } catch (System.Exception ex) {
        Log.Error($"? 创建/分配 Job 失败: {ex.Message}");
    }
}
```

## 完整测试脚本（复制到控制台）
```csharp
// 一键运行所有诊断
Log.Message("\n========== ListenToMe 完整诊断 ==========\n");

// 1. 检查 DLL
var assembly = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "ListenToMe");
Log.Message(assembly != null ? "? DLL 已加载" : "? DLL 未加载");

// 2. 检查补丁
var harmony = HarmonyLib.Harmony.GetPatchInfo(typeof(Verse.Pawn).GetMethod("GetGizmos"));
Log.Message((harmony != null && harmony.Owners.Any(o => o.Contains("listenToMe"))) ? "? 补丁已应用" : "? 补丁未应用");

// 3. 测试小人
var pawn = Find.Selector.SingleSelectedThing as Verse.Pawn;
if (pawn == null) {
    Log.Error("? 请先选中一个小人");
} else {
    Log.Message($"? 选中小人: {pawn.LabelShort}");
    Log.Message($"  状态: Dead={pawn.Dead}, Downed={pawn.Downed}");
    Log.Message($"  派系: {pawn.Faction?.Name} (IsPlayer={pawn.Faction?.IsPlayer})");
    
    // 4. 测试解析
    var command = ListenToMe.CommandParser.ParseCommand("等待", pawn);
    Log.Message($"? 解析结果: {command.Type}");
    
    // 5. 测试执行
    bool success = ListenToMe.CommandExecutor.ExecuteCommand(command, pawn);
    Log.Message(success ? "? 执行成功" : "? 执行失败");
    
    if (pawn.jobs?.curJob != null) {
        Log.Message($"? 当前任务: {pawn.jobs.curJob.def.defName}");
    } else {
        Log.Warning("小人没有当前任务");
    }
}

Log.Message("\n========== 诊断完成 ==========\n");
```

## 使用说明

1. 启动 RimWorld 并进入游戏
2. 按 `Ctrl + F12` 启用开发者模式
3. 按 `~` 或 `` ` `` 打开开发者控制台
4. 选中一个小人
5. 复制"完整测试脚本"到控制台
6. 按 Enter 运行
7. 查看输出结果

## 常见问题和解决方案

### 如果 "DLL 未加载"
- 检查 `Assemblies/ListenToMe.dll` 是否存在
- 检查 Mod 是否在游戏中启用
- 重启游戏

### 如果 "补丁未应用"
- 检查日志中是否有 Harmony 错误
- 可能与其他 Mod 冲突
- 尝试禁用其他 Mod

### 如果 "执行失败"
- 查看详细的错误日志
- 确认小人状态正常（未死亡、未倒下）
- 确认小人属于玩家派系

### 如果 "小人没有当前任务"
- 可能 Job 没有正确创建
- 可能 TryTakeOrderedJob 返回 false
- 检查小人是否被征召（Drafted）状态
