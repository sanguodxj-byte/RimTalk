@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo ========================================
echo 贴图完整性检查工具
echo ========================================
echo.

set "MISSING_COUNT=0"
set "FOUND_COUNT=0"

echo [检查] 扫描贴图文件...
echo.

echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
echo 【优先级1】身体贴图 (必需)
echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

set "BODY_PATH=Textures\Things\Pawn\Humanlike\Bodies\Dracovampir"
set "BODY_TEXTURES=Naked_Male_south.png Naked_Male_east.png Naked_Male_north.png Naked_Female_south.png Naked_Female_east.png Naked_Female_north.png"

for %%T in (%BODY_TEXTURES%) do (
    if exist "%BODY_PATH%\%%T" (
        echo [?] %%T
        set /a FOUND_COUNT+=1
    ) else (
        echo [?] 缺失: %%T
        set /a MISSING_COUNT+=1
    )
)

echo.
echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
echo 【优先级2】Body Addons (高优先)
echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

set "ADDON_PATH=Textures\Things\Pawn\Humanlike\BodyAddons"
set "ADDON_TYPES=DragonHorns DragonWings DragonTail BloodMarkings"
set "DIRECTIONS=south east north"

for %%A in (%ADDON_TYPES%) do (
    echo.
    echo [%%A]
    for %%D in (%DIRECTIONS%) do (
        if exist "%ADDON_PATH%\%%A_%%D.png" (
            echo   [?] %%A_%%D.png
            set /a FOUND_COUNT+=1
        ) else (
            echo   [?] 缺失: %%A_%%D.png
            set /a MISSING_COUNT+=1
        )
    )
)

echo.
echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
echo 【优先级3】武器贴图 (中优先)
echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

set "WEAPON_PATH=Textures\Things\Item\Equipment\WeaponMelee"
set "WEAPON_TEXTURES=Atzgand.png Atzgand_Ascended.png BloodDagger.png"

for %%W in (%WEAPON_TEXTURES%) do (
    if exist "%WEAPON_PATH%\%%W" (
        echo [?] %%W
        set /a FOUND_COUNT+=1
    ) else (
        echo [?] 缺失: %%W
        set /a MISSING_COUNT+=1
    )
)

echo.
echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
echo 【优先级4】技能图标 (低优先)
echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

set "ICON_PATH=Textures\UI\Abilities"
set "ICON_TEXTURES=DragonBreath.png DragonicAura.png DragonWings.png DragonicTransformation.png BloodDrain.png VampiricEmbrace.png BloodFrenzy.png OathbreakerTransformation.png"

for %%I in (%ICON_TEXTURES%) do (
    if exist "%ICON_PATH%\%%I" (
        echo [?] %%I
        set /a FOUND_COUNT+=1
    ) else (
        echo [?] 缺失: %%I
        set /a MISSING_COUNT+=1
    )
)

echo.
echo ========================================
echo 检查结果统计
echo ========================================
echo.

set /a TOTAL=%FOUND_COUNT%+%MISSING_COUNT%
set /a PERCENT=%FOUND_COUNT%*100/%TOTAL%

echo 总计文件: %TOTAL%
echo 已找到:   %FOUND_COUNT% (%%%PERCENT%)
echo 缺失:     %MISSING_COUNT%
echo.

if %MISSING_COUNT% equ 0 (
    echo ? 所有贴图文件完整！
    echo.
    echo ?? 可以开始游戏测试了！
) else if %FOUND_COUNT% gtr 0 (
    echo ?? 部分贴图缺失，但mod仍可运行
    echo    缺失的贴图会显示为粉红色占位符
    echo.
    echo ?? 建议：
    echo    ? 优先补充【优先级1】身体贴图
    echo    ? 然后是【优先级2】Body Addons
    echo    ? 其余可以慢慢添加
) else (
    echo ? 所有贴图均缺失
    echo    Mod可以运行，但所有贴图都是粉红色
    echo.
    echo ?? 请按照以下顺序添加贴图：
    echo    1. 运行 CreateTextureStructure.bat 创建文件夹
    echo    2. 将贴图文件放入对应文件夹
    echo    3. 重新运行此脚本检查
)

echo.
echo ========================================
echo 详细文件清单
echo ========================================
echo.

echo ?? 查看完整贴图清单：
echo    Textures\STRUCTURE.md
echo.
echo ?? 贴图制作指南：
echo    TEXTURE_GUIDE_CORRECT.md
echo    BODY_ADDONS_GUIDE.md
echo    ATZGAND_ART_GUIDE.md
echo.

pause
