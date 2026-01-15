# ? 部署成功！

## 部署信息

**时间**: 2025-11-23 22:06  
**版本**: v1.1.0  
**模式**: 符号链接  
**状态**: ? 已完成

---

## 部署详情

### 位置
```
D:\Steam\steamapps\common\RimWorld\Mods\ListenToMe
```

### 统计
- **文件数**: 235 个
- **总大小**: 10.3 MB
- **DLL 大小**: 50 KB
- **模式**: 符号链接（开发模式）

### 部署内容
? About/ - Mod 元数据  
? Assemblies/ - DLL 文件  
? Defs/ - 游戏定义  
? Languages/ - 本地化文件  
? Patches/ - XML 补丁  
? Source/ - 源代码（符号链接）  
? 文档文件（README.md, COMMAND_FORMAT.md 等）

---

## 使用符号链接的优势

### ? 开发便利性
1. **无需重复部署** - 修改代码后只需重新编译
2. **实时同步** - 文件自动同步到游戏目录
3. **节省空间** - 不占用额外磁盘空间
4. **方便调试** - 修改配置文件实时生效

### ?? 工作流程
```
修改代码
    ↓
重新编译 (build-sdk9.ps1)
    ↓
重启 RimWorld
    ↓
测试新功能
```

**无需重新运行部署脚本！**

---

## 下一步操作

### 1. 启动游戏
```
双击: D:\Steam\steamapps\common\RimWorld\RimWorldWin64.exe
或在 Steam 中启动
```

### 2. 启用 Mod

1. 启动 RimWorld
2. 点击主菜单的 **"Mods"** 按钮
3. 在列表中找到 **"Listen To Me - 听我指挥"**
4. 勾选启用
5. 点击 **"关闭"** 按钮
6. **重启游戏**

### 3. 开始使用

进入游戏后：
1. 选中任意小人
2. 点击出现的 **"文本指令"** 按钮（或按 `L` 键）
3. 输入指令，例如：
   ```
   去厨房做饭
   到仓库拿木材
   前往医疗室治疗伤员
   在裁缝台制作防尘大衣
   ```
4. 点击 **"执行指令"**
5. 观察小人的反应！

---

## 测试建议

### 基础测试
- [ ] Mod 出现在 Mod 列表中
- [ ] 启用后游戏正常启动
- [ ] 选中小人后出现"文本指令"按钮
- [ ] 点击按钮打开输入窗口

### 功能测试
- [ ] 简单移动: `去厨房`
- [ ] 组合指令: `去厨房做饭`
- [ ] 制作指令: `制作防尘大衣`
- [ ] 战斗指令: `攻击敌人`
- [ ] 数量指定: `制作3个木墙`
- [ ] 对话模式: 勾选后小人会回应

### 检查日志
如果遇到问题，查看日志：
```
%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log
```
搜索 `[ListenToMe]` 前缀的消息

---

## 开发工作流

### 修改代码后

1. **重新编译**
   ```powershell
   .\build-sdk9.ps1
   ```
   或在 Visual Studio 中按 `Ctrl+Shift+B`

2. **重启游戏**
   - 关闭 RimWorld
   - 重新启动
   - 测试新功能

3. **查看日志**
   - 检查是否有错误
   - 搜索 `[ListenToMe]` 消息

**注意**: 由于使用符号链接，无需重新运行部署脚本！

### 常用脚本

| 脚本 | 用途 |
|------|------|
| `build-sdk9.ps1` | 仅编译 |
| `build-and-deploy.ps1` | 编译 + 部署 |
| `deploy.ps1` | 仅部署 |

---

## 调试技巧

### 启用调试模式
在游戏内按 `~` 打开控制台，输入：
```csharp
ListenToMe.DebugTools.DebugMode = true
```

### 测试指令解析
```csharp
ListenToMe.DebugTools.TestCommandParsing("去厨房做饭", pawn);
```

### 生成诊断报告
```csharp
string report = ListenToMe.DebugTools.GenerateDiagnosticReport(pawn);
Log.Message(report);
```

---

## 故障排除

### Mod 不显示
- 检查文件夹名称是否正确
- 确认 About.xml 存在
- 查看游戏日志

### 按钮不出现
- 确认 Mod 已启用
- 重启游戏
- 检查 DLL 文件是否存在

### 指令无效
- 启用调试模式
- 查看日志中的错误信息
- 尝试简单指令如"等待"

### 符号链接问题
如果符号链接失败：
1. 使用管理员权限运行脚本
2. 或改用文件复制模式：
   ```powershell
   .\deploy.ps1
   # 选择选项 2
   ```

---

## 修改配置

### 更改快捷键
编辑 `Defs/KeyBindings.xml`:
```xml
<MainButtonKeyBinding>L</MainButtonKeyBinding>
```
改为其他按键

### 添加新关键词
编辑 `Source/ListenToMe/CommandParser.cs`，查找：
- `MovementVerbs` - 移动动词
- `LocationKeywords` - 位置关键词
- `ActionKeywords` - 动作关键词
- `ObjectKeywords` - 对象关键词

修改后重新编译即可

### 修改对话内容
编辑 `Source/ListenToMe/DialogueSystem.cs`，查找：
- `positiveResponses` - 积极回应
- `negativeResponses` - 消极回应
- `neutralResponses` - 中性回应

---

## 版本更新流程

### 当有新版本时

1. **拉取最新代码**（如果使用 Git）
2. **重新编译**
   ```powershell
   .\build-sdk9.ps1
   ```
3. **重启游戏**
4. **测试新功能**

由于使用符号链接，代码更新会自动同步到游戏目录！

---

## 备份和恢复

### 备份存档
```
%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Saves
```

### 卸载 Mod
1. 删除符号链接：
   ```powershell
   Remove-Item "D:\Steam\steamapps\common\RimWorld\Mods\ListenToMe"
   ```
2. 或在游戏中禁用 Mod

### 重新部署
如果需要重新部署：
```powershell
.\build-and-deploy.ps1
```

---

## 性能监控

### 查看性能统计
在开发者控制台：
```csharp
ListenToMe.DebugTools.PerformanceStats.PrintStats();
```

### 重置统计
```csharp
ListenToMe.DebugTools.PerformanceStats.Reset();
```

---

## 反馈渠道

### 报告问题
- GitHub Issues: [链接]
- Steam Workshop: [链接]
- Discord: [链接]

### 提交改进
1. Fork 项目
2. 创建功能分支
3. 提交 Pull Request

---

## 相关文档

- **[README.md](README.md)** - 完整功能说明
- **[COMMAND_FORMAT.md](COMMAND_FORMAT.md)** - 指令格式详解
- **[QUICKSTART.md](QUICKSTART.md)** - 快速入门
- **[BUILD.md](BUILD.md)** - 构建指南
- **[API.md](API.md)** - 开发者 API

---

## 快捷指令速查

### 移动类
```
去厨房
到仓库
前往医疗室
```

### 工作类
```
去厨房做饭
在裁缝台工作
到工作间干活
```

### 战斗类
```
攻击敌人
消灭入侵者
```

### 制作类
```
制作防尘大衣
在裁缝台做3个防尘大衣
```

### 其他
```
清洁
治疗伤员
狩猎
等待
```

---

**部署完成！开始享受用嘴指挥小人的乐趣吧！** ???

*最后更新: 2025-11-23 22:06*  
*版本: v1.1.0*  
*状态: ? 已部署*
