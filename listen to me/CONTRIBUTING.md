# 贡献指南 / Contributing Guide

感谢您考虑为 Listen To Me 做出贡献！

## 如何贡献

### 报告问题 (Bug Reports)

如果您发现了 bug，请创建一个 Issue 并包含以下信息：

1. **问题描述**: 清晰地描述问题是什么
2. **重现步骤**: 详细的步骤说明如何重现问题
3. **期望行为**: 说明您期望发生什么
4. **实际行为**: 说明实际发生了什么
5. **环境信息**:
   - RimWorld 版本
   - Mod 版本
   - 其他已安装的 mod
   - 操作系统
6. **截图/日志**: 如果可能，提供截图或日志文件

### 功能请求 (Feature Requests)

我们欢迎新功能建议！请创建 Issue 并说明：

1. **功能描述**: 详细描述建议的功能
2. **使用场景**: 说明为什么需要这个功能
3. **预期效果**: 描述功能应该如何工作
4. **替代方案**: 如果有的话，说明其他可能的实现方式

### 提交代码 (Pull Requests)

1. **Fork 项目**
2. **创建分支**: `git checkout -b feature/your-feature-name`
3. **编写代码**: 
   - 遵循现有代码风格
   - 添加必要的注释
   - 确保代码编译通过
4. **测试**: 在游戏中测试您的更改
5. **提交**: `git commit -m "Add: 功能描述"`
6. **推送**: `git push origin feature/your-feature-name`
7. **创建 Pull Request**

## 代码规范

### C# 编码风格

```csharp
// 使用 PascalCase 命名类和方法
public class MyClass
{
    // 使用 camelCase 命名私有字段
    private int myField;
    
    // 使用 PascalCase 命名属性
    public int MyProperty { get; set; }
    
    // 方法注释
    /// <summary>
    /// 方法描述
    /// </summary>
    public void MyMethod()
    {
        // 实现
    }
}
```

### 命名约定

- **类**: `PascalCase` (例如: `CommandParser`)
- **方法**: `PascalCase` (例如: `ParseCommand`)
- **变量**: `camelCase` (例如: `inputText`)
- **常量**: `PascalCase` (例如: `MaxCount`)
- **私有字段**: `camelCase` (例如: `targetPawn`)

### 注释规范

```csharp
/// <summary>
/// 类或方法的简要说明
/// </summary>
/// <param name="paramName">参数说明</param>
/// <returns>返回值说明</returns>
public ReturnType MethodName(ParamType paramName)
{
    // 单行注释用于解释代码逻辑
    
    /* 多行注释
     * 用于更详细的说明
     */
}
```

## 开发流程

### 设置开发环境

1. 克隆项目: `git clone [repository-url]`
2. 打开 `Source/ListenToMe/ListenToMe.csproj`
3. 配置 RimWorld 游戏路径
4. 构建项目

### 测试

1. 确保代码编译成功
2. 在游戏中启用 DevMode
3. 测试所有相关功能
4. 检查日志中的错误

### 提交前检查清单

- [ ] 代码编译无错误
- [ ] 代码符合编码规范
- [ ] 添加了必要的注释
- [ ] 在游戏中测试过
- [ ] 更新了相关文档
- [ ] 没有遗留调试代码

## 优先级领域

我们特别欢迎以下方面的贡献：

1. **关键词扩展**: 添加更多的命令关键词
2. **语言支持**: 改进多语言支持
3. **AI 分析**: 改进智能指令分析
4. **UI 改进**: 改善用户界面
5. **性能优化**: 提升 mod 性能
6. **文档**: 完善文档和示例

## 分支策略

- `main`: 稳定版本
- `develop`: 开发版本
- `feature/*`: 新功能分支
- `bugfix/*`: Bug 修复分支
- `hotfix/*`: 紧急修复分支

## 版本号规则

使用语义化版本 (Semantic Versioning):

- **主版本号 (Major)**: 不兼容的 API 修改
- **次版本号 (Minor)**: 向下兼容的功能新增
- **修订号 (Patch)**: 向下兼容的问题修正

例如: `1.2.3`

## 文档贡献

文档改进同样重要！如果您发现文档中的错误或不清楚的地方：

1. 直接提交 PR 修改 Markdown 文件
2. 或创建 Issue 说明问题

## 本地化 (Localization)

要添加新语言支持：

1. 在 `Languages/` 下创建新的语言文件夹
2. 复制 `English/` 中的文件结构
3. 翻译 XML 文件中的文本
4. 测试确保翻译正确显示

## 代码审查

所有 Pull Request 都需要经过代码审查：

- 至少一个维护者审批
- 所有讨论已解决
- CI 检查通过（如果有）

## 行为准则

我们致力于提供友好、安全和包容的环境：

- 尊重不同观点和经验
- 接受建设性批评
- 关注对社区最有利的事情
- 对其他社区成员表现出同理心

## 许可证

提交贡献表示您同意该贡献在 MIT 许可证下发布。

## 联系方式

如有疑问，请通过以下方式联系：

- GitHub Issues: [项目 Issues 页面]
- Discord: [Discord 服务器]
- Email: [邮箱地址]

---

再次感谢您的贡献！??
