@echo off
chcp 65001 >nul
set RIMWORLD_MODS=D:\steam\steamapps\common\RimWorld\Mods
set MOD_NAME=Culinary Arts 100
set TARGET_DIR=%RIMWORLD_MODS%\%MOD_NAME%

echo ====================================
echo   厨间百艺快速部署脚本
echo ====================================
echo.

echo [1/6] 清理目标目录...
if exist "%TARGET_DIR%" (
    rmdir /S /Q "%TARGET_DIR%"
    echo   Done - 旧文件已清理
)
mkdir "%TARGET_DIR%"
echo   Done - 目录已重建

echo [2/6] 复制About文件夹...
xcopy /E /I /Y "About" "%TARGET_DIR%\About" >nul
echo   Done - About文件夹已复制

echo [3/6] 复制Assemblies文件夹...
xcopy /E /I /Y "Assemblies" "%TARGET_DIR%\Assemblies" >nul
echo   Done - Assemblies文件夹已复制

echo [4/6] 复制Patches文件夹...
xcopy /E /I /Y "Patches" "%TARGET_DIR%\Patches" >nul
echo   Done - Patches文件夹已复制

echo [5/6] 复制Defs文件夹...
xcopy /E /I /Y "Defs" "%TARGET_DIR%\Defs" >nul
echo   Done - Defs文件夹已复制

echo [6/6] 复制Languages文件夹...
xcopy /E /I /Y "Languages" "%TARGET_DIR%\Languages" >nul
echo   Done - 所有文件已复制

echo.
echo ====================================
echo   部署完成！
echo ====================================
echo.
echo 下一步操作：
echo 1. 启动RimWorld
echo 2. 进入Mod管理器
echo 3. 启用 Culinary Arts 100
echo 4. 重启游戏
echo.
echo 日志路径：%RIMWORLD_MODS%\..\Player.log
echo.
pause