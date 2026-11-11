param([string]$OutputDirectory = (Split-Path $PSScriptRoot -Parent))
$ErrorActionPreference = 'Stop'
$taskRoot = $PSScriptRoot
$finalRoot = Split-Path $taskRoot -Parent
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (-not $OutputDirectory.Equals($finalRoot,[StringComparison]::OrdinalIgnoreCase) -and -not $OutputDirectory.StartsWith($finalRoot + '\',[StringComparison]::OrdinalIgnoreCase)) { throw 'Build outputs must stay inside the permanent CosmoTweest FINAL folder.' }
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
foreach ($name in @('CosmoTweest.exe','CosmoTweestSetup.exe')) {
    $target = Join-Path $OutputDirectory $name
    if (Test-Path -LiteralPath $target) {
        try { $handle = [IO.File]::Open($target,'Open','ReadWrite','None'); $handle.Dispose() }
        catch { throw "Close the running game/setup before rebuilding: $target" }
    }
}
$staging = Join-Path $taskRoot ('build-staging\' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $staging | Out-Null
$sourceFiles = @(Get-ChildItem (Join-Path $taskRoot 'src') -Filter '*.cs' | Sort-Object Name | ForEach-Object FullName)
& $compiler /nologo /target:winexe /platform:x64 /optimize+ "/win32manifest:$taskRoot\src\app.manifest" "/out:$staging\CosmoTweest.exe" "/win32icon:$taskRoot\src\CosmoTweest.ico" /reference:System.Drawing.dll /reference:System.Windows.Forms.dll @sourceFiles
if ($LASTEXITCODE -ne 0) { throw 'Game compilation failed; published files were left unchanged.' }
& $compiler /nologo /target:winexe /platform:x64 /optimize+ "/win32manifest:$taskRoot\src\app.manifest" "/out:$staging\CosmoTweestSetup.exe" "/win32icon:$taskRoot\src\CosmoTweest.ico" /reference:System.Drawing.dll /reference:System.Windows.Forms.dll "/resource:$staging\CosmoTweest.exe,GamePayload" "/resource:$taskRoot\LICENSE.txt,LICENSE.txt" "/resource:$taskRoot\CREDITS.md,CREDITS.md" "$taskRoot\tools\Installer.cs"
if ($LASTEXITCODE -ne 0) { throw 'Installer compilation failed; published files were left unchanged.' }
Copy-Item "$staging\CosmoTweest.exe","$staging\CosmoTweestSetup.exe" -Destination $OutputDirectory -Force
New-Item -ItemType Directory -Force (Join-Path $OutputDirectory 'Credits') | Out-Null
Copy-Item "$taskRoot\LICENSE.txt","$taskRoot\CREDITS.md" -Destination (Join-Path $OutputDirectory 'Credits') -Force
if ($OutputDirectory.TrimEnd('\') -eq $finalRoot.TrimEnd('\')) {
    New-Item -ItemType Directory -Force (Join-Path $finalRoot 'Data') | Out-Null
    $marker = Join-Path $finalRoot 'Data\portable.mode'
    if (-not (Test-Path -LiteralPath $marker)) { Set-Content -LiteralPath $marker -Value 'CosmoTweest portable profile. Keep with the Data folder.' }
}
$resolvedStaging = (Resolve-Path -LiteralPath $staging).Path
if (-not $resolvedStaging.StartsWith((Join-Path $taskRoot 'build-staging') + '\',[StringComparison]::OrdinalIgnoreCase)) { throw 'Unexpected staging directory' }
Remove-Item -LiteralPath $resolvedStaging -Recurse -Force
Write-Host "Updated the game and setup in: $OutputDirectory"
Write-Host 'Existing saved progress was preserved.'
