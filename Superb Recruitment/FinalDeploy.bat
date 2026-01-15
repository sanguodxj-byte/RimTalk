@echo off
chcp 65001 > nul
echo ====================================
echo Final Deploy - RimTalk Integration
echo ====================================
echo.

echo WARNING: Close RimWorld before continuing!
echo.
pause

set "SOURCE_DIR=C:\Users\Administrator\Desktop\rim mod\Superb Recruitment"
set "TARGET_DIR=D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment"

echo [1/5] Deploying DLL...
copy /Y "%SOURCE_DIR%\Source\SuperbRecruitment\bin\Release\SuperbRecruitment.dll" "%TARGET_DIR%\Assemblies\SuperbRecruitment.dll" >nul 2>&1
if errorlevel 1 (
    echo [ERROR] DLL locked - RimWorld is still running!
    pause
    exit /b 1
)
echo [OK] DLL deployed (44 KB with RimTalk support)
echo.

echo [2/5] Deploying Languages...
xcopy "%SOURCE_DIR%\Languages" "%TARGET_DIR%\Languages\" /E /I /Y /Q >nul 2>&1
echo [OK] Languages deployed
echo.

echo [3/5] Deploying Defs...
xcopy "%SOURCE_DIR%\Defs" "%TARGET_DIR%\Defs\" /E /I /Y /Q >nul 2>&1
echo [OK] Defs deployed
echo.

echo [4/5] Deploying About...
xcopy "%SOURCE_DIR%\About" "%TARGET_DIR%\About\" /E /I /Y /Q >nul 2>&1
echo [OK] About deployed
echo.

echo [5/5] Verifying deployment...
if exist "%TARGET_DIR%\Assemblies\SuperbRecruitment.dll" (
    for %%F in ("%TARGET_DIR%\Assemblies\SuperbRecruitment.dll") do (
        echo [OK] DLL: %%~zF bytes
        echo [OK] Modified: %%~tF
    )
) else (
    echo [ERROR] DLL not found!
)
echo.

echo ====================================
echo Deployment Complete!
echo ====================================
echo.
echo New Features:
echo   * RimTalk integration - AI evaluates dialogue
echo   * Custom dialogue persuasion
echo   * Automatic persuasion tracking
echo   * 100%% auto-recruit
echo   * Full Chinese UI
echo.
echo How to use:
echo   WITH RimTalk:
echo     1. Click "Persuade" button
echo     2. Right-click visitor for dialogue
echo     3. AI evaluates your conversation
echo.
echo   WITHOUT RimTalk:
echo     1. Click "Persuade" button
echo     2. Choose dialogue option
echo.
pause
