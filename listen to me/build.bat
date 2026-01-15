@echo off
echo ==========================================
echo Building ListenToMe Mod for RimWorld
echo ==========================================

set DLL_PATH=C:\Users\Administrator\Desktop\rim mod
set OUTPUT_PATH=C:\Users\Administrator\Desktop\rim mod\listen to me\Assemblies
set SOURCE_PATH=C:\Users\Administrator\Desktop\rim mod\listen to me\Source\ListenToMe

echo.
echo Step 1: Creating output directory...
if not exist "%OUTPUT_PATH%" mkdir "%OUTPUT_PATH%"

echo.
echo Step 2: Finding C# compiler...
set CSC_PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC_PATH%" (
    echo ERROR: C# compiler not found at %CSC_PATH%
    echo Please install .NET Framework 4.7.2 Developer Pack
    pause
    exit /b 1
)

echo Found: %CSC_PATH%

echo.
echo Step 3: Compiling source files...
"%CSC_PATH%" /target:library ^
    /out:"%OUTPUT_PATH%\ListenToMe.dll" ^
    /reference:"%DLL_PATH%\Assembly-CSharp.dll" ^
    /reference:"%DLL_PATH%\UnityEngine.CoreModule.dll" ^
    /reference:"%DLL_PATH%\UnityEngine.dll" ^
    /reference:"%DLL_PATH%\UnityEngine.IMGUIModule.dll" ^
    /reference:"%DLL_PATH%\0Harmony.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Core.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Xml.dll" ^
    /langversion:latest ^
    /unsafe ^
    /optimize ^
    /debug:portable ^
    "%SOURCE_PATH%\*.cs"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ==========================================
    echo Build SUCCESS!
    echo Output: %OUTPUT_PATH%\ListenToMe.dll
    echo ==========================================
) else (
    echo.
    echo ==========================================
    echo Build FAILED with error code %ERRORLEVEL%
    echo ==========================================
)

echo.
pause
