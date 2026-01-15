@echo off
chcp 65001 >nul
echo ====================================
echo   厨间百艺 - PowerShell 部署脚本
echo ====================================
echo.

set "PWSH=C:\Program Files (x86)\WindowsPowerShell\pwsh.exe"
set "SOURCE=d:\rim mod\Hundred Culinary Arts"
set "TARGET=D:\steam\steamapps\common\RimWorld\Mods\Culinary Arts 100"

echo [1/2] 清理目标目录...
"%PWSH%" -Command "if (Test-Path '%TARGET%') { Remove-Item '%TARGET%' -Recurse -Force }; New-Item '%TARGET%' -ItemType Directory -Force | Out-Null"
echo   [完成] 目录已准备
echo.

echo [2/2] 部署文件...
"%PWSH%" -Command "Copy-Item '%SOURCE%\About' '%TARGET%\About' -Recurse -Force"
echo   - 复制 About 完成

"%PWSH%" -Command "Copy-Item '%SOURCE%\Assemblies' '%TARGET%\Assemblies' -Recurse -Force"
echo   - 复制 Assemblies 完成

"%PWSH%" -Command "Copy-Item '%SOURCE%\Defs' '%TARGET%\Defs' -Recurse -Force"
echo   - 复制 Defs 完成

"%PWSH%" -Command "Copy-Item '%SOURCE%\Languages' '%TARGET%\Languages' -Recurse -Force"
echo   - 复制 Languages 完成

"%PWSH%" -Command "if (Test-Path '%SOURCE%\Patches') { Copy-Item '%SOURCE%\Patches' '%TARGET%\Patches' -Recurse -Force }"
echo   - 复制 Patches 完成

"%PWSH%" -Command "if (Test-Path '%SOURCE%\Textures') { Copy-Item '%SOURCE%\Textures' '%TARGET%\Textures' -Recurse -Force }"
echo   - 复制 Textures 完成

echo.
echo ====================================
echo   [成功] 部署完成！
echo ====================================
echo.
echo 目标位置: %TARGET%
echo.
echo 修改内容：优化食物摄入性能，减少卡顿
echo   - 时间窗口：2500 ticks → 900 ticks (15秒)
echo   - 遍历优化：正向 → 反向 + 早停
echo   - 关键词匹配：25个循环 → 11个直接判断
echo.
pause
