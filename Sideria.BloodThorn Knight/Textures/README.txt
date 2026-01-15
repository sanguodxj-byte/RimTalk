贴图文件夹说明：

需要的贴图文件：

1. About/Preview.png (640x360) - Steam工作坊预览图

2. Things/Pawn/Humanlike/Bodies/
   - Sideria_Body_Fat_north.png
   - Sideria_Body_Fat_south.png
   - Sideria_Body_Fat_east.png
   - Sideria_Body_Hulk_north.png
   - Sideria_Body_Hulk_south.png
   - Sideria_Body_Hulk_east.png
   - Sideria_Body_Thin_north.png
   - Sideria_Body_Thin_south.png
   - Sideria_Body_Thin_east.png

3. Things/Pawn/Humanlike/Heads/
   - Sideria_Head_01.png (各种发型和面部)

4. UI/Icons/
   - Sideria_Icon.png (角色图标)

===========================================
血龙种（Dracovampir）贴图文件夹
Texture Folder for Dracovampir Race
===========================================

本文件夹用于存放血龙种的所有贴图资源。

?? 文件夹结构
===========================================

Things/
└── Pawn/
    └── Humanlike/
        ├── Bodies/           # 身体贴图
        │   ├── Naked_Dracovampir_south.png  (背面, 128x128)
        │   ├── Naked_Dracovampir_north.png  (正面, 128x128)
        │   └── Naked_Dracovampir_east.png   (侧面, 128x128)
        │
        ├── Addons/           # 附加部件（可选）
        │   ├── Dracovampir_Horn_south.png   (龙角-背面)
        │   ├── Dracovampir_Horn_north.png   (龙角-正面)
        │   ├── Dracovampir_Horn_east.png    (龙角-侧面)
        │   ├── Dracovampir_Tail_south.png   (尾巴-背面)
        │   ├── Dracovampir_Tail_north.png   (尾巴-正面)
        │   ├── Dracovampir_Tail_east.png    (尾巴-侧面)
        │   ├── Dracovampir_Wings_south.png  (翅膀-背面)
        │   ├── Dracovampir_Wings_north.png  (翅膀-正面)
        │   └── Dracovampir_Wings_east.png   (翅膀-侧面)
        │
        └── Apparel/          # 装备贴图（可选）
            └── BloodThornHood/
                ├── BloodThornHood_south.png  (头巾-背面)
                ├── BloodThornHood_north.png  (头巾-正面)
                └── BloodThornHood_east.png   (头巾-侧面)


?? 贴图规格要求
===========================================

文件格式：PNG (RGBA，带透明通道)
推荐尺寸：128x128 像素
原始尺寸：512x512 像素 (保留备份)
视角要求：45度俯视角
方向说明：
  - south = 背面（角色背对屏幕）
  - north = 正面（角色面向屏幕）
  - east  = 侧面（角色向右）


?? 快速部署步骤
===========================================

阶段1 - 基础部署 (30分钟)
-------------------------------------------
1. 将512x512的三张图缩放到128x128
2. 重命名为标准格式（见上方文件夹结构）
3. 放入 Bodies/ 文件夹
4. 运行 DeployTextures.bat 检查
5. 启动游戏测试

阶段2 - 图层分离 (2-3小时，可选)
-------------------------------------------
1. 分离龙角、尾巴、翅膀到独立图层
2. 导出到 Addons/ 文件夹
3. 修改 Races_Sideria.xml 配置bodyAddons
4. 调整位置和大小
5. 游戏中测试效果

阶段3 - 装备系统 (1-2小时，可选)
-------------------------------------------
1. 分离头巾到独立图层
2. 导出到 Apparel/BloodThornHood/ 文件夹
3. 创建 Apparel_Sideria_Hood.xml
4. 测试装备穿脱


?? 当前贴图来源
===========================================

来源：AI生成（原始512x512）
状态：未分离图层（整体图）
版本：戴头巾版本（可选装备设计）
计划：未来添加无头巾版本


?? 常见问题
===========================================

Q: 角色显示粉红色方块？
A: 检查文件名和路径是否正确，文件是否存在

Q: 角色太大或太小？
A: 修改 Races_Sideria.xml 中的 customDrawSize 值

Q: 部件位置不对？
A: 调整 bodyAddons 中的 offsets 值

Q: 如何测试？
A: 开发模式 > DebugSpawn > Spawn pawn > Sideria_Dracovampir


?? 相关文档
===========================================

详细指南：
- TEXTURE_DEPLOY_GUIDE.md     (快速部署)
- TEXTURE_FORMAT_ANALYSIS.md  (技术分析)
- TEXTURE_QUICK_REFERENCE.md  (快速参考)

RimWorld Modding:
- HAR Wiki: github.com/RimWorld-zh/Humanoid-Alien-Races/wiki
- Ludeon Forums: ludeon.com/forums


?? 提示
===========================================

1. 始终保留512x512原图作为备份
2. 使用版本号管理不同迭代（v1, v2等）
3. 先完成基础部署再优化细节
4. 使用开发模式快速测试调整
5. 记录成功的offset值以备后用


? 部署检查清单
===========================================

基础部署 (阶段1)：
[ ] 文件已缩放到128x128
[ ] 文件命名符合规范
[ ] 文件已放入Bodies文件夹
[ ] XML路径配置正确
[ ] 游戏中角色正常显示

进阶优化 (阶段2，可选)：
[ ] 龙角图层已分离
[ ] 尾巴图层已分离
[ ] 翅膀图层已分离
[ ] bodyAddons配置完成
[ ] 各部件位置正确

装备系统 (阶段3，可选)：
[ ] 头巾独立图层已创建
[ ] 装备定义已添加
[ ] 装备可正常穿脱
[ ] 无头巾版本已准备


===========================================
最后更新：2024
版本：1.0
状态：等待贴图文件
===========================================
