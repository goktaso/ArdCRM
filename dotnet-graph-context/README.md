# .NET Graph Context

Codex ve Claude Code ile calisirken **tum dokumani korlemesine okumak yerine**,
gorevi siniflandirip yalnizca gerekli baglami yukleyen tasinabilir bir proje-baglam
sistemi.

```text
Gorev -> Siniflandirma -> Ilgili baglam secimi -> Inceleme/Plan -> Uygulama
      -> Build/Test dogrulamasi -> Hata varsa yalniz ilgili domain/karar dosyasina donus
```

Yontemin ayrintisi: [`docs/graph-engineering.md`](docs/graph-engineering.md)

## Temel guvence

Kurulum betikleri **hicbir dosyanin uzerine yazmaz**:

- Var olan `context/`, `tasks/`, `AGENTS.md`, `CLAUDE.md` dosyalari korunur.
- Var olan talimat dosyasina yalnizca istege bagli olarak (`-AddPointer`)
  **dosyanin sonuna** isaretli bir router bolumu eklenir; mevcut satirlar degismez.
- Varsayilan calisma modu **plan modudur**; `-Apply` verilmeden hicbir sey yazilmaz.
- Varsayilan kurulum modu **Sidecar**'dir; proje klasorunde tek dosya bile degismez.

## Iki kurulum modu

| Mod | Ne yapar | Ne zaman |
|---|---|---|
| **Sidecar** (varsayilan) | `context/` ve `tasks/` proje klasorunun **disinda**, `<kok>\_graph-context\<Proje>` altinda olusur. Proje deposu hic dokunulmadan kalir. | Olgun/kritik projeler (orn. PackErp), git gecmisini kirletmek istemediginiz her sey |
| **InPlace** | `context/` ve `tasks/` proje kokune eklenir; router bolumu `AGENTS.md`/`CLAUDE.md` sonuna eklenir | Baglami ekiple paylasmak ve depoya commit etmek istediginiz projeler |

Sidecar ile basla, ise yaradigina ikna olunca `baglanti.md` icindeki iki satirla
iceri tasi. Ters yonde geri donus yok; bu yuzden varsayilan sidecar.

## Hizli kullanim

```powershell
# 1) Once yalnizca plan: hicbir dosya yazilmaz
.\tools\Install-GraphContextAll.ps1 -Root "D:\C#"

# 2) Uygun projelere sidecar kurulum + baslatma kisayollari + rapor
.\tools\Install-GraphContextAll.ps1 -Root "D:\C#" -Apply -WithLauncher `
    -ReportPath "D:\C#\_graph-context\kurulum-raporu.md"

# 3) Tek proje, proje icine kurulum (var olan CLAUDE.md korunur, sonuna router eklenir)
.\tools\Initialize-GraphContext.ps1 -ProjectPath "D:\C#\ArdCRM" -Mode InPlace -AddPointer
```

Tum betikler `-WhatIf` destekler.

## Siniflandirma

`Install-GraphContextAll.ps1` her klasoru tarar ve uc sinifa ayirir:

| Sinif | Olcut | Davranis |
|---|---|---|
| `Uygun` | Cok projeli .NET cozumu | Kurulur |
| `Incelenecek` | Tek katmanli, test projesi yok veya egitim/ornek isim kalibi | Yalnizca `-IncludeMarginal` ile kurulur |
| `Atlandi` | .NET projesi yok veya tek proje + 30'dan az kod dosyasi | Kurulmaz |

Kucuk, tek katmanli ve egitim projeleri varsayilan olarak degistirilmez.

## Build/test komutlari uydurulmaz

`context/testing.md` diskteki gercek dosyalardan uretilir: `.sln` / `.csproj`
taranir, test projeleri (`IsTestProject`, xunit/NUnit/MSTest referansi) tespit
edilir, `package.json` script'leri okunur, `.editorconfig` / analyzer /
CI tanimlari isaretlenir. Tespit edilemeyen alan `[tespit edilemedi]` olarak
birakilir ve raporda "elle doldurulacak" listesine girer.

## Klasor yapisi

```text
dotnet-graph-context/
├─ template/                 <- projelere kopyalanan baglam iskeleti
│  ├─ AGENTS.md              <- Codex router'i (proje talimati yoksa)
│  ├─ AGENTS.graph-context.md<- Codex router eki (talimat varsa)
│  ├─ CLAUDE.template.md     <- Claude Code router'i (CLAUDE.md yoksa)
│  ├─ CLAUDE.graph-context.md<- Claude Code router eki (CLAUDE.md varsa)
│  ├─ context/               <- README(router), proje-ozeti, kurallar, kararlar,
│  │                            hedefler, testing, domains/, kaynaklar/
│  └─ tasks/                 <- todo (CHECKPOINT), lessons
├─ tools/
│  ├─ GraphContext.Detection.ps1   <- tespit + siniflandirma
│  ├─ Initialize-GraphContext.ps1  <- tek proje kurulumu (Sidecar | InPlace)
│  └─ Install-GraphContextAll.ps1  <- kok klasor tarama, plan + toplu kurulum
└─ docs/
   ├─ graph-engineering.md   <- yontem
   ├─ sidecar-modu.md        <- sifir-dokunus modu
   ├─ kurulum.md             <- adim adim kurulum ve parametreler
   └─ ornek-ardcrm.md        <- doldurulmus gercek ornek
```

## Ilk 15 dakika

1. `context/proje-ozeti.md` icindeki koseli parantezli alanlari doldurun.
2. `context/kurallar-ve-sinirlar.md` icine degismemesi gereken is kurallarini yazin.
3. Mimari veya urun karari alindiginda `context/kararlar.md`ye ekleyin.
4. Tekrar tekrar baglam gerektiren her is alani icin `context/domains/` altinda
   dosya acin ve `context/README.md` haritasina satir ekleyin.
5. Oturum sonunda kalici ders varsa `tasks/lessons.md`ye, devam eden is varsa
   `tasks/todo.md` CHECKPOINT'ine kisa kayit dusun.

## Guvenlik

Gizli bilgi, baglanti dizesi, gercek musteri verisi ve erisim anahtari hicbir
baglam dosyasina yazilmaz. Sablonlarda bu kural `kurallar-ve-sinirlar.md` icinde
sabit madde olarak gelir.

## Gereksinimler

Windows PowerShell 5.1 veya PowerShell 7+. Betikler yalnizca dosya sistemi okur/yazar;
ag erisimi, paket kurulumu veya `dotnet` calistirmasi yapmaz.

## Bu klasoru ayri repoya tasima

Bu sistem su an ArdCRM deposunun icinde tasiniyor ancak hicbir ArdCRM dosyasina
bagimli degildir. Kendi deposuna almak icin:

```powershell
# 1) GitHub'da bos bir depo acin: goktaso/dotnet-graph-context
# 2) Yerelde:
cd D:\C#
git clone https://github.com/goktaso/ArdCRM.git _tmp-ardcrm
Copy-Item "_tmp-ardcrm\dotnet-graph-context" "D:\C#\_graph-context-system" -Recurse
Remove-Item "_tmp-ardcrm" -Recurse -Force

cd D:\C#\_graph-context-system
git init
git add .
git commit -m "init: .NET Graph Context sistemi"
git branch -M main
git remote add origin https://github.com/goktaso/dotnet-graph-context.git
git push -u origin main
```

Tasima sonrasi ArdCRM deposundaki `dotnet-graph-context/` klasoru silinebilir;
ArdCRM'in kendi `context/` ve `tasks/` klasorleri bundan bagimsizdir ve yerinde kalir.
