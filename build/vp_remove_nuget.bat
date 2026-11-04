@echo off
setlocal EnableExtensions DisableDelayedExpansion
set "promptedForVersion=0"
set "promptedForRemoval=0"
set "exitCode=0"

set "packageVersion=%~1"
set "removeMode=%~2"
if /I "%~1"=="--dry-run" (
    set "packageVersion="
    set "removeMode=--dry-run"
)
if not defined packageVersion (
    set "promptedForVersion=1"
    set /p "packageVersion=Ange paketversion att ta bort (t.ex. 2.0.0-beta.1): "
)
if not defined packageVersion goto :usage
if defined removeMode if /I not "%removeMode%"=="--dry-run" goto :usage

set "scriptFolder=%~dp0"
for %%D in ("%scriptFolder%..") do set "repoRoot=%%~fD"
if not exist "%repoRoot%\N2.Everything.2026.sln" (
    echo ERROR: The repository root was not found from the script location.
    goto :fail
)

rem Keep the same environment variable names and default source as the publish script.
if not defined nugetSeverBrowsePath if defined nugetServerBrowsePath set "nugetSeverBrowsePath=%nugetServerBrowsePath%"
if not defined nugetSeverFullPath if defined nugetServerFullPath set "nugetSeverFullPath=%nugetServerFullPath%"
if not defined nugetSeverBrowsePath set "nugetSeverBrowsePath=https://nuget2.voidpointer.se"
if not defined nugetSeverFullPath set "nugetSeverFullPath=%nugetSeverBrowsePath%/v3/index.json"

set "packageIds=VP_N2CMS VP_N2CMS.Mvc VP_N2CMS.Management VP_N2CMS.Search.Lucene VP_N2CMS.Razor VP_N2CMS.Config.SQLite VP_N2CMS.Library VP_N2CMS.Config"
set /a packageCount=0 >nul
for %%P in (%packageIds%) do set /a packageCount+=1 >nul

echo Script folder: "%scriptFolder%"
echo Repository root: "%repoRoot%"
echo Local version folder: "%repoRoot%\output\vp-n2cms\%packageVersion%"
echo NuGet source: "%nugetSeverFullPath%"
echo Package version: %packageVersion%
echo Packages to remove: %packageCount%
for %%P in (%packageIds%) do echo Package: %%P
echo The server decides whether delete removes or unlists a package.

if /I "%removeMode%"=="--dry-run" (
    echo Dry run complete. No server requests were made.
    goto :finish
)

:askRemoval
set "promptedForRemoval=1"
set "removeAnswer="
set /p "removeAnswer=Ta bort denna version for alla paketen nu? [j/N]: "
if /I "%removeAnswer%"=="J" goto :remove
if /I "%removeAnswer%"=="JA" goto :remove
if /I "%removeAnswer%"=="Y" goto :remove
if /I "%removeAnswer%"=="YES" goto :remove
if not defined removeAnswer goto :cancelRemoval
if /I "%removeAnswer%"=="N" goto :cancelRemoval
if /I "%removeAnswer%"=="NEJ" goto :cancelRemoval
echo Svara J eller N.
goto :askRemoval

:cancelRemoval
echo Inga serveranrop gjordes.
goto :finish

:remove
if not defined NugetServerPassword (
    echo ERROR: NugetServerPassword is missing from the environment.
    goto :fail
)
where dotnet.exe >nul 2>&1
if errorlevel 1 (
    echo ERROR: dotnet.exe was not found on PATH.
    goto :fail
)

for %%P in (%packageIds%) do (
    call :removePackage "%%P"
    if errorlevel 1 goto :fail
)

echo Delete or unlist requested for %packageCount% packages at version %packageVersion%.
goto :finish

:removePackage
echo Removing %~1 %packageVersion%...
dotnet nuget delete "%~1" "%packageVersion%" --source "%nugetSeverFullPath%" --api-key "%NugetServerPassword%" --non-interactive
if errorlevel 1 (
    echo ERROR: Removing %~1 failed. Earlier packages may already have been removed or unlisted.
    exit /b 1
)
exit /b 0

:usage
echo Usage: vp_remove_nuget.bat [VERSION] [--dry-run]
echo Example: vp_remove_nuget.bat 2.0.0-beta.1
set "exitCode=2"
goto :finish

:fail
set "exitCode=1"

:finish
if "%promptedForVersion%"=="1" pause
if "%promptedForVersion%"=="0" if "%promptedForRemoval%"=="1" pause
exit /b %exitCode%
