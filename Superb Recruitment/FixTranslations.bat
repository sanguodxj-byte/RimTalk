@echo off
chcp 65001 > nul
echo ====================================
echo Fix Translation Encoding Issues
echo ====================================
echo.

echo [1/3] Checking current encoding...
echo.

set "LANG_DIR=C:\Users\Administrator\Desktop\rim mod\Superb Recruitment\Languages"
set "TARGET_DIR=D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment\Languages"

echo Source: %LANG_DIR%
echo Target: %TARGET_DIR%
echo.

echo [2/3] Copying language files with correct encoding...
xcopy "%LANG_DIR%\ChineseSimplified" "%TARGET_DIR%\ChineseSimplified\" /E /I /Y
xcopy "%LANG_DIR%\English" "%TARGET_DIR%\English\" /E /I /Y
echo [OK] Files copied
echo.

echo [3/3] Verifying files...
if exist "%TARGET_DIR%\ChineseSimplified\Keyed\SuperbRecruitment.xml" (
    echo [OK] Chinese translation found
) else (
    echo [ERROR] Chinese translation missing!
)

if exist "%TARGET_DIR%\English\Keyed\SuperbRecruitment.xml" (
    echo [OK] English translation found
) else (
    echo [ERROR] English translation missing!
)
echo.

echo ====================================
echo Common Issues and Solutions:
echo ====================================
echo.
echo 1. If buttons show translation keys (e.g., "SuperbRecruitment_PersuadeLabel"):
echo    - Game language may not be set to Chinese
echo    - Translation file path may be wrong
echo    - Check: Options - Language - Choose Chinese(Simplified)
echo.
echo 2. If buttons show garbled text:
echo    - File encoding issue - FIXED by this script
echo    - Restart RimWorld to reload translations
echo.
echo 3. If buttons are blank:
echo    - Translation keys don't match code
echo    - Check log for errors
echo.
echo ====================================
echo Fix Complete!
echo ====================================
echo.
echo Next steps:
echo   1. Restart RimWorld
echo   2. Check Options - Language
echo   3. Should be Chinese(Simplified) or English
echo   4. If still broken, check Player.log
echo.
pause
