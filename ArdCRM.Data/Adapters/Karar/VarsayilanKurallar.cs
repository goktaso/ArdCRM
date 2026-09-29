using ArdCRM.Core.Interfaces;

namespace ArdCRM.Data.Adapters.Karar;

/// <summary>
/// ArdCRM'in başlangıç kural kümesi.
///
/// Her kural deterministiktir ve yalnızca türetilmiş özniteliklerle çalışır —
/// böylece aynı istek JEV'e de gönderilebilir ve iki motorun kararı
/// karşılaştırılabilir. Ölçüm yapılmadan JEV'e geçilmez.
/// </summary>
public static class VarsayilanKurallar
{
    /// <summary>ERP'den gelen cari, ArdCRM'deki müşteriyle aynı mı?</summary>
    public const string ErpCariEslesme = "erp.cari-eslesme";

    /// <summary>Bir metin/komut ERP'ye yazma girişimi içeriyor mu? (guardrail)</summary>
    public const string ErpYazmaGirisimi = "guardrail.erp-yazma-girisimi";

    /// <summary>Teklifin takip önceliği.</summary>
    public const string TeklifOncelik = "teklif.oncelik";

    public static KuralKatalogu Olustur()
    {
        var katalog = new KuralKatalogu();

        // --- erp.cari-eslesme -------------------------------------------------
        // Öznitelikler: adBenzerlik (0..1), vergiNoEslesti (bool), sehirEslesti (bool)
        // Vergi no eşleşmesi tek başına belirleyicidir; ad benzerliği destekleyicidir.
        katalog.EvetHayirKaydet(ErpCariEslesme, istek =>
        {
            var adBenzerlik    = KuralKatalogu.SayiOku(istek, "adBenzerlik");
            var vergiNoEslesti = KuralKatalogu.MantikOku(istek, "vergiNoEslesti");
            var sehirEslesti   = KuralKatalogu.MantikOku(istek, "sehirEslesti");

            double olasilik;
            string gerekce;

            if (vergiNoEslesti)
            {
                olasilik = 0.97d;
                gerekce  = "Vergi numarası eşleşti.";
            }
            else if (adBenzerlik >= 0.90d && sehirEslesti)
            {
                olasilik = 0.85d;
                gerekce  = "Ad benzerliği yüksek ve şehir aynı.";
            }
            else if (adBenzerlik >= 0.90d)
            {
                olasilik = 0.65d;
                gerekce  = "Ad benzerliği yüksek, şehir doğrulanmadı.";
            }
            else if (adBenzerlik >= 0.75d && sehirEslesti)
            {
                olasilik = 0.55d;
                gerekce  = "Ad benzerliği orta, şehir aynı.";
            }
            else
            {
                olasilik = 0.10d;
                gerekce  = "Yeterli kanıt yok.";
            }

            return new EvetHayirKarari
            {
                Karar    = olasilik >= 0.5d,
                Olasilik = olasilik,
                Kaynak   = KararKaynagi.Kural,
                Gerekce  = gerekce
            };
        });

        // --- guardrail.erp-yazma-girisimi ------------------------------------
        // Öznitelikler: yazmaAnahtarKelimesiVar (bool), hedefErp (bool)
        // CLAUDE.md kuralı: ERP salt okunurdur. Bu kural o sınırın koruyucusudur.
        katalog.EvetHayirKaydet(ErpYazmaGirisimi, istek =>
        {
            var yazmaVar = KuralKatalogu.MantikOku(istek, "yazmaAnahtarKelimesiVar");
            var hedefErp = KuralKatalogu.MantikOku(istek, "hedefErp");

            var riskli   = yazmaVar && hedefErp;
            var olasilik = riskli ? 0.95d : (yazmaVar ? 0.40d : 0.02d);

            return new EvetHayirKarari
            {
                Karar    = riskli,
                Olasilik = olasilik,
                Kaynak   = KararKaynagi.Kural,
                Gerekce  = riskli
                    ? "ERP hedefli yazma ifadesi tespit edildi — engellenmeli."
                    : "ERP hedefli yazma ifadesi yok."
            };
        });

        // --- teklif.oncelik ---------------------------------------------------
        // Öznitelikler: tutarOrani (0..1), gecenGun (int), musteriAktif (bool)
        katalog.PuanKaydet(TeklifOncelik, istek =>
        {
            var tutarOrani   = KuralKatalogu.SayiOku(istek, "tutarOrani");
            var gecenGun     = KuralKatalogu.SayiOku(istek, "gecenGun");
            var musteriAktif = KuralKatalogu.MantikOku(istek, "musteriAktif");

            var puan = 1;
            if (tutarOrani   >= 0.50d) puan++;
            if (tutarOrani   >= 0.80d) puan++;
            if (gecenGun     >= 7d)    puan++;
            if (musteriAktif)          puan++;

            if (puan < istek.EnDusuk)  puan = istek.EnDusuk;
            if (puan > istek.EnYuksek) puan = istek.EnYuksek;

            return new PuanKarari
            {
                Puan    = puan,
                Guven   = 1.0d,
                Kaynak  = KararKaynagi.Kural,
                Gerekce = $"tutarOrani={tutarOrani:F2}, gecenGun={gecenGun:F0}, musteriAktif={musteriAktif}"
            };
        });

        return katalog;
    }
}
