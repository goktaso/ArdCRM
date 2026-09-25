# Domain — Veri Erişimi ve Migration

## Sorumluluk

ArdCRM veritabanı şeması, EF Core erişim katmanı, generic repository davranışı ve
şema değişikliklerinin sürümlenmesi.

## Temel kurallar

- ArdCRM veritabanı EF Core 8 ile yönetilir; ERP okuması bu kapsamda değildir
  (bkz. `erp-senkron.md`).
- Business katmanı `DbContext`'i doğrudan kullanmaz; erişim `IRepository<T>` üzerindendir.
- `DeleteAsync` soft delete yapar (`Aktif = false`); `GetAllActiveAsync` yalnızca
  aktif kayıtları getirir.
- Tüm entity'ler `BaseEntity`'den türer (Id, Aktif, oluşturma/güncelleme alanları).
- **Her şema değişikliğinde iki çıktı birlikte üretilir:**
  1. EF Core migration → `ArdCRM.Data/Migrations/`
  2. Sıralı SQL script → `docs/migrations/NNN_Aciklama.sql`
  Gerekçe: yetki kısıtlı SQL Server ortamlarında `dotnet ef database update`
  çalıştırılamıyor, script SSMS ile elle uygulanıyor.
- Bağlantı dizesi koda yazılmaz; `appsettings.{Ortam}.json` (gitignore) veya
  `ConnectionStrings__DefaultConnection` ortam değişkeni kullanılır.
- Gerçek veri içeren seed script'leri repoya girmez (`**/seed_*.sql` gitignore'da).

## İlgili kod konumları

- `ArdCRM.Data/Context/ArdCrmDbContext.cs`
- `ArdCRM.Data/Repositories/GenericRepository.cs`
- `ArdCRM.Core/Interfaces/IRepository.cs`, `ArdCRM.Core/Entities/BaseEntity.cs`
- `ArdCRM.Data/Migrations/`, `docs/migrations/`

## Komutlar

```bash
dotnet ef migrations add <MigrationAdi> --project ArdCRM.Data --startup-project ArdCRM.Web
dotnet ef database update --project ArdCRM.Data --startup-project ArdCRM.Web
```

## Doğrulama

- `dotnet build ArdCRM.sln` sonrası `dotnet test ArdCRM.Tests/ArdCRM.Tests.csproj`.
- Şema değişikliği kullanıcı onayı gerektirir; uygulanan script `docs/migrations/`
  altında numaralandırılmış olarak durur.
