namespace ArdCRM.Core.Interfaces;

/// <summary>
/// ERP sistemlerinden veri okumak için soyutlama katmanı.
/// Netsis, Logo, SAP gibi farklı ERP'ler bu interface'i implement eder.
/// KURAL: ERP her zaman READ-ONLY'dir. Asla yazma yapılmaz.
/// </summary>
public interface IErpAdapter
{
    /// <summary>Bağlantıyı test eder.</summary>
    Task<bool> TestConnectionAsync();

    /// <summary>ERP'deki tüm aktif cari hesapları getirir.</summary>
    Task<IEnumerable<ErpMusteri>> GetMusterilerAsync();

    /// <summary>Tek bir cari hesabı kodu ile getirir.</summary>
    Task<ErpMusteri?> GetMusteriByKodAsync(string cariKod);

    /// <summary>ERP'deki stok hareketlerini tarih aralığı ile getirir.</summary>
    Task<IEnumerable<ErpStokHareketi>> GetStokHareketleriAsync(DateTime baslangic, DateTime bitis);
}

/// <summary>ERP'den gelen ham müşteri/cari verisi.</summary>
public class ErpMusteri
{
    public string CariKod    { get; set; } = string.Empty;
    public string CariAdi    { get; set; } = string.Empty;
    public string? Telefon   { get; set; }
    public string? Email     { get; set; }
    public string? Adres     { get; set; }
    public string? Sehir     { get; set; }
    public string? VergiNo   { get; set; }
    public string? VergiDairesi { get; set; }
    public bool   Aktif      { get; set; } = true;
}

/// <summary>ERP'den gelen ham stok hareketi.</summary>
public class ErpStokHareketi
{
    public string   StokKod    { get; set; } = string.Empty;
    public string   StokAdi    { get; set; } = string.Empty;
    public decimal  Miktar     { get; set; }
    public string   Birim      { get; set; } = string.Empty;
    public DateTime Tarih      { get; set; }
    public string   BelgeNo    { get; set; } = string.Empty;
    public string   HareketTip { get; set; } = string.Empty;
}
