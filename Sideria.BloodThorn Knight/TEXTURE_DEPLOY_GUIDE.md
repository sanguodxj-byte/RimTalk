# ?? 血龙种贴图快速部署指南

## ? 30分钟快速上线

### 步骤1：准备贴图（10分钟）

1. **缩放图片** 512x512 → 128x128
   ```
   使用任何图片编辑器
   保持纵横比
   保留透明背景
   ```

2. **重命名文件**
   ```
   第一张（背面） → Naked_Dracovampir_south.png
   第二张（正面） → Naked_Dracovampir_north.png
   第三张（侧面） → Naked_Dracovampir_east.png
   ```

### 步骤2：放置文件（5分钟）

创建文件夹并放入文件：
```
项目根目录/Textures/Things/Pawn/Humanlike/Bodies/
└── 放入三张128x128的PNG文件
```

### 步骤3：更新XML（10分钟）

编辑 `Defs/ThingDefs_Races/Races_Sideria.xml`

找到 `<alienPartGenerator>` 标签，添加：

```xml
<customDrawSize>0.88</customDrawSize>
<customPortraitDrawSize>0.90</customPortraitDrawSize>
```

找到 `<graphicPaths>` 标签，确认路径：

```xml
<graphicPaths>
  <li>
    <body>Things/Pawn/Humanlike/Bodies/Naked_Dracovampir</body>
  </li>
</graphicPaths>
```

### 步骤4：测试（5分钟）

1. 启动RimWorld
2. 按 `~` 键打开开发模式
3. 输入：`DebugSpawn` → `Spawn pawn` → `Sideria_Dracovampir`
4. 检查角色显示

**完成！** ??

---

## ?? 进阶：分离图层（2-3小时）

### 需要分离的部分

1. **龙角** - 头部附加
2. **尾巴** - 身体后方
3. **翅膀** - 背部
4. **头巾** - 可选装备

### 分离后的好处

- ? 更好的视觉层次
- ? 头巾可以穿脱
- ? 可以单独调整每个部件

### 配置示例

```xml
<bodyAddons>
  <!-- 龙角 -->
  <li>
    <path>Things/Pawn/Humanlike/Addons/Dracovampir_Horn</path>
    <bodyPart>Head</bodyPart>
    <offsets>
      <south>(0, 0.35)</south>
      <north>(0, 0.30)</north>
      <east>(0.08, 0.32)</east>
    </offsets>
  </li>
  
  <!-- 尾巴 -->
  <li>
    <path>Things/Pawn/Humanlike/Addons/Dracovampir_Tail</path>
    <bodyPart>Torso</bodyPart>
    <layerOffset>-0.20</layerOffset>
    <offsets>
      <south>(0, -0.25)</south>
      <north>(0, -0.15)</north>
      <east>(0.15, -0.20)</east>
    </offsets>
  </li>
  
  <!-- 翅膀 -->
  <li>
    <path>Things/Pawn/Humanlike/Addons/Dracovampir_Wings</path>
    <bodyPart>Torso</bodyPart>
    <layerOffset>-0.15</layerOffset>
    <offsets>
      <south>(0, 0.05)</south>
      <north>(0, 0.10)</north>
      <east>(0.25, 0.08)</east>
    </offsets>
  </li>
</bodyAddons>
```

---

## ?? 调试技巧

### 角色太大/太小？

调整 `customDrawSize`：
- 太大 → 减小数值（如0.80）
- 太小 → 增大数值（如0.95）

### 部件位置不对？

调整 `offsets`：
- X值：正数向右，负数向左
- Y值：正数向上，负数向下

### 查看详细信息

开发模式中：
```
Dev Mode > Toggle: Draw Pawn Debug
```

---

## ?? 快速检查清单

阶段1（必须）：
- [ ] 文件缩放到128x128
- [ ] 文件命名正确
- [ ] 放入正确文件夹
- [ ] XML路径正确
- [ ] 游戏中能看到角色

阶段2（可选）：
- [ ] 分离龙角图层
- [ ] 分离尾巴图层
- [ ] 分离翅膀图层
- [ ] 配置bodyAddons
- [ ] 位置和大小调整正确

阶段3（可选）：
- [ ] 创建头巾装备
- [ ] 准备无头巾版本
- [ ] 装备切换正常

---

## ?? 推荐流程

**第一天**：完成阶段1，看到角色在游戏中
**第二天**：完成阶段2，优化视觉效果  
**第三天**：完成阶段3，添加头巾装备

**不着急，慢慢来，每个阶段都测试好再进行下一步！**

---

需要帮助？查看详细文档：
- `TEXTURE_FORMAT_ANALYSIS.md` - 完整技术分析
- `TEXTURE_QUICK_REFERENCE.md` - 快速参考手册
