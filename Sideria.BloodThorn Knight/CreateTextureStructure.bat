@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo ========================================
echo 贴图文件夹结构创建工具
echo ========================================
echo.

REM 定义贴图文件夹列表
set "FOLDERS=Textures\Things\Pawn\Humanlike\Bodies\Dracovampir"
set "FOLDERS=%FOLDERS%;Textures\Things\Pawn\Humanlike\BodyAddons"
set "FOLDERS=%FOLDERS%;Textures\Things\Pawn\Humanlike\Heads\Male"
set "FOLDERS=%FOLDERS%;Textures\Things\Pawn\Humanlike\Heads\Female"
set "FOLDERS=%FOLDERS%;Textures\Things\Item\Equipment\WeaponMelee"
set "FOLDERS=%FOLDERS%;Textures\UI\Abilities"
set "FOLDERS=%FOLDERS%;Textures\UI\Icons"

echo [创建] 正在创建贴图文件夹结构...
echo.

REM 创建每个文件夹
for %%F in ("%FOLDERS:;=" "%") do (
    if not exist "%%~F" (
        mkdir "%%~F" 2>nul
        if exist "%%~F" (
            echo [?] %%~F
        ) else (
            echo [?] 创建失败: %%~F
        )
    ) else (
        echo [已存在] %%~F
    )
)

echo.
echo ========================================
echo 文件夹结构创建完成！
echo ========================================
echo.

echo ?? 贴图放置说明：
echo.
echo 【必需贴图 - 优先级1】
echo   ?? Textures\Things\Pawn\Humanlike\Bodies\Dracovampir\
echo      ? Naked_Male_south.png (512x512)
echo      ? Naked_Male_east.png (512x512)
echo      ? Naked_Male_north.png (512x512)
echo      ? Naked_Female_south.png (512x512)
echo      ? Naked_Female_east.png (512x512)
echo      ? Naked_Female_north.png (512x512)
echo.
echo 【Body Addons - 优先级2】
echo   ?? Textures\Things\Pawn\Humanlike\BodyAddons\
echo      ? DragonHorns_south/east/north.png (512x512 各3个)
echo      ? DragonWings_south/east/north.png (512x512 各3个)
echo      ? DragonTail_south/east/north.png (512x512 各3个)
echo      ? BloodMarkings_south/east/north.png (512x512 各3个)
echo.
echo 【武器贴图 - 优先级3】
echo   ?? Textures\Things\Item\Equipment\WeaponMelee\
echo      ? Atzgand.png (256x256)
echo      ? Atzgand_Ascended.png (256x256)
echo      ? BloodDagger.png (256x256)
echo.
echo 【技能图标 - 优先级4】
echo   ?? Textures\UI\Abilities\
echo      ? DragonBreath.png (64x64)
echo      ? DragonicAura.png (64x64)
echo      ? DragonWings.png (64x64)
echo      ? DragonicTransformation.png (64x64)
echo      ? BloodDrain.png (64x64)
echo      ? VampiricEmbrace.png (64x64)
echo      ? BloodFrenzy.png (64x64)
echo      ? OathbreakerTransformation.png (64x64)
echo.
echo ========================================
echo 下一步操作：
echo ========================================
echo 1. 将贴图文件放入对应文件夹
echo 2. 运行 DeployWithSymlink.bat 部署到RimWorld
echo 3. 启动游戏测试
echo.

pause
