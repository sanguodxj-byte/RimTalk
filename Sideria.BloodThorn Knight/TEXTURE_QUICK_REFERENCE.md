# ?? RimWorld贴图快速参考

## 标准尺寸
| 贴图类型 | 推荐尺寸 | 格式 |
|---------|---------|------|
| 身体 | 128x128 | PNG (RGBA) |
| 头部 | 128x128 | PNG (RGBA) |
| 附加部件 | 64x64 或 128x128 | PNG (RGBA) |
| 武器 | 64x64 或 128x128 | PNG (RGBA) |

## 命名规范
```
Naked_[RaceName]_south.png   # 背面
Naked_[RaceName]_north.png   # 正面  
Naked_[RaceName]_east.png    # 侧面（会自动镜像为west）
```

## 视角要求
- **角度**：45度俯视
- **投影**：正交（非透视）
- **朝向**：
  - South = 背对屏幕（向下走）
  - North = 面向屏幕（向上走）
  - East = 侧面向右

## 风格指南
- ? 半写实、低多边形
- ? 简洁线条和阴影
- ? 适度细节
- ? 避免过度动漫化
- ? 避免Q版比例（除非整体风格统一）

## 您的三视图状态

### ? 正确的地方
- 透明背景
- 三个视角完整
- 角色特征清晰

### ?? 需要调整
- 视角：当前~90度 → 需要45度
- 风格：Q版/动漫 → 建议更写实
- 比例：头身比1:2 → 建议1:3或1:4

## 快速修复步骤

### 1. 调整视角（Photoshop）
```
编辑 > 变换 > 透视
向下拉底部边缘，创造倾斜效果
```

### 2. 调整画布
```
图像 > 画布大小 > 128x128像素
居中对齐角色
```

### 3. 导出设置
```
文件 > 导出为 > PNG
勾选：透明度
取消：隔行扫描
```

### 4. 分层保存
```
Body层 → Naked_Dracovampir_[方向].png
Head层 → Head_Dracovampir_[方向].png  
Tail层 → Tail_[方向].png
Horn层 → Horn_[方向].png
```

## XML配置示例

```xml
<!-- 身体贴图 -->
<graphicData>
  <texPath>Things/Pawn/Humanlike/Bodies/Naked_Dracovampir</texPath>
  <graphicClass>Graphic_Multi</graphicClass>
  <drawSize>1.0</drawSize>
</graphicData>

<!-- 尾巴附加 -->
<bodyAddons>
  <li>
    <path>Things/Pawn/Humanlike/Addons/Tail</path>
    <bodyPart>Tail</bodyPart>
    <drawSize>1.2</drawSize>
  </li>
</bodyAddons>
```

## 测试检查清单
- [ ] 贴图显示正常
- [ ] 没有白边或透明问题
- [ ] 角色大小合适
- [ ] 与其他角色风格协调
- [ ] 各方向都能正确显示

## 常见问题

**Q: 角色显示太大/太小？**
A: 调整XML中的 `<drawSize>` 值

**Q: 位置偏移不对？**  
A: 使用 `<offset>(x, y, z)</offset>` 调整

**Q: 贴图有白边？**
A: 重新导出，确保透明通道正确

**Q: 风格与游戏不搭？**
A: 考虑重绘或调整到更写实风格

---

**需要详细信息？** 查看 `TEXTURE_FORMAT_ANALYSIS.md`
