# 更新日志 | Changelog

所有重要的项目更改都会记录在这个文件中。

## [未发布] - Unreleased

### 计划功能 | Planned Features
- 完整的 RimTalk 集成
- 自定义对话树系统
- 访客特性影响说服难度
- 阵营关系影响说服效果
- 统计界面（成功/失败追踪）

## [1.0.0] - 2024-01-XX (初始版本)

### 新增功能 | Added
- ? 对话式访客招募系统
- ?? 隐藏的说服值机制（20%-80%正态分布）
- ?? 简单对话窗口（5种对话选项）
- ?? 玩家和NPC说服系统
  - 玩家可以亲自说服3次
  - 每个殖民者可以尝试1次
- ?? 基于幂函数的成功率计算（persuasionValue^2.5）
- ?? 招募尝试系统（成功/失败/访客离开）
- ?? 完整的中英文本地化
- ?? 详细的用户和开发者文档

### 游戏机制 | Mechanics
- 说服值范围：0% - 99%
- 玩家尝试次数：3次
- 殖民者尝试次数：每人1次
- 基础NPC说服效果：8%
- 社交技能加成：每级+0.5%
- 善良特性加成：+15%
- 精神病质惩罚：-20%
- 招募失败惩罚：-15%说服值
- 访客生气阈值：说服值<10%

### 对话选项 | Dialogue Options
1. **友好闲聊**: 5%-12% (难度：简单)
2. **分享故事**: 8%-15% (难度：中等)
3. **描述机会**: 10%-20% (难度：困难)
4. **情感诉求**: 0%-25% (难度：非常困难)
5. **简短交谈**: 2%-5% (难度：非常简单)

### 技术实现 | Technical
- Harmony 2.3.3 补丁系统
- 自定义 Hediff 追踪系统
- 单例模式对话管理器
- Gizmo 命令系统
- 自定义 Window 对话界面

### 文件结构 | Files
```
核心代码:
- Hediff_PersuasionTracking.cs (说服追踪)
- HediffComp_PersuasionTracker.cs (Hediff组件)
- PersuasionDialogueManager.cs (对话管理器)
- Dialog_SimplePersuasion.cs (对话窗口)
- Command_PersuadeVisitor.cs (说服命令)
- Command_RecruitVisitor.cs (招募命令)
- HarmonyPatches.cs (Harmony补丁)

定义文件:
- Hediffs_PersuasionTracking.xml (Hediff定义)

本地化:
- English/Keyed/SuperbRecruitment.xml
- ChineseSimplified/Keyed/SuperbRecruitment.xml

文档:
- README.md
- UserGuide_CN.md
- DEVELOPER_GUIDE.md
- DEPLOYMENT_CHECKLIST.md
```

### 兼容性 | Compatibility
- ? RimWorld 1.5
- ? Hospitality
- ? Psychology
- ?? 可能与其他招募模组冲突

### 已知问题 | Known Issues
- RimTalk 集成尚未完成（计划中）
- 访客离开时 Hediff 清理可能不完整
- 某些边缘情况下的错误处理需要改进

### 平衡性考虑 | Balance Notes
- 平均说服值：50%
- 50%说服值的成功率：~17.7%
- 预期需要2-4次对话才能达到可招募状态
- 预期总体招募成功率：30-40%（经过多次尝试）

---

## 版本规范 | Version Format

使用语义化版本：`主版本.次版本.修订号`

- **主版本**: 重大功能变更或不兼容更新
- **次版本**: 新增功能，向下兼容
- **修订号**: Bug修复和小改进

## 变更类型 | Change Types

- `新增 | Added` - 新功能
- `修改 | Changed` - 现有功能的变更
- `弃用 | Deprecated` - 即将移除的功能
- `移除 | Removed` - 已移除的功能
- `修复 | Fixed` - Bug修复
- `安全 | Security` - 安全性修复

---

## 反馈渠道 | Feedback

- **GitHub Issues**: 报告Bug和功能请求
- **Steam Workshop**: 评论和讨论
- **Discord**: 社区交流

感谢您的支持！??
