# ?? 紧急恢复指南

## 当前问题
- 按钮消失
- RimTalk 对话无效
- 可能的编译错误

## ?? 立即修复步骤

### 步骤 1: 关闭游戏
**必须完全关闭 RimWorld！**

### 步骤 2: 删除有问题的文件
运行以下命令：

```powershell
cd "C:\Users\Administrator\Desktop\rim mod\listen to me"

# 删除 RimTalk 集成文件（这些可能导致问题）
Remove-Item "Source\ListenToMe\RimTalkDialogueListener.cs" -ErrorAction SilentlyContinue
Remove-Item "Source\ListenToMe\RimTalkIntegration.cs" -ErrorAction SilentlyContinue

Write-Host "已删除 RimTalk 集成文件" -ForegroundColor Green
```

### 步骤 3: 简化 HarmonyPatches.cs
从 HarmonyInitializer 中移除 RimTalk 调用：

```csharp
// 在 HarmonyInitializer 的 static 构造函数中
// 注释掉这两行：
// RimTalkDialogueListener.TryPatchRimTalk(harmony);
// RimTalkDialogueListener.TryPatchDialogInput(harmony);
```

### 步骤 4: 重新编译
```powershell
.\build-sdk9.ps1
```

### 步骤 5: 重启游戏测试

---

## ?? 快速恢复脚本

复制并运行此完整脚本：

```powershell
# 紧急恢复脚本
$modPath = "C:\Users\Administrator\Desktop\rim mod\listen to me"
cd $modPath

Write-Host "`n?? 开始紧急恢复..." -ForegroundColor Yellow

# 1. 检查游戏是否运行
$rimworld = Get-Process "RimWorldWin64" -ErrorAction SilentlyContinue
if ($rimworld) {
    Write-Host "??  RimWorld 正在运行，请先关闭游戏！" -ForegroundColor Red
    exit
}

# 2. 删除有问题的文件
Write-Host "`n?? 删除 RimTalk 集成文件..." -ForegroundColor Cyan
Remove-Item "Source\ListenToMe\RimTalkDialogueListener.cs" -ErrorAction SilentlyContinue
Remove-Item "Source\ListenToMe\RimTalkIntegration.cs" -ErrorAction SilentlyContinue

# 3. 清理旧的 DLL
Write-Host "`n?? 清理旧文件..." -ForegroundColor Cyan
Remove-Item "Assemblies\ListenToMe.dll" -ErrorAction SilentlyContinue
Remove-Item "Source\ListenToMe\obj" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "Source\ListenToMe\bin" -Recurse -Force -ErrorAction SilentlyContinue

# 4. 重新编译
Write-Host "`n?? 重新编译..." -ForegroundColor Cyan
& .\build-sdk9.ps1

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n? 恢复成功！" -ForegroundColor Green
    Write-Host "`n下一步:" -ForegroundColor Cyan
    Write-Host "  1. 启动 RimWorld" -ForegroundColor White
    Write-Host "  2. 进入游戏" -ForegroundColor White
    Write-Host "  3. 选中小人" -ForegroundColor White
    Write-Host "  4. 应该看到'文本指令'按钮" -ForegroundColor White
    Write-Host "`n如果还是没有按钮，查看日志：" -ForegroundColor Yellow
    Write-Host "  $env:USERPROFILE\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log" -ForegroundColor Gray
} else {
    Write-Host "`n? 编译失败" -ForegroundColor Red
    Write-Host "请查看错误信息" -ForegroundColor Yellow
}
```

---

## ?? 恢复后的功能状态

### ? 保留的功能
- 文本指令按钮（底部 Gizmo）
- 右键菜单（右键点击小人）
- L 键快捷键
- Enter 键执行
- 对话模式（使用预设回应）
- 所有指令类型（Move/Work/Fight等）

### ? 临时移除的功能
- RimTalk 对话集成（因为导致问题）

---

## ?? 如果还是不工作

### 检查清单

1. **DLL 是否存在？**
   ```powershell
   Test-Path "Assemblies\ListenToMe.dll"
   ```

2. **符号链接是否正常？**
   ```powershell
   Get-Item "D:\Steam\steamapps\common\RimWorld\Mods\ListenToMe" | Select-Object LinkType, Target
   ```

3. **查看游戏日志**
   搜索 `[ListenToMe]`，应该看到：
   ```
   [ListenToMe] ? Harmony 补丁应用成功！
   ```

### 如果日志中没有 [ListenToMe]
→ Mod 没有加载
→ 检查 Mod 列表中是否启用
→ 尝试禁用其他 Mod 测试

### 如果有 [ListenToMe] 但按钮不显示
→ Harmony 补丁可能失败
→ 在控制台运行诊断：
```csharp
var pawn = Find.Selector.SingleSelectedThing as Verse.Pawn;
var harmony = HarmonyLib.Harmony.GetPatchInfo(typeof(Verse.Pawn).GetMethod("GetGizmos"));
Log.Message(harmony != null ? "补丁已应用" : "补丁未应用");
```

---

## ?? 为什么删除 RimTalk 集成？

**原因**：
1. RimTalkDialogueListener 使用反射访问 RimTalk
2. 如果 RimTalk API 变化，可能导致错误
3. 反射调用可能在某些情况下失败
4. 导致整个 Mod 初始化失败

**后果**：
- Harmony 补丁未应用
- 按钮不显示
- 所有功能失效

**解决方案**：
- 先让基本功能工作
- 稍后单独测试 RimTalk 集成
- 确保失败不影响主功能

---

## ?? 立即行动

1. **关闭游戏**
2. **运行恢复脚本**（上面的 PowerShell 脚本）
3. **重启游戏**
4. **测试按钮**

如果按钮恢复了，我们再慢慢添加 RimTalk 集成！

