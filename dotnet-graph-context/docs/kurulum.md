# Kurulum ve Parametreler

## Gereksinim

Windows PowerShell 5.1 veya PowerShell 7+. Betikler yalnizca dosya sistemi
okur/yazar; ag erisimi yapmaz, paket kurmaz, `dotnet` calistirmaz.

Sistemi bir kez makineye klonlayin:

```powershell
git clone https://github.com/goktaso/dotnet-graph-context.git D:\C#\_graph-context-system
cd D:\C#\_graph-context-system
```

## 1) Once plan

```powershell
.\tools\Install-GraphContextAll.ps1 -Root "D:\C#"
```

Cikti her klasor icin sinif, proje/test sayisi, tespit edilen build komutu ve
zaten var olan `context/`, `tasks/`, `AGENTS.md`, `CLAUDE.md` dosyalarini gosterir.
**Bu modda hicbir dosya yazilmaz.**

## 2) Kurulum

```powershell
.\tools\Install-GraphContextAll.ps1 -Root "D:\C#" -Apply -WithLauncher `
    -ReportPath "D:\C#\_graph-context\kurulum-raporu.md"
```

## Install-GraphContextAll.ps1 parametreleri

| Parametre | Varsayilan | Aciklama |
|---|---|---|
| `-Root` | `D:\C#` | Taranacak kok klasor |
| `-Mode` | `Sidecar` | `Sidecar` veya `InPlace` |
| `-SidecarRoot` | `<Root>\_graph-context` | Sidecar klasorlerinin koku |
| `-Priority` | PackErp, ArdCRM, ProductionPlanning, EtiketTasarim | Listede once degerlendirilecek projeler |
| `-Apply` | kapali | Verilmezse yalnizca plan |
| `-IncludeMarginal` | kapali | `Incelenecek` sinifindakileri de kurar |
| `-AddPointer` | kapali | InPlace'te var olan talimat dosyasinin sonuna router bolumu ekler |
| `-LinkPointer` | kapali | Sidecar'da proje kokune izlenmeyen isaret dosyasi birakir |
| `-WithLauncher` | kapali | `baslat-claude.cmd` / `baslat-codex.cmd` uretir |
| `-NoDetect` | kapali | `testing.md` icin tespit yerine bos sablon kullanir |
| `-ReportPath` | yok | Markdown kurulum raporu yazar |

## Initialize-GraphContext.ps1 parametreleri

| Parametre | Varsayilan | Aciklama |
|---|---|---|
| `-ProjectPath` | zorunlu | Hedef proje klasoru |
| `-Mode` | `Sidecar` | `Sidecar` veya `InPlace` |
| `-SidecarRoot` | `<ust klasor>\_graph-context` | Sidecar koku |
| `-AddPointer` | kapali | InPlace: var olan `AGENTS.md`/`CLAUDE.md` sonuna router bolumu |
| `-LinkPointer` | kapali | Sidecar: proje kokune untracked `.graph-context.md` |
| `-WithLauncher` | kapali | Baslatma kisayollari |
| `-NoDetect` | kapali | Bos `testing.md` sablonu |
| `-PassThru` | kapali | Sonuc nesnesi dondurur |
| `-WhatIf` | - | Hicbir sey yazmadan ne yapilacagini gosterir |

## Var olan dosyalara ne olur

| Durum | Davranis |
|---|---|
| `context/...` zaten var | Dosya korunur, "korundu" olarak raporlanir |
| `tasks/...` zaten var | Korunur |
| `AGENTS.md` yok | Sablondan router olarak olusturulur |
| `AGENTS.md` var | **Degistirilmez**; `AGENTS.graph-context.md` eki birakilir, `-AddPointer` ile dosya sonuna isaretli bolum eklenir |
| `CLAUDE.md` yok | `CLAUDE.template.md` icerigi `CLAUDE.md` olarak olusturulur |
| `CLAUDE.md` var | **Degistirilmez**; `CLAUDE.graph-context.md` eki + istege bagli `-AddPointer` |
| Router bolumu zaten eklenmis | Tekrar eklenmez (isaret: `<!-- graph-context:start -->`) |

## Geri alma

```powershell
# Sidecar kurulumu
Remove-Item "D:\C#\_graph-context\PackErp" -Recurse
Remove-Item "D:\C#\PackErp\.graph-context.md"   # -LinkPointer kullanildiysa

# InPlace kurulumu (git deposunda)
git status                 # eklenen dosyalari gor
git clean -nd context tasks   # once kuru calistirma
```

`-AddPointer` ile eklenen bolum `<!-- graph-context:start -->` ve
`<!-- graph-context:end -->` isaretleri arasindadir; elle silmek guvenlidir.

## Sik karsilasilanlar

**Build komutu `[tespit edilemedi]` cikti.**
Projede `.sln` yok ve birden fazla `.csproj` var demektir. `context/testing.md`
icinde dogru hedefi elle yazin; betik tahmin uretmez.

**Test komutu bulunamadi.**
Test projesi `IsTestProject` veya xunit/NUnit/MSTest referansi tasimiyordur.
Test projesi gercekten yoksa satir "[test projesi bulunamadi]" olarak kalir.

**Proje `Atlandi` siniflandi ama kurmak istiyorum.**
Tek projeye dogrudan kurun:
`\tools\Initialize-GraphContext.ps1 -ProjectPath "D:\C#\KucukProje"`

**Klasor adi `_` veya `.` ile basliyor.**
Taramada atlanir (sidecar kokunun kendisini taramamak icin).
