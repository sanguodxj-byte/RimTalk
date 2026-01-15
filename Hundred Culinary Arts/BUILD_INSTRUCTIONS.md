# 厨间百艺 - 编译部署说明

## 问题说明

RimWorld的Harmony是作为mod加载的，不在游戏的Managed目录中。因此编译时找不到`0Harmony.dll`。

## 解决方案

### 方案1：从现有Harmony Mod复制DLL（推荐）

1. **找到Harmony Mod**
   - Steam Workshop路径：`D:\steam\steamapps\workshop\content\294100\2009463077\Current\Assemblies\0Harmony.dll`
   - 或者从您已安装的mod中寻找包含Harmony的mod

2. **复制到本地引用目录**
   ```powershell
   # 创建引用目录
   mkdir "References"
   
   # 复制Harmony dll
   copy "D:\steam\steamapps\workshop\content\294100\2009463077\Current\Assemblies\0Harmony.dll" "References\"
   ```

3. **更新项目文件引用**
   修改 `Source/CulinaryArts/CulinaryArts.csproj`，将Harmony引用改为：
   ```xml
   <Reference Include="0Harmony">
     <HintPath>..\..\References\0Harmony.dll</HintPath>
     <Private>False</Private>
   </Reference>
   ```

### 方案2：直接部署源码（无需编译）

由于mod已经完整开发，您可以：

1. **手动复制文件到RimWorld Mods目录**
   ```powershell
   xcopy /E /I "About" "D:\steam\steamapps\common\RimWorld\Mods\Culinary Arts 100\About"
   xcopy /E /I "Defs" "D:\steam\steamapps\common\RimWorld\Mods\Culinary Arts 100\Defs"
   xcopy /E /I "Languages" "D:\steam\steamapps\common\RimWorld\Mods\Culinary Arts 100\Languages"
   xcopy /E /I "Source" "D:\steam\steamapps\common\RimWorld\Mods\Culinary Arts 100\Source"
   ```

2. **在游戏中启用Mod**
   - 启动RimWorld
   - 在Mod管理器中启用 "Culinary Arts 100"
   - 游戏会自动编译源码（第一次加载较慢）

### 方案3：使用Visual Studio编译

1. **安装Visual Studio 2022**（免费社区版）

2. **手动添加引用**
   - 右键项目 → 添加 → 引用
   - 浏览到RimWorld安装目录的DLL文件
   - 添加：Assembly-CSharp.dll, UnityEngine.CoreModule.dll, 0Harmony.dll

3. **编译**
   - 在Visual Studio中按F5或Ctrl+Shift+B编译

## 快速部署脚本（无需编译）

创建 `QuickDeploy.bat`：

```batch
@echo off
set RIMWORLD_MODS=D:\steam\steamapps\common\RimWorld\Mods
set MOD_NAME=Culinary Arts 100

echo 正在部署 %MOD_NAME% ...

xcopy /E /I /Y "About" "%RIMWORLD_MODS%\%MOD_NAME%\About"
xcopy /E /I /Y "Defs" "%RIMWORLD_MODS%\%MOD_NAME%\Defs"
xcopy /E /I /Y "Languages" "%RIMWORLD_MODS%\%MOD_NAME%\Languages"
xcopy /E /I /Y "Source" "%RIMWORLD_MODS%\%MOD_NAME%\Source"

echo.
echo 部署完成！
echo 启动游戏时RimWorld会自动编译源码。
echo.
pause
```

运行此脚本即可部署mod，游戏会在加载时自动编译。

## 验证部署

1. 启动RimWorld
2. 进入Mod管理器
3. 查找 "厨间百艺 (Culinary Arts 100)"
4. 启用mod并重启游戏
5. 查看日志：`RimWorld\Player.log`
6. 搜索 "[厨间百艺]" 确认mod已加载

## 预期日志输出

```
[厨间百艺] Mod初始化成功！
[厨间百艺] 技法总数: 116
[厨间百艺] 前缀总数: 48
[厨间百艺] 食材映射: 13
```

## 故障排除

### 错误：找不到Assembly-CSharp
**解决**：确认RimWorld路径正确，检查 `D:\steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll` 是否存在

### 错误：找不到0Harmony
**解决**：使用方案2直接部署源码，让游戏自动编译

### Mod未显示在列表中
**解决**：检查About/About.xml格式是否正确，确保文件夹名称为 "Culinary Arts 100"

### 游戏崩溃或红色错误
**解决**：查看Player.log，搜索错误信息，检查代码语法

## 推荐方式

**对于普通用户**：使用方案2（直接部署源码），最简单快捷

**对于开发者**：配置好引用后使用方案1，便于调试和迭代开发