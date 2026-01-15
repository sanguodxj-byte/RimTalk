@echo off
echo 正在通过注册表添加 PowerShell 到 PATH...
reg query "HKCU\Environment" /v Path >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    for /f "tokens=2*" %%a in ('reg query "HKCU\Environment" /v Path ^| findstr "Path"') do set "CURRENT_PATH=%%b"
    reg add "HKCU\Environment" /v Path /t REG_EXPAND_SZ /d "%CURRENT_PATH%;C:\Program Files (x86)\WindowsPowerShell" /f
    echo 完成！请重启所有命令提示符窗口。
) else (
    reg add "HKCU\Environment" /v Path /t REG_EXPAND_SZ /d "C:\Program Files (x86)\WindowsPowerShell" /f
    echo 完成！请重启所有命令提示符窗口。
)
pause
