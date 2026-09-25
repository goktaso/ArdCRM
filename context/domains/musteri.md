# Domain — Müşteri (Cari)

## Sorumluluk

Müşteri/cari kayıtlarının oluşturulması, güncellenmesi, listelenmesi, detay
görünümü ve pasife alınması.

## Giriş / çıkışlar

- Giriş: Web formları (`Ekle`, `Duzenle`), ERP aktarımı (`ErpSenkronService`).
- Çıkış: `ServiceResult<Musteri>` / `ServiceResult<IEnumerable<Musteri>>`,
  müşteri detay sayfasındaki teklif listesi ve özet istatistikler, dashboard sayacı.

## Temel kurallar

- `Ad` ve `FirmaAdi` zorunludur; `Email` verildiyse geçerli e-posta olmalıdır
  (`MusteriValidator`). Uzunluk sınırları: Ad 100, FirmaAdi 200, Telefon 20, VergiNo 20.
- `Tip` alanı `MusteriTipi` enum'ıdır: `Potansiyel(0)`, `Aktif(1)`, `Pasif(2)`, `VIP(3)`.
  Varsayılan `Potansiyel`.
- Silme fiziksel değildir: `Aktif = false` (soft delete). Listeler
  `GetAllActiveAsync` kullanır.
- `TamAd` hesaplanmış alandır (`Ad + Soyad`), veritabanına yazılmaz.
- Müşteri silinirken bağlı teklifler dikkate alınır; teklif geçmişi korunur.

## İlgili kod konumları

- Entity: `ArdCRM.Core/Entities/Musteri.cs`, `BaseEntity.cs`
- Enum: `ArdCRM.Core/Enums/Enums.cs` (`MusteriTipi`)
- Sözleşme: `ArdCRM.Core/Interfaces/IServices.cs` (`IMusteriService`)
- Servis: `ArdCRM.Business/Services/MusteriService.cs`
- Doğrulama: `ArdCRM.Business/Validators/MusteriValidator.cs`
- Veri: `ArdCRM.Data/Repositories/GenericRepository.cs`, `Context/ArdCrmDbContext.cs`
- Web: `ArdCRM.Web/Controllers/MusteriController.cs`, `Views/Musteri/*`,
  `Models/MusteriDetayViewModel.cs`

## Bağımlılıklar

- `IRepository<Musteri>` (Data), `ITeklifService` (detay sayfasındaki teklif listesi için),
  ERP tarafında `ErpSenkronService`.

## Doğrulama

- `dotnet test ArdCRM.Tests/ArdCRM.Tests.csproj --filter FullyQualifiedName~MusteriServiceTests`
- UI değişikliğinde müşteri listesi + detay sayfası manuel açılır.
