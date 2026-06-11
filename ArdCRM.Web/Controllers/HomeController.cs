using System.Diagnostics;
using ArdCRM.Core.Enums;
using ArdCRM.Core.Interfaces;
using ArdCRM.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArdCRM.Web.Controllers;

public class HomeController : Controller
{
    private readonly IMusteriService _musteriService;
    private readonly ITeklifService _teklifService;

    public HomeController(IMusteriService musteriService, ITeklifService teklifService)
    {
        _musteriService = musteriService;
        _teklifService = teklifService;
    }

    public async Task<IActionResult> Index()
    {
        var musteriler = (await _musteriService.GetAllAsync()).Data?.ToList() ?? new();
        var teklifler  = (await _teklifService.GetAllAsync()).Data?.ToList() ?? new();

        var vm = new DashboardViewModel
        {
            // Müşteri
            ToplamMusteri     = musteriler.Count,
            AktifMusteri      = musteriler.Count(m => m.Tip == MusteriTipi.Aktif),
            PotansiyelMusteri = musteriler.Count(m => m.Tip == MusteriTipi.Potansiyel),
            VipMusteri        = musteriler.Count(m => m.Tip == MusteriTipi.VIP),

            // Teklif
            ToplamTeklif    = teklifler.Count,
            OnaylananTeklif = teklifler.Count(t => t.Durum == TeklifDurumu.Onaylandi),
            BekleyenTeklif  = teklifler.Count(t => t.Durum == TeklifDurumu.Gonderildi
                                                 || t.Durum == TeklifDurumu.Gorusuluyor),
            AcikTeklif      = teklifler.Count(t => t.Durum != TeklifDurumu.Onaylandi
                                                 && t.Durum != TeklifDurumu.Reddedildi
                                                 && t.Durum != TeklifDurumu.Iptal),

            // Tutarlar
            ToplamTutar      = teklifler.Sum(t => t.Tutar),
            OnaylananTutar   = teklifler.Where(t => t.Durum == TeklifDurumu.Onaylandi).Sum(t => t.Tutar),
            BekleyenTutar    = teklifler.Where(t => t.Durum == TeklifDurumu.Gonderildi
                                                  || t.Durum == TeklifDurumu.Gorusuluyor).Sum(t => t.Tutar),

            // Son eklenenler
            SonMusteriler = musteriler.OrderByDescending(m => m.OlusturmaTarihi).Take(5),
            SonTeklifler  = teklifler.OrderByDescending(t => t.OlusturmaTarihi).Take(5),
        };

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
