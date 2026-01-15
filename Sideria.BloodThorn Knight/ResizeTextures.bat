@echo off
echo ===================================
echo 贴图自动缩放工具
echo 512x512 -> 128x128
echo ===================================
echo.

REM 检查是否安装了ImageMagick
where magick >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [错误] 未找到ImageMagick
    echo.
    echo 请先安装ImageMagick：
    echo https://imagemagick.org/script/download.php
    echo.
    echo 或者使用方法2：在线工具或图像编辑软件
    pause
    exit /b 1
)

echo [信息] 开始缩放贴图...
echo.

REM 创建输出目录
if not exist "Textures_Resized" mkdir "Textures_Resized"

REM 缩放所有512x512的PNG文件到128x128
for %%f in (*.png) do (
    echo 处理: %%f
    magick "%%f" -resize 128x128! "Textures_Resized\%%f"
)

echo.
echo [完成] 所有贴图已缩放到128x128
echo 输出位置: Textures_Resized\
echo.
pause
