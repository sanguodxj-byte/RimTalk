# 人格标签系统使用指南 (Personality Tag System Guide)

The Second Seat 引入了全新的人格标签系统，允许玩家和 Modder 自由定义和扩展 AI 叙事者的人格特征。

## 1. 什么是人格标签？

人格标签（Personality Tags）是定义 AI 性格特征的关键词。例如：
- `Yandere` (病娇)
- `Tsundere` (傲娇)
- `Kuudere` (冷娇)
- `Doting` (溺爱)
- `Gentle` (温柔)

当 AI 拥有某个标签时，它会加载对应的行为指令，从而改变其对话风格和行为模式。

## 2. 如何使用（针对不同用户）

### 2.1 轻度用户：游戏内编辑

你可以在游戏内直接修改当前叙事者的人格标签，无需重启游戏即可生效。

1.  打开 **The Second Seat 设置** 或点击主界面上的叙事者图标。
2.  选择 **"编辑人格卡片"**。
3.  在 **"个性标签"** 区域：
    *   查看当前生效的标签。
    *   点击标签旁的 **X** 删除标签。
    *   在输入框中输入新标签（如 `Kuudere`），点击 **"添加"**。
4.  点击底部的 **"保存"**。
    *   系统会提示已保存，并自动更新当前运行时的 AI 人格。

### 2.2 高级用户：自定义标签行为

你可以为任意标签创建自定义的行为指令文件。

1.  进入 Mod 配置文件夹：
    *   路径通常为：`RimWorld/Config/TheSecondSeat/Prompts/ChineseSimplified/` (或你使用的语言)
    *   你也可以在游戏内打开 **"提示词管理"** 界面，点击 **"打开文件夹"**。
2.  创建一个新的文本文件，命名格式为 `Relationship_{Tag}.txt`。
    *   例如：`Relationship_Kuudere.txt`
3.  在文件中编写你希望 AI 遵循的指令。
    *   示例内容：
        ```text
        [Kuudere Mode]
        你现在是冷娇性格。
        平时保持冷淡、少言寡语，但在关键时刻（或好感度高时）会流露出深沉的关心。
        不要使用过多的表情符号。
        ```
4.  保存文件。
5.  在游戏内给叙事者添加对应的标签（如 `Kuudere`）。
6.  AI 现在会读取并执行你编写的指令。

### 2.3 Modder：XML 定义

你可以在 `NarratorPersonaDef` 中预设标签。

```xml
<NarratorPersonaDef>
    <defName>MyCustomNarrator</defName>
    <narratorName>Sideria</narratorName>
    <personalityTags>
        <li>Kuudere</li>
        <li>Protective</li>
    </personalityTags>
    <!-- 其他配置 -->
</NarratorPersonaDef>
```

## 3. 内置标签与文件

系统默认支持以下标签（对应文件已内置）：
- `Yandere` -> `Relationship_Yandere.txt`
- `Tsundere` -> `Relationship_Tsundere.txt`
- `Kuudere` -> `Relationship_Kuudere.txt`
- `Doting` -> `Relationship_Doting.txt`
- `Gentle` -> `Relationship_Gentle.txt`

你可以随时添加新的标签，只要创建一个对应的 `.txt` 文件即可。

## 4. 常见问题

**Q: 修改标签后需要重启游戏吗？**
A: 不需要。通过游戏内 "编辑人格卡片" 修改并保存后，更改会立即应用到当前游戏。

**Q: 我添加了一个标签，但没有对应的文件，会发生什么？**
A: AI 会拥有这个标签，但如果没有对应的 `Relationship_{Tag}.txt` 文件，它不会加载额外的指令。不过，AI 的底层模型可能仍然理解该标签的字面含义（例如 "Shy"），并尝试表现出来。

**Q: 支持中文标签吗？**
A: 支持。你可以使用 `Relationship_病娇.txt`，然后在游戏中添加 "病娇" 标签。
