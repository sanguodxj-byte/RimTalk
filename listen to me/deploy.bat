@echo off
chcp 65001 >nul
echo =========================================
echo Listen To Me - 快速部署
echo =========================================
echo.

REM 检查管理员权限
net session >nul 2>&1
if %errorLevel% == 0 (
    echo [?] 以管理员权限运行
    echo.
) else (
    echo [!] 警告: 未以管理员权限运行
    echo [!] 创建符号链接可能失败
    echo [!] 建议: 右键点击此文件，选择"以管理员身份运行"
    echo.
    pause
)

REM 运行 PowerShell 部署脚本
PowerShell -ExecutionPolicy Bypass -File "%~dp0deploy.ps1"
