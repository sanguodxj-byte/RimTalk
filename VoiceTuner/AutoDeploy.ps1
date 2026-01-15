# ╔═══════════════════════════════════════════════════════════════════════════════════╗
#  VoiceTuner Mod - 自动部署脚本
#  功能：编译、部署、清理，保证部署一致性
# ╚═══════════════════════════════════════════════════════════════════════════════════╝

param(
    [string]$Mode = "copy",  # copy 或 symlink
    [string]$RimWorldPath = ""
)

# 设置错误处理
$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "╔═══════════════════════════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                                                                                   ║" -ForegroundColor Green
Write-Host "║        VoiceTuner Mod - 自动部署脚本 v1.0                                         ║" -ForegroundColor Green
Write-Host "║                                                                                   ║" -ForegroundColor Green
Write-Host "╚═══════════════════════════════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""

# ╔═══════════════════════════════════════════════════════════════════════════════════╗
# 1. 确认路径
# ╚═══════════════════════════════════════════════════════════════════════════════════╝

Write-Host "🚀 步骤1：确认路径" -ForegroundColor Cyan
Write-Host "--------------------------------------------------------------------------------" -ForegroundColor Gray

# 获取脚本所在目录（Mod根目录）
$modRootPath = (Get-Location).Path
if (-not $modRootPath) {
    $modRootPath = Get-Location
}

Write-Host "  Mod根目录: $modRootPath" -ForegroundColor White

# 源代码目录
$sourceDir = Join-Path $modRootPath "Source\VoiceTuner"
$assembliesDir = Join-Path $modRootPath "Assemblies"

if (-not (Test-Path $sourceDir)) {
    Write-Host "  ❌ 错误：未找到源代码目录" -ForegroundColor Red
    exit 1
}

Write-Host "  ✅ 源代码目录: $sourceDir" -ForegroundColor Green
Write-Host "  ✅ 程序集目录: $assembliesDir" -ForegroundColor Green

# 自动搜索RimWorld安装目录
if ([string]::IsNullOrEmpty($RimWorldPath)) {
    Write-Host ""
    Write-Host "  🔍 正在搜索RimWorld安装目录..." -ForegroundColor Yellow
    
    $searchPaths = @(
        "C:\Program Files (x86)\Steam\steamapps\common\RimWorld",
        "C:\Program Files\Steam\steamapps\common\RimWorld",
        "D:\SteamLibrary\steamapps\common\RimWorld",
        "E:\SteamLibrary\steamapps\common\RimWorld",
        "F:\SteamLibrary\steamapps\common\RimWorld"
    )
    
    foreach ($path in $searchPaths) {
        if (Test-Path $path) {
            $RimWorldPath = $path
            Write-Host "  ✅ 找到RimWorld: $RimWorldPath" -ForegroundColor Green
            break
        }
    }
    
    if ([string]::IsNullOrEmpty($RimWorldPath)) {
        Write-Host ""
        Write-Host "  ❌ 未找到RimWorld安装目录" -ForegroundColor Red
        Write-Host "  请手动指定路径: .\AutoDeploy.ps1 -RimWorldPath 'C:\...\RimWorld'" -ForegroundColor Yellow
        exit 1
    }
}

$modsPath = Join-Path $RimWorldPath "Mods"
$targetPath = Join-Path $modsPath "VoiceTuner"

if (-not (Test-Path $modsPath)) {
    Write-Host "  ❌ 错误：Mods目录不存在: $modsPath" -ForegroundColor Red
    exit 1
}

Write-Host "  ✅ RimWorld Mods: $modsPath" -ForegroundColor Green
Write-Host ""

# ╔═══════════════════════════════════════════════════════════════════════════════════╗
# 2. 编译项目
# ╚═══════════════════════════════════════════════════════════════════════════════════╝

Write-Host "🚀 步骤2：编译项目" -ForegroundColor Cyan
Write-Host "--------------------------------------------------------------------------------" -ForegroundColor Gray

Push-Location $sourceDir

try {
    Write-Host "  清理旧的生成文件..." -ForegroundColor Gray
    & dotnet clean --nologo --verbosity quiet 2>&1 | Out-Null
    
    Write-Host "  正在编译项目..." -ForegroundColor Gray
    $buildOutput = & dotnet build --nologo --verbosity minimal 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  ❌ 编译失败：" -ForegroundColor Red
        Write-Host $buildOutput -ForegroundColor Red
        Pop-Location
        exit 1
    }
    
    Write-Host "  ✅ 编译成功" -ForegroundColor Green
}
finally {
    Pop-Location
}

Write-Host ""

# ╔═══════════════════════════════════════════════════════════════════════════════════╗
# 3. 验证依赖文件
# ╚═══════════════════════════════════════════════════════════════════════════════════╝

Write-Host "🚀 步骤3：验证依赖文件" -ForegroundColor Cyan
Write-Host "--------------------------------------------------------------------------------" -ForegroundColor Gray

$requiredDlls = @(
    "VoiceTuner.dll",
    "0Harmony.dll",
    "Newtonsoft.Json.dll",
    "System.Net.Http.dll"
)

$allDllsPresent = $true
foreach ($dll in $requiredDlls) {
    $dllPath = Join-Path $assembliesDir $dll
    if (Test-Path $dllPath) {
        $size = [math]::Round((Get-Item $dllPath).Length / 1KB, 2)
        Write-Host "  ✅ $dll ($size KB)" -ForegroundColor Green
    }
    else {
        Write-Host "  ❌ $dll 缺失" -ForegroundColor Red
        $allDllsPresent = $false
    }
}

if (-not $allDllsPresent) {
    Write-Host ""
    Write-Host "  ❌ 错误：缺少必要的DLL文件" -ForegroundColor Red
    
    # 尝试自动复制System.Net.Http.dll
    if (-not (Test-Path (Join-Path $assembliesDir "System.Net.Http.dll"))) {
        Write-Host "  正在从NuGet缓存复制System.Net.Http.dll..." -ForegroundColor Yellow
        $nugetDll = "$env:USERPROFILE\.nuget\packages\system.net.http\4.3.4\lib\net46\System.Net.Http.dll"
        if (Test-Path $nugetDll) {
            Copy-Item $nugetDll $assembliesDir -Force
            Write-Host "  ✅ 已复制System.Net.Http.dll" -ForegroundColor Green
        }
        else {
            Write-Host "  ❌ 无法找到System.Net.Http.dll" -ForegroundColor Red
            exit 1
        }
    }
}

Write-Host ""

# ╔═══════════════════════════════════════════════════════════════════════════════════╗
# 4. 部署到RimWorld
# ╚═══════════════════════════════════════════════════════════════════════════════════╝

Write-Host "🚀 步骤4：部署到RimWorld" -ForegroundColor Cyan
Write-Host "--------------------------------------------------------------------------------" -ForegroundColor Gray

if ($Mode -eq "symlink") {
    Write-Host "  模式：符号链接（需要管理员权限）" -ForegroundColor Yellow
    
    if (Test-Path $targetPath) {
        Write-Host "  删除旧的目标目录..." -ForegroundColor Gray
        Remove-Item $targetPath -Recurse -Force
    }
    
    try {
        New-Item -ItemType SymbolicLink -Path $targetPath -Target $modRootPath -ErrorAction Stop | Out-Null
        Write-Host "  ✅ 符号链接创建成功" -ForegroundColor Green
        Write-Host "  提示：源代码修改后只需重启游戏即可生效" -ForegroundColor Cyan
    }
    catch {
        Write-Host "  ❌ 符号链接创建失败，需要管理员权限！" -ForegroundColor Red
        Write-Host "  切换到复制模式..." -ForegroundColor Yellow
        $Mode = "copy"
    }
}

if ($Mode -eq "copy") {
    Write-Host "  模式：文件复制" -ForegroundColor Yellow
    
    # 确保目标目录存在
        if (-not (Test-Path $targetPath)) {
            New-Item -ItemType Directory -Path $targetPath -Force | Out-Null
        }
    
        # 复制新的Mod文件（强制覆盖）
        Write-Host "  正在复制文件（强制覆盖）..." -ForegroundColor Gray
        # 确保目标目录存在
        if (-not (Test-Path $targetPath)) {
            New-Item -ItemType Directory -Path $targetPath -Force | Out-Null
        }
        # 复制所有内容
        Copy-Item -Path "$modRootPath\*" -Destination $targetPath -Recurse -Force
        Write-Host "  ✅ 文件覆盖完成" -ForegroundColor Green
}

Write-Host ""

# ╔═══════════════════════════════════════════════════════════════════════════════════╗
# 5. 清理不必要的文件
# ╚═══════════════════════════════════════════════════════════════════════════════════╝

Write-Host "🚀 步骤5：清理部署文件" -ForegroundColor Cyan
Write-Host "--------------------------------------------------------------------------------" -ForegroundColor Gray

$excludePatterns = @(
    "*.pdb",
    "*.user",
    "*.suo",
    "obj",
    "bin",
    ".vs",
    ".git",
    "*.backup_*",
    "AutoDeploy.ps1",
    "Deploy.ps1",
    "PrepareRelease.ps1",
    "BUILD_REPORT.md",
    "DEPLOY_GUIDE.md",
    "DEPLOYMENT_COMPLETE.md",
    "DEPLOYMENT_FIX_REPORT.txt",
    "global.json",
    "Build.ps1"
)

$cleanedCount = 0
foreach ($pattern in $excludePatterns) {
    $items = Get-ChildItem $targetPath -Recurse -Include $pattern -Force -ErrorAction SilentlyContinue
    foreach ($item in $items) {
        Remove-Item $item -Recurse -Force -ErrorAction SilentlyContinue
        $cleanedCount++
    }
}

Write-Host "  ✅ 清理了 $cleanedCount 个不必要的文件" -ForegroundColor Green
Write-Host ""

# ╔═══════════════════════════════════════════════════════════════════════════════════╗
# 6. 验证部署
# ╚═══════════════════════════════════════════════════════════════════════════════════╝

Write-Host "? 步骤6：验证部署" -ForegroundColor Cyan
Write-Host "--------------------------------------------------------------------------------" -ForegroundColor Gray

$targetAbout = Join-Path $targetPath "About\About.xml"
$targetAssemblies = Join-Path $targetPath "Assemblies"

$verifyItems = @(
    @{Name = "About.xml"; Path = $targetAbout},
    @{Name = "VoiceTuner.dll"; Path = (Join-Path $targetAssemblies "VoiceTuner.dll")},
    @{Name = "0Harmony.dll"; Path = (Join-Path $targetAssemblies "0Harmony.dll")},
    @{Name = "Newtonsoft.Json.dll"; Path = (Join-Path $targetAssemblies "Newtonsoft.Json.dll")},
    @{Name = "System.Net.Http.dll"; Path = (Join-Path $targetAssemblies "System.Net.Http.dll")}
)

$allVerified = $true
foreach ($item in $verifyItems) {
    if (Test-Path $item.Path) {
        if ($item.Name -like "*.dll") {
            $size = [math]::Round((Get-Item $item.Path).Length / 1KB, 2)
            Write-Host "  ✅ $($item.Name) ($size KB)" -ForegroundColor Green
        }
        else {
            Write-Host "  ✅ $($item.Name)" -ForegroundColor Green
        }
    }
    else {
        Write-Host "  ❌ $($item.Name) 缺失" -ForegroundColor Red
        $allVerified = $false
    }
}

Write-Host ""

# ╔═══════════════════════════════════════════════════════════════════════════════════╗
# 7. 完成总结
# ╚═══════════════════════════════════════════════════════════════════════════════════╝

if ($allVerified) {
    Write-Host "╔═══════════════════════════════════════════════════════════════════════════════════╗" -ForegroundColor Green
    Write-Host "║                                                                                   ║" -ForegroundColor Green
    Write-Host "║                  ✅ 部署成功完成！                                                  ║" -ForegroundColor Green
    Write-Host "║                                                                                   ║" -ForegroundColor Green
    Write-Host "╚═══════════════════════════════════════════════════════════════════════════════════╝" -ForegroundColor Green
    Write-Host ""
    Write-Host "📍 部署位置:" -ForegroundColor Cyan
    Write-Host "   $targetPath" -ForegroundColor White
    Write-Host ""
    Write-Host "🎮 如何在游戏中使用:" -ForegroundColor Cyan
    Write-Host "   1. 启动RimWorld" -ForegroundColor White
    Write-Host "   2. 在Mod列表中启用 'Voice Tuner'" -ForegroundColor White
    Write-Host "   3. 重启游戏" -ForegroundColor White
    Write-Host "   4. 按 Ctrl+T 打开语音调整窗口" -ForegroundColor White
    Write-Host ""
    Write-Host "💡 开发者提示:" -ForegroundColor Cyan
    Write-Host "   - 推荐使用Azure TTS以获得最佳稳定性" -ForegroundColor Gray
    Write-Host "   - 需要API密钥才能使用TTS服务" -ForegroundColor Gray
    Write-Host "   - 详细说明请查看README.md" -ForegroundColor Gray
    Write-Host ""
    
    exit 0
}
else {
    Write-Host "╔═══════════════════════════════════════════════════════════════════════════════════╗" -ForegroundColor Red
    Write-Host "║                                                                                   ║" -ForegroundColor Red
    Write-Host "║                  ❌ 部署验证失败                                                    ║" -ForegroundColor Red
    Write-Host "║                                                                                   ║" -ForegroundColor Red
    Write-Host "╚═══════════════════════════════════════════════════════════════════════════════════╝" -ForegroundColor Red
    Write-Host ""
    Write-Host "请检查上面的错误信息并重试" -ForegroundColor Yellow
    Write-Host ""
    
    exit 1
}
