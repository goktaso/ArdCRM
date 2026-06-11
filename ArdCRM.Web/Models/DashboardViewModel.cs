using ArdCRM.Core.Entities;
using ArdCRM.Core.Enums;

namespace ArdCRM.Web.Models;

public class DashboardViewModel
{
    // Müşteri istatistikleri
    public int ToplamMusteri { get; set; }
    public int AktifMusteri { get; set; }
    public int PotansiyelMusteri { get; set; }
    public int VipMusteri { get; set; }

    // Teklif istatistikleri
    public int ToplamTeklif { get; set; }
    public int AcikTeklif { get; set; }
    public int OnaylananTeklif { get; set; }
    public int BekleyenTeklif { get; set; }

    // Tutarlar
    public decimal ToplamTutar { get; set; }
    public decimal OnaylananTutar { get; set; }
    public decimal BekleyenTutar { get; set; }

    // Son eklenenler
    public IEnumerable<Musteri> SonMusteriler { get; set; } = new List<Musteri>();
    public IEnumerable<Teklif> SonTeklifler { get; set; } = new List<Teklif>();

    // Hesaplanan
    public decimal OnayOrani => ToplamTeklif > 0
        ? Math.Round((decimal)OnaylananTeklif / ToplamTeklif * 100, 1)
        : 0;
}
