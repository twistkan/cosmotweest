$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$testOutput = Join-Path $PSScriptRoot 'build'
& (Join-Path $taskRoot 'build.ps1') -OutputDirectory $testOutput
foreach ($mode in @('--selftest', '--qa', '--benchmark')) {
    $startedAt = [DateTime]::UtcNow
    $process = Start-Process -FilePath (Join-Path $testOutput 'CosmoTweest.exe') -ArgumentList $mode -PassThru
    if (-not $process.WaitForExit(180000)) {
        $process.Kill()
        throw "Timed out: $mode"
    }
    if ($process.ExitCode -ne 0) { throw "Failed: $mode" }
    $resultName = if ($mode -eq '--selftest') { 'selftest-results.txt' } elseif ($mode -eq '--qa') { 'ui-qa-results.txt' } else { 'benchmark-results.txt' }
    $resultFile = Join-Path $testOutput $resultName
    if (-not (Test-Path $resultFile) -or (Get-Item $resultFile).LastWriteTimeUtc -lt $startedAt) { throw "No fresh report: $mode" }
}
$report = Join-Path $PSScriptRoot 'campaign-report.txt'
$startedAt = [DateTime]::UtcNow
$process = Start-Process -FilePath (Join-Path $testOutput 'CosmoTweest.exe') -ArgumentList "--autoplay --report `"$report`"" -PassThru
if (-not $process.WaitForExit(600000)) {
    $process.Kill()
    throw 'Campaign QA timed out'
}
if (-not (Test-Path $report) -or (Get-Item $report).LastWriteTimeUtc -lt $startedAt -or (Get-Content $report -Raw) -notmatch 'VICTORY') { throw 'Campaign QA failed' }
Write-Host 'Verification completed. Review Source/tests/build/*-results.txt and tests/campaign-report.txt.'
