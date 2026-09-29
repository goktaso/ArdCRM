# Domain — Koli

## Sorumluluk

Bir sevkiyat içindeki kolilerin açılması, numaralandırılması, doldurulması,
kapatılması ve iptali.

## Giriş / çıkışlar

- Giriş: `irsaliyeNo`, `KoliSatiri` (stok kodu, miktar, ağırlık).
- Çıkış: `Sonuc<Koli>` / `Sonuc`; koli numarası, brüt ağırlık, durum.

## Durum akışı

```text
KoliAc ──► Acik ──KoliKapat──► Kapali
             │                   │
             └────KoliIptal──────┴──► Iptal   (numara serbest kalmaz)
```

- `Acik`: satır eklenebilir.
- `Kapali`: içerik donar. Düzeltme yolu **yok** — iptal edip yeni koli açılır.
- `Iptal`: nihai. Kayıt silinmez, numara yeniden kullanılmaz.
- Boş koli kapatılamaz.

## Temel kurallar

| # | Kural | Kod | Test |
|---|---|---|---|
| 1 | Format `{IrsaliyeNo}-{Sira:000}` | `KoliNumaratoru.Uret` | `Kural1_*` |
| 2 | Sıra 1'den başlar, üretimde atlanmaz | `Sevkiyat.SonrakiSiraNoUret` | `Kural2_*` |
| 3 | Kapalı/iptal koliye satır eklenemez | `KoliService.SatirEkle` | `Kural3_*` |
| 4 | Brüt ağırlık ≤ 30 kg (dara 0.8 kg dahil) | `KoliService.SatirEkle` | `Kural4_*` |
| 5 | Numara tekrar kullanılmaz | `Sevkiyat.SonSiraNo` | `Kural5_*` |

Sınır değer: brüt tam 30.0 kg **kabul edilir**, 30.01 kg reddedilir
(`Kural4_TamSinirdakiSatir_Kabul_Edilir`).

## İlgili kod konumları

- `SevkiyatKoli.Core/Modeller.cs` — `Sevkiyat`, `Koli`, `KoliSatiri`, `KoliDurumu`
- `SevkiyatKoli.Core/KoliNumaratoru.cs` — numara üretimi ve biçim denetimi
- `SevkiyatKoli.Core/IKoliService.cs` — sözleşme
- `SevkiyatKoli.Business/KoliService.cs` — kuralların tamamı
- `SevkiyatKoli.Tests/KoliServiceTests.cs` — kural başına test

## Bağımlılıklar

Yok. Bellek içi çalışır, dış servise bağlanmaz.

## Doğrulama

```bash
dotnet test "SevkiyatKoli.Tests/SevkiyatKoli.Tests.csproj"
dotnet test "SevkiyatKoli.Tests/SevkiyatKoli.Tests.csproj" --filter FullyQualifiedName~Kural4
```

Yeni kural eklenirken: `kurallar-ve-sinirlar.md`ye numarasıyla yaz →
`KoliService`e uygula → `Kural{N}_` önekli test ekle → bu tabloya satır ekle.
