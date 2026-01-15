# 希德莉亚·血棘骑士 - 开发清单

## ?? 当前进度：基础XML模组完成

---

## ? 已完成

### 核心文件
- [x] About/About.xml - 模组信息
- [x] About/Preview.png - 预览图占位符
- [x] README.md - 项目说明
- [x] DESIGN.md - 设计文档

### 角色定义
- [x] PawnKindDefs - 角色类型（希德莉亚 + 普通血骑士）
- [x] BackstoryDefs - 背景故事（3个可选背景）
- [x] TraitDefs - 特质系统（5个专属特质）

### 战斗系统
- [x] HediffDefs - 血原质系统和战斗效果
  - [x] 血原质资源定义
  - [x] 血族龙裔体质
  - [x] 5个技能buff效果
  - [x] 嗜血狂怒状态
- [x] ThoughtDefs - 心情系统（9种思想）
- [x] WeaponDefs - 专属武器（血棘 + 血刃短剑）

### 场景和本地化
- [x] ScenarioDefs - 血棘骑士开局场景
- [x] Languages/English - 英文翻译
- [x] Languages/ChineseSimplified - 中文翻译

---

## ?? 待完成 - 优先级高

### 1. 贴图资源 ?????
**重要性：必需**

#### About目录
- [ ] Preview.png (640x360) - Steam预览图
  - 需要展示希德莉亚形象
  - 添加模组标题和特色

#### 角色贴图
- [ ] Things/Pawn/Humanlike/Bodies/
  - [ ] 身体_南(正面)
  - [ ] 身体_北(背面)
  - [ ] 身体_东(侧面)
  - [ ] 多种体型变体

- [ ] Things/Pawn/Humanlike/Heads/
  - [ ] 头部贴图（多个发型）
  - [ ] 面部特征

#### 武器贴图
- [ ] Things/Weapons/BloodThorn.png
  - 血棘长剑主体
  - 荆棘细节
  - 发光效果（可选）
  
- [ ] Things/Weapons/BloodDagger.png
  - 血刃短剑

#### UI图标
- [ ] UI/Icons/Sideria_Icon.png - 角色图标
- [ ] UI/Abilities/ - 5个技能图标
  - [ ] BloodThornStrike.png
  - [ ] BloodRush.png
  - [ ] SanguineReforge.png
  - [ ] DragonbloodAegis.png
  - [ ] BloodThornStorm.png

---

### 2. C#代码实现 ?????
**重要性：技能系统必需**

#### 核心类
- [ ] Source/BloodEssence/BloodEssenceManager.cs
  - [ ] 血原质资源管理
  - [ ] UI显示组件
  - [ ] 资源消耗/回复逻辑

- [ ] Source/BloodEssence/BloodEssenceGizmo.cs
  - [ ] 血原质UI条
  - [ ] 实时显示
  - [ ] 颜色变化

#### 技能系统
- [ ] Source/Abilities/BloodAbilityBase.cs
  - [ ] 技能基类
  - [ ] 资源消耗检测
  - [ ] 冷却管理

- [ ] Source/Abilities/Ability_BloodThornStrike.cs
- [ ] Source/Abilities/Ability_BloodRush.cs
- [ ] Source/Abilities/Ability_SanguineReforge.cs
- [ ] Source/Abilities/Ability_DragonbloodAegis.cs
- [ ] Source/Abilities/Ability_BloodThornStorm.cs

#### 战斗机制
- [ ] Source/Combat/BloodEssenceRecovery.cs
  - [ ] 击杀回复机制
  - [ ] 伤害回复机制

- [ ] Source/Combat/BerserkTrigger.cs
  - [ ] 低血量狂怒触发

#### Harmony补丁
- [ ] Source/HarmonyPatches/Patch_Pawn_Kill.cs
  - 击杀事件注入
  
- [ ] Source/HarmonyPatches/Patch_Pawn_TakeDamage.cs
  - 受伤事件注入

#### 项目配置
- [ ] Source/Sideria.csproj - C#项目文件
- [ ] 引用必要的DLL
  - Assembly-CSharp.dll
  - UnityEngine.dll
  - 0Harmony.dll

---

### 3. 游戏测试 ????
**重要性：平衡性必需**

#### 基础测试
- [ ] 角色生成测试
- [ ] 特质应用测试
- [ ] 装备测试
- [ ] 场景开局测试

#### 战斗测试
- [ ] 近战伤害输出
- [ ] 生存能力（坦度）
- [ ] 回复速度
- [ ] 多敌人战斗
- [ ] Boss级敌人战斗

#### 平衡性调整
- [ ] 血原质消耗比例
- [ ] 技能冷却时间
- [ ] 伤害数值
- [ ] 护甲数值
- [ ] 回复速度

---

## ?? 待完成 - 优先级中

### 4. 内容扩展 ???
- [ ] 添加专属护甲套装
- [ ] 添加血族龙裔种族定义（完整版）
- [ ] 创建更多血棘武器变体
- [ ] 添加专属饰品/配件

### 5. 特效和音效 ???
- [ ] Sounds/Abilities/ - 技能音效
  - [ ] 血棘突刺.wav
  - [ ] 血之疾行.wav
  - [ ] 血肉重铸.wav
  - [ ] 龙血护盾.wav
  - [ ] 血棘风暴.wav

- [ ] 粒子特效（需要Unity）
  - [ ] 血雾效果
  - [ ] 护盾特效
  - [ ] 风暴特效

### 6. 文档完善 ??
- [ ] 编写详细的玩家指南
- [ ] 添加FAQ常见问题
- [ ] 制作技能演示视频
- [ ] 撰写更新日志

---

## ?? 待完成 - 优先级低

### 7. 高级特性 ??
- [ ] 血棘骑士团派系
- [ ] 专属任务事件
- [ ] 多个可招募角色
- [ ] 故事线系统
- [ ] 成就系统

### 8. 联动和兼容 ?
- [ ] Combat Extended兼容补丁
- [ ] Royalty DLC集成
- [ ] 其他角色模组联动
- [ ] 自定义研究项目

---

## ?? 已知问题

目前没有已知问题（基础XML阶段）

---

## ?? 时间规划

### 第1周：美术资源
- 完成所有必需的贴图
- 制作预览图
- 准备Steam宣传材料

### 第2-3周：C#开发
- 搭建项目框架
- 实现核心类
- 完成技能系统
- 完成Harmony补丁

### 第4周：测试和平衡
- 全面游戏测试
- 数值调整
- bug修复
- 性能优化

### 第5周：发布准备
- 文档完善
- Steam页面制作
- 宣传材料准备
- 社区预热

---

## ?? 开发建议

### 推荐工具
1. **Visual Studio 2022** - C#开发
2. **dnSpy** - RimWorld代码查看
3. **Photoshop/GIMP** - 贴图制作
4. **Aseprite** - 像素艺术（如果用像素风格）
5. **Git** - 版本控制

### 学习资源
- [RimWorld Modding Wiki](https://rimworldwiki.com/wiki/Modding_Tutorials)
- [Harmony Documentation](https://harmony.pardeike.net/)
- [RimWorld Discord](https://discord.gg/rimworld)
- [Ludeon论坛](https://ludeon.com/forums/)

---

## ?? 里程碑

- [ ] **里程碑1**: 基础XML模组 ? **已完成**
- [ ] **里程碑2**: 添加贴图，可视化完成
- [ ] **里程碑3**: C#代码实现，技能系统可用
- [ ] **里程碑4**: 测试完成，平衡性调整
- [ ] **里程碑5**: 发布Steam创意工坊

---

**当前状态**: 基础XML完成，等待贴图和C#开发
**下一步**: 制作角色和武器贴图
**预计完成时间**: 根据开发进度调整

---

## ?? 需要帮助？

如果在开发过程中遇到问题：
1. 查看RimWorld官方文档
2. 在Discord或论坛寻求帮助
3. 参考其他优秀模组的代码
4. 使用dnSpy查看游戏源代码

**加油！创造出令人惊艳的模组！** ??
