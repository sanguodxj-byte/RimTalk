@echo off
setlocal enabledelayedexpansion

echo ====================================
echo Auto Build and Deploy
echo ====================================
echo.

set "PROJECT_DIR=C:\Users\Administrator\Desktop\rim mod\Superb Recruitment\Source\SuperbRecruitment"
set "TARGET_DIR=D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment"

cd /d "%PROJECT_DIR%"

echo [1/4] Building project...
dotnet build -c Release --verbosity quiet
if errorlevel 1 (
    echo [ERROR] Build failed!
    pause
    exit /b 1
)
echo [OK] Build successful
echo.

echo [2/4] Copying DLL...
copy /Y "bin\Release\SuperbRecruitment.dll" "%TARGET_DIR%\Assemblies\SuperbRecruitment.dll" >nul 2>&1
if errorlevel 1 (
    echo [WARN] DLL copy failed - RimWorld may be running
) else (
    echo [OK] DLL updated
)
echo.

echo [3/4] Copying Languages...
xcopy "C:\Users\Administrator\Desktop\rim mod\Superb Recruitment\Languages" "%TARGET_DIR%\Languages\" /E /I /Y /Q >nul 2>&1
echo [OK] Languages updated
echo.

echo [4/4] Copying Defs...
xcopy "C:\Users\Administrator\Desktop\rim mod\Superb Recruitment\Defs" "%TARGET_DIR%\Defs\" /E /I /Y /Q >nul 2>&1
echo [OK] Defs updated
echo.

echo ====================================
echo Deploy Complete!
echo ====================================
echo.
for %%F in ("%TARGET_DIR%\Assemblies\SuperbRecruitment.dll") do (
    echo DLL size: %%~zF bytes
    echo Last modified: %%~tF
)
echo.
