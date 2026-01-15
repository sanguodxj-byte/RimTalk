# Sideria 降临特效

Sideria 人格的降临特效资源。

## ?? 文件放置

```
Textures/Narrators/Descent/Effects/Sideria/
├── effect_assist.png  ← 援助特效（友好降临）
└── effect_attack.png  ← 袭击特效（敌对降临）
```

## ?? Sideria 特效风格

### 援助特效（effect_assist.png）
- **主题**: 星界守护、电子网络
- **色调**: 蓝白色调 + 金色点缀
- **元素**: 
  - 六角形网格
  - 数字流
  - 星光闪烁
  - 半透明光环

### 袭击特效（effect_attack.png）
- **主题**: 故障艺术、数字攻击
- **色调**: 红紫色调 + 黑色阴影
- **元素**:
  - 破碎的像素
  - 故障线条
  - 错位扭曲
  - 暗红色能量波

## ?? 制作参考

### 援助特效灵感
- 科幻电影中的全息投影
- 赛博朋克游戏的 UI 特效
- 星空元素（渐变透明）

### 袭击特效灵感
- Glitch Art（故障艺术）
- 数字病毒感染特效
- 黑客入侵视觉效果

## ?? Def 配置

在 `Defs/NarratorPersonaDefs_Sideria.xml` 中：

```xml
<descentEffectPath>effect</descentEffectPath>
```

系统会自动添加 `_assist` 或 `_attack` 后缀。

## ?? 规格标准

- **尺寸**: 1024x2048px
- **格式**: PNG-24（RGBA）
- **文件大小**: < 2MB（建议压缩）
- **命名**: `effect_assist.png`, `effect_attack.png`

## ? 质量检查

- [ ] 透明通道正确（边缘渐变）
- [ ] 不遮挡脸部关键特征
- [ ] 颜色与 Sideria 主题一致
- [ ] 尺寸 1024x2048px
- [ ] 文件名正确
