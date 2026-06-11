using ArdCRM.Core.Entities;
using ArdCRM.Core.Interfaces;
using ArdCRM.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArdCRM.Web.Controllers;

public class MusteriController : Controller
{
    private readonly IMusteriService _musteriService;
    private readonly ITeklifService _teklifService;

    public MusteriController(IMusteriService musteriService, ITeklifService teklifService)
    {
        _musteriService = musteriService;
        _teklifService = teklifService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _musteriService.GetAllAsync();
        if (!result.Success) { TempData["Hata"] = result.Message; return View(Enumerable.Empty<Musteri>()); }
        return View(result.Data);
    }

    public async Task<IActionResult> Detay(int id)
    {
        var musteriResult = await _musteriService.GetByIdAsync(id);
        if (!musteriResult.Success) { TempData["Hata"] = musteriResult.Message; return RedirectToAction(nameof(Index)); }

        var teklifResult = await _teklifService.GetByMusteriAsync(id);
        var vm = new MusteriDetayViewModel
        {
            Musteri   = musteriResult.Data!,
            Teklifler = teklifResult.Success ? teklifResult.Data! : Enumerable.Empty<Teklif>()
        };
        return View(vm);
    }

    public IActionResult Ekle() => View(new Musteri());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(Musteri musteri)
    {
        var result = await _musteriService.CreateAsync(musteri);
        if (!result.Success) { result.Errors.ForEach(e => ModelState.AddModelError("", e)); return View(musteri); }
        TempData["Basari"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Duzenle(int id)
    {
        var result = await _musteriService.GetByIdAsync(id);
        if (!result.Success) { TempData["Hata"] = result.Message; return RedirectToAction(nameof(Index)); }
        return View(result.Data);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(Musteri musteri)
    {
        var result = await _musteriService.UpdateAsync(musteri);
        if (!result.Success) { result.Errors.ForEach(e => ModelState.AddModelError("", e)); return View(musteri); }
        TempData["Basari"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id)
    {
        var result = await _musteriService.DeleteAsync(id);
        TempData[result.Success ? "Basari" : "Hata"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}
