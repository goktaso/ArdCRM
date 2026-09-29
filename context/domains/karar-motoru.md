# Domain — Karar Motoru

## Sorumluluk

Küçük, tekrarlayan, kapalı kümeli kararları tek bir sözleşme arkasında toplamak:
eşleşme (evet/hayır), sınıflandırma (seçim), risk/öncelik (puan).

Amaç iki katmanlıdır:
1. Bu kararlar bugün kod içine dağılmış `if/else`'lerde duruyor → tek yerde toplanır, test edilir.
2. Yarın bir karar modeli (JEV / TypeSafe System One) devreye alınmak istenirse
   **kod değişmeden** takılabilir.

## Değişmez kurallar

- **Hassas veri dış sağlayıcıya çıkmaz.** `KararIstegi.Ozellikler` yalnızca türetilmiş
  öznitelik taşır (`adBenzerlik: 0.82`, `vergiNoEslesti: true`). Ham cari adı, vergi no,
  e-posta, telefon, adres, serbest metin **gönderilemez**.
  Bu kural `HassasVeriFiltresi` ile makine tarafından uygulanır — tek yerde, her çağrıda.
- **Kural motoru daima vardır.** JEV birincil olsa bile `KuralTabanliKararMotoru` yedek
  olarak kalır. Dış sağlayıcı kapalı/yavaş/hatalıyken sistem durmaz.
- **Yedeğe düşme sessiz değildir.** Sonuç `KararKaynagi.Yedek` ile işaretlenir ve
  birincil motorun hatası mesajda taşınır.
- **JEV şema kilidi.** `Jev:SemaDogrulandi = false` olduğu sürece JEV adapter'ı
  **hiçbir ağ çağrısı yapmaz**. Resmî `/v1/systemone` şeması doğrulanmadan true yapılmaz.
- **Ölçmeden geçiş yok.** JEV'e geçiş kararı, aynı istekler üzerinde kural motoruyla
  karşılaştırma yapılmadan verilmez.
- Seçim kararları **kapalı küme** dışına çıkamaz; motor küme dışı değer dönerse hata verilir.
- Puan kararları tanımlı aralık dışına çıkamaz.

## Akış

```text
Çağıran servis
   → KararIstegi (soruAnahtari + soru + türetilmiş öznitelikler)
      → IKararMotoru
         ├─ JevKararMotoru      (Jev:ApiAnahtari varsa, şema kilidi açıksa)
         │     └─ HassasVeriFiltresi → HTTP POST /v1/systemone
         └─ KuralTabanliKararMotoru  (her zaman; birincil başarısızsa yedek)
      ← ServiceResult<EvetHayirKarari | SecimKarari | PuanKarari>
```

DI davranışı `IErpAdapter` ile birebir aynıdır: bağlantı tanımlıysa dış adapter,
değilse yerel karşılığı.

## Tanımlı kararlar

| Soru anahtarı | Tip | Öznitelikler | Ne yapar |
|---|---|---|---|
| `erp.cari-eslesme` | evet/hayır | `adBenzerlik`, `vergiNoEslesti`, `sehirEslesti` | ERP carisi ArdCRM müşterisiyle aynı mı |
| `guardrail.erp-yazma-girisimi` | evet/hayır | `yazmaAnahtarKelimesiVar`, `hedefErp` | "ERP salt okunur" sınırının koruyucusu |
| `teklif.oncelik` | puan (1-5) | `tutarOrani`, `gecenGun`, `musteriAktif` | Teklif takip önceliği |

Yeni karar eklemek: `VarsayilanKurallar.Olustur()` içine kural + bu tabloya satır +
`ArdCRM.Tests/KararMotoruTests.cs` içine en az bir test.

## İlgili kod konumları

- Sözleşme: `ArdCRM.Core/Interfaces/IKararMotoru.cs`
- Hassas veri filtresi: `ArdCRM.Core/Kararlar/HassasVeriFiltresi.cs`
- Kural kataloğu: `ArdCRM.Data/Adapters/Karar/KuralKatalogu.cs`
- Kural motoru: `ArdCRM.Data/Adapters/Karar/KuralTabanliKararMotoru.cs`
- Varsayılan kurallar: `ArdCRM.Data/Adapters/Karar/VarsayilanKurallar.cs`
- JEV adapter (kapalı): `ArdCRM.Data/Adapters/Karar/JevKararMotoru.cs`, `JevSecenekleri.cs`
- Yedekleme: `ArdCRM.Data/Adapters/Karar/YedekliKararMotoru.cs`
- DI: `ArdCRM.Web/Extensions/KararMotoruServiceCollectionExtensions.cs`, `Program.cs`
- Testler: `ArdCRM.Tests/KararMotoruTests.cs`

## Yapılandırma

`appsettings.{Ortam}.json` (gitignore'da) veya ortam değişkeni:

```json
{
  "Jev": {
    "ApiAnahtari": "",
    "SemaDogrulandi": false
  }
}
```

API anahtarı repoya, bu dosyaya veya herhangi bir bağlam dosyasına yazılmaz.

## JEV adapter'ını açma adımları

1. `console.typesafe.ai` üzerinden anahtar alın (resmî domain; benzer adlı siteler değil).
2. Resmî dokümandaki istek gövdesiyle `CagirAsync` içindeki `govde` alan adlarını eşleştirin.
3. Yanıt gövdesiyle `OndalikOku` / `MetinOku` alan adaylarını eşleştirin.
4. Kimlik doğrulama başlığını doğrulayın (şu an `Bearer` varsayılıyor).
5. Tek bir gerçek çağrıyla doğrulayın, sonucu `context/kararlar.md`ye yazın.
6. `Jev:SemaDogrulandi = true` yapın.
7. En az bir hafta gölge modda çalıştırıp kural motoruyla karşılaştırın; ancak ondan
   sonra birincil yapın.

## Doğrulama

```bash
dotnet test ArdCRM.Tests/ArdCRM.Tests.csproj --filter FullyQualifiedName~KararMotoru
dotnet test ArdCRM.Tests/ArdCRM.Tests.csproj --filter FullyQualifiedName~HassasVeriFiltresi
```

Dış servise bağlanan test yazılmaz; JEV adapter'ı şema kilidi kapalıyken test edilir.
