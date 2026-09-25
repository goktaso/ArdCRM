# dotnet-graph-context — Depo Talimati

Bu depo bir **arac**tir: .NET projelerine tasinabilir bir baglam sistemi kurar.
Burada uygulama kodu yoktur; sablon metinleri ve PowerShell betikleri vardir.

## Degismez kurallar

- **Hicbir betik dosya ezmez.** Yeni bir dosya yazma yolu eklerken mutlaka
  "varsa koru" davranisini kullanin (`Write-GcFile`). Ezme davranisi eklenmez.
- **Varsayilan plan modudur.** `Install-GraphContextAll.ps1` `-Apply` almadan
  hicbir sey yazmaz. Bu varsayilan degistirilmez.
- **Varsayilan kurulum modu Sidecar'dir.** Proje klasorunu kirletmeyen mod
  varsayilan kalir.
- **Komut uydurulmaz.** `context/testing.md` icerigi yalnizca diskte bulunan
  `.sln` / `.csproj` / `package.json` / CI dosyalarindan uretilir. Tespit
  edilemeyen alan `[tespit edilemedi]` olarak birakilir.
- **Gizli bilgi yok.** Sablonlara veya uretilen dosyalara baglanti dizesi,
  anahtar veya gercek veri yazilmaz.
- Sablon metinleri ASCII Turkce ile yazilir (diakritik yok); Windows PowerShell
  5.1 konsolunda bozulmamasi icin.

## Yapi

| Klasor | Icerik |
|---|---|
| `template/` | Projelere kopyalanan baglam iskeleti (context/, tasks/, router dosyalari) |
| `tools/` | Tespit + kurulum betikleri |
| `docs/` | Yontem, sidecar modu, kurulum, doldurulmus ornek |

## Degisiklik yaparken

1. `tools/GraphContext.Detection.ps1` tespit ve siniflandirmayi tutar; kural
   degisikligi once burada yapilir.
2. `tools/Initialize-GraphContext.ps1` tek proje kurulumudur; iki mod
   (Sidecar/InPlace) ayni "ezme" garantisini paylasir.
3. `tools/Install-GraphContextAll.ps1` yalnizca tarama, siralama, raporlama yapar;
   dosya yazmaz, Initialize'i cagirir.

## Dogrulama

Bu depoda derlenecek proje yoktur. Degisiklik sonrasi asgari kontrol:

```powershell
# Sozdizimi
Get-ChildItem .\tools\*.ps1 | ForEach-Object {
    $e = $null
    [void][System.Management.Automation.Language.Parser]::ParseFile($_.FullName, [ref]$null, [ref]$e)
    if ($e) { Write-Host "HATA: $($_.Name)"; $e }
}

# Davranis: gecici bir agac uzerinde
.\tools\Initialize-GraphContext.ps1 -ProjectPath "<gecici-proje>" -WhatIf
.\tools\Initialize-GraphContext.ps1 -ProjectPath "<gecici-proje>"
.\tools\Initialize-GraphContext.ps1 -ProjectPath "<gecici-proje>"   # 0 eklendi olmali
```

Uc kontrol her zaman dogrulanir: **-WhatIf hicbir sey yazmaz**, **ikinci calistirma
hicbir sey eklemez**, **var olan dosya degismez**.

## PowerShell tuzagi (tekrar etmeyin)

Bos bir dosyada `Get-Content -Raw` `$null` doner ve `$null -notmatch '...'`
**False** uretir (skaler degil, bos koleksiyon islemi). Dosya icerigi okunurken
her zaman `[string](Get-Content ... -Raw)` kullanin.
