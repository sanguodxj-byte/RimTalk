# Translation Troubleshooting Guide

## Problem: Buttons Show Garbled Text or Translation Keys

### Symptom Checklist
- [ ] Buttons show: `SuperbRecruitment_PersuadeLabel` (translation key)
- [ ] Buttons show: garbled Chinese characters
- [ ] Buttons are completely blank
- [ ] Some buttons work, others don't

---

## Solution 1: Check Game Language

**Problem**: RimWorld not set to Chinese

**Fix**:
1. Launch RimWorld
2. Options → Language
3. Select `Chinese (Simplified)` or `中文（简体）`
4. Restart game

---

## Solution 2: Fix File Encoding

**Problem**: Translation file wrong encoding

**Fix**:
Run `FixTranslations.bat` then restart RimWorld

**Manual Fix**:
1. Open `Languages\ChineseSimplified\Keyed\SuperbRecruitment.xml`
2. Save as UTF-8 with BOM
3. Copy to RimWorld mods folder
4. Restart game

---

## Solution 3: Check File Structure

**Problem**: Folder structure wrong

**Correct Structure**:
```
D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment\
└── Languages\
    ├── ChineseSimplified\      ← NOT "Chinese(Simplified)"
    │   └── Keyed\
    │       └── SuperbRecruitment.xml
    └── English\
        └── Keyed\
            └── SuperbRecruitment.xml
```

**Wrong Names to Avoid**:
- ? `Chinese(Simplified)`
- ? `ChineseTraditional`
- ? `Chinese Simplified`
- ? `ChineseSimplified` (correct)

---

## Solution 4: Verify Translation Keys

**Check if keys exist**:

Open `Languages\ChineseSimplified\Keyed\SuperbRecruitment.xml` and verify:

```xml
<SuperbRecruitment_PersuadeLabel>说服</SuperbRecruitment_PersuadeLabel>
<SuperbRecruitment_RecruitLabel>尝试招募</SuperbRecruitment_RecruitLabel>
```

**Check code uses correct keys**:

In `Command_PersuadeVisitor.cs`:
```csharp
defaultLabel = "SuperbRecruitment_PersuadeLabel".Translate();
```

Keys must match exactly!

---

## Solution 5: Check RimWorld Log

**Log Location**:
```
C:\Users\Administrator\AppData\LocalLow\Ludeon Studios\
  RimWorld by Ludeon Studios\Player.log
```

**Search for**:
- `[Superb Recruitment]` - mod messages
- `translation` - translation errors
- `missing` - missing keys
- Red error text

**Common Errors**:
```
Translation key not found: SuperbRecruitment_PersuadeLabel
```
→ Translation file not loaded

```
Could not load language ChineseSimplified
```
→ Folder name wrong

---

## Quick Test

**Test if translation loading**:

1. Start RimWorld
2. Open Dev Mode (press `~` then type `dev mode`)
3. Check log for: `[Superb Recruitment] Harmony补丁已应用`
4. Spawn visitor
5. Select visitor
6. Check button text

**Expected Results**:
- Chinese: `说服`
- English: `Persuade`

**If shows key**: `SuperbRecruitment_PersuadeLabel`
→ Translation not loading

---

## Emergency Fallback: Use English

If Chinese keeps failing:

1. Delete `Languages\ChineseSimplified\` folder
2. Keep only `Languages\English\`
3. Set game to English
4. Restart

Should at least work in English while debugging Chinese.

---

## Current File Status

### Deployed Files
```
? Languages\ChineseSimplified\Keyed\SuperbRecruitment.xml
? Languages\English\Keyed\SuperbRecruitment.xml
? Both files UTF-8 with BOM encoding
? Copied to RimWorld Mods folder
```

### If Still Not Working

Run this PowerShell command to check encoding:
```powershell
Get-Content "D:\steam\steamapps\common\RimWorld\Mods\Superb Recruitment\Languages\ChineseSimplified\Keyed\SuperbRecruitment.xml" -Encoding UTF8 | Select-Object -First 5
```

Should show:
```xml
<?xml version="1.0" encoding="utf-8"?>
<LanguageData>

  <!-- 命令标签 -->
  <SuperbRecruitment_PersuadeLabel>说服</SuperbRecruitment_PersuadeLabel>
```

---

## Final Checklist

- [ ] Run `FixTranslations.bat`
- [ ] Restart RimWorld
- [ ] Check game language setting
- [ ] Verify folder structure
- [ ] Check Player.log for errors
- [ ] Test with English if Chinese fails

---

**If all else fails**: Post the error from Player.log and screenshot of button text
