# Sideria BloodThorn Knight - Textures Directory

This directory contains all texture assets for the Sideria: BloodThorn Knight mod.

## Directory Structure

```
Textures/
©À©¤©¤ Things/
©¦   ©À©¤©¤ Pawn/
©¦   ©¦   ©¸©¤©¤ Humanlike/
©¦   ©¦       ©À©¤©¤ Bodies/
©¦   ©¦       ©¦   ©¸©¤©¤ Dracovampir/
©¦   ©¦       ©¦       ©À©¤©¤ Naked_Male_south.png (512x512)
©¦   ©¦       ©¦       ©À©¤©¤ Naked_Male_east.png (512x512)
©¦   ©¦       ©¦       ©À©¤©¤ Naked_Male_north.png (512x512)
©¦   ©¦       ©¦       ©À©¤©¤ Naked_Female_south.png (512x512)
©¦   ©¦       ©¦       ©À©¤©¤ Naked_Female_east.png (512x512)
©¦   ©¦       ©¦       ©¸©¤©¤ Naked_Female_north.png (512x512)
©¦   ©¦       ©¦
©¦   ©¦       ©À©¤©¤ BodyAddons/
©¦   ©¦       ©¦   ©À©¤©¤ DragonHorns_south.png (512x512)
©¦   ©¦       ©¦   ©À©¤©¤ DragonHorns_east.png (512x512)
©¦   ©¦       ©¦   ©À©¤©¤ DragonHorns_north.png (512x512)
©¦   ©¦       ©¦   ©À©¤©¤ DragonWings_south.png (512x512)
©¦   ©¦       ©¦   ©À©¤©¤ DragonWings_east.png (512x512)
©¦   ©¦       ©¦   ©À©¤©¤ DragonWings_north.png (512x512)
©¦   ©¦       ©¦   ©À©¤©¤ DragonTail_south.png (512x512)
©¦   ©¦       ©¦   ©À©¤©¤ DragonTail_east.png (512x512)
©¦   ©¦       ©¦   ©À©¤©¤ DragonTail_north.png (512x512)
©¦   ©¦       ©¦   ©À©¤©¤ BloodMarkings_south.png (512x512)
©¦   ©¦       ©¦   ©À©¤©¤ BloodMarkings_east.png (512x512)
©¦   ©¦       ©¦   ©¸©¤©¤ BloodMarkings_north.png (512x512)
©¦   ©¦       ©¦
©¦   ©¦       ©¸©¤©¤ Heads/
©¦   ©¦           ©¸©¤©¤ (Optional: custom head textures)
©¦   ©¦
©¦   ©¸©¤©¤ Item/
©¦       ©¸©¤©¤ Equipment/
©¦           ©¸©¤©¤ WeaponMelee/
©¦               ©À©¤©¤ Atzgand.png (256x256)
©¦               ©À©¤©¤ Atzgand_Ascended.png (256x256)
©¦               ©¸©¤©¤ BloodDagger.png (256x256)
©¦
©¸©¤©¤ UI/
    ©À©¤©¤ Abilities/
    ©¦   ©À©¤©¤ DragonBreath.png (64x64)
    ©¦   ©À©¤©¤ DragonicAura.png (64x64)
    ©¦   ©À©¤©¤ DragonWings.png (64x64)
    ©¦   ©À©¤©¤ DragonicTransformation.png (64x64)
    ©¦   ©À©¤©¤ BloodDrain.png (64x64)
    ©¦   ©À©¤©¤ VampiricEmbrace.png (64x64)
    ©¦   ©À©¤©¤ BloodFrenzy.png (64x64)
    ©¦   ©¸©¤©¤ OathbreakerTransformation.png (64x64)
    ©¦
    ©¸©¤©¤ Icons/
        ©¸©¤©¤ (Optional: additional UI elements)
```

## Required Textures

### Priority 1: Bodies (Required)
- **Location**: `Textures/Things/Pawn/Humanlike/Bodies/Dracovampir/`
- **Files**: 6 files (Male & Female ¡Á 3 directions)
- **Size**: 512x512 pixels each
- **Format**: PNG with transparency

### Priority 2: Body Addons (High Priority)
- **Location**: `Textures/Things/Pawn/Humanlike/BodyAddons/`
- **Files**: 12 files (4 types ¡Á 3 directions)
- **Size**: 512x512 pixels each
- **Format**: PNG with transparency

### Priority 3: Weapons (Medium Priority)
- **Location**: `Textures/Things/Item/Equipment/WeaponMelee/`
- **Files**: 3 files
- **Size**: 256x256 pixels each
- **Format**: PNG with transparency

### Priority 4: Ability Icons (Low Priority)
- **Location**: `Textures/UI/Abilities/`
- **Files**: 8 files
- **Size**: 64x64 pixels each
- **Format**: PNG

## Texture Requirements

### Body Textures
- Transparent background
- Pale red skin tone (RGB: 224, 199, 199)
- Three directions: south (front), east (side), north (back)
- Muscular build with draconic features

### Body Addons
- **Dragon Horns**: Curved, crystalline horns
- **Dragon Wings**: Large bat-like wings with membrane
- **Dragon Tail**: Long, scaled tail with spikes
- **Blood Markings**: Glowing crimson runes and patterns

### Weapons
- **Atzgand**: Crimson longsword with thorn protrusions
- **Atzgand Ascended**: Golden divine version
- **Blood Dagger**: Curved red blade

### Ability Icons
- Clear, recognizable icons
- Consistent art style
- Appropriate color coding (red for blood, gold for dragon)

## Current Status
- All folders created
- Waiting for texture assets
- Pink placeholder will appear until textures are added

## Notes
- Without textures, the mod is fully functional but will show pink placeholders
- Textures can be added incrementally
- See TEXTURE_GUIDE_CORRECT.md for detailed specifications
