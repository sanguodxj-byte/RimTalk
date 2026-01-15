# 将 1.6 DLL 复制到 1.5 目录进行测试
# ?? 警告：这是实验性操作，可能导致游戏崩溃

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Yellow
Write-Host "  实验性操作：1.5 使用 1.6 DLL 测试" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "?? 警告：此操作可能导致游戏无法启动或崩溃！" -ForegroundColor Red
Write-Host "请先备份 1.5\Assemblies 目录！" -ForegroundColor Red
Write-Host ""

$Confirm = Read-Host "是否继续？(输入 YES 继续)"
if ($Confirm -ne "YES") {
    Write-Host "操作已取消" -ForegroundColor Green
    exit 0
}

# 备份 1.5 目录
$BackupDir = "1.5\Assemblies_Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
Write-Host ""
Write-Host "[备份] 正在备份原始 1.5 DLL..." -ForegroundColor Cyan
Copy-Item -Path "1.5\Assemblies" -Destination $BackupDir -Recurse
Write-Host "  ? 备份完成: $BackupDir" -ForegroundColor Green

# 复制 1.6 DLL 到 1.5
Write-Host ""
Write-Host "[复制] 将 1.6 DLL 复制到 1.5..." -ForegroundColor Cyan

# 复制主 DLL
Copy-Item -Path "1.6\Assemblies\RimTalkMemoryPatch.dll" -Destination "1.5\Assemblies\" -Force
Write-Host "  ? RimTalkMemoryPatch.dll (376.5 KB)" -ForegroundColor Green

# 复制 Newtonsoft.Json.dll
Copy-Item -Path "1.6\Assemblies\Newtonsoft.Json.dll" -Destination "1.5\Assemblies\" -Force
Write-Host "  ? Newtonsoft.Json.dll (695 KB)" -ForegroundColor Green

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  复制完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "测试步骤：" -ForegroundColor Cyan
Write-Host "1. 启动 RimWorld 1.5" -ForegroundColor White
Write-Host "2. 检查是否有红色错误提示" -ForegroundColor White
Write-Host "3. 尝试打开 Memory 标签页" -ForegroundColor White
Write-Host "4. 测试常识库功能" -ForegroundColor White
Write-Host ""
Write-Host "如果出现问题，请恢复备份：" -ForegroundColor Yellow
Write-Host "  Remove-Item '1.5\Assemblies' -Recurse -Force" -ForegroundColor Gray
Write-Host "  Copy-Item '$BackupDir' '1.5\Assemblies' -Recurse" -ForegroundColor Gray
Write-Host ""
