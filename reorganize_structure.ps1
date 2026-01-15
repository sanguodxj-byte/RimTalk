# PowerShell script to reorganize project structure

$root = "The Second Seat/Source/TheSecondSeat"

# Helper function to move file
function Move-File {
    param (
        [string]$source,
        [string]$destination
    )
    
    $srcPath = Join-Path $root $source
    $destPath = Join-Path $root $destination
    $destDir = Split-Path $destPath -Parent
    
    if (-not (Test-Path $destDir)) {
        New-Item -ItemType Directory -Force -Path $destDir | Out-Null
        Write-Host "Created directory: $destDir"
    }
    
    if (Test-Path $srcPath) {
        Move-Item -Path $srcPath -Destination $destPath -Force
        Write-Host "Moved: $source -> $destination"
    } else {
        Write-Warning "Source file not found: $source"
    }
}

# 1. Intelligence Core
# RimAgent/Tools is already correct

# 2. Command & Control
# Commands/Implementations is already correct
Move-File "Execution/GameActionExecutor.cs" "Execution/GameActionExecutor.cs" # Already there
Move-File "Autonomous/AutonomousBehaviorSystem.cs" "Autonomous/AutonomousBehaviorSystem.cs" # Already there

# 3. Narrator Presentation
Move-File "Core/EmotionTracker.cs" "PersonaGeneration/EmotionTracker.cs"
Move-File "Core/PortraitOverlaySystem.cs" "PersonaGeneration/PortraitOverlaySystem.cs"
Move-File "Core/ProactiveDialogueSystem.cs" "Narrator/ProactiveDialogueSystem.cs"

# 4. Gameplay Mechanics
Move-File "Framework/NarratorEventDef.cs" "Events/NarratorEventDef.cs"
Move-File "Framework/NarratorEventManager.cs" "Events/NarratorEventManager.cs"

# 5. Infrastructure
Move-File "TheSecondSeatCore.cs" "Core/TheSecondSeatCore.cs"
Move-File "TheSecondSeatMod.cs" "Core/TheSecondSeatMod.cs"

# Merge Observer into Monitoring
# Rename GameStateObserver.cs from Observer to GameStateSnapshot.cs in Monitoring to avoid conflict
Move-File "Observer/GameStateObserver.cs" "Monitoring/GameStateSnapshot.cs"
if (Test-Path (Join-Path $root "Observer")) {
    Remove-Item -Path (Join-Path $root "Observer") -Recurse -Force
    Write-Host "Removed Observer directory"
}

# Framework adjustments
Move-File "Framework/TSSAction.cs" "Framework/Actions/TSSAction.cs"
Move-File "Framework/TSSTrigger.cs" "Framework/Triggers/TSSTrigger.cs"

# Defs adjustments
Move-File "Defs/AgentPromptDef.cs" "RimAgent/AgentPromptDef.cs"
if (Test-Path (Join-Path $root "Defs")) {
    # Check if Defs is empty
    if ((Get-ChildItem (Join-Path $root "Defs")).Count -eq 0) {
        Remove-Item -Path (Join-Path $root "Defs") -Recurse -Force
        Write-Host "Removed empty Defs directory"
    }
}

Write-Host "Reorganization complete."
