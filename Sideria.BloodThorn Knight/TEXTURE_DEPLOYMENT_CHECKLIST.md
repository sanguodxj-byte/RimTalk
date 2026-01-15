# ? 血龙种贴图部署完整清单

## ?? 当前状态

### 你的资源
- ? 三张512x512 PNG图片（透明背景）
- ? AI生成，戴头巾版本
- ? 三视图完整（south/north/east）
- ? 图层未分离（整体图）
- ?? 计划：未来添加无头巾版本

### 已创建的文件
- ? `DeployTextures.bat` - 自动化部署脚本
- ? `Textures/README.txt` - 贴图文件夹说明
- ? `TEXTURE_DEPLOY_GUIDE.md` - 快速部署指南
- ? `TEXTURE_FORMAT_ANALYSIS.md` - 技术分析文档
- ? `TEXTURE_QUICK_REFERENCE.md` - 快速参考手册

---

## ?? 立即行动：30分钟部署计划

### 第1步：准备贴图 (10分钟)

**你需要做的**：
1. 打开三张512x512的图片
2. 缩放到128x128（保持比例）
3. 确保透明背景保留
4. 另存为新文件

**工具选择**：
- Photoshop: `图像 > 图像大小 > 128x128`
- GIMP: `图像 > 缩放图像 > 128x128`
- 在线工具: [ResizePixel.com](https://www.resizepixel.com/)

**文件命名**：
```
背面图 → Naked_Dracovampir_south.png
正面图 → Naked_Dracovampir_north.png
侧面图 → Naked_Dracovampir_east.png
```

### 第2步：运行部署脚本 (2分钟)

**操作**：
1. 双击项目根目录的 `DeployTextures.bat`
2. 脚本会自动创建文件夹
3. 按照提示打开文件夹
4. 将三张128x128的图片放入

**预期结果**：
```
? 找到 south.png
? 找到 north.png
? 找到 east.png
```

### 第3步：确认XML配置 (5分钟)

**检查文件**：`Defs/ThingDefs_Races/Races_Sideria.xml`

**找到并确认以下配置**：

```xml
<alienPartGenerator>
  <!-- 应该有这两行 -->
  <customDrawSize>0.88</customDrawSize>
  <customPortraitDrawSize>0.90</customPortraitDrawSize>
</alienPartGenerator>

<graphicPaths>
  <li>
    <!-- 确认路径正确 -->
    <body>Things/Pawn/Humanlike/Bodies/Naked_Dracovampir</body>
  </li>
</graphicPaths>
```

**如果没有，添加上去**。

### 第4步：游戏测试 (10分钟)

**启动游戏**：
1. 运行RimWorld
2. 加载存档或新游戏
3. 按 `~` 键（波浪号）打开控制台

**生成角色**：
```
输入: DebugSpawn
选择: Spawn pawn
找到: Sideria_Dracovampir
点击生成
```

**检查**：
- [ ] 角色正常显示
- [ ] 没有粉红色方块
- [ ] 三个方向都能看到
- [ ] 大小基本合适
- [ ] 头巾、龙角、尾巴、翅膀可见

### 第5步：微调（可选）

**如果角色太大**：
```xml
<customDrawSize>0.80</customDrawSize> <!-- 减小 -->
```

**如果角色太小**：
```xml
<customDrawSize>0.95</customDrawSize> <!-- 增大 -->
```

**修改后**：
- 按 `` ` `` 键打开控制台
- 输入: `reload defs`
- 重新生成角色查看效果

---

## ?? 进阶计划：分离图层（2-3小时）

### 为什么要分离？

**好处**：
- ? 更好的视觉层次
- ? 头巾可以作为装备穿脱
- ? 可以单独调整每个部件
- ? 未来可以添加更多变体

**需要分离的部分**：
1. **龙角** - 头部固定附加
2. **尾巴** - 身体后方
3. **翅膀** - 背部
4. **头巾** - 可选装备

### 分离步骤概要

1. **在图像编辑器中**：
   - 打开512x512原图
   - 使用选择工具框选每个部分
   - 复制到新图层
   - 隐藏其他图层
   - 导出为独立PNG

2. **缩放和命名**：
   ```
   龙角: Dracovampir_Horn_[方向].png
   尾巴: Dracovampir_Tail_[方向].png
   翅膀: Dracovampir_Wings_[方向].png
   头巾: BloodThornHood_[方向].png
   ```

3. **放入文件夹**：
   ```
   Textures/Things/Pawn/Humanlike/Addons/
   Textures/Things/Pawn/Humanlike/Apparel/BloodThornHood/
   ```

4. **更新XML配置**：
   - 添加bodyAddons配置
   - 创建头巾装备定义

**详细指南**：见 `TEXTURE_DEPLOY_GUIDE.md` 的阶段2

---

## ?? 完整检查清单

### 立即部署（必须完成）

**准备阶段**：
- [ ] 三张图片已缩放到128x128
- [ ] 文件已正确命名
- [ ] 透明背景保留

**部署阶段**：
- [ ] 运行DeployTextures.bat
- [ ] 文件已放入Bodies文件夹
- [ ] XML路径已确认

**测试阶段**：
- [ ] 游戏正常启动
- [ ] 角色能生成
- [ ] 贴图正确显示
- [ ] 三个方向都正常
- [ ] 大小比例合适

### 进阶优化（可选）

**图层分离**：
- [ ] 龙角图层已分离并导出
- [ ] 尾巴图层已分离并导出
- [ ] 翅膀图层已分离并导出
- [ ] 头巾图层已分离并导出

**XML配置**：
- [ ] bodyAddons已添加
- [ ] 各部件offset已调整
- [ ] 头巾装备已定义

**测试验证**：
- [ ] 龙角位置正确
- [ ] 尾巴位置正确
- [ ] 翅膀位置正确
- [ ] 头巾可以穿脱

### 未来扩展（计划中）

**无头巾版本**：
- [ ] AI生成无头巾版本
- [ ] 处理和部署
- [ ] 切换系统实现

**多服装变体**：
- [ ] 设计其他服装
- [ ] 创建装备定义
- [ ] 测试兼容性

---

## ?? 故障排除

### 问题：贴图不显示（粉红色方块）

**原因**：
1. 文件不存在
2. 文件名错误
3. 路径拼写错误

**解决**：
1. 检查文件是否在正确位置
2. 检查文件名大小写
3. 重启游戏

### 问题：角色太大/太小

**解决**：
调整 `customDrawSize` 值：
- 0.80 = 80%大小
- 1.00 = 100%大小（标准）
- 1.20 = 120%大小

### 问题：部件位置错误

**解决**：
调整 `bodyAddons` 中的 `offsets`：
```xml
<offsets>
  <south>(X, Y)</south>  <!-- X:左右, Y:上下 -->
</offsets>
```

### 问题：修改XML后不生效

**解决**：
```
控制台输入: reload defs
或重启游戏
```

---

## ?? 参考资料

### 项目文档
- `TEXTURE_DEPLOY_GUIDE.md` - 完整部署指南
- `TEXTURE_FORMAT_ANALYSIS.md` - 技术深度分析
- `TEXTURE_QUICK_REFERENCE.md` - 快速查询手册
- `Textures/README.txt` - 贴图文件夹说明

### 外部资源
- [HAR Wiki](https://github.com/RimWorld-zh/Humanoid-Alien-Races/wiki) - 官方文档
- [Ludeon Forums](https://ludeon.com/forums/index.php?board=14.0) - 社区论坛
- [RimWorld Discord](https://discord.gg/rimworld) - 实时交流

### 工具推荐
- **Photoshop/GIMP** - 图像编辑
- **ImageMagick** - 批量处理
- **TinyPNG** - 文件压缩
- **ResizePixel** - 在线缩放

---

## ?? 最佳实践

### 1. 保留备份
```
OriginalTextures/
├── v1_512x512_hooded/
│   ├── back.png
│   ├── front.png
│   └── side.png
└── v1_512x512_no_hood/  (未来)
```

### 2. 版本管理
- 给每个版本编号（v1, v2, v3）
- 记录修改内容
- 保存成功的配置

### 3. 测试流程
```
修改 → 保存 → 重载Defs → 生成角色 → 检查 → 记录
```

### 4. 渐进式开发
```
阶段1: 基础显示（1天）
    ↓
阶段2: 图层优化（2-3天）
    ↓
阶段3: 装备系统（1-2天）
    ↓
阶段4: 多样化（持续）
```

---

## ?? 下一步行动

### 今天（立即开始）
1. ? 阅读本清单
2. ? 缩放三张图片到128x128
3. ? 运行DeployTextures.bat
4. ? 放置文件到Bodies文件夹
5. ? 游戏中测试

### 本周内
1. ? 完成基础部署
2. ? 微调大小和位置
3. ? 开始规划图层分离

### 下周
1. ? 学习图层分离技术
2. ? 分离第一个部件（龙角）
3. ? 测试bodyAddons配置

---

## ? 激励信息

**你已经拥有**：
- ? 高质量的AI生成贴图
- ? 完整的三视图
- ? 清晰的实施计划
- ? 详细的参考文档

**你即将完成**：
- ?? 第一个可玩的自定义种族
- ?? 精美的角色外观
- ?? 独特的血龙种体验

**预计时间**：
- ? 30分钟看到第一个成果
- ?? 1天完成基础版本
- ?? 1周完成完整版本

**开始吧！你的血棘骑士正在等待！** ????

---

**当前状态**: 准备就绪
**下一步**: 缩放贴图到128x128
**预计完成**: 30分钟后

Good luck! ??
