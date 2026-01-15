@echo off
echo ===================================
echo Sideria BloodThorn Knight - Build and Deploy
echo ===================================
echo.

REM 设置RimWorld Mods路径
set RIMWORLD_MODS_PATH=D:\steam\steamapps\common\RimWorld\Mods
set MOD_NAME=Sideria.BloodThorn Knight

REM 设置目标路径
set TARGET_PATH=%RIMWORLD_MODS_PATH%\%MOD_NAME%

REM 检查RimWorld Mods路径是否存在
if not exist "%RIMWORLD_MODS_PATH%" (
    echo [ERROR] RimWorld Mods path not found: %RIMWORLD_MODS_PATH%
    echo Please edit this file and set correct RIMWORLD_MODS_PATH
    pause
    exit /b 1
)

echo [INFO] RimWorld Mods found at: %RIMWORLD_MODS_PATH%
echo [INFO] Target mod path: %TARGET_PATH%
echo.

REM ========================================
REM 第1步：编译C#代码
REM ========================================
echo [STEP 1/3] Building C# project...
echo.

cd /d "%~dp0Source"

REM 尝试使用dotnet build
where dotnet >nul 2>nul
if %ERRORLEVEL% EQU 0 (
    echo [INFO] Using dotnet CLI to build
    dotnet build SideriaBloodThornKnight.csproj -c Release
    if %ERRORLEVEL% NEQ 0 (
        echo [ERROR] Build failed with dotnet
        cd /d "%~dp0"
        pause
        exit /b 1
    )
) else (
    REM 尝试使用MSBuild
    set MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe
    
    if exist "%MSBUILD_PATH%" (
        echo [INFO] Using MSBuild to build
        "%MSBUILD_PATH%" SideriaBloodThornKnight.csproj /p:Configuration=Release
        if %ERRORLEVEL% NEQ 0 (
            echo [ERROR] Build failed with MSBuild
            cd /d "%~dp0"
            pause
            exit /b 1
        )
    ) else (
        echo [WARNING] No build tool found, skipping C# compilation
        echo [INFO] Mod will run in XML-only mode (no event system)
    )
)

cd /d "%~dp0"
echo [SUCCESS] Build completed!
echo.

REM ========================================
REM 第2步：创建/清理目标文件夹
REM ========================================
echo [STEP 2/3] Preparing target directory...
echo.

if exist "%TARGET_PATH%" (
    echo [INFO] Removing old mod files...
    rmdir /s /q "%TARGET_PATH%"
)

echo [INFO] Creating mod directory structure...
mkdir "%TARGET_PATH%"
mkdir "%TARGET_PATH%\About"
mkdir "%TARGET_PATH%\Assemblies"
mkdir "%TARGET_PATH%\Defs"
mkdir "%TARGET_PATH%\Languages"
mkdir "%TARGET_PATH%\Patches"
mkdir "%TARGET_PATH%\Textures"

echo [SUCCESS] Directory structure created!
echo.

REM ========================================
REM 第3步：复制文件
REM ========================================
echo [STEP 3/3] Copying mod files...
echo.

echo [INFO] Copying About...
xcopy /E /I /Y "About" "%TARGET_PATH%\About" >nul

echo [INFO] Copying Assemblies...
if exist "Assemblies\*.dll" (
    xcopy /Y "Assemblies\*.dll" "%TARGET_PATH%\Assemblies" >nul
    echo   - DLL files copied
) else (
    echo   - No DLL files found (XML-only mode)
)

echo [INFO] Copying Defs...
xcopy /E /I /Y "Defs" "%TARGET_PATH%\Defs" >nul

echo [INFO] Copying Languages...
xcopy /E /I /Y "Languages" "%TARGET_PATH%\Languages" >nul

echo [INFO] Copying Patches...
xcopy /E /I /Y "Patches\*.xml" "%TARGET_PATH%\Patches" >nul 2>nul

echo [INFO] Copying Textures...
xcopy /E /I /Y "Textures" "%TARGET_PATH%\Textures" >nul

echo [SUCCESS] All files copied!
echo.

REM ========================================
REM 完成
REM ========================================
echo ===================================
echo Deployment Complete!
echo ===================================
echo.
echo Mod location: %TARGET_PATH%
echo.
echo Next steps:
echo 1. Start RimWorld
echo 2. Enable "Sideria: BloodThorn Knight" in Mod menu
echo 3. Make sure Harmony and HAR are enabled first
echo 4. Start a new game or load existing save
echo 5. Press ~ for dev mode and test
echo.
echo For testing instructions, see: QUICK_TEST.md
echo.
echo ===================================
pause
