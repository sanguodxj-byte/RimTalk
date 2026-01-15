@echo off
echo Starting PowerShell deployment script...
echo.
PowerShell.exe -ExecutionPolicy Bypass -File "%~dp0Deploy.ps1"
pause
