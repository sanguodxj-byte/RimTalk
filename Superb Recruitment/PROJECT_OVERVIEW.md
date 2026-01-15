# ?? Superb Recruitment (卓越招募) - 项目总览

## ?? 项目简介

Superb Recruitment 是一个 RimWorld 模组，通过对话系统来招募访客，而不是传统的囚禁招募方式。

### 核心理念
- **对话驱动**: 通过有意义的对话建立关系
- **隐藏机制**: 说服值对玩家保持神秘
- **策略选择**: 平衡尝试次数和成功率
- **社交系统**: 利用殖民者的社交技能

---

## ?? 项目结构

```
Superb Recruitment/
│
├── ?? 核心文档
│   ├── README.md                    # 项目说明
│   ├── CHANGELOG.md                 # 更新日志
│   ├── LICENSE                      # MIT许可证
│   ├── UserGuide_CN.md             # 用户指南（中文）
│   ├── DEVELOPER_GUIDE.md          # 开发者指南
│   └── DEPLOYMENT_CHECKLIST.md     # 部署检查清单
│
├── ?? 工具脚本
│   ├── Build.bat                    # 编译脚本
│   └── Deploy.bat                   # 部署脚本
│
├── ?? 模组文件
│   ├── About/                       # 模组信息
│   │   ├── About.xml
│   │   └── PublishedFileId.txt
│   │
│   ├── Assemblies/                  # 编译输出（自动生成）
│   │   └── SuperbRecruitment.dll
│   │
│   ├── Defs/                        # 游戏定义
│   │   └── HediffDefs/
│   │       └── Hediffs_PersuasionTracking.xml
│   │
│   ├── Languages/                   # 本地化
│   │   ├── English/
│   │   │   └── Keyed/
│   │   │       └── SuperbRecruitment.xml
│   │   └── ChineseSimplified/
│   │       └── Keyed/
│   │           └── SuperbRecruitment.xml
│   │
│   └── Source/                      # 源代码
│       └── SuperbRecruitment/
│           ├── SuperbRecruitment.csproj
│           ├── Command_PersuadeVisitor.cs
│           ├── Command_RecruitVisitor.cs
│           ├── Dialog_SimplePersuasion.cs
│           ├── HarmonyPatches.cs
│           ├── Hediff_PersuasionTracking.cs
│           ├── HediffComp_PersuasionTracker.cs
│           └── PersuasionDialogueManager.cs
│
└── ?? 集成示例
    └── RimTalk_Integration/
        └── DialogueSample.xml       # RimTalk对话示例
```

---

## ?? 快速开始

### 方式1: 使用脚本（推荐）

1. **编译项目**
   ```bash
   双击 Build.bat
   ```

2. **部署模组**
   ```bash
   双击 Deploy.bat
   ```

3. **启动游戏**
   - 打开 RimWorld
   - 启用 "Superb Recruitment"
   - 重启游戏

### 方式2: 手动操作

1. **Visual Studio 编译**
   - 打开 `Source\SuperbRecruitment\SuperbRecruitment.csproj`
   - 选择 Release 配置
   - 生成解决方案

2. **手动部署**
   - 复制文件到 `D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment\`

---

## ?? 核心功能

### 1. 说服系统
- ? 隐藏的说服值（20%-80%正态分布）
- ? 玩家亲自说服：3次机会
- ? 殖民者辅助：每人1次
- ? 社交技能影响效果

### 2. 对话系统
- ? 5种对话选项
- ? 难度与收益平衡
- ? 随机成功率
- ? 未来支持RimTalk集成

### 3. 招募机制
- ? 基于说服值的成功率计算
- ? 失败惩罚机制
- ? 访客生气离开系统

---

## ?? 游戏机制详解

### 说服值计算
```
初始值: 正态分布 N(0.5, 0.15)
范围: 0% - 99%
显示: "?" (隐藏)
```

### 成功率公式
```
成功率 = (说服值)^2.5

示例:
30% → 4.9%
50% → 17.7%
70% → 42.7%
90% → 72.5%
```

### NPC说服效果
```
基础: 8%
社交技能: +0.5% per level
善良特性: +15%
精神病质: -20%
随机变化: ±20%
```

---

## ?? 技术栈

### 依赖项
- **RimWorld**: 1.5
- **Harmony**: 2.3.3
- **Krafs.Rimworld.Ref**: 1.5.4104
- **RimTalk**: 可选

### 开发工具
- Visual Studio 2019/2022
- .NET Framework 4.7.2
- Git (版本控制)

---

## ?? 文档导航

### 用户文档
- **[README.md](README.md)** - 项目概述和基本说明
- **[UserGuide_CN.md](UserGuide_CN.md)** - 详细的游戏指南
- **[CHANGELOG.md](CHANGELOG.md)** - 版本更新记录

### 开发者文档
- **[DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md)** - 开发指南和API说明
- **[DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)** - 部署前检查清单

### 其他文档
- **[LICENSE](LICENSE)** - MIT开源许可证

---

## ?? 代码架构

### 核心类说明

| 类名 | 职责 | 文件 |
|------|------|------|
| `Hediff_PersuasionTracking` | 追踪说服进度 | Hediff_PersuasionTracking.cs |
| `PersuasionDialogueManager` | 管理对话流程 | PersuasionDialogueManager.cs |
| `Dialog_SimplePersuasion` | 对话界面 | Dialog_SimplePersuasion.cs |
| `Command_PersuadeVisitor` | 说服按钮 | Command_PersuadeVisitor.cs |
| `Command_RecruitVisitor` | 招募按钮 | Command_RecruitVisitor.cs |
| `HarmonyPatches` | Harmony补丁 | HarmonyPatches.cs |

### 设计模式
- **单例模式**: PersuasionDialogueManager
- **组件模式**: HediffComp系统
- **命令模式**: Gizmo系统
- **策略模式**: 对话选项处理

---

## ? 测试清单

### 基础功能
- [x] 说服按钮显示
- [x] 对话窗口打开
- [x] 玩家说服功能
- [x] NPC说服功能
- [x] 招募功能

### 边缘情况
- [x] 多访客处理
- [x] 保存/加载
- [x] 访客离开
- [x] 错误处理

---

## ?? 已知问题

1. **RimTalk集成** - 当前使用简单对话窗口
2. **Hediff清理** - 某些情况下可能不完整
3. **性能优化** - 频繁查询可以优化

---

## ?? 未来计划

### v1.1 (短期)
- [ ] 自定义图标
- [ ] 音效系统
- [ ] 更多对话选项
- [ ] 优化UI

### v1.2 (中期)
- [ ] 完整RimTalk集成
- [ ] 访客特性影响
- [ ] 阵营关系影响
- [ ] 统计系统

### v2.0 (长期)
- [ ] 自定义对话树编辑器
- [ ] 成就系统
- [ ] 多人对话
- [ ] 事件系统

---

## ?? 支持与反馈

### 报告问题
1. 查看已知问题列表
2. 检查日志文件
3. 在 GitHub Issues 报告

### 功能建议
- GitHub Issues: 功能请求
- Steam Workshop: 评论反馈
- Discord: 社区讨论

---

## ?? 贡献指南

欢迎贡献！请：
1. Fork 项目
2. 创建功能分支
3. 提交代码
4. 发起 Pull Request

确保：
- ? 代码注释完整
- ? 遵循代码规范
- ? 测试所有更改
- ? 更新文档

---

## ?? 许可证

本项目使用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件

---

## ?? 致谢

- **RimWorld** by Ludeon Studios
- **Harmony** by Andreas Pardeike
- **RimTalk** 模组作者
- **社区贡献者**

---

## ?? 项目统计

- **代码行数**: ~1500 行 C#
- **文件数量**: 15+ 文件
- **支持语言**: 中文、英文
- **开发时间**: 约X小时

---

**? 如果您喜欢这个项目，请给一个星标！**

**?? 祝您游戏愉快！**
