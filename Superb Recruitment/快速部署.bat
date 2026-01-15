@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

echo ====================================
echo Superb Recruitment Mod 部署脚本
echo ====================================
echo.

set "SOURCE_DIR=%~dp0"
set "TARGET_DIR=D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment"

echo 源目录: %SOURCE_DIR%
echo 目标目录: %TARGET_DIR%
echo.

echo [1/5] 创建目标目录...
if not exist "%TARGET_DIR%" (
    mkdir "%TARGET_DIR%" 2>nul
    if !errorlevel! equ 0 (
        echo [OK] 创建目录成功
    ) else (
        echo [ERROR] 创建目录失败
        pause
        exit /b 1
    )
) else (
    echo [OK] 目标目录已存在
)
echo.

echo [2/5] 复制 About 文件夹...
if exist "%SOURCE_DIR%About" (
    if exist "%TARGET_DIR%\About" (
        rd /s /q "%TARGET_DIR%\About" 2>nul
    )
    xcopy "%SOURCE_DIR%About" "%TARGET_DIR%\About\" /E /I /Y /Q >nul 2>&1
    if !errorlevel! equ 0 (
        echo [OK] About 文件夹已复制
    ) else (
        echo [ERROR] About 文件夹复制失败
    )
) else (
    echo [WARN] About 文件夹不存在
)
echo.

echo [3/5] 复制 Assemblies 文件夹...
if exist "%SOURCE_DIR%Assemblies" (
    if exist "%TARGET_DIR%\Assemblies" (
        rd /s /q "%TARGET_DIR%\Assemblies" 2>nul
    )
    xcopy "%SOURCE_DIR%Assemblies" "%TARGET_DIR%\Assemblies\" /E /I /Y /Q >nul 2>&1
    if !errorlevel! equ 0 (
        echo [OK] Assemblies 文件夹已复制
    ) else (
        echo [ERROR] Assemblies 文件夹复制失败
    )
) else (
    echo [WARN] Assemblies 文件夹不存在，请先编译项目！
    echo       需要先用 Visual Studio 编译 Source\SuperbRecruitment\SuperbRecruitment.csproj
)
echo.

echo [4/5] 复制 Defs 文件夹...
if exist "%SOURCE_DIR%Defs" (
    if exist "%TARGET_DIR%\Defs" (
        rd /s /q "%TARGET_DIR%\Defs" 2>nul
    )
    xcopy "%SOURCE_DIR%Defs" "%TARGET_DIR%\Defs\" /E /I /Y /Q >nul 2>&1
    if !errorlevel! equ 0 (
        echo [OK] Defs 文件夹已复制
    ) else (
        echo [ERROR] Defs 文件夹复制失败
    )
) else (
    echo [WARN] Defs 文件夹不存在
)
echo.

echo [5/5] 复制 Languages 文件夹...
if exist "%SOURCE_DIR%Languages" (
    if exist "%TARGET_DIR%\Languages" (
        rd /s /q "%TARGET_DIR%\Languages" 2>nul
    )
    xcopy "%SOURCE_DIR%Languages" "%TARGET_DIR%\Languages\" /E /I /Y /Q >nul 2>&1
    if !errorlevel! equ 0 (
        echo [OK] Languages 文件夹已复制
    ) else (
        echo [ERROR] Languages 文件夹复制失败
    )
) else (
    echo [WARN] Languages 文件夹不存在
)
echo.

echo ====================================
echo 部署完成！
echo ====================================
echo.
echo 目标位置: %TARGET_DIR%
echo.

REM 检查 DLL 文件
if exist "%TARGET_DIR%\Assemblies\SuperbRecruitment.dll" (
    echo [OK] 检测到 DLL 文件: SuperbRecruitment.dll
) else (
    echo [WARN] 未检测到 DLL 文件！
    echo.
    echo 请先编译项目:
    echo   1. 打开 Visual Studio
    echo   2. 加载 Source\SuperbRecruitment\SuperbRecruitment.csproj
    echo   3. 选择 Release 配置
    echo   4. 点击 "生成" - "生成解决方案"
    echo   5. 再次运行本脚本
)
echo.

echo 下一步操作:
echo   1. 确保已编译项目（如果 DLL 文件不存在）
echo   2. 启动 RimWorld
echo   3. 在模组管理器中启用 "Superb Recruitment"
echo   4. 重启游戏以加载模组
echo.

pause
