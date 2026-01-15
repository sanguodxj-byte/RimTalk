# ?? 血龙种 Facial Animation 完整实施指南

## ?? 项目概述

**目标**：为血龙种实现完整的Facial Animation支持
**角色特征**：戴头巾但脸部可见、龙角、红眼、白/银发
**表情风格**：冷峻、神秘、战士气质

---

## ?? 需要的表情差分文件

### 完整清单

```
Textures/Things/Pawn/Humanlike/Heads/Dracovampir/
├── Base_south.png          (基础头部-背面)
├── Base_north.png          (基础头部-正面)
├── Base_east.png           (基础头部-侧面)
│
├── Eyes_Open_south.png     (眼睛-正常睁开)
├── Eyes_Open_north.png
├── Eyes_Open_east.png
│
├── Eyes_Half_south.png     (眼睛-半闭/困倦)
├── Eyes_Half_north.png
├── Eyes_Half_east.png
│
├── Eyes_Closed_south.png   (眼睛-完全闭合/眨眼)
├── Eyes_Closed_north.png
├── Eyes_Closed_east.png
│
├── Eyes_Happy_south.png    (眼睛-开心)
├── Eyes_Happy_north.png
├── Eyes_Happy_east.png
│
├── Eyes_Angry_south.png    (眼睛-愤怒)
├── Eyes_Angry_north.png
├── Eyes_Angry_east.png
│
├── Eyes_Sad_south.png      (眼睛-悲伤)
├── Eyes_Sad_north.png
├── Eyes_Sad_east.png
│
├── Eyes_Pain_south.png     (眼睛-痛苦)
├── Eyes_Pain_north.png
├── Eyes_Pain_east.png
│
├── Mouth_Neutral_south.png (嘴巴-中性)
├── Mouth_Neutral_north.png
├── Mouth_Neutral_east.png
│
├── Mouth_Smile_south.png   (嘴巴-微笑)
├── Mouth_Smile_north.png
├── Mouth_Smile_east.png
│
├── Mouth_Frown_south.png   (嘴巴-皱眉)
├── Mouth_Frown_north.png
├── Mouth_Frown_east.png
│
├── Mouth_Talk1_south.png   (嘴巴-说话帧1)
├── Mouth_Talk1_north.png
├── Mouth_Talk1_east.png
│
├── Mouth_Talk2_south.png   (嘴巴-说话帧2)
├── Mouth_Talk2_north.png
├── Mouth_Talk2_east.png
│
├── Mouth_Talk3_south.png   (嘴巴-说话帧3)
├── Mouth_Talk3_north.png
├── Mouth_Talk3_east.png
│
├── Mouth_Open_south.png    (嘴巴-张大/惊讶)
├── Mouth_Open_north.png
└── Mouth_Open_east.png
```

**总计**：45张表情差分文件（15种表情 × 3个方向）

---

## ?? 表情差分规格

### 文件规格

| 属性 | 要求 |
|------|------|
| **格式** | PNG (RGBA) |
| **尺寸** | 128x128 像素 |
| **透明度** | ? 必须保留透明背景 |
| **方向** | south/north/east |
| **分层** | 每个表情元素独立 |

### 图层结构

```
完整头部渲染顺序（从后到前）：
1. Base层 - 基础头部（头巾、发型、轮廓）
2. Mouth层 - 嘴巴部分
3. Eyes层 - 眼睛部分
4. Overlay层 - 可选的叠加效果（如眼睛发光）
```

### 重要提示

?? **眼睛和嘴巴层**：
- 只包含眼睛/嘴巴本身
- 其他部分必须透明
- 位置要与Base层精确对齐

?? **Base层**：
- 包含头部、头巾、头发
- **不包含眼睛和嘴巴**（留空透明）
- 保持其他特征完整

---

## ?? AI生成提示词

### 基础设置（所有表情共用）

```
Base Prompt:
anime style, chibi, RimWorld game character, 
45-degree isometric view, pixel art influence,

Character: female dracovampir knight,
- hooded cloak (dark red/black)
- white/silver long hair
- red glowing eyes
- dragon horns
- pale skin with red markings
- mysterious and cold warrior

Background: transparent (alpha channel)
Size: 512x512 px (will scale to 128x128)
Style: clean lines, moderate details, game-ready
```

### 表情1：Base（基础头部）

```
Prompt:
[Base Prompt]

Face details:
- NO eyes drawn (leave transparent)
- NO mouth drawn (leave transparent)
- Complete hood, hair, horns
- Facial structure and markings visible
- All other features intact

Direction: [south/north/east]
Note: This is the base layer, eyes and mouth will be added separately
```

### 表情2-8：眼睛变体

#### Eyes_Open（睁眼）
```
Prompt:
[Base Prompt]

Face: ONLY eyes, all else transparent
Eyes:
- Fully open
- Sharp red glowing eyes
- Cold and focused gaze
- Thin eyebrows slightly furrowed
- Confident warrior expression

Direction: [south/north/east]
```

#### Eyes_Half（半闭）
```
Prompt:
[Base Prompt]

Face: ONLY eyes, all else transparent
Eyes:
- Half-closed, sleepy/tired
- Red glow slightly dimmed
- Relaxed eyebrows
- Calm or exhausted look

Direction: [south/north/east]
```

#### Eyes_Closed（闭眼）
```
Prompt:
[Base Prompt]

Face: ONLY eyes, all else transparent
Eyes:
- Completely closed
- Peaceful expression
- Thin closed eyelids
- For blinking animation

Direction: [south/north/east]
```

#### Eyes_Happy（开心）
```
Prompt:
[Base Prompt]

Face: ONLY eyes, all else transparent
Eyes:
- Gentle squinted shape (anime happy eyes)
- Red glow softened
- Slightly curved eyebrows
- Warm and pleased expression
- Small creases at corners

Direction: [south/north/east]
```

#### Eyes_Angry（愤怒）
```
Prompt:
[Base Prompt]

Face: ONLY eyes, all else transparent
Eyes:
- Wide open, intense glare
- Red glow VERY bright and fierce
- Sharply angled eyebrows (downward)
- Intimidating warrior rage
- Visible veins (optional, subtle)

Direction: [south/north/east]
```

#### Eyes_Sad（悲伤）
```
Prompt:
[Base Prompt]

Face: ONLY eyes, all else transparent
Eyes:
- Slightly downturned shape
- Red glow dimmed and dull
- Upward curved eyebrows (sad)
- Melancholic and lonely
- Small tears at corner (optional)

Direction: [south/north/east]
```

#### Eyes_Pain（痛苦）
```
Prompt:
[Base Prompt]

Face: ONLY eyes, all else transparent
Eyes:
- Tightly squinted/grimacing
- Red glow flickering (drawn as partial)
- Eyebrows pinched together
- Painful and suffering expression
- Strain lines around eyes

Direction: [south/north/east]
```

### 表情9-15：嘴巴变体

#### Mouth_Neutral（中性）
```
Prompt:
[Base Prompt]

Face: ONLY mouth area, all else transparent
Mouth:
- Small closed line
- Neutral expression
- No smile or frown
- Calm and composed
- Thin lips

Direction: [south/north/east]
```

#### Mouth_Smile（微笑）
```
Prompt:
[Base Prompt]

Face: ONLY mouth area, all else transparent
Mouth:
- Gentle upward curve
- Slight smile, not wide
- Elegant and reserved
- Closed lips
- Warm but controlled

Direction: [south/north/east]
```

#### Mouth_Frown（皱眉）
```
Prompt:
[Base Prompt]

Face: ONLY mouth area, all else transparent
Mouth:
- Downward curve
- Displeasure or concern
- Lips slightly pursed
- Stern warrior disapproval

Direction: [south/north/east]
```

#### Mouth_Talk1（说话1）
```
Prompt:
[Base Prompt]

Face: ONLY mouth area, all else transparent
Mouth:
- Slightly open (2-3px)
- Oval shape
- Speaking position frame 1
- Casual conversation

Direction: [south/north/east]
```

#### Mouth_Talk2（说话2）
```
Prompt:
[Base Prompt]

Face: ONLY mouth area, all else transparent
Mouth:
- More open than Talk1
- Wider oval
- Speaking position frame 2
- Mid-word articulation

Direction: [south/north/east]
```

#### Mouth_Talk3（说话3）
```
Prompt:
[Base Prompt]

Face: ONLY mouth area, all else transparent
Mouth:
- Open about 5-6px
- Rounded shape
- Speaking position frame 3
- Vowel pronunciation

Direction: [south/north/east]
```

#### Mouth_Open（张大）
```
Prompt:
[Base Prompt]

Face: ONLY mouth area, all else transparent
Mouth:
- Wide open (surprise or shout)
- Oval to circular shape
- Visible teeth (optional, anime style)
- Shocked or yelling expression
- Can show faint inner mouth shadow

Direction: [south/north/east]
```

---

## ?? AI生成工作流程

### 推荐工具

1. **Stable Diffusion** (本地/在线)
   - ControlNet for pose consistency
   - Inpainting for precise edits

2. **Midjourney** (快速原型)
   - 高质量输出
   - 需要手动分离图层

3. **NovelAI** (动漫风格)
   - 专精anime风格
   - 一致性好

### 生成步骤

#### 阶段1：生成完整角色（参考用）

```
1. 使用你的512x512三视图作为参考
2. 生成一组标准表情（正常脸）
3. 确认风格一致性
```

#### 阶段2：生成Base层

```
1. 使用inpainting移除眼睛和嘴巴
2. 确保留下的区域透明
3. 保存为Base_[方向].png
```

#### 阶段3：生成眼睛层

```
1. 只生成眼睛部分
2. 使用mask确保其他区域透明
3. 生成所有7种眼睛表情
4. 保存为Eyes_[类型]_[方向].png
```

#### 阶段4：生成嘴巴层

```
1. 只生成嘴巴部分
2. 使用mask确保其他区域透明
3. 生成所有7种嘴巴表情
4. 保存为Mouth_[类型]_[方向].png
```

#### 阶段5：后期处理

```
1. Photoshop/GIMP批量处理：
   - 缩放到128x128
   - 确认透明通道正确
   - 对齐检查（叠加测试）

2. 测试合成：
   Base + Eyes_Open + Mouth_Neutral
   应该看起来像完整的脸
```

---

## ?? 位置对齐指南

### 眼睛位置参考

```
在128x128画布上：

South (背面):
- X: 64 (居中)
- Y: 75 (从顶部向下)

North (正面):
- X: 64
- Y: 70

East (侧面):
- X: 70 (略向右)
- Y: 72
```

### 嘴巴位置参考

```
在128x128画布上：

South (背面):
- X: 64
- Y: 55

North (正面):
- X: 64
- Y: 52

East (侧面):
- X: 70
- Y: 53
```

### 对齐测试方法

```
Photoshop/GIMP:
1. 打开Base层
2. 创建新图层放Eyes
3. 创建新图层放Mouth
4. 调整到正确位置
5. 记录偏移值
6. 批量应用所有文件
```

---

## ?? 测试和验证

### 本地测试（Photoshop）

```
1. 创建128x128画布
2. 图层顺序（从下到上）：
   - Base
   - Mouth_Neutral
   - Eyes_Open
3. 检查：
   - 是否有空白或重叠
   - 透明度是否正确
   - 整体是否像一张脸
```

### 游戏内测试

```
1. 放入贴图文件夹
2. 启动RimWorld + Facial Animation
3. 生成血龙种角色
4. 观察：
   - 眨眼是否流畅
   - 说话时嘴巴是否动
   - 表情切换是否自然
```

---

## ?? 制作检查清单

### Base层（3张）
- [ ] Base_south.png
- [ ] Base_north.png
- [ ] Base_east.png

### 眼睛层（21张）
- [ ] Eyes_Open × 3
- [ ] Eyes_Half × 3
- [ ] Eyes_Closed × 3
- [ ] Eyes_Happy × 3
- [ ] Eyes_Angry × 3
- [ ] Eyes_Sad × 3
- [ ] Eyes_Pain × 3

### 嘴巴层（21张）
- [ ] Mouth_Neutral × 3
- [ ] Mouth_Smile × 3
- [ ] Mouth_Frown × 3
- [ ] Mouth_Talk1 × 3
- [ ] Mouth_Talk2 × 3
- [ ] Mouth_Talk3 × 3
- [ ] Mouth_Open × 3

### 质量检查
- [ ] 所有文件128x128
- [ ] PNG格式，带Alpha透明
- [ ] 透明区域正确
- [ ] 位置对齐一致
- [ ] 风格统一
- [ ] 文件命名正确

---

## ?? 高级技巧

### 技巧1：使用参考网格

```
创建一个128x128的参考模板：
- 标记眼睛中心点
- 标记嘴巴中心点
- 绘制引导线
- 所有表情使用此模板
```

### 技巧2：批量处理脚本

```python
# Photoshop脚本示例
# 批量调整所有Eyes文件的位置

import os
for file in eyes_files:
    open(file)
    move_layer_to(x=64, y=75)
    save()
    close()
```

### 技巧3：渐进式测试

```
顺序：
1. 先完成Base + Eyes_Open + Mouth_Neutral
2. 测试基础显示
3. 再添加Eyes_Closed
4. 测试眨眼
5. 再添加Mouth_Talk系列
6. 测试说话
7. 最后添加所有情绪表情
```

---

## ?? 参考资源

### Facial Animation文档
- Workshop: [#1635901197](https://steamcommunity.com/sharedfiles/filedetails/?id=1635901197)
- Discord: RimWorld Modding #facial-animation

### 参考Mod
- **推荐参考**：查看其他使用FA的种族mod
- **学习对象**：Kurin, Moyo, Racc

### 工具
- Photoshop/GIMP - 图像编辑
- ImageMagick - 批量处理
- 在线对齐工具 - sprite alignment checkers

---

## ?? 快速开始

### 最小可行版本（MVP）

如果你想先快速测试，可以只做：

```
必需文件（9张）：
? Base × 3
? Eyes_Open × 3
? Mouth_Neutral × 3

这样就能看到基础的静态脸
```

### 基础动画版本

```
必需文件（15张）：
? MVP的9张
? Eyes_Closed × 3 (眨眼)
? Mouth_Talk1 × 3 (说话)

这样就有眨眼和说话动画
```

### 完整版本

```
所有45张文件
完整的表情系统
```

---

## ?? 预估工作量

| 任务 | 时间估计 |
|------|---------|
| **学习AI工具** | 2-4小时 |
| **生成Base层** | 1小时 |
| **生成眼睛层** | 3-5小时 |
| **生成嘴巴层** | 3-5小时 |
| **后期处理** | 2-3小时 |
| **测试调整** | 2-3小时 |
| **总计** | **13-23小时** |

### 分阶段计划

**第1天**：
- 学习工具
- 生成Base + Eyes_Open + Mouth_Neutral
- 测试显示

**第2天**：
- 生成所有眼睛表情
- 测试眨眼和情绪

**第3天**：
- 生成所有嘴巴表情
- 测试说话动画

**第4天**：
- 微调和优化
- 最终测试

---

## ?? 下一步行动

1. ? **阅读本指南**
2. ? **选择AI工具并学习**
3. ? **生成第一组测试图（MVP）**
4. ? **游戏内测试**
5. ? **生成完整表情集**
6. ? **最终调优**

---

**准备好开始了吗？告诉我你选择哪个AI工具，我可以提供更详细的操作步骤！** ???
