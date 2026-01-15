# 一键构建和部署脚本
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Listen To Me - 一键构建和部署" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"

# 路径配置
$projectRoot = "C:\Users\Administrator\Desktop\rim mod\listen to me"
$projectPath = "$projectRoot\Source\ListenToMe"
$targetPath = "D:\Steam\steamapps\common\RimWorld\Mods\ListenToMe"

# 步骤 1: 编译
Write-Host "步骤 1/3: 编译项目" -ForegroundColor Yellow
Write-Host "==================" -ForegroundColor Yellow
Write-Host ""

Push-Location $projectPath
try {
    Write-Host "清理旧文件..." -ForegroundColor Gray
    Remove-Item "obj" -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item "bin" -Recurse -Force -ErrorAction SilentlyContinue
    
    Write-Host "还原 NuGet 包..." -ForegroundColor Gray
    dotnet restore --verbosity quiet
    
    if ($LASTEXITCODE -ne 0) {
        throw "NuGet 还原失败"
    }
    
    Write-Host "编译项目..." -ForegroundColor Gray
    dotnet build -c Release --no-restore
    
    if ($LASTEXITCODE -ne 0) {
        throw "编译失败"
    }
    
    Write-Host ""
    Write-Host "? 编译成功！" -ForegroundColor Green
    
    $dll = Get-Item "$projectRoot\Assemblies\ListenToMe.dll"
    Write-Host "  DLL: $([math]::Round($dll.Length / 1KB, 2)) KB" -ForegroundColor Cyan
    Write-Host "  时间: $($dll.LastWriteTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Cyan
}
catch {
    Write-Host ""
    Write-Host "? 编译失败: $_" -ForegroundColor Red
    Pop-Location
    Read-Host "按 Enter 键退出"
    exit 1
}
finally {
    Pop-Location
}

Write-Host ""

# 步骤 2: 部署
Write-Host "步骤 2/3: 部署到 RimWorld" -ForegroundColor Yellow
Write-Host "==========================" -ForegroundColor Yellow
Write-Host ""

# 检查目标路径
if (!(Test-Path "D:\Steam\steamapps\common\RimWorld")) {
    Write-Host "? 错误: RimWorld 目录不存在" -ForegroundColor Red
    Write-Host "  路径: D:\Steam\steamapps\common\RimWorld" -ForegroundColor Red
    Read-Host "按 Enter 键退出"
    exit 1
}

# 检查现有部署
$deployType = "none"
if (Test-Path $targetPath) {
    $existing = Get-Item $targetPath
    if ($existing.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
        $deployType = "symlink"
        Write-Host "发现现有符号链接" -ForegroundColor Cyan
    } else {
        $deployType = "copy"
        Write-Host "发现现有文件复制" -ForegroundColor Cyan
    }
}

if ($deployType -eq "none") {
    # 首次部署，询问方式
    Write-Host "选择部署方式:" -ForegroundColor Yellow
    Write-Host "  1. 符号链接 (推荐)" -ForegroundColor White
    Write-Host "  2. 文件复制" -ForegroundColor White
    Write-Host ""
    $choice = Read-Host "请选择 (1/2)"
    
    if ($choice -eq "1") {
        $deployType = "symlink"
    } else {
        $deployType = "copy"
    }
}

# 执行部署
if ($deployType -eq "symlink") {
    Write-Host "使用符号链接模式..." -ForegroundColor Gray
    
    if (Test-Path $targetPath) {
        Remove-Item $targetPath -Force -Recurse
    }
    
    try {
        New-Item -ItemType SymbolicLink -Path $targetPath -Target $projectRoot -ErrorAction Stop | Out-Null
        Write-Host "? 符号链接创建成功" -ForegroundColor Green
    }
    catch {
        Write-Host "? 符号链接创建失败: $_" -ForegroundColor Red
        Write-Host "改用文件复制模式..." -ForegroundColor Yellow
        $deployType = "copy"
    }
}

if ($deployType -eq "copy") {
    Write-Host "使用文件复制模式..." -ForegroundColor Gray
    
    if (Test-Path $targetPath) {
        Remove-Item $targetPath -Recurse -Force
    }
    
    New-Item -ItemType Directory -Path $targetPath -Force | Out-Null
    
    # 复制文件
    $folders = @("About", "Assemblies", "Defs", "Languages", "Patches")
    foreach ($folder in $folders) {
        $src = Join-Path $projectRoot $folder
        $dst = Join-Path $targetPath $folder
        if (Test-Path $src) {
            Copy-Item -Path $src -Destination $dst -Recurse -Force
            Write-Host "  复制 $folder" -ForegroundColor Gray
        }
    }
    
    # 复制文档
    @("README.md", "LICENSE", "QUICKSTART.md", "COMMAND_FORMAT.md") | ForEach-Object {
        $src = Join-Path $projectRoot $_
        if (Test-Path $src) {
            Copy-Item -Path $src -Destination $targetPath -Force
        }
    }
    
    Write-Host "? 文件复制完成" -ForegroundColor Green
}

Write-Host ""

# 步骤 3: 验证
Write-Host "步骤 3/3: 验证部署" -ForegroundColor Yellow
Write-Host "==================" -ForegroundColor Yellow
Write-Host ""

$aboutXml = Join-Path $targetPath "About\About.xml"
$dllFile = Join-Path $targetPath "Assemblies\ListenToMe.dll"

$issues = @()

if (!(Test-Path $aboutXml)) {
    $issues += "About.xml 不存在"
}

if (!(Test-Path $dllFile)) {
    $issues += "ListenToMe.dll 不存在"
} else {
    $dll = Get-Item $dllFile
    if ($dll.Length -eq 0) {
        $issues += "DLL 文件大小为 0"
    }
}

if ($issues.Count -gt 0) {
    Write-Host "? 验证失败:" -ForegroundColor Red
    foreach ($issue in $issues) {
        Write-Host "  - $issue" -ForegroundColor Red
    }
    Write-Host ""
    Read-Host "按 Enter 键退出"
    exit 1
} else {
    Write-Host "? 部署验证通过" -ForegroundColor Green
    Write-Host ""
    
    # 显示统计
    $fileCount = (Get-ChildItem -Path $targetPath -Recurse -File).Count
    $totalSize = (Get-ChildItem -Path $targetPath -Recurse -File | Measure-Object -Property Length -Sum).Sum
    
    Write-Host "部署统计:" -ForegroundColor Cyan
    Write-Host "  位置: $targetPath" -ForegroundColor White
    $modeText = if ($deployType -eq 'symlink') { '符号链接' } else { '文件复制' }
    Write-Host "  模式: $modeText" -ForegroundColor White
    Write-Host "  文件数: $fileCount" -ForegroundColor White
    Write-Host "  大小: $([math]::Round($totalSize / 1KB, 2)) KB" -ForegroundColor White
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "? 构建和部署完成！" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""

Write-Host "下一步:" -ForegroundColor Yellow
Write-Host "  1. 启动 RimWorld" -ForegroundColor White
Write-Host "  2. 在 Mod 菜单启用 'Listen To Me - 听我指挥'" -ForegroundColor White
Write-Host "  3. 重启游戏" -ForegroundColor White
Write-Host "  4. 试试 '去厨房做饭' 指令！" -ForegroundColor White
Write-Host ""

if ($deployType -eq "symlink") {
    Write-Host "提示 (符号链接模式):" -ForegroundColor Cyan
    Write-Host "  修改代码后只需重新编译，无需重新部署" -ForegroundColor White
    Write-Host "  运行: build-and-restart.ps1" -ForegroundColor White
} else {
    Write-Host "提示 (文件复制模式):" -ForegroundColor Cyan
    Write-Host "  修改代码后需要重新运行此脚本" -ForegroundColor White
}

Write-Host ""

$launch = Read-Host "是否现在启动 RimWorld? (Y/N)"
if ($launch -eq 'Y' -or $launch -eq 'y') {
    $gameExe = "D:\Steam\steamapps\common\RimWorld\RimWorldWin64.exe"
    if (Test-Path $gameExe) {
        Write-Host "正在启动 RimWorld..." -ForegroundColor Green
        Start-Process $gameExe
        Write-Host "游戏已启动！" -ForegroundColor Green
    } else {
        Write-Host "未找到游戏执行文件，请手动启动" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "完成！" -ForegroundColor Green
Write-Host ""
Read-Host "按 Enter 键退出"
