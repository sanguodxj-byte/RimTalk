# Copilot Instructions: 厨间百艺 (Culinary Arts 100)

## Project Overview
RimWorld 1.4+ mod that generates procedural, skill-based dish names with mood effects. Uses Harmony patches to intercept food creation and consumption.

## Architecture

### Core Flow
1. **Food Creation** → `Patch_GenRecipe.MakeRecipeProducts` intercepts `GenRecipe.MakeRecipeProducts`
2. **Name Generation** → `NameGenerator.GenerateMealName` uses time-seed + chef skill + ingredients
3. **Label Display** → `Patch_Thing_Label` shows custom names (stack-aware: custom name for single items, base name for stacks)
4. **Consumption** → `Patch_Thing_Ingested` applies mood thoughts based on `CompNamedMeal.MoodOffset`

### Time-Seed System
- **6-hour window consistency**: Same chef + same ingredients = same dish name within 15000 ticks
- Implementation: `int seed = pawnHash ^ ingredientHash ^ (ticksGame / 15000)`
- Critical: Ingredients sorted by `defName` before hashing to ensure order-independence

### Component Architecture
- **CompNamedMeal**: Stores `customName`, `moodOffset` (-3/0/3/8), `generationSeed`, `cuisineStyle`
- Attached via XML patch: `Patches/Food_Patches.xml` adds to all `ThingDef[ingestible/foodType="Meal"]`
- Must call `Scribe_Values.Look` in `PostExposeData` for save/load persistence

## Build & Deploy

### Standard Build (requires RimWorld DLLs)
```powershell
dotnet build "Source\CulinaryArts\CulinaryArts.csproj" -c Release
```
**Critical**: `CulinaryArts.csproj` hardcodes RimWorld path `D:\steam\steamapps\common\RimWorld`
- If build fails with MSB6006 or missing references, update `<HintPath>` to your RimWorld installation
- Harmony comes from NuGet (`Lib.Harmony` v2.2.2) for compile-time, runtime uses RimWorld's bundled version

### Quick Deploy (no build needed)
```bat
QuickDeploy.bat
```
Copies `About/`, `Defs/`, `Languages/`, `Source/` to RimWorld Mods folder. Game auto-compiles C# on first load.

## Code Conventions

### Harmony Patching Patterns
```csharp
[HarmonyPatch(typeof(TargetType), "MethodName")]
public static class Patch_TargetType_MethodName
{
    [HarmonyPostfix]
    public static void Postfix(ref Type __result, Type __instance) { }
}
```
- **Always use Postfix** for this mod (no Prefix/Transpiler usage)
- Patch naming: `Patch_{ClassName}_{MethodName}.cs`
- IEnumerable trap: Convert to List before modifying (`__result.ToList()`) in `Patch_GenRecipe`

### Data Databases
- **Static dictionaries**: `TechniqueDatabase`, `PrefixDatabase`, `IngredientDatabase`
- Skill-tiered: 0-5 (beginner), 6-10 (competent), 11-15 (skilled), 16+ (master)
- Dual cuisine: `CuisineStyle.Chinese` / `CuisineStyle.Western` (50/50 random per dish)

### ThoughtDef Integration
Custom thoughts in `Defs/ThoughtDefs/Thoughts_Memory_CulinaryArts.xml`:
- `CulinaryArts_Terrible` (-3 mood, 0.5 days)
- `CulinaryArts_Delicious` (+3 mood, 1 day)
- `CulinaryArts_Legendary` (+8 mood, 2 days, stack limit 1)

Must define in `ThoughtDefOf.cs` utility class for C# access.

## Common Tasks

### Adding New Cooking Technique
1. Edit `Data/TechniqueDatabase.cs`
2. Add to appropriate skill tier dictionary
3. Follow pattern: Chinese uses verbs (炒/煮), Western uses past participles (Braised/Grilled)

### Debugging Name Generation
- Enable dev mode in RimWorld
- Check `Player.log` for `[CulinaryArts]` messages
- `CompNamedMeal` exposes `GenerationSeed` for reproducibility testing

### Testing Build Locally
Update `CulinaryArts.csproj` post-build target:
```xml
<TestModsPath>YOUR_RIMWORLD_PATH\Mods\Culinary Arts 100</TestModsPath>
```

## Critical Gotchas
- **Never modify** `__result` directly if it's `IEnumerable<T>` - always `.ToList()` first
- **Stack display logic**: Use `NameGenerator.GetDisplayLabel()` not direct `comp.CustomName`
- **XML patches load order**: Defined in `About.xml` `<loadAfter>` - must load after Harmony
- **Localization**: Keys in `Languages/{ChineseSimplified,English}/Keyed/CulinaryArts_Keys.xml` (currently unused, names are procedural)
