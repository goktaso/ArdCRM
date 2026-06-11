# ArdCRM — Claude Code Proje Bağlamı

## Proje Tanımı
ARD Sistem Danışmanlık için geliştirilmiş, müşteri/cari takip ve teklif yönetim sistemi.
Standalone çalışır; Netsis/Logo ERP adapter pattern ile sonradan entegre edilebilir.

## Geliştirici
- **Ad:** Özay Göktaş
- **Firma:** ARD Sistem Danışmanlık, İzmir
- **GitHub:** goktaso

## Ortam Konfigürasyonu
| Ortam | Server | Veritabanı | Kullanım |
|-------|--------|------------|----------|
| İş | `OZAY\DATA` | `ArdCRM` | Windows Auth |
| Ev | `ARDA\ARDA` | `ArdCRM` | Windows Auth |

> appsettings.Work.json ve appsettings.Home.json dosyaları .gitignore'dadır.
> Yeni makinede kurulum için: docs/SETUP.md'ye bak.

## Mimari — 4 Katman
```
ArdCRM.Core       → Entity, Interface, DTO, Enum (bağımlılık YOK)
ArdCRM.Data       → Repository, DbContext, Migration (Core'a bağımlı)
ArdCRM.Business   → Service, Validator (Core + Data'ya bağımlı)
ArdCRM.Web        → Controller, View, ViewModel (Business'a bağımlı)
```

## Kritik Kurallar (asla ihlal etme)
- [ ] `dotnet run` KULLANMA → her zaman IIS Express / VS Debugger
- [ ] Hardcoded connection string YOK → appsettings + environment
- [ ] ServiceResult<T> pattern → tüm Business servis dönüşleri
- [ ] Generic Repository → Data katmanında IRepository<T>
- [ ] ERP (Netsis/Logo) → SADECE READ, asla yazma
- [ ] Hassas veri → dış bulut servise gönderme
- [ ] Migration script → her şema değişikliğinde docs/migrations/ klasörüne ekle

## Kullanılan Teknolojiler
- ASP.NET Core 8 MVC / Razor Pages
- Dapper (Netsis okuma) + Entity Framework Core (ArdCRM DB)
- SQL Server (Windows Auth)
- Bootstrap 5 + Vanilla JS
- xUnit (test)

## Klasör Yapısı
```
ArdCRM/
├── CLAUDE.md                  ← bu dosya (git'e girer)
├── klavuz.md                  ← geliştirici kılavuzu (git'e girer)
├── .claude/
│   └── settings.json          ← Claude Code ayarları (git'e girer)
├── docs/
│   ├── SETUP.md               ← yeni makine kurulum rehberi
│   └── migrations/            ← sıralı SQL migration dosyaları
├── ArdCRM.Core/
│   ├── Entities/              ← Musteri, Teklif, Iletisim...
│   ├── Interfaces/            ← IRepository<T>, IService<T>...
│   ├── DTOs/                  ← Request/Response DTO'ları
│   └── Enums/                 ← MusteriTipi, TeklifDurumu...
├── ArdCRM.Data/
│   ├── Context/               ← ArdCrmDbContext
│   ├── Repositories/          ← GenericRepository<T>, özel repo'lar
│   └── Migrations/            ← EF Core migration dosyaları
├── ArdCRM.Business/
│   ├── Services/              ← MusteriService, TeklifService...
│   └── Validators/            ← FluentValidation kuralları
└── ArdCRM.Web/
    ├── Controllers/           ← MusteriController, TeklifController...
    ├── Views/                 ← Razor .cshtml dosyaları
    ├── Models/                ← ViewModel'lar (sadece View için)
    └── wwwroot/               ← CSS, JS, görseller
```

## Aktif Geliştirme Durumu
> Bu bölümü her önemli adımdan sonra güncelle.

- [x] Proje yapısı oluşturuldu
- [ ] DbContext ve bağlantı konfigürasyonu
- [ ] Musteri entity + repository
- [ ] MusteriService (ServiceResult<T>)
- [ ] MusteriController + Views (Liste, Detay, Ekle, Düzenle)
- [ ] Teklif modülü
- [ ] Netsis adapter (opsiyonel)

## Devam Edilecekler (WIP notu)
> Her oturum kapanışında buraya not düş, bir sonraki oturumda okuyarak başla.

<!-- ÖRNEK:
Son bırakılan yer: MusteriService.GetAll() metodu yazıldı,
unit test eksik. Sonraki adım: MusteriControllerTest.
-->

## Sık Kullanılan Komutlar
```bash
# Migration oluştur
dotnet ef migrations add [MigrationAdi] --project ArdCRM.Data --startup-project ArdCRM.Web

# Migration uygula
dotnet ef database update --project ArdCRM.Data --startup-project ArdCRM.Web

# Test çalıştır
dotnet test ArdCRM.Tests/

# Build kontrol
dotnet build ArdCRM.sln
```

## ERP Adapter Notu
Netsis entegrasyonu hazır olduğunda şu adımları izle:
1. `ArdCRM.Core/Interfaces/IErpAdapter.cs` → interface tanımla
2. `ArdCRM.Data/Adapters/NetsisAdapter.cs` → implement et
3. `appsettings` → Netsis connection string ekle (gitignore'da)
4. DI container'a kaydet → `services.AddScoped<IErpAdapter, NetsisAdapter>()`
