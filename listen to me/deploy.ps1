# 自动部署脚本 - Listen To Me Mod
# Deploy to RimWorld Mods folder
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Listen To Me - 自动部署脚本" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# 路径配置
$sourcePath = "C:\Users\Administrator\Desktop\rim mod\listen to me"
$targetPath = "D:\Steam\steamapps\common\RimWorld\Mods\ListenToMe"

# 检查源目录
if (!(Test-Path $sourcePath)) {
    Write-Host "错误: 源目录不存在: $sourcePath" -ForegroundColor Red
    Read-Host "按 Enter 键退出"
    exit 1
}

# 检查 DLL 是否存在
$dllPath = Join-Path $sourcePath "Assemblies\ListenToMe.dll"
if (!(Test-Path $dllPath)) {
    Write-Host "警告: 未找到 DLL 文件，正在编译..." -ForegroundColor Yellow
    Write-Host ""
    
    # 自动编译
    Push-Location (Join-Path $sourcePath "Source\ListenToMe")
    try {
        Write-Host "还原 NuGet 包..." -ForegroundColor Yellow
        dotnet restore --verbosity quiet
        
        Write-Host "编译项目..." -ForegroundColor Yellow
        dotnet build -c Release --no-restore
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "编译失败！" -ForegroundColor Red
            Pop-Location
            Read-Host "按 Enter 键退出"
            exit 1
        }
        
        Write-Host "编译成功！" -ForegroundColor Green
        Write-Host ""
    }
    finally {
        Pop-Location
    }
}

# 显示文件信息
$dll = Get-Item $dllPath
Write-Host "DLL 信息:" -ForegroundColor Cyan
Write-Host "  大小: $([math]::Round($dll.Length / 1KB, 2)) KB" -ForegroundColor White
Write-Host "  修改时间: $($dll.LastWriteTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor White
Write-Host ""

# 询问部署方式
Write-Host "请选择部署方式:" -ForegroundColor Yellow
Write-Host "  1. 创建符号链接 (推荐，方便开发)" -ForegroundColor White
Write-Host "  2. 复制文件 (适合最终发布)" -ForegroundColor White
Write-Host "  3. 取消部署" -ForegroundColor White
Write-Host ""

$choice = Read-Host "请输入选项 (1/2/3)"

switch ($choice) {
    "1" {
        # 创建符号链接
        Write-Host ""
        Write-Host "创建符号链接..." -ForegroundColor Yellow
        
        # 检查目标是否已存在
        if (Test-Path $targetPath) {
            $existing = Get-Item $targetPath
            if ($existing.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
                Write-Host "符号链接已存在，正在删除..." -ForegroundColor Yellow
                Remove-Item $targetPath -Force
            } else {
                Write-Host "目标目录已存在（非符号链接）" -ForegroundColor Yellow
                $confirm = Read-Host "是否删除并重新创建? (Y/N)"
                if ($confirm -eq 'Y' -or $confirm -eq 'y') {
                    Remove-Item $targetPath -Recurse -Force
                } else {
                    Write-Host "部署已取消" -ForegroundColor Yellow
                    Read-Host "按 Enter 键退出"
                    exit 0
                }
            }
        }
        
        # 创建符号链接（需要管理员权限）
        try {
            $result = New-Item -ItemType SymbolicLink -Path $targetPath -Target $sourcePath -ErrorAction Stop
            Write-Host ""
            Write-Host "==========================================" -ForegroundColor Green
            Write-Host "? 符号链接创建成功！" -ForegroundColor Green
            Write-Host "==========================================" -ForegroundColor Green
            Write-Host ""
            Write-Host "链接信息:" -ForegroundColor Cyan
            Write-Host "  源路径: $sourcePath" -ForegroundColor White
            Write-Host "  目标: $targetPath" -ForegroundColor White
            Write-Host ""
            Write-Host "优势:" -ForegroundColor Yellow
            Write-Host "  ? 修改代码后只需重新编译" -ForegroundColor White
            Write-Host "  ? 无需每次复制文件" -ForegroundColor White
            Write-Host "  ? 方便调试和开发" -ForegroundColor White
        }
        catch {
            Write-Host ""
            Write-Host "错误: 创建符号链接失败" -ForegroundColor Red
            Write-Host "原因: $_" -ForegroundColor Red
            Write-Host ""
            Write-Host "可能的原因:" -ForegroundColor Yellow
            Write-Host "  1. 需要管理员权限" -ForegroundColor White
            Write-Host "  2. 目标目录权限不足" -ForegroundColor White
            Write-Host ""
            Write-Host "解决方案:" -ForegroundColor Yellow
            Write-Host "  1. 以管理员身份运行 PowerShell" -ForegroundColor White
            Write-Host "  2. 或使用选项 2 (复制文件)" -ForegroundColor White
            Read-Host "按 Enter 键退出"
            exit 1
        }
    }
    
    "2" {
        # 复制文件
        Write-Host ""
        Write-Host "复制文件到 RimWorld Mods 目录..." -ForegroundColor Yellow
        
        # 删除旧文件
        if (Test-Path $targetPath) {
            Write-Host "删除旧版本..." -ForegroundColor Yellow
            Remove-Item $targetPath -Recurse -Force
        }
        
        # 创建目标目录
        New-Item -ItemType Directory -Path $targetPath -Force | Out-Null
        
        # 复制文件
        Write-Host "复制文件..." -ForegroundColor Yellow
        
        # 需要复制的目录
        $folders = @("About", "Assemblies", "Defs", "Languages", "Patches")
        
        foreach ($folder in $folders) {
            $srcFolder = Join-Path $sourcePath $folder
            $dstFolder = Join-Path $targetPath $folder
            
            if (Test-Path $srcFolder) {
                Write-Host "  复制 $folder..." -ForegroundColor Gray
                Copy-Item -Path $srcFolder -Destination $dstFolder -Recurse -Force
            }
        }
        
        # 复制根目录文件
        $rootFiles = @("README.md", "LICENSE", "QUICKSTART.md", "COMMAND_FORMAT.md")
        foreach ($file in $rootFiles) {
            $srcFile = Join-Path $sourcePath $file
            if (Test-Path $srcFile) {
                Copy-Item -Path $srcFile -Destination $targetPath -Force
            }
        }
        
        Write-Host ""
        Write-Host "==========================================" -ForegroundColor Green
        Write-Host "? 文件复制成功！" -ForegroundColor Green
        Write-Host "==========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "已复制到: $targetPath" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "包含:" -ForegroundColor Yellow
        Write-Host "  ? About/ - Mod 信息" -ForegroundColor White
        Write-Host "  ? Assemblies/ - DLL 文件" -ForegroundColor White
        Write-Host "  ? Defs/ - 游戏定义" -ForegroundColor White
        Write-Host "  ? Languages/ - 本地化" -ForegroundColor White
        Write-Host "  ? Patches/ - XML 补丁" -ForegroundColor White
        Write-Host "  ? README.md - 说明文档" -ForegroundColor White
        
        # 显示文件统计
        $fileCount = (Get-ChildItem -Path $targetPath -Recurse -File).Count
        $totalSize = (Get-ChildItem -Path $targetPath -Recurse -File | Measure-Object -Property Length -Sum).Sum
        Write-Host ""
        Write-Host "统计信息:" -ForegroundColor Cyan
        Write-Host "  文件数: $fileCount" -ForegroundColor White
        Write-Host "  总大小: $([math]::Round($totalSize / 1KB, 2)) KB" -ForegroundColor White
    }
    
    "3" {
        Write-Host ""
        Write-Host "部署已取消" -ForegroundColor Yellow
        Read-Host "按 Enter 键退出"
        exit 0
    }
    
    default {
        Write-Host ""
        Write-Host "无效的选项" -ForegroundColor Red
        Read-Host "按 Enter 键退出"
        exit 1
    }
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "下一步操作:" -ForegroundColor Yellow
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. 启动 RimWorld" -ForegroundColor White
Write-Host "2. 在 Mod 菜单中找到 'Listen To Me - 听我指挥'" -ForegroundColor White
Write-Host "3. 启用 Mod" -ForegroundColor White
Write-Host "4. 重启游戏" -ForegroundColor White
Write-Host "5. 开始使用！试试 '去厨房做饭' 指令" -ForegroundColor White
Write-Host ""
Write-Host "提示:" -ForegroundColor Yellow
if ($choice -eq "1") {
    Write-Host "  使用符号链接模式，修改代码后只需:" -ForegroundColor Cyan
    Write-Host "    1. 重新编译 (运行 build-sdk9.ps1)" -ForegroundColor White
    Write-Host "    2. 重启 RimWorld" -ForegroundColor White
} else {
    Write-Host "  使用复制模式，修改代码后需要:" -ForegroundColor Cyan
    Write-Host "    1. 重新编译" -ForegroundColor White
    Write-Host "    2. 重新运行此脚本部署" -ForegroundColor White
    Write-Host "    3. 重启 RimWorld" -ForegroundColor White
}
Write-Host ""

# 询问是否启动游戏
$launch = Read-Host "是否现在启动 RimWorld? (Y/N)"
if ($launch -eq 'Y' -or $launch -eq 'y') {
    $gameExe = "D:\Steam\steamapps\common\RimWorld\RimWorldWin64.exe"
    if (Test-Path $gameExe) {
        Write-Host ""
        Write-Host "正在启动 RimWorld..." -ForegroundColor Green
        Start-Process $gameExe
    } else {
        Write-Host ""
        Write-Host "未找到游戏执行文件: $gameExe" -ForegroundColor Yellow
        Write-Host "请手动启动游戏" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "部署完成！" -ForegroundColor Green
Write-Host ""
Read-Host "按 Enter 键退出"
