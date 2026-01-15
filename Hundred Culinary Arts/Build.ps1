# 厨间百艺 (Culinary Arts 100) 构建脚本
# 用于自动编译mod并可选地部署到RimWorld Mods目录

param(
    [string]$RimWorldPath = "D:\steam\steamapps\common\RimWorld",
    [switch]$Deploy,
    [switch]$Clean
)

$ErrorActionPreference = "Stop"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  厨间百艺 Mod 构建脚本" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# 获取项目根目录
$ProjectRoot = $PSScriptRoot
$SourcePath = Join-Path $ProjectRoot "Source\CulinaryArts"
$AssembliesPath = Join-Path $ProjectRoot "Assemblies"
$ModName = "Culinary Arts 100"

# 清理旧文件
if ($Clean) {
    Write-Host "[1/4] 清理旧文件..." -ForegroundColor Yellow
    if (Test-Path $AssembliesPath) {
        Remove-Item "$AssembliesPath\*.dll" -Force -ErrorAction SilentlyContinue
        Remove-Item "$AssembliesPath\*.pdb" -Force -ErrorAction SilentlyContinue
        Write-Host "  ✓ 已清理 Assemblies 目录" -ForegroundColor Green
    }
} else {
    Write-Host "[1/4] 跳过清理（使用 -Clean 参数启用）" -ForegroundColor Gray
}

# 编译项目
Write-Host "[2/4] 编译 C# 项目..." -ForegroundColor Yellow
try {
    $ProjectFile = Join-Path $SourcePath "CulinaryArts.csproj"
    
    if (-not (Test-Path $ProjectFile)) {
        throw "未找到项目文件: $ProjectFile"
    }

    # 使用 dotnet build
    dotnet build $ProjectFile -c Release --nologo
    
    if ($LASTEXITCODE -ne 0) {
        throw "编译失败，退出代码: $LASTEXITCODE"
    }
    
    Write-Host "  ✓ 编译成功" -ForegroundColor Green
} catch {
    Write-Host "  ✗ 编译失败: $_" -ForegroundColor Red
    exit 1
}

# 验证输出
Write-Host "[3/4] 验证输出文件..." -ForegroundColor Yellow
$DllPath = Join-Path $AssembliesPath "CulinaryArts.dll"
if (Test-Path $DllPath) {
    $DllSize = (Get-Item $DllPath).Length / 1KB
    Write-Host "  ✓ 找到 CulinaryArts.dll (${DllSize:N2} KB)" -ForegroundColor Green
} else {
    Write-Host "  ✗ 未找到 CulinaryArts.dll" -ForegroundColor Red
    exit 1
}

# 部署到RimWorld
if ($Deploy) {
    Write-Host "[4/4] 部署到 RimWorld Mods 目录..." -ForegroundColor Yellow
    
    $ModsPath = Join-Path $RimWorldPath "Mods"
    $TargetModPath = Join-Path $ModsPath $ModName
    
    if (-not (Test-Path $ModsPath)) {
        Write-Host "  ✗ 未找到 RimWorld Mods 目录: $ModsPath" -ForegroundColor Red
        Write-Host "  提示: 使用 -RimWorldPath 参数指定正确的路径" -ForegroundColor Yellow
        exit 1
    }
    
    try {
        # 创建目标目录
        if (-not (Test-Path $TargetModPath)) {
            New-Item -ItemType Directory -Path $TargetModPath -Force | Out-Null
        }
        
        # 复制文件
        $ItemsToCopy = @("About", "Assemblies", "Defs", "Languages")
        foreach ($Item in $ItemsToCopy) {
            $SourceItem = Join-Path $ProjectRoot $Item
            if (Test-Path $SourceItem) {
                $TargetItem = Join-Path $TargetModPath $Item
                Copy-Item -Path $SourceItem -Destination $TargetItem -Recurse -Force
                Write-Host "  ✓ 复制 $Item" -ForegroundColor Green
            }
        }
        
        Write-Host "  ✓ 部署完成: $TargetModPath" -ForegroundColor Green
    } catch {
        Write-Host "  ✗ 部署失败: $_" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "[4/4] 跳过部署（使用 -Deploy 参数启用）" -ForegroundColor Gray
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  构建完成！" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "下一步:" -ForegroundColor Yellow
Write-Host "  1. 使用 -Deploy 参数自动部署到 RimWorld" -ForegroundColor White
Write-Host "  2. 或手动复制到 RimWorld\Mods\$ModName" -ForegroundColor White
Write-Host "  3. 启动游戏并在 Mod 管理器中启用本 mod" -ForegroundColor White
Write-Host ""
