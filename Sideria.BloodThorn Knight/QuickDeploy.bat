@echo off
echo ===================================
echo Quick Deploy (No Compilation)
echo ===================================
echo.

REM 设置路径
set RIMWORLD_MODS_PATH=D:\steam\steamapps\common\RimWorld\Mods
set MOD_NAME=Sideria.BloodThorn Knight
set TARGET_PATH=%RIMWORLD_MODS_PATH%\%MOD_NAME%

REM 检查路径
if not exist "%RIMWORLD_MODS_PATH%" (
    echo [ERROR] RimWorld Mods path not found: %RIMWORLD_MODS_PATH%
    pause
    exit /b 1
)

echo [INFO] Deploying to: %TARGET_PATH%
echo.

REM 清理旧文件
if exist "%TARGET_PATH%" (
    echo [INFO] Removing old files...
    rmdir /s /q "%TARGET_PATH%"
)

REM 创建文件夹
echo [INFO] Creating directories...
mkdir "%TARGET_PATH%"
mkdir "%TARGET_PATH%\About"
mkdir "%TARGET_PATH%\Assemblies"
mkdir "%TARGET_PATH%\Defs"
mkdir "%TARGET_PATH%\Languages"
mkdir "%TARGET_PATH%\Patches"
mkdir "%TARGET_PATH%\Textures"

REM 复制文件
echo [INFO] Copying files...
xcopy /E /I /Y "About" "%TARGET_PATH%\About" >nul
xcopy /E /I /Y "Defs" "%TARGET_PATH%\Defs" >nul
xcopy /E /I /Y "Languages" "%TARGET_PATH%\Languages" >nul
xcopy /Y "Patches\*.xml" "%TARGET_PATH%\Patches" >nul 2>nul
xcopy /E /I /Y "Textures" "%TARGET_PATH%\Textures" >nul

REM 复制DLL（如果存在）
if exist "Assemblies\*.dll" (
    xcopy /Y "Assemblies\*.dll" "%TARGET_PATH%\Assemblies" >nul
    echo [INFO] DLL files copied
) else (
    echo [WARNING] No DLL files found - mod will run in XML-only mode
)

echo.
echo ===================================
echo Quick Deploy Complete!
echo ===================================
echo.
echo Mod deployed to: %TARGET_PATH%
echo.
echo You can now start RimWorld and enable the mod!
echo.
pause
