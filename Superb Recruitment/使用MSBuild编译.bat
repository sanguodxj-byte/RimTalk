@echo off
chcp 65001 > nul
echo ====================================
echo Superb Recruitment - MSBuild 编译
echo ====================================
echo.

cd /d "%~dp0Source\SuperbRecruitment"

echo [1/4] 查找 MSBuild...

set MSBUILD_PATH=

REM 查找 Visual Studio 2022
if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    goto :found
)
if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
    goto :found
)
if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    goto :found
)

REM 查找 Visual Studio 2019
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
    goto :found
)
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
    goto :found
)

echo [ERROR] 未找到 MSBuild
echo 请确保已安装 Visual Studio 2019 或 2022
pause
exit /b 1

:found
echo [OK] 找到 MSBuild
echo.

echo [2/4] 清理旧文件...
if exist "bin" rd /s /q "bin"
if exist "obj" rd /s /q "obj"
echo [OK] 清理完成
echo.

echo [3/4] 恢复 NuGet 包...
"%MSBUILD_PATH%" SuperbRecruitment.csproj /t:Restore /p:Configuration=Release
if errorlevel 1 (
    echo [ERROR] NuGet 包恢复失败
    pause
    exit /b 1
)
echo [OK] NuGet 包恢复成功
echo.

echo [4/4] 编译项目 (Release)...
"%MSBUILD_PATH%" SuperbRecruitment.csproj /p:Configuration=Release /p:DebugType=none /p:DebugSymbols=false
if errorlevel 1 (
    echo.
    echo [ERROR] 编译失败
    pause
    exit /b 1
)

echo.
echo ====================================
echo [成功] 编译完成！
echo ====================================
echo.

REM 检查 DLL 文件
set "DLL_PATH=D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment\Assemblies\SuperbRecruitment.dll"
if exist "%DLL_PATH%" (
    echo [OK] DLL 文件已生成:
    echo     %DLL_PATH%
    echo.
    for %%F in ("%DLL_PATH%") do echo     大小: %%~zF 字节
) else (
    echo [WARN] DLL 文件未在目标位置找到
    echo 正在查找...
    dir /s /b SuperbRecruitment.dll 2>nul
)

echo.
echo 下一步: 运行 "快速部署.bat" 完成最终部署
echo.
pause
