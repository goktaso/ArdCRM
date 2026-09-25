# Proje Kararları — ArdCRM

Yalnızca kesinleşmiş ve gelecekteki çalışmayı etkileyen kararlar kaydedilir.
Bir karar değişirse eski kayıt silinmez; durumu güncellenir ve yeni karar eklenir.

| Tarih | Karar | Gerekçe | Durum | Etkilenen alan |
|---|---|---|---|---|
| 2026-06-11 | 4 katmanlı mimari: Core / Data / Business / Web | Bağımlılık yönünü tek yönlü tutup test edilebilirliği korumak | aktif | tüm çözüm |
| 2026-06-11 | Business dönüşleri `ServiceResult` / `ServiceResult<T>` | Controller'da exception akışı yerine açık başarı/hata sözleşmesi | aktif | Business, Web |
| 2026-06-11 | Generic Repository (`IRepository<T>`) + EF Core | Tekrarlayan CRUD kodunu tek yerde toplamak | aktif | Data, Business |
| 2026-06-11 | Silme işlemi soft delete (`Aktif = false`) | Cari ve teklif geçmişinin kaybolmaması | aktif | Data, tüm listeler |
| 2026-06-11 | ArdCRM veritabanı EF Core, ERP okuması Dapper | ERP şeması bizim değil; POCO mapping yerine hızlı ham sorgu | aktif | Data/Adapters |
| 2026-06-11 | ERP erişimi `IErpAdapter` arkasında ve salt okunur | ERP'ye yazmanın veri bütünlüğü riski; farklı ERP'lere geçiş kolaylığı | aktif | Core/Interfaces, Data/Adapters |
| 2026-06-11 | ERP bağlantısı yoksa `NullErpAdapter` DI'a kaydedilir | Uygulamanın ERP'siz ortamda (ev makinesi) çalışabilmesi | aktif | Web/Program.cs |
| 2026-06-11 | Şema değişiklikleri `docs/migrations/` altında sıralı SQL script olarak da tutulur | Yetki kısıtlı SQL Server ortamlarında `dotnet ef` çalıştırılamıyor | aktif | Data/Migrations, docs/migrations |
| 2026-06-11 | Ortam ayrımı `Work` / `Home` profilleri + gitignore'daki appsettings | İş ve ev makinelerinde farklı SQL instance; bağlantı dizesi repoya girmez | aktif | Web/Properties/launchSettings.json |
| 2026-06-11 | DataProtection anahtarları proje klasöründe (`DataProtection-Keys/`) | Varsayılan profil konumunda yetki hatası alınması | aktif | Web/Program.cs |
| 2026-06-11 | Doğrulama FluentValidation ile Business katmanında | Kuralların controller'lara dağılmasını engellemek | aktif | Business/Validators |
| 2026-06-11 | Test yığını xUnit + Moq, servis seviyesinde birim test | Repository mock'lanarak veritabanı olmadan doğrulama | aktif | ArdCRM.Tests |
| 2026-06-11 | Uygulama IIS Express / VS Debugger ile çalıştırılır | Windows Auth ve IIS davranışının geliştirme ortamında birebir olması | aktif | çalıştırma akışı |
| 2026-09-25 | Graph Context bağlam sistemi (`context/` + `tasks/` + router) devreye alındı | Her oturumda tüm dokümanı okumak yerine göreve göre bağlam seçmek | aktif | CLAUDE.md, AGENTS.md, context/, tasks/ |
| 2026-09-25 | CHECKPOINT/WIP kaydının tek yeri `tasks/todo.md`'dir | `CLAUDE.md` içindeki "Devam Edilecekler" ile çift kayıt oluşmasını önlemek | aktif | CLAUDE.md, tasks/todo.md |
