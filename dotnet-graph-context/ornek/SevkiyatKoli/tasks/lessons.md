# Kalici Dersler

Yalnizca tekrar edilmesi muhtemel, dogrulanmis teknik veya surec derslerini
kaydedin. Gecici sohbet notlarini buraya koymayin.

## 2026-09-29 - internal uyeler assembly sinirini gecmez

- Baglam: `Koli.SatirEkle`, `Sevkiyat.SonrakiSiraNoUret` gibi mutasyon metotlari
  Core'da `internal` yazilmisti; Business katmani derlenmedi (CS1061 x5).
- Ders: Modelin mutasyon API'sini disariya kapali tutmak icin `internal` doğru
  tercih; ancak Core.csproj'a `<InternalsVisibleTo Include="...Business" />`
  eklenmeli. Uyeleri `public` yapmak kolay ama kapsullemeyi bozar.
- Kanit: `SevkiyatKoli.Core/SevkiyatKoli.Core.csproj`, ilk derleme hatasi CS1061.

## 2026-09-29 - Iki kural celisiyor gorunuyorsa ayrimi yaziya dok

- Baglam: "Sira atlanmaz" (Kural 2) ile "numara tekrar kullanilmaz" (Kural 5)
  iptal senaryosunda celisik duruyordu.
- Ders: Celiski gercek degil, tanim eksikligiydi. Ayrim `kurallar-ve-sinirlar.md`
  icine ve `kararlar.md`ye yazildi; boylece her oturumda yeniden tartisilmiyor.
- Kanit: `context/kurallar-ve-sinirlar.md` -> "Kural 2 ile Kural 5 arasindaki
  gorunur celiski", test `Kural5_IptalSonrasi_SevkiyattaNumaraBoslugu_Gorunur`.

