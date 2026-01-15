# VoiceTuner Build Script
# 用于编译和部署VoiceTuner mod

param(
    [switch]$Release,
    [switch]$Deploy
)

$ErrorActionPreference = "Stop"

# 配置
$ModName = "VoiceTuner"
$SourcePath = "Source\VoiceTuner"
$AssembliesPath = "Assemblies"
$Configuration = if ($Release) { "Release" } else { "Debug" }

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Building $ModName ($Configuration)" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

# 确保Assemblies目录存在
if (!(Test-Path $AssembliesPath)) {
    New-Item -ItemType Directory -Path $AssembliesPath -Force | Out-Null
}

# 复制0Harmony.dll（如果不存在）
$HarmonySource = "..\The Second Seat\Assemblies\0Harmony.dll"
$HarmonyDest = "$AssembliesPath\0Harmony.dll"
if (!(Test-Path $HarmonyDest) -and (Test-Path $HarmonySource)) {
    Copy-Item $HarmonySource $HarmonyDest
    Write-Host "Copied 0Harmony.dll" -ForegroundColor Green
}

# 编译项目
Write-Host "`nCompiling..." -ForegroundColor Yellow
try {
    dotnet build "$SourcePath\VoiceTuner.csproj" -c $Configuration
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }
    Write-Host "Build successful!" -ForegroundColor Green
}
catch {
    Write-Host "Build failed: $_" -ForegroundColor Red
    exit 1
}

# 部署到RimWorld Mods目录
if ($Deploy) {
    $RimWorldModsPath = "D:\steam\steamapps\common\RimWorld\Mods"
    
    if (Test-Path $RimWorldModsPath) {
        $DeployPath = Join-Path $RimWorldModsPath $ModName
        
        Write-Host "`nDeploying to $DeployPath..." -ForegroundColor Yellow
        
        # 创建目标目录
        if (!(Test-Path $DeployPath)) {
            New-Item -ItemType Directory -Path $DeployPath -Force | Out-Null
        }
        
        # 复制文件
        $FilesToCopy = @(
            "About",
            "Assemblies",
            "Languages"
        )
        
        foreach ($folder in $FilesToCopy) {
            if (Test-Path $folder) {
                Copy-Item -Path $folder -Destination $DeployPath -Recurse -Force
                Write-Host "  Copied $folder" -ForegroundColor Gray
            }
        }
        
        Write-Host "Deployment complete!" -ForegroundColor Green
    }
    else {
        Write-Host "RimWorld Mods directory not found: $RimWorldModsPath" -ForegroundColor Red
    }
}

Write-Host "`n=====================================" -ForegroundColor Cyan
Write-Host "Build complete!" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan