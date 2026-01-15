@echo off
echo ====================================
echo   紧急修复系统 PATH
echo ====================================
echo.
echo 正在恢复 Windows 系统路径...
echo.

REM 使用 PowerShell 修复用户 PATH
"C:\Program Files (x86)\WindowsPowerShell\pwsh.exe" -Command "[Environment]::SetEnvironmentVariable('Path', 'C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0\;C:\Program Files (x86)\WindowsPowerShell', 'User')"

echo.
echo 已恢复基本系统路径。
echo.
echo 请关闭所有命令提示符窗口，重新打开后测试：
echo   setx
echo   reg
echo   xcopy
echo.
pause
