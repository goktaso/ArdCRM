using ArdCRM.Core.Enums;

namespace ArdCRM.Core.Entities;

public class Musteri : BaseEntity
{
    public string Ad { get; set; } = string.Empty;
    public string? Soyad { get; set; }
    public string FirmaAdi { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? Adres { get; set; }
    public string? Sehir { get; set; }
    public string? VergiNo { get; set; }
    public string? VergiDairesi { get; set; }
    public MusteriTipi Tip { get; set; } = MusteriTipi.Potansiyel;
    public string? Notlar { get; set; }

    // Navigation
    public ICollection<Teklif> Teklifler { get; set; } = new List<Teklif>();

    public string TamAd => $"{Ad} {Soyad}".Trim();
}
