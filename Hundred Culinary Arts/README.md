# Living Weapons - 活体兵器

**宣传语：** "别扔掉那把极差的短刀，它可能饿了。"

## 核心体验
废土捡漏 + Roguelike养成 + 电子宠物式维护

## 功能特性

### 1. 逆向品质概率
- **极差品质**: 20% 概率成为活体兵器（黄金矿山）
- **差品质**: 10% 概率
- **普通品质**: 1% 概率（彩蛋）
- **良好及以上**: 0% 概率（绝无可能）

### 2. 潜伏与觉醒
- 新生成的活体兵器处于**潜伏状态**，外观和属性与普通武器无异
- 积累100点经验后**觉醒**，开启成长系统
- 只有查看详细信息才能看到真实等级和加成

### 3. Roguelike成长系统
- 最高等级：Lv.20
- 每次升级随机获得一个属性点
- **属性池**：
  - 伤害：+10% 最终伤害
  - 穿甲：+0.05 护甲穿透
  - 命中：+5% 命中率
  - 攻速：-3% 冷却时间（近战）
  - 瞄准：-4% 瞄准时间（远程）

### 4. 技能系统
- **Lv.5 被动技能**：
  - 近战：嗜血（回血）、极速（移速+0.5）
  - 远程：鹰眼（无视掩体）、狩猎（对动物+50%伤害）
- **Lv.10 主动技能**：
  - 近战：暗影步（瞬移攻击）、剑刃风暴（AOE）
  - 远程：过载射击（3倍伤害必中）、多重射击（连射3发）

### 5. 饥饿机制
- 3天未获得经验开始**饥饿**
- 每天扣除大量经验，可能导致**降级**
- 降级时随机移除一个属性点

### 6. 战略性重铸
- 对武器Build不满意？故意饿它几天
- 降级后重新练级，有机会洗出更好的属性组合

### 7. 敌人掉落
- 敌人有10%概率携带Lv.1-5的觉醒武器
- 击败强敌可能获得已成型的强力武器

### 8. 科技平衡
- 低科技（新石器/中世纪）：100% 经验获取
- 高科技（工业/太空）：20%-50% 经验获取
- 防止批量工业化生产活体兵器

## 构建说明

### 前置要求
1. RimWorld 1.5
2. Harmony库
3. .NET Framework 4.7.2

### 构建步骤

1. **修改项目文件中的RimWorld路径**

编辑 `Source/LivingWeapons/LivingWeapons.csproj`，将以下路径修改为你的RimWorld安装路径：

```xml
<Reference Include="Assembly-CSharp">
  <HintPath>你的RimWorld路径\RimWorldWin64_Data\Managed\Assembly-CSharp.dll</HintPath>
</Reference>
```

2. **编译项目**

```powershell
cd Source/LivingWeapons
dotnet build
```

编译后的DLL会自动输出到 `Assemblies` 文件夹。

3. **安装模组**

将整个模组文件夹复制到：
```
RimWorld/Mods/LivingWeapons/
```

4. **启动游戏**

在模组列表中启用"Living Weapons - 活体兵器"。

## 开发指南

### 项目结构

```
LivingWeapons/
├── About/
│   └── About.xml                      # 模组元数据
├── Assemblies/                        # 编译输出（自动生成）
├── Defs/
│   ├── ThingDefs/
│   │   └── LivingWeaponDefs.xml      # 物品定义
│   └── AbilityDefs/                   # 技能定义
├── Languages/
│   ├── ChineseSimplified/            # 简体中文
│   └── English/                       # 英文
└── Source/
    └── LivingWeapons/
        ├── CompLivingWeapon.cs        # 核心组件
        ├── LivingWeaponAttributes.cs  # 属性系统
        ├── Gizmo_LivingWeaponAbility.cs # UI组件
        └── HarmonyPatches/            # Harmony补丁
            ├── HarmonyInit.cs
            ├── Patch_CompQuality_SetQuality.cs
            ├── Patch_StatInjection.cs
            ├── Patch_ExperienceGain.cs
            ├── Patch_EnemyGeneration.cs
            └── Patch_AbilitySystem.cs
```

### 核心类说明

#### CompLivingWeapon
活体武器的核心组件，负责：
- 状态管理（潜伏/觉醒）
- 等级和经验
- 属性存储和计算
- 饥饿检查
- 升级/降级逻辑

#### LWAttribute (枚举)
定义所有可能的属性类型。

#### LWConstants
游戏平衡常量配置。

### Harmony补丁说明

1. **Patch_CompQuality_SetQuality**: 拦截品质生成，实现逆向概率
2. **Patch_StatInjection**: 注入属性加成到武器数值
3. **Patch_ExperienceGain**: 监听战斗事件，添加经验
4. **Patch_EnemyGeneration**: 为敌人生成觉醒武器
5. **Patch_AbilitySystem**: 管理技能系统

## 兼容性

- ? 完全兼容原版
- ? 不修改任何原版Def
- ? 不冲突原版Biocoding
- ? 理论上兼容所有添加武器的模组

## 已知问题

1. 主动技能目前只有框架，具体逻辑需要进一步实现
2. 被动技能的Hediff应用需要完善
3. UI图标使用占位符，需要自定义贴图

## 计划功能

- [ ] 武器外观特效（粒子效果）
- [ ] 更多技能种类
- [ ] 武器"饱食度"UI显示
- [ ] 武器命名系统
- [ ] 传说武器（超越Lv.20）

## 许可证

MIT License

## 作者

[Your Name]

## 致谢

感谢RimWorld社区和Harmony库的贡献者们。
