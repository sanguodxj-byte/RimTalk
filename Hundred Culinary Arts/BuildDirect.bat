@echo off
echo 正在编译 Living Weapons 模组...
echo.

set RIMWORLD_DIR=D:\steam\steamapps\common\RimWorld
set HARMONY_DIR=D:\steam\steamapps\workshop\content\294100\1235181370\1.1\Assemblies
set MANAGED_DIR=C:\Users\Administrator\Desktop\rim mod\Managed

csc /target:library ^
    /out:Assemblies\LivingWeapons.dll ^
    /reference:"%MANAGED_DIR%\Assembly-CSharp.dll" ^
    /reference:"%MANAGED_DIR%\UnityEngine.CoreModule.dll" ^
    /reference:"%MANAGED_DIR%\UnityEngine.IMGUIModule.dll" ^
    /reference:"%MANAGED_DIR%\UnityEngine.TextRenderingModule.dll" ^
    /reference:"%HARMONY_DIR%\0Harmony.dll" ^
    /reference:"%MANAGED_DIR%\netstandard.dll" ^
    /nostdlib+ ^
    /reference:"%MANAGED_DIR%\mscorlib.dll" ^
    /reference:"%MANAGED_DIR%\System.dll" ^
    /reference:"%MANAGED_DIR%\System.Core.dll" ^
    Source\LivingWeapons\*.cs ^
    Source\LivingWeapons\HarmonyPatches\*.cs

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ? 编译成功！
    echo DLL已输出到: Assemblies\LivingWeapons.dll
    echo.
    echo 现在可以将整个文件夹复制到：
    echo D:\steam\steamapps\common\RimWorld\Mods\LivingWeapons\
) else (
    echo.
    echo ? 编译失败！
)

pause
