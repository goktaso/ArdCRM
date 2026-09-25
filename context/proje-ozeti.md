# Proje Özeti — ArdCRM

## Kısa tanım

ARD Sistem Danışmanlık (İzmir) için müşteri/cari takibi ve teklif yönetimi yapan
ASP.NET Core 8 MVC uygulaması. Standalone çalışır; Netsis/Logo ERP'ye adapter
pattern ile **salt okuma** yapacak şekilde bağlanabilir.

## Mimari

- Teknoloji: .NET 8 / ASP.NET Core 8 MVC (Razor Views) / EF Core 8 + Dapper 2.1
- Katmanlar ve bağımlılık yönü:
  `ArdCRM.Core` (bağımlılık yok) ← `ArdCRM.Data` ← `ArdCRM.Business` ← `ArdCRM.Web`
  - Core: Entity, Interface, Enum, `ServiceResult` / `ServiceResult<T>`
  - Data: `ArdCrmDbContext`, `GenericRepository<T>`, `Adapters/` (Netsis, Null)
  - Business: `MusteriService`, `TeklifService`, `ErpSenkronService`, FluentValidation validator'ları
  - Web: Controller + Razor View + ViewModel, DI kayıtları (`Program.cs`)
- Veri kaynağı: SQL Server (Windows Authentication). ERP tarafında ayrı, salt
  okunur bir Netsis veritabanı (`NetsisConnection`) opsiyoneldir.
- Ana çözüm dosyası: `ArdCRM.sln` (5 proje: Core, Data, Business, Web, Tests)

## Kapsam

- Dahil: müşteri CRUD + detay, teklif CRUD + durum akışı, dashboard istatistikleri,
  liste içi arama/filtreleme (JS), ERP'den müşteri önizleme ve seçerek aktarım,
  soft delete, FluentValidation kuralları, xUnit birim testleri.
- Hariç: kimlik doğrulama/yetkilendirme, çok kullanıcılı rol yönetimi, fatura/sipariş
  modülü, ERP'ye yazma, raporlama/DWH katmanı, çoklu dil.

## Mevcut durum

- Çalışanlar: Musteri modülü, Teklif modülü, Dashboard, ERP senkron (önizleme +
  aktarım), DataProtection anahtarları proje klasöründe, Work/Home ortam profilleri.
- Bilinen eksikler veya riskler:
  - Kimlik doğrulama yok — uygulama açıkta çalışır varsayımıyla geliştirilmiştir.
  - `app.UseDeveloperExceptionPage()` ortamdan bağımsız açıktır (`ArdCRM.Web/Program.cs`).
  - `NetsisAdapter` sütun adları tek bir Netsis kurulumuna göre doğrulanmıştır.
  - EF Core migration'ları ile `docs/migrations/*.sql` script'leri elle senkron tutulur.
- Son doğrulama: 2026-06-11 — 27 xUnit testi yeşil (kaynak: `CLAUDE.md` geliştirme
  durumu). Bu bağlam dosyası oluşturulurken build/test bu ortamda **çalıştırılmadı**.

## Önemli konumlar

- Uygulama kodu: `ArdCRM.Core/`, `ArdCRM.Data/`, `ArdCRM.Business/`, `ArdCRM.Web/`
- Testler: `ArdCRM.Tests/` (xUnit + Moq)
- Migration/SQL: `docs/migrations/` (`001_InitialCreate.sql`, `002_TestVerisi.sql`)
- EF Core migration'ları: `ArdCRM.Data/Migrations/`
- Kurulum rehberi: `docs/SETUP.md`
- Geliştirici kılavuzu (iş ↔ ev senkronizasyonu): `klavuz.md`
- Ortam dosyaları: `appsettings.Work.json` / `appsettings.Home.json` — **.gitignore'da**,
  içerikleri hiçbir bağlam dosyasına kopyalanmaz.
