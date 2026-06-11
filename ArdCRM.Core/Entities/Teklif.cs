using ArdCRM.Core.Enums;

namespace ArdCRM.Core.Entities;

public class Teklif : BaseEntity
{
    public string TeklifNo { get; set; } = string.Empty;
    public int MusteriId { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string? Aciklama { get; set; }
    public decimal Tutar { get; set; }
    public string Para { get; set; } = "TRY";
    public DateTime TeklifTarihi { get; set; } = DateTime.Now;
    public DateTime? GecerlilikTarihi { get; set; }
    public TeklifDurumu Durum { get; set; } = TeklifDurumu.Taslak;
    public string? Notlar { get; set; }

    // Navigation
    public Musteri Musteri { get; set; } = null!;
}
