@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo ========================================
echo 符号链接部署工具
echo ========================================
echo.

REM 目标路径
set "TARGET=D:\steam\steamapps\common\RimWorld\Mods\Sideria.BloodThorn Knight"
set "SOURCE=%~dp0"

REM 检查是否以管理员身份运行
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [错误] 需要管理员权限来创建符号链接！
    echo.
    echo 请右键点击此脚本，选择"以管理员身份运行"
    echo.
    pause
    exit /b 1
)

echo [信息] 检测到管理员权限 ?
echo.

REM 检查目标目录
if not exist "%TARGET%" (
    echo [创建] 目标目录不存在，正在创建...
    mkdir "%TARGET%" 2>nul
    if not exist "%TARGET%" (
        echo [错误] 无法创建目标目录！
        pause
        exit /b 1
    )
)

echo [信息] 源目录: %SOURCE%
echo [信息] 目标目录: %TARGET%
echo.

REM 定义需要链接的文件夹和文件
set "LINK_FOLDERS=Defs Languages Textures Patches Assemblies"
set "LINK_FILES=About\About.xml About\Preview.png About\PublishedFileId.txt"

echo ========================================
echo 开始创建符号链接...
echo ========================================
echo.

REM 创建About文件夹（如果不存在）
if not exist "%TARGET%\About" (
    mkdir "%TARGET%\About"
)

REM 链接文件夹
for %%F in (%LINK_FOLDERS%) do (
    echo [处理] 文件夹: %%F
    
    REM 如果目标已存在，删除
    if exist "%TARGET%\%%F" (
        echo   [删除] 移除旧的链接/文件夹...
        rmdir /s /q "%TARGET%\%%F" 2>nul
        del /f /q "%TARGET%\%%F" 2>nul
    )
    
    REM 创建符号链接
    mklink /D "%TARGET%\%%F" "%SOURCE%%%F" >nul 2>&1
    
    if exist "%TARGET%\%%F" (
        echo   [?] 符号链接创建成功
    ) else (
        echo   [?] 符号链接创建失败
    )
    echo.
)

REM 链接文件
for %%F in (%LINK_FILES%) do (
    echo [处理] 文件: %%F
    
    REM 如果目标已存在，删除
    if exist "%TARGET%\%%F" (
        echo   [删除] 移除旧文件...
        del /f /q "%TARGET%\%%F" 2>nul
    )
    
    REM 创建硬链接（文件使用硬链接更可靠）
    mklink /H "%TARGET%\%%F" "%SOURCE%%%F" >nul 2>&1
    
    if exist "%TARGET%\%%F" (
        echo   [?] 硬链接创建成功
    ) else (
        echo   [?] 硬链接创建失败
    )
    echo.
)

echo ========================================
echo 部署完成！
echo ========================================
echo.

echo ? Mod已部署到: %TARGET%
echo.
echo ?? 符号链接说明：
echo   ? 修改源文件会自动同步到RimWorld
echo   ? 无需重复复制文件
echo   ? 适合开发调试
echo.
echo ?? 现在可以：
echo   1. 启动RimWorld
echo   2. 在Mod菜单中启用本mod
echo   3. 开始测试
echo.

echo ?? 提示：
echo   ? 如需更新，直接修改源文件即可
echo   ? 如需删除符号链接，直接删除目标文件夹
echo   ? 不会影响源文件
echo.

pause
