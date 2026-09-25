# Kurallar ve Sınırlar — ArdCRM

Buraya yalnızca proje boyunca geçerliliğini koruyacak kurallar yazılır.
Gerekçeli kararlar için `kararlar.md` dosyasına bakın. Ana talimat dosyası
`CLAUDE.md`'dir; bu dosya onunla çelişmez, uygulanabilir sınır listesi sunar.

## Değişmez iş kuralları

- **ERP salt okunurdur.** Netsis/Logo/SAP tarafına hiçbir koşulda yazma, güncelleme
  veya silme yapılmaz. `IErpAdapter` yalnızca okuma metodları tanımlar.
- **Silme = soft delete.** `IRepository<T>.DeleteAsync` kaydı fiziksel silmez,
  `Aktif = false` yapar. Listelerde `GetAllActiveAsync` kullanılır.
- **Tüm Business servis dönüşleri `ServiceResult` / `ServiceResult<T>`'dir.**
  Controller'lar exception yakalamak yerine `Success` alanını kontrol eder.
- **Teklif numarası servis tarafından üretilir** (`ITeklifService.GenerateTeklifNoAsync`);
  kullanıcı girdisinden alınmaz.
- Kullanıcıya görünen metinler Türkçe, kod tanımlayıcıları İngilizce/Türkçe karışık
  mevcut düzene sadık kalır (entity ve alan adları Türkçe'dir).

## Mimari sınırlar

- Katman bağımlılığı tek yönlüdür: `Core ← Data ← Business ← Web`.
  Core hiçbir projeye referans vermez; Web doğrudan `DbContext` kullanmaz.
- Veri erişimi `IRepository<T>` üzerinden yapılır; Business katmanında ham
  `DbContext` veya SQL yazılmaz.
- ERP okuması Dapper ile `ArdCRM.Data/Adapters/` altında kalır; ArdCRM'in kendi
  veritabanı EF Core ile yönetilir.
- ERP bağlantı dizesi tanımlı değilse DI `NullErpAdapter` kaydeder — kod ERP'siz
  ortamda da çalışmak zorundadır.
- Doğrulama kuralları FluentValidation validator'larında tutulur; controller içinde
  dağınık if/else doğrulama yazılmaz.

## Güvenlik ve veri sınırları

- Gizli bilgi, bağlantı dizesi, erişim anahtarı ve gerçek müşteri verisi repoya,
  bu dosyaya veya herhangi bir `context/` dosyasına yazılmaz.
- Bağlantı dizeleri yalnızca `appsettings.Work.json` / `appsettings.Home.json`
  (gitignore'da) veya ortam değişkeni ile verilir. Kodda hardcoded bağlantı yok.
- Hassas veri dış bulut servislerine gönderilmez.
- `appsettings.json` git'e yalnızca **boş şablon** olarak girer.
- `**/seed_*.sql`, `*_credentials*`, `*_secrets*` ve `DataProtection-Keys/`
  gitignore'dadır; bu kapsam daraltılmaz.

## Kullanıcı onayı gerektiren işlemler

- Veritabanı şema değişikliği (yeni migration + `docs/migrations/` script'i).
- ERP adapter'ında sorgu/sütun eşlemesi değişikliği (canlı ERP'ye sorgu atar).
- `.gitignore` kapsamını daraltan değişiklikler.
- Yayınlama, veri silme veya geri alınamaz herhangi bir işlem.

## Çalıştırma kuralı

- Uygulama IIS Express / Visual Studio Debugger ile başlatılır. `dotnet run`
  proje kuralı gereği normal geliştirme akışında kullanılmaz; depodaki
  `baslat.bat` / `baslat.ps1` yalnızca hızlı Kestrel denemesi içindir.
