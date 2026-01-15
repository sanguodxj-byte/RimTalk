Add-Type -AssemblyName System.Drawing

$targetDir = "D:\rim mod\[TSS]Sideria - Dragon Guard\Textures\UI\Abilities"

# CalamityThrow - 红色圆形
$bmp = New-Object System.Drawing.Bitmap(64, 64)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::FromArgb(50, 50, 50))
$brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::Crimson)
$g.FillEllipse($brush, 6, 6, 52, 52)
$brush.Dispose()
$g.Dispose()
$bmp.Save("$targetDir\CalamityThrow.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-Host "Created CalamityThrow.png"

# CrimsonBloom - 深红多层圆形
$bmp = New-Object System.Drawing.Bitmap(64, 64)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::FromArgb(50, 50, 50))
$brush1 = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::DarkRed)
$g.FillEllipse($brush1, 4, 4, 56, 56)
$brush1.Dispose()
$brush2 = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::Red)
$g.FillEllipse($brush2, 14, 14, 36, 36)
$brush2.Dispose()
$g.Dispose()
$bmp.Save("$targetDir\CrimsonBloom.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-Host "Created CrimsonBloom.png"

# DragonGate - 金色三角形
$bmp = New-Object System.Drawing.Bitmap(64, 64)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::FromArgb(50, 50, 50))
$brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::Goldenrod)
$points = @(
    [System.Drawing.Point]::new(32, 4),
    [System.Drawing.Point]::new(4, 60),
    [System.Drawing.Point]::new(60, 60)
)
$g.FillPolygon($brush, $points)
$brush.Dispose()
$g.Dispose()
$bmp.Save("$targetDir\DragonGate.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-Host "Created DragonGate.png"

Write-Host "`nAll icons created. Listing files:"
Get-ChildItem $targetDir | Format-Table Name, Length -AutoSize