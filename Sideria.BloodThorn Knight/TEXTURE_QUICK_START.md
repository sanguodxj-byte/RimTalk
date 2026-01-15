# ?? 贴图部署快速指南

## ? 3步快速开始

### 第1步：创建文件夹（5秒）
```batch
右键管理员运行 → CreateTextureStructure.bat
```

### 第2步：放入贴图文件（看你速度）
```
拖拽贴图到对应文件夹
```

### 第3步：部署到RimWorld（10秒）
```batch
右键管理员运行 → DeployWithSymlink.bat
```

**完成！启动游戏测试！** ??

---

## ?? 必需文件夹位置

```
你的mod文件夹\
└── Textures\
    ├── Things\Pawn\Humanlike\Bodies\Dracovampir\    ← 6个PNG
    ├── Things\Pawn\Humanlike\BodyAddons\            ← 12个PNG
    ├── Things\Item\Equipment\WeaponMelee\           ← 3个PNG
    └── UI\Abilities\                                ← 8个PNG
```

---

## ?? 最小可用配置

### 只想先测试？只需这6个文件！
```
Bodies/Dracovampir/
  ├── Naked_Male_south.png    (必需)
  ├── Naked_Male_east.png     (必需)
  ├── Naked_Male_north.png    (必需)
  ├── Naked_Female_south.png  (必需)
  ├── Naked_Female_east.png   (必需)
  └── Naked_Female_north.png  (必需)
```

**其他文件缺失只会显示粉红色，不影响游戏！**

---

## ??? 符号链接说明

### 什么是符号链接？
- Windows的"快捷方式增强版"
- 修改源文件 = 自动同步到RimWorld
- 不需要重复复制文件

### 为什么需要管理员权限？
- Windows创建符号链接需要管理员权限
- 只在第一次部署时需要
- 后续修改文件不需要权限

### 优势
- ? 实时同步
- ? 节省空间
- ? 便于调试
- ? 一次设置永久有效

---

## ?? 贴图优先级

### 【必需】优先级1 - 身体 (6个)
没有这些 = 粉红色角色

### 【高优先】优先级2 - Body Addons (12个)
没有这些 = 没有龙角/龙翼装饰

### 【中优先】优先级3 - 武器 (3个)
没有这些 = 粉红色武器

### 【低优先】优先级4 - 图标 (8个)
没有这些 = 粉红色图标（技能还能用）

---

## ?? 常见问题

### Q: 需要所有贴图才能玩吗？
**A**: 不需要！缺失的贴图显示粉红色，功能完全正常。

### Q: 我没有贴图制作经验怎么办？
**A**: 
1. 先用mod（粉红色）
2. 慢慢学习制作
3. 参考 `AI_GENERATION_GUIDE.md` 用AI生成

### Q: 修改贴图后需要重新部署吗？
**A**: 不需要！符号链接会自动同步。只需重启游戏。

### Q: 符号链接失败怎么办？
**A**: 
1. 确认以管理员身份运行
2. 使用 `QuickDeploy.bat`（传统复制方式）

---

## ?? 检查是否成功

### 方法1：运行检查脚本
```batch
CheckTextures.bat
```

### 方法2：手动检查
1. 打开RimWorld Mods文件夹
2. 找到 `Sideria.BloodThorn Knight\Textures\`
3. 查看是否有箭头图标（符号链接标志）

### 方法3：游戏内检查
1. 启动RimWorld
2. 开发模式生成角色
3. 看看是不是粉红色

---

## ?? 完整文档

如需详细信息，请查看：

- **完整清单**: `TEXTURE_FILE_LIST.md`
- **制作指南**: `TEXTURE_GUIDE_CORRECT.md`
- **Body Addons**: `BODY_ADDONS_GUIDE.md`
- **武器美术**: `ATZGAND_ART_GUIDE.md`

---

## ?? 现在就开始！

```batch
第1步: CreateTextureStructure.bat      ← 右键管理员运行
第2步: 放入你的贴图文件                 ← 拖拽到对应文件夹
第3步: DeployWithSymlink.bat            ← 右键管理员运行
第4步: 启动RimWorld测试！               ← 享受游戏！
```

**总耗时: < 1分钟** ?

---

**祝你玩得开心！** ???
