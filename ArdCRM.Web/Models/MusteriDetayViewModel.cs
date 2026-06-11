using ArdCRM.Core.Entities;

namespace ArdCRM.Web.Models;

public class MusteriDetayViewModel
{
    public Musteri Musteri { get; set; } = null!;
    public IEnumerable<Teklif> Teklifler { get; set; } = new List<Teklif>();

    public decimal ToplamTutar => Teklifler.Sum(t => t.Tutar);
    public int AcikTeklifSayisi => Teklifler.Count(t =>
        t.Durum != Core.Enums.TeklifDurumu.Onaylandi &&
        t.Durum != Core.Enums.TeklifDurumu.Reddedildi &&
        t.Durum != Core.Enums.TeklifDurumu.Iptal);
}
