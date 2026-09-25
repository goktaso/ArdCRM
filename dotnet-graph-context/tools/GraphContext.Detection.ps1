<#
.SYNOPSIS
    Graph Context tespit yardimcilari.
.DESCRIPTION
    Bir .NET proje klasorunu inceleyip gercek build/test komutlarini, test
    yiginini ve kurulum uygunlugunu tespit eder. Komut uretmez, yalnizca
    diskte bulunan dosyalardan cikarim yapar. Tespit edilemeyen alan
    "[tespit edilemedi]" olarak isaretlenir.

    Bu dosya dot-source edilerek kullanilir:
        . "$PSScriptRoot\GraphContext.Detection.ps1"
#>

$script:GcExcludeRx = '[\\/](bin|obj|node_modules|\.git|\.vs|\.idea|packages|TestResults|artifacts|publish)[\\/]'
$script:GcEducationRx = 'ogrenci|egitim|ders|odev|tutorial|sample|ornek|demo|deneme|hello|kurs|lab|test-?proje'
$script:GcPointerStart = '<!-- graph-context:start -->'
$script:GcPointerEnd = '<!-- graph-context:end -->'

function Get-GcRelativePath {
    param(
        [Parameter(Mandatory)][string]$Root,
        [Parameter(Mandatory)][string]$FullName
    )
    $normalizedRoot = $Root.TrimEnd('\', '/')
    if ($FullName.StartsWith($normalizedRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $FullName.Substring($normalizedRoot.Length).TrimStart('\', '/')
    }
    return $FullName
}

function Get-GcFiles {
    param(
        [Parameter(Mandatory)][string]$Root,
        [Parameter(Mandatory)][string[]]$Include
    )
    $result = @()
    foreach ($pattern in $Include) {
        $found = Get-ChildItem -LiteralPath $Root -Recurse -File -Filter $pattern -Force -ErrorAction SilentlyContinue
        if ($found) { $result += $found }
    }
    return @($result | Where-Object { $_.FullName -notmatch $script:GcExcludeRx })
}

function Test-GcIsTestProject {
    param([Parameter(Mandatory)][string]$Content)
    if ($Content -match '<IsTestProject>\s*true\s*</IsTestProject>') { return $true }
    if ($Content -match 'PackageReference\s+Include="(xunit|NUnit|MSTest|Microsoft\.NET\.Test\.Sdk)') { return $true }
    return $false
}

function Get-GcProjectInfo {
<#
.SYNOPSIS
    Tek bir proje klasorunu inceler ve tespit sonucunu dondurur.
#>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)][string]$ProjectPath
    )

    if (-not (Test-Path -LiteralPath $ProjectPath -PathType Container)) {
        throw "Klasor bulunamadi: $ProjectPath"
    }

    $root = (Resolve-Path -LiteralPath $ProjectPath).Path
    $name = Split-Path -Leaf $root

    $solutions   = Get-GcFiles -Root $root -Include @('*.sln', '*.slnx')
    $projects    = Get-GcFiles -Root $root -Include @('*.csproj', '*.fsproj', '*.vbproj')
    $codeFiles   = Get-GcFiles -Root $root -Include @('*.cs', '*.fs', '*.vb')

    $testProjects = @()
    $frameworks   = @()
    $projectKinds = @()

    foreach ($proj in $projects) {
        $content = ''
        try { $content = [string](Get-Content -LiteralPath $proj.FullName -Raw -ErrorAction Stop) } catch { $content = '' }
        if (-not $content) { $content = '' }
        if ($content -and (Test-GcIsTestProject -Content $content)) { $testProjects += $proj }

        foreach ($match in [regex]::Matches($content, '<TargetFrameworks?>([^<]+)<')) {
            foreach ($tfm in ($match.Groups[1].Value -split ';')) {
                $tfm = $tfm.Trim()
                if ($tfm -and ($frameworks -notcontains $tfm)) { $frameworks += $tfm }
            }
        }

        if ($content -match 'Sdk="Microsoft\.NET\.Sdk\.Web"')     { $projectKinds += 'ASP.NET Core' }
        if ($content -match 'Sdk="Microsoft\.NET\.Sdk\.Razor"')   { $projectKinds += 'Razor Class Library' }
        if ($content -match 'Sdk="Microsoft\.NET\.Sdk\.Worker"')  { $projectKinds += 'Worker Service' }
        if ($content -match '<UseWindowsForms>\s*true')           { $projectKinds += 'Windows Forms' }
        if ($content -match '<UseWPF>\s*true')                    { $projectKinds += 'WPF' }
        if ($content -match '<UseMaui>\s*true')                   { $projectKinds += 'MAUI' }
        if ($content -match '<OutputType>\s*Exe')                 { $projectKinds += 'Console/Exe' }
    }
    $projectKinds = @($projectKinds | Select-Object -Unique)

    # --- Build komutlari -------------------------------------------------
    $buildCommands = @()
    $buildNote = ''
    if ($solutions.Count -ge 1) {
        foreach ($sln in ($solutions | Sort-Object FullName | Select-Object -First 3)) {
            $rel = Get-GcRelativePath -Root $root -FullName $sln.FullName
            $buildCommands += ('dotnet build "{0}"' -f $rel)
        }
        if ($solutions.Count -gt 3) { $buildNote = "$($solutions.Count) cozum dosyasi bulundu; ilk 3 listelendi." }
    }
    elseif ($projects.Count -eq 1) {
        $rel = Get-GcRelativePath -Root $root -FullName $projects[0].FullName
        $buildCommands += ('dotnet build "{0}"' -f $rel)
    }
    elseif ($projects.Count -gt 1) {
        $buildNote = 'Cozum dosyasi yok, birden fazla proje var; derleme hedefi elle secilmeli.'
    }

    # --- Test komutlari --------------------------------------------------
    $testCommands = @()
    foreach ($testProj in ($testProjects | Sort-Object FullName)) {
        $rel = Get-GcRelativePath -Root $root -FullName $testProj.FullName
        $testCommands += ('dotnet test "{0}"' -f $rel)
    }

    # --- Node/npm ---------------------------------------------------------
    $npmCommands = @()
    $packageJsonFiles = @(Get-GcFiles -Root $root -Include @('package.json'))
    foreach ($pkg in ($packageJsonFiles | Select-Object -First 3)) {
        try {
            $json = Get-Content -LiteralPath $pkg.FullName -Raw -ErrorAction Stop | ConvertFrom-Json
        } catch { continue }
        if (-not $json.PSObject.Properties.Name.Contains('scripts')) { continue }
        $rel = Get-GcRelativePath -Root $root -FullName $pkg.Directory.FullName
        foreach ($scriptName in @('build', 'test', 'lint')) {
            if ($json.scripts.PSObject.Properties.Name -contains $scriptName) {
                $prefix = ''
                if ($rel) { $prefix = "($rel) " }
                $npmCommands += ('{0}npm run {1}' -f $prefix, $scriptName)
            }
        }
    }

    # --- Lint / analiz ----------------------------------------------------
    $lintNotes = @()
    if (Test-Path -LiteralPath (Join-Path $root '.editorconfig')) { $lintNotes += '.editorconfig mevcut' }
    foreach ($propsName in @('Directory.Build.props', 'Directory.Packages.props')) {
        if (Test-Path -LiteralPath (Join-Path $root $propsName)) { $lintNotes += "$propsName mevcut" }
    }
    $analyzerHit = $projects | Where-Object {
        $c = ''
        try { $c = [string](Get-Content -LiteralPath $_.FullName -Raw -ErrorAction Stop) } catch { $c = '' }
        $c -match 'StyleCop\.Analyzers|SonarAnalyzer|Roslynator|Microsoft\.CodeAnalysis\.NetAnalyzers|TreatWarningsAsErrors'
    }
    if ($analyzerHit) { $lintNotes += 'Analyzer/TreatWarningsAsErrors yapilandirmasi bulundu' }

    # --- CI ---------------------------------------------------------------
    $ciFiles = @()
    $workflowDir = Join-Path $root '.github/workflows'
    if (Test-Path -LiteralPath $workflowDir) {
        $ciFiles += @(Get-ChildItem -LiteralPath $workflowDir -File -ErrorAction SilentlyContinue |
            ForEach-Object { Get-GcRelativePath -Root $root -FullName $_.FullName })
    }
    foreach ($ciName in @('azure-pipelines.yml', '.gitlab-ci.yml')) {
        if (Test-Path -LiteralPath (Join-Path $root $ciName)) { $ciFiles += $ciName }
    }

    # --- Mevcut yapi ------------------------------------------------------
    $existing = [ordered]@{
        Context = Test-Path -LiteralPath (Join-Path $root 'context')
        Tasks   = Test-Path -LiteralPath (Join-Path $root 'tasks')
        Agents  = Test-Path -LiteralPath (Join-Path $root 'AGENTS.md')
        Claude  = Test-Path -LiteralPath (Join-Path $root 'CLAUDE.md')
        Git     = Test-Path -LiteralPath (Join-Path $root '.git')
    }

    # --- Siniflandirma ----------------------------------------------------
    $classification = 'Uygun'
    $reason = 'Cok projeli .NET cozumu'

    if ($projects.Count -eq 0) {
        $classification = 'Atlandi'
        $reason = '.NET proje dosyasi bulunamadi'
    }
    elseif ($projects.Count -le 1 -and $codeFiles.Count -lt 30) {
        $classification = 'Atlandi'
        $reason = "Tek proje, $($codeFiles.Count) kod dosyasi - kucuk olcek"
    }
    elseif ($name -match $script:GcEducationRx) {
        $classification = 'Incelenecek'
        $reason = 'Isim kalibi egitim/ornek projeye benziyor'
    }
    elseif ($projects.Count -eq 1) {
        $classification = 'Incelenecek'
        $reason = "Tek katmanli proje ($($codeFiles.Count) kod dosyasi)"
    }
    elseif ($projects.Count -eq 2 -and $testProjects.Count -eq 0) {
        $classification = 'Incelenecek'
        $reason = '2 proje, test projesi yok'
    }
    else {
        $reason = "$($projects.Count) proje, $($testProjects.Count) test projesi, $($codeFiles.Count) kod dosyasi"
    }

    return [pscustomobject]@{
        Name            = $name
        Path            = $root
        Solutions       = @($solutions | ForEach-Object { Get-GcRelativePath -Root $root -FullName $_.FullName })
        Projects        = @($projects  | ForEach-Object { Get-GcRelativePath -Root $root -FullName $_.FullName })
        TestProjects    = @($testProjects | ForEach-Object { Get-GcRelativePath -Root $root -FullName $_.FullName })
        TargetFrameworks= @($frameworks)
        ProjectKinds    = @($projectKinds)
        CodeFileCount   = $codeFiles.Count
        BuildCommands   = @($buildCommands)
        BuildNote       = $buildNote
        TestCommands    = @($testCommands)
        NpmCommands     = @($npmCommands)
        LintNotes       = @($lintNotes)
        CiFiles         = @($ciFiles)
        Existing        = $existing
        Classification  = $classification
        Reason          = $reason
    }
}

function Get-GcTestingMarkdown {
<#
.SYNOPSIS
    Tespit sonucundan context/testing.md icerigi uretir.
#>
    [CmdletBinding()]
    param([Parameter(Mandatory)][psobject]$Info)

    $today = (Get-Date).ToString('yyyy-MM-dd')
    $lines = New-Object System.Collections.Generic.List[string]

    $lines.Add("# Dogrulama ve Kabul Kriterleri - $($Info.Name)")
    $lines.Add('')
    $lines.Add("Bu dosya $today tarihinde proje dosyalarindan otomatik tespit edilmistir.")
    $lines.Add('Komutlar uydurulmaz; tespit edilemeyen alan "[tespit edilemedi]" olarak isaretlenir.')
    $lines.Add('Komutlar proje kok dizininde calistirilir.')
    $lines.Add('')
    $lines.Add('## Standart kontroller')
    $lines.Add('')
    $lines.Add('| Amac | Komut |')
    $lines.Add('|---|---|')

    if ($Info.BuildCommands.Count -gt 0) {
        foreach ($cmd in $Info.BuildCommands) { $lines.Add("| Derleme | ``$cmd`` |") }
    } else {
        $lines.Add('| Derleme | [tespit edilemedi - elle doldurun] |')
    }

    if ($Info.TestCommands.Count -gt 0) {
        foreach ($cmd in $Info.TestCommands) { $lines.Add("| Test | ``$cmd`` |") }
    } else {
        $lines.Add('| Test | [test projesi bulunamadi - elle doldurun] |')
    }

    foreach ($cmd in $Info.NpmCommands) { $lines.Add("| Frontend | ``$cmd`` |") }
    $lines.Add('')

    if ($Info.BuildNote) {
        $lines.Add("> Not: $($Info.BuildNote)")
        $lines.Add('')
    }

    $lines.Add('## Tespit edilen yapi')
    $lines.Add('')
    if ($Info.Solutions.Count -gt 0)        { $lines.Add("- Cozum dosyasi: $($Info.Solutions -join ', ')") }
    else                                    { $lines.Add('- Cozum dosyasi: yok') }
    $lines.Add("- Proje sayisi: $($Info.Projects.Count)")
    if ($Info.TestProjects.Count -gt 0)     { $lines.Add("- Test projeleri: $($Info.TestProjects -join ', ')") }
    else                                    { $lines.Add('- Test projesi: bulunamadi') }
    if ($Info.TargetFrameworks.Count -gt 0) { $lines.Add("- Hedef framework: $($Info.TargetFrameworks -join ', ')") }
    if ($Info.ProjectKinds.Count -gt 0)     { $lines.Add("- Proje tipleri: $($Info.ProjectKinds -join ', ')") }
    if ($Info.LintNotes.Count -gt 0)        { $lines.Add("- Statik analiz: $($Info.LintNotes -join '; ')") }
    else                                    { $lines.Add('- Statik analiz: yapilandirilmis lint/analyzer kurali bulunamadi') }
    if ($Info.CiFiles.Count -gt 0)          { $lines.Add("- CI tanimi: $($Info.CiFiles -join ', ')") }
    else                                    { $lines.Add('- CI tanimi: bulunamadi') }
    $lines.Add('')
    $lines.Add('## Goreve ozel kabul kriterleri')
    $lines.Add('')
    $lines.Add('- [Beklenen davranis, kanit ve ilgili test]')
    $lines.Add('')
    $lines.Add('## Hata geri donus kurali')
    $lines.Add('')
    $lines.Add('Bir test basarisiz olursa once hata mesaji ile degisen alanin baglantisini kurun;')
    $lines.Add('ardindan yalnizca ilgili domain, sozlesme veya mimari karar dosyasina donun.')
    $lines.Add('Iki denemede cozulmeyen hatayi `tasks/todo.md` CHECKPOINT bolumune engel olarak yazin.')
    $lines.Add('')

    return ($lines -join [System.Environment]::NewLine)
}

function Get-GcPointerBlock {
<#
.SYNOPSIS
    Mevcut AGENTS.md / CLAUDE.md sonuna eklenecek isaretli router bolumunu uretir.
#>
    [CmdletBinding()]
    param([string]$ContextRelativePath = 'context')

    $lines = @(
        $script:GcPointerStart,
        '## Graph Context Router (baglam secimi)',
        '',
        '> Bu bolum baglam sistemini bu dosyaya baglar. Yukaridaki bolumler degismedi;',
        '> catisma durumunda yukaridaki proje kurallari baglayicidir.',
        '',
        '### Her gorevde',
        '',
        ("1. ``$ContextRelativePath/README.md`` dosyasini oku - baglam haritasi / router."),
        ("2. Haritada **Her gorevde oku** olarak isaretli dosyalari oku: ``$ContextRelativePath/proje-ozeti.md``, ``$ContextRelativePath/kurallar-ve-sinirlar.md``."),
        '3. Gorevi siniflandir: feature, bugfix, refactor, review, research, planning, documentation.',
        ("4. Yalnizca gorevle ilgili ``$ContextRelativePath/domains/*.md``, ``$ContextRelativePath/kararlar.md``, ``$ContextRelativePath/testing.md`` dosyalarini yukle. Tum dizini okuma."),
        '5. Devam eden is icin `tasks/todo.md` CHECKPOINT bolumunu oku.',
        '',
        '### Akis',
        '',
        '```text',
        'Siniflandir -> ilgili baglami sec -> incele/planla -> uygula',
        '-> build/test dogrulamasi -> sonuc',
        '                        ^          |',
        '                        +-- hata --+',
        '```',
        '',
        'Hata durumunda tum projeyi yeniden yorumlama; yalnizca hatanin iliskili oldugu',
        'domain, karar veya sozlesme dosyasina don.',
        '',
        ("Gercek build/test komutlari: ``$ContextRelativePath/testing.md``"),
        $script:GcPointerEnd
    )

    return ($lines -join [System.Environment]::NewLine)
}
