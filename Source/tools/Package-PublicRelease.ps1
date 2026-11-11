param([string]$OutputDirectory = (Join-Path (Split-Path $PSScriptRoot -Parent) 'tests\public-release'))
$ErrorActionPreference = 'Stop'
$sourceRoot = Split-Path $PSScriptRoot -Parent
$gameRoot = Split-Path $sourceRoot -Parent
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (-not $OutputDirectory.StartsWith($gameRoot + '\',[StringComparison]::OrdinalIgnoreCase)) { throw 'Release output must stay inside the final project folder.' }
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$stage = Join-Path $OutputDirectory ('stage-' + [Guid]::NewGuid().ToString('N'))
$portable = Join-Path $stage 'CosmoTweest FINAL'
$sourceArchive = Join-Path $stage 'source.zip'
New-Item -ItemType Directory -Force $stage | Out-Null
Push-Location $gameRoot
try {
    & git archive --format=zip --output $sourceArchive HEAD
    if ($LASTEXITCODE -ne 0) { throw 'Create a reviewed source commit before packaging a public release.' }
} finally { Pop-Location }
Expand-Archive -LiteralPath $sourceArchive -DestinationPath $portable
if (Test-Path -LiteralPath (Join-Path $portable 'Data\settings.txt')) { throw 'Refusing to publish a player save.' }
if (Test-Path -LiteralPath (Join-Path $portable 'Source\tests\publish-tools')) { throw 'Refusing to publish local tooling.' }
& (Join-Path $portable 'Source\build.ps1')
if ($LASTEXITCODE -ne 0) { throw 'Clean committed-source release build failed.' }
$zip = Join-Path $OutputDirectory 'CosmoTweest-Portable.zip'
Compress-Archive -LiteralPath $portable -DestinationPath $zip -Force
$setup = Join-Path $OutputDirectory 'CosmoTweestSetup.exe'
Copy-Item (Join-Path $portable 'CosmoTweestSetup.exe') -Destination $setup -Force
Get-FileHash -Algorithm SHA256 $zip,$setup | ForEach-Object {
    $_.Hash.ToLowerInvariant() + '  ' + (Split-Path $_.Path -Leaf)
} | Set-Content -LiteralPath (Join-Path $OutputDirectory 'SHA256SUMS.txt') -Encoding ascii
Write-Host "Clean public assets: $OutputDirectory"
Write-Host 'Only committed source and the final executables were packaged; player profiles were excluded.'
