# Superb Recruitment 部署脚本 (PowerShell)
# 编码: UTF-8

Write-Host "====================================" -ForegroundColor Cyan
Write-Host "Superb Recruitment Mod 部署脚本" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

# 设置路径
$SourceDir = $PSScriptRoot
$TargetDir = "D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment"

Write-Host "源目录: $SourceDir" -ForegroundColor Yellow
Write-Host "目标目录: $TargetDir" -ForegroundColor Yellow
Write-Host ""

# 创建目标目录
Write-Host "[1/5] 检查并创建目标目录..." -ForegroundColor Green
if (-not (Test-Path $TargetDir)) {
    New-Item -Path $TargetDir -ItemType Directory -Force | Out-Null
    Write-Host "? 创建目录: $TargetDir" -ForegroundColor Green
} else {
    Write-Host "? 目标目录已存在" -ForegroundColor Green
}
Write-Host ""

# 复制 About 文件夹
Write-Host "[2/5] 复制 About 文件夹..." -ForegroundColor Green
$AboutSource = Join-Path $SourceDir "About"
$AboutTarget = Join-Path $TargetDir "About"
if (Test-Path $AboutSource) {
    if (Test-Path $AboutTarget) {
        Remove-Item -Path $AboutTarget -Recurse -Force
    }
    Copy-Item -Path $AboutSource -Destination $AboutTarget -Recurse -Force
    Write-Host "? About 文件夹已复制" -ForegroundColor Green
} else {
    Write-Host "? 警告: About 文件夹不存在" -ForegroundColor Red
}
Write-Host ""

# 复制 Assemblies 文件夹
Write-Host "[3/5] 复制 Assemblies 文件夹..." -ForegroundColor Green
$AssembliesSource = Join-Path $SourceDir "Assemblies"
$AssembliesTarget = Join-Path $TargetDir "Assemblies"
if (Test-Path $AssembliesSource) {
    if (Test-Path $AssembliesTarget) {
        Remove-Item -Path $AssembliesTarget -Recurse -Force
    }
    Copy-Item -Path $AssembliesSource -Destination $AssembliesTarget -Recurse -Force
    Write-Host "? Assemblies 文件夹已复制" -ForegroundColor Green
} else {
    Write-Host "? 警告: Assemblies 文件夹不存在，请先编译项目！" -ForegroundColor Yellow
    Write-Host "  提示: 需要先编译 Source\SuperbRecruitment\SuperbRecruitment.csproj" -ForegroundColor Yellow
}
Write-Host ""

# 复制 Defs 文件夹
Write-Host "[4/5] 复制 Defs 文件夹..." -ForegroundColor Green
$DefsSource = Join-Path $SourceDir "Defs"
$DefsTarget = Join-Path $TargetDir "Defs"
if (Test-Path $DefsSource) {
    if (Test-Path $DefsTarget) {
        Remove-Item -Path $DefsTarget -Recurse -Force
    }
    Copy-Item -Path $DefsSource -Destination $DefsTarget -Recurse -Force
    Write-Host "? Defs 文件夹已复制" -ForegroundColor Green
} else {
    Write-Host "? 警告: Defs 文件夹不存在" -ForegroundColor Red
}
Write-Host ""

# 复制 Languages 文件夹
Write-Host "[5/5] 复制 Languages 文件夹..." -ForegroundColor Green
$LanguagesSource = Join-Path $SourceDir "Languages"
$LanguagesTarget = Join-Path $TargetDir "Languages"
if (Test-Path $LanguagesSource) {
    if (Test-Path $LanguagesTarget) {
        Remove-Item -Path $LanguagesTarget -Recurse -Force
    }
    Copy-Item -Path $LanguagesSource -Destination $LanguagesTarget -Recurse -Force
    Write-Host "? Languages 文件夹已复制" -ForegroundColor Green
} else {
    Write-Host "? 警告: Languages 文件夹不存在" -ForegroundColor Red
}
Write-Host ""

# 完成
Write-Host "====================================" -ForegroundColor Cyan
Write-Host "? 部署完成！" -ForegroundColor Green
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "目标位置: $TargetDir" -ForegroundColor Yellow
Write-Host ""
Write-Host "下一步操作:" -ForegroundColor Cyan
Write-Host "1. 确保已编译项目（如果 Assemblies 文件夹为空）" -ForegroundColor White
Write-Host "2. 启动 RimWorld" -ForegroundColor White
Write-Host "3. 在模组管理器中启用 'Superb Recruitment'" -ForegroundColor White
Write-Host "4. 重启游戏以加载模组" -ForegroundColor White
Write-Host ""

# 检查是否有 DLL 文件
$DllPath = Join-Path $AssembliesTarget "SuperbRecruitment.dll"
if (Test-Path $DllPath) {
    Write-Host "? 检测到 DLL 文件: SuperbRecruitment.dll" -ForegroundColor Green
    $DllInfo = Get-Item $DllPath
    Write-Host "  文件大小: $($DllInfo.Length) 字节" -ForegroundColor Gray
    Write-Host "  修改时间: $($DllInfo.LastWriteTime)" -ForegroundColor Gray
} else {
    Write-Host "? 未检测到 DLL 文件！" -ForegroundColor Yellow
    Write-Host "  请先编译项目:" -ForegroundColor Yellow
    Write-Host "  1. 打开 Visual Studio" -ForegroundColor White
    Write-Host "  2. 加载 Source\SuperbRecruitment\SuperbRecruitment.csproj" -ForegroundColor White
    Write-Host "  3. 选择 Release 配置" -ForegroundColor White
    Write-Host "  4. 点击 '生成' -> '生成解决方案'" -ForegroundColor White
}
Write-Host ""

Read-Host "按回车键退出"
