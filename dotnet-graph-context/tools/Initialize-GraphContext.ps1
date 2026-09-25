<#
.SYNOPSIS
    Bir .NET projesine Graph Context baglam sistemini guvenli sekilde kurar.

.DESCRIPTION
    Iki kurulum modu vardir:

      Sidecar  (varsayilan) : context/ ve tasks/ proje klasorunun DISINDA, ayri bir
                              klasorde olusturulur. Proje deposunda hicbir dosya
                              degismez. Olgun veya kritik projeler icin onerilir.

      InPlace               : context/ ve tasks/ proje kokune eklenir. Var olan
                              hicbir dosya ezilmez; AGENTS.md / CLAUDE.md varsa
                              yanina *.graph-context.md eki birakilir, -AddPointer
                              verilirse dosyanin SONUNA isaretli bir router bolumu
                              eklenir (mevcut satirlar degistirilmez).

    Her iki modda da var olan dosyalar korunur; betik hicbir dosyanin uzerine yazmaz.
    -WhatIf ile once ne yapilacagini gorebilirsiniz.

.PARAMETER ProjectPath
    Hedef .NET proje klasoru.

.PARAMETER Mode
    Sidecar | InPlace. Varsayilan: Sidecar.

.PARAMETER SidecarRoot
    Sidecar modunda baglam klasorlerinin toplandigi kok. Varsayilan:
    <projenin ust klasoru>\_graph-context. Hedef: <SidecarRoot>\<ProjeAdi>.

.PARAMETER AddPointer
    InPlace modunda, var olan AGENTS.md / CLAUDE.md dosyalarinin sonuna isaretli
    router bolumu ekler. Ikinci calistirmada tekrar eklemez.

.PARAMETER LinkPointer
    Sidecar modunda, proje kokune izlenmeyen (untracked) tek bir .graph-context.md
    isaret dosyasi birakir ve .git/info/exclude icine ekler. Depoya hicbir sey
    commit edilmez.

.PARAMETER WithLauncher
    Sidecar klasorune baslat-claude.cmd / baslat-codex.cmd kisayollarini ekler.

.PARAMETER NoDetect
    context/testing.md dosyasini tespit edilen gercek komutlarla degil, bos sablonla
    olusturur.

.EXAMPLE
    .\Initialize-GraphContext.ps1 -ProjectPath "D:\C#\PackErp" -WhatIf

.EXAMPLE
    .\Initialize-GraphContext.ps1 -ProjectPath "D:\C#\PackErp" -WithLauncher

.EXAMPLE
    .\Initialize-GraphContext.ps1 -ProjectPath "D:\C#\ArdCRM" -Mode InPlace -AddPointer
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory)]
    [ValidateScript({ Test-Path $_ -PathType Container })]
    [string]$ProjectPath,

    [ValidateSet('Sidecar', 'InPlace')]
    [string]$Mode = 'Sidecar',

    [string]$SidecarRoot,

    [switch]$AddPointer,
    [switch]$LinkPointer,
    [switch]$WithLauncher,
    [switch]$NoDetect,
    [switch]$PassThru
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'GraphContext.Detection.ps1')

$repoRoot     = Split-Path -Parent $PSScriptRoot
$templateRoot = Join-Path $repoRoot 'template'
if (-not (Test-Path -LiteralPath $templateRoot -PathType Container)) {
    throw "Sablon klasoru bulunamadi: $templateRoot"
}

$projectRoot = (Resolve-Path -LiteralPath $ProjectPath).Path
$projectName = Split-Path -Leaf $projectRoot
$info = Get-GcProjectInfo -ProjectPath $projectRoot

if ($Mode -eq 'Sidecar') {
    if (-not $SidecarRoot) {
        $SidecarRoot = Join-Path (Split-Path -Parent $projectRoot) '_graph-context'
    }
    $targetRoot = Join-Path $SidecarRoot $projectName
} else {
    $targetRoot = $projectRoot
}
$targetRoot = [System.IO.Path]::GetFullPath($targetRoot)

$added     = New-Object System.Collections.Generic.List[string]
$preserved = New-Object System.Collections.Generic.List[string]
$notes     = New-Object System.Collections.Generic.List[string]

function Write-GcFile {
    param(
        [Parameter(Mandatory)][string]$Destination,
        [Parameter(Mandatory)][string]$Content,
        [Parameter(Mandatory)][string]$Label
    )
    if (Test-Path -LiteralPath $Destination) {
        $preserved.Add($Label)
        Write-Host ("  korundu : {0}" -f $Label) -ForegroundColor Yellow
        return
    }
    if ($PSCmdlet.ShouldProcess($Destination, 'Baglam dosyasi olustur')) {
        $parent = Split-Path -Parent $Destination
        if (-not (Test-Path -LiteralPath $parent)) { New-Item -ItemType Directory -Path $parent -Force | Out-Null }
        Set-Content -LiteralPath $Destination -Value $Content -Encoding UTF8
        $added.Add($Label)
        Write-Host ("  eklendi : {0}" -f $Label) -ForegroundColor Green
    }
}

function Copy-GcTemplateFile {
    param(
        [Parameter(Mandatory)][string]$RelativeSource,
        [string]$RelativeDestination
    )
    if (-not $RelativeDestination) { $RelativeDestination = $RelativeSource }
    $source = Join-Path $templateRoot $RelativeSource
    if (-not (Test-Path -LiteralPath $source)) { throw "Sablon dosyasi yok: $source" }
    $content = Get-Content -LiteralPath $source -Raw
    Write-GcFile -Destination (Join-Path $targetRoot $RelativeDestination) -Content $content -Label $RelativeDestination
}

function Add-GcBlock {
    param(
        [Parameter(Mandatory)][string]$File,
        [Parameter(Mandatory)][string]$Block,
        [Parameter(Mandatory)][string]$Label
    )
    if (-not (Test-Path -LiteralPath $File)) { return }
    $existing = [string](Get-Content -LiteralPath $File -Raw -ErrorAction SilentlyContinue)
    if ($existing -like '*graph-context:start*') {
        $notes.Add("$Label : router bolumu zaten var, tekrar eklenmedi")
        Write-Host ("  atlandi : {0} (router bolumu zaten var)" -f $Label) -ForegroundColor Yellow
        return
    }
    if ($PSCmdlet.ShouldProcess($File, 'Router bolumu ekle (dosya sonuna)')) {
        $nl = [System.Environment]::NewLine
        Add-Content -LiteralPath $File -Value ($nl + $Block + $nl) -Encoding UTF8
        $notes.Add("$Label : dosyanin sonuna router bolumu eklendi (mevcut satirlar degismedi)")
        Write-Host ("  eklendi : {0} -> router bolumu" -f $Label) -ForegroundColor Green
    }
}

Write-Host ""
Write-Host ("=== {0} ({1}) ===" -f $projectName, $Mode) -ForegroundColor Cyan
Write-Host ("  kaynak  : {0}" -f $projectRoot)
Write-Host ("  hedef   : {0}" -f $targetRoot)
Write-Host ("  tespit  : {0} - {1}" -f $info.Classification, $info.Reason)

# --- context/ + tasks/ ---------------------------------------------------
$contextFiles = @(
    'context/README.md',
    'context/proje-ozeti.md',
    'context/kurallar-ve-sinirlar.md',
    'context/kararlar.md',
    'context/hedefler.md',
    'context/domains/README.md',
    'context/kaynaklar/README.md',
    'context/kaynaklar/kaynak-indeksi.md',
    'tasks/todo.md',
    'tasks/lessons.md'
)
foreach ($file in $contextFiles) { Copy-GcTemplateFile -RelativeSource $file }

# --- testing.md : gercek komutlarla -------------------------------------
$testingDestination = Join-Path $targetRoot 'context/testing.md'
if ($NoDetect) {
    Copy-GcTemplateFile -RelativeSource 'context/testing.md'
} else {
    Write-GcFile -Destination $testingDestination -Content (Get-GcTestingMarkdown -Info $info) -Label 'context/testing.md (tespit edilen komutlar)'
}

# --- Router dosyalari -----------------------------------------------------
if ($Mode -eq 'InPlace') {
    if (Test-Path -LiteralPath (Join-Path $projectRoot 'AGENTS.md')) {
        Copy-GcTemplateFile -RelativeSource 'AGENTS.graph-context.md'
        if ($AddPointer) { Add-GcBlock -File (Join-Path $projectRoot 'AGENTS.md') -Block (Get-GcPointerBlock) -Label 'AGENTS.md' }
        else { $notes.Add('AGENTS.md korundu; AGENTS.graph-context.md ekini inceleyin veya -AddPointer kullanin') }
    } else {
        Copy-GcTemplateFile -RelativeSource 'AGENTS.md'
    }

    if (Test-Path -LiteralPath (Join-Path $projectRoot 'CLAUDE.md')) {
        Copy-GcTemplateFile -RelativeSource 'CLAUDE.graph-context.md'
        if ($AddPointer) { Add-GcBlock -File (Join-Path $projectRoot 'CLAUDE.md') -Block (Get-GcPointerBlock) -Label 'CLAUDE.md' }
        else { $notes.Add('CLAUDE.md korundu; CLAUDE.graph-context.md ekini inceleyin veya -AddPointer kullanin') }
    } else {
        Copy-GcTemplateFile -RelativeSource 'CLAUDE.template.md' -RelativeDestination 'CLAUDE.md'
    }
}
else {
    Copy-GcTemplateFile -RelativeSource 'AGENTS.md'
    Copy-GcTemplateFile -RelativeSource 'CLAUDE.template.md' -RelativeDestination 'CLAUDE.md'

    $nl = [System.Environment]::NewLine
    $routerPath  = Join-Path (Join-Path $targetRoot 'context') 'README.md'
    $agentsPath  = Join-Path $targetRoot 'AGENTS.md'
    $contextPath = Join-Path $targetRoot 'context'
    $tasksPath   = Join-Path $targetRoot 'tasks'
    $baglanti = @(
        "# Baglanti - $projectName",
        '',
        'Bu klasor **sidecar** baglam klasorudur. Proje deposunda hicbir dosya degismedi.',
        '',
        '| Alan | Deger |',
        '|---|---|',
        "| Proje klasoru | ``$projectRoot`` |",
        "| Baglam klasoru | ``$targetRoot`` |",
        "| Tespit | $($info.Classification) - $($info.Reason) |",
        '',
        '## Claude Code ile kullanim',
        '',
        '```powershell',
        "cd `"$projectRoot`"",
        "claude --add-dir `"$targetRoot`"",
        '```',
        '',
        'Ilk mesaj olarak sunu verin:',
        '',
        '```text',
        "Once $routerPath router'ini oku, gorevi siniflandir,",
        'yalnizca ilgili baglam dosyalarini yukle. Gorev: ...',
        '```',
        '',
        '## Codex ile kullanim',
        '',
        'Codex `AGENTS.md` dosyasini calisma dizininde arar. Sidecar modda proje koku',
        'degismedigi icin router yolunu ilk mesajda verin:',
        '',
        '```text',
        "Once $agentsPath ve $routerPath dosyalarini oku.",
        'Gorev: ...',
        '```',
        '',
        'Alternatif: `Initialize-GraphContext.ps1 -LinkPointer` ile proje kokune',
        'izlenmeyen (untracked, .git/info/exclude icinde) tek satirlik bir isaret',
        'dosyasi birakabilirsiniz; depoya yine hicbir sey commit edilmez.',
        '',
        '## Iceri tasima (sonradan)',
        '',
        'Sistem isinize yaradiginda baglam klasorunu proje icine tasiyabilirsiniz:',
        '',
        '```powershell',
        ("Copy-Item `"{0}`" `"{1}`" -Recurse" -f $contextPath, (Join-Path $projectRoot 'context')),
        ("Copy-Item `"{0}`" `"{1}`" -Recurse" -f $tasksPath, (Join-Path $projectRoot 'tasks')),
        '```',
        '',
        'Ardindan proje kokundeki AGENTS.md / CLAUDE.md dosyalarina router bolumunu',
        '`-Mode InPlace -AddPointer` ile ekleyin.'
    ) -join $nl

    Write-GcFile -Destination (Join-Path $targetRoot 'baglanti.md') -Content $baglanti -Label 'baglanti.md'

    $projectBlock = @(
        '',
        '## Proje konumu (sidecar)',
        '',
        "Kod bu klasorde degildir. Uygulama kodu: ``$projectRoot``",
        "Degisiklikler orada yapilir; baglam kayitlari bu klasorde tutulur.",
        "Detay: ``baglanti.md``"
    ) -join $nl

    foreach ($routerFile in @('AGENTS.md', 'CLAUDE.md')) {
        $full = Join-Path $targetRoot $routerFile
        if ((Test-Path -LiteralPath $full) -and ($added -contains $routerFile)) {
            if ($PSCmdlet.ShouldProcess($full, 'Proje konumu bolumu ekle')) {
                Add-Content -LiteralPath $full -Value $projectBlock -Encoding UTF8
            }
        }
    }

    if ($WithLauncher) {
        $claudeCmd = @(
            '@echo off',
            "cd /d `"$projectRoot`"",
            'echo Ilk mesaj icin oneri:',
            "echo   Once $routerPath router'ini oku, gorevi siniflandir.",
            "claude --add-dir `"$targetRoot`""
        ) -join "`r`n"
        Write-GcFile -Destination (Join-Path $targetRoot 'baslat-claude.cmd') -Content $claudeCmd -Label 'baslat-claude.cmd'

        $codexCmd = @(
            '@echo off',
            "cd /d `"$projectRoot`"",
            'echo Ilk mesaj icin oneri:',
            "echo   Once $agentsPath ve $routerPath dosyalarini oku.",
            'codex'
        ) -join "`r`n"
        Write-GcFile -Destination (Join-Path $targetRoot 'baslat-codex.cmd') -Content $codexCmd -Label 'baslat-codex.cmd'
    }

    if ($LinkPointer) {
        $pointerFile = Join-Path $projectRoot '.graph-context.md'
        $pointerContent = @(
            '# Graph Context isaret dosyasi',
            '',
            "Bu projenin baglam kayitlari proje disinda tutulur: ``$targetRoot``",
            '',
            ("Her gorevde once ``{0}`` router'ini oku," -f $routerPath),
            'gorevi siniflandir ve yalnizca ilgili baglam dosyalarini yukle.',
            '',
            'Bu dosya izlenmeyen (untracked) bir isaret dosyasidir; depoya commit edilmez.'
        ) -join $nl
        Write-GcFile -Destination $pointerFile -Content $pointerContent -Label '.graph-context.md (proje kokunde, untracked)'

        $excludeFile = Join-Path $projectRoot '.git/info/exclude'
        if (Test-Path -LiteralPath $excludeFile) {
            # Bos dosyada Get-Content -Raw $null doner; $null -notmatch False verir.
            $excludeContent = [string](Get-Content -LiteralPath $excludeFile -Raw -ErrorAction SilentlyContinue)
            if ($excludeContent -notmatch '(?m)^\.graph-context\.md\s*$') {
                if ($PSCmdlet.ShouldProcess($excludeFile, 'Isaret dosyasini git exclude listesine ekle')) {
                    Add-Content -LiteralPath $excludeFile -Value ".graph-context.md"
                    $notes.Add('.git/info/exclude : .graph-context.md eklendi (yerel, commit edilmez)')
                }
            }
        } else {
            $notes.Add('Git deposu bulunamadi; .graph-context.md dosyasini kendiniz gitignore edin')
        }
    }
}

# --- Elle doldurulacak alanlar -------------------------------------------
$manual = New-Object System.Collections.Generic.List[string]
if (-not $WhatIfPreference) {
    $scanRoots = @((Join-Path $targetRoot 'context'), (Join-Path $targetRoot 'tasks'))
    foreach ($scanRoot in $scanRoots) {
        if (-not (Test-Path -LiteralPath $scanRoot)) { continue }
        foreach ($mdFile in (Get-ChildItem -LiteralPath $scanRoot -Recurse -File -Filter '*.md' -ErrorAction SilentlyContinue)) {
            $text = [string](Get-Content -LiteralPath $mdFile.FullName -Raw -ErrorAction SilentlyContinue)
            if ($text -match '\[[^\]]+\]' -and $text -notmatch '^\s*$') {
                $placeholders = [regex]::Matches($text, '\[[a-zA-Z0-9çğıöşüÇĞİÖŞÜ ,\./-]{3,60}\]') |
                    Where-Object { $_.Value -notmatch '^\[(x| )\]$' } |
                    Select-Object -ExpandProperty Value -Unique
                if ($placeholders) {
                    $rel = Get-GcRelativePath -Root $targetRoot -FullName $mdFile.FullName
                    $manual.Add(("{0} -> {1}" -f $rel, (($placeholders | Select-Object -First 4) -join ', ')))
                }
            }
        }
    }
}

Write-Host ""
Write-Host ("  ozet    : {0} eklendi, {1} korundu" -f $added.Count, $preserved.Count) -ForegroundColor Cyan

$result = [pscustomobject]@{
    Project    = $projectName
    ProjectPath= $projectRoot
    Mode       = $Mode
    TargetRoot = $targetRoot
    Added      = @($added)
    Preserved  = @($preserved)
    Notes      = @($notes)
    Manual     = @($manual)
    Info       = $info
}

if ($PassThru) { return $result }
