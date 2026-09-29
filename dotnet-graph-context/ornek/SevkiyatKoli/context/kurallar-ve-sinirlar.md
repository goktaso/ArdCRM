# Kurallar ve Sınırlar — SevkiyatKoli

Kural numaraları kodda ve testlerde aynı numarayla anılır
(`KoliService.cs` başlığı, `KoliServiceTests.cs` bölüm başlıkları).

## Değişmez iş kuralları

1. **Numara formatı** `{IrsaliyeNo}-{Sira:000}` → `SEV2026001-003`.
   Sıra 999'u aşarsa basamak büyür (`-1000`), format bozulmaz.
2. **Sıra numarası sevkiyat içinde 1'den başlar ve üretimde atlanmaz.**
   Her sevkiyatın kendi sayacı vardır.
3. **Kapalı veya iptal koliye satır eklenemez.** Düzeltmenin tek yolu:
   koliyi iptal et, yeni koli aç.
4. **Brüt ağırlık 30 kg'ı aşamaz.** Brüt = dara (0.8 kg) + satır ağırlıkları.
   Sınırı aşan satır reddedilir; koli değişmeden kalır.
5. **Üretilen numara tekrar kullanılmaz** — koli iptal edilse bile. Sayaç geri alınmaz.

## Kural 2 ile Kural 5 arasındaki görünür çelişki

İptal edilen koli kaydı silinmez ve numarası boşta kalmaz. Bu yüzden aktif koli
listesinde numara **boşluğu** görülebilir (001 iptal → aktifler 002'den başlar).
Bu bir hata değildir: "atlanmaz" kuralı **üretim sırası** içindir, aktif koli
listesi için değil. Bkz. `kararlar.md` 2026-09-29 kaydı.

## Mimari sınırlar

- `Core` hiçbir projeye referans vermez.
- Model mutasyonu yalnızca `Business` katmanından yapılır (`InternalsVisibleTo`).
- Tüm servis dönüşleri `Sonuc` / `Sonuc<T>`'dir; kural ihlali exception ile değil
  `Basarili = false` ile bildirilir. Exception yalnızca programlama hatası içindir
  (örn. `KoliNumaratoru.Uret` geçersiz argüman).

## Güvenlik ve veri sınırları

- Gerçek müşteri/sevkiyat verisi bu repoya veya bağlam dosyalarına yazılmaz.
  Testlerde `SEV2026001`, `STK-1` gibi kurgusal değerler kullanılır.

## Kullanıcı onayı gerektiren işlemler

- Ağırlık sınırının (30 kg) veya numara formatının değiştirilmesi.
- Kapalı koli düzeltme yolunun değiştirilmesi (ör. "yeniden açma" eklenmesi).
