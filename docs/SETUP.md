# ArdCRM — Yeni Makine Kurulum Rehberi

Bu rehber, projeyi yeni bir bilgisayarda sıfırdan ayağa kaldırmak için kullanılır.

## Gereksinimler
- .NET 8 SDK
- SQL Server (Express yeterli)
- Visual Studio 2022 veya VS Code
- Git

## Adımlar

### 1. Repoyu Klonla
```bash
git clone https://github.com/goktaso/ArdCRM.git
cd ArdCRM
```

### 2. Ortam Dosyasını Oluştur
Bu makine iş makinesi ise `appsettings.Work.json` oluştur:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=OZAY\\DATA;Database=ArdCRM;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Bu makine ev makinesi ise `appsettings.Home.json` oluştur:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=ARDA\\ARDA;Database=ArdCRM;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> Bu dosyalar .gitignore'dadır. Her makinede elle oluşturulur.

### 3. Veritabanını Oluştur
```bash
dotnet ef database update \
  --project ArdCRM.Data \
  --startup-project ArdCRM.Web
```

Hata alırsan önce migration script'lerini sırayla uygula:
```
docs/migrations/001_initial.sql
docs/migrations/002_...sql
...
```

### 4. Build Al ve Çalıştır
```bash
dotnet build ArdCRM.sln
```

Visual Studio'da:
- Profil olarak **Work** veya **Home** seç
- **IIS Express** ile başlat (dotnet run KULLANMA)

### 5. Kontrol
Tarayıcıda `https://localhost:[port]/` açılıyorsa kurulum tamam.
