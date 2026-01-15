# Sideria 分层立绘资源

存放 Sideria 的分层立绘文件（1024x2048px）。

## ?? 分层系统

立绘由多个图层组成，从下到上绘制：

```
Layer 1: base_body.png      ← 身体基础层（必需）
Layer 2: mouth_*.png        ← 嘴巴层（张嘴动画）
Layer 3: eyes_*.png         ← 眼睛层（眨眼动画）
Layer 4: flush_*.png        ← 腮红层（害羞/愤怒表情）
```

## ?? 文件结构

```
Textures/Narrators/Layered/Sideria/
├── base_body.png           ← 身体基础层（必需）
├── eyes_neutral.png        ← 睁眼状态
├── eyes_closed.png         ← 闭眼状态（眨眼）
├── eyes_half.png           ← 半闭眼（可选）
├── mouth_closed.png        ← 闭嘴状态
├── mouth_open.png          ← 张嘴状态（说话/TTS）
├── flush_shy.png           ← 害羞腮红
└── flush_angry.png         ← 愤怒腮红
```

## ?? 规格要求

- **尺寸**: 1024x2048px（9:16 竖版）
- **格式**: PNG，RGBA
- **透明通道**: **除 base_body 外，其他层必须透明**
- **对齐**: 所有图层像素完美对齐

## ?? 图层说明

### 基础身体层（base_body.png）
- **内容**: 完整的身体、服装、头发
- **透明**: 不需要透明通道（可以有背景）
- **用途**: 作为底层，所有其他层叠加在上面

### 眼睛层
| 文件名 | 用途 | 透明要求 |
|--------|------|---------|
| `eyes_neutral.png` | 默认睁眼 | ? 仅眼睛部分 |
| `eyes_closed.png` | 眨眼动画 | ? 仅眼睛部分 |
| `eyes_half.png` | 半闭眼（可选） | ? 仅眼睛部分 |

**眨眼动画**:
- 每隔 3-7 秒自动眨眼
- 闭眼持续 0.15 秒
- 自动切换 `neutral` ? `closed`

### 嘴巴层
| 文件名 | 用途 | 透明要求 |
|--------|------|---------|
| `mouth_closed.png` | 默认闭嘴 | ? 仅嘴巴部分 |
| `mouth_open.png` | 说话/TTS | ? 仅嘴巴部分 |

**张嘴动画**:
- TTS 播放时自动张嘴
- 根据口型编码（Viseme）动态调整
- TTS 结束后自动闭嘴

### 腮红层（可选）
| 文件名 | 用途 | 触发条件 |
|--------|------|---------|
| `flush_shy.png` | 害羞腮红 | 表情为 Shy |
| `flush_angry.png` | 愤怒脸红 | 表情为 Angry |

## ?? 制作建议

### 眼睛层
1. 在 Photoshop 中打开 `base_body.png`
2. 复制图层，删除除眼睛外的所有部分
3. 保存为 `eyes_neutral.png`（PNG-24，透明）
4. 重复步骤制作 `eyes_closed.png`（闭眼）

### 嘴巴层
1. 在 Photoshop 中打开 `base_body.png`
2. 复制图层，删除除嘴巴外的所有部分
3. 保存为 `mouth_closed.png`（PNG-24，透明）
4. 调整嘴巴形状为张开状态
5. 保存为 `mouth_open.png`（PNG-24，透明）

### 对齐检查
- 将所有图层叠加，应与原始立绘完全一致
- 使用 Photoshop 的"链接图层"功能确保对齐
- 导出时勾选"快速导出为 PNG"（保留透明）

## ?? 常见错误

| 错误 | 后果 | 解决方法 |
|------|------|---------|
| 图层未对齐 | 眼睛/嘴巴位置偏移 | 使用相同画布尺寸导出 |
| 背景不透明 | 遮挡下层 | 删除背景层，保存为 PNG-24 |
| 尺寸不一致 | 显示错误 | 统一使用 1024x2048px |
| 缺少 base_body | 无法显示立绘 | 必须有基础层 |

## ?? 相关系统

- **动画系统**: `BlinkAnimationSystem.cs`, `MouthAnimationSystem.cs`
- **合成器**: `LayeredPortraitCompositor.cs`
- **表情系统**: `ExpressionSystem.cs`

## ?? 快速测试

游戏内按 `F12` 开启开发模式，立绘会显示当前图层信息。
