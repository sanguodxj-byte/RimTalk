# 紧急恢复和重新编译脚本
$ErrorActionPreference = "Stop"
$modPath = "C:\Users\Administrator\Desktop\rim mod\listen to me"

Write-Host "`n?? 紧急恢复 - Listen To Me Mod" -ForegroundColor Yellow
Write-Host "====================================`n" -ForegroundColor Yellow

# 切换到 Mod 目录
Set-Location $modPath

# 1. 检查游戏是否运行
Write-Host "1. 检查游戏状态..." -ForegroundColor Cyan
$rimworld = Get-Process "RimWorldWin64" -ErrorAction SilentlyContinue
if ($rimworld) {
    Write-Host "   ??  RimWorld 正在运行！" -ForegroundColor Red
    Write-Host "   请先关闭游戏，然后按任意键继续..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    
    # 再次检查
    $rimworld = Get-Process "RimWorldWin64" -ErrorAction SilentlyContinue
    if ($rimworld) {
        Write-Host "   ? 游戏仍在运行，退出" -ForegroundColor Red
        exit 1
    }
}
Write-Host "   ? 游戏已关闭" -ForegroundColor Green

# 2. 备份当前 DLL
Write-Host "`n2. 备份文件..." -ForegroundColor Cyan
if (Test-Path "Assemblies\ListenToMe.dll") {
    $backupName = "Assemblies\ListenToMe.dll.backup." + (Get-Date -Format "yyyyMMdd_HHmmss")
    Copy-Item "Assemblies\ListenToMe.dll" $backupName
    Write-Host "   ? 已备份到: $backupName" -ForegroundColor Green
} else {
    Write-Host "   ! DLL 不存在，跳过备份" -ForegroundColor Yellow
}

# 3. 清理编译输出
Write-Host "`n3. 清理编译输出..." -ForegroundColor Cyan
Remove-Item "Source\ListenToMe\obj" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "Source\ListenToMe\bin" -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "   ? 已清理 obj 和 bin 文件夹" -ForegroundColor Green

# 4. 重新编译
Write-Host "`n4. 重新编译..." -ForegroundColor Cyan
Write-Host "   执行: .\build-sdk9.ps1" -ForegroundColor Gray

& .\build-sdk9.ps1

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n? 编译成功！" -ForegroundColor Green
    
    # 检查 DLL
    if (Test-Path "Assemblies\ListenToMe.dll") {
        $dll = Get-Item "Assemblies\ListenToMe.dll"
        Write-Host "`n?? DLL 信息:" -ForegroundColor Cyan
        Write-Host "   路径: $($dll.FullName)" -ForegroundColor White
        Write-Host "   大小: $([math]::Round($dll.Length / 1KB, 2)) KB" -ForegroundColor White
        Write-Host "   时间: $($dll.LastWriteTime)" -ForegroundColor White
        
        # 检查符号链接
        Write-Host "`n?? 检查部署状态:" -ForegroundColor Cyan
        $deployPath = "D:\Steam\steamapps\common\RimWorld\Mods\ListenToMe"
        if (Test-Path $deployPath) {
            $deployItem = Get-Item $deployPath
            if ($deployItem.LinkType -eq "SymbolicLink") {
                Write-Host "   ? 符号链接正常" -ForegroundColor Green
                Write-Host "   目标: $($deployItem.Target)" -ForegroundColor Gray
            } else {
                Write-Host "   ??  不是符号链接" -ForegroundColor Yellow
            }
        } else {
            Write-Host "   ? 部署路径不存在" -ForegroundColor Red
        }
        
        Write-Host "`n====================================`n" -ForegroundColor Green
        Write-Host "? 恢复完成！" -ForegroundColor Green
        Write-Host "`n?? 下一步操作:" -ForegroundColor Cyan
        Write-Host "   1. 启动 RimWorld" -ForegroundColor White
        Write-Host "   2. 进入游戏，加载存档" -ForegroundColor White
        Write-Host "   3. 选中一个小人" -ForegroundColor White
        Write-Host "   4. 查看底部是否有'文本指令'按钮" -ForegroundColor White
        Write-Host "`n?? 如果按钮还是不显示:" -ForegroundColor Yellow
        Write-Host "   - 查看游戏日志（搜索 [ListenToMe]）" -ForegroundColor Gray
        Write-Host "   - 确认 Mod 在列表中已启用" -ForegroundColor Gray
        Write-Host "   - 尝试禁用其他 Mod 测试" -ForegroundColor Gray
        Write-Host "`n?? 日志位置:" -ForegroundColor Yellow
        Write-Host "   $env:USERPROFILE\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log" -ForegroundColor Gray
        
    } else {
        Write-Host "`n? DLL 文件未生成！" -ForegroundColor Red
        Write-Host "请检查编译错误" -ForegroundColor Yellow
    }
} else {
    Write-Host "`n? 编译失败！" -ForegroundColor Red
    Write-Host "请查看上面的错误信息" -ForegroundColor Yellow
    Write-Host "`n常见问题:" -ForegroundColor Yellow
    Write-Host "   - 缺少引用的 DLL" -ForegroundColor Gray
    Write-Host "   - 代码语法错误" -ForegroundColor Gray
    Write-Host "   - .NET SDK 版本不匹配" -ForegroundColor Gray
}

Write-Host "`n按任意键退出..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
