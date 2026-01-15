# 构建说明

## 前置要求

### 必需的 DLL 文件

项目已配置为从 `C:\Users\Administrator\Desktop\rim mod` 读取所需的 RimWorld DLL。

确保该文件夹包含以下文件：
- `Assembly-CSharp.dll` - RimWorld 核心程序集
- `UnityEngine.CoreModule.dll` - Unity 核心模块
- `UnityEngine.dll` - Unity 引擎
- `UnityEngine.IMGUIModule.dll` - Unity UI 模块
- `0Harmony.dll` - Harmony 补丁库

如果 DLL 位置不同，请修改 `Source/ListenToMe/ListenToMe.csproj` 中的 `DllFolder` 路径。

## 编译项目

### 方法1: 使用Visual Studio
1. 打开 `Source/ListenToMe/ListenToMe.csproj`
2. 确保已安装 .NET Framework 4.7.2 SDK
3. 确认 DLL 路径正确（默认: `C:\Users\Administrator\Desktop\rim mod`）
4. 点击"生成" -> "生成解决方案"
5. DLL将输出到 `Assemblies/` 文件夹

### 方法2: 使用 MSBuild 命令行
```powershell
cd "Source/ListenToMe"
msbuild ListenToMe.csproj /p:Configuration=Release
```

### 方法3: 使用 dotnet CLI（如果支持）
```powershell
cd "Source/ListenToMe"
dotnet build ListenToMe.csproj -c Release
```

## 项目结构

```
listen to me/
├── About/                      # Mod 信息
│   ├── About.xml              # Mod 描述
│   ├── Preview.png            # 预览图（需要添加）
│   ├── LoadFolders.xml        # 加载顺序
│   └── Changelog.txt          # 更新日志
├── Assemblies/                 # 编译输出（自动生成）
│   └── ListenToMe.dll
├── Defs/                       # 游戏定义
│   └── KeyBindings.xml        # 键位绑定
├── Languages/                  # 本地化
│   ├── ChineseSimplified/     # 简体中文
│   └── English/               # 英文
├── Patches/                    # XML 补丁
│   └── ListenToMeCore.xml
├── Source/                     # 源代码
│   └── ListenToMe/
│       ├── CommandParser.cs           # 指令解析
│       ├── CommandExecutor.cs         # 指令执行
│       ├── DialogueSystem.cs          # 对话系统
│       ├── Dialog_TextCommand.cs      # UI窗口
│       ├── Command_TextInput.cs       # Gizmo按钮
│       ├── HarmonyPatches.cs          # Harmony补丁
│       ├── ModInitializer.cs          # 初始化
│       ├── AdvancedCommandParser.cs   # 高级解析
│       ├── DebugTools.cs              # 调试工具
│       └── ListenToMe.csproj          # 项目文件
├── Textures/                   # 纹理（可选）
│   └── UI/
│       └── Commands/
│           └── ListenToMe.png # 按钮图标（可选）
└── README.md                   # 说明文档
```

## 依赖项配置

### 1. RimWorld DLL 引用
项目使用本地 DLL 引用，位于：`C:\Users\Administrator\Desktop\rim mod`

如果您的 DLL 在其他位置，请修改项目文件：

```xml
<PropertyGroup>
  <DllFolder>您的DLL文件夹路径</DllFolder>
</PropertyGroup>
```

### 2. .NET Framework
需要 .NET Framework 4.7.2 或更高版本

### 3. NuGet 包
- Krafs.Rimworld.Ref (1.5.4104) - 仅用于编译时引用

## 安装 Mod

### 自动部署（推荐）?

我们提供了自动化部署脚本，可以一键完成编译和部署：

#### 方法1: 一键构建和部署
```powershell
.\build-and-deploy.ps1
```

**功能**:
- ? 自动清理和编译项目
- ? 自动部署到 RimWorld Mods 目录
- ? 支持符号链接或文件复制两种模式
- ? 自动验证部署
- ? 可选启动游戏

**位置**: `D:\Steam\steamapps\common\RimWorld\Mods\ListenToMe`

#### 方法2: 仅部署（已编译）
```powershell
.\deploy.ps1
```
或双击 `deploy.bat`（需要管理员权限）

**选项**:
1. **符号链接模式**（推荐开发使用）
   - 优势: 修改代码后只需重新编译，无需重新部署
   - 要求: 需要管理员权限
   - 适合: 开发和测试

2. **文件复制模式**（适合发布）
   - 优势: 不需要管理员权限
   - 缺点: 每次修改需要重新部署
   - 适合: 最终发布或无管理员权限时

### 方法3: 直接复制到 Mods 文件夹
1. 将整个 `listen to me` 文件夹复制到 RimWorld Mods 目录：
   - **Steam (本机)**: `D:\Steam\steamapps\common\RimWorld\Mods`
   - Windows (Steam): `C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods`
   - Windows (本地): `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Mods`

2. 启动 RimWorld

3. 在 Mod 列表中启用 "Listen To Me - 听我指挥"

4. 重启游戏或开始新游戏

### 方法4: 创建符号链接（开发推荐）

**Windows (以管理员身份运行 PowerShell)**:
```powershell
# 使用自动脚本（推荐）
.\deploy.ps1

# 或手动创建
New-Item -ItemType SymbolicLink `
  -Path "D:\Steam\steamapps\common\RimWorld\Mods\ListenToMe" `
  -Target "C:\Users\Administrator\Desktop\rim mod\listen to me"
```

**Windows (CMD 管理员)**:
```cmd
mklink /D "D:\Steam\steamapps\common\RimWorld\Mods\ListenToMe" "C:\Users\Administrator\Desktop\rim mod\listen to me"
```

这样可以直接在原位置编辑和测试，无需每次都复制文件。

### 部署脚本对比

| 脚本 | 功能 | 适用场景 |
|------|------|----------|
| `build-and-deploy.ps1` | 编译 + 部署 | 完整开发流程 |
| `deploy.ps1` | 仅部署 | DLL 已编译 |
| `deploy.bat` | 仅部署 | Windows 快捷方式 |
| `build-sdk9.ps1` | 仅编译 | 只需编译 |

## 故障排除

### 编译错误

#### 找不到 Assembly-CSharp
**问题**: 编译器无法找到 RimWorld 的核心 DLL

**解决方案**:
1. 确认 `C:\Users\Administrator\Desktop\rim mod` 文件夹存在
2. 确认该文件夹包含 `Assembly-CSharp.dll`
3. 检查项目文件中的 `DllFolder` 路径是否正确

#### NuGet 包还原失败
**问题**: 无法下载 NuGet 包

**解决方案**:
```powershell
cd "Source/ListenToMe"
dotnet restore
# 或
nuget restore
```

#### 编译器崩溃 (csc.exe 退出代码 -1073741819)
**问题**: C# 编译器崩溃

**解决方案**:
1. 使用 Visual Studio 而不是 dotnet CLI
2. 清理项目: `dotnet clean` 或 "清理解决方案"
3. 删除 `obj` 和 `bin` 文件夹后重新构建
4. 确保 .NET Framework 4.7.2 SDK 已正确安装

### 运行时错误

#### Mod 不显示
**问题**: 游戏中看不到 mod

**解决方案**:
1. 确保 `About/About.xml` 存在且格式正确
2. 检查 mod 文件夹名称是否正确
3. 查看游戏日志中的错误信息

#### 按钮不出现
**问题**: 选中小人后没有"文本指令"按钮

**解决方案**:
1. 检查 `Assemblies/ListenToMe.dll` 是否存在
2. 查看日志中是否有 Harmony 补丁错误
3. 确认 0Harmony.dll 在 DLL 引用中

#### 指令不执行
**问题**: 输入指令后小人没有反应

**解决方案**:
1. 启用开发者模式查看详细日志
2. 检查日志中的 `[ListenToMe]` 消息
3. 确认小人状态正常（未死亡、未倒下）
4. 使用 `DebugTools.TestCommandParsing()` 测试指令解析

### 调试技巧

#### 1. 启用调试模式
在游戏中打开开发者控制台（按 `~` 键），输入：
```
DebugTools.DebugMode = true
```

#### 2. 查看日志
日志位置: 
```
%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log
```

搜索 `[ListenToMe]` 前缀的消息

#### 3. 测试指令解析
```csharp
DebugTools.TestCommandParsing("去厨房做饭", pawn);
```

#### 4. 生成诊断报告
```csharp
string report = DebugTools.GenerateDiagnosticReport(pawn);
Log.Message(report);
```

## 开发工作流

### 快速迭代流程
1. 修改代码
2. 编译项目 (Ctrl+Shift+B)
3. 重启 RimWorld
4. 测试更改
5. 查看日志

### 使用符号链接
如果使用符号链接方式安装，只需：
1. 修改代码并编译
2. 重启游戏即可看到更改

### 调试建议
1. **日志记录**: 大量使用 `Log.Message()` 记录关键步骤
2. **开发者模式**: 始终在开发者模式下测试
3. **增量测试**: 每次只修改一小部分功能
4. **版本控制**: 使用 Git 管理代码变更

## 性能优化建议

1. **避免频繁查询**: 缓存常用的查询结果
2. **限制更新频率**: 对话系统每10 tick更新一次，不是每帧
3. **使用对象池**: 重用 ParsedCommand 对象
4. **延迟加载**: 只在需要时才查询地图数据

## 发布准备

### 发布前检查清单
- [ ] 代码编译无错误
- [ ] 在游戏中测试所有主要功能
- [ ] 检查日志中没有错误
- [ ] 更新 `About/About.xml` 版本号
- [ ] 更新 `About/Changelog.txt`
- [ ] 添加 `About/Preview.png` (600x600)
- [ ] 更新 `README.md`
- [ ] 清理调试代码

### 打包发布
1. 删除不必要的文件：
   - `Source/` 文件夹（可选，如果要开源则保留）
   - `.vs/` 文件夹
   - `obj/` 和 `bin/` 文件夹
   - `*.user` 文件

2. 压缩为 ZIP 文件

3. 上传到 Steam Workshop 或其他平台

## 贡献

欢迎提交 Issue 和 Pull Request！

详见 [CONTRIBUTING.md](CONTRIBUTING.md)

## 许可证

MIT License - 详见 [LICENSE](LICENSE)

## 相关资源

- [RimWorld Modding Wiki](https://rimworldwiki.com/wiki/Modding_Tutorials)
- [Harmony Documentation](https://harmony.pardeike.net/)
- [RimWorld Discord](https://discord.gg/rimworld)
