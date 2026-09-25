# Domain — Teklif

## Sorumluluk

Tekliflerin oluşturulması, güncellenmesi, listelenmesi, durum akışının yönetilmesi
ve müşteri bazlı teklif görünümü.

## Giriş / çıkışlar

- Giriş: Web formları (`Ekle`, `Duzenle`), liste üzerinden tek tıkla durum değişikliği.
- Çıkış: `ServiceResult<Teklif>`, müşteri detayındaki teklif listesi,
  dashboard'daki açık teklif sayısı ve toplam tutar.

## Temel kurallar

- `TeklifNo` servis tarafından üretilir (`ITeklifService.GenerateTeklifNoAsync`);
  kullanıcıdan alınmaz.
- `Durum` alanı `TeklifDurumu` enum'ıdır: `Taslak(0)`, `Gonderildi(1)`,
  `Gorusuluyor(2)`, `Onaylandi(3)`, `Reddedildi(4)`, `Iptal(5)`. Varsayılan `Taslak`.
- `Tutar > 0`, `MusteriId > 0`, `Baslik` zorunlu (maks. 300), `Para` zorunlu
  (maks. 3 karakter, varsayılan `TRY`) — `TeklifValidator`.
- `GecerlilikTarihi` verilmişse `TeklifTarihi`'nden önce olamaz.
- Silme soft delete'tir; teklif geçmişi korunur.
- Durum geçişleri kullanıcı tarafından serbest seçilir; otomatik iş akışı yoktur.
  Yeni bir kısıt gerekiyorsa önce `kararlar.md`ye karar yazılır.

## İlgili kod konumları

- Entity: `ArdCRM.Core/Entities/Teklif.cs`
- Enum: `ArdCRM.Core/Enums/Enums.cs` (`TeklifDurumu`)
- Sözleşme: `ArdCRM.Core/Interfaces/IServices.cs` (`ITeklifService`)
- Servis: `ArdCRM.Business/Services/TeklifService.cs`
- Doğrulama: `ArdCRM.Business/Validators/TeklifValidator.cs`
- Web: `ArdCRM.Web/Controllers/TeklifController.cs`, `Views/Teklif/*`

## Bağımlılıklar

- `IRepository<Teklif>`, `Musteri` navigation property (teklif listesi müşteri adını gösterir).

## Doğrulama

- `dotnet test ArdCRM.Tests/ArdCRM.Tests.csproj --filter FullyQualifiedName~TeklifServiceTests`
- Durum akışı değişikliğinde teklif listesi manuel doğrulanır.
