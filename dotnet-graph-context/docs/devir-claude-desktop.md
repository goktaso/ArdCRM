# Devir Notu — Claude Desktop (yerel oturum)

Bu dosya, bulut oturumunda kurulan Graph Context sistemini **yerel** Claude Desktop
oturumunda devam ettirmek icindir. Tek basina yeterlidir; baska bir sohbete ihtiyac yoktur.

## 1. Yerel oturumu ac

Claude Desktop -> **Code** sekmesi -> yeni oturum:

| Alan | Deger |
|---|---|
| Environment | **Local** (bulut degil - bu adim kritik) |
| Project folder | `D:\C#\ArdCRM` |
| Permission mode | Auto veya Accept edits (PowerShell'i sormadan kossun) |

Ortam oturum basinda secilir, sonradan degistirilemez. Bulut oturumunda
`D:\C#` gorunmez, `dotnet` ve PowerShell calismaz.

## 2. Depoyu guncelle

```powershell
cd D:\C#\ArdCRM
git fetch origin
git checkout claude/elegant-fermat-m8h69g
```

## 3. Ilk mesaj (kopyala-yapistir)

```text
Bu depoda Graph Engineering tabanli bir proje-baglam sistemi var. Amac: her
gorevde tum dokumani okumak yerine gorevi siniflandirip yalnizca gerekli
baglam dosyalarini yuklemek.

Once su iki dosyayi oku (baskasini okuma):
  dotnet-graph-context/README.md
  dotnet-graph-context/docs/graph-engineering.md

Sonra ArdCRM'de kurulu ornegi incele:
  context/README.md          (router - hangi gorevde ne okunur)
  context/domains/teklif.md  (doldurulmus domain ornegi)
  context/testing.md         (gercek build/test komutlari)

Ardindan iki isi sirayla yapacagiz:

IS 1 - D:\C# taramasi (plan modu, hicbir dosya yazilmaz):
  cd dotnet-graph-context
  .\tools\Install-GraphContextAll.ps1 -Root "D:\C#"
Cikan plan tablosunu bana goster ve yorumla: hangi proje neden "Uygun",
hangisi neden "Atlandi". Kurulum yapma, once benim onayimi al.

IS 2 - Ogrenmek icin sifirdan kucuk bir ornek proje: "Sevkiyat Koli Numaratoru".
Sirasi kesin sekilde sudur ve her adimda durup onayimi alacaksin:
  ADIM 1: Is kurallarini netlestir (kod YOK) - asagidaki 5 taslak kurali
          bana dogrulat/duzelt
  ADIM 2: context/ + tasks/ dosyalarini yaz (kod YOK)
  ADIM 3: Iskeleti kur (Core/Business/Tests, 3 proje, dotnet new)
  ADIM 4: Kurallari kodla + her kural icin en az bir xUnit testi
  ADIM 5: dotnet build + dotnet test calistir, cikti ile kanitla
  ADIM 6: Kalici karar cikti ise context/kararlar.md'ye, yarim is varsa
          tasks/todo.md CHECKPOINT'ine yaz

ADIM 1 icin taslak kurallar (benim duzeltmem gerekiyor):
  1. Koli numarasi formati: {IrsaliyeNo}-{Sira:000}  ornek: SEV2026001-003
  2. Sira numarasi sevkiyat icinde 1'den baslar ve atlanmaz
  3. Kapali koliye satir eklenemez; duzeltme yolu iptal + yeni koli
  4. Koli brut agirligi 30 kg'i asamaz; asan satir reddedilir
  5. Uretilen numara tekrar kullanilmaz (koli iptal edilse bile)
Kapsam disi: barkod basimi, ERP'ye yazma, yetkilendirme, veritabani (bellek ici)

Kurallar:
- Var olan hicbir dosyayi ezme; CLAUDE.md'ye dokunacaksan yalnizca dosyanin
  sonuna ekleme yap.
- Build/test komutu uydurma; .sln/.csproj'dan tespit et.
- Gizli bilgi, baglanti dizesi, gercek musteri verisi hicbir baglam dosyasina
  yazilmaz.
- Calistiramadigin bir kontrolu "dogrulandi" diye raporlama.

ADIM 1 ile basla: 5 kurali bana sor, cevabimi bekle.
```

## 4. Ornek proje nereye kurulacak

Ornek, mevcut projelerden bagimsiz dursun:

```text
D:\C#\SevkiyatKoli\
├─ SevkiyatKoli.sln
├─ SevkiyatKoli.Core\        (Sevkiyat, Koli, KoliSatiri, enum'lar)
├─ SevkiyatKoli.Business\    (KoliService - numara uretimi + dogrulama)
├─ SevkiyatKoli.Tests\       (xUnit - her kural icin test)
├─ context\                  (ADIM 2'de yazilacak)
├─ tasks\
├─ CLAUDE.md
└─ AGENTS.md
```

Bu proje egitim amaclidir; ArdCRM ve PackErp'ten tamamen ayridir.

## 5. PackErp icin hatirlatma

PackErp'e kurulum **sidecar** modda yapilir: baglam dosyalari
`D:\C#\_graph-context\PackErp\` altina gider, PackErp klasorunde tek dosya bile
degismez. Detay: `dotnet-graph-context/docs/sidecar-modu.md`

```powershell
.\tools\Initialize-GraphContext.ps1 -ProjectPath "D:\C#\PackErp" -WhatIf       # once kuru calistirma
.\tools\Initialize-GraphContext.ps1 -ProjectPath "D:\C#\PackErp" -WithLauncher # kurulum
```

## 6. Bulut oturumunda tamamlananlar

- `dotnet-graph-context/` - sablon + 3 PowerShell araci + 4 dokuman
- ArdCRM'e InPlace referans kurulum: `context/` (router, proje-ozeti, kurallar,
  kararlar, hedefler, testing, 4 domain dosyasi), `tasks/` (todo, lessons),
  `AGENTS.md`; `CLAUDE.md` korunarak sonuna router bolumu eklendi
- Araclar PowerShell 7.4.6 ile sentetik 5 projelik agac uzerinde calistirildi:
  -WhatIf hicbir sey yazmiyor, ikinci calistirma 0 dosya ekliyor, var olan
  dosyalar degismiyor
- Bulut oturumunda **calistirilamayan**: `dotnet build` / `dotnet test`
  (SDK indirme adresi ag politikasi tarafindan engelli). Ilk gercek dogrulama
  yerel oturumda yapilacak.
