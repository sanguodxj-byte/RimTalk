# ?? 立即部署清单

## 准备完成！

### ? 已修复的所有错误

1. ? HAR种族定义 - 使用正确语法
2. ? PawnKindDef - 移除Visitor派系，移除DLC属性
3. ? TraitDef - 移除无效标签
4. ? 武器定义 - 移除CompProperties_Quality，添加costList
5. ? Hediff stages - 正确排序

### ?? 已知限制（不影响功能）

- 贴图显示粉红色占位符
- C#功能需要编译（事件系统）
- 部分视觉特效缺失

---

## ?? 现在可以做什么

### 选项A：快速测试（5分钟）

```powershell
# 1. 双击运行
Build.bat

# 2. 启动RimWorld
# 3. 启用mod
# 4. 测试生成角色
```

### 选项B：仅复制XML（1分钟）

```powershell
# 双击运行
QuickDeploy.bat

# 这会复制所有XML文件到RimWorld\Mods
# 不编译C#，但核心功能可用
```

---

## ?? 测试步骤

### 1. 检查Mod加载
```
RimWorld → Mods菜单
找到 "Sideria: BloodThorn Knight"
没有红色错误图标 = 成功
```

### 2. 生成角色
```
游戏中按 ` 键（开发模式）
Shift+F12 → Debug Actions
"Spawn pawn"
找到 "Sideria_Dracovampir"
点击 → 角色出现（粉红色）
```

### 3. 检查属性
```
选中角色 → 按 i 键
? Health Tab: 有"Dragon Soul"和"Blood Curse"
? Character Tab: 有4个特质
? Stats Tab: 移动速度4.9，健康1.4x
```

### 4. 测试战斗
```
生成一些敌人
让血龙种角色战斗
观察血原质变化
角色应该非常强大
```

---

## ?? 如果遇到问题

### 问题：Mod不出现
**解决**：检查About.xml路径是否正确

### 问题：角色无法生成
**解决**：
1. 检查HAR是否已安装
2. 确认HAR在本mod之前加载

### 问题：游戏崩溃
**解决**：
1. 查看错误日志（F12）
2. 检查是否有mod冲突
3. 尝试禁用其他mod

---

## ? 部署命令

### Windows PowerShell

```powershell
# 方法1：完整构建（含C#）
.\Build.bat

# 方法2：仅复制文件
.\QuickDeploy.bat

# 方法3：手动复制
$source = Get-Location
$target = "D:\steam\steamapps\common\RimWorld\Mods\Sideria.BloodThorn Knight"
Copy-Item -Path "$source\*" -Destination $target -Recurse -Force -Exclude "Source","*.md","*.bat"
```

---

## ?? 成功标志

看到以下情况就成功了：

1. ? Mod出现在Mods列表
2. ? 没有红色错误图标
3. ? 可以生成粉红色血龙种角色
4. ? 角色有4个特质
5. ? 角色有血原质和龙魂hediff
6. ? 角色移动速度很快
7. ? 角色战斗很强

---

## ?? 需要帮助？

告诉我：
1. 哪一步出错了
2. 错误信息是什么
3. RimWorld版本
4. 已安装的其他mod

我会立即帮你解决！

---

**准备好了吗？双击 Build.bat 开始部署！** ??
