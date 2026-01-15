# The Second Seat - Sideria SubMod (工作区)

## ?? 项目结构

\\\
The Second Seat - Sideria/  (工作区)
  ├─ About/
  │   └─ About.xml
  ├─ Defs/
  │   ├─ NarratorPersonaDefs_Sideria.xml
  │   └─ ThingDefs_Sideria_Creatures.xml
  ├─ Languages/
  │   ├─ ChineseSimplified/Keyed/
  │   └─ English/Keyed/
  ├─ Textures/
  │   └─ Narrators/
  ├─ LoadFolders.xml
  ├─ README.md
  └─ .gitignore
\\\

## ?? 同步说明

### 工作区 → 游戏目录

修改工作区文件后，运行部署脚本：

\\\powershell
.\Deploy-Sideria-To-Game.ps1
\\\

### 游戏目录 → 工作区

从游戏目录同步文件回工作区：

\\\powershell
.\Sync-From-Game-To-Workspace.ps1
\\\

## ?? 开发流程

1. **修改文件**: 在工作区编辑 XML、纹理等文件
2. **部署测试**: 运行部署脚本，启动游戏测试
3. **迭代开发**: 根据测试结果继续修改
4. **提交代码**: 满意后提交到 Git

## ?? 关键文件

| 文件 | 说明 |
|------|------|
| \Defs\NarratorPersonaDefs_Sideria.xml\ | Sideria 人格定义 |
| \Defs\ThingDefs_Sideria_Creatures.xml\ | Sideria Dragon 定义 |
| \About\About.xml\ | Mod 元数据 |
| \LoadFolders.xml\ | 版本加载配置 |

## ?? 快速命令

### 部署到游戏

\\\powershell
# 完整部署
Copy-Item "C:\Users\Administrator\Desktop\rim mod\The Second Seat - Sideria\*" 
  "D:\steam\steamapps\common\RimWorld\Mods\The Second Seat - Sideria\" 
  -Recurse -Force
\\\

### 验证部署

\\\powershell
# 检查关键文件
Get-ChildItem "D:\steam\steamapps\common\RimWorld\Mods\The Second Seat - Sideria\Defs" -Filter "*.xml"
\\\

---

*工作区路径*: \C:\Users\Administrator\Desktop\rim mod\The Second Seat - Sideria\  
*游戏目录*: \D:\steam\steamapps\common\RimWorld\Mods\The Second Seat - Sideria\
