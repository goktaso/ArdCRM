# Kalıcı Dersler — ArdCRM

Yalnızca tekrar edilmesi muhtemel, doğrulanmış teknik veya süreç dersleri kaydedilir.
Geçici sohbet notları buraya konmaz.

## 2026-06-11 - DataProtection anahtarları profil klasöründe yazılamıyor

- Bağlam: Yetki kısıtlı Windows kullanıcısında uygulama açılışta DataProtection
  anahtarını varsayılan konuma yazamıyordu.
- Ders: Anahtarlar `ContentRootPath/DataProtection-Keys` altına yönlendirilir ve bu
  klasör gitignore'da tutulur.
- Kanıt: `ArdCRM.Web/Program.cs` → `PersistKeysToFileSystem(...)`, `.gitignore`.

## 2026-06-11 - Yetki kısıtlı SQL ortamında `dotnet ef database update` güvenilmez

- Bağlam: İş makinesindeki SQL Server instance'ında migration komutu çalıştırılamadı.
- Ders: Her şema değişikliği ayrıca `docs/migrations/NNN_*.sql` olarak da üretilir ve
  SSMS ile uygulanır.
- Kanıt: `docs/migrations/001_InitialCreate.sql`, `docs/SETUP.md` 3. adım.

## 2026-06-11 - ERP bağlantısı olmayan makinede uygulama ayağa kalkmalı

- Bağlam: Ev makinesinde Netsis veritabanı yok; ERP ekranı uygulamayı kırıyordu.
- Ders: `NetsisConnection` boşsa DI `NullErpAdapter` kaydeder; ERP'ye bağımlı kod
  her zaman adapter arkasından çağrılır.
- Kanıt: `ArdCRM.Web/Program.cs`, `ArdCRM.Data/Adapters/NullErpAdapter.cs`.
