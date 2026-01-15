# Cthulhu Defs 文件夹

存放克苏鲁娘的所有 Def 定义文件。

## ?? 文件说明

### NarratorPersonaDefs_Cthulhu.xml
克苏鲁娘人格定义，包含：
- 基础信息（defName, label, narratorName）
- 立绘路径（portraitPath）
- 克系主题配置
- **降临系统配置**（descentPawnKind, companionPawnKind）

### HediffDefs_Cthulhu.xml
疯狂机制 Hediff 定义：
- `Cthulhu_Madness`: 克苏鲁疯狂状态
- 阶段性效果（轻度、中度、重度疯狂）
- 能力削弱（意识、操控）
- 心情影响（-15 心情）

### ThingDefs_Cthulhu.xml
克系生物实体定义：
- 触手怪（Cthulhu_Tentacle）
- 深潜者（Cthulhu_DeepOne）
- 星之眷族（Cthulhu_Spawn）

### PawnKindDefs_Cthulhu.xml
克系生物种类定义：
- 战斗力配置
- 装备和能力
- 生成规则

### PawnKindDefs_Cthulhu_Descent.xml
降临实体定义：
- 克苏鲁娘降临形态
- 默认派系（玩家/敌对）
- 伴随召唤物

## ?? 配置示例

### 人格定义（NarratorPersonaDefs_Cthulhu.xml）

```xml
<TheSecondSeat.PersonaGeneration.NarratorPersonaDef>
  <defName>Cthulhu</defName>
  <label>Cthulhu</label>
  <narratorName>克苏鲁娘</narratorName>
  
  <!-- 降临系统 -->
  <descentPawnKind>Cthulhu_Descent</descentPawnKind>
  <descentPosturePath>descent_pose</descentPosturePath>
  <descentEffectPath>effect</descentEffectPath>
  <companionPawnKind>Cthulhu_Tentacle</companionPawnKind>
</TheSecondSeat.PersonaGeneration.NarratorPersonaDef>
```

### 疯狂机制（HediffDefs_Cthulhu.xml）

```xml
<HediffDef>
  <defName>Cthulhu_Madness</defName>
  <label>克苏鲁疯狂</label>
  <description>精神受到克苏鲁实体的污染...</description>
  <stages>
    <li>
      <minSeverity>0</minSeverity>
      <capMods>
        <li>
          <capacity>Consciousness</capacity>
          <offset>-0.1</offset>
        </li>
      </capMods>
      <mentalBreakMtbDays>10</mentalBreakMtbDays>
    </li>
  </stages>
</HediffDef>
```

### 召唤物（PawnKindDefs_Cthulhu.xml）

```xml
<PawnKindDef>
  <defName>Cthulhu_Tentacle</defName>
  <label>触手怪</label>
  <race>Cthulhu_Tentacle_Race</race>
  <combatPower>150</combatPower>
  <defaultFactionType>PlayerColony</defaultFactionType>
</PawnKindDef>
```

## ?? 注意事项

1. **defName** 必须以 `Cthulhu_` 开头，避免冲突
2. **疯狂机制** 的严重度（severity）范围 0-1
3. **召唤物** 必须定义对应的 ThingDef（race）
4. XML 必须使用 UTF-8 编码（无 BOM）

## ?? 相关文件

- 语言文件：`Languages/`
- 纹理资源：`Textures/`（主 Mod 共享）

## ?? 游戏内测试

1. 开启开发模式（`F12`）
2. 搜索 Debug Actions → `Apply Hediff`
3. 选择 `Cthulhu_Madness` 测试疯狂效果
