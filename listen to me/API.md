# Listen To Me - API 文档

## 核心类

### CommandParser
文本指令解析器，将用户输入的文本转换为游戏可执行的指令。

#### 主要方法

```csharp
// 解析指令
ParsedCommand ParseCommand(string input, Pawn pawn)

// 参数:
//   input - 用户输入的文本指令
//   pawn - 执行指令的小人
// 返回:
//   ParsedCommand - 解析后的指令对象
```

#### 支持的指令类型

- `Move` - 移动指令
- `Work` - 工作指令
- `Fight` - 战斗指令
- `Wait` - 等待指令
- `Craft` - 制作指令
- `Construct` - 建造指令
- `Haul` - 搬运指令
- `Hunt` - 狩猎指令
- `Gather` - 采集指令
- `Mine` - 挖矿指令
- `Tend` - 医疗指令
- `Clean` - 清洁指令
- `Repair` - 修理指令

---

### CommandExecutor
指令执行器，将解析后的指令转换为实际的游戏任务。

#### 主要方法

```csharp
// 执行指令
bool ExecuteCommand(ParsedCommand command, Pawn pawn)

// 参数:
//   command - 解析后的指令
//   pawn - 执行指令的小人
// 返回:
//   bool - 是否成功执行
```

---

### DialogueSystem
对话系统，管理小人与玩家的交互对话。

#### 主要方法

```csharp
// 开始对话
void StartDialogue(Pawn pawn, string playerInput, ParsedCommand command)

// 更新对话状态（每帧调用）
void Update()

// 检查小人是否在对话中
bool IsInDialogue(Pawn pawn)

// 取消对话
void CancelDialogue(Pawn pawn)
```

---

### AdvancedCommandParser
高级指令解析器，提供扩展功能。

#### 主要方法

```csharp
// 解析复合指令
List<ParsedCommand> ParseCompoundCommand(string input, Pawn pawn)

// 提取位置信息
IntVec3 ExtractLocation(string input, Pawn pawn)

// 识别物品质量
void ParseItemQuality(string input, out QualityCategory quality, out ThingDef stuff)

// 解析条件指令
bool ParseConditionalCommand(string input, Pawn pawn, out string condition, out ParsedCommand command)

// 评估条件
bool EvaluateCondition(string condition, Pawn pawn)

// 自动纠错
string AutoCorrect(string input)

// 生成建议
List<string> GenerateCommandSuggestions(string input, Pawn pawn)
```

---

## 数据结构

### ParsedCommand
解析后的指令对象。

```csharp
public class ParsedCommand
{
    public CommandType Type { get; set; }           // 指令类型
    public Thing Target { get; set; }               // 目标物体
    public ThingDef ItemToCraft { get; set; }       // 要制作的物品
    public int Count { get; set; }                  // 数量
    public string OriginalText { get; set; }        // 原始文本
    public IntVec3 TargetLocation { get; set; }     // 目标位置
}
```

### DialogueState
对话状态对象。

```csharp
public class DialogueState
{
    public Pawn Pawn { get; set; }                  // 对话的小人
    public string PlayerInput { get; set; }         // 玩家输入
    public ParsedCommand Command { get; set; }      // 指令
    public string PawnResponse { get; set; }        // 小人回应
    public DialogueStage Stage { get; set; }        // 对话阶段
    public int StartTick { get; set; }              // 开始时间
    public int ResponseTick { get; set; }           // 回应时间
    public int ExecutionTick { get; set; }          // 执行时间
}
```

---

## 扩展开发

### 添加新的指令类型

1. 在 `CommandParser.CommandType` 枚举中添加新类型
2. 在 `CommandParser.KeywordDictionary` 中添加关键词
3. 在 `CommandExecutor` 中实现执行逻辑

示例：

```csharp
// 1. 添加枚举
public enum CommandType
{
    // ...existing types...
    CustomAction  // 新类型
}

// 2. 添加关键词
private static readonly Dictionary<CommandType, List<string>> KeywordDictionary = new Dictionary<CommandType, List<string>>
{
    // ...existing entries...
    { CommandType.CustomAction, new List<string> { "自定义", "custom" }}
};

// 3. 实现执行
private static bool ExecuteCustomAction(ParsedCommand command, Pawn pawn)
{
    // 实现逻辑
    return true;
}
```

### 自定义对话回应

修改 `DialogueSystem.GenerateResponse()` 方法：

```csharp
private static string GenerateResponse(Pawn pawn, string playerInput, ParsedCommand command)
{
    var responses = new List<string>();
    
    // 添加自定义回应逻辑
    if (command.Type == CommandParser.CommandType.CustomAction)
    {
        responses.Add("我的自定义回应");
    }
    
    return responses.RandomElement();
}
```

### Harmony 补丁

如果需要修改游戏核心行为，可以添加 Harmony 补丁：

```csharp
[HarmonyPatch(typeof(TargetClass), "TargetMethod")]
public static class Patch_CustomPatch
{
    [HarmonyPrefix]
    public static bool Prefix(/* parameters */)
    {
        // 补丁逻辑
        return true; // true = 继续执行原方法, false = 跳过
    }
    
    [HarmonyPostfix]
    public static void Postfix(/* parameters */)
    {
        // 后处理逻辑
    }
}
```

---

## 调试工具

### 启用调试模式

```csharp
DebugTools.DebugMode = true;
```

### 测试指令解析

```csharp
DebugTools.TestCommandParsing("去厨房做饭", pawn);
```

### 生成诊断报告

```csharp
string report = DebugTools.GenerateDiagnosticReport(pawn);
Log.Message(report);
```

### 性能统计

```csharp
DebugTools.PerformanceStats.PrintStats();
DebugTools.PerformanceStats.Reset();
```

---

## 事件系统

目前未实现完整的事件系统，但可以通过以下方式订阅关键事件：

```csharp
// 在 CommandExecutor.ExecuteCommand 中添加回调
public static Action<Pawn, ParsedCommand, bool> OnCommandExecuted;

// 使用
CommandExecutor.OnCommandExecuted += (pawn, command, success) =>
{
    // 处理逻辑
};
```

---

## 配置选项

未来版本可能添加配置文件支持，当前可以通过修改代码实现：

### 修改默认延迟

在 `DialogueSystem.GetResponseDelay()` 中：

```csharp
int baseDelay = 60;      // 基础延迟（tick）
int perCharDelay = 10;   // 每字延迟（tick）
```

### 修改关键词

在 `CommandParser.KeywordDictionary` 中添加或修改关键词。

---

## 最佳实践

1. **性能考虑**: 避免在每帧更新中执行复杂查询
2. **错误处理**: 使用 try-catch 包裹可能出错的代码
3. **日志记录**: 使用 `Log.Message()` 记录重要信息
4. **用户反馈**: 使用 `Messages.Message()` 向玩家显示消息
5. **兼容性**: 考虑与其他 mod 的兼容性

---

## 常见问题

### Q: 如何添加新的关键词？
A: 修改 `CommandParser.KeywordDictionary`

### Q: 如何自定义小人回应？
A: 修改 `DialogueSystem.GenerateResponse()`

### Q: 如何禁用对话系统？
A: 在 UI 中取消勾选"使用对话模式"

### Q: 如何识别自定义物品？
A: 确保物品有正确的 `label` 或在 `CommandParser.ExtractCraftItem()` 中添加特殊处理

---

## 联系方式

- GitHub: [项目地址]
- Steam Workshop: [工坊页面]
- Discord: [Discord服务器]

---

最后更新: 2024-01-01
版本: 1.0.0
