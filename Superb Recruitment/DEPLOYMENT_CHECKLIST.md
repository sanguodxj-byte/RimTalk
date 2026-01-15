# 部署前检查清单

## ?? 编译前检查

- [ ] 所有 C# 代码没有语法错误
- [ ] 引用路径正确（Krafs.Rimworld.Ref, Lib.Harmony）
- [ ] 项目配置为 Release 模式
- [ ] OutputPath 指向正确的 RimWorld Mods 目录

## ?? 编译步骤

1. [ ] 打开 Visual Studio
2. [ ] 加载 `Source\SuperbRecruitment\SuperbRecruitment.csproj`
3. [ ] 选择 **Release** 配置
4. [ ] 生成解决方案 (Ctrl+Shift+B)
5. [ ] 检查输出窗口，确认没有错误
6. [ ] 确认 DLL 已复制到 Assemblies 文件夹

## ?? 文件结构检查

```
Superb Recruitment/
├── About/
│   ├── [?] About.xml
│   └── [?] PublishedFileId.txt
├── Assemblies/
│   └── [ ] SuperbRecruitment.dll (编译后生成)
├── Defs/
│   └── HediffDefs/
│       └── [?] Hediffs_PersuasionTracking.xml
├── Languages/
│   ├── English/
│   │   └── Keyed/
│   │       └── [?] SuperbRecruitment.xml
│   └── ChineseSimplified/
│       └── Keyed/
│           └── [?] SuperbRecruitment.xml
└── Textures/ (可选)
    └── UI/
        └── Commands/
            └── [ ] Persuade.png
```

## ?? 部署步骤

### 自动部署（推荐）
1. [ ] 运行 `Deploy.bat`
2. [ ] 检查输出信息
3. [ ] 确认文件已复制到目标目录

### 手动部署
1. [ ] 复制 About/ 到目标
2. [ ] 复制 Assemblies/ 到目标
3. [ ] 复制 Defs/ 到目标
4. [ ] 复制 Languages/ 到目标

### 目标位置
```
D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment\
```

## ? 游戏内测试

### 启动测试
1. [ ] 启动 RimWorld
2. [ ] 打开 Mod 管理器
3. [ ] 找到 "Superb Recruitment (卓越招募)"
4. [ ] 启用模组
5. [ ] 重启游戏
6. [ ] 检查主菜单右下角模组列表
7. [ ] 查看日志，确认没有红色错误

### 功能测试
1. [ ] 加载或创建游戏
2. [ ] 等待访客到来
3. [ ] 选中访客，检查是否有"说服"按钮
4. [ ] 点击说服按钮，测试对话系统
5. [ ] 测试玩家说服
6. [ ] 测试殖民者说服
7. [ ] 测试招募功能

### 边缘测试
1. [ ] 测试多个访客同时存在
2. [ ] 测试保存/加载
3. [ ] 测试访客离开后再次返回
4. [ ] 测试招募失败后的处理

## ?? 调试清单

如果出现问题：

### 模组不加载
- [ ] 检查 About.xml 格式
- [ ] 检查 packageId 是否唯一
- [ ] 查看 Player.log 日志

### 按钮不显示
- [ ] 确认访客是 Guest 状态（不是囚犯）
- [ ] 检查阵营关系（非敌对）
- [ ] 查看 Harmony 补丁是否应用

### 对话窗口问题
- [ ] 检查翻译文件是否正确加载
- [ ] 查看日志中的异常
- [ ] 确认 Dialog_SimplePersuasion 类加载

### 招募功能异常
- [ ] 检查 Hediff 是否正确创建
- [ ] 查看说服值计算逻辑
- [ ] 确认招募事件正确触发

## ?? 日志检查

查看日志文件：
```
C:\Users\Administrator\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log
```

搜索关键字：
- `[Superb Recruitment]`
- `SuperbRecruitment`
- `PersuasionTracking`

应该看到的日志：
```
[Superb Recruitment] Harmony补丁已应用
[Superb Recruitment] 初始化访客 XXX 的说服值: XX%
[Superb Recruitment] XXX 对 XXX 的说服效果: +X.XX (从 XX% 到 XX%)
```

## ?? 可选改进

- [ ] 添加自定义说服按钮图标 (Textures/UI/Commands/Persuade.png)
- [ ] 添加模组预览图 (About/Preview.png)
- [ ] 创建 Steam Workshop 页面
- [ ] 编写更详细的 Workshop 描述

## ?? 文档检查

- [?] README.md
- [?] UserGuide_CN.md
- [?] DEVELOPER_GUIDE.md
- [ ] LICENSE 文件
- [ ] CHANGELOG.md

## ?? 发布清单

### Steam Workshop 发布
1. [ ] 确保所有功能正常
2. [ ] 准备预览图和截图
3. [ ] 编写 Workshop 描述（中英文）
4. [ ] 在 RimWorld 中使用"上传"功能
5. [ ] 填写 Workshop 页面信息
6. [ ] 发布并测试

### GitHub 发布
1. [ ] 创建 repository
2. [ ] 上传源代码
3. [ ] 创建 Release
4. [ ] 添加编译后的文件下载链接

## ?? 注意事项

1. **版本管理**
   - 每次更新修改版本号
   - 记录 CHANGELOG

2. **兼容性**
   - 测试与主流模组的兼容性
   - 标注可能的冲突

3. **性能**
   - 避免过度的日志输出（Release版本）
   - 优化频繁调用的代码

4. **用户体验**
   - 确保提示信息清晰
   - 错误处理友好
   - 文档完善

## ? 完成确认

全部完成后：
- [ ] 所有测试通过
- [ ] 文档完整
- [ ] 准备发布

---

**准备发布！** ??
