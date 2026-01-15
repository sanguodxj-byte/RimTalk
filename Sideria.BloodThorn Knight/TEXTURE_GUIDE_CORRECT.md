# ?? 贴图部署指南（正确版本 - 512x512）

## ? 正确的贴图尺寸

根据HAR社区标准和实际mod实践：

| 贴图类型 | 推荐尺寸 | 说明 |
|---------|---------|------|
| **身体贴图** | 512x512 | HAR标准，通过drawSize自动缩放 |
| **武器贴图** | 256x256 或 512x512 | 高质量武器可用512 |
| **UI图标** | 64x64 | 技能/buff图标 |

**RimWorld会通过`drawSize`自动缩放，无需手动缩小！**

---

## ?? 文件夹结构

### 完整的贴图目录

```
Textures/
├── Things/
│   ├── Pawn/
│   │   └── Humanlike/
│   │       ├── Bodies/
│   │       │   └── Dracovampir/         # 血龙种身体贴图
│   │       │       ├── Naked_Male_south.png     (512x512)
│   │       │       ├── Naked_Male_east.png      (512x512)
│   │       │       ├── Naked_Male_north.png     (512x512)
│   │       │       ├── Naked_Female_south.png   (512x512)
│   │       │       ├── Naked_Female_east.png    (512x512)
│   │       │       └── Naked_Female_north.png   (512x512)
│   │       │
│   │       └── Heads/                   # 可选：自定义头部
│   │           ├── Male/
│   │           │   └── (各种头部变体 512x512)
│   │           └── Female/
│   │               └── (各种头部变体 512x512)
│   │
│   └── Item/
│       └── Equipment/
│           └── WeaponMelee/
│               ├── Atzgand.png              (256x256 或 512x512)
│               ├── Atzgand_Ascended.png     (256x256 或 512x512)
│               └── BloodDagger.png          (256x256)
│
└── UI/
    └── Abilities/
        ├── DragonBreath.png         (64x64)
        ├── DragonicAura.png         (64x64)
        ├── DragonWings.png          (64x64)
        ├── DragonicTransformation.png (64x64)
        ├── BloodDrain.png           (64x64)
        ├── VampiricEmbrace.png      (64x64)
        ├── BloodFrenzy.png          (64x64)
        └── OathbreakerTransformation.png (64x64)
```

---

## ?? 部署步骤

### 1. 创建文件夹结构

运行此PowerShell命令：

```powershell
# 创建所有需要的文件夹
$folders = @(
    "Textures\Things\Pawn\Humanlike\Bodies\Dracovampir",
    "Textures\Things\Pawn\Humanlike\Heads\Male",
    "Textures\Things\Pawn\Humanlike\Heads\Female",
    "Textures\Things\Item\Equipment\WeaponMelee",
    "Textures\UI\Abilities"
)

foreach ($folder in $folders) {
    New-Item -ItemType Directory -Path $folder -Force | Out-Null
    Write-Host "? 创建: $folder" -ForegroundColor Green
}

Write-Host ""
Write-Host "文件夹结构创建完成！" -ForegroundColor Cyan
```

### 2. 放置贴图文件

#### 必需的身体贴图（最高优先级）

**位置**: `Textures/Things/Pawn/Humanlike/Bodies/Dracovampir/`

**文件列表**：
```
Naked_Male_south.png      # 男性朝下（正面）
Naked_Male_east.png       # 男性朝右（侧面）
Naked_Male_north.png      # 男性朝上（背面）
Naked_Female_south.png    # 女性朝下
Naked_Female_east.png     # 女性朝右
Naked_Female_north.png    # 女性朝上
```

**要求**：
- 尺寸：512x512像素
- 格式：PNG，透明背景
- 命名：严格按照上述格式

#### 武器贴图（中等优先级）

**位置**: `Textures/Things/Item/Equipment/WeaponMelee/`

**文件列表**：
```
Atzgand.png              # 阿茨冈德（传奇长剑）
Atzgand_Ascended.png     # 升华版阿茨冈德
BloodDagger.png          # 血刃短剑
```

**要求**：
- 尺寸：256x256 或 512x512
- 格式：PNG，透明背景
- 单图（无方向后缀）

#### UI图标（低优先级）

**位置**: `Textures/UI/Abilities/`

**要求**：
- 尺寸：64x64像素
- 格式：PNG
- 可以有背景色

---

## ?? 验证贴图路径

### 当前XML配置的路径

在 `Races_Sideria.xml` 中：

```xml
<graphicPaths>
  <li>
    <body>Things/Pawn/Humanlike/Bodies/Dracovampir/Naked_Male</body>
  </li>
  <li>
    <body>Things/Pawn/Humanlike/Bodies/Dracovampir/Naked_Female</body>
  </li>
</graphicPaths>
```

**对应的文件**：
- `Textures/Things/Pawn/Humanlike/Bodies/Dracovampir/Naked_Male_south.png`
- `Textures/Things/Pawn/Humanlike/Bodies/Dracovampir/Naked_Male_east.png`
- `Textures/Things/Pawn/Humanlike/Bodies/Dracovampir/Naked_Male_north.png`
- 同理女性版本

---

## ?? 贴图制作建议

### 身体贴图

**AI生成提示词示例**：
```
A 512x512 pixel transparent PNG character sprite, 
draconic vampire humanoid body, pale red skin, 
muscular build, blood-red markings, 
[south/east/north] facing view, 
anime/manga style, clear lines, no background
```

### 三视图要求

```
South (正面):
- 完整的正面视图
- 显示面部细节
- 双手自然下垂

East (侧面):
- 完整的右侧面
- 显示轮廓
- 手臂贴身

North (背面):
- 完整的背面视图
- 显示后脑勺
- 双手自然下垂
```

### 颜色方案

根据你的配置：
```xml
<alienskincolorgen>
  <min>(0.88, 0.78, 0.78)</min>  <!-- 浅红白色 -->
  <max>(0.96, 0.88, 0.88)</max>  <!-- 苍白色 -->
</alienskincolorgen>
```

**转换为RGB**：
- 主色调：RGB(224, 199, 199) - 苍白带红
- 阴影：RGB(167, 20, 20) - 深红色
- 发光效果：RGB(200, 50, 80) - 血红色

---

## ? 快速部署命令

创建文件夹并检查：

```powershell
# 一键创建所有文件夹
New-Item -ItemType Directory -Path "Textures\Things\Pawn\Humanlike\Bodies\Dracovampir" -Force
New-Item -ItemType Directory -Path "Textures\Things\Item\Equipment\WeaponMelee" -Force
New-Item -ItemType Directory -Path "Textures\UI\Abilities" -Force

# 检查文件夹是否存在
Get-ChildItem -Path "Textures" -Recurse -Directory | Select-Object FullName
```

复制贴图后重新部署：

```powershell
# 重新部署到RimWorld
.\QuickDeploy.bat
```

---

## ?? 常见问题

### Q: 为什么用512x512而不是128x128？

**A**: 
1. HAR标准：社区mod（如柯莉姆）都用512x512
2. 自动缩放：RimWorld通过`drawSize`自动缩放
3. 高质量：512x512保证清晰度和细节
4. 兼容性：Facial Animation等mod需要高分辨率

### Q: 如果没有自定义贴图会怎样？

**A**: 
- 显示为**粉红色占位符**
- 不影响游戏功能
- 所有属性和机制正常工作

### Q: drawSize如何设置？

**A**: 
```xml
<!-- 当前设置 -->
<customDrawSize>(1.02, 1.02)</customDrawSize>  <!-- HAR控制 -->
<graphicData>
  <drawSize>0.92</drawSize>  <!-- ThingDef控制 -->
</graphicData>

<!-- 数值含义 -->
1.0 = 标准人类大小
0.92 = 略小（你的血龙种）
1.02 = 略大（HAR额外缩放）
```

### Q: 贴图必须是透明背景吗？

**A**: 
- **身体贴图**：必须透明，否则会有白框
- **武器贴图**：必须透明
- **UI图标**：可以有背景

---

## ?? 检查清单

### 部署前

- [ ] 创建了Textures文件夹结构
- [ ] 准备了512x512的身体贴图（3方向 x 2性别）
- [ ] 准备了武器贴图（256或512）
- [ ] （可选）准备了UI图标（64x64）

### 部署后

- [ ] 运行QuickDeploy.bat
- [ ] 启动RimWorld
- [ ] 开发模式生成角色
- [ ] 检查贴图是否正常显示
- [ ] 测试三个方向的显示

---

## ?? 立即行动

1. **如果有512x512贴图**：
   ```powershell
   # 创建文件夹
   New-Item -ItemType Directory -Path "Textures\Things\Pawn\Humanlike\Bodies\Dracovampir" -Force
   
   # 复制贴图进去
   # 重新部署
   .\QuickDeploy.bat
   ```

2. **如果没有贴图**：
   - 当前mod完全可用（粉红色占位符）
   - 慢慢制作贴图
   - 随时可以添加

---

**总结：使用512x512贴图，让RimWorld自动缩放！** ?
