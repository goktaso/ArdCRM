# Ornek: ArdCRM (InPlace kurulum)

ArdCRM, bu sistemin doldurulmus referans ornegidir: sablon iskelet degil,
gercek proje bilgisiyle yazilmis bir `context/` agaci.

Depo: https://github.com/goktaso/ArdCRM

## Kurulum bicimi

InPlace + pointer. ArdCRM'in zaten bir `CLAUDE.md` dosyasi vardi; o dosya
degistirilmedi, yalnizca sonuna isaretli router bolumu eklendi (52 satir ekleme,
0 satir silme). `AGENTS.md` yoktu, Codex router'i olarak olusturuldu.

## Uretilen agac

```text
ArdCRM/
├─ AGENTS.md                      <- Codex router'i (yeni)
├─ CLAUDE.md                      <- mevcut dosya + sonuna router bolumu
├─ context/
│  ├─ README.md                   <- router + gorev siniflandirma tablosu
│  ├─ proje-ozeti.md              <- 4 katman, kapsam, bilinen riskler
│  ├─ kurallar-ve-sinirlar.md     <- ERP salt-okunur, soft delete, ServiceResult...
│  ├─ kararlar.md                 <- 15 tarihli karar (2026-06-11 / 2026-09-25)
│  ├─ hedefler.md                 <- aday hedefler, doldurulacak alanlar isaretli
│  ├─ testing.md                  <- gercek build/test komutlari
│  ├─ domains/
│  │  ├─ musteri.md
│  │  ├─ teklif.md
│  │  ├─ erp-senkron.md
│  │  └─ veri-ve-migration.md
│  └─ kaynaklar/
└─ tasks/
   ├─ todo.md                     <- tek CHECKPOINT
   └─ lessons.md                  <- 3 kanitli ders
```

## Ornek: domain dosyasi ne kadar somut olmali

`context/domains/erp-senkron.md` dosyasindan:

- **Degismez kural:** ERP'ye asla yazma yapilmaz; `IErpAdapter` yalnizca okuma
  metodu tanimlar.
- **Akis:** `TestConnectionAsync` -> `OnizlemeGetirAsync` (DB'ye yazmaz) ->
  kullanici secimi -> `MusterileriAktarAsync`.
- **Kod konumlari:** `ArdCRM.Core/Interfaces/IErpAdapter.cs`,
  `ArdCRM.Data/Adapters/NetsisAdapter.cs`, `ArdCRM.Business/Services/ErpSenkronService.cs`,
  `ArdCRM.Web/Controllers/ErpController.cs`
- **Dogrulama:** adapter mock'lanir; canli ERP'ye sorgu atan dogrulama kullanici
  onayi ister.

Bir domain dosyasi bu dort basligi tasiyorsa ise yarar: kural, akis, kod konumu,
dogrulama. Genel gecer aciklama tasiyorsa yaramaz.

## Ornek: karar kaydi

| Tarih | Karar | Gerekce | Durum |
|---|---|---|---|
| 2026-06-11 | ArdCRM veritabani EF Core, ERP okumasi Dapper | ERP semasi bizim degil; POCO mapping yerine hizli ham sorgu | aktif |
| 2026-06-11 | Sema degisiklikleri `docs/migrations/` altinda sirali SQL script olarak da tutulur | Yetki kisitli SQL Server ortamlarinda `dotnet ef` calistirilamiyor | aktif |

Karar kaydi "ne yaptik"i degil, **"neden boyle, degistirirsek ne kirilir"i** tutar.

## Ornek: tespit edilen dogrulama komutlari

```text
dotnet build ArdCRM.sln
dotnet test ArdCRM.Tests/ArdCRM.Tests.csproj
```

Statik analiz: yapilandirilmis lint/analyzer kurali yok.
CI: `.github/workflows` yok.

Bu satirlar tahmin degil; `ArdCRM.sln`, `ArdCRM.Tests/ArdCRM.Tests.csproj` ve
depo kokunun taranmasiyla uretildi.
