@echo off
chcp 65001 >nul
echo ====================================
echo   PowerShell 6.2.4 环境配置
echo ====================================
echo.

echo 检查 pwsh.exe 位置...

set "PWSH_DIR="
if exist "C:\Program Files\PowerShell\6\pwsh.exe" set "PWSH_DIR=C:\Program Files\PowerShell\6"
if exist "C:\Program Files\PowerShell\7\pwsh.exe" set "PWSH_DIR=C:\Program Files\PowerShell\7"
if exist "C:\Program Files (x86)\PowerShell\6\pwsh.exe" set "PWSH_DIR=C:\Program Files (x86)\PowerShell\6"
if exist "C:\Program Files (x86)\WindowsPowerShell\pwsh.exe" set "PWSH_DIR=C:\Program Files (x86)\WindowsPowerShell"

if defined PWSH_DIR (
    echo ✓ 找到 pwsh.exe 在: %PWSH_DIR%
    echo.
    echo 正在添加到用户 PATH...
    setx PATH "%PATH%;%PWSH_DIR%"
    echo ✓ 已添加到用户 PATH
    echo.
    echo ========================================
    echo 重要：请关闭所有命令提示符窗口
    echo 然后重新打开，测试命令:
    echo   pwsh --version
    echo ========================================
) else (
    echo ✗ 未找到 pwsh.exe
    echo.
    echo 正在搜索所有可能的位置...
    echo.
    for /f "delims=" %%i in ('dir /s /b "C:\Program Files\pwsh.exe" 2^>nul') do echo 找到: %%i
    for /f "delims=" %%i in ('dir /s /b "C:\Program Files (x86)\pwsh.exe" 2^>nul') do echo 找到: %%i
    echo.
    echo 请手动将 pwsh.exe 所在目录添加到 PATH
)

echo.
pause
