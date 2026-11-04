param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9]+(\.[0-9]+){2,3}(-[0-9A-Za-z.-]+)?$')]
    [string]$Version,
    [switch]$SkipBuild,
    [string]$NuGetExe
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$buildDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoDir = (Resolve-Path (Join-Path $buildDir '..')).Path
$solution = Join-Path $repoDir 'N2.Everything.2026.sln'
$outputDir = Join-Path $repoDir ("output\vp-n2cms\{0}" -f $Version)
$stagingDir = Join-Path $outputDir '_staging'

function Require-File([string]$Path) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "Required file is missing: $Path"
    }
    return (Resolve-Path -LiteralPath $Path).Path
}

function Copy-Required([string]$Source, [string]$Destination) {
    $resolved = Require-File $Source
    $parent = Split-Path -Parent $Destination
    New-Item -ItemType Directory -Path $parent -Force | Out-Null
    Copy-Item -LiteralPath $resolved -Destination $Destination
}

function Copy-Tree([string]$Source, [string]$Destination) {
    if (-not (Test-Path -LiteralPath $Source -PathType Container)) {
        throw "Required directory is missing: $Source"
    }
    $sourceRoot = (Resolve-Path -LiteralPath $Source).Path.TrimEnd('\')
    foreach ($file in Get-ChildItem -LiteralPath $sourceRoot -Recurse -File) {
        $relative = $file.FullName.Substring($sourceRoot.Length).TrimStart('\')
        Copy-Required $file.FullName (Join-Path $Destination $relative)
    }
}

function Package-Version([string]$ConfigPath, [string]$Id) {
    [xml]$config = Get-Content -LiteralPath (Require-File $ConfigPath) -Raw
    $match = @($config.packages.package | Where-Object { $_.id -eq $Id })
    if ($match.Count -ne 1) { throw "Expected one $Id entry in $ConfigPath" }
    return [string]$match[0].version
}

function Dependency([string]$Id, [string]$RequiredVersion) {
    return [pscustomobject]@{ Id = $Id; Version = $RequiredVersion }
}

function ExactDependency([string]$Id) {
    return (Dependency $Id ("[{0}]" -f $Version))
}

function ExternalDependencies([string]$ConfigPath) {
    [xml]$config = Get-Content -LiteralPath (Require-File $ConfigPath) -Raw
    foreach ($package in $config.packages.package) {
        Dependency ([string]$package.id) ("[{0}]" -f [string]$package.version)
    }
}

function Find-MSBuild {
    $programFilesX86 = [Environment]::GetFolderPath('ProgramFilesX86')
    $vswhere = Join-Path $programFilesX86 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path -LiteralPath $vswhere) {
        $found = @(& $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\Current\Bin\MSBuild.exe')
        if ($found.Count -gt 0 -and (Test-Path -LiteralPath $found[0])) { return $found[0] }
    }
    $command = Get-Command MSBuild.exe -ErrorAction SilentlyContinue
    if ($command) { return $command.Source }
    throw 'MSBuild.exe was not found. Run from a Visual Studio Developer Command Prompt or install the MSBuild workload.'
}

function Write-Nuspec([string]$Stage, [string]$Id, [string]$Description, [object[]]$Dependencies) {
    $ns = 'http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'
    $doc = New-Object System.Xml.XmlDocument
    $declaration = $doc.CreateXmlDeclaration('1.0', 'utf-8', $null)
    [void]$doc.AppendChild($declaration)
    $package = $doc.CreateElement('package', $ns)
    [void]$doc.AppendChild($package)
    $metadata = $doc.CreateElement('metadata', $ns)
    [void]$package.AppendChild($metadata)
    foreach ($pair in @(
        @('id', $Id), @('version', $Version), @('authors', 'Cristian Libardo'),
        @('owners', 'Cristian Libardo'), @('requireLicenseAcceptance', 'false'),
        @('description', $Description))) {
        $element = $doc.CreateElement([string]$pair[0], $ns)
        $element.InnerText = [string]$pair[1]
        [void]$metadata.AppendChild($element)
    }
    if ($Dependencies.Count -gt 0) {
        $dependenciesNode = $doc.CreateElement('dependencies', $ns)
        [void]$metadata.AppendChild($dependenciesNode)
        $group = $doc.CreateElement('group', $ns)
        [void]$group.SetAttribute('targetFramework', 'net48')
        [void]$dependenciesNode.AppendChild($group)
        foreach ($dependency in $Dependencies) {
            $item = $doc.CreateElement('dependency', $ns)
            [void]$item.SetAttribute('id', [string]$dependency.Id)
            [void]$item.SetAttribute('version', [string]$dependency.Version)
            [void]$group.AppendChild($item)
        }
    }
    $path = Join-Path $Stage "$Id.nuspec"
    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Encoding = [System.Text.UTF8Encoding]::new($false)
    $settings.Indent = $true
    $writer = [System.Xml.XmlWriter]::Create($path, $settings)
    try { $doc.Save($writer) } finally { $writer.Dispose() }
    return $path
}

function New-Package([string]$Id, [string]$Description, [object[]]$Dependencies, [scriptblock]$StageFiles) {
    $stage = Join-Path $stagingDir $Id
    New-Item -ItemType Directory -Path $stage -Force | Out-Null
    & $StageFiles $stage
    $nuspec = Write-Nuspec $stage $Id $Description $Dependencies
    if ($packer -eq 'nuget') {
        & $NuGetExe pack $nuspec -OutputDirectory $outputDir -NonInteractive
    } else {
        & $dotnetExe pack $nuspec --output $outputDir
    }
    if ($LASTEXITCODE -ne 0) { throw "Package creation failed for $Id (exit $LASTEXITCODE)." }
    [void](Require-File (Join-Path $outputDir "$Id.$Version.nupkg"))
    Write-Host "Created $Id.$Version.nupkg"
}

if (Test-Path -LiteralPath $outputDir) {
    throw "The output folder already exists: $outputDir. Choose a new version or move the existing output first."
}
[void](Require-File $solution)
if (-not $NuGetExe) {
    $nugetCommand = Get-Command NuGet.exe -ErrorAction SilentlyContinue
    if ($nugetCommand) { $NuGetExe = $nugetCommand.Source }
}
$packer = $null
$dotnetExe = $null
if ($NuGetExe) {
    $NuGetExe = Require-File $NuGetExe
    $productVersion = [Diagnostics.FileVersionInfo]::GetVersionInfo($NuGetExe).ProductVersion
    if (-not ($productVersion -match '^(\d+)\.(\d+)')) { throw "Cannot read NuGet.exe version: $NuGetExe" }
    if ([int]$Matches[1] -lt 5) { throw "NuGet.exe 5 or newer is required for net48 packages: $NuGetExe" }
    $packer = 'nuget'
} else {
    $dotnetCommand = Get-Command dotnet.exe -ErrorAction SilentlyContinue
    if ($dotnetCommand) {
        $dotnetExe = $dotnetCommand.Source
        $sdks = @(& $dotnetExe --list-sdks)
        if (@($sdks | Where-Object { $_ -match '^(1[0-9]|[2-9][0-9])\.' }).Count -gt 0) {
            $packer = 'dotnet'
        }
    }
    if (-not $packer) {
        throw 'Use NuGet.exe 5 or newer (-NuGetExe path), or install a .NET 10+ SDK for direct .nuspec packing. The bundled NuGet.exe 2.8 is too old.'
    }
}
Write-Host "Using $packer to create packages."
if (-not $SkipBuild) {
    $msbuild = Find-MSBuild
    & $msbuild $solution '/t:Build' '/p:Configuration=Release' '/p:Platform=Any CPU' '/m' '/nologo'
    if ($LASTEXITCODE -ne 0) { throw "Release build failed (exit $LASTEXITCODE)." }
}

$libraryConfig = Join-Path $repoDir 'src\Framework\N2\packages.config'
$razorConfig = Join-Path $repoDir 'src\Framework\Razor\packages.config'
$managementConfig = Join-Path $repoDir 'src\Mvc\MvcTemplates\N2\packages.config'
$searchConfig = Join-Path $repoDir 'src\Framework\Search.Lucene\packages.config'
$mvcConfig = Join-Path $repoDir 'src\Mvc\MvcTemplates\packages.config'
$libraryDependencies = @(ExternalDependencies $libraryConfig)
$razorDependencies = @((ExactDependency 'VP_N2CMS.Library')) + @(ExternalDependencies $razorConfig)
$managementDependencies = @(
    (ExactDependency 'VP_N2CMS.Library'), (ExactDependency 'VP_N2CMS.Config'),
    (Dependency 'SharpZipLib' ("[{0}]" -f (Package-Version $managementConfig 'SharpZipLib'))),
    (Dependency 'System.Data.SQLite.Linq' ("[{0}]" -f (Package-Version $managementConfig 'System.Data.SQLite.Linq')))
)
$searchDependencies = @((ExactDependency 'VP_N2CMS.Library')) + @(ExternalDependencies $searchConfig)
$mvcDependencies = @(
    (ExactDependency 'VP_N2CMS.Library'), (ExactDependency 'VP_N2CMS.Razor'),
    (Dependency 'Microsoft.AspNet.Mvc' ("[{0}]" -f (Package-Version $mvcConfig 'Microsoft.AspNet.Mvc')))
)
$sqliteDependencies = @(
    (Dependency 'System.Data.SQLite.Core' ("[{0}]" -f (Package-Version $mvcConfig 'System.Data.SQLite.Core')))
)
$frameworkDependencies = @(
    (ExactDependency 'VP_N2CMS.Config'), (ExactDependency 'VP_N2CMS.Library'),
    (ExactDependency 'VP_N2CMS.Management'), (ExactDependency 'VP_N2CMS.Search.Lucene')
)

$requiredBinaries = @(
    'src\Framework\N2\bin\Release\N2.dll',
    'src\Framework\Extensions\bin\Release\N2.Extensions.dll',
    'src\Framework\Packaging\bin\Release\N2.Packaging.dll',
    'src\Framework\Razor\bin\Release\N2.Razor.dll',
    'src\Framework\Search.Lucene\bin\Release\N2.Search.Lucene.dll',
    'src\Mvc\MvcTemplates\bin\N2.Management.dll'
)
foreach ($relative in $requiredBinaries) { [void](Require-File (Join-Path $repoDir $relative)) }
New-Item -ItemType Directory -Path $stagingDir -Force | Out-Null

New-Package -Id 'VP_N2CMS.Config' -Description 'N2 CMS web configuration transforms.' -Dependencies @() -StageFiles {
    param($stage)
    Copy-Tree (Join-Path $buildDir 'nuget\Config\Content') (Join-Path $stage 'content')
}
New-Package -Id 'VP_N2CMS.Config.SQLite' -Description 'SQLite connection configuration for N2 CMS.' -Dependencies $sqliteDependencies -StageFiles {
    param($stage)
    Copy-Tree (Join-Path $buildDir 'nuget\SQLite\Content') (Join-Path $stage 'content')
}
New-Package -Id 'VP_N2CMS.Library' -Description 'N2 CMS core library.' -Dependencies $libraryDependencies -StageFiles {
    param($stage)
    Copy-Required (Join-Path $repoDir 'src\Framework\N2\bin\Release\N2.dll') (Join-Path $stage 'lib\net48\N2.dll')
    $xml = Join-Path $repoDir 'src\Framework\N2\bin\N2.XML'
    Copy-Required $xml (Join-Path $stage 'lib\net48\N2.XML')
}
New-Package -Id 'VP_N2CMS.Management' -Description 'N2 CMS management interface and virtual path support.' -Dependencies $managementDependencies -StageFiles {
    param($stage)
    foreach ($entry in @(
        @('src\Mvc\MvcTemplates\bin\N2.Management.dll', 'N2.Management.dll'),
        @('src\Framework\Extensions\bin\Release\N2.Extensions.dll', 'N2.Extensions.dll'),
        @('src\Framework\Packaging\bin\Release\N2.Packaging.dll', 'N2.Packaging.dll'))) {
        Copy-Required (Join-Path $repoDir $entry[0]) (Join-Path $stage ("lib\net48\{0}" -f $entry[1]))
    }
    Copy-Tree (Join-Path $buildDir 'nuget\ManagementZip\Content') (Join-Path $stage 'content')
    $managementProject = Join-Path $repoDir 'src\Mvc\MvcTemplates\N2\N2.Management.csproj'
    [xml]$project = Get-Content -LiteralPath (Require-File $managementProject) -Raw
    $namespace = [System.Xml.XmlNamespaceManager]::new($project.NameTable)
    $namespace.AddNamespace('m', 'http://schemas.microsoft.com/developer/msbuild/2003')
    $siteRoot = Join-Path $stagingDir 'management-site'
    $siteN2 = Join-Path $siteRoot 'N2'
    foreach ($node in $project.SelectNodes('//m:Content[@Include]', $namespace)) {
        $relative = [Uri]::UnescapeDataString([string]$node.Include)
        $source = Join-Path (Split-Path -Parent $managementProject) $relative
        Copy-Required $source (Join-Path $siteN2 $relative)
    }
    $zipPath = Join-Path $stage 'content\N2\N2.zip'
    New-Item -ItemType Directory -Path (Split-Path -Parent $zipPath) -Force | Out-Null
    $buildSupport = Require-File (Join-Path $buildDir 'lib\BuildSupport.exe')
    & $buildSupport ZipDirectory $zipPath $siteN2 'N2'
    if ($LASTEXITCODE -ne 0) { throw "Management ZIP creation failed (exit $LASTEXITCODE)." }
    [void](Require-File $zipPath)
    Copy-Required (Join-Path $siteN2 'Web.config') (Join-Path $stage 'content\N2\Web.config')
    foreach ($doc in @('history.txt', 'howto_install.txt', 'howto_develop.txt', 'License.txt', 'n2_readme.txt', 'upgrade.txt', 'n2cms.configuration.xsd')) {
        Copy-Required (Join-Path $repoDir "docs\$doc") (Join-Path $stage "content\N2\$doc")
    }
}
New-Package -Id 'VP_N2CMS.Razor' -Description 'N2 CMS Razor integration.' -Dependencies $razorDependencies -StageFiles {
    param($stage)
    Copy-Required (Join-Path $repoDir 'src\Framework\Razor\bin\Release\N2.Razor.dll') (Join-Path $stage 'lib\net48\N2.Razor.dll')
}
New-Package -Id 'VP_N2CMS.Mvc' -Description 'N2 CMS MVC bootstrap code.' -Dependencies $mvcDependencies -StageFiles {
    param($stage)
    Copy-Required (Join-Path $buildDir 'nuget\Mvc\Content\net45\App_Start\CmsConfig.cs.pp') (Join-Path $stage 'content\App_Start\CmsConfig.cs.pp')
}
New-Package -Id 'VP_N2CMS.Search.Lucene' -Description 'N2 CMS Lucene search integration.' -Dependencies $searchDependencies -StageFiles {
    param($stage)
    Copy-Required (Join-Path $repoDir 'src\Framework\Search.Lucene\bin\Release\N2.Search.Lucene.dll') (Join-Path $stage 'lib\net48\N2.Search.Lucene.dll')
}
New-Package -Id 'VP_N2CMS' -Description 'N2 CMS package bundle for VPBase3.' -Dependencies $frameworkDependencies -StageFiles {
    param($stage)
    foreach ($doc in @('history.txt', 'howto_install.txt', 'howto_develop.txt', 'License.txt', 'n2_readme.txt', 'upgrade.txt', 'n2cms.configuration.xsd')) {
        Copy-Required (Join-Path $repoDir "docs\$doc") (Join-Path $stage "content\N2\$doc")
    }
}
Remove-Item -LiteralPath $stagingDir -Recurse -Force
Write-Host "Eight local packages are ready in $outputDir"
