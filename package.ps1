﻿param (
    # Archive flag
    [switch]$archive = $false,
    # Commit flag
    [switch]$commit = $false
)

# Set up constants.
$inno = 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe'

# Any error kills the script.
$ErrorActionPreference = 'Stop'

# Set up Azure login to do signing.
Connect-AzAccount | Out-Null

# Build.
Write-Host -ForegroundColor Green 'Building x86'
& msbuild wx3270\wx3270.sln /t:Rebuild "/p:Configuration=Release;Platform=x86" /nologo /v:q
Write-Host -ForegroundColor Green 'Building x64'
& msbuild wx3270\wx3270.sln /t:Rebuild "/p:Configuration=Release;Platform=x64" /nologo /v:q

# Sign the binaries that are not yet signed.
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

# Figure out the version.
$vfile = [System.IO.Path]::GetTempFileName()
Start-Process -Wait -FilePath .\Wx3270\bin\x64\Release\wx3270.exe -ArgumentList "-vfile",$vfile
$version = ((Get-Content $vfile) -split " ")[1]
Remove-Item $vfile
Write-Host -ForegroundColor Green 'Version is', $version

# Filter the Inno Setup file.
$iss = ((Get-Content wx3270.iss) -replace "%VERSION%", $version) -replace "%YEAR%", (Get-Date).Year
Set-Content -Encoding UTF8 -Path tmp.iss -Value $iss

# Run Inno Setup to create the installer.
Write-Host -ForegroundColor Green 'Running Inno Setup'
$cwd = (Get-Location)
$signparm = '/smystandard="' + "powershell.exe $cwd\run-signtool.ps1 `$p" + '"'
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
    & scp $files pmattes@frs.sourceforge.net:/home/frs/p/x3270/wx3270/
}

# Commit.
if ($commit)
{
    Write-Host -ForegroundColor Green 'Committing'
    & git add .
    & git commit -m$version
    & git tag -a -m$version $version
    & git push
    & git push --tags
}