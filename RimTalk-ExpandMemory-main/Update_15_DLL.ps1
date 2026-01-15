# 安全方案：更新 1.5 目录的 DLL（推荐）
# 这样 1.5 和 1.6 都使用相同的新版本 DLL

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  更新 1.5 DLL 为最新版本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 备份 1.5 目录
$BackupDir = "1.5\Assemblies_Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
Write-Host "[备份] 正在备份原始 1.5 DLL..." -ForegroundColor Cyan
Copy-Item -Path "1.5\Assemblies" -Destination $BackupDir -Recurse
Write-Host "  ? 备份完成: $BackupDir" -ForegroundColor Green

# 复制 1.6 DLL 到 1.5
Write-Host ""
Write-Host "[更新] 将最新 DLL 复制到 1.5..." -ForegroundColor Cyan

# 复制主 DLL
Copy-Item -Path "1.6\Assemblies\RimTalkMemoryPatch.dll" -Destination "1.5\Assemblies\" -Force
Write-Host "  ? RimTalkMemoryPatch.dll (376.5 KB)" -ForegroundColor Green

# 复制 Newtonsoft.Json.dll（如果不存在）
if (-not (Test-Path "1.5\Assemblies\Newtonsoft.Json.dll")) {
    Copy-Item -Path "1.6\Assemblies\Newtonsoft.Json.dll" -Destination "1.5\Assemblies\" -Force
    Write-Host "  ? Newtonsoft.Json.dll (695 KB) [新增]" -ForegroundColor Green
} else {
    Write-Host "  ? Newtonsoft.Json.dll 已存在" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  更新完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "现在 1.5 和 1.6 都将使用相同的新版本 DLL" -ForegroundColor Yellow
Write-Host ""
Write-Host "包含的新功能：" -ForegroundColor Cyan
Write-Host "  1. ? 事件记录时间修复" -ForegroundColor White
Write-Host "  2. ? 扩展属性默认启用" -ForegroundColor White
Write-Host "  3. ? 左侧绿色常识库按钮" -ForegroundColor White
Write-Host ""
Write-Host "如需恢复旧版本：" -ForegroundColor Yellow
Write-Host "  Remove-Item '1.5\Assemblies' -Recurse -Force" -ForegroundColor Gray
Write-Host "  Copy-Item '$BackupDir' '1.5\Assemblies' -Recurse" -ForegroundColor Gray
Write-Host ""
