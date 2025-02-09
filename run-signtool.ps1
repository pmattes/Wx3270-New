# run-signtool.ps1 <files-to-sign>

# Any error kills the script.
$ErrorActionPreference = 'Stop'

# Find everything.
$signtool = 'C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\signtool.exe'
$dlib = 'C:\Users\pdm\AppData\Local\Microsoft\MicrosoftTrustedSigningClientTools\Azure.CodeSigning.Dlib.dll'
$timestamp = 'http://timestamp.acs.microsoft.com'

# Put the Azure Trusted Signing metadata in a temporary file.
$tempJson = Get-ChildItem ([IO.Path]::GetTempFileName()) | Rename-Item -NewName { [IO.Path]::ChangeExtension($_, ".json") } -PassThru
$json = @'
{
  "Endpoint": "https://wcus.codesigning.azure.net",
  "CodeSigningAccountName": "x3270",
  "CertificateProfileName": "x3270",
  "ExcludeCredentials": [
    "AzureCliCredential",
    "ManagedIdentityCredential",
    "EnvironmentCredential",
    "WorkloadIdentityCredential",
    "SharedTokenCacheCredential",
    "VisualStudioCredential",
    "VisualStudioCodeCredential",
    "AzureDeveloperCliCredential",
    "InteractiveBrowserCredential"
  ]
}
'@
$json | Out-File -FilePath $tempJson -Encoding ascii

# Sign, suppressing output.
$count = 0
foreach ($file in $args[0])
{
    $percent = ($count / $args[0].Count) * 100
    $remaining = ($args[0].Count - $count) * 5
    $count = $count + 1
    Write-Progress -Activity "Signing" -CurrentOperation $file -PercentComplete $percent -SecondsRemaining $remaining
    $out = (& $signtool sign /v /td SHA256 /tr $timestamp /fd SHA256 /dlib $dlib /dmdf $tempjson $file)
    if ((Get-AuthenticodeSignature $file).Status -ne "Valid")
    {
        Write-Error "$file not signed"
        Write-Error "Signtool output: $out"
    }
}
Write-Progress -Activity "Signing" -Completed
Remove-Item $tempJson

# Check.
$invalid = ($args[0] | Where-Object { (Get-AuthenticodeSignature $_).Status -ne "Valid" })
if ($invalid.Count -ne 0)
{
    # Signtool failed.
    Write-Error "File(s) were not signed: $invalid"
    exit 1
}