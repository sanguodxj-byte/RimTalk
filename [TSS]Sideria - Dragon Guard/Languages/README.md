# Sideria 语言文件

Sideria 人格的多语言翻译。

## ?? 文件结构

```
Languages/
├── ChineseSimplified/
│   └── Keyed/
│       └── Sideria_Keys.xml
└── English/
    └── Keyed/
        └── Sideria_Keys.xml
```

## ?? 翻译键

### 人格信息
- `Sideria_Name` - 人格名称
- `Sideria_Label` - 标签
- `Sideria_Biography` - 简介

### 降临系统
- `Sideria_Descent_Assist_Letter_Label` - 援助降临信件标题
- `Sideria_Descent_Assist_Letter_Text` - 援助降临信件内容
- `Sideria_Descent_Attack_Letter_Label` - 袭击降临信件标题
- `Sideria_Descent_Attack_Letter_Text` - 袭击降临信件内容

### UI 文本
- `Sideria_Portrait_Tooltip` - 立绘工具提示
- `Sideria_Interaction_Head` - 头部交互提示
- `Sideria_Interaction_Body` - 身体交互提示

## ?? XML 格式

```xml
<?xml version="1.0" encoding="utf-8"?>
<LanguageData>
  <Sideria_Name>希德利亚</Sideria_Name>
  <Sideria_Label>星界守护者</Sideria_Label>
  <Sideria_Biography>
    来自星界的神秘存在，拥有操控星光的能力...
  </Sideria_Biography>
  
  <!-- 降临系统 -->
  <Sideria_Descent_Assist_Letter_Label>希德利亚降临（援助）</Sideria_Descent_Assist_Letter_Label>
  <Sideria_Descent_Assist_Letter_Text>
    希德利亚响应你的呼唤，以星光之力降临到了这片土地！
  </Sideria_Descent_Assist_Letter_Text>
</LanguageData>
```

## ?? 注意事项

1. **编码**: 必须使用 UTF-8 编码（无 BOM）
2. **键名**: 必须与代码中的引用一致
3. **换行**: 长文本可以使用 `\n` 换行
4. **特殊字符**: XML 转义（`&lt;`, `&gt;`, `&amp;`）

## ?? 支持语言

| 语言 | 文件夹 | 完成度 |
|------|--------|--------|
| 简体中文 | `ChineseSimplified` | ? 100% |
| 英文 | `English` | ? 100% |

## ?? 相关文件

- **人格定义**: `Defs/NarratorPersonaDefs_Sideria.xml`
- **降临系统**: `Defs/PawnKindDefs_Sideria_Descent.xml`
