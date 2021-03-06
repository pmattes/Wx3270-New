﻿# Set up constants.
$inno = 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe'
$cert = 'C:\Users\Paul Mattes\Documents\Cert2020.p12'
$signtool = 'C:\Program Files (x86)\Windows Kits\10\bin\x86\signtool.exe'
$timestamp = 'http://timestamp.digicert.com'

# Any error kills the script.
$ErrorActionPreference = 'Stop'

# Read the password.
$p = Read-Host 'Enter key password' -AsSecureString
$pass = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($p))

# Build.
Write-Host -ForegroundColor Green 'Building x86'
& msbuild wx3270\wx3270.sln /t:Rebuild "/p:Configuration=Release;Platform=x86" /nologo /v:q
Write-Host -ForegroundColor Green 'Building x64'
& msbuild wx3270\wx3270.sln /t:Rebuild "/p:Configuration=Release;Platform=x64" /nologo /v:q

# Sign the binaries.
$exclude = "Newtonsoft.Json.dll", "System.ValueTuple.dll"
$files = `
   (Get-ChildItem -Path Wx3270\bin\x86\Release, Wx3270\bin\x64\Release -Recurse -Filter "*.exe").FullName `
 + (Get-ChildItem -Path Wx3270\bin\x86\Release, Wx3270\bin\x64\Release -Recurse -Filter "*.dll" -Exclude $exclude).FullName `
 + (Get-ChildItem -Path Wx3270Restrict\bin\Release -Recurse -Filter "*.exe").FullName
Write-Host -ForegroundColor Green 'Signing', $files.Count, 'binaries'
& $signtool sign /f $cert /p $pass /td SHA256 /tr $timestamp $files

# Run Inno Setup to create the installer.
Write-Host -ForegroundColor Green 'Running Inno Setup'
$signparm = '/smystandard="' + "$signtool sign /f `$q$cert`$q /p $pass /td SHA256 /tr $timestamp `$p" + '"'
& $inno $signparm /Qp wx3270.iss
