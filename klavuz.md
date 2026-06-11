# Geliştirici Kılavuzu — İş Yeri ↔ Ev Senkronizasyon Rehberi

> Bu dosya ArdCRM projesi üzerinden yazılmıştır.
> Diğer projelerine uyarlamak için "ArdCRM" yazan yerleri kendi proje adınla değiştir.

---

## 1. Bu Kılavuz Ne İçin?

İki farklı bilgisayarda (iş + ev), farklı SQL Server instance'larıyla,
yetki kısıtlamaları olan bir ortamda geliştirme yaparken:
- Hiç süre kaybı yaşamadan devam etmek
- Claude Code ile her oturumda sıfırdan başlamamak
- Git ile güvenli senkronizasyon sağlamak

---

## 2. Dosya Sistemi — Ne Nereye Gider?

```
GIT'E GİREN (her iki makinede aynı)     GİTİGNORE'DA (makineye özel)
────────────────────────────────────    ──────────────────────────────
CLAUDE.md                               appsettings.Work.json
klavuz.md                               appsettings.Home.json
.claude/settings.json                   appsettings.Development.json
docs/                                   bin/ obj/ .vs/
src/ (tüm C# kodu)                      *.user
docs/migrations/*.sql                   **/seed_*.sql
```

### Altın Kural
> Bağlantı bilgisi, şifre, IP adresi içeren HİÇBİR dosya git'e girmez.
> Sadece `appsettings.json` (içi boş template) git'e girer.

---

## 3. appsettings Yapısı

### appsettings.json (git'e girer — sadece şablon)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### appsettings.Work.json (gitignore — iş makinesinde)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=OZAY\\DATA;Database=ArdCRM;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### appsettings.Home.json (gitignore — ev makinesinde)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=ARDA\\ARDA;Database=ArdCRM;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### launchSettings.json (git'e girer)
```json
{
  "profiles": {
    "Work": {
      "commandName": "IISExpress",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Work"
      }
    },
    "Home": {
      "commandName": "IISExpress",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Home"
      }
    }
  }
}
```

### Program.cs'de Okuma Sırası
```csharp
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
```

---

## 4. Git Branch Stratejisi

```
main          → sadece kararlı, çalışan kod
  └── dev     → aktif geliştirme (her gün buradan çalış)
        ├── feature/musteri-modulu
        ├── feature/teklif-ekrani
        └── hotfix/baglanti-hatasi
```

### Günlük Rutin

**İşten ayrılırken (2 dakika):**
```bash
git add -A
git commit -m "WIP: [ne bıraktığını yaz]"
git push origin dev
```
> WIP = Work In Progress. Yarım kod commit etmek normaldir, utanılacak bir şey değil.

**Eve gelince (1 dakika):**
```bash
git pull origin dev
# VS'i aç, CLAUDE.md'yi oku, kaldığın yerden devam et
```

**Feature tamamlanınca:**
```bash
git checkout dev
git merge feature/musteri-modulu
git push origin dev
# Kararlı hale gelince: dev → main'e merge
```

---

## 5. CLAUDE.md — Claude Code'un Hafızası

### Nedir?
Claude Code her oturum açıldığında projenin kökündeki `CLAUDE.md` dosyasını okur.
Bu sayede her seferinde "4 katman mimarisi kullan, dotnet run yapma" gibi
şeyleri tekrar anlatmana gerek kalmaz.

### Neleri Koy?
- Mimari kararlar (4 katman, Generic Repository, ServiceResult<T>)
- Kritik kurallar (dotnet run yasak, ERP read-only, hardcoded string yok)
- Ortam bilgisi (hangi server, hangi DB)
- Son bırakılan nokta (WIP notu)
- Sık kullanılan komutlar

### WIP Notu Nasıl Kullanılır?
Her oturum kapanışında CLAUDE.md'deki şu bölümü güncelle:
```markdown
## Devam Edilecekler (WIP notu)
Son bırakılan yer: MusteriService.GetAll() yazıldı, test eksik.
Sonraki adım: MusteriController'da liste action'ı.
```

Bir sonraki gün (veya eve gelince) Claude Code'a şunu söyle:
```
CLAUDE.md'yi oku ve kaldığımız yerden devam edelim.
```

### .claude/settings.json
```json
{
  "project": "ArdCRM",
  "conventions": [
    "4-layer: Core/Data/Business/Web",
    "ServiceResult<T> for all service returns",
    "Generic Repository pattern",
    "No dotnet run - IIS Express only",
    "No hardcoded connection strings",
    "ERP is read-only"
  ]
}
```

---

## 6. Yeni Makineye Kurulum

### Repo bilgisi
```
GitHub : https://github.com/goktaso/ArdCRM.git
Branch : main (kararlı) / dev (aktif geliştirme)
```

### Adım 1 — Repoyu klonla
```powershell
git clone https://github.com/goktaso/ArdCRM.git
cd ArdCRM
git checkout dev
```

### Adım 2 — Ortam dosyasını oluştur (makineye göre seç)

**İş makinesi → `ArdCRM.Web\appsettings.Work.json` oluştur:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=OZAY\\DATA;Database=ArdCRM;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Ev makinesi → `ArdCRM.Web\appsettings.Home.json` oluştur:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=ARDA\\ARDA;Database=ArdCRM;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> Bu dosyalar .gitignore'dadır. Her makinede bir kez elle oluşturulur, bir daha dokunmana gerek yok.

### Adım 3 — Veritabanını oluştur
```powershell
dotnet ef database update --project ArdCRM.Data --startup-project ArdCRM.Web
```

Hata alırsan `docs\migrations\` klasöründeki SQL dosyalarını sırayla SSMS'de çalıştır.

### Adım 4 — Build al
```powershell
dotnet build ArdCRM.sln
```

### Adım 5 — Çalıştır
Visual Studio'da:
- Üst menüde profil seçici → **Work** veya **Home** seç
- **IIS Express** butonu ile başlat

> ⚠️ `dotnet run` KULLANMA. Her zaman VS içinden IIS Express ile çalıştır.

---

## 7. Şema Değişikliklerini Senkron Tutmak

Her veritabanı değişikliğinde:

```bash
# 1. EF Migration oluştur
dotnet ef migrations add MigrationAdi \
  --project ArdCRM.Data \
  --startup-project ArdCRM.Web

# 2. SQL scriptini çıkar ve docs/migrations/ klasörüne kaydet
dotnet ef migrations script \
  --project ArdCRM.Data \
  --startup-project ArdCRM.Web \
  --output docs/migrations/NNN_MigrationAdi.sql

# 3. Her iki makinede uygula
dotnet ef database update \
  --project ArdCRM.Data \
  --startup-project ArdCRM.Web
```

---

## 8. Yetki Kısıtlamaları ile Başa Çıkma

İş yerinde yaşanan kısıtlamalar için çözümler:

| Sorun | Çözüm |
|-------|-------|
| NuGet paketi yükleyemiyorum | Evde `packages/` klasörüne al, nuget.config ile local source ekle |
| Global .NET tool yükleyemiyorum | `dotnet-tools.json` (local manifest) kullan |
| Port açamıyorum | launchSettings'te farklı port dene (5001, 7001) |
| Admin gerektiren işlem | Evde geliştir, iş yerinde sadece `git pull` + çalıştır |
| VS Extension yükleyemiyorum | Claude Code terminal tabanlı çalışır, sorun olmaz |

---

## 9. Bu Kılavuzu Başka Projeye Uyarlamak

1. Bu dosyayı yeni projenin köküne kopyala
2. "ArdCRM" yazan her yeri yeni proje adıyla değiştir
3. Server isimlerini kontrol et (`OZAY\DATA` veya `ARDA\ARDA`)
4. Mimari farklıysa bölüm 5'teki kuralları güncelle
5. CLAUDE.md'yi projeye özel içerikle doldur

---

## 10. Özet — Günlük 3 Dakika Kuralı

### İşten/Evden ayrılırken — her satırı ayrı çalıştır:
```powershell
git add -A
git commit -m "WIP: ne biraktığını buraya yaz"
git push origin main
```

> ⚠️ PowerShell'de && çalışmaz — komutları ayrı satırlarda gir.

### Yeni makineye oturunca — kopyala çalıştır:
```powershell
git pull origin dev
```
Sonra VS'i aç, `CLAUDE.md`'yi oku, Claude Code'a şunu yaz:
```
CLAUDE.md'yi oku ve kaldığımız yerden devam edelim.
```

---

> Bu 3 dakikayı atlama. Atlarsan ertesi gün nereden kaldığını bulmak 30 dakika alır.
>
> Yarım kod commit etmek normaldir. `WIP:` prefix'i tam da bunun için var.
