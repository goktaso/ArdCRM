using ArdCRM.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArdCRM.Web.Controllers;

public class ErpController : Controller
{
    private readonly IErpSenkronService _senkronService;

    public ErpController(IErpSenkronService senkronService)
    {
        _senkronService = senkronService;
    }

    // GET: /Erp
    public IActionResult Index() => View();

    // POST: /Erp/Onizle
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Onizle()
    {
        var result = await _senkronService.OnizlemeGetirAsync();
        if (!result.Success)
        {
            TempData["Hata"] = result.Message;
            return View("Index");
        }
        return View("Onizleme", result.Data);
    }

    // POST: /Erp/Aktar
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Aktar(List<string> secilenKodlar)
    {
        if (secilenKodlar is null || !secilenKodlar.Any())
        {
            TempData["Hata"] = "En az bir müşteri seçmelisiniz.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _senkronService.MusterileriAktarAsync(secilenKodlar);
        TempData[result.Success ? "Basari" : "Hata"] = result.Message;

        if (result.Success && result.Data!.Hatalar.Any())
            TempData["Uyari"] = string.Join(" | ", result.Data.Hatalar);

        return RedirectToAction("Index", "Musteri");
    }
}
