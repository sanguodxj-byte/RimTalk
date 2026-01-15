# Cthulhu 语言文件

克苏鲁娘人格的多语言翻译。

## ?? 文件结构

```
Languages/
├── ChineseSimplified/
│   └── Keyed/
│       └── Cthulhu_Keys.xml
└── English/
    └── Keyed/
        └── Cthulhu_Keys.xml
```

## ?? 翻译键

### 人格信息
- `Cthulhu_Name` - 人格名称
- `Cthulhu_Label` - 标签
- `Cthulhu_Biography` - 简介

### 疯狂机制
- `Cthulhu_Madness_Label` - 疯狂状态名称
- `Cthulhu_Madness_Description` - 疯狂状态描述
- `Cthulhu_Madness_Stage_Mild` - 轻度疯狂
- `Cthulhu_Madness_Stage_Moderate` - 中度疯狂
- `Cthulhu_Madness_Stage_Severe` - 重度疯狂

### 召唤物
- `Cthulhu_Tentacle_Label` - 触手怪名称
- `Cthulhu_Tentacle_Description` - 触手怪描述
- `Cthulhu_DeepOne_Label` - 深潜者名称
- `Cthulhu_Spawn_Label` - 星之眷族名称

### 降临系统
- `Cthulhu_Descent_Assist_Letter_Label` - 援助降临信件标题
- `Cthulhu_Descent_Assist_Letter_Text` - 援助降临信件内容
- `Cthulhu_Descent_Attack_Letter_Label` - 袭击降临信件标题
- `Cthulhu_Descent_Attack_Letter_Text` - 袭击降临信件内容

## ?? XML 格式

```xml
<?xml version="1.0" encoding="utf-8"?>
<LanguageData>
  <!-- 人格信息 -->
  <Cthulhu_Name>克苏鲁娘</Cthulhu_Name>
  <Cthulhu_Label>深海的呼唤</Cthulhu_Label>
  <Cthulhu_Biography>
    来自深海城市拉莱耶的古老存在，
    她的存在本身就是对理智的挑战...
  </Cthulhu_Biography>
  
  <!-- 疯狂机制 -->
  <Cthulhu_Madness_Label>克苏鲁疯狂</Cthulhu_Madness_Label>
  <Cthulhu_Madness_Description>
    精神受到克苏鲁实体的污染，理智逐渐崩溃。
    患者会出现幻觉、妄想、无法控制的恐惧。
  </Cthulhu_Madness_Description>
  <Cthulhu_Madness_Stage_Mild>轻度疯狂（意识-10%）</Cthulhu_Madness_Stage_Mild>
  <Cthulhu_Madness_Stage_Moderate>中度疯狂（意识-20%）</Cthulhu_Madness_Stage_Moderate>
  <Cthulhu_Madness_Stage_Severe>重度疯狂（意识-30%）</Cthulhu_Madness_Stage_Severe>
  
  <!-- 召唤物 -->
  <Cthulhu_Tentacle_Label>触手怪</Cthulhu_Tentacle_Label>
  <Cthulhu_Tentacle_Description>
    克苏鲁的触手化身，拥有强大的近战能力。
  </Cthulhu_Tentacle_Description>
  
  <Cthulhu_DeepOne_Label>深潜者</Cthulhu_DeepOne_Label>
  <Cthulhu_DeepOne_Description>
    水陆两栖的克系仆从，可在水中和陆地作战。
  </Cthulhu_DeepOne_Description>
  
  <Cthulhu_Spawn_Label>星之眷族</Cthulhu_Spawn_Label>
  <Cthulhu_Spawn_Description>
    克苏鲁的空中仆从，能够飞行并释放精神冲击。
  </Cthulhu_Spawn_Description>
  
  <!-- 降临系统 -->
  <Cthulhu_Descent_Assist_Letter_Label>克苏鲁娘降临（援助）</Cthulhu_Descent_Assist_Letter_Label>
  <Cthulhu_Descent_Assist_Letter_Text>
    深海的古老之主听到了你的呼唤，
    克苏鲁娘从拉莱耶城降临，带来混沌的力量！
  </Cthulhu_Descent_Assist_Letter_Text>
  
  <Cthulhu_Descent_Attack_Letter_Label>克苏鲁娘降临（袭击）</Cthulhu_Descent_Attack_Letter_Label>
  <Cthulhu_Descent_Attack_Letter_Text>
    "Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn"
    
    克苏鲁娘率领深海眷族，带着疯狂与混沌而来！
  </Cthulhu_Descent_Attack_Letter_Text>
</LanguageData>
```

## ?? 注意事项

1. **编码**: 必须使用 UTF-8 编码（无 BOM）
2. **键名**: 必须与 Defs 中的引用一致
3. **克苏鲁主题**: 文本应体现深海、疯狂、古老的氛围
4. **咒语翻译**: 克苏鲁咒语（R'lyehian）可保留原文

## ?? 支持语言

| 语言 | 文件夹 | 完成度 |
|------|--------|--------|
| 简体中文 | `ChineseSimplified` | ? 100% |
| 英文 | `English` | ? 100% |

## ?? 相关文件

- **人格定义**: `Defs/NarratorPersonaDefs_Cthulhu.xml`
- **疯狂机制**: `Defs/HediffDefs_Cthulhu.xml`
- **召唤物**: `Defs/ThingDefs_Cthulhu.xml`
