# ?? FA表情差分快速参考卡

## ?? 必需文件清单（45张）

### 基础层（3张）
```
? Base_south.png
? Base_north.png  
? Base_east.png
```

### 眼睛层（21张）
```
? Eyes_Open × 3      (睁眼-正常)
? Eyes_Half × 3      (半闭-困倦)
? Eyes_Closed × 3    (闭眼-眨眼)
? Eyes_Happy × 3     (开心-眯眼)
? Eyes_Angry × 3     (愤怒-瞪眼)
? Eyes_Sad × 3       (悲伤-低垂)
? Eyes_Pain × 3      (痛苦-紧闭)
```

### 嘴巴层（21张）
```
? Mouth_Neutral × 3  (中性-闭嘴)
? Mouth_Smile × 3    (微笑-上扬)
? Mouth_Frown × 3    (皱眉-下垂)
? Mouth_Talk1 × 3    (说话帧1)
? Mouth_Talk2 × 3    (说话帧2)
? Mouth_Talk3 × 3    (说话帧3)
? Mouth_Open × 3     (张大-惊讶)
```

---

## ?? AI生成速查表

### 通用Base Prompt
```
anime chibi, RimWorld style, 45-degree isometric,
female dracovampir knight, hooded cloak, white hair,
red glowing eyes, dragon horns, pale skin,
512x512, transparent background, game-ready
```

### 分层生成要点

| 层 | 关键要求 | 特殊说明 |
|---|---------|---------|
| **Base** | 头巾+发型+轮廓 | ? 不画眼睛和嘴巴 |
| **Eyes** | 只画眼睛 | ? 其他部分透明 |
| **Mouth** | 只画嘴巴 | ? 其他部分透明 |

---

## ?? 位置参考（128x128）

### 眼睛位置
```
South: (64, 75)
North: (64, 70)
East:  (70, 72)
```

### 嘴巴位置
```
South: (64, 55)
North: (64, 52)
East:  (70, 53)
```

---

## ? MVP最小版本（9张）

想快速测试？先做这些：

```
1. Base_south/north/east
2. Eyes_Open_south/north/east
3. Mouth_Neutral_south/north/east
```

合成测试：Base + Eyes_Open + Mouth_Neutral = 完整脸

---

## ?? Photoshop对齐模板

```
1. 创建128x128画布
2. 绘制参考线：
   - 垂直: x=64 (中心)
   - 水平眼睛: y=75, 70, 72
   - 水平嘴巴: y=55, 52, 53
3. 所有表情使用此模板
```

---

## ?? 快速测试步骤

```
1. 在Photoshop中叠加：
   Base (底层)
   + Mouth_Neutral (中层)
   + Eyes_Open (顶层)

2. 看起来像完整的脸？
   ? Yes → 继续
   ? No  → 调整位置
```

---

## ?? 文件夹结构

```
Textures/Things/Pawn/Humanlike/Heads/Dracovampir/
└── 放入所有45张PNG文件
```

---

## ?? 制作顺序建议

```
Day 1: Base + Eyes_Open + Mouth_Neutral (MVP)
Day 2: 所有Eyes变体 (眼睛表情)
Day 3: 所有Mouth变体 (嘴巴动画)
Day 4: 测试和微调
```

---

## ?? 关键提示

?? **透明度检查**
```
- Base层：眼睛和嘴巴区域必须透明
- Eyes层：只有眼睛，其他全透明
- Mouth层：只有嘴巴，其他全透明
```

?? **位置对齐**
```
- 使用网格/参考线确保一致
- 所有south文件眼睛位置相同
- 所有north文件眼睛位置相同
- 所有east文件眼睛位置相同
```

?? **文件命名**
```
严格按照格式：
[部位]_[类型]_[方向].png
例如：Eyes_Happy_south.png
```

---

## ?? 常见问题

**Q: 眼睛位置对不上？**
A: 在Photoshop中用参考线，记录精确坐标

**Q: 透明区域有白边？**
A: 导出时选择PNG-24，勾选"透明度"

**Q: 表情看起来怪？**
A: 参考RimWorld原版角色表情，保持简洁

**Q: AI生成不稳定？**
A: 使用ControlNet或img2img保持一致性

---

## ?? 需要帮助？

查看完整指南：
- `FACIAL_ANIMATION_IMPLEMENTATION.md` - 详细教程
- `TEXTURE_DEPLOY_GUIDE.md` - 贴图部署
- `Patches/FacialAnimation_Compatibility.xml` - 配置文件

---

**预估时间**：13-23小时
**难度**：★★★☆☆ (中等)
**效果**：★★★★★ (超赞)

开始制作吧！???
