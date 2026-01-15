@echo off
chcp 65001 > nul
echo ====================================
echo Superb Recruitment - 编译脚本
echo ====================================
echo.

set PROJECT_FILE=Source\SuperbRecruitment\SuperbRecruitment.csproj
set MSBUILD_PATH=

echo [1/4] 查找 MSBuild...

REM 尝试查找 Visual Studio 2022
if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe
    echo 找到 Visual Studio 2022 Community
    goto :found
)

if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe
    echo 找到 Visual Studio 2022 Professional
    goto :found
)

if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe
    echo 找到 Visual Studio 2022 Enterprise
    goto :found
)

REM 尝试查找 Visual Studio 2019
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe
    echo 找到 Visual Studio 2019 Community
    goto :found
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe
    echo 找到 Visual Studio 2019 Professional
    goto :found
)

echo ? 错误: 找不到 MSBuild
echo 请安装 Visual Studio 2019 或 2022
pause
exit /b 1

:found
echo ? MSBuild 路径: %MSBUILD_PATH%
echo.

echo [2/4] 清理旧的编译文件...
if exist "Source\SuperbRecruitment\bin" (
    rmdir /s /q "Source\SuperbRecruitment\bin"
    echo ? 清理 bin 文件夹
)
if exist "Source\SuperbRecruitment\obj" (
    rmdir /s /q "Source\SuperbRecruitment\obj"
    echo ? 清理 obj 文件夹
)
echo.

echo [3/4] 恢复 NuGet 包...
"%MSBUILD_PATH%" "%PROJECT_FILE%" /t:Restore /p:Configuration=Release
if errorlevel 1 (
    echo ? NuGet 包恢复失败
    pause
    exit /b 1
)
echo ? NuGet 包恢复成功
echo.

echo [4/4] 编译项目 (Release 配置)...
"%MSBUILD_PATH%" "%PROJECT_FILE%" /p:Configuration=Release /p:DebugType=none /p:DebugSymbols=false
if errorlevel 1 (
    echo.
    echo ====================================
    echo ? 编译失败
    echo ====================================
    echo.
    echo 请检查错误信息，然后重试
    pause
    exit /b 1
)

echo.
echo ====================================
echo ? 编译成功！
echo ====================================
echo.
echo DLL 位置:
echo D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment\Assemblies\
echo.
echo 下一步:
echo 1. 检查编译输出（上方）
echo 2. 运行 Deploy.bat 部署完整模组
echo 3. 在 RimWorld 中测试
echo.
pause
