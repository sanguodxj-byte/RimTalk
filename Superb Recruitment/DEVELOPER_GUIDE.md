# Superb Recruitment - 开发者说明

## 项目结构

```
Superb Recruitment/
├── About/                          # 模组信息
│   ├── About.xml                   # 模组描述
│   ├── Preview.png                 # 预览图（需要添加）
│   └── PublishedFileId.txt         # Steam Workshop ID
│
├── Assemblies/                     # 编译后的DLL（自动生成）
│   └── SuperbRecruitment.dll
│
├── Defs/                           # 游戏定义文件
│   └── HediffDefs/
│       └── Hediffs_PersuasionTracking.xml
│
├── Languages/                      # 本地化文件
│   ├── English/
│   │   └── Keyed/
│   │       └── SuperbRecruitment.xml
│   └── ChineseSimplified/
│       └── Keyed/
│           └── SuperbRecruitment.xml
│
├── Source/                         # 源代码
│   └── SuperbRecruitment/
│       ├── Command_PersuadeVisitor.cs
│       ├── Command_RecruitVisitor.cs
│       ├── Dialog_SimplePersuasion.cs
│       ├── HarmonyPatches.cs
│       ├── Hediff_PersuasionTracking.cs
│       ├── HediffComp_PersuasionTracker.cs
│       ├── PersuasionDialogueManager.cs
│       └── SuperbRecruitment.csproj
│
├── Textures/                       # 贴图文件（需要添加）
│   └── UI/
│       └── Commands/
│           └── Persuade.png        # 说服按钮图标
│
├── RimTalk_Integration/            # RimTalk集成示例
│   └── DialogueSample.xml
│
├── Deploy.bat                      # 部署脚本
├── README.md                       # 项目说明
└── UserGuide_CN.md                 # 用户指南
```

## 开发环境设置

### 必需软件
- Visual Studio 2019/2022
- .NET Framework 4.7.2
- RimWorld (Steam版本)

### 项目配置

#### 1. 输出路径设置
在 `SuperbRecruitment.csproj` 中已配置：
```xml
<OutputPath>..\..\..\..\..\..\..\..\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\Superb Recruitment\Assemblies\</OutputPath>
```

如果您的 RimWorld 路径不同，请修改此路径。

#### 2. 引用库路径
确保以下引用正确：
- `Krafs.Rimworld.Ref` (NuGet包)
- `Lib.Harmony` (NuGet包)
- `RimTalk.dll` (可选，如果要集成RimTalk)

### 编译步骤

1. **打开解决方案**
   ```
   双击 Source\SuperbRecruitment\SuperbRecruitment.csproj
   ```

2. **选择 Release 配置**
   - 在 Visual Studio 中选择 "Release" 而不是 "Debug"

3. **编译项目**
   - 按 Ctrl+Shift+B 或选择 "生成" → "生成解决方案"

4. **检查输出**
   - DLL 应该自动复制到 RimWorld\Mods\Superb Recruitment\Assemblies\

## 部署流程

### 方法1: 自动部署（推荐）
运行 `Deploy.bat` 脚本：
```bash
双击 Deploy.bat
```

脚本会自动复制：
- About 文件夹
- Assemblies 文件夹
- Defs 文件夹
- Languages 文件夹

### 方法2: 手动部署
1. 编译项目（Release模式）
2. 复制以下文件夹到 `D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment\`：
   - About/
   - Assemblies/
   - Defs/
   - Languages/

### 方法3: Steam Workshop 部署
1. 确保所有文件在正确位置
2. 在 RimWorld 中打开 Mod 管理器
3. 点击 "Upload" 上传到 Steam Workshop

## 测试清单

### 基础功能测试
- [ ] 模组正确加载（没有红色错误）
- [ ] 访客到来时显示"说服"按钮
- [ ] 点击说服按钮显示选项菜单
- [ ] 玩家可以进行对话
- [ ] 殖民者可以自动说服
- [ ] 说服值正确更新
- [ ] 招募按钮在合适时机出现
- [ ] 招募成功/失败正确处理

### 边缘情况测试
- [ ] 访客离开时 Hediff 正确保留/移除
- [ ] 保存/加载游戏后数据保持
- [ ] 多个访客同时存在
- [ ] 访客变成敌对后的处理
- [ ] 殖民者死亡/离开后的处理

### 兼容性测试
- [ ] 与 Hospitality 模组兼容
- [ ] 与其他招募模组的冲突检查
- [ ] 多语言支持正确

## 调试技巧

### 1. 启用开发模式
在 RimWorld 中按 `Ctrl + F12` 打开开发者工具

### 2. 查看日志
日志文件位置：
```
C:\Users\<用户名>\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log
```

搜索关键字：
```
[Superb Recruitment]
```

### 3. 调试输出
代码中已包含调试输出：
```csharp
Log.Message($"[Superb Recruitment] 初始化访客 {pawn.LabelShort} 的说服值: {persuasionValue:P0}");
```

### 4. 使用 Debug 配置
修改 `.csproj` 中的 DebugType：
```xml
<DebugType>portable</DebugType>
```

然后可以附加调试器到 RimWorld 进程。

## 常见问题解决

### Q: 编译错误 - 找不到引用
**解决方案**:
1. 右键项目 → 管理 NuGet 包
2. 确保安装了：
   - Krafs.Rimworld.Ref (版本 1.5.4104)
   - Lib.Harmony (版本 2.3.3)

### Q: 模组不加载
**检查清单**:
1. About.xml 格式正确
2. DLL 在 Assemblies 文件夹
3. 没有编译错误
4. 检查 Player.log 中的错误信息

### Q: 按钮不显示
**可能原因**:
1. Harmony 补丁未正确应用
2. 访客条件不满足（不是 Guest 状态）
3. 阵营关系敌对

### Q: 对话窗口不弹出
**检查**:
1. PersuasionDialogueManager 是否正确初始化
2. 检查日志中的错误
3. 确认 Dialog_SimplePersuasion 类正确加载

## 代码说明

### 核心类

#### 1. Hediff_PersuasionTracking
- 存储说服进度
- 管理尝试次数
- 计算招募成功率

#### 2. PersuasionDialogueManager
- 管理对话流程
- 单例模式
- 处理玩家和NPC说服

#### 3. Command_PersuadeVisitor
- 说服按钮 Gizmo
- 显示说服者选择菜单
- 创建/获取说服追踪 Hediff

#### 4. Command_RecruitVisitor
- 招募按钮 Gizmo
- 处理招募确认
- 执行招募结果

#### 5. Dialog_SimplePersuasion
- 简单对话窗口
- 后备对话系统（无RimTalk时使用）
- 多种对话选项

### 关键算法

#### 说服值初始化（正态分布）
```csharp
float u1 = Rand.Value;
float u2 = Rand.Value;
float randStdNormal = (float)(Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2));
persuasionValue = 0.5f + 0.15f * randStdNormal;
```

#### 成功率计算
```csharp
float successChance = (float)Math.Pow(persuasionValue, 2.5);
```

这个公式使得：
- 50% 说服值 → ~17.7% 成功率
- 70% 说服值 → ~42.7% 成功率
- 90% 说服值 → ~72.5% 成功率

## 性能优化建议

1. **避免频繁的 Hediff 查询**
   - 在 Gizmo 中缓存 Hediff 引用
   
2. **减少日志输出**
   - Release 版本移除 Debug 日志
   
3. **优化殖民者列表查询**
   - 使用 LINQ 延迟执行
   - 避免重复查询

## 未来改进方向

### 短期目标
1. 添加自定义图标（Persuade.png）
2. 添加音效
3. 优化对话选项文本
4. 添加更多特性影响

### 中期目标
1. 完整的 RimTalk 集成
2. 更复杂的对话树
3. 访客特性影响难度
4. 阵营关系影响

### 长期目标
1. 统计系统（成功率追踪）
2. 成就系统
3. 自定义事件
4. 多人说服（对话参与多人）

## 贡献指南

如果您想改进这个模组：

1. Fork 项目
2. 创建功能分支
3. 提交更改
4. 发起 Pull Request

请确保：
- 代码注释完整（中英文）
- 遵循现有代码风格
- 测试所有更改
- 更新文档

## 许可证

MIT License - 详见 LICENSE 文件

## 联系方式

- GitHub Issues: 报告 Bug 或功能请求
- Steam Workshop: 用户反馈和讨论

---

**祝开发愉快！**
