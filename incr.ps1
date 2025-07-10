<#
    .SYNOPSIS
     wx3270 incremental release script
#>
[CmdletBinding(DefaultParameterSetName = 'Default')]
param (
    # Archive the code.
    [switch]$archive,
    # Commit the code to git.
    [switch]$commit,
    # Push the code to github.
    [switch]$push,
    # Sign the binaries.
    [switch]$sign,
    # Shortcut for -archive -commit -push.
    [switch]$go,
    # Do a pre-release rather than a proper release.
    [switch]$pre
)

# Any error kills the script.
$ErrorActionPreference = 'Stop'

# '-go' means 'do it all for real'
if ($go) {
    $archive = $true
    $commit = $true
    $push = $true
    $sign = $true
}

# Set up constants.
$inno = 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe'
$release_path = 'Wx3270\Release.cs'
$release_pattern = '^        private const string PreReleaseIteration = "([A-Z])"'
$assembly_info_path = 'wx3270\Properties\AssemblyInfo.cs'
$assembly_info_pattern = '^\[assembly: AssemblyVersion\("([0-9.]+)"\)'

# Make sure we are not running from Visual Studio.
if (Test-Path 'env:VisualStudioDir') {
    Write-Error "This script cannot be run from Visual Studio (it breaks signing)"
}

# Make sure we're in the right spot.
if (-not (Test-Path "wx3270.iss" -PathType Leaf)) {
    Write-Error "Must run in the wx3270 repo folder (cannot find wx3270.iss)"
}
$status = (git status)
if ($status -match "^fatal:") {
    Write-Error "Must run in the wx3270 repo folder (git is unhappy)"
}

# Make sure nothing is checked out.
if (-not ($status -match "^nothing to commit,")) {
    Write-Error "Cannot run when there are files checked out"
}

# Make sure we are on a release branch.
$branch = ($status[0] -split " ")[2]
if (-not ($branch -match "^[0-9]+\.[0-9]+$")) {
    Write-Error "Not on a release branch ($branch)"
}

# Read in and check the .cs files we're going to change.
# We do this ahead of time, because these are the likeliest things to go wrong and
# we don't want to get too far into the process before stumbling over them.
$release_cs = (Get-Content -Encoding UTF8 $release_path)
if (-not ($release_cs[17] -match $release_pattern)) {
    Write-Error "$release_path line 18 is not as expected"
}
$assembly_info = (Get-Content -Encoding UTF8 $assembly_info_path)
if (-not ($assembly_info[37] -match $assembly_info_pattern)) {
    Write-Error "$assembly_info_path line 38 is not as expected"
}

# Set the PreReleaseIteration.
$iteration = $release_cs[17]
if (-not ($iteration -match $release_pattern)) {
    Write-Error "$release_path line 18 is not as expected ($iteration)"
}
if ($pre) {
    $next_letter = [char]([int]([char]$Matches[1]) + 1)
    $next_pre = '        private const string PreReleaseIteration = "' + $next_letter + '";'
} else {
    $release_cs[17] = "        private const string PreReleaseIteration = null;"
    Set-Content -Encoding UTF8 $release_path $release_cs
}

# Set up Azure login to do signing.
Connect-AzAccount | Out-Null 

# Build.
Write-Host -ForegroundColor Green 'Building x86'
& msbuild wx3270\wx3270.sln /t:Rebuild "/p:Configuration=Release;Platform=x86" /nologo /v:q
Write-Host -ForegroundColor Green 'Building x64'
& msbuild wx3270\wx3270.sln /t:Rebuild "/p:Configuration=Release;Platform=x64" /nologo /v:q

# Sign the binaries that are not yet signed.
if ($sign) {
    $exclude = "Newtonsoft.Json.dll"
    $files = `
       (Get-ChildItem -Path Wx3270\bin\x86\Release, Wx3270\bin\x64\Release -Recurse -Filter "*.exe").FullName `
     + (Get-ChildItem -Path Wx3270\bin\x86\Release, Wx3270\bin\x64\Release -Recurse -Filter "*.dll" -Exclude $exclude).FullName `
     + (Get-ChildItem -Path Wx3270Restrict\bin\Release -Recurse -Filter "*.exe").FullName |
       Where-Object { (Get-AuthenticodeSignature $_).Status -eq "NotSigned" }
    if ($files.Count -gt 0)
    {
        Write-Host -ForegroundColor Green 'Signing', $files.Count, "binaries"
        & .\run-signtool.ps1 $files
    }
}

# Figure out the version.
$vfile = [System.IO.Path]::GetTempFileName()
Start-Process -Wait -FilePath .\Wx3270\bin\x64\Release\wx3270.exe -ArgumentList "-vfile",$vfile
$version = ((Get-Content $vfile) -split " ")[1]
Remove-Item $vfile
Write-Host -ForegroundColor Green 'Version is', $version

# Filter the Inno Setup file.
$iss = ((Get-Content -Encoding UTF8 wx3270.iss) -replace "%VERSION%", $version) -replace "%YEAR%", (Get-Date).Year
if (-not $sign) {
    $iss = $iss -replace "SignTool", "; SignTool"
}
Set-Content -Encoding UTF8 -Path tmp.iss -Value $iss

# Run Inno Setup to create the installer.
Write-Host -ForegroundColor Green 'Running Inno Setup'
$cwd = (Get-Location)
if ($sign) {
    $signparm = '/smystandard="' + "powershell.exe $cwd\run-signtool.ps1 `$p" + '"'
}
& $inno $signparm /Qp tmp.iss
Remove-Item tmp.iss

# Create the no-install zipfiles.
Write-Host -ForegroundColor Green 'Creating no-install zipfiles'
$files = Get-Content noinstall-files.txt
$files.ForEach({$_ -replace '^', 'wx3270\bin\x64\Release\'}) | Compress-Archive -Force -DestinationPath "wx3270-$version-noinstall64.zip"
$files.ForEach({$_ -replace '^', 'wx3270\bin\x86\Release\'}) | Compress-Archive -Force -DestinationPath "wx3270-$version-noinstall32.zip"

# Archive.
if ($archive)
{
    Write-Host -ForegroundColor Green 'Archiving'
    $env:PATH += ';C:\Windows\System32\OpenSSH'
    $files = "wx3270-$version-setup.exe", "wx3270-$version-noinstall64.zip", "wx3270-$version-noinstall32.zip"
    & scp $files 10.0.0.12:psrc/x3270/Release/
    $verparts = $version -replace "[a-z]+.*", "" -split "\."
    $bgpdir = "www/download/wx3270/{0:D2}.{1:D2}" -f [int]$verparts[0],[int]$verparts[1]
    & ssh bgp.nu "mkdir -p $bgpdir"
    & scp $files bgp.nu:$bgpdir/
    & scp $files pmattes@frs.sourceforge.net:/home/frs/p/x3270/wx3270
}

# Commit.
if ($commit)
{
    Write-Host -ForegroundColor Green "Committing $version"
    & git add .
    & git commit -m$version
    if (-not $pre) {
        & git tag -a -m$version $version
    }
    if ($push) {
        Write-Host -ForegroundColor Green "Pushing $version"
        & git push
        if (-not $pre) {
            & git push --tags
        }
    }
}

# Get ready for the next version.
if ($pre) {
    $release_cs[17] = $next_pre
} else {
    # Start over with "A" in Release.cs.
    $release_cs[17] = '        private const string PreReleaseIteration = "A";'
}
Set-Content -Encoding UTF8 $release_path $release_cs

if (-not $pre) {
    # Increment the assembly and file versions (third field).
    if (-not ($assembly_info[37] -match $assembly_info_pattern)) {
        Write-Error "$assembly_info_path version is not as expected"
    }
    $split_version = $Matches[1].Split(".")
    $split_version[2] = [int]$split_version[2] + 1
    $new_version = $split_version -join "."
    $new_display_version = $split_version[0] + "." + $split_version[1] + "x" + $split_version[2]
    $assembly_info[37] = $assembly_info[37] -replace '[0-9.]+', $new_version
    $assembly_info[38] = $assembly_info[38] -replace '[0-9.]+', $new_version
    Set-Content -Encoding UTF8 $assembly_info_path $assembly_info
} else {
    $new_display_version = "Pre-" + $next_letter
}
Write-Host -ForegroundColor Green "Getting ready for $new_display_version"
if ($commit) {
    Write-Host -ForegroundColor Green "Committing $new_display_version"
    git add .
    & git commit -m"Get ready for $new_display_version"
    if ($push) {
        Write-Host -ForegroundColor Green "Pushing $new_display_version"
        & git push
    }
}
