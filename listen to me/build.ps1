# PowerShell Build Script for ListenToMe Mod
Write-Host "=========================================="  -ForegroundColor Cyan
Write-Host "Building ListenToMe Mod for RimWorld" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Paths
$dllPath = "C:\Users\Administrator\Desktop\rim mod"
$projectPath = "C:\Users\Administrator\Desktop\rim mod\listen to me"
$sourcePath = "$projectPath\Source\ListenToMe"
$outputPath = "$projectPath\Assemblies"

# Create output directory
Write-Host "Creating output directory..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

# Check if DLLs exist
Write-Host "Checking required DLLs..." -ForegroundColor Yellow
$requiredDlls = @(
    "$dllPath\Assembly-CSharp.dll",
    "$dllPath\UnityEngine.CoreModule.dll",
    "$dllPath\UnityEngine.dll",
    "$dllPath\UnityEngine.IMGUIModule.dll",
    "$dllPath\0Harmony.dll"
)

foreach ($dll in $requiredDlls) {
    if (!(Test-Path $dll)) {
        Write-Host "ERROR: Missing DLL: $dll" -ForegroundColor Red
        Read-Host "Press Enter to exit"
        exit 1
    }
}
Write-Host "All required DLLs found!" -ForegroundColor Green

# Try using MSBuild from dotnet SDK
Write-Host ""
Write-Host "Attempting to build using MSBuild..." -ForegroundColor Yellow

Push-Location $sourcePath

try {
    # Clean first
    Write-Host "Cleaning project..." -ForegroundColor Yellow
    if (Test-Path "obj") { Remove-Item -Recurse -Force "obj" }
    if (Test-Path "bin") { Remove-Item -Recurse -Force "bin" }
    
    # Restore NuGet packages
    Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
    dotnet restore ListenToMe.csproj
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "NuGet restore failed!" -ForegroundColor Red
        Read-Host "Press Enter to exit"
        exit 1
    }
    
    # Build
    Write-Host "Building project..." -ForegroundColor Yellow
    msbuild ListenToMe.csproj /p:Configuration=Release /p:Platform=AnyCPU /v:minimal /nologo /t:Rebuild
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "==========================================" -ForegroundColor Green
        Write-Host "BUILD SUCCESSFUL!" -ForegroundColor Green
        Write-Host "==========================================" -ForegroundColor Green
        Write-Host "Output: $outputPath\ListenToMe.dll" -ForegroundColor Green
        
        # Check if DLL was created
        if (Test-Path "$outputPath\ListenToMe.dll") {
            $dllInfo = Get-Item "$outputPath\ListenToMe.dll"
            Write-Host "DLL Size: $([math]::Round($dllInfo.Length / 1KB, 2)) KB" -ForegroundColor Green
            Write-Host "Created: $($dllInfo.LastWriteTime)" -ForegroundColor Green
        }
    } else {
        Write-Host ""
        Write-Host "==========================================" -ForegroundColor Red
        Write-Host "BUILD FAILED!" -ForegroundColor Red
        Write-Host "==========================================" -ForegroundColor Red
        Write-Host "Error Code: $LASTEXITCODE" -ForegroundColor Red
    }
} finally {
    Pop-Location
}

Write-Host ""
Read-Host "Press Enter to exit"
