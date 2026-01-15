# Sideria 子 Mod

Sideria 人格资源包，包含完整的立绘、降临系统和语言文件。

## ?? 文件夹结构

```
The Second Seat - Sideria/
├── About/              # Mod 元数据
├── Defs/               # 定义文件（PawnKindDef, PersonaDef）
├── Languages/          # 多语言翻译
├── Textures/           # 纹理资源
│   ├── DifficultyMode/ # 难度模式图标
│   └── Narrators/      # 叙事者资源
│       ├── Avatars/    # 头像（64x64）
│       ├── Descent/    # 降临动画资源
│       ├── Expressions/# 表情差分
│       ├── Layered/    # 分层立绘
│       └── Outfits/    # 服装差分
└── LoadFolders.xml     # 版本加载配置
```

## ?? 资源规范

### 头像（Avatars）
- **路径**: `Textures/Narrators/Avatars/Sideria/`
- **格式**: PNG，64x64px
- **命名**: `{表情名}.png`

### 立绘（Layered）
- **路径**: `Textures/Narrators/Layered/Sideria/`
- **格式**: PNG，1024x2048px
- **命名**: `base_body.png`, `eyes_neutral.png`, 等

### 降临动画（Descent）
- **路径**: `Textures/Narrators/Descent/Postures/Sideria/`
- **格式**: PNG，1024x2048px
- **命名**: `descent_pose.png`, `effect_assist.png`, `effect_attack.png`

## ?? 配置

在 `Defs/NarratorPersonaDefs_Sideria.xml` 中配置：

```xml
<descentPawnKind>Sideria_Descent</descentPawnKind>
<descentPosturePath>descent_pose</descentPosturePath>
<descentEffectPath>effect</descentEffectPath>
```

## ?? 语言支持

- 中文简体：`Languages/ChineseSimplified/Keyed/`
- 英文：`Languages/English/Keyed/`

## ?? 依赖

- **主 Mod**: The Second Seat (必需)
- **加载顺序**: 必须在主 Mod 之后加载

## ?? 安装

1. 解压到 `RimWorld/Mods/`
2. 在 Mod 列表中启用
3. 确保加载顺序正确
