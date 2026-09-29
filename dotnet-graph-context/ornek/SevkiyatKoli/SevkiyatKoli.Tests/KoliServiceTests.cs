using SevkiyatKoli.Business;
using SevkiyatKoli.Core;

namespace SevkiyatKoli.Tests;

/// <summary>
/// Her iş kuralı için en az bir test. Kural numaraları
/// context/kurallar-ve-sinirlar.md ile aynıdır.
/// </summary>
public class KoliServiceTests
{
    private const string Irsaliye = "SEV2026001";

    private static KoliSatiri Satir(string stokKod, decimal agirlik, decimal miktar = 1m)
        => new() { StokKod = stokKod, Agirlik = agirlik, Miktar = miktar };

    // --- Kural 1: numara formatı ------------------------------------------
    [Fact]
    public void Kural1_KoliNumarasi_DogruBicimdeUretilir()
    {
        var servis = new KoliService();

        var koli = servis.KoliAc(Irsaliye).Veri!;

        Assert.Equal("SEV2026001-001", koli.KoliNo);
        Assert.True(KoliNumaratoru.BicimGecerli(koli.KoliNo, Irsaliye));
    }

    [Theory]
    [InlineData(1,   "SEV2026001-001")]
    [InlineData(3,   "SEV2026001-003")]
    [InlineData(42,  "SEV2026001-042")]
    [InlineData(999, "SEV2026001-999")]
    [InlineData(1000,"SEV2026001-1000")]
    public void Kural1_SiraNumarasi_UcBasamagaTamamlanir(int sira, string beklenen)
        => Assert.Equal(beklenen, KoliNumaratoru.Uret(Irsaliye, sira));

    [Fact]
    public void Kural1_GecersizGirdi_HataFirlatir()
    {
        Assert.Throws<ArgumentException>(() => KoliNumaratoru.Uret("", 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => KoliNumaratoru.Uret(Irsaliye, 0));
    }

    // --- Kural 2: sıra 1'den başlar, atlanmaz ------------------------------
    [Fact]
    public void Kural2_SiraNumarasi_BirdenBaslar_VeAtlanmaz()
    {
        var servis = new KoliService();

        var birinci = servis.KoliAc(Irsaliye).Veri!;
        var ikinci  = servis.KoliAc(Irsaliye).Veri!;
        var ucuncu  = servis.KoliAc(Irsaliye).Veri!;

        Assert.Equal(1, birinci.SiraNo);
        Assert.Equal(2, ikinci.SiraNo);
        Assert.Equal(3, ucuncu.SiraNo);
    }

    [Fact]
    public void Kural2_FarkliSevkiyatlar_KendiSirasindanBaslar()
    {
        var servis = new KoliService();

        servis.KoliAc(Irsaliye);
        var digerSevkiyat = servis.KoliAc("SEV2026002").Veri!;

        Assert.Equal(1, digerSevkiyat.SiraNo);
        Assert.Equal("SEV2026002-001", digerSevkiyat.KoliNo);
    }

    // --- Kural 3: kapalı koliye satır eklenemez ----------------------------
    [Fact]
    public void Kural3_KapaliKoliye_SatirEklenemez()
    {
        var servis = new KoliService();
        var koli   = servis.KoliAc(Irsaliye).Veri!;

        servis.SatirEkle(koli.KoliNo, Satir("STK-1", 2m));
        servis.KoliKapat(koli.KoliNo);

        var sonuc = servis.SatirEkle(koli.KoliNo, Satir("STK-2", 1m));

        Assert.False(sonuc.Basarili);
        Assert.Contains("iptal edip yeni koli", sonuc.Mesaj);
        Assert.Single(koli.Satirlar);
    }

    [Fact]
    public void Kural3_IptalKoliye_SatirEklenemez()
    {
        var servis = new KoliService();
        var koli   = servis.KoliAc(Irsaliye).Veri!;

        servis.KoliIptal(koli.KoliNo);

        Assert.False(servis.SatirEkle(koli.KoliNo, Satir("STK-1", 1m)).Basarili);
    }

    [Fact]
    public void Kural3_DuzeltmeYolu_IptalArtiYeniKoli()
    {
        var servis = new KoliService();
        var eski   = servis.KoliAc(Irsaliye).Veri!;
        servis.SatirEkle(eski.KoliNo, Satir("STK-1", 2m));
        servis.KoliKapat(eski.KoliNo);

        Assert.True(servis.KoliIptal(eski.KoliNo).Basarili);

        var yeni = servis.KoliAc(Irsaliye).Veri!;
        Assert.Equal("SEV2026001-002", yeni.KoliNo);
        Assert.True(servis.SatirEkle(yeni.KoliNo, Satir("STK-1", 2m)).Basarili);
    }

    // --- Kural 4: brüt ağırlık 30 kg'ı aşamaz ------------------------------
    [Fact]
    public void Kural4_SiniriAsanSatir_Reddedilir()
    {
        var servis = new KoliService();
        var koli   = servis.KoliAc(Irsaliye).Veri!;

        servis.SatirEkle(koli.KoliNo, Satir("STK-1", 25m));
        var sonuc = servis.SatirEkle(koli.KoliNo, Satir("STK-2", 5m));   // 0.8 + 25 + 5 = 30.8

        Assert.False(sonuc.Basarili);
        Assert.Contains("Brüt ağırlık sınırı", sonuc.Mesaj);
        Assert.Single(koli.Satirlar);
        Assert.Equal(25.8m, koli.BrutAgirlik);
    }

    [Fact]
    public void Kural4_TamSinirdakiSatir_Kabul_Edilir()
    {
        var servis = new KoliService();
        var koli   = servis.KoliAc(Irsaliye).Veri!;

        var sonuc = servis.SatirEkle(koli.KoliNo, Satir("STK-1", 29.2m)); // 0.8 + 29.2 = 30.0

        Assert.True(sonuc.Basarili);
        Assert.Equal(30m, koli.BrutAgirlik);
    }

    [Fact]
    public void Kural4_DaraBrutAgirligaDahildir()
    {
        var servis = new KoliService();
        var koli   = servis.KoliAc(Irsaliye).Veri!;

        servis.SatirEkle(koli.KoliNo, Satir("STK-1", 10m));

        Assert.Equal(10m,   koli.NetAgirlik);
        Assert.Equal(10.8m, koli.BrutAgirlik);
    }

    // --- Kural 5: numara tekrar kullanılmaz --------------------------------
    [Fact]
    public void Kural5_IptalEdilenNumara_TekrarKullanilmaz()
    {
        var servis = new KoliService();

        var birinci = servis.KoliAc(Irsaliye).Veri!;
        servis.KoliIptal(birinci.KoliNo);

        var ikinci = servis.KoliAc(Irsaliye).Veri!;

        Assert.Equal("SEV2026001-001", birinci.KoliNo);
        Assert.Equal("SEV2026001-002", ikinci.KoliNo);   // 001'e geri dönülmez
        Assert.NotEqual(birinci.KoliNo, ikinci.KoliNo);
    }

    [Fact]
    public void Kural5_IptalSonrasi_SevkiyattaNumaraBoslugu_Gorunur()
    {
        var servis = new KoliService();

        var birinci = servis.KoliAc(Irsaliye).Veri!;
        servis.KoliIptal(birinci.KoliNo);
        servis.KoliAc(Irsaliye);

        var sevkiyat = servis.SevkiyatGetir(Irsaliye).Veri!;
        var aktifler = sevkiyat.Koliler.Where(k => k.Durum != KoliDurumu.Iptal).ToList();

        Assert.Equal(2, sevkiyat.Koliler.Count);       // iptal kaydı silinmez
        Assert.Single(aktifler);                        // aktif tek koli
        Assert.Equal(2, sevkiyat.SonSiraNo);            // sayaç geri alınmaz
    }

    // --- Genel doğrulamalar ------------------------------------------------
    [Fact]
    public void BosKoli_Kapatilamaz()
    {
        var servis = new KoliService();
        var koli   = servis.KoliAc(Irsaliye).Veri!;

        var sonuc = servis.KoliKapat(koli.KoliNo);

        Assert.False(sonuc.Basarili);
        Assert.Contains("boş", sonuc.Mesaj);
    }

    [Fact]
    public void OlmayanKoli_IslemleriHataDoner()
    {
        var servis = new KoliService();

        Assert.False(servis.SatirEkle("YOK-001", Satir("STK-1", 1m)).Basarili);
        Assert.False(servis.KoliKapat("YOK-001").Basarili);
        Assert.False(servis.KoliIptal("YOK-001").Basarili);
        Assert.False(servis.KoliGetir("YOK-001").Basarili);
    }

    [Fact]
    public void GecersizMiktarVeAgirlik_Reddedilir()
    {
        var servis = new KoliService();
        var koli   = servis.KoliAc(Irsaliye).Veri!;

        Assert.False(servis.SatirEkle(koli.KoliNo, Satir("STK-1", 1m, miktar: 0m)).Basarili);
        Assert.False(servis.SatirEkle(koli.KoliNo, Satir("STK-1", -1m)).Basarili);
    }

    [Fact]
    public void BosIrsaliyeNo_Reddedilir()
        => Assert.False(new KoliService().KoliAc("  ").Basarili);
}
