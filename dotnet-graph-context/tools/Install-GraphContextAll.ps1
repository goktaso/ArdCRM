<#
.SYNOPSIS
    Bir kok klasordeki (orn. D:\C#) .NET projelerini tarar, siniflandirir ve
    uygun olanlara Graph Context baglam sistemini kurar.

.DESCRIPTION
    Varsayilan davranis **plan modudur**: hicbir dosya yazilmaz, yalnizca her proje
    icin ne yapilacagi raporlanir. Kurulum icin -Apply verilmelidir.

    Siniflandirma:
      Uygun        : cok projeli .NET cozumu - kurulur
      Incelenecek  : tek katmanli / test projesi yok / egitim-ornek isim kalibi
                     - yalnizca -IncludeMarginal ile kurulur
      Atlandi      : .NET projesi yok veya cok kucuk olcek - kurulmaz

    Varsayilan kurulum modu Sidecar'dir: proje klasorlerinde hicbir dosya degismez.

.PARAMETER Root
    Taranacak kok klasor. Varsayilan: D:\C#

.PARAMETER Priority
    Once degerlendirilecek proje adlari (listede ustte gosterilir).

.PARAMETER Apply
    Verilmezse yalnizca plan gosterilir; verilirse kurulum yapilir.

.EXAMPLE
    .\Install-GraphContextAll.ps1
    # yalnizca plan

.EXAMPLE
    .\Install-GraphContextAll.ps1 -Apply -WithLauncher
    # sidecar kurulum + baslatma kisayollari

.EXAMPLE
    .\Install-GraphContextAll.ps1 -Root "D:\C#" -Apply -ReportPath "D:\C#\_graph-context\kurulum-raporu.md"
#>
[CmdletBinding()]
param(
    [string]$Root = 'D:\C#',

    [ValidateSet('Sidecar', 'InPlace')]
    [string]$Mode = 'Sidecar',

    [string]$SidecarRoot,

    [string[]]$Priority = @('PackErp', 'ArdCRM', 'ProductionPlanning', 'EtiketTasarim'),

    [switch]$Apply,
    [switch]$IncludeMarginal,
    [switch]$AddPointer,
    [switch]$LinkPointer,
    [switch]$WithLauncher,
    [switch]$NoDetect,
    [string]$ReportPath
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'GraphContext.Detection.ps1')
$initScript = Join-Path $PSScriptRoot 'Initialize-GraphContext.ps1'

if (-not (Test-Path -LiteralPath $Root -PathType Container)) {
    throw "Kok klasor bulunamadi: $Root"
}
$Root = (Resolve-Path -LiteralPath $Root).Path
if (-not $SidecarRoot) { $SidecarRoot = Join-Path $Root '_graph-context' }

Write-Host ""
Write-Host "Graph Context toplu kurulum" -ForegroundColor Cyan
Write-Host ("  kok klasor   : {0}" -f $Root)
Write-Host ("  kurulum modu : {0}" -f $Mode)
if ($Mode -eq 'Sidecar') { Write-Host ("  sidecar kok  : {0}" -f $SidecarRoot) }
if ($Apply) { Write-Host "  calisma modu : UYGULAMA" -ForegroundColor Yellow }
else        { Write-Host "  calisma modu : PLAN (hicbir dosya yazilmaz)" -ForegroundColor Green }
Write-Host ""

# --- Aday klasorler -------------------------------------------------------
$candidates = Get-ChildItem -LiteralPath $Root -Directory -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -notmatch '^[._]' }

if (-not $candidates) {
    Write-Host "Aday klasor bulunamadi." -ForegroundColor Yellow
    return
}

$infos = @()
foreach ($dir in $candidates) {
    Write-Host ("  taraniyor: {0}" -f $dir.Name)
    try {
        $infos += (Get-GcProjectInfo -ProjectPath $dir.FullName)
    } catch {
        Write-Host ("  atlandi  : {0} ({1})" -f $dir.Name, $_.Exception.Message) -ForegroundColor Yellow
    }
}

# Oncelikli projeler once
$ordered = @()
foreach ($name in $Priority) {
    $ordered += ($infos | Where-Object { $_.Name -ieq $name })
}
$ordered += ($infos | Where-Object { $Priority -notcontains $_.Name } | Sort-Object Name)

Write-Host ""
Write-Host "PLAN" -ForegroundColor Cyan
$ordered | ForEach-Object {
    [pscustomobject]@{
        Proje    = $_.Name
        Sinif    = $_.Classification
        Proje_Sy = $_.Projects.Count
        Test_Sy  = $_.TestProjects.Count
        Build    = if ($_.BuildCommands.Count -gt 0) { $_.BuildCommands[0] } else { '[tespit edilemedi]' }
        Mevcut   = (@(
                      $(if ($_.Existing.Context) { 'context/' }),
                      $(if ($_.Existing.Tasks)   { 'tasks/' }),
                      $(if ($_.Existing.Agents)  { 'AGENTS.md' }),
                      $(if ($_.Existing.Claude)  { 'CLAUDE.md' })
                    ) | Where-Object { $_ }) -join ' '
        Neden    = $_.Reason
    }
} | Format-Table -AutoSize | Out-String -Width 200 | Write-Host

$eligible = $ordered | Where-Object {
    $_.Classification -eq 'Uygun' -or ($IncludeMarginal -and $_.Classification -eq 'Incelenecek')
}

Write-Host ("Kurulacak proje sayisi: {0}" -f @($eligible).Count) -ForegroundColor Cyan
if (-not $Apply) {
    Write-Host ""
    Write-Host "Plan modu. Kurulum icin ayni komutu -Apply ile calistirin." -ForegroundColor Green
    Write-Host "'Incelenecek' sinifindakileri de kurmak icin -IncludeMarginal ekleyin." -ForegroundColor Green
    return
}

# --- Uygulama -------------------------------------------------------------
$results = @()
foreach ($info in $eligible) {
    $params = @{
        ProjectPath = $info.Path
        Mode        = $Mode
        PassThru    = $true
    }
    if ($Mode -eq 'Sidecar') { $params['SidecarRoot'] = $SidecarRoot }
    if ($AddPointer)   { $params['AddPointer']   = $true }
    if ($LinkPointer)  { $params['LinkPointer']  = $true }
    if ($WithLauncher) { $params['WithLauncher'] = $true }
    if ($NoDetect)     { $params['NoDetect']     = $true }

    try {
        $results += (& $initScript @params)
    } catch {
        Write-Host ("  HATA: {0} -> {1}" -f $info.Name, $_.Exception.Message) -ForegroundColor Red
    }
}

# --- Rapor ----------------------------------------------------------------
$nl = [System.Environment]::NewLine
$report = New-Object System.Collections.Generic.List[string]
$report.Add('# Graph Context Kurulum Raporu')
$report.Add('')
$report.Add(("Tarih: {0}" -f (Get-Date).ToString('yyyy-MM-dd HH:mm')))
$report.Add(("Kok klasor: ``{0}``" -f $Root))
$report.Add(("Kurulum modu: {0}" -f $Mode))
$report.Add('')

foreach ($r in $results) {
    $report.Add(("## {0}" -f $r.Project))
    $report.Add('')
    $report.Add(("- Proje klasoru: ``{0}``" -f $r.ProjectPath))
    $report.Add(("- Baglam klasoru: ``{0}``" -f $r.TargetRoot))
    $report.Add(("- Tespit: {0} - {1}" -f $r.Info.Classification, $r.Info.Reason))
    $report.Add('')
    $report.Add('**Eklenen dosyalar**')
    if ($r.Added.Count -gt 0) { foreach ($a in $r.Added) { $report.Add("- $a") } } else { $report.Add('- (yok)') }
    $report.Add('')
    $report.Add('**Korunan dosyalar**')
    if ($r.Preserved.Count -gt 0) { foreach ($p in $r.Preserved) { $report.Add("- $p") } } else { $report.Add('- (yok)') }
    $report.Add('')
    $report.Add('**Build / test komutlari**')
    if ($r.Info.BuildCommands.Count -gt 0) { foreach ($c in $r.Info.BuildCommands) { $report.Add("- Derleme: ``$c``") } }
    else { $report.Add('- Derleme: [tespit edilemedi]') }
    if ($r.Info.TestCommands.Count -gt 0) { foreach ($c in $r.Info.TestCommands) { $report.Add("- Test: ``$c``") } }
    else { $report.Add('- Test: [test projesi bulunamadi]') }
    $report.Add('')
    if ($r.Notes.Count -gt 0) {
        $report.Add('**Talimat dosyasi entegrasyonu / notlar**')
        foreach ($n in $r.Notes) { $report.Add("- $n") }
        $report.Add('')
    }
    if ($r.Manual.Count -gt 0) {
        $report.Add('**Elle doldurulacak alanlar**')
        foreach ($m in $r.Manual) { $report.Add("- $m") }
        $report.Add('')
    }
}

$reportText = ($report -join $nl)
Write-Host ""
Write-Host $reportText

if ($ReportPath) {
    $reportDir = Split-Path -Parent $ReportPath
    if ($reportDir -and -not (Test-Path -LiteralPath $reportDir)) {
        New-Item -ItemType Directory -Path $reportDir -Force | Out-Null
    }
    Set-Content -LiteralPath $ReportPath -Value $reportText -Encoding UTF8
    Write-Host ""
    Write-Host ("Rapor yazildi: {0}" -f $ReportPath) -ForegroundColor Green
}
