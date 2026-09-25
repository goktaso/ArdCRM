# Domain — ERP Senkronizasyonu (Netsis / Logo)

## Sorumluluk

ERP veritabanından cari (müşteri) ve stok hareketi verisini **salt okunur** biçimde
getirmek; kullanıcıya önizleme sunmak ve seçilen kayıtları ArdCRM'e aktarmak.

## Değişmez kural

**ERP'ye asla yazma yapılmaz.** `IErpAdapter` yalnızca okuma metodları tanımlar;
yeni bir metot eklenirken de bu sınır korunur. Bu kural `kurallar-ve-sinirlar.md`
ve `kararlar.md` (2026-06-11) ile bağlayıcıdır.

## Giriş / çıkışlar

- Giriş: `NetsisConnection` bağlantı dizesi (gitignore'daki appsettings veya ortam değişkeni),
  kullanıcının önizleme ekranında seçtiği cari kodları.
- Çıkış: `ServiceResult<IEnumerable<ErpMusteri>>` (önizleme, DB'ye yazmaz),
  `ServiceResult<ErpSenkronSonuc>` (`YeniEklenen`, `Guncellenen`, `Atlanan`, `Hatalar`).

## Akış

1. `TestConnectionAsync()` — bağlantı doğrulanır, başarısızsa `Fail` döner.
2. `OnizlemeGetirAsync()` — ERP carileri listelenir, **yazma yok**.
3. Kullanıcı cari kodlarını seçer.
4. `MusterileriAktarAsync(cariKodlar)` — her kod için ERP'den kayıt okunur,
   ArdCRM'de varsa güncellenir, yoksa eklenir, bulunamazsa `Atlanan` sayılır.

## Temel kurallar

- `NetsisConnection` tanımlı değilse DI `NullErpAdapter` kaydeder; uygulama ERP'siz
  ortamda (ev makinesi) sorunsuz çalışmalıdır (`ArdCRM.Web/Program.cs`).
- ERP okuması Dapper ile yapılır; ERP şeması EF Core modeline alınmaz.
- Adapter içindeki sütun adları tek bir Netsis kurulumuna göre doğrulanmıştır;
  sorgu/sütun değişikliği kullanıcı onayı gerektirir.
- ERP'den gelen ham veri (`ErpMusteri`, `ErpStokHareketi`) Core'da DTO olarak durur;
  doğrudan entity olarak kaydedilmez.
- Gerçek müşteri verisi loglara, test fixture'larına veya bağlam dosyalarına kopyalanmaz.

## İlgili kod konumları

- Sözleşme: `ArdCRM.Core/Interfaces/IErpAdapter.cs`, `IErpSenkronService.cs`
- Adapter: `ArdCRM.Data/Adapters/NetsisAdapter.cs`, `NullErpAdapter.cs`
- Servis: `ArdCRM.Business/Services/ErpSenkronService.cs`
- Web: `ArdCRM.Web/Controllers/ErpController.cs`, `Views/Erp/Index`, `Views/Erp/Onizleme`
- DI: `ArdCRM.Web/Program.cs` (NetsisConnection varsa `NetsisAdapter`, yoksa `NullErpAdapter`)

## Doğrulama

- Birim test: `IErpAdapter` mock'lanır; canlı ERP'ye bağlanan test yazılmaz.
- Manuel: ERP ekranında önizleme alınır, ardından küçük bir seçimle aktarım denenir.
- Canlı ERP'ye sorgu atan doğrulama öncesi kullanıcı onayı alınır.
