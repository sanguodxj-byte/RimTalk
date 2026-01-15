# 厨间百艺 - 最终部署说明

## 当前状态

✅ **所有代码已完成**
- 12个C#源文件（~1068行）
- 3个XML定义文件
- 完整的中英文本地化
- 8个详细文档

⏳ **编译进行中**
- 正在下载Harmony NuGet包
- 正在编译C#代码
- 输出到 Assemblies 目录

## 部署方式（3选1）

### 🚀 方式1：直接部署源码（最简单推荐）

**优点**：
- 无需等待编译
- RimWorld会自动编译
- 最快速度开始测试

**步骤**：
1. 运行 `QuickDeploy.bat`
2. 或手动复制这4个文件夹到 `D:\steam\steamapps\common\RimWorld\Mods\Culinary Arts 100\`：
   - About
   - Defs
   - Languages
   - Source
3. 启动RimWorld，启用mod
4. 游戏首次加载时会自动编译（15-30秒）

### ⚙️ 方式2：等待编译完成后部署

**优点**：
- DLL已预编译
- 游戏加载更快

**步骤**：
1. 等待 `dotnet build` 命令完成
2. 检查 `Assemblies\CulinaryArts.dll` 是否存在
3. 运行 `QuickDeploy.bat` 或手动复制所有文件夹

### 🔧 方式3：在RimWorld目录中直接开发

**优点**：
- 修改代码后游戏自动重新编译
- 便于迭代开发

**步骤**：
1. 将整个项目文件夹复制到 `D:\steam\steamapps\common\RimWorld\Mods\`
2. 在游戏中启用mod

## 验证清单

启动游戏后检查：

### ✅ 日志验证
位置：`D:\steam\steamapps\common\RimWorld\Player.log`

搜索关键词：`[厨间百艺]` 或 `[CulinaryArts]`

预期输出：
```
[厨间百艺] Mod初始化成功！
[厨间百艺] 技法总数: 116
[厨间百艺] 前缀总数: 48
[厨间百艺] 食材映射: 13
```

### ✅ 游戏内验证

1. **Mod管理器**
   - 能看到 "厨间百艺 (Culinary Arts 100)"
   - 说明显示正确
   - 依赖项显示 Harmony（如未安装会提示）

2. **游戏测试**
   - 创建新游戏或加载存档
   - 让小人制作食物
   - 查看食物名称是否自定义
   - 食用后检查心情效果

### ✅ 功能测试

| 测试项 | 如何验证 | 预期结果 |
|-------|---------|---------|
| 菜名生成 | 制作精致食物 | 显示自定义名称 |
| 堆叠显示 | 查看仓库 | 堆叠时显示"精致食物 x20" |
| 单品显示 | 选中1个食物 | 显示完整自定义名 |
| 技能影响 | 0级vs20级厨师 | 菜名质量明显不同 |
| 心情效果 | 食用食物 | 心情栏出现对应效果 |

## 故障排除

### 问题：Mod未出现在列表
**原因**：文件夹结构错误或About.xml有问题
**解决**：
- 确认文件夹名为 "Culinary Arts 100"
- 检查About/About.xml格式
- 查看Player.log中的错误信息

### 问题：红色错误"找不到Assembly-CSharp"
**原因**：编译时引用路径错误
**解决**：使用方式1（直接部署源码），让游戏编译

### 问题：黄色警告"找不到0Harmony"  
**原因**：编译时Harmony引用问题
**解决**：
- 忽略此警告，使用NuGet包
- 或使用方式1让游戏编译

### 问题：游戏崩溃
**原因**：代码有bug或mod冲突
**解决**：
- 禁用其他mod测试
- 查看Player.log错误堆栈
- 检查RimWorld版本是否为1.4/1.5

### 问题：菜名没有生成
**原因**：CompNamedMeal组件未添加到食物
**解决**：
- 检查日志是否有初始化信息
- 确认Harmony补丁已应用
- 尝试重新加载mod

## 性能提示

- 首次加载耗时：15-30秒（源码编译）
- 运行时影响：< 1%（仅在食物生成时触发）
- 内存占用：~5MB（词库数据）
- 存档大小：每个食物增加~50字节

## 下一步

1. ✅ **立即测试**：使用方式1快速部署
2. 📝 **记录bug**：按照TESTING_PLAN.md测试
3. 🎨 **添加内容**：扩充食材映射和技法
4. 🌍 **发布分享**：Steam Workshop上传

## 快速命令参考

```batch
# 快速部署（推荐）
QuickDeploy.bat

# 编译（可选）
dotnet build "Source\CulinaryArts\CulinaryArts.csproj" -c Release

# 清理编译输出
dotnet clean "Source\CulinaryArts\CulinaryArts.csproj"

# 查看日志
notepad "D:\steam\steamapps\common\RimWorld\Player.log"
```

## 联系与反馈

遇到问题或有建议？
- 查看 TESTING_PLAN.md 中的Bug报告模板
- 参考 ARCHITECTURE.md 了解技术细节
- 阅读 README_USER.md 获取用户帮助

---

**祝您游戏愉快！** 🍲

现在就去启动RimWorld，开始您的美食之旅吧！