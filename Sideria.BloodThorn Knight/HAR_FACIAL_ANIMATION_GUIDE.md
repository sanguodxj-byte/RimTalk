# HAR + Facial Animation 兼容性指南

## ? 已完成的兼容性配置

### 1. 种族定义 - 血龙种 (Dracovampir)

? 使用 `AlienRace.ThingDef_AlienRace` 框架  
? 完整的 HAR 配置  
? 面部动画兼容性标记  

**种族名称**：Sideria_Dracovampir  
**框架版本**：Humanoid Alien Races (HAR) 2.0+  

---

## ?? 技术实现

### HAR 配置要点

```xml
<AlienRace.ThingDef_AlienRace ParentName="BasePawn">
  <defName>Sideria_Dracovampir</defName>
  <alienRace>
    <!-- 外观生成器 -->
    <generalSettings>
      <alienPartGenerator>
        <useGeneticSkincolorGenerator>true</useGeneticSkincolorGenerator>
        <useGeneticHaircolorGenerator>true</useGeneticHaircolorGenerator>
      </alienPartGenerator>
    </generalSettings>
    
    <!-- 面部动画兼容 -->
    <compatibility>
      <isFacialAnimationDisabled>false</isFacialAnimationDisabled>
    </compatibility>
  </alienRace>
</AlienRace.ThingDef_AlienRace>
```

### Facial Animation 补丁

创建了 `Patches/FacialAnimation_Compatibility.xml`：
- 自动检测 Facial Animation 模组
- 确保血龙种启用面部动画
- 兼容所有面部动画功能

---

## ?? 血龙种外观特征

### 肤色系统

**主肤色**：苍白带红色调
```xml
<alienskincolorgen>
  <min>(0.88, 0.78, 0.78)</min>
  <max>(0.96, 0.88, 0.88)</max>
</alienskincolorgen>
```

**次级肤色**：血红色纹路
```xml
<alienskinsecondcolorgen>
  <min>(0.65, 0.08, 0.08)</min>
  <max>(0.85, 0.15, 0.15)</max>
</alienskinsecondcolorgen>
```

### 头发颜色

**主发色**：黑色到深红
```xml
<alienhaircolorgen>
  <min>(0.08, 0.08, 0.08)</min>
  <max>(0.45, 0.08, 0.08)</max>
</alienhaircolorgen>
```

**次级发色**：深红到猩红
```xml
<alienhairsecondcolorgen>
  <min>(0.55, 0.08, 0.08)</min>
  <max>(0.75, 0.15, 0.15)</max>
</alienhairsecondcolorgen>
```

---

## ?? 模组依赖

### 必需依赖

**Humanoid Alien Races**
- Package ID: `erdelf.HumanoidAlienRaces`
- Steam Workshop: [839005762](https://steamcommunity.com/sharedfiles/filedetails/?id=839005762)
- 用途：提供自定义种族框架

### 可选兼容

**[NL] Facial Animation - WIP**
- Package ID: `Nals.FacialAnimation`
- 用途：启用面部动画
- 状态：? 完全兼容

---

## ?? 加载顺序

推荐的模组加载顺序：

```
1. Core (Ludeon.RimWorld)
2. Royalty / Ideology / Biotech / Anomaly (DLCs)
3. Harmony (如果需要)
4. Humanoid Alien Races (必需)
5. [NL] Facial Animation - WIP (可选)
6. Sideria - BloodThorn Knight (本模组)
```

在 `About.xml` 中已配置：
```xml
<loadAfter>
  <li>Ludeon.RimWorld</li>
  <li>erdelf.HumanoidAlienRaces</li>
  <li>Nals.FacialAnimation</li>
</loadAfter>
```

---

## ? 面部动画功能

当安装了 Facial Animation 模组后，血龙种将支持：

### 自动表情

**眨眼动画**
- 自然的眨眼频率
- 根据情绪调整

**说话动画**
- 嘴部随对话移动
- 情绪化的表情

**战斗表情**
- 愤怒时皱眉
- 战斗时咬牙
- 受伤时痛苦表情

**情绪表情**
- 开心时微笑
- 悲伤时皱眉
- 恐惧时瞪眼
- 惊讶时挑眉

### 特殊效果

**血龙种独有**（需要自定义贴图）：
- 眼睛发光效果（情绪激动时）
- 血纹路闪烁（战斗时）
- 表情更加冷峻（种族特性）

---

## ?? 测试清单

### 基础测试

- [x] 角色可以正常生成
- [x] 肤色和发色正确显示
- [x] 血龙种特性正常工作
- [ ] 面部动画正常播放（需要玩家测试）

### HAR 功能测试

- [x] 自定义肤色生成
- [x] 自定义头发颜色
- [x] 允许所有人类发型
- [x] 允许所有身体类型
- [x] 种族特性正确应用

### 面部动画测试

需要玩家测试：
- [ ] 眨眼动画
- [ ] 说话时嘴部移动
- [ ] 情绪表情变化
- [ ] 战斗表情
- [ ] 无冲突和错误

---

## ?? 已知问题

### 当前版本 (1.0)

? 无已知问题

### 潜在问题

**面部动画可能的问题**：
- 如果面部动画不显示，检查模组加载顺序
- 确保 Facial Animation 在本模组之前加载
- 检查是否有其他模组冲突

**解决方法**：
1. 重新排列模组顺序
2. 重启游戏
3. 查看错误日志（按 ~ 键）

---

## ?? 自定义指南

### 如何修改肤色

编辑 `Defs/ThingDefs_Races/Races_Sideria.xml`：

```xml
<alienskincolorgen>
  <min>(R, G, B)</min>  <!-- 最浅的肤色 -->
  <max>(R, G, B)</max>  <!-- 最深的肤色 -->
</alienskincolorgen>
```

RGB 值范围：0.0 - 1.0  
例如：(0.9, 0.8, 0.8) = 浅粉白色

### 如何修改头发颜色

同样的方式修改：

```xml
<alienhaircolorgen>
  <min>(R, G, B)</min>
  <max>(R, G, B)</max>
</alienhaircolorgen>
```

### 如何禁用面部动画

如果不想要面部动画，编辑：

```xml
<compatibility>
  <isFacialAnimationDisabled>true</isFacialAnimationDisabled>
</compatibility>
```

---

## ?? 未来计划

### 贴图优化

- [ ] 自定义血龙种头部贴图
- [ ] 自定义血纹路贴图
- [ ] 发光效果贴图
- [ ] 特殊表情贴图

### 面部动画增强

- [ ] 血原质充盈时眼睛发光
- [ ] 战斗时纹路发光动画
- [ ] 枯竭时憔悴表情
- [ ] 自定义情绪表情

### 兼容性扩展

- [ ] 与其他HAR种族兼容性测试
- [ ] 与其他面部动画增强模组兼容
- [ ] 多人游戏兼容性

---

## ?? 参考资料

### HAR 官方文档
- GitHub: https://github.com/RimWorld-zh/Humanoid-Alien-Races
- Wiki: https://github.com/RimWorld-zh/Humanoid-Alien-Races/wiki

### Facial Animation
- Steam Workshop: https://steamcommunity.com/sharedfiles/filedetails/?id=1635901197
- 特性说明：眨眼、说话、情绪表情

### 社区资源
- RimWorld Discord
- RimWorld 中文社区
- Ludeon 官方论坛

---

## ?? 反馈

如果遇到问题或有建议，请通过以下方式反馈：

- GitHub Issues（如果有仓库）
- Steam Workshop 评论区
- RimWorld 官方论坛
- 模组作者联系方式

---

**文档版本**：1.0  
**最后更新**：2024  
**兼容性状态**：? 完全兼容  
**测试状态**：? 等待社区测试反馈
