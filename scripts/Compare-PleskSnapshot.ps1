[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateScript({ Test-Path -LiteralPath $_ -PathType Container })]
    [string]$PleskSnapshot,

    [ValidateScript({ Test-Path -LiteralPath $_ -PathType Container })]
    [string]$LocalRoot = (Join-Path $PSScriptRoot '..\ShipleySwine'),

    [string]$CsvPath
)

$ErrorActionPreference = 'Stop'

$ignoredDirectories = @(
    '.git', '.vs', 'bin', 'obj', 'packages'
)

function Get-FileInventory {
    param([string]$Root)

    $resolvedRoot = (Resolve-Path -LiteralPath $Root).Path.TrimEnd('\', '/')
    $files = Get-ChildItem -LiteralPath $resolvedRoot -File -Recurse
    $inventory = @{}

    foreach ($file in $files) {
        $relativePath = $file.FullName.Substring($resolvedRoot.Length).TrimStart('\', '/')
        $segments = $relativePath -split '[\\/]'

        if ($segments | Where-Object { $ignoredDirectories -contains $_ }) {
            continue
        }

        $key = $relativePath.Replace('\', '/').ToLowerInvariant()
        $inventory[$key] = [pscustomobject]@{
            RelativePath = $relativePath.Replace('\', '/')
            FullName     = $file.FullName
            Length       = $file.Length
        }
    }

    return $inventory
}

$localFiles = Get-FileInventory -Root $LocalRoot
$pleskFiles = Get-FileInventory -Root $PleskSnapshot
$allKeys = @($localFiles.Keys + $pleskFiles.Keys | Sort-Object -Unique)
$results = foreach ($key in $allKeys) {
    $local = $localFiles[$key]
    $plesk = $pleskFiles[$key]

    if ($null -eq $local) {
        [pscustomobject]@{ Status = 'Plesk only'; Path = $plesk.RelativePath }
        continue
    }

    if ($null -eq $plesk) {
        [pscustomobject]@{ Status = 'Local only'; Path = $local.RelativePath }
        continue
    }

    $isDifferent = $local.Length -ne $plesk.Length
    if (-not $isDifferent) {
        $localHash = (Get-FileHash -LiteralPath $local.FullName -Algorithm SHA256).Hash
        $pleskHash = (Get-FileHash -LiteralPath $plesk.FullName -Algorithm SHA256).Hash
        $isDifferent = $localHash -ne $pleskHash
    }

    if ($isDifferent) {
        [pscustomobject]@{ Status = 'Changed'; Path = $local.RelativePath }
    }
}

$results = @($results | Sort-Object Status, Path)

if ($results.Count -eq 0) {
    Write-Host 'No file differences found.'
} else {
    $results | Format-Table -AutoSize
}

Write-Host ""
Write-Host "Summary"
Write-Host ("  Changed:    {0}" -f @($results | Where-Object Status -eq 'Changed').Count)
Write-Host ("  Plesk only: {0}" -f @($results | Where-Object Status -eq 'Plesk only').Count)
Write-Host ("  Local only: {0}" -f @($results | Where-Object Status -eq 'Local only').Count)

if ($CsvPath) {
    $results | Export-Csv -LiteralPath $CsvPath -NoTypeInformation
    Write-Host "Report written to $CsvPath"
}

