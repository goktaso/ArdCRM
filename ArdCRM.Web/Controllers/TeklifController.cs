using ArdCRM.Core.Entities;
using ArdCRM.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArdCRM.Web.Controllers;

public class TeklifController : Controller
{
    private readonly ITeklifService _teklifService;
    private readonly IMusteriService _musteriService;

    public TeklifController(ITeklifService teklifService, IMusteriService musteriService)
    {
        _teklifService = teklifService;
        _musteriService = musteriService;
    }

    // GET: /Teklif
    public async Task<IActionResult> Index()
    {
        var result = await _teklifService.GetAllAsync();
        if (!result.Success)
        {
            TempData["Hata"] = result.Message;
            return View(Enumerable.Empty<Teklif>());
        }
        return View(result.Data);
    }

    // GET: /Teklif/Detay/5
    public async Task<IActionResult> Detay(int id)
    {
        var result = await _teklifService.GetByIdAsync(id);
        if (!result.Success)
        {
            TempData["Hata"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(result.Data);
    }

    // GET: /Teklif/Ekle?musteriId=5
    public async Task<IActionResult> Ekle(int? musteriId)
    {
        await MusteriListesiYukle(musteriId);
        var teklif = new Teklif
        {
            MusteriId = musteriId ?? 0,
            TeklifTarihi = DateTime.Now,
            GecerlilikTarihi = DateTime.Now.AddDays(30)
        };
        return View(teklif);
    }

    // POST: /Teklif/Ekle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(Teklif teklif)
    {
        var result = await _teklifService.CreateAsync(teklif);
        if (!result.Success)
        {
            result.Errors.ForEach(e => ModelState.AddModelError("", e));
            await MusteriListesiYukle(teklif.MusteriId);
            return View(teklif);
        }
        TempData["Basari"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    // GET: /Teklif/Duzenle/5
    public async Task<IActionResult> Duzenle(int id)
    {
        var result = await _teklifService.GetByIdAsync(id);
        if (!result.Success)
        {
            TempData["Hata"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        await MusteriListesiYukle(result.Data!.MusteriId);
        return View(result.Data);
    }

    // POST: /Teklif/Duzenle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(Teklif teklif)
    {
        var result = await _teklifService.UpdateAsync(teklif);
        if (!result.Success)
        {
            result.Errors.ForEach(e => ModelState.AddModelError("", e));
            await MusteriListesiYukle(teklif.MusteriId);
            return View(teklif);
        }
        TempData["Basari"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    // POST: /Teklif/Sil/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id)
    {
        var result = await _teklifService.DeleteAsync(id);
        TempData[result.Success ? "Basari" : "Hata"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private async Task MusteriListesiYukle(int? secilenId = null)
    {
        var result = await _musteriService.GetAllAsync();
        var liste = result.Success ? result.Data! : Enumerable.Empty<Musteri>();
        ViewBag.MusteriListesi = new SelectList(liste, "Id", "FirmaAdi", secilenId);
    }
}
