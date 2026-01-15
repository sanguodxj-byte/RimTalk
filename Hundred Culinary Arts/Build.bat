@echo off
chcp 65001 >nul
echo ====================================
echo   厨间百艺 - 完整编译部署
echo ====================================
echo.

set "PWSH=C:\Program Files (x86)\WindowsPowerShell\pwsh.exe"
set "DOTNET=C:\Program Files\dotnet\dotnet.exe"
if not exist "%DOTNET%" set "DOTNET=C:\Program Files (x86)\dotnet\dotnet.exe"

set "SOURCE=d:\rim mod\Hundred Culinary Arts"
set "TARGET=D:\steam\steamapps\common\RimWorld\Mods\Culinary Arts 100"

echo [1/3] 编译项目...
cd /d "%SOURCE%\Source\CulinaryArts"
"%DOTNET%" build -c Release --nologo -v q
if errorlevel 1 (
    echo [错误] 编译失败！
    pause
    exit /b 1
)
echo   [完成] 编译成功
echo.

echo [2/3] 清理目标目录...
"%PWSH%" -Command "if (Test-Path '%TARGET%') { Remove-Item '%TARGET%' -Recurse -Force }; New-Item '%TARGET%' -ItemType Directory -Force | Out-Null"
echo   [完成] 目录已准备
echo.

echo [3/3] 部署文件...
cd /d "%SOURCE%"

"%PWSH%" -Command "Copy-Item '%SOURCE%\About' '%TARGET%\About' -Recurse -Force"
echo   - About 完成

"%PWSH%" -Command "Copy-Item '%SOURCE%\Assemblies' '%TARGET%\Assemblies' -Recurse -Force"
echo   - Assemblies 完成

"%PWSH%" -Command "Copy-Item '%SOURCE%\Defs' '%TARGET%\Defs' -Recurse -Force"
echo   - Defs 完成

"%PWSH%" -Command "Copy-Item '%SOURCE%\Languages' '%TARGET%\Languages' -Recurse -Force"
echo   - Languages 完成

"%PWSH%" -Command "if (Test-Path '%SOURCE%\Patches') { Copy-Item '%SOURCE%\Patches' '%TARGET%\Patches' -Recurse -Force }"
echo   - Patches 完成

"%PWSH%" -Command "if (Test-Path '%SOURCE%\Textures') { Copy-Item '%SOURCE%\Textures' '%TARGET%\Textures' -Recurse -Force }"
echo   - Textures 完成

echo.
echo ====================================
echo   [成功] 部署完成！
echo ====================================
echo.
echo 目标位置: %TARGET%
echo.
echo 修改内容：优化食物摄入性能，减少卡顿
echo   - 时间窗口从 2500 ticks 缩短到 900 ticks (15秒)
echo   - 遍历优化：反向遍历 + 早停机制
echo   - 关键词匹配：从 25 个循环优化为 11 个直接判断
echo.
pause
