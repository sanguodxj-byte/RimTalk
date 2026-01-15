@echo off
chcp 65001 > nul
echo ====================================
echo Superb Recruitment Mod 部署脚本
echo ====================================
echo.

set SOURCE_DIR=%~dp0
set TARGET_DIR=D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment

echo 源目录: %SOURCE_DIR%
echo 目标目录: %TARGET_DIR%
echo.

echo [1/5] 检查目标目录...
if not exist "%TARGET_DIR%" (
    echo 创建目录: %TARGET_DIR%
    mkdir "%TARGET_DIR%"
)

echo [2/5] 复制 About 文件夹...
if exist "%SOURCE_DIR%About" (
    xcopy "%SOURCE_DIR%About" "%TARGET_DIR%\About\" /E /I /Y /Q
    echo ? About 文件夹已复制
) else (
    echo ? 警告: About 文件夹不存在
)

echo [3/5] 复制 Assemblies 文件夹...
if exist "%SOURCE_DIR%Assemblies" (
    xcopy "%SOURCE_DIR%Assemblies" "%TARGET_DIR%\Assemblies\" /E /I /Y /Q
    echo ? Assemblies 文件夹已复制
) else (
    echo ? 警告: Assemblies 文件夹不存在，请先编译项目
)

echo [4/5] 复制 Defs 文件夹...
if exist "%SOURCE_DIR%Defs" (
    xcopy "%SOURCE_DIR%Defs" "%TARGET_DIR%\Defs\" /E /I /Y /Q
    echo ? Defs 文件夹已复制
) else (
    echo ? 警告: Defs 文件夹不存在
)

echo [5/5] 复制 Languages 文件夹...
if exist "%SOURCE_DIR%Languages" (
    xcopy "%SOURCE_DIR%Languages" "%TARGET_DIR%\Languages\" /E /I /Y /Q
    echo ? Languages 文件夹已复制
) else (
    echo ? 警告: Languages 文件夹不存在
)

echo.
echo ====================================
echo 部署完成！
echo ====================================
echo.
echo 目标位置: %TARGET_DIR%
echo.
echo 提示:
echo 1. 请确保已编译项目（Release 模式）
echo 2. 在 RimWorld 中启用模组
echo 3. 重启游戏以加载模组
echo.
pause
