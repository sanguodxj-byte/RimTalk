@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

echo ========================================
echo Superb Recruitment - 一键编译并部署
echo ========================================
echo.

set "SOURCE_DIR=%~dp0"
set "PROJECT_DIR=%SOURCE_DIR%Source\SuperbRecruitment"
set "TARGET_DIR=D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment"

REM ========== 第一步：查找 MSBuild ==========
echo [步骤 1/6] 查找 MSBuild...

set MSBUILD_PATH=

if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    echo [OK] 找到 Visual Studio 2022 Community
    goto :msbuild_found
)
if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
    echo [OK] 找到 Visual Studio 2022 Professional
    goto :msbuild_found
)
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
    echo [OK] 找到 Visual Studio 2019 Community
    goto :msbuild_found
)

echo [ERROR] 未找到 MSBuild！
echo 请安装 Visual Studio 2019 或 2022
echo.
pause
exit /b 1

:msbuild_found
echo.

REM ========== 第二步：编译项目 ==========
echo [步骤 2/6] 编译项目...
cd /d "%PROJECT_DIR%"

REM 清理
if exist "bin" rd /s /q "bin" 2>nul
if exist "obj" rd /s /q "obj" 2>nul

REM 恢复包
"%MSBUILD_PATH%" SuperbRecruitment.csproj /t:Restore /p:Configuration=Release /nologo /v:minimal
if errorlevel 1 (
    echo [ERROR] NuGet 包恢复失败
    pause
    exit /b 1
)

REM 编译
"%MSBUILD_PATH%" SuperbRecruitment.csproj /p:Configuration=Release /p:DebugType=none /p:DebugSymbols=false /nologo /v:minimal
if errorlevel 1 (
    echo [ERROR] 编译失败
    pause
    exit /b 1
)
echo [OK] 编译成功
echo.

REM ========== 第三步：创建目标目录 ==========
echo [步骤 3/6] 创建目标目录...
if not exist "%TARGET_DIR%" (
    mkdir "%TARGET_DIR%" 2>nul
)
echo [OK] 目录已准备
echo.

REM ========== 第四步：复制 About ==========
echo [步骤 4/6] 部署 About 文件夹...
if exist "%TARGET_DIR%\About" rd /s /q "%TARGET_DIR%\About" 2>nul
xcopy "%SOURCE_DIR%About" "%TARGET_DIR%\About\" /E /I /Y /Q >nul 2>&1
echo [OK] About 已复制
echo.

REM ========== 第五步：复制 Defs ==========
echo [步骤 5/6] 部署 Defs 和 Languages...
if exist "%TARGET_DIR%\Defs" rd /s /q "%TARGET_DIR%\Defs" 2>nul
xcopy "%SOURCE_DIR%Defs" "%TARGET_DIR%\Defs\" /E /I /Y /Q >nul 2>&1

if exist "%TARGET_DIR%\Languages" rd /s /q "%TARGET_DIR%\Languages" 2>nul
xcopy "%SOURCE_DIR%Languages" "%TARGET_DIR%\Languages\" /E /I /Y /Q >nul 2>&1
echo [OK] Defs 和 Languages 已复制
echo.

REM ========== 第六步：验证 DLL ==========
echo [步骤 6/6] 验证部署...
set "DLL_PATH=%TARGET_DIR%\Assemblies\SuperbRecruitment.dll"
if exist "%DLL_PATH%" (
    echo [OK] DLL 文件已生成并部署
    for %%F in ("%DLL_PATH%") do (
        echo     位置: %%~dpF
        echo     文件: %%~nxF
        echo     大小: %%~zF 字节
    )
) else (
    echo [ERROR] DLL 文件未找到！
    echo 检查编译输出...
    dir /s /b "%PROJECT_DIR%\bin\Release\SuperbRecruitment.dll" 2>nul
)
echo.

echo ========================================
echo [完成] 模组已编译并部署！
echo ========================================
echo.
echo 部署位置: %TARGET_DIR%
echo.
echo 下一步:
echo   1. 启动 RimWorld
echo   2. 在模组管理器中启用 "Superb Recruitment"
echo   3. 重启游戏
echo   4. 开始测试！
echo.
pause
