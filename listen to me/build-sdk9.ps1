# 使用 .NET SDK 9.0 编译脚本
Write-Host "=========================================="  -ForegroundColor Cyan
Write-Host "ListenToMe Mod - Build with .NET 9.0 SDK" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# 路径配置
$dllPath = "C:\Users\Administrator\Desktop\rim mod"
$projectPath = "C:\Users\Administrator\Desktop\rim mod\listen to me"
$sourcePath = "$projectPath\Source\ListenToMe"
$outputPath = "$projectPath\Assemblies"

# 创建输出目录
Write-Host "创建输出目录..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

# 检查 DLL 文件
Write-Host "检查必需的 DLL 文件..." -ForegroundColor Yellow
$requiredDlls = @(
    "$dllPath\Assembly-CSharp.dll",
    "$dllPath\UnityEngine.CoreModule.dll",
    "$dllPath\UnityEngine.dll",
    "$dllPath\UnityEngine.IMGUIModule.dll",
    "$dllPath\0Harmony.dll"
)

$missingDlls = @()
foreach ($dll in $requiredDlls) {
    if (!(Test-Path $dll)) {
        $missingDlls += $dll
        Write-Host "  ? 缺失: $(Split-Path $dll -Leaf)" -ForegroundColor Red
    } else {
        Write-Host "  ? 找到: $(Split-Path $dll -Leaf)" -ForegroundColor Green
    }
}

if ($missingDlls.Count -gt 0) {
    Write-Host ""
    Write-Host "错误: 缺少必需的 DLL 文件！" -ForegroundColor Red
    Write-Host "请确保从 RimWorld 安装目录复制以下文件到: $dllPath" -ForegroundColor Yellow
    $missingDlls | ForEach-Object { Write-Host "  - $(Split-Path $_ -Leaf)" -ForegroundColor Yellow }
    Read-Host "`n按 Enter 键退出"
    exit 1
}

Write-Host ""
Write-Host "切换到项目目录..." -ForegroundColor Yellow
Push-Location $sourcePath

try {
    # 检查 .NET SDK 版本
    Write-Host "检查 .NET SDK 版本..." -ForegroundColor Yellow
    $sdkVersion = dotnet --version
    Write-Host "当前 SDK 版本: $sdkVersion" -ForegroundColor Cyan
    
    if ($sdkVersion -notmatch "^9\.") {
        Write-Host "警告: 推荐使用 .NET SDK 9.0" -ForegroundColor Yellow
        Write-Host "当前版本: $sdkVersion" -ForegroundColor Yellow
        $continue = Read-Host "是否继续? (Y/N)"
        if ($continue -ne 'Y' -and $continue -ne 'y') {
            exit 0
        }
    }
    
    Write-Host ""
    Write-Host "步骤 1: 清理项目..." -ForegroundColor Yellow
    if (Test-Path "obj") { Remove-Item -Recurse -Force "obj" -ErrorAction SilentlyContinue }
    if (Test-Path "bin") { Remove-Item -Recurse -Force "bin" -ErrorAction SilentlyContinue }
    if (Test-Path "$outputPath\ListenToMe.dll") { 
        Remove-Item "$outputPath\ListenToMe.dll" -Force -ErrorAction SilentlyContinue 
    }
    Write-Host "  ? 清理完成" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "步骤 2: 还原 NuGet 包..." -ForegroundColor Yellow
    dotnet restore --verbosity quiet
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  ? NuGet 包还原失败！" -ForegroundColor Red
        Read-Host "`n按 Enter 键退出"
        exit 1
    }
    Write-Host "  ? NuGet 包还原成功" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "步骤 3: 编译项目..." -ForegroundColor Yellow
    Write-Host "使用配置: Release" -ForegroundColor Cyan
    Write-Host "目标框架: net472" -ForegroundColor Cyan
    Write-Host ""
    
    # 使用 dotnet build 而不是 msbuild
    dotnet build ListenToMe.csproj `
        -c Release `
        -p:Platform=AnyCPU `
        -p:DebugType=portable `
        --no-restore `
        /clp:ErrorsOnly
    
    $buildResult = $LASTEXITCODE
    
    Write-Host ""
    if ($buildResult -eq 0) {
        Write-Host "==========================================" -ForegroundColor Green
        Write-Host "? 编译成功！" -ForegroundColor Green
        Write-Host "==========================================" -ForegroundColor Green
        
        # 检查输出文件
        if (Test-Path "$outputPath\ListenToMe.dll") {
            $dllInfo = Get-Item "$outputPath\ListenToMe.dll"
            Write-Host ""
            Write-Host "输出信息:" -ForegroundColor Cyan
            Write-Host "  位置: $outputPath\ListenToMe.dll" -ForegroundColor White
            Write-Host "  大小: $([math]::Round($dllInfo.Length / 1KB, 2)) KB" -ForegroundColor White
            Write-Host "  创建时间: $($dllInfo.LastWriteTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor White
            
            Write-Host ""
            Write-Host "下一步:" -ForegroundColor Cyan
            Write-Host "  1. 将 'listen to me' 文件夹复制到 RimWorld\Mods\" -ForegroundColor White
            Write-Host "  2. 或使用符号链接: mklink /D ""RimWorld\Mods\ListenToMe"" ""$projectPath""" -ForegroundColor White
            Write-Host "  3. 在游戏中启用 mod" -ForegroundColor White
        } else {
            Write-Host "警告: 未找到输出 DLL 文件" -ForegroundColor Yellow
        }
    } else {
        Write-Host "==========================================" -ForegroundColor Red
        Write-Host "? 编译失败！" -ForegroundColor Red
        Write-Host "==========================================" -ForegroundColor Red
        Write-Host "错误代码: $buildResult" -ForegroundColor Red
        Write-Host ""
        Write-Host "故障排除建议:" -ForegroundColor Yellow
        Write-Host "  1. 检查上面的错误消息" -ForegroundColor White
        Write-Host "  2. 确认所有 DLL 文件路径正确" -ForegroundColor White
        Write-Host "  3. 尝试使用 Visual Studio 打开项目并编译" -ForegroundColor White
        Write-Host "  4. 查看 BUILD.md 获取详细说明" -ForegroundColor White
    }
    
} catch {
    Write-Host ""
    Write-Host "发生异常: $_" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
} finally {
    Pop-Location
}

Write-Host ""
Read-Host "按 Enter 键退出"
