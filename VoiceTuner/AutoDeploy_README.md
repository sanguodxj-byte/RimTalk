# AutoDeploy.ps1 - 自动部署脚本使用指南

## ?? 脚本功能

这是一个全自动的VoiceTuner Mod部署脚本，包含以下功能：

1. ? **自动查找RimWorld安装目录**
2. ? **编译项目**（清理+构建）
3. ? **验证必需的DLL文件**
4. ? **自动复制System.Net.Http.dll**（如果缺失）
5. ? **部署到RimWorld Mods目录**
6. ? **清理开发文件**（.pdb, .vs, 脚本等）
7. ? **验证部署完整性**

## ?? 快速使用

### 方法1：双击运行（推荐）

直接双击 `AutoDeploy.ps1` 文件，脚本会自动完成所有步骤。

### 方法2：PowerShell命令行

```powershell
# 基本用法（自动搜索RimWorld）
.\AutoDeploy.ps1

# 指定RimWorld路径
.\AutoDeploy.ps1 -RimWorldPath "D:\Games\RimWorld"

# 使用符号链接模式（开发者推荐）
.\AutoDeploy.ps1 -Mode symlink
```

## ?? 使用参数

### `-Mode` 参数

指定部署模式：

- **`copy`**（默认）- 复制文件模式
  - 优点：稳定，不需要管理员权限
  - 缺点：修改源代码后需要重新部署
  - 适合：普通用户、最终发布

- **`symlink`** - 符号链接模式
  - 优点：修改源代码后只需重启游戏
  - 缺点：需要管理员权限
  - 适合：开发调试

### `-RimWorldPath` 参数

手动指定RimWorld安装路径：

```powershell
.\AutoDeploy.ps1 -RimWorldPath "C:\Program Files\RimWorld"
```

脚本会自动搜索常见位置，通常不需要手动指定。

## ?? 脚本执行流程

### 第1步：确定路径
- 定位Mod根目录
- 查找RimWorld安装目录
- 验证必要的目录存在

### 第2步：编译项目
```
dotnet clean
dotnet build
```
- 清理旧的编译输出
- 重新编译项目
- 检查编译错误

### 第3步：验证输出文件
检查必需的DLL文件：
- ? VoiceTuner.dll
- ? 0Harmony.dll
- ? Newtonsoft.Json.dll
- ? System.Net.Http.dll

如果System.Net.Http.dll缺失，自动从NuGet缓存复制。

### 第4步：部署到RimWorld

**复制模式**：
1. 备份现有Mod（如果存在）
2. 复制整个Mod目录到RimWorld/Mods

**符号链接模式**：
1. 删除现有Mod（如果存在）
2. 创建符号链接指向源目录

### 第5步：清理开发文件

自动删除以下文件：
- `*.pdb` - 调试符号
- `.vs/` - Visual Studio缓存
- `obj/`, `bin/` - 临时编译文件
- `*.ps1` - 部署脚本
- `*.md` - 文档（除README.md）
- `global.json` - SDK配置

### 第6步：验证部署

检查部署完整性：
- About.xml
- 所有必需的DLL文件
- 文件大小正常

## ? 成功输出示例

```
╔══════════════════════════════════════════════════════════════════╗
║                  ? 部署成功完成！                                 ║
╚══════════════════════════════════════════════════════════════════╝

?? 部署位置:
   D:\SteamLibrary\steamapps\common\RimWorld\Mods\VoiceTuner

?? 启动游戏步骤:
   1. 启动RimWorld
   2. 在Mod管理器中启用 'Voice Tuner'
   3. 重启游戏
   4. 按 Ctrl+T 打开调节器窗口
```

## ? 常见问题

### 问题1：执行策略错误

**错误信息**：
```
无法加载文件 AutoDeploy.ps1，因为在此系统上禁止运行脚本
```

**解决方案**：
以管理员身份运行PowerShell，执行：
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### 问题2：未找到RimWorld

**错误信息**：
```
? 未找到RimWorld安装目录
```

**解决方案**：
手动指定RimWorld路径：
```powershell
.\AutoDeploy.ps1 -RimWorldPath "你的RimWorld路径"
```

### 问题3：编译失败

**错误信息**：
```
? 编译失败！
```

**解决方案**：
1. 检查.NET SDK是否安装（需要SDK 9）
2. 查看错误详细信息
3. 手动执行：`dotnet build` 查看详细错误

### 问题4：System.Net.Http.dll缺失

**错误信息**：
```
? System.Net.Http.dll 缺失
```

**解决方案**：
脚本会自动从NuGet缓存复制。如果仍然失败：
1. 手动运行：`dotnet restore`
2. 检查NuGet缓存：`%USERPROFILE%\.nuget\packages\system.net.http\4.3.4\lib\net46\`

### 问题5：符号链接创建失败

**错误信息**：
```
? 创建符号链接失败（需要管理员权限）
```

**解决方案**：
- 以管理员身份运行PowerShell
- 或使用复制模式：`.\AutoDeploy.ps1 -Mode copy`

## ?? 开发者模式

### 推荐工作流程

1. **首次部署**：
   ```powershell
   # 以管理员身份运行
   .\AutoDeploy.ps1 -Mode symlink
   ```

2. **修改代码后**：
   - 保存文件
   - 重启RimWorld（无需重新部署）

3. **修改项目配置后**：
   ```powershell
   .\AutoDeploy.ps1 -Mode symlink
   ```

### 优点
- 快速迭代开发
- 无需每次复制文件
- 源代码和游戏目录同步

## ?? 输出文件清单

脚本执行后，RimWorld Mods目录将包含：

```
VoiceTuner/
├── About/
│   └── About.xml                 (1 KB)
├── Assemblies/
│   ├── VoiceTuner.dll           (70 KB)
│   ├── 0Harmony.dll             (910 KB)
│   ├── Newtonsoft.Json.dll      (695 KB)
│   └── System.Net.Http.dll      (193 KB)
├── Defs/
├── Languages/
│   ├── ChineseSimplified/
│   └── English/
├── Source/                       (可选，符号链接模式包含)
└── README.md
```

## ?? 提示和技巧

### 提示1：自动化部署
将脚本添加到Visual Studio的生成后事件：
```xml
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
  <Exec Command="powershell -ExecutionPolicy Bypass -File &quot;$(SolutionDir)..\..\AutoDeploy.ps1&quot;" />
</Target>
```

### 提示2：快速重新部署
创建桌面快捷方式：
```
目标: powershell.exe -ExecutionPolicy Bypass -File "C:\...\AutoDeploy.ps1"
```

### 提示3：批量部署多个Mod
修改脚本，在循环中处理多个Mod目录。

## ?? 相关文档

- **README.md** - Mod完整使用手册
- **BUILD_REPORT.md** - 编译技术报告
- **DEPLOYMENT_COMPLETE.md** - 部署完成指南

## ?? 获取帮助

如果脚本执行失败：

1. **查看详细错误信息**（脚本会显示）
2. **检查日志文件**：
   ```
   C:\Users\[用户名]\AppData\LocalLow\Ludeon Studios\
   RimWorld by Ludeon Studios\Player.log
   ```
3. **验证环境**：
   - .NET SDK 9.0
   - RimWorld 1.4或1.5
   - PowerShell 5.1+

## ?? 版本历史

### v1.0 (2025-12-31)
- ? 初始版本
- ? 自动编译和部署
- ? 自动复制System.Net.Http.dll
- ? 支持符号链接模式
- ? 完整的错误处理

---

**祝您部署顺利！** ??
