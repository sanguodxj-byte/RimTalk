@echo off
setlocal enabledelayedexpansion

echo ====================================
echo Update DLL - Close RimWorld First!
echo ====================================
echo.

set "SOURCE_DLL=C:\Users\Administrator\Desktop\rim mod\Superb Recruitment\Source\SuperbRecruitment\bin\Release\SuperbRecruitment.dll"
set "TARGET_DLL=D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment\Assemblies\SuperbRecruitment.dll"

echo Source: %SOURCE_DLL%
echo Target: %TARGET_DLL%
echo.

REM Check if source exists
if not exist "%SOURCE_DLL%" (
    echo [ERROR] Source DLL not found!
    echo Please compile first using: dotnet build -c Release
    pause
    exit /b 1
)

REM Try to copy
echo [1/3] Copying new DLL...
copy /Y "%SOURCE_DLL%" "%TARGET_DLL%" >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Copy failed! RimWorld might be running.
    echo.
    echo Please:
    echo   1. Close RimWorld completely
    echo   2. Run this script again
    echo.
    pause
    exit /b 1
)
echo [OK] DLL updated successfully
echo.

REM Copy language files
echo [2/3] Updating language files...
xcopy "C:\Users\Administrator\Desktop\rim mod\Superb Recruitment\Languages" "D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment\Languages\" /E /I /Y /Q >nul 2>&1
echo [OK] Language files updated
echo.

REM Verify
echo [3/3] Verifying...
if exist "%TARGET_DLL%" (
    for %%F in ("%TARGET_DLL%") do (
        echo [OK] DLL file size: %%~zF bytes
        echo [OK] Last modified: %%~tF
    )
) else (
    echo [ERROR] Target DLL not found
)
echo.

echo ====================================
echo Update Complete!
echo ====================================
echo.
echo New Features in this version:
echo   - Auto-recruit when persuasion reaches 100%%
echo   - Persuasion value can now reach 100%%
echo   - New letter when visitor auto-joins
echo.
echo Next steps:
echo   1. Start RimWorld
echo   2. Load your save
echo   3. Test the 100%% auto-recruit feature!
echo.
pause
