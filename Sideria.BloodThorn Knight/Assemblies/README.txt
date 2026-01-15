# Assemblies Directory

This directory should contain compiled DLL files for the mod.

## Required Files

After building the C# project, you should have:

### Main DLL
- `SideriaBloodThornKnight.dll` - The main mod DLL (compiled from Source/)

### Dependencies
- `0Harmony.dll` - Harmony library (automatically included via NuGet)

## Build Instructions

### Option 1: Use Build.bat (Recommended)
```
Double-click Build.bat in the mod root directory
```

### Option 2: Manual Build
```
cd Source
dotnet build SideriaBloodThornKnight.csproj -c Release
```

or

```
cd Source
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" SideriaBloodThornKnight.csproj /p:Configuration=Release
```

## Verification

After successful build, this directory should contain:
- ? SideriaBloodThornKnight.dll
- ? 0Harmony.dll (if using NuGet package)

If files are missing, check:
1. Build output in Visual Studio/console
2. Project file references are correct
3. RimWorld path is set correctly

## C# Event System Features

The DLL implements:
- ? Day 3 automatic Sideria arrival event
- ? Visitor persistence system
- ? Daily feeding system with mood bonuses
- ? 30-day gratitude join mechanic
- ? Legendary sword transformation (Blood Resonance)
- ? Right-click interaction menu
- ? Dialog system
- ? Atzgand weapon transformation and biocoding

## Troubleshooting

**DLL not loading:**
- Check RimWorld log (press ~ in-game)
- Ensure HAR (Humanoid Alien Races) is loaded before this mod
- Verify 0Harmony.dll is present

**Event not triggering:**
- Confirm DLL is loaded (check log for "[Sideria] Mod loaded successfully!")
- Ensure you have a player home map on day 3
- Check GameComponent is registered

**Right-click menu not appearing:**
- Verify Harmony patches applied successfully
- Check Sideria is in Visitor faction (not yet joined)
- Confirm she hasn't already joined

For detailed documentation, see:
- `WANDERER_KNIGHT_EVENT_IMPLEMENTATION.md` - Full technical docs
- `WANDERER_KNIGHT_QUICK_START.md` - Quick start guide
- `WANDERER_KNIGHT_COMPLETE_SUMMARY.md` - Complete summary

---

**Note:** If you don't want to compile C#, you can use the XML-only version. However, the full event system requires the compiled DLL.
