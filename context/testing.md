# Doğrulama ve Kabul Kriterleri — ArdCRM

Aşağıdaki komutlar depodaki gerçek dosyalardan tespit edilmiştir
(`ArdCRM.sln`, `ArdCRM.Tests/ArdCRM.Tests.csproj`, `ArdCRM.Web/Properties/launchSettings.json`).
Komutlar Windows geliştirme makinesinde .NET 8 SDK ile çalıştırılır.

## Standart kontroller

| Amaç | Komut |
|---|---|
| Derleme (tüm çözüm) | `dotnet build ArdCRM.sln` |
| Test (tüm test projesi) | `dotnet test ArdCRM.Tests/ArdCRM.Tests.csproj` |
| Test (çözüm üzerinden) | `dotnet test ArdCRM.sln` |
| Tek test sınıfı | `dotnet test ArdCRM.Tests/ArdCRM.Tests.csproj --filter FullyQualifiedName~MusteriServiceTests` |
| Kapsam (coverlet yüklü) | `dotnet test ArdCRM.Tests/ArdCRM.Tests.csproj --collect:"XPlat Code Coverage"` |

- Statik analiz/lint: projede yapılandırılmış bir lint/analyzer kuralı **yok**
  (`.editorconfig`, StyleCop veya özel analyzer paketi bulunmuyor). Varsayılan
  derleyici uyarıları tek kontroldür; `Nullable` tüm projelerde `enable`.
- CI: depoda yapılandırılmış bir CI iş akışı yok (`.github/workflows` bulunmuyor).

## Test yığını

- xUnit 2.4.2 + Moq 4.20.69 + coverlet.collector 6.0.0, `Microsoft.NET.Test.Sdk` 17.6.0.
- Test projesi yalnızca `ArdCRM.Business` ve `ArdCRM.Core` projelerine referans verir;
  veritabanı gerektirmez, `IRepository<T>` mock'lanır.
- Mevcut test dosyaları: `MusteriServiceTests.cs` (13), `TeklifServiceTests.cs` (13),
  `UnitTest1.cs` (1) → toplam 27 test.
- Son bilinen sonuç: 2026-06-11, 27/27 yeşil (kaynak: `CLAUDE.md` geliştirme durumu).
  Bu bağlam dosyası oluşturulurken build/test bu ortamda çalıştırılmadı.

## Uygulamayı çalıştırma (manuel doğrulama)

- Normal akış: Visual Studio → `Work` veya `Home` profili → **IIS Express**.
- Hızlı Kestrel denemesi: `baslat.bat` (Work profili, `http://localhost:5050`).
- Bağlantı dizesi ortam değişkeni veya gitignore'daki `appsettings.{Ortam}.json`
  ile verilir; komut satırına veya bağlam dosyalarına yazılmaz.

## Göreve özel kabul kriterleri

- Business katmanında değişiklik → ilgili servis testi eklenir/güncellenir ve
  `dotnet test` yeşil olmalıdır.
- Yeni doğrulama kuralı → FluentValidation validator'ında tanımlanır, testle kanıtlanır.
- Şema değişikliği → EF Core migration **ve** `docs/migrations/NNN_*.sql` script'i birlikte eklenir.
- ERP adapter değişikliği → birim testle mock üzerinden doğrulanır; canlı ERP'ye
  sorgu atan doğrulama kullanıcı onayı ister.
- UI değişikliği → ilgili sayfa IIS Express ile açılarak manuel doğrulanır; testle
  kapsanamayan doğrulama "manuel doğrulandı" olarak raporlanır.

## Hata geri dönüş kuralı

Bir test veya derleme hatasında tüm projeyi yeniden yorumlama. Önce hata mesajı ile
değişen alanın bağlantısını kur; ardından yalnızca ilgili `context/domains/*.md`,
`kararlar.md` veya `kurallar-ve-sinirlar.md` dosyasına dön. İki denemede çözülmeyen
hatada `tasks/todo.md` CHECKPOINT'ine engel olarak yaz.
