# ?? HAR Body Addons 位置控制指南

## 什么是Body Addons？

Body Addons是HAR提供的**图层叠加系统**，允许在基础身体上添加额外的部位，如：
- 龙角、龙翼、龙尾
- 血纹、发光效果
- 装甲部件、机械肢体
- 任何需要独立控制的视觉元素

---

## ?? 坐标系统

### RimWorld坐标系

```
      North (0, +Y)
           ↑
           |
West ←-----+----→ East
(-X, 0)    |    (+X, 0)
           |
           ↓
      South (0, -Y)
```

### 方向说明

```
South (朝下): 正面视图
East (朝右): 右侧视图
North (朝上): 背面视图
West (朝左): 自动镜像East
```

---

## ?? Body Addon 完整配置参数

### 基础结构

```xml
<li>
  <!-- 路径和外观 -->
  <path>贴图路径（不含_south等后缀）</path>
  <colorChannel>使用哪个颜色通道</colorChannel>
  <drawSize>绘制大小</drawSize>
  
  <!-- 层级控制 -->
  <inFrontOfBody>是否在身体前面</inFrontOfBody>
  <layerOffset>Z轴层级偏移</layerOffset>
  
  <!-- 对齐方式 -->
  <alignWithHead>是否跟随头部</alignWithHead>
  <bodyPart>绑定到特定身体部位</bodyPart>
  
  <!-- 显示条件 -->
  <drawForMale>男性显示</drawForMale>
  <drawForFemale>女性显示</drawForFemale>
  <hediffGraphics>基于Hediff显示</hediffGraphics>
  
  <!-- 位置偏移 -->
  <offsets>
    <south><offset>(X, Y)</offset></south>
    <north><offset>(X, Y)</offset></north>
    <east><offset>(X, Y)</offset></east>
  </offsets>
  
  <!-- 隐藏条件 -->
  <hiddenUnderApparelTag>
    <li>UpperBody</li>
  </hiddenUnderApparelTag>
</li>
```

---

## ?? 参数详解

### 1. path - 贴图路径

```xml
<path>Things/Pawn/Humanlike/BodyAddons/DragonWings</path>
```

**对应文件**：
- `DragonWings_south.png`
- `DragonWings_east.png`
- `DragonWings_north.png`

**注意**：路径中**不包含**方向后缀！

---

### 2. colorChannel - 颜色通道

```xml
<colorChannel>skin</colorChannel>
```

**可选值**：
- `skin` - 使用肤色
- `hair` - 使用发色
- `null` - 不着色（使用贴图原色）

**对应配置**：
```xml
<colorChannels>
  <li>
    <name>skin</name>  <!-- 这里定义的名字 -->
    <first Class="ColorGenerator_Options">...</first>
  </li>
</colorChannels>
```

---

### 3. drawSize - 绘制大小

```xml
<drawSize>1.5</drawSize>
```

**含义**：
- `1.0` = 与身体相同大小
- `1.5` = 1.5倍大小（适合翅膀）
- `0.8` = 0.8倍大小（适合小装饰）

---

### 4. inFrontOfBody - 前后层级

```xml
<inFrontOfBody>false</inFrontOfBody>
```

**说明**：
- `true` = 绘制在身体前面（如前胸装甲）
- `false` = 绘制在身体后面（如翅膀、背包）

---

### 5. layerOffset - 精确层级

```xml
<layerOffset>0.02</layerOffset>
```

**说明**：
- 正值 = 更靠前（离摄像机近）
- 负值 = 更靠后
- 范围：通常 -0.1 到 0.1

**示例**：
```
背景 (-0.1) → 翅膀 (-0.05) → 身体 (0) → 血纹 (0.01) → 龙角 (0.02)
```

---

### 6. alignWithHead - 头部对齐

```xml
<alignWithHead>true</alignWithHead>
```

**说明**：
- `true` = 跟随头部旋转（适合龙角、头饰）
- `false` = 固定在身体（适合翅膀、尾巴）

---

### 7. offsets - 位置偏移 ?

```xml
<offsets>
  <south>
    <offset>(0, 0.35)</offset>  <!-- X右移, Y上移 -->
  </south>
  <north>
    <offset>(0, 0.35)</offset>
  </north>
  <east>
    <offset>(-0.2, -0.1)</offset>  <!-- X左移, Y下移 -->
  </east>
</offsets>
```

**坐标说明**：
```
       +Y (上移)
        ↑
        |
-X ←----+----→ +X
(左移)  |  (右移)
        |
        ↓
       -Y (下移)
```

**实例**：
```xml
<!-- 龙角：头顶上方 -->
<offset>(0, 0.35)</offset>

<!-- 翅膀：背部偏下 -->
<south><offset>(0, -0.1)</offset></south>
<east><offset>(-0.2, -0.1)</offset></east>

<!-- 尾巴：身体下方 -->
<offset>(0, -0.5)</offset>
```

---

### 8. hiddenUnderApparelTag - 装备遮挡

```xml
<hiddenUnderApparelTag>
  <li>UpperBody</li>
  <li>Shell</li>
</hiddenUnderApparelTag>
```

**说明**：
- 穿上指定标签的装备时，隐藏该addon
- 适合翅膀（被盔甲遮挡）

**常用标签**：
```xml
UpperBody     上身装备
Shell         外套
FullBody      全身装备
Head          头部装备
```

---

## ?? 实际案例

### 案例1：龙角（跟随头部）

```xml
<li>
  <path>Things/Pawn/Humanlike/BodyAddons/DragonHorns</path>
  <colorChannel>skin</colorChannel>
  <drawSize>1.0</drawSize>
  <inFrontOfBody>false</inFrontOfBody>
  <alignWithHead>true</alignWithHead>  <!-- 关键：跟随头部 -->
  <layerOffset>0.01</layerOffset>
  
  <offsets>
    <south>
      <offset>(0, 0.35)</offset>  <!-- 头顶上方 -->
    </south>
    <north>
      <offset>(0, 0.35)</offset>
    </north>
    <east>
      <offset>(0, 0.35)</offset>
    </east>
  </offsets>
</li>
```

**贴图要求**：
- `DragonHorns_south.png` (512x512)
- `DragonHorns_east.png` (512x512)
- `DragonHorns_north.png` (512x512)

---

### 案例2：龙翼（背后大型）

```xml
<li>
  <path>Things/Pawn/Humanlike/BodyAddons/DragonWings</path>
  <colorChannel>skin</colorChannel>
  <drawSize>1.5</drawSize>  <!-- 1.5倍大 -->
  <inFrontOfBody>false</inFrontOfBody>  <!-- 身体后面 -->
  <alignWithHead>false</alignWithHead>  <!-- 不跟随头部 -->
  <layerOffset>-0.1</layerOffset>  <!-- 最后面 -->
  
  <offsets>
    <south>
      <offset>(0, -0.1)</offset>  <!-- 略低于肩部 -->
    </south>
    <north>
      <offset>(0, -0.1)</offset>
    </north>
    <east>
      <offset>(-0.2, -0.1)</offset>  <!-- 侧面稍后 -->
    </east>
  </offsets>
  
  <!-- 穿上衣服时隐藏 -->
  <hiddenUnderApparelTag>
    <li>UpperBody</li>
    <li>Shell</li>
  </hiddenUnderApparelTag>
</li>
```

---

### 案例3：血纹（发光叠加）

```xml
<li>
  <path>Things/Pawn/Humanlike/BodyAddons/BloodMarkings</path>
  <colorChannel>hair</colorChannel>  <!-- 使用血红色 -->
  <drawSize>1.0</drawSize>
  <inFrontOfBody>true</inFrontOfBody>  <!-- 身体前面 -->
  <alignWithHead>false</alignWithHead>
  <layerOffset>0.02</layerOffset>  <!-- 最前面 -->
  
  <offsets>
    <south><offset>(0, 0)</offset></south>  <!-- 完全对齐身体 -->
    <north><offset>(0, 0)</offset></north>
    <east><offset>(0, 0)</offset></east>
  </offsets>
  
  <!-- 基于Hediff动态显示 -->
  <hediffGraphics>
    <Sideria_BloodEssence>  <!-- 血原质充盈时显示 -->
      <severity>0.8</severity>  <!-- 80%以上才显示 -->
    </Sideria_BloodEssence>
  </hediffGraphics>
</li>
```

---

### 案例4：龙尾（动态摆动）

```xml
<li>
  <path>Things/Pawn/Humanlike/BodyAddons/DragonTail</path>
  <colorChannel>skin</colorChannel>
  <drawSize>1.2</drawSize>
  <inFrontOfBody>false</inFrontOfBody>
  <alignWithHead>false</alignWithHead>
  <layerOffset>-0.05</layerOffset>
  
  <offsets>
    <south>
      <offset>(0, -0.5)</offset>  <!-- 身体下方 -->
    </south>
    <north>
      <offset>(0, -0.5)</offset>
    </north>
    <east>
      <offset>(-0.3, -0.5)</offset>  <!-- 侧面偏后 -->
    </east>
  </offsets>
</li>
```

---

## ?? 调试技巧

### 1. 快速定位偏移量

**步骤**：
1. 先设置 `<offset>(0, 0)</offset>`
2. 进入游戏查看addon位置
3. 根据需要调整：
   - 太低？增加Y值：`(0, 0.2)`
   - 太右？减少X值：`(-0.1, 0)`
   - 太前？减少layerOffset
4. 每次调整0.05-0.1增量

### 2. 常用偏移参考

```xml
<!-- 头部装饰 -->
<offset>(0, 0.3 to 0.4)</offset>

<!-- 肩部翅膀 -->
<offset>(0, 0 to -0.1)</offset>

<!-- 背部装备 -->
<offset>(-0.1 to -0.2, -0.1)</offset>

<!-- 腰部装饰 -->
<offset>(0, -0.2 to -0.3)</offset>

<!-- 尾巴 -->
<offset>(0, -0.4 to -0.6)</offset>
```

### 3. 层级Z-fighting问题

如果两个addon重叠闪烁：
```xml
<!-- 确保layerOffset不同 -->
<li><!-- 后层 --><layerOffset>-0.05</layerOffset></li>
<li><!-- 前层 --><layerOffset>-0.04</layerOffset></li>
```

---

## ?? 贴图文件组织

### 目录结构

```
Textures/
└── Things/
    └── Pawn/
        └── Humanlike/
            ├── Bodies/
            │   └── Dracovampir/
            │       └── Naked_Male_xxx.png
            │
            └── BodyAddons/
                ├── DragonHorns_south.png      (512x512)
                ├── DragonHorns_east.png       (512x512)
                ├── DragonHorns_north.png      (512x512)
                │
                ├── DragonWings_south.png      (512x512或更大)
                ├── DragonWings_east.png
                ├── DragonWings_north.png
                │
                ├── DragonTail_south.png       (512x512)
                ├── DragonTail_east.png
                ├── DragonTail_north.png
                │
                ├── BloodMarkings_south.png    (512x512)
                ├── BloodMarkings_east.png
                └── BloodMarkings_north.png
```

---

## ?? 贴图制作要点

### 透明度处理

```
1. 基础身体：完全不透明
2. 龙角：透明背景，只有角本体
3. 翅膀：透明背景，翅膀可有半透明
4. 血纹：使用Alpha通道，发光效果
```

### 对齐参考

创建贴图时，以512x512画布为基准：
```
画布中心 (256, 256) = 角色身体中心
上半部分 = 头部、龙角区域
下半部分 = 躯干、腿部、尾巴区域
```

---

## ? 快速部署清单

### 1. 创建Body Addon贴图

```powershell
# 创建BodyAddons文件夹
New-Item -ItemType Directory -Path "Textures\Things\Pawn\Humanlike\BodyAddons" -Force
```

### 2. 放入贴图文件

```
DragonHorns_south/east/north.png  (512x512)
DragonWings_south/east/north.png  (512x512)
DragonTail_south/east/north.png   (512x512)
BloodMarkings_south/east/north.png (512x512)
```

### 3. 配置XML

已在 `Races_Sideria.xml` 中添加！

### 4. 重新部署

```powershell
.\QuickDeploy.bat
```

### 5. 测试

```
1. 启动RimWorld
2. 生成血龙种角色
3. 检查各个addon是否正确显示
4. 调整offset直到满意
```

---

## ?? 常见问题

### Q: Addon不显示？

**检查**：
1. 贴图文件名是否正确（包含方向后缀）
2. path是否正确（不含后缀）
3. drawForMale/Female是否启用
4. 是否被hiddenUnderApparelTag隐藏

### Q: Addon位置不对？

**解决**：
```xml
<!-- 调整offsets -->
<offset>(X左右, Y上下)</offset>

<!-- 每次调整0.05-0.1 -->
```

### Q: Addon被身体遮挡？

**解决**：
```xml
<!-- 增加layerOffset -->
<layerOffset>0.02</layerOffset>  <!-- 更大的正值 -->
<inFrontOfBody>true</inFrontOfBody>
```

### Q: 不同方向偏移不一致？

**正常现象**！不同视角需要不同偏移：
```xml
<south><offset>(0, 0.3)</offset></south>   <!-- 正面 -->
<east><offset>(-0.2, 0.3)</offset></east>  <!-- 侧面要偏后 -->
<north><offset>(0, 0.3)</offset></north>   <!-- 背面 -->
```

---

## ?? 完整配置示例

```xml
<bodyAddons>
  <!-- 龙角 -->
  <li>
    <path>Things/Pawn/Humanlike/BodyAddons/DragonHorns</path>
    <colorChannel>skin</colorChannel>
    <drawSize>1.0</drawSize>
    <inFrontOfBody>false</inFrontOfBody>
    <alignWithHead>true</alignWithHead>
    <drawForMale>true</drawForMale>
    <drawForFemale>true</drawForFemale>
    <layerOffset>0.01</layerOffset>
    <offsets>
      <south><offset>(0, 0.35)</offset></south>
      <north><offset>(0, 0.35)</offset></north>
      <east><offset>(0, 0.35)</offset></east>
    </offsets>
  </li>
  
  <!-- 更多addon... -->
</bodyAddons>
```

---

**准备好贴图文件了吗？现在可以开始配置位置了！** ?
