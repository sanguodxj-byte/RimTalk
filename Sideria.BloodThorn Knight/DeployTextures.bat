@echo off
echo ========================================
echo 血龙种贴图部署工具
echo Dracovampir Texture Deployment Tool
echo ========================================
echo.

REM 创建必要的文件夹
echo [1/3] 创建文件夹结构...
mkdir "Textures\Things\Pawn\Humanlike\Bodies" 2>nul
mkdir "Textures\Things\Pawn\Humanlike\Addons" 2>nul
mkdir "Textures\Things\Pawn\Humanlike\Apparel\BloodThornHood" 2>nul
echo 完成！
echo.

REM 检查贴图文件
echo [2/3] 检查贴图文件...
if exist "Textures\Things\Pawn\Humanlike\Bodies\Naked_Dracovampir_south.png" (
    echo ? 找到 south.png
) else (
    echo ? 缺少 south.png - 请放入背面视图
)

if exist "Textures\Things\Pawn\Humanlike\Bodies\Naked_Dracovampir_north.png" (
    echo ? 找到 north.png
) else (
    echo ? 缺少 north.png - 请放入正面视图
)

if exist "Textures\Things\Pawn\Humanlike\Bodies\Naked_Dracovampir_east.png" (
    echo ? 找到 east.png
) else (
    echo ? 缺少 east.png - 请放入侧面视图
)
echo.

REM 显示说明
echo [3/3] 部署说明
echo.
echo 接下来的步骤：
echo 1. 将你的512x512图片缩放到128x128
echo 2. 重命名为：
echo    - Naked_Dracovampir_south.png （背面）
echo    - Naked_Dracovampir_north.png （正面）
echo    - Naked_Dracovampir_east.png  （侧面）
echo 3. 放入 Textures\Things\Pawn\Humanlike\Bodies\ 文件夹
echo 4. 重新运行此脚本检查
echo.

REM 打开文件夹
echo 按任意键打开贴图文件夹...
pause >nul
explorer "Textures\Things\Pawn\Humanlike\Bodies"

echo.
echo 部署工具运行完成！
echo.
pause
