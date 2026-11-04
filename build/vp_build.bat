@echo off
setlocal EnableExtensions DisableDelayedExpansion
set "promptedForVersion=0"
set "promptedForBuild=0"
set "exitCode=0"

set "packageVersion=%~1"
set "PACK_SKIP_BUILD="
if /I "%~1"=="--no-build" (
    set "packageVersion="
    set "PACK_SKIP_BUILD=-SkipBuild"
)
if /I "%~2"=="--no-build" set "PACK_SKIP_BUILD=-SkipBuild"

if not defined packageVersion (
    set "promptedForVersion=1"
    set /p "packageVersion=Ange paketversion (t.ex. 2.0.0-beta.1): "
)
if not defined packageVersion goto :usage

set "scriptFolder=%~dp0"
for %%D in ("%scriptFolder%..") do set "repoRoot=%%~fD"
set "outputFolder=%repoRoot%\output\vp-n2cms\%packageVersion%"
echo Script folder: "%scriptFolder%"
echo Repository root: "%repoRoot%"
echo Package version: %packageVersion%
echo Output folder: "%outputFolder%"
if defined PACK_SKIP_BUILD echo Mode: package existing Release output
if not defined PACK_SKIP_BUILD echo Mode: build Release and package

if not exist "%repoRoot%\N2.Everything.2026.sln" (
    echo ERROR: The repository root was not found from the script location.
    goto :fail
)
if exist "%outputFolder%" (
    echo ERROR: The output folder already exists. Choose a new version.
    goto :fail
)

:askBuild
set "promptedForBuild=1"
set "buildAnswer="
set /p "buildAnswer=Starta med dessa installningar? [j/N]: "
if /I "%buildAnswer%"=="J" goto :build
if /I "%buildAnswer%"=="JA" goto :build
if /I "%buildAnswer%"=="Y" goto :build
if /I "%buildAnswer%"=="YES" goto :build
if not defined buildAnswer goto :cancelBuild
if /I "%buildAnswer%"=="N" goto :cancelBuild
if /I "%buildAnswer%"=="NEJ" goto :cancelBuild
echo Svara J eller N.
goto :askBuild

:cancelBuild
echo Inget bygge eller paket skapades.
goto :finish

:build
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0pack-vp-n2cms.ps1" -Version "%packageVersion%" %PACK_SKIP_BUILD%
set "exitCode=%ERRORLEVEL%"
goto :finish

:usage
echo Usage: vp_build.bat [VERSION] [--no-build]
echo Example: vp_build.bat 2.0.0-beta.1
set "exitCode=2"
goto :finish

:fail
set "exitCode=1"

:finish
if "%promptedForVersion%"=="1" pause
if "%promptedForVersion%"=="0" if "%promptedForBuild%"=="1" pause
exit /b %exitCode%
