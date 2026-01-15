# ?? 完整贴图文件夹结构和清单

## ?? 快速导航

- [文件夹结构](#文件夹结构)
- [必需贴图列表](#必需贴图列表)
- [贴图规格要求](#贴图规格要求)
- [部署说明](#部署说明)
- [检查工具](#检查工具)

---

## ?? 文件夹结构

```
Textures/
├── Things/
│   ├── Pawn/
│   │   └── Humanlike/
│   │       ├── Bodies/
│   │       │   └── Dracovampir/           [6个文件 - 优先级1]
│   │       ├── BodyAddons/                [12个文件 - 优先级2]
│   │       └── Heads/
│   │           ├── Male/                  [可选]
│   │           └── Female/                [可选]
│   └── Item/
│       └── Equipment/
│           └── WeaponMelee/               [3个文件 - 优先级3]
└── UI/
    ├── Abilities/                         [8个文件 - 优先级4]
    └── Icons/                             [可选]
```

---

## ?? 必需贴图列表

### 【优先级1 - 必需】身体贴图

**路径**: `Textures/Things/Pawn/Humanlike/Bodies/Dracovampir/`

| 文件名 | 尺寸 | 格式 | 说明 |
|--------|------|------|------|
| `Naked_Male_south.png` | 512×512 | PNG透明 | 男性正面 |
| `Naked_Male_east.png` | 512×512 | PNG透明 | 男性侧面 |
| `Naked_Male_north.png` | 512×512 | PNG透明 | 男性背面 |
| `Naked_Female_south.png` | 512×512 | PNG透明 | 女性正面 |
| `Naked_Female_east.png` | 512×512 | PNG透明 | 女性侧面 |
| `Naked_Female_north.png` | 512×512 | PNG透明 | 女性背面 |

**总计**: 6个文件

**配色方案**:
- 主色调: RGB(224, 199, 199) - 浅红白色
- 阴影: RGB(167, 20, 20) - 深红色
- 肌肉纹理，龙族特征

---

### 【优先级2 - 高优先】Body Addons

**路径**: `Textures/Things/Pawn/Humanlike/BodyAddons/`

#### 龙角 (Dragon Horns)
| 文件名 | 尺寸 | 格式 | 说明 |
|--------|------|------|------|
| `DragonHorns_south.png` | 512×512 | PNG透明 | 龙角正面 |
| `DragonHorns_east.png` | 512×512 | PNG透明 | 龙角侧面 |
| `DragonHorns_north.png` | 512×512 | PNG透明 | 龙角背面 |

**设计要点**:
- 弯曲的结晶角
- 深红/黑色配色
- 位于头顶上方

#### 龙翼 (Dragon Wings)
| 文件名 | 尺寸 | 格式 | 说明 |
|--------|------|------|------|
| `DragonWings_south.png` | 512×512 | PNG透明 | 龙翼正面 |
| `DragonWings_east.png` | 512×512 | PNG透明 | 龙翼侧面 |
| `DragonWings_north.png` | 512×512 | PNG透明 | 龙翼背面 |

**设计要点**:
- 蝙蝠式膜翼
- 骨架清晰可见
- 血红色半透明膜

#### 龙尾 (Dragon Tail)
| 文件名 | 尺寸 | 格式 | 说明 |
|--------|------|------|------|
| `DragonTail_south.png` | 512×512 | PNG透明 | 龙尾正面 |
| `DragonTail_east.png` | 512×512 | PNG透明 | 龙尾侧面 |
| `DragonTail_north.png` | 512×512 | PNG透明 | 龙尾背面 |

**设计要点**:
- 长而有力的尾巴
- 鳞片纹理
- 末端带刺

#### 血纹 (Blood Markings)
| 文件名 | 尺寸 | 格式 | 说明 |
|--------|------|------|------|
| `BloodMarkings_south.png` | 512×512 | PNG透明 | 血纹正面 |
| `BloodMarkings_east.png` | 512×512 | PNG透明 | 血纹侧面 |
| `BloodMarkings_north.png` | 512×512 | PNG透明 | 血纹背面 |

**设计要点**:
- 发光的红色符文
- 覆盖身体表面
- 半透明叠加效果

**Body Addons总计**: 12个文件

---

### 【优先级3 - 中优先】武器贴图

**路径**: `Textures/Things/Item/Equipment/WeaponMelee/`

| 文件名 | 尺寸 | 格式 | 说明 |
|--------|------|------|------|
| `Atzgand.png` | 256×256 | PNG透明 | 阿茨冈德（基础版） |
| `Atzgand_Ascended.png` | 256×256 | PNG透明 | 阿茨冈德（升华版） |
| `BloodDagger.png` | 256×256 | PNG透明 | 血刃短剑 |

**设计要点**:

#### Atzgand (基础版)
- 长剑造型
- 深红色刀身
- 荆棘装饰
- 血红色光晕

#### Atzgand Ascended (升华版)
- 金色光芒
- 更华丽的纹理
- 神圣感觉
- 龙鳞纹理

#### Blood Dagger
- 短剑/匕首
- 弯曲刀身
- 简单设计

**武器总计**: 3个文件

---

### 【优先级4 - 低优先】技能图标

**路径**: `Textures/UI/Abilities/`

| 文件名 | 尺寸 | 格式 | 说明 |
|--------|------|------|------|
| `DragonBreath.png` | 64×64 | PNG | 龙息 |
| `DragonicAura.png` | 64×64 | PNG | 龙之光环 |
| `DragonWings.png` | 64×64 | PNG | 龙翼展开 |
| `DragonicTransformation.png` | 64×64 | PNG | 龙化 |
| `BloodDrain.png` | 64×64 | PNG | 血液汲取 |
| `VampiricEmbrace.png` | 64×64 | PNG | 血族拥抱 |
| `BloodFrenzy.png` | 64×64 | PNG | 血之狂怒 |
| `OathbreakerTransformation.png` | 64×64 | PNG | 弃誓者转化 |

**设计要点**:
- 清晰的图标设计
- 统一风格
- 龙魂路线：金色调
- 血骸路线：红色调

**图标总计**: 8个文件

---

## ?? 贴图规格要求

### 身体和Body Addons (512×512)
- **分辨率**: 512×512像素
- **格式**: PNG
- **颜色**: 32位RGBA
- **透明**: 必须有Alpha通道
- **命名**: 严格按照`名称_方向.png`格式

### 武器 (256×256)
- **分辨率**: 256×256像素
- **格式**: PNG
- **颜色**: 32位RGBA
- **透明**: 必须有Alpha通道

### 图标 (64×64)
- **分辨率**: 64×64像素
- **格式**: PNG
- **颜色**: 32位RGBA
- **背景**: 可以有背景色

---

## ?? 配色参考

### 血龙种肤色
```
主色调: RGB(224, 199, 199) = #E0C7C7
深色:   RGB(199, 178, 178) = #C7B2B2
阴影:   RGB(167, 20, 20)   = #A71414
高光:   RGB(235, 215, 215) = #EBD7D7
```

### 龙魂路线（金色）
```
主色:   RGB(255, 215, 0)   = #FFD700
光晕:   RGB(255, 235, 100) = #FFEB64
```

### 血骸路线（红色）
```
主色:   RGB(200, 50, 80)   = #C83250
暗红:   RGB(120, 20, 40)   = #781428
血光:   RGB(255, 100, 130) = #FF6482
```

---

## ??? 工具脚本

### 1. 创建文件夹结构
```batch
CreateTextureStructure.bat
```
**功能**: 自动创建所有必需的文件夹

### 2. 检查贴图完整性
```batch
CheckTextures.bat
```
**功能**: 
- 扫描所有贴图文件
- 显示缺失文件列表
- 统计完成度

### 3. 部署到RimWorld
```batch
DeployWithSymlink.bat
```
**功能**: 
- 使用符号链接部署
- 修改源文件自动同步
- 需要管理员权限

---

## ?? 部署流程

### 完整部署步骤

1. **创建文件夹结构**
   ```batch
   右键管理员运行: CreateTextureStructure.bat
   ```

2. **放置贴图文件**
   - 按照上述清单放入对应文件夹
   - 确保文件名和格式正确

3. **检查完整性**
   ```batch
   运行: CheckTextures.bat
   ```

4. **部署到RimWorld**
   ```batch
   右键管理员运行: DeployWithSymlink.bat
   ```

5. **测试**
   - 启动RimWorld
   - 启用mod
   - 生成角色检查

---

## ?? 注意事项

### 必须遵守的规则
1. **文件名大小写敏感** - 必须完全匹配
2. **方向后缀必须** - _south, _east, _north
3. **透明通道必须** - 身体和Body Addons必须有透明背景
4. **尺寸必须准确** - 不能缩放或变形

### 常见错误
? 文件名拼写错误
? 忘记添加方向后缀
? 没有透明通道
? 尺寸不正确
? 格式不是PNG

---

## ?? 贴图优先级和影响

### 无贴图时
- ? 显示粉红色占位符
- ? 功能完全正常
- ? 可以正常游玩

### 只有身体贴图
- ? 角色正常显示
- ? 没有龙角/龙翼等装饰
- ?? 可以游玩，但缺少视觉效果

### 身体 + Body Addons
- ? 角色完整显示
- ? 龙族特征完整
- ? 推荐配置

### 完整贴图
- ? 所有视觉效果
- ? 武器显示正常
- ? 技能图标完整
- ? 最佳体验

---

## ?? 相关文档

- **贴图制作指南**: `TEXTURE_GUIDE_CORRECT.md`
- **Body Addons详解**: `BODY_ADDONS_GUIDE.md`
- **武器美术指南**: `ATZGAND_ART_GUIDE.md`
- **AI生成指南**: `AI_GENERATION_GUIDE.md`

---

## ?? 获取帮助

### 问题排查

**Q: 贴图不显示？**
A: 
1. 检查文件名是否正确
2. 检查路径是否正确
3. 运行CheckTextures.bat检查
4. 确认文件格式是PNG

**Q: 粉红色占位符？**
A: 
- 正常现象，表示贴图缺失
- 不影响功能
- 添加贴图后自动显示

**Q: 贴图位置不对？**
A: 
- 检查Body Addons配置
- 调整defaultOffset值
- 参考BODY_ADDONS_GUIDE.md

---

## ? 检查清单

### 部署前检查
- [ ] 所有文件夹已创建
- [ ] 身体贴图已放置（6个）
- [ ] Body Addons已放置（12个）
- [ ] 武器贴图已放置（3个）
- [ ] 图标已放置（8个）
- [ ] 运行CheckTextures.bat确认
- [ ] 运行DeployWithSymlink.bat部署

### 测试检查
- [ ] RimWorld正常启动
- [ ] Mod成功加载
- [ ] 角色显示正常
- [ ] 龙角/龙翼显示正常
- [ ] 武器显示正常
- [ ] 无错误日志

---

**文档版本**: 1.0  
**最后更新**: 2024  
**维护者**: Sideria Mod Team  

**状态**: ? 准备就绪 - 等待贴图文件
