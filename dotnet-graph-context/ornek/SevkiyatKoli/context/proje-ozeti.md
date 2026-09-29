# Proje Ozeti — SevkiyatKoli

## Kısa tanım

Bir sevkiyat için **koli numarası üreten** ve kolinin doğru doldurulduğunu
**doğrulayan** örnek uygulama. Amacı çalışan bir ürün değil, Graph Context
bağlam sisteminin uçtan uca nasıl kullanıldığını göstermektir.

## Mimari

- Teknoloji: .NET 8, xUnit
- Katmanlar: `Core` (model + sözleşme, bağımlılık yok) → `Business` (kurallar) → `Tests`
- Veri kaynağı: **bellek içi** (`Dictionary`). Veritabanı bilerek kapsam dışıdır.
- Ana çözüm dosyası: `SevkiyatKoli.sln`

`Core` içindeki durum değiştiren metotlar `internal`'dır ve `InternalsVisibleTo`
ile yalnızca `Business` katmanına açılır — model dışarıdan keyfi değiştirilemez.

## Kapsam

- Dahil: koli açma + numaralandırma, satır ekleme, ağırlık sınırı, kapatma, iptal.
- Hariç: barkod/etiket basımı, ERP'ye yazma, kullanıcı yetkilendirme, veritabanı,
  kullanıcı arayüzü.

## Mevcut durum

- Çalışanlar: 5 iş kuralının tamamı kodlandı ve testlendi.
- Bilinen eksikler: kalıcılık yok (süreç kapanınca veri gider) — bilinçli tercih.
- Son doğrulama: 2026-09-29 — `dotnet test` 21/21 yeşil.

## Önemli konumlar

- Model + sözleşme: `SevkiyatKoli.Core/` (`Modeller.cs`, `KoliNumaratoru.cs`, `IKoliService.cs`)
- Kurallar: `SevkiyatKoli.Business/KoliService.cs`
- Testler: `SevkiyatKoli.Tests/KoliServiceTests.cs`
