@echo off
setlocal EnableExtensions DisableDelayedExpansion
set "promptedForVersion=0"
set "promptedForPublish=0"
set "exitCode=0"

set "packageVersion=%~1"
set "publishMode=%~2"
if /I "%~1"=="--dry-run" (
    set "packageVersion="
    set "publishMode=--dry-run"
)
if /I "%~1"=="--publish" (
    set "packageVersion="
    set "publishMode=--publish"
)
if not defined packageVersion (
    set "promptedForVersion=1"
    set /p "packageVersion=Ange paketversion (t.ex. 2.0.0-beta.1): "
)
if not defined packageVersion goto :usage
if not defined publishMode set "publishMode=--ask"
if /I not "%publishMode%"=="--ask" if /I not "%publishMode%"=="--dry-run" if /I not "%publishMode%"=="--publish" goto :usage

set "scriptFolder=%~dp0"
for %%D in ("%scriptFolder%..") do set "repoRoot=%%~fD"
echo Script folder: "%scriptFolder%"
echo Repository root: "%repoRoot%"
if not exist "%repoRoot%\N2.Everything.2026.sln" (
    echo ERROR: The repository root was not found from the script location.
    goto :fail
)

set "publishFolder=%repoRoot%\output\vp-n2cms\%packageVersion%"
echo Package folder: "%publishFolder%"
if not exist "%publishFolder%\" (
    echo ERROR: Package folder is missing. Check the entered version and the path shown above.
    goto :fail
)

set /a packageCount=0 >nul
for %%F in ("%publishFolder%\*.nupkg") do if exist "%%~fF" set /a packageCount+=1 >nul
if %packageCount% EQU 0 (
    echo ERROR: No .nupkg files were found in the package folder shown above.
    goto :fail
)
echo Packages found: %packageCount%, version %packageVersion%

rem Keep the environment variable names used by the other VP NuGet scripts.
if not defined nugetSeverBrowsePath if defined nugetServerBrowsePath set "nugetSeverBrowsePath=%nugetServerBrowsePath%"
if not defined nugetSeverFullPath if defined nugetServerFullPath set "nugetSeverFullPath=%nugetServerFullPath%"
if not defined nugetSeverBrowsePath set "nugetSeverBrowsePath=https://nuget2.voidpointer.se"
if not defined nugetSeverFullPath set "nugetSeverFullPath=%nugetSeverBrowsePath%/v3/index.json"

for %%F in ("%publishFolder%\*.nupkg") do if exist "%%~fF" echo Package: %%~nxF
if defined nugetSeverFullPath echo NuGet source: "%nugetSeverFullPath%"
if not defined nugetSeverFullPath echo NuGet source: not configured yet
if /I "%publishMode%"=="--dry-run" (
    echo Dry run complete. No packages were published.
    goto :finish
)
if /I "%publishMode%"=="--ask" goto :askPublish
goto :publish

:askPublish
set "promptedForPublish=1"
set "publishAnswer="
set /p "publishAnswer=Publicera dessa paket nu? [j/N]: "
if /I "%publishAnswer%"=="J" goto :publish
if /I "%publishAnswer%"=="JA" goto :publish
if /I "%publishAnswer%"=="Y" goto :publish
if /I "%publishAnswer%"=="YES" goto :publish
if not defined publishAnswer goto :cancelPublish
if /I "%publishAnswer%"=="N" goto :cancelPublish
if /I "%publishAnswer%"=="NEJ" goto :cancelPublish
echo Svara J eller N.
goto :askPublish

:cancelPublish
echo Inga paket publicerades.
goto :finish

:publish
if not defined nugetSeverFullPath (
    echo ERROR: NuGet server source is missing.
    goto :fail
)
if not defined NugetServerPassword (
    echo ERROR: NugetServerPassword is missing from the environment.
    goto :fail
)
where dotnet.exe >nul 2>&1
if errorlevel 1 (
    echo ERROR: dotnet.exe was not found on PATH.
    goto :fail
)
echo NuGet source: "%nugetSeverFullPath%"

for %%F in ("%publishFolder%\*.nupkg") do (
    if exist "%%~fF" (
        call :push "%%~fF"
        if errorlevel 1 goto :fail
    )
)

echo Published %packageCount% packages. Check the server feed for version %packageVersion%.
goto :finish

:push
echo Publishing %~nx1...
dotnet nuget push "%~f1" --source "%nugetSeverFullPath%" --api-key "%NugetServerPassword%"
if errorlevel 1 (
    echo ERROR: Publishing %~nx1 failed. Earlier packages may already be on the server.
    exit /b 1
)
exit /b 0

:usage
echo Usage: vp_publish_to_nuget.bat [VERSION] [--dry-run ^| --publish]
echo With no mode, the script asks before publishing.
echo Example: vp_publish_to_nuget.bat 2.0.0-beta.1
set "exitCode=2"
goto :finish

:fail
set "exitCode=1"

:finish
if "%promptedForVersion%"=="1" pause
if "%promptedForVersion%"=="0" if "%promptedForPublish%"=="1" pause
exit /b %exitCode%