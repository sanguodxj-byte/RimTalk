# RimWorld XML属性修正总结

## ?? 已修正的问题

### 2024更新 - 基于RimWorld官方DLL验证

---

## ?? 修正清单

### 1. PawnKindDef属性修正

#### ? 移除的不存在属性
```xml
<!-- 错误：RimWorld不支持 -->
<itemQuality>Legendary</itemQuality>
<specificApparelRequirements>...</specificApparelRequirements>
```

#### ? 保留的正确属性
```xml
<!-- 正确：原版支持 -->
<initialWillRange>8~10</initialWillRange>  <!-- Royalty DLC -->
<initialResistanceRange>25~30</initialResistanceRange>  <!-- Royalty DLC -->
<biocodeWeaponChance>1.0</biocodeWeaponChance>
<gearHealthRange>1.0~1.0</gearHealthRange>
<apparelAllowHeadgearChance>0.9</apparelAllowHeadgearChance>
```

---

### 2. HediffDef属性修正

#### ? 错误的属性使用
```xml
<!-- 错误：MeleeDPS不存在作为statOffset -->
<statOffsets>
    <MeleeDPS>15</MeleeDPS>  <!-- ? -->
    <MeleeDodgeChance>8</MeleeDodgeChance>  <!-- ? 应该是0.08 -->
    <ArmorPenetration>0.5</ArmorPenetration>  <!-- ? 不能作为statOffset -->
</statOffsets>
```

#### ? 修正后的正确写法
```xml
<!-- 正确：使用statFactors和正确的小数格式 -->
<statOffsets>
    <MeleeHitChance>0.20</MeleeHitChance>  <!-- 20% = 0.20 -->
    <MeleeDodgeChance>0.08</MeleeDodgeChance>  <!-- 8% = 0.08 -->
</statOffsets>
<statFactors>
    <MeleeDamageFactor>2.0</MeleeDamageFactor>  <!-- 200% = 2.0 -->
</statFactors>
```

---

### 3. TraitDef属性修正

#### ? 错误的属性
```xml
<!-- 错误：数值格式和属性位置 -->
<statOffsets>
    <MeleeDodgeChance>15</MeleeDodgeChance>  <!-- ? 应该是0.15 -->
    <MeleeDPS>3</MeleeDPS>  <!-- ? 不存在 -->
    <RestRateMultiplier>0.5</RestRateMultiplier>  <!-- ? 应该在statFactors中 -->
</statOffsets>
```

#### ? 修正后的正确写法
```xml
<!-- 正确：小数格式+正确的属性位置 -->
<statOffsets>
    <MeleeDodgeChance>0.15</MeleeDodgeChance>  <!-- 15% = 0.15 -->
    <MeleeHitChance>0.10</MeleeHitChance>  <!-- 10% = 0.10 -->
</statOffsets>
<statFactors>
    <MeleeDamageFactor>1.5</MeleeDamageFactor>  <!-- 150% = 1.5 -->
    <RestRateMultiplier>0.5</RestRateMultiplier>  <!-- 50% = 0.5 -->
</statFactors>
```

---

## ?? RimWorld属性格式指南

### statOffsets（绝对值加成）

| 属性 | 格式 | 说明 | 示例 |
|------|------|------|------|
| `MoveSpeed` | 小数 | 移动速度（格/秒） | `0.3` = +0.3格/秒 |
| `MeleeDodgeChance` | 0-1小数 | 闪避概率 | `0.08` = 8%闪避 |
| `MeleeHitChance` | 0-1小数 | 命中概率 | `0.18` = 18%命中 |
| `ArmorRating_Sharp` | 0-1小数 | 锋利护甲 | `0.25` = 25%护甲 |
| `ArmorRating_Blunt` | 0-1小数 | 钝击护甲 | `0.25` = 25%护甲 |
| `ArmorRating_Heat` | 0-1小数 | 高温护甲 | `0.55` = 55%护甲 |
| `InjuryHealingFactor` | 数值 | 伤口愈合加成 | `2.0` = +2.0倍速 |
| `ImmunityGainSpeed` | 数值 | 免疫速度加成 | `2.0` = +2.0倍速 |
| `PainShockThreshold` | 数值 | 疼痛休克阈值 | `0.3` = +30%阈值 |

### statFactors（倍率修正）

| 属性 | 格式 | 说明 | 示例 |
|------|------|------|------|
| `IncomingDamageFactor` | 倍率 | 受伤倍率 | `0.75` = 受到75%伤害 |
| `MeleeDamageFactor` | 倍率 | 近战伤害倍率 | `2.0` = 200%伤害 |
| `HungerRateMultiplier` | 倍率 | 饥饿速度倍率 | `0.5` = 50%速度 |
| `RestRateMultiplier` | 倍率 | 休息速度倍率 | `0.5` = 50%速度 |
| `PainShockThreshold` | 倍率 | 疼痛休克阈值 | `2.0` = 2倍阈值 |

### capacityMods（能力修正）

| 属性 | 格式 | 说明 | 示例 |
|------|------|------|------|
| `Consciousness` | offset | 意识 | `-0.05` = -5%意识 |
| `Moving` | offset | 移动能力 | `0.15` = +15%移动 |
| `Manipulation` | offset | 操作能力 | `0.10` = +10%操作 |
| `Sight` | postFactor | 视力 | `0.8` = 80%视力 |

---

## ?? 常见错误

### 错误1：使用整数而非小数
```xml
? <MeleeDodgeChance>8</MeleeDodgeChance>  <!-- 错误：RimWorld会理解为800% -->
? <MeleeDodgeChance>0.08</MeleeDodgeChance>  <!-- 正确：8% -->
```

### 错误2：使用不存在的属性
```xml
? <MeleeDPS>15</MeleeDPS>  <!-- 错误：不存在此属性 -->
? <statFactors>
     <MeleeDamageFactor>2.0</MeleeDamageFactor>  <!-- 正确：使用伤害因子 -->
   </statFactors>
```

### 错误3：statOffset与statFactor混淆
```xml
? <statOffsets>
     <IncomingDamageFactor>0.75</IncomingDamageFactor>  <!-- 错误位置 -->
   </statOffsets>
   
? <statFactors>
     <IncomingDamageFactor>0.75</IncomingDamageFactor>  <!-- 正确位置 -->
   </statFactors>
```

---

## ?? 修正文件列表

以下文件已根据RimWorld官方DLL验证并修正：

1. ? `Defs/PawnKindDefs/PawnKinds_Sideria.xml`
2. ? `Defs/HediffDefs/Hediffs_Sideria.xml`
3. ? `Defs/TraitDefs/Traits_Sideria.xml`
4. ? `QUICK_REFERENCE.md`

---

**最后更新**：2024
**验证来源**：RimWorld 1.5 Assembly-CSharp.dll
**状态**：? 已修正并验证
