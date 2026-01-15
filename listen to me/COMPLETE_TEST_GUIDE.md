# ?? 完整测试和诊断指南

## ? 已完成的修复

1. **添加详细日志** - 每个步骤都会输出日志
2. **修复 Wait 指令** - 使用 `Wait_Combat` JobDef
3. **RimTalk 集成** - 支持 AI 生成对话
4. **Harmony 补丁** - 自动添加按钮和更新对话系统

---

## ?? 测试步骤

### 第一步：启动游戏并检查 Mod 加载

1. **启动 RimWorld**
2. **查看日志** - 应该看到：
   ```
   [ListenToMe] ========================================
   [ListenToMe] 开始初始化 Listen To Me Mod
   [ListenToMe] ? Harmony 补丁应用成功！
   [ListenToMe]   - Pawn.GetGizmos (添加文本指令按钮)
   [ListenToMe]   - TickManager.DoSingleTick (更新对话系统)
   [ListenToMe] Mod 初始化完成！
   ```

**如果看不到这些日志** → Mod 未正确加载

---

### 第二步：检查按钮是否出现

1. **进入游戏**
2. **选中一个殖民者小人**
3. **查看屏幕底部的 Gizmo 栏**

**应该看到**：
- 一个标签为 "文本指令" 的按钮
- 图标是征召图标（或自定义图标）

**如果看不到按钮**：
- 确认小人是你的派系成员（蓝色）
- 确认小人没有死亡
- 按 `F12` 启用开发者模式，查看是否有错误

---

### 第三步：测试直接执行模式

1. **选中小人**
2. **点击 "文本指令" 按钮**（或按 `L` 键）
3. **? 取消勾选 "使用对话模式"**
4. **输入**：`等待`
5. **点击 "执行指令"**

**预期结果**：
- ? 窗口关闭
- ? 看到消息：`? [小人名字] 开始等待`
- ? 小人停止当前工作
- ? 小人头上出现绿色 ?
- ? 小人进入等待状态（约40秒）

**日志应该显示**：
```
[ListenToMe] ========== ExecuteCommand 开始 ==========
[ListenToMe] 输入文本: '等待'
[ListenToMe] 目标小人: [名字]
[ListenToMe] 小人状态: Dead=False, Downed=False, Drafted=False
[ListenToMe] 开始解析指令...
[ListenToMe] 解析结果: Type=Wait, Original='等待'
[ListenToMe] 使用直接执行模式
[ListenToMe] 开始执行指令: 等待 | 类型: Wait | 小人: [名字]
[ListenToMe] [名字] 成功开始等待任务，持续 2500 ticks
[ListenToMe] ? 指令执行成功: 等待 - [名字]
[ListenToMe] CommandExecutor.ExecuteCommand 返回: True
[ListenToMe] ? 指令执行成功
[ListenToMe] ========== ExecuteCommand 结束 ==========
```

---

### 第四步：测试对话模式（可选）

1. **选中小人**
2. **点击 "文本指令" 按钮**
3. **? 勾选 "使用对话模式"**
4. **输入**：`等待`
5. **点击 "执行指令"**

**预期结果**：
- ? 小人头上出现对话气泡（如："好的" / "明白了"）
- ? 1-2秒后开始执行指令
- ? 小人进入等待状态

---

## ?? 故障排除

### 问题 1: 没有任何反应，也没有日志

**可能原因**：
- ExecuteCommand 方法根本没有被调用
- Dialog 窗口的按钮事件没有触发

**测试**：在控制台运行
```csharp
// 检查 Dialog_TextCommand 是否存在
var dialogType = System.Type.GetType("ListenToMe.Dialog_TextCommand, ListenToMe");
Log.Message(dialogType != null ? "? Dialog_TextCommand 存在" : "? Dialog_TextCommand 不存在");
```

---

### 问题 2: 小人被征召（Drafted）

**症状**：小人不执行任何自动任务

**解决**：
- 在测试前**取消征召**小人（按 R 键）
- 征召状态的小人只接受直接命令，不会执行 Job 任务

**检查**：
```csharp
var pawn = Find.Selector.SingleSelectedThing as Verse.Pawn;
Log.Message($"Drafted: {pawn.Drafted}");
```

---

### 问题 3: Job 被立即取消

**可能原因**：
- 小人的工作优先级设置
- 其他 Mod 冲突
- 小人心理状态（精神崩溃等）

**测试**：使用最简单的 Goto Job
```csharp
var pawn = Find.Selector.SingleSelectedThing as Verse.Pawn;
var dest = pawn.Position + new IntVec3(5, 0, 0);
var job = Verse.AI.JobMaker.MakeJob(RimWorld.JobDefOf.Goto, dest);
bool success = pawn.jobs.TryTakeOrderedJob(job, Verse.AI.JobTag.Misc);
Log.Message($"Goto Job: {success}");
```

如果这个都不工作 → 问题在游戏状态，不是我们的代码

---

### 问题 4: forcePause 导致游戏暂停

**症状**：执行指令后游戏暂停，小人不动

**解决**：修改 Dialog_TextCommand
```csharp
// 在构造函数中
this.forcePause = false;  // 改为 false
```

---

## ?? 高级诊断

### 完整诊断脚本

在游戏控制台运行（按 `~` 或 `` ` ``）：

```csharp
Log.Message("\n=== ListenToMe 完整诊断 ===\n");

// 1. 检查 DLL
var asm = System.AppDomain.CurrentDomain.GetAssemblies()
    .FirstOrDefault(a => a.GetName().Name == "ListenToMe");
Log.Message(asm != null ? "? DLL 已加载" : "? DLL 未加载");

// 2. 检查补丁
var patchInfo = HarmonyLib.Harmony.GetPatchInfo(
    typeof(Verse.Pawn).GetMethod("GetGizmos"));
bool patched = patchInfo != null && 
    patchInfo.Owners.Any(o => o.Contains("listenToMe"));
Log.Message(patched ? "? 补丁已应用" : "? 补丁未应用");

// 3. 获取选中的小人
var pawn = Find.Selector.SingleSelectedThing as Verse.Pawn;
if (pawn == null) {
    Log.Error("? 请先选中一个小人");
} else {
    Log.Message($"? 选中小人: {pawn.LabelShort}");
    Log.Message($"  Dead: {pawn.Dead}");
    Log.Message($"  Downed: {pawn.Downed}");
    Log.Message($"  Drafted: {pawn.Drafted}");
    Log.Message($"  Faction: {pawn.Faction?.Name} (IsPlayer={pawn.Faction?.IsPlayer})");
    
    // 4. 测试解析
    var cmd = ListenToMe.CommandParser.ParseCommand("等待", pawn);
    Log.Message($"? 解析: Type={cmd.Type}");
    
    // 5. 测试执行
    bool success = ListenToMe.CommandExecutor.ExecuteCommand(cmd, pawn);
    Log.Message($"{(success ? "?" : "?")} 执行: {success}");
    
    // 6. 检查 Job
    if (pawn.jobs?.curJob != null) {
        Log.Message($"? 当前Job: {pawn.jobs.curJob.def.defName}");
    } else {
        Log.Warning("? 没有当前Job");
    }
}

Log.Message("\n=== 诊断完成 ===\n");
```

---

## ?? 日志文件位置

```
%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log
```

或在 Windows 中：
```
C:\Users\[你的用户名]\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log
```

**查看日志**：
1. 打开文件（用记事本或 Notepad++）
2. 搜索 `[ListenToMe]`
3. 查看所有相关消息

---

## ? 成功标志

如果一切正常，你应该看到：

### 启动时：
```
[ListenToMe] ? Harmony 补丁应用成功！
[ListenToMe] Mod 初始化完成！
```

### 选中小人时（如果启用开发者模式）：
```
[ListenToMe] 为 [名字] 添加了文本指令按钮
```

### 执行指令时：
```
[ListenToMe] ========== ExecuteCommand 开始 ==========
[ListenToMe] 输入文本: '等待'
[ListenToMe] 开始执行指令: 等待 | 类型: Wait
[ListenToMe] [名字] 成功开始等待任务，持续 2500 ticks
[ListenToMe] ? 指令执行成功
[ListenToMe] ========== ExecuteCommand 结束 ==========
```

### 屏幕消息：
```
? [小人名字] 开始等待
```

### 小人行为：
- ? 停止当前工作
- ? 站立不动
- ? 头上出现绿色 ?
- ? 约40秒后恢复正常

---

## ?? 下一步

1. **重启 RimWorld**
2. **按照测试步骤操作**
3. **查看日志**
4. **报告结果**

如果还是不工作，请提供：
- 完整的 `[ListenToMe]` 日志（从启动到执行指令）
- 小人的状态（Drafted? Dead? Downed?）
- 是否有其他 Mod 启用
- 截图（如果可能）

---

**现在已经添加了大量调试日志，应该能准确定位问题所在！** ???
