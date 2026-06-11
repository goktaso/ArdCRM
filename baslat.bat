@echo off
echo ArdCRM baslatiliyor...
echo Adres: http://localhost:5050
echo Durdurmak icin: Ctrl+C
echo.
set ConnectionStrings__DefaultConnection=Server=OZAY\DATA;Database=ArdCRM;Trusted_Connection=True;TrustServerCertificate=True;
dotnet run --project ArdCRM.Web --launch-profile Work-Kestrel
pause
