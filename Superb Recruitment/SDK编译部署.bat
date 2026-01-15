@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

echo ========================================
echo Superb Recruitment - SDK 8/9 编译部署
echo ========================================
echo.

set "SOURCE_DIR=%~dp0"
set "PROJECT_DIR=%SOURCE_DIR%Source\SuperbRecruitment"
set "TARGET_DIR=D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment"

REM 检查 dotnet SDK
echo [步骤 1/7] 检查 .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] 未找到 .NET SDK
    pause
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set SDK_VERSION=%%i
echo [OK] 找到 .NET SDK %SDK_VERSION%
echo.

REM 切换到项目目录
cd /d "%PROJECT_DIR%"

REM 清理旧文件
echo [步骤 2/7] 清理旧文件...
if exist "bin" rd /s /q "bin" 2>nul
if exist "obj" rd /s /q "obj" 2>nul
echo [OK] 清理完成
echo.

REM 恢复 NuGet 包
echo [步骤 3/7] 恢复 NuGet 包...
dotnet restore --verbosity quiet
if errorlevel 1 (
    echo [ERROR] NuGet 包恢复失败
    pause
    exit /b 1
)
echo [OK] NuGet 包恢复成功
echo.

REM 编译项目
echo [步骤 4/7] 编译项目 (Release)...
dotnet build -c Release --no-restore --verbosity minimal
if errorlevel 1 (
    echo.
    echo [ERROR] 编译失败，请检查上方错误信息
    pause
    exit /b 1
)
echo [OK] 编译成功
echo.

REM 创建目标目录
echo [步骤 5/7] 准备部署目录...
if not exist "%TARGET_DIR%" mkdir "%TARGET_DIR%" 2>nul
echo [OK] 目录已准备
echo.

REM 复制文件
echo [步骤 6/7] 复制模组文件...

REM About
if exist "%TARGET_DIR%\About" rd /s /q "%TARGET_DIR%\About" 2>nul
xcopy "%SOURCE_DIR%About" "%TARGET_DIR%\About\" /E /I /Y /Q >nul 2>&1
echo [OK] About 已复制

REM Defs
if exist "%TARGET_DIR%\Defs" rd /s /q "%TARGET_DIR%\Defs" 2>nul
xcopy "%SOURCE_DIR%Defs" "%TARGET_DIR%\Defs\" /E /I /Y /Q >nul 2>&1
echo [OK] Defs 已复制

REM Languages
if exist "%TARGET_DIR%\Languages" rd /s /q "%TARGET_DIR%\Languages" 2>nul
xcopy "%SOURCE_DIR%Languages" "%TARGET_DIR%\Languages\" /E /I /Y /Q >nul 2>&1
echo [OK] Languages 已复制
echo.

REM 验证部署
echo [步骤 7/7] 验证部署...
set "DLL_PATH=%TARGET_DIR%\Assemblies\SuperbRecruitment.dll"
if exist "%DLL_PATH%" (
    echo [OK] 模组已成功编译并部署！
    echo.
    echo 文件信息:
    for %%F in ("%DLL_PATH%") do (
        echo   文件名: %%~nxF
        echo   位置: %%~dpF
        echo   大小: %%~zF 字节
        echo   日期: %%~tF
    )
) else (
    echo [ERROR] DLL 文件未找到！
    echo.
    echo 正在查找编译输出...
    dir /s /b "%PROJECT_DIR%\bin\Release\*SuperbRecruitment.dll" 2>nul
    if errorlevel 1 (
        echo [ERROR] 未找到任何 DLL 文件
    )
)
echo.

echo ========================================
echo 部署完成！
echo ========================================
echo.
echo 模组位置: %TARGET_DIR%
echo.
echo 下一步操作:
echo   1. 启动 RimWorld
echo   2. 打开 Mods 管理器
echo   3. 启用 "Superb Recruitment (卓越招募)"
echo   4. 重启游戏
echo   5. 等待访客到来测试功能
echo.
echo 测试要点:
echo   - 选中访客，查看是否有"说服"按钮
echo   - 点击说服按钮，测试对话系统
echo   - 说服几次后，查看是否出现"招募"按钮
echo.
pause
