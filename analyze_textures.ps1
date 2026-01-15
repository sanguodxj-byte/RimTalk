Add-Type -AssemblyName System.Drawing

$textureDir = "[TSS]Sideria - Dragon Guard/Textures/Sideria/Narrators/Layered"
Write-Output "Analyzing textures in $textureDir..."

if (-not (Test-Path $textureDir)) {
    Write-Error "Directory not found: $textureDir"
    exit
}

$files = Get-ChildItem -Path $textureDir -Filter "*.png" | Sort-Object Name

foreach ($file in $files) {
    try {
        $img = [System.Drawing.Bitmap]::FromFile($file.FullName)
        $width = $img.Width
        $height = $img.Height
        
        $minX = $width
        $minY = $height
        $maxX = 0
        $maxY = 0
        $pixels = 0
        
        for ($x = 0; $x -lt $width; $x++) {
            for ($y = 0; $y -lt $height; $y++) {
                $pixel = $img.GetPixel($x, $y)
                if ($pixel.A -gt 0) {
                    $pixels++
                    if ($x -lt $minX) { $minX = $x }
                    if ($y -lt $minY) { $minY = $y }
                    if ($x -gt $maxX) { $maxX = $x }
                    if ($y -gt $maxY) { $maxY = $y }
                }
            }
        }
        
        $img.Dispose()
        
        if ($pixels -eq 0) {
             Write-Output "$($file.Name): Empty"
        } else {
             # BBox format: (left, upper, right, lower)
             # Right and lower are exclusive to match PIL behavior for easier comparison if needed
             $right = $maxX + 1
             $bottom = $maxY + 1
             $bbox = "($minX, $minY, $right, $bottom)"
             
             $w = $right - $minX
             $h = $bottom - $minY
             
             Write-Output "$($file.Name): Size: ${w}x${h}, Area: $pixels, BBox: $bbox"
        }
    } catch {
        Write-Output "$($file.Name): Error - $_"
    }
}
