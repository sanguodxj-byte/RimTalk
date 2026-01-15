# 阿茨冈德武器转化系统 - C#实现指南

## ?? 功能需求

### 核心机制
当希德莉亚装备任意**传奇品质长剑**时：
1. 触发"血之共鸣"事件
2. 长剑转化为**阿茨冈德**
3. 自动**生物编码**绑定到希德莉亚
4. 其他角色无法使用此武器

---

## ?? 实现方案

### 方案1：使用Harmony补丁（推荐）

#### 文件位置
```
Source/
├── Atzgand/
│   ├── CompAtzgandTransformer.cs
│   ├── HarmonyPatches.cs
│   └── AtzgandUtility.cs
```

#### 核心代码

##### 1. CompAtzgandTransformer.cs
```csharp
using Verse;
using RimWorld;

namespace Sideria.Atzgand
{
    /// <summary>
    /// 阿茨冈德转化器组件
    /// </summary>
    public class CompAtzgandTransformer : ThingComp
    {
        public CompProperties_AtzgandTransformer Props => 
            (CompProperties_AtzgandTransformer)props;

        /// <summary>
        /// 检查是否可以转化
        /// </summary>
        public bool CanTransform(Pawn pawn, ThingWithComps weapon)
        {
            // 必须是希德莉亚
            if (pawn.def.defName != "Sideria_BloodThornKnight")
                return false;

            // 必须是传奇品质
            QualityCategory quality;
            if (!weapon.TryGetQuality(out quality))
                return false;

            if (quality != QualityCategory.Legendary)
                return false;

            // 必须是长剑类型
            if (weapon.def.defName.Contains("Longsword") || 
                weapon.def.weaponTags.Contains("MedievalMeleeAdvanced"))
                return true;

            return false;
        }

        /// <summary>
        /// 执行转化
        /// </summary>
        public ThingWithComps TransformToAtzgand(Pawn pawn, ThingWithComps oldWeapon)
        {
            // 创建阿茨冈德
            ThingDef atzgandDef = DefDatabase<ThingDef>.GetNamed("Sideria_Weapon_Atzgand");
            ThingWithComps atzgand = (ThingWithComps)ThingMaker.MakeThing(atzgandDef, null);
            
            // 设置为传奇品质
            CompQuality qualityComp = atzgand.TryGetComp<CompQuality>();
            if (qualityComp != null)
            {
                qualityComp.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Colony);
            }

            // 应用生物编码
            CompBiocodable biocodableComp = atzgand.TryGetComp<CompBiocodable>();
            if (biocodableComp != null)
            {
                biocodableComp.CodeFor(pawn);
            }

            // 设置血原质
            atzgand.HitPoints = atzgand.MaxHitPoints;

            // 销毁旧武器
            if (!oldWeapon.Destroyed)
            {
                oldWeapon.Destroy(DestroyMode.Vanish);
            }

            // 发送转化消息
            Messages.Message(
                "Sideria_AtzgandTransformation".Translate(pawn.LabelShort),
                pawn,
                MessageTypeDefOf.PositiveEvent
            );

            // 添加特效（可选）
            // FleckMaker.ThrowSmoke(pawn.DrawPos, pawn.Map, 1f);

            return atzgand;
        }
    }

    public class CompProperties_AtzgandTransformer : CompProperties
    {
        public CompProperties_AtzgandTransformer()
        {
            compClass = typeof(CompAtzgandTransformer);
        }
    }
}
```

##### 2. HarmonyPatches.cs
```csharp
using HarmonyLib;
using Verse;
using RimWorld;

namespace Sideria.Atzgand
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("com.sideria.atzgand");
            harmony.PatchAll();
        }
    }

    /// <summary>
    /// 监听装备武器事件
    /// </summary>
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "AddEquipment")]
    public static class Patch_Pawn_EquipmentTracker_AddEquipment
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_EquipmentTracker __instance, ThingWithComps newEq)
        {
            Pawn pawn = __instance.pawn;
            
            // 只处理希德莉亚
            if (pawn?.def?.defName != "Sideria_BloodThornKnight")
                return;

            // 只处理武器
            if (newEq?.def?.IsWeapon != true)
                return;

            // 已经是阿茨冈德，跳过
            if (newEq.def.defName == "Sideria_Weapon_Atzgand")
                return;

            // 检查是否可以转化
            if (AtzgandUtility.CanTransformToAtzgand(pawn, newEq))
            {
                // 延迟执行转化（避免在装备过程中修改）
                Find.TickManager.CurTimeSpeed = TimeSpeed.Paused;
                
                // 移除旧武器
                __instance.Remove(newEq);
                
                // 创建并装备阿茨冈德
                ThingWithComps atzgand = AtzgandUtility.CreateAtzgand(pawn, newEq);
                __instance.AddEquipment(atzgand);
                
                Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
            }
        }
    }
}
```

##### 3. AtzgandUtility.cs
```csharp
using Verse;
using RimWorld;

namespace Sideria.Atzgand
{
    /// <summary>
    /// 阿茨冈德工具类
    /// </summary>
    public static class AtzgandUtility
    {
        /// <summary>
        /// 检查是否可以转化为阿茨冈德
        /// </summary>
        public static bool CanTransformToAtzgand(Pawn pawn, ThingWithComps weapon)
        {
            // 必须是希德莉亚
            if (pawn?.def?.defName != "Sideria_BloodThornKnight")
                return false;

            // 必须是武器
            if (weapon?.def?.IsWeapon != true)
                return false;

            // 已经是阿茨冈德
            if (weapon.def.defName == "Sideria_Weapon_Atzgand")
                return false;

            // 必须是传奇品质
            QualityCategory quality;
            if (!weapon.TryGetQuality(out quality))
                return false;

            if (quality != QualityCategory.Legendary)
                return false;

            // 必须是长剑类型
            if (IsLongsword(weapon))
                return true;

            return false;
        }

        /// <summary>
        /// 判断是否是长剑
        /// </summary>
        private static bool IsLongsword(ThingWithComps weapon)
        {
            // 方法1：检查defName
            if (weapon.def.defName.ToLower().Contains("longsword"))
                return true;

            // 方法2：检查weaponTags
            if (weapon.def.weaponTags?.Contains("MedievalMeleeAdvanced") == true)
                return true;

            // 方法3：检查weaponClasses
            if (weapon.def.weaponClasses?.Contains(WeaponClassDefOf.Melee) == true)
                return true;

            // 可以添加更多判断条件
            // 例如：伤害类型、攻击速度等

            return false;
        }

        /// <summary>
        /// 创建阿茨冈德
        /// </summary>
        public static ThingWithComps CreateAtzgand(Pawn pawn, ThingWithComps sourceWeapon = null)
        {
            // 获取阿茨冈德定义
            ThingDef atzgandDef = DefDatabase<ThingDef>.GetNamed("Sideria_Weapon_Atzgand");
            if (atzgandDef == null)
            {
                Log.Error("[Sideria] Cannot find Atzgand ThingDef!");
                return null;
            }

            // 创建阿茨冈德
            ThingWithComps atzgand = (ThingWithComps)ThingMaker.MakeThing(atzgandDef, null);

            // 设置品质为传奇
            CompQuality qualityComp = atzgand.TryGetComp<CompQuality>();
            if (qualityComp != null)
            {
                qualityComp.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Colony);
            }

            // 应用生物编码
            CompBiocodable biocodableComp = atzgand.TryGetComp<CompBiocodable>();
            if (biocodableComp != null)
            {
                biocodableComp.CodeFor(pawn);
            }

            // 设置满血
            atzgand.HitPoints = atzgand.MaxHitPoints;

            // 继承原武器的一些属性（可选）
            if (sourceWeapon != null)
            {
                // 可以继承艺术描述、制作者等信息
                CompArt artComp = atzgand.TryGetComp<CompArt>();
                CompArt sourceArtComp = sourceWeapon.TryGetComp<CompArt>();
                if (artComp != null && sourceArtComp != null)
                {
                    // 保留原武器的艺术信息
                }
            }

            // 发送消息
            SendTransformationMessage(pawn);

            // 播放音效（可选）
            // SoundDefOf.PsycastPsychicEffect.PlayOneShot(pawn);

            return atzgand;
        }

        /// <summary>
        /// 发送转化消息
        /// </summary>
        private static void SendTransformationMessage(Pawn pawn)
        {
            string messageKey = "Sideria_AtzgandTransformation";
            string message = messageKey.Translate(pawn.LabelShort);
            
            Messages.Message(
                message,
                pawn,
                MessageTypeDefOf.PositiveEvent,
                historical: true
            );
        }
    }
}
```

---

## ?? 翻译文件添加

### Languages/English/Keyed/Messages.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<LanguageData>
    <Sideria_AtzgandTransformation>{0}'s blade resonates with blood essence and transforms into Atzgand!</Sideria_AtzgandTransformation>
</LanguageData>
```

### Languages/ChineseSimplified/Keyed/Messages.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<LanguageData>
    <Sideria_AtzgandTransformation>{0}的剑刃与血原质产生共鸣，转化为阿茨冈德！</Sideria_AtzgandTransformation>
</LanguageData>
```

---

## ?? 项目配置

### Sideria.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
    <AssemblyName>Sideria.Atzgand</AssemblyName>
    <RootNamespace>Sideria.Atzgand</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="Assembly-CSharp">
      <HintPath>D:\steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="UnityEngine">
      <HintPath>D:\steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="UnityEngine.CoreModule">
      <HintPath>D:\steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.CoreModule.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="0Harmony">
      <HintPath>C:\Users\Administrator\Desktop\rim mod\0Harmony.dll</HintPath>
      <Private>True</Private>
    </Reference>
  </ItemGroup>
</Project>
```

---

## ?? 方案2：使用场景脚本（简化版）

如果不想写C#代码，可以使用开局场景脚本：

### Scenarios_Sideria.xml（修改部分）
```xml
<!-- 开局强制装备阿茨冈德 -->
<li Class="ScenPart_StartingThing_Defined">
    <def>StartingThing_Defined</def>
    <thingDef>Sideria_Weapon_Atzgand</thingDef>
    <count>1</count>
    <quality>Legendary</quality>
</li>

<!-- 开局执行生物编码 -->
<!-- 注意：这个需要自定义ScenPart -->
```

---

## ?? 测试方案

### 测试步骤
1. 编译DLL放入Assemblies文件夹
2. 启动游戏并启用模组
3. 使用开发模式创建希德莉亚
4. 生成传奇长剑
5. 让希德莉亚装备传奇长剑
6. 观察是否转化为阿茨冈德
7. 检查生物编码是否正确应用

### 调试命令
```
开发模式控制台命令：
- 生成武器：Thing.Make MeleeWeapon_LongSword Legendary
- 查看装备：Pawn.Equipment
- 测试转化：AtzgandUtility.TestTransform
```

---

## ?? 注意事项

### 已知限制
1. **原版限制**：生物编码是原版功能，兼容性好
2. **性能考虑**：装备事件频繁触发，需要优化判断
3. **存档兼容**：添加/删除mod时可能影响已有阿茨冈德

### 边界情况处理
```csharp
// 1. 希德莉亚死亡后阿茨冈德掉落
// 解决：保持生物编码，其他人无法使用

// 2. 希德莉亚失去意识/精神崩溃
// 解决：阿茨冈德自动掉落（原版行为）

// 3. 多把传奇长剑快速切换
// 解决：添加冷却时间或只保留一把阿茨冈德

// 4. 存档中已有的传奇长剑
// 解决：在装备时检测并转化
```

---

## ?? 参考资料

### RimWorld API
- `Pawn_EquipmentTracker` - 装备管理
- `CompBiocodable` - 生物编码组件
- `CompQuality` - 品质组件
- `ThingMaker` - 物品创建

### Harmony文档
- https://harmony.pardeike.net/
- Prefix/Postfix补丁
- 事件监听

---

## ? 实现优先级

### 必需功能
- [x] XML定义阿茨冈德
- [x] 原版生物编码支持
- [ ] C#转化逻辑（可选）

### 推荐功能
- [ ] 装备转化机制
- [ ] 转化特效
- [ ] 转化音效

### 可选功能
- [ ] 武器升级系统
- [ ] 血原质强化
- [ ] 专属技能

---

**当前状态**：XML版本已完成，阿茨冈德可直接装备
**下一步**：根据需要实现C#转化逻辑
