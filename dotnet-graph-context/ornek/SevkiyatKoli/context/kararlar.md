# Proje Kararlari

Yalnizca kesinlesmis ve gelecekteki calismayi etkileyen kararlar kaydedilir.
Bir karar degisirse eski kayit silinmez; durumu guncellenir ve yeni karar eklenir.

| Tarih | Karar | Gerekce | Durum | Etkilenen alan |
|---|---|---|---|---|
| 2026-09-29 | Kurallar koddan once `context/` dosyalarina yazilir | Bu proje baglam sisteminin ornegi; kod kurali degil kural kodu belirler | aktif | tum proje |
| 2026-09-29 | Dogrulama `Sonuc`/`Sonuc<T>` ile bildirilir, exception ile degil | Kural ihlali beklenen bir durumdur; exception programlama hatasi icindir | aktif | Core, Business |
| 2026-09-29 | Model mutasyonu `internal` + `InternalsVisibleTo` ile yalniz Business'a acik | Modelin disaridan keyfi degistirilmesini engellemek | aktif | Core.csproj |
| 2026-09-29 | Iptal edilen koli numarasi serbest birakilmaz, kayit silinmez | Izlenebilirlik: hangi numaranin neden bos oldugu gorulebilmeli | aktif | Sevkiyat, KoliService |
| 2026-09-29 | "Sira atlanmaz" kurali uretim sirasi icindir, aktif koli listesi icin degil | Kural 2 ile Kural 5 ilk bakista celisiyor; ayrim yazili olmazsa her oturumda yeniden tartisilir | aktif | kurallar-ve-sinirlar.md |
| 2026-09-29 | Dara sabit 0.8 kg ve brut agirliga dahil | "Brut" kelimesi darayi ima eder; tanim yazili olmazsa sinir kontrolu tutarsizlasir | aktif | Koli, KoliService |

