using ArdCRM.Core.Entities;
using ArdCRM.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArdCRM.Web.Controllers;

public class MusteriController : Controller
{
    private readonly IMusteriService _musteriService;

    public MusteriController(IMusteriService musteriService)
    {
        _musteriService = musteriService;
    }

    // GET: /Musteri
    public async Task<IActionResult> Index()
    {
        var result = await _musteriService.GetAllAsync();
        if (!result.Success)
        {
            TempData["Hata"] = result.Message;
            return View(Enumerable.Empty<Musteri>());
        }
        return View(result.Data);
    }

    // GET: /Musteri/Detay/5
    public async Task<IActionResult> Detay(int id)
    {
        var result = await _musteriService.GetByIdAsync(id);
        if (!result.Success)
        {
            TempData["Hata"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(result.Data);
    }

    // GET: /Musteri/Ekle
    public IActionResult Ekle() => View(new Musteri());

    // POST: /Musteri/Ekle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(Musteri musteri)
    {
        var result = await _musteriService.CreateAsync(musteri);
        if (!result.Success)
        {
            result.Errors.ForEach(e => ModelState.AddModelError("", e));
            return View(musteri);
        }
        TempData["Basari"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    // GET: /Musteri/Duzenle/5
    public async Task<IActionResult> Duzenle(int id)
    {
        var result = await _musteriService.GetByIdAsync(id);
        if (!result.Success)
        {
            TempData["Hata"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(result.Data);
    }

    // POST: /Musteri/Duzenle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(Musteri musteri)
    {
        var result = await _musteriService.UpdateAsync(musteri);
        if (!result.Success)
        {
            result.Errors.ForEach(e => ModelState.AddModelError("", e));
            return View(musteri);
        }
        TempData["Basari"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    // POST: /Musteri/Sil/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id)
    {
        var result = await _musteriService.DeleteAsync(id);
        TempData[result.Success ? "Basari" : "Hata"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}
