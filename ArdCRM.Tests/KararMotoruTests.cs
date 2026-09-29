using ArdCRM.Core.Interfaces;
using ArdCRM.Core.Kararlar;
using ArdCRM.Data.Adapters.Karar;

namespace ArdCRM.Tests;

public class KuralTabanliKararMotoruTests
{
    private static KuralTabanliKararMotoru Motor() =>
        new(VarsayilanKurallar.Olustur());

    private static KararIstegi CariIstegi(double adBenzerlik, bool vergiNoEslesti, bool sehirEslesti) =>
        new()
        {
            SoruAnahtari = VarsayilanKurallar.ErpCariEslesme,
            Soru         = "ERP carisi ile ArdCRM musterisi ayni mi?",
            Ozellikler   = new Dictionary<string, object?>
            {
                ["adBenzerlik"]    = adBenzerlik,
                ["vergiNoEslesti"] = vergiNoEslesti,
                ["sehirEslesti"]   = sehirEslesti
            }
        };

    [Fact]
    public async Task VergiNoEslesince_EslesmeKarariVerir()
    {
        var sonuc = await Motor().EvetHayirAsync(CariIstegi(0.10d, true, false));

        Assert.True(sonuc.Success);
        Assert.True(sonuc.Data!.Karar);
        Assert.True(sonuc.Data.Olasilik > 0.9d);
        Assert.Equal(KararKaynagi.Kural, sonuc.Data.Kaynak);
    }

    [Fact]
    public async Task ZayifKanitta_EslesmeYokKarariVerir()
    {
        var sonuc = await Motor().EvetHayirAsync(CariIstegi(0.40d, false, false));

        Assert.True(sonuc.Success);
        Assert.False(sonuc.Data!.Karar);
        Assert.True(sonuc.Data.Olasilik < 0.5d);
    }

    [Fact]
    public async Task YuksekAdBenzerligiVeAyniSehir_EslesmeKarariVerir()
    {
        var sonuc = await Motor().EvetHayirAsync(CariIstegi(0.92d, false, true));

        Assert.True(sonuc.Success);
        Assert.True(sonuc.Data!.Karar);
    }

    [Fact]
    public async Task TanimsizSoruAnahtari_HataDoner()
    {
        var sonuc = await Motor().EvetHayirAsync(new KararIstegi
        {
            SoruAnahtari = "olmayan.kural",
            Soru         = "?"
        });

        Assert.False(sonuc.Success);
        Assert.Contains("tanımlı değil", sonuc.Message);
    }

    [Fact]
    public async Task ErpYazmaGirisimi_Guardrail_RiskliyiYakalar()
    {
        var sonuc = await Motor().EvetHayirAsync(new KararIstegi
        {
            SoruAnahtari = VarsayilanKurallar.ErpYazmaGirisimi,
            Soru         = "Bu islem ERP'ye yazma girisimi mi?",
            Ozellikler   = new Dictionary<string, object?>
            {
                ["yazmaAnahtarKelimesiVar"] = true,
                ["hedefErp"]                = true
            }
        });

        Assert.True(sonuc.Success);
        Assert.True(sonuc.Data!.Karar);
    }

    [Fact]
    public async Task TeklifOncelik_AraliginIcindeKalir()
    {
        var sonuc = await Motor().PuanAsync(new PuanIstegi
        {
            SoruAnahtari = VarsayilanKurallar.TeklifOncelik,
            Soru         = "Bu teklifin takip onceligi nedir?",
            EnDusuk      = 1,
            EnYuksek     = 5,
            Ozellikler   = new Dictionary<string, object?>
            {
                ["tutarOrani"]   = 0.95d,
                ["gecenGun"]     = 30,
                ["musteriAktif"] = true
            }
        });

        Assert.True(sonuc.Success);
        Assert.InRange(sonuc.Data!.Puan, 1, 5);
    }

    [Fact]
    public async Task GecersizPuanAraligi_HataDoner()
    {
        var sonuc = await Motor().PuanAsync(new PuanIstegi
        {
            SoruAnahtari = VarsayilanKurallar.TeklifOncelik,
            Soru         = "?",
            EnDusuk      = 5,
            EnYuksek     = 1
        });

        Assert.False(sonuc.Success);
    }

    [Fact]
    public async Task BosSecenekListesi_HataDoner()
    {
        var sonuc = await Motor().SecimAsync(new SecimIstegi
        {
            SoruAnahtari = "her.hangi",
            Soru         = "?",
            Secenekler   = Array.Empty<string>()
        });

        Assert.False(sonuc.Success);
    }
}

public class HassasVeriFiltresiTests
{
    [Fact]
    public void TuretilmisOznitelikler_Gecerli()
    {
        var ihlaller = new HassasVeriFiltresi().Dogrula(new Dictionary<string, object?>
        {
            ["adBenzerlik"]    = 0.82d,
            ["vergiNoEslesti"] = true,
            ["sehirEslesti"]   = true
        });

        Assert.Empty(ihlaller);
    }

    [Theory]
    [InlineData("vergiNo")]
    [InlineData("vergi_no")]
    [InlineData("VERGI NO")]
    [InlineData("cariAdi")]
    [InlineData("telefon")]
    [InlineData("connectionString")]
    public void HassasAlanAdi_Reddedilir(string anahtar)
    {
        var ihlaller = new HassasVeriFiltresi().Dogrula(new Dictionary<string, object?>
        {
            [anahtar] = "deger"
        });

        Assert.NotEmpty(ihlaller);
    }

    [Fact]
    public void EpostaIcerenDeger_Reddedilir()
    {
        var ihlaller = new HassasVeriFiltresi().Dogrula(new Dictionary<string, object?>
        {
            ["kaynak"] = "ozay@ornek.com"
        });

        Assert.NotEmpty(ihlaller);
    }

    [Fact]
    public void OnHaneliNumaraIcerenDeger_Reddedilir()
    {
        var ihlaller = new HassasVeriFiltresi().Dogrula(new Dictionary<string, object?>
        {
            ["kod"] = "1234567890"
        });

        Assert.NotEmpty(ihlaller);
    }

    [Fact]
    public void SerbestMetin_Reddedilir()
    {
        var ihlaller = new HassasVeriFiltresi().Dogrula(new Dictionary<string, object?>
        {
            ["aciklamaMetni"] = new string('a', 200)
        });

        Assert.NotEmpty(ihlaller);
    }
}

public class JevKararMotoruTests
{
    private static JevKararMotoru KapaliMotor() =>
        new(new HttpClient(), new JevSecenekleri { ApiAnahtari = "test-anahtar" }, new HassasVeriFiltresi());

    [Fact]
    public async Task SemaDogrulanmadan_HazirDegildir()
    {
        Assert.False(await KapaliMotor().HazirMiAsync());
    }

    [Fact]
    public async Task SemaDogrulanmadan_AgCagrisiYapmadanHataDoner()
    {
        var sonuc = await KapaliMotor().EvetHayirAsync(new KararIstegi
        {
            SoruAnahtari = VarsayilanKurallar.ErpCariEslesme,
            Soru         = "?"
        });

        Assert.False(sonuc.Success);
        Assert.Contains("SemaDogrulandi", sonuc.Message);
    }

    [Fact]
    public async Task ApiAnahtariYoksa_HazirDegildir()
    {
        var motor = new JevKararMotoru(
            new HttpClient(),
            new JevSecenekleri { SemaDogrulandi = true },
            new HassasVeriFiltresi());

        Assert.False(await motor.HazirMiAsync());
    }
}

public class YedekliKararMotoruTests
{
    [Fact]
    public async Task BirincilBasarisizOlunca_YedegeDuser()
    {
        var motor = new YedekliKararMotoru(
            birincil: new JevKararMotoru(new HttpClient(),
                                         new JevSecenekleri { ApiAnahtari = "test" },
                                         new HassasVeriFiltresi()),
            yedek:    new KuralTabanliKararMotoru(VarsayilanKurallar.Olustur()));

        var sonuc = await motor.EvetHayirAsync(new KararIstegi
        {
            SoruAnahtari = VarsayilanKurallar.ErpCariEslesme,
            Soru         = "?",
            Ozellikler   = new Dictionary<string, object?> { ["vergiNoEslesti"] = true }
        });

        Assert.True(sonuc.Success);
        Assert.True(sonuc.Data!.Karar);
        Assert.Equal(KararKaynagi.Yedek, sonuc.Data.Kaynak);
        Assert.Contains("Yedek motor kullanıldı", sonuc.Message);
    }

    [Fact]
    public async Task IkisiDeBasarisizsa_HerIkiHataDaRaporlanir()
    {
        var motor = new YedekliKararMotoru(
            birincil: new JevKararMotoru(new HttpClient(),
                                         new JevSecenekleri { ApiAnahtari = "test" },
                                         new HassasVeriFiltresi()),
            yedek:    new KuralTabanliKararMotoru(new KuralKatalogu()));

        var sonuc = await motor.EvetHayirAsync(new KararIstegi
        {
            SoruAnahtari = "olmayan.kural",
            Soru         = "?"
        });

        Assert.False(sonuc.Success);
        Assert.Equal(2, sonuc.Errors.Count);
    }
}
