@echo off
chcp 65001 >nul
echo ====================================
echo   Sideria 立绘修复部署
echo ====================================
echo.

set "SOURCE_SIDERIA=d:\rim mod\The Second Seat - Sideria"
set "SOURCE_TSS=d:\rim mod\The Second Seat"
set "TARGET_SIDERIA=D:\steam\steamapps\common\RimWorld\Mods\The Second Seat - Sideria"
set "TARGET_TSS=D:\steam\steamapps\common\RimWorld\Mods\The Second Seat"

echo [1/4] 部署 Sideria Defs（修复的路径配置）...
xcopy "%SOURCE_SIDERIA%\Defs" "%TARGET_SIDERIA%\Defs\" /E /I /Y /Q >nul
echo   ✓ Defs 已部署

echo.
echo [2/4] 部署 Sideria 纹理...
xcopy "%SOURCE_SIDERIA%\Textures" "%TARGET_SIDERIA%\Textures\" /E /I /Y /Q >nul
echo   ✓ Textures 已部署

echo.
echo [3/4] 编译主 Mod...
cd "%SOURCE_TSS%\Source"
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "TheSecondSeat.csproj" /p:Configuration=Release /t:Rebuild /v:q /nologo
if %ERRORLEVEL% NEQ 0 (
    echo   ✗ 编译失败！
    pause
    exit /b 1
)
echo   ✓ 编译成功

echo.
echo [4/4] 部署主 Mod Assemblies（修复的 PortraitLoader）...
xcopy "%SOURCE_TSS%\Source\bin\Release\TheSecondSeat.dll" "%TARGET_TSS%\Assemblies\" /Y /Q >nul
echo   ✓ Assemblies 已部署

echo.
echo ====================================
echo   [完成] 修复已部署！
echo ====================================
echo.
echo 修复内容：
echo   ✓ Sideria/Narrators/Layered 路径支持
echo   ✓ 基于 portraitPath 的智能路径回退
echo   ✓ 降临姿态/特效路径修复
echo.
echo 请重启游戏测试！
echo.
pause
