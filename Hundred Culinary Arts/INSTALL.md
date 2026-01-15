# ?? Living Weapons 模组安装指南

## ? 编译成功！

恭喜！模组已成功编译，DLL文件位于：`Assemblies/LivingWeapons.dll`

## ?? 安装步骤

### 1. 复制模组到游戏目录

将整个模组文件夹复制到RimWorld的Mods目录：

**目标路径**:
```
D:\steam\steamapps\common\RimWorld\Mods\LivingWeapons\
```

**完整操作**:
```powershell
# 使用PowerShell命令（在模组根目录执行）
Copy-Item -Path . -Destination "D:\steam\steamapps\common\RimWorld\Mods\LivingWeapons\" -Recurse -Force
```

或手动复制整个文件夹到Mods目录。

### 2. 启动RimWorld

1. 启动游戏
2. 进入主菜单
3. 点击 **Mods（模组）**

### 3. 启用模组

1. 在模组列表中找到 **"Living Weapons - 活体兵器"**
2. 勾选启用
3. 确保模组顺序：
   ```
   Core（核心）
   Harmony
   ↓
   Living Weapons - 活体兵器  ← 放在Harmony之后
   ↓
   其他模组...
   ```
4. 点击 **Accept（接受）** 并重启游戏

## ?? 开始游戏

### 新建游戏
建议新建一个测试存档来体验模组功能。

### 测试功能

1. **开启开发模式**（F11）
2. 使用测试工具（详见 `DEV_TOOLS_GUIDE.md`）：
   - 生成极差武器
   - 测试20%概率
   - 生成Lv.5觉醒武器

3. **正常游戏流程**：
   - 让工匠制作武器
   - 品质越差，成为活体武器概率越高！
   - 极差：20%，差：10%，普通：1%

## ?? 如何修改概率（可选）

如果想调整活体武器生成概率：

### 方法1: 修改源代码后重新编译

编辑 `Source/LivingWeapons/LivingWeaponAttributes.cs`:

```csharp
public static class LWConstants
{
    // 生成概率
    public const float AWFUL_SPAWN_CHANCE = 0.50f;      // 改为50%
    public const float POOR_SPAWN_CHANCE = 0.25f;       // 改为25%
    public const float NORMAL_SPAWN_CHANCE = 0.05f;     // 改为5%
    
    // ... 其他设置
}
```

然后重新编译：
```powershell
.\BuildDirect.bat
```

### 方法2: 使用Mod配置（未实现）

未来版本可能添加游戏内配置选项。

## ?? 模组文件结构

安装后的目录结构应该是：
```
D:\steam\steamapps\common\RimWorld\Mods\LivingWeapons\
├── About\
│   └── About.xml                          # 模组元数据
├── Assemblies\
│   └── LivingWeapons.dll                 # 编译后的DLL
├── Defs\
│   └── ThingDefs\
│       └── LivingWeaponDefs.xml          # 技能定义
├── Languages\
│   ├── ChineseSimplified\                 # 简体中文
│   └── English\                           # 英文
├── Source\                                # 源代码（可选）
├── README.md                              # 模组说明
├── DEV_TOOLS_GUIDE.md                    # 开发工具指南
└── QUICKSTART.md                          # 快速开始

```

## ?? 故障排除

### 问题1: 模组列表中找不到模组

**解决方案**:
- 检查文件夹名称是否正确
- 确认 `About/About.xml` 文件存在
- 重启游戏

### 问题2: 启用模组后游戏崩溃

**解决方案**:
- 检查是否安装了Harmony前置模组
- 查看日志文件：`%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld\Player.log`
- 搜索 `[Living Weapons]` 查看错误信息

### 问题3: 武器没有成为活体武器

**可能原因**:
1. **概率问题**: 极差武器也只有20%概率，多试几次
2. **品质太高**: 良好及以上品质不会成为活体武器
3. **补丁未生效**: 使用开发工具测试

**测试方法**:
```
1. 开启Dev模式（F11）
2. 使用测试工具生成100把武器
3. 查看概率是否正常（15%-25%）
```

### 问题4: 武器属性没有生效

**检查**:
- 武器是否已觉醒（查看详细信息应显示等级）
- 是否使用了正确的武器类型
- 查看日志是否有Harmony补丁错误

## ?? 性能影响

- **内存占用**: 极小（<5MB）
- **性能影响**: 可忽略
- **存档兼容**: 可以安全添加到现有存档

## ?? 更新模组

如果修改了源代码：

1. 重新编译：
   ```powershell
   .\BuildDirect.bat
   ```

2. 新的DLL会自动输出到 `Assemblies\` 文件夹

3. 如果模组已安装，只需替换DLL文件：
   ```powershell
   Copy-Item Assemblies\LivingWeapons.dll "D:\steam\steamapps\common\RimWorld\Mods\LivingWeapons\Assemblies\" -Force
   ```

4. 重启游戏即可生效

## ?? 快速测试流程

安装完成后，按以下流程快速验证模组：

1. ? 启动游戏，确认模组出现在列表中
2. ? 启用模组并重启
3. ? 新建测试存档
4. ? 开启Dev模式（F11）
5. ? 生成极差武器3-5次
6. ? 运行概率测试
7. ? 生成Lv.5武器查看属性
8. ? 装备给小人战斗测试经验获取

## ?? 已知限制

1. **主动技能**: 目前只有框架，具体效果需要进一步实现
2. **音效**: 觉醒音效暂未实现
3. **特效**: 没有粒子特效（未来版本考虑）
4. **UI**: 使用原版UI，没有自定义界面

## ?? 获取帮助

如遇到问题：

1. **查看日志**: 
   - 游戏内按 F12
   - 搜索 `[Living Weapons]`

2. **查看文档**:
   - `README.md` - 完整功能说明
   - `DEV_TOOLS_GUIDE.md` - 测试工具使用
   - `QUICKSTART.md` - 快速开始指南

3. **调试建议**:
   - 使用开发工具验证功能
   - 检查Harmony补丁是否成功应用
   - 确认武器确实有CompQuality组件

## ?? 享受游戏！

现在你可以：
- ?? 制作大量极差武器寻找潜伏的战魂
- ?? 培养独一无二的Roguelike武器
- ?? 体验废土捡漏的乐趣
- ?? 通过饥饿机制战略性重铸属性

**记住**: 别扔掉那把极差的短刀，它可能饿了！????

---

**祝你在边缘世界找到传说中的活体兵器！**
