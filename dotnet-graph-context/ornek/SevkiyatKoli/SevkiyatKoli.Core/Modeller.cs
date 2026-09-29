namespace SevkiyatKoli.Core;

public enum KoliDurumu
{
    Acik  = 0,
    Kapali = 1,
    Iptal  = 2
}

public sealed class Sevkiyat
{
    public required string IrsaliyeNo { get; init; }
    public DateTime Tarih { get; init; } = DateTime.Now;

    private readonly List<Koli> _koliler = new();
    public IReadOnlyList<Koli> Koliler => _koliler;

    /// <summary>
    /// Üretilen son sıra numarası. İptal edilen koli bu sayacı GERİ ALMAZ —
    /// numara tekrar kullanılmaz (Kural 5).
    /// </summary>
    public int SonSiraNo { get; private set; }

    internal int SonrakiSiraNoUret() => ++SonSiraNo;

    internal void KoliEkle(Koli koli) => _koliler.Add(koli);
}

public sealed class Koli
{
    public required string KoliNo     { get; init; }
    public required string IrsaliyeNo { get; init; }
    public int    SiraNo              { get; init; }
    public KoliDurumu Durum           { get; private set; } = KoliDurumu.Acik;

    /// <summary>Boş koli darası (kg). Brüt ağırlığa dahildir.</summary>
    public decimal Dara { get; init; } = 0.8m;

    private readonly List<KoliSatiri> _satirlar = new();
    public IReadOnlyList<KoliSatiri> Satirlar => _satirlar;

    public decimal NetAgirlik   => _satirlar.Sum(s => s.Agirlik);
    public decimal BrutAgirlik  => Dara + NetAgirlik;

    internal void SatirEkle(KoliSatiri satir) => _satirlar.Add(satir);
    internal void Kapat()  => Durum = KoliDurumu.Kapali;
    internal void IptalEt() => Durum = KoliDurumu.Iptal;
}

public sealed class KoliSatiri
{
    public required string  StokKod { get; init; }
    public required decimal Miktar  { get; init; }
    public string  Birim            { get; init; } = "AD";
    /// <summary>Satırın toplam ağırlığı (kg).</summary>
    public required decimal Agirlik { get; init; }
}
