# ArdCRM - Uygulama Başlatma Scripti
# Kullanım: PowerShell'de ./baslat.ps1 veya çift tıkla

$env:ConnectionStrings__DefaultConnection = "Server=OZAY\DATA;Database=ArdCRM;Trusted_Connection=True;TrustServerCertificate=True;"

Write-Host "ArdCRM baslatiliyor..." -ForegroundColor Cyan
Write-Host "Adres: http://localhost:5050" -ForegroundColor Green
Write-Host "Durdurmak icin: Ctrl+C" -ForegroundColor Yellow
Write-Host ""

dotnet run --project ArdCRM.Web --launch-profile Work-Kestrel
