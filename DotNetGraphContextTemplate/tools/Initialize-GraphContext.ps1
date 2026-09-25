[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory)]
    [ValidateScript({ Test-Path $_ -PathType Container })]
    [string]$ProjectPath
)

$ErrorActionPreference = 'Stop'
$templateRoot = Split-Path -Parent $PSScriptRoot
$projectRoot = (Resolve-Path -LiteralPath $ProjectPath).Path

function Copy-IfMissing {
    param([string]$RelativePath)

    $source = Join-Path $templateRoot $RelativePath
    $destination = Join-Path $projectRoot $RelativePath
    if (Test-Path -LiteralPath $destination) {
        Write-Host "Atlandi (zaten var): $RelativePath" -ForegroundColor Yellow
        return
    }

    $parent = Split-Path -Parent $destination
    if ($PSCmdlet.ShouldProcess($destination, 'Baglam sablon dosyasi olustur')) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
        Copy-Item -LiteralPath $source -Destination $destination
        Write-Host "Eklendi: $RelativePath" -ForegroundColor Green
    }
}

$contextFiles = @(
    'context/README.md',
    'context/proje-ozeti.md',
    'context/kurallar-ve-sinirlar.md',
    'context/kararlar.md',
    'context/hedefler.md',
    'context/testing.md',
    'context/domains/README.md',
    'context/kaynaklar/README.md',
    'context/kaynaklar/kaynak-indeksi.md',
    'tasks/todo.md',
    'tasks/lessons.md'
)

foreach ($file in $contextFiles) { Copy-IfMissing -RelativePath $file }

$agentsPath = Join-Path $projectRoot 'AGENTS.md'
if (Test-Path -LiteralPath $agentsPath) {
    Copy-IfMissing -RelativePath 'AGENTS.graph-context.md'
    Write-Host 'Mevcut AGENTS.md korunuyor. AGENTS.graph-context.md dosyasini inceleyip uygun kurallari ana talimata ekleyin.' -ForegroundColor Cyan
}
else {
    Copy-IfMissing -RelativePath 'AGENTS.md'
}

Write-Host "`nTamamlandi. Sonraki adim: $projectRoot\context\proje-ozeti.md dosyasini doldurun." -ForegroundColor Green
