# Cthulhu 子 Mod

克苏鲁娘人格资源包，包含完整的降临系统、疯狂机制和召唤物。

## ?? 文件夹结构

```
The Second Seat - Cthulhu/
├── About/              # Mod 元数据
├── Defs/               # 定义文件（HediffDef, ThingDef, PawnKindDef, PersonaDef）
├── Languages/          # 多语言翻译
└── Textures/           # 纹理资源（立绘、降临动画）
```

## ?? 独特系统

### 疯狂机制（HediffDefs_Cthulhu.xml）
- **Cthulhu_Madness**: 克苏鲁疯狂状态
- **精神污染**: 接触克苏鲁实体会积累疯狂值
- **疯狂效果**: 降低意识、影响心情、随机精神崩溃

### 召唤物（ThingDefs_Cthulhu.xml + PawnKindDefs_Cthulhu.xml）
- **触手怪（Cthulhu_Tentacle）**: 近战攻击单位
- **深潜者（Cthulhu_DeepOne）**: 水陆两栖战士
- **星之眷族（Cthulhu_Spawn）**: 飞行单位

### 降临系统（PawnKindDefs_Cthulhu_Descent.xml）
- **友好模式**: 克苏鲁娘作为强力盟友降临
- **敌对模式**: 克苏鲁娘率领克系生物袭击殖民地

## ?? Def 配置

在 `Defs/NarratorPersonaDefs_Cthulhu.xml` 中：

```xml
<descentPawnKind>Cthulhu_Descent</descentPawnKind>
<descentPosturePath>descent_pose</descentPosturePath>
<descentEffectPath>effect</descentEffectPath>
<companionPawnKind>Cthulhu_Tentacle</companionPawnKind>
```

**新增字段**:
- `companionPawnKind`: 降临时伴随的召唤物

## ?? 语言支持

- 中文简体：`Languages/ChineseSimplified/Keyed/Cthulhu_Keys.xml`
- 英文：`Languages/English/Keyed/Cthulhu_Keys.xml`

### 翻译键示例
```xml
<Cthulhu_Madness_Label>克苏鲁疯狂</Cthulhu_Madness_Label>
<Cthulhu_Madness_Description>
  精神受到克苏鲁实体的污染，理智逐渐崩溃...
</Cthulhu_Madness_Description>
```

## ?? 依赖

- **主 Mod**: The Second Seat (必需)
- **加载顺序**: 必须在主 Mod 之后加载

## ?? 平衡性调整

### 疯狂机制强度
在 `HediffDefs_Cthulhu.xml` 中调整：

```xml
<stages>
  <li>
    <minSeverity>0</minSeverity>
    <capMods>
      <li>
        <capacity>Consciousness</capacity>
        <offset>-0.1</offset>  <!-- 降低意识 10% -->
      </li>
    </capMods>
  </li>
</stages>
```

### 召唤物能力
在 `PawnKindDefs_Cthulhu.xml` 中调整：

```xml
<combatPower>150</combatPower>  <!-- 战斗力 -->
<maxGenerationAge>1000</maxGenerationAge>
```

## ?? 安装

1. 解压到 `RimWorld/Mods/`
2. 在 Mod 列表中启用
3. 确保加载顺序：主 Mod → Cthulhu 子 Mod
4. 重启游戏

## ?? 相关系统

- **疯狂光环**: `CompSanityAura.cs`
- **边缘生成**: `CompSpawnerFromEdge.cs`
- **降临系统**: `NarratorDescentSystem.cs`
