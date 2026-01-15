# Superb Recruitment (卓越招募)

A RimWorld mod that adds a dialogue-based visitor recruitment system.

## 功能特性 / Features

### 中文
- **对话式招募系统**: 通过与访客对话来提升说服值，而不是传统的囚禁招募
- **隐藏的说服值**: 玩家不知道访客的确切说服度，增加不确定性和挑战
- **多次尝试机会**: 
  - 玩家可以亲自说服访客3次
  - 每个殖民者可以尝试说服1次
- **社交技能影响**: 殖民者的社交技能会影响说服效果
- **RimTalk集成**: 如果安装了RimTalk模组，将使用完整的对话系统
- **简单对话窗口**: 如果没有RimTalk，使用内置的简单对话界面

### English
- **Dialogue-Based Recruitment**: Persuade visitors through conversations instead of traditional imprisonment
- **Hidden Persuasion Values**: Players don't know the exact persuasion level, adding uncertainty
- **Multiple Attempts**:
  - Player can personally persuade 3 times
  - Each colonist can attempt once
- **Social Skill Impact**: Colonists' social skills affect persuasion effectiveness
- **RimTalk Integration**: Full dialogue system if RimTalk is installed
- **Simple Dialogue Window**: Built-in simple interface if RimTalk is not available

## 安装 / Installation

1. 下载并解压到 RimWorld 的 Mods 文件夹
2. 在游戏中启用模组
3. (可选) 安装 RimTalk 模组以获得更丰富的对话体验

1. Download and extract to RimWorld's Mods folder
2. Enable the mod in-game
3. (Optional) Install RimTalk mod for richer dialogue experience

## 使用方法 / How to Use

### 中文
1. 当有访客来到你的殖民地时，选择他们
2. 点击"说服"按钮
3. 选择是自己说服还是让殖民者说服
4. 进行对话，尝试提升说服值
5. 当说服值足够高时，会出现"尝试招募"按钮
6. 点击招募按钮，有一定概率成功招募访客

### English
1. When a visitor arrives at your colony, select them
2. Click the "Persuade" button
3. Choose to persuade personally or send a colonist
4. Have a conversation to increase persuasion value
5. When persuasion is high enough, "Try to Recruit" button appears
6. Click recruit button for a chance to successfully recruit the visitor

## 游戏平衡 / Game Balance

- 初始说服值: 随机分布在 20%-80% 之间（正态分布）
- 成功率公式: persuasionValue^2.5
- 平均需要多次对话才能成功招募
- 失败招募会降低说服值
- 过度尝试可能导致访客生气离开

- Initial persuasion: Random between 20%-80% (normal distribution)
- Success formula: persuasionValue^2.5
- Multiple conversations usually needed for successful recruitment
- Failed recruitment attempts lower persuasion value
- Excessive attempts may cause visitor to leave angry

## 技术细节 / Technical Details

### 文件结构 / File Structure
```
Superb Recruitment/
├── About/
│   ├── About.xml
│   └── PublishedFileId.txt
├── Assemblies/
│   └── SuperbRecruitment.dll
├── Defs/
│   └── HediffDefs/
│       └── Hediffs_PersuasionTracking.xml
├── Languages/
│   ├── English/
│   │   └── Keyed/
│   │       └── SuperbRecruitment.xml
│   └── ChineseSimplified/
│       └── Keyed/
│           └── SuperbRecruitment.xml
├── RimTalk_Integration/
│   └── DialogueSample.xml (示例对话定义)
└── Source/
    └── SuperbRecruitment/
        ├── Command_PersuadeVisitor.cs
        ├── Command_RecruitVisitor.cs
        ├── Dialog_SimplePersuasion.cs
        ├── HarmonyPatches.cs
        ├── Hediff_PersuasionTracking.cs
        ├── HediffComp_PersuasionTracker.cs
        ├── PersuasionDialogueManager.cs
        └── SuperbRecruitment.csproj
```

### 依赖 / Dependencies
- RimWorld 1.5
- Harmony 2.3.3
- (可选) RimTalk 模组

### 编译 / Compilation
1. 打开 Visual Studio
2. 加载 SuperbRecruitment.csproj
3. 确保引用路径正确
4. 编译项目

## RimTalk 集成说明 / RimTalk Integration

如果你想创建自定义对话：
1. 复制 `RimTalk_Integration/DialogueSample.xml`
2. 修改对话节点和选项
3. 在响应中设置 `effectOnPersuasion` 来影响说服值
4. 将文件放入 RimTalk 的 Dialogues 文件夹

If you want to create custom dialogues:
1. Copy `RimTalk_Integration/DialogueSample.xml`
2. Modify dialogue nodes and options
3. Set `effectOnPersuasion` in responses to affect persuasion
4. Place file in RimTalk's Dialogues folder

## 已知问题 / Known Issues

- RimTalk API 调用可能需要根据实际 RimTalk 版本调整
- 某些边缘情况下访客可能离开时不会移除 Hediff

## 更新日志 / Changelog

### v1.0.0 (初始版本)
- 基础说服系统
- 简单对话窗口
- RimTalk 集成框架
- 中英文本地化

## 许可 / License

MIT License

## 作者 / Author

Your Name / 你的名字

## 鸣谢 / Credits

- RimWorld by Ludeon Studios
- Harmony by Andreas Pardeike
- RimTalk Mod (如果使用)
