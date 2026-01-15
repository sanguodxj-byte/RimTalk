@echo off
chcp 65001 > nul
echo ====================================
echo Superb Recruitment - 自动编译
echo ====================================
echo.

cd /d "%~dp0Source\SuperbRecruitment"

echo [1/3] 清理旧文件...
if exist "bin" rd /s /q "bin"
if exist "obj" rd /s /q "obj"
echo [OK] 清理完成
echo.

echo [2/3] 恢复 NuGet 包...
dotnet restore
if errorlevel 1 (
    echo [ERROR] NuGet 包恢复失败
    pause
    exit /b 1
)
echo [OK] NuGet 包恢复成功
echo.

echo [3/3] 编译项目 (Release)...
dotnet build -c Release --no-restore
if errorlevel 1 (
    echo.
    echo [ERROR] 编译失败，请检查错误信息
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
    dir "%DLL_PATH%" | findstr "SuperbRecruitment.dll"
) else (
    echo [WARN] DLL 文件未找到，可能输出到了其他位置
    echo.
    echo 正在查找 DLL...
    dir /s /b "SuperbRecruitment.dll"
)

echo.
echo 下一步:
echo   1. 运行 "快速部署.bat" 完成部署
echo   2. 启动 RimWorld 测试模组
echo.
pause
