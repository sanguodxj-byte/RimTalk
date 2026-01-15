@echo off
setlocal enabledelayedexpansion

echo ==========================================
echo   The Second Seat - 全量构建与部署
echo ==========================================

:: 配置路径
set "STEAM_MODS_DIR=D:\steam\steamapps\common\RimWorld\Mods"
set "SOURCE_ROOT=%~dp0"
:: 去掉末尾的反斜杠
set "SOURCE_ROOT=%SOURCE_ROOT:~0,-1%"

:: 临时配置环境变量
set "PATH=%PATH%;C:\Program Files\dotnet"

:: 1. 编译项目
echo [1/2] 编译项目...
if exist "%SOURCE_ROOT%\The Second Seat\Source\TheSecondSeat\TheSecondSeat.csproj" (
    cd /d "%SOURCE_ROOT%\The Second Seat\Source\TheSecondSeat"
    dotnet build -c Release --nologo -v q
    if !ERRORLEVEL! NEQ 0 (
        echo [错误] 编译失败！
        pause
        exit /b 1
    )
    echo   [完成] 编译成功
) else (
    echo [警告] 未找到项目文件，跳过编译步骤。
)

cd /d "%SOURCE_ROOT%"

:: 2. 部署所有相关 Mod
echo [2/2] 部署到 Steam 目录...
echo 目标: %STEAM_MODS_DIR%

if not exist "%STEAM_MODS_DIR%" (
    echo [错误] 找不到 Steam Mod 目录: %STEAM_MODS_DIR%
    pause
    exit /b 1
)

:: 显式部署每个 Mod
call :DeployMod "The Second Seat"
call :DeployMod "The Second Seat - Cthulhu"
call :DeployMod "[TSS]Sideria - Dragon Guard"

:: 额外确保 DLL 从开发目录正确复制到 Steam
echo   正在同步 DLL...
copy /Y "%SOURCE_ROOT%\The Second Seat\1.6\Assemblies\TheSecondSeat.dll" "%STEAM_MODS_DIR%\The Second Seat\1.6\Assemblies\" >nul
if !ERRORLEVEL! EQU 0 (
    echo     [成功] TheSecondSeat.dll 已同步
) else (
    echo     [失败] DLL 同步失败
)

echo.
echo ==========================================
echo   [成功] 所有模组已部署完成！
echo ==========================================
exit /b 0

:DeployMod
set "MOD_NAME=%~1"
echo.
echo   正在部署: !MOD_NAME!

:: 对主 Mod "The Second Seat" 进行特殊处理
if /I "!MOD_NAME!" == "The Second Seat" (
    :: 1. 同步除 Source 和顶层 Assemblies 外的所有文件（包括 1.6\Assemblies）
    robocopy "%SOURCE_ROOT%\!MOD_NAME!" "%STEAM_MODS_DIR%\!MOD_NAME!" /MIR /XD Source .git .vs .roo obj bin Properties /XF *.pdb *.user *.suo *.bat /R:3 /W:1 >nul
    
    :: 2. 清理可能存在的多余根目录 Assemblies
    if exist "%STEAM_MODS_DIR%\!MOD_NAME!\Assemblies" (
        rmdir /s /q "%STEAM_MODS_DIR%\!MOD_NAME!\Assemblies"
    )
) else (
    :: 对子 Mod 使用原有的同步逻辑
    robocopy "%SOURCE_ROOT%\!MOD_NAME!" "%STEAM_MODS_DIR%\!MOD_NAME!" /MIR /XD Source .git .vs .roo obj bin Properties /XF *.pdb *.user *.suo *.bat /R:3 /W:1 >nul
)

if !ERRORLEVEL! LSS 8 (
    echo     [成功] !MOD_NAME! 已同步
) else (
    echo     [失败] 部署 !MOD_NAME! 时出错 (代码: !ERRORLEVEL!)
)
goto :eof
