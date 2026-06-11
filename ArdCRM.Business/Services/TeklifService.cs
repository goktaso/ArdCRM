using ArdCRM.Core;
using ArdCRM.Core.Entities;
using ArdCRM.Core.Interfaces;

namespace ArdCRM.Business.Services;

public class TeklifService : ITeklifService
{
    private readonly IRepository<Teklif> _teklifRepo;
    private readonly IRepository<Musteri> _musteriRepo;

    public TeklifService(IRepository<Teklif> teklifRepo, IRepository<Musteri> musteriRepo)
    {
        _teklifRepo = teklifRepo;
        _musteriRepo = musteriRepo;
    }

    public async Task<ServiceResult<IEnumerable<Teklif>>> GetAllAsync()
    {
        var liste = await _teklifRepo.GetAllActiveAsync();
        return ServiceResult<IEnumerable<Teklif>>.Ok(liste);
    }

    public async Task<ServiceResult<IEnumerable<Teklif>>> GetByMusteriAsync(int musteriId)
    {
        var tumListe = await _teklifRepo.GetAllActiveAsync();
        var musteriTeklifleri = tumListe.Where(t => t.MusteriId == musteriId);
        return ServiceResult<IEnumerable<Teklif>>.Ok(musteriTeklifleri);
    }

    public async Task<ServiceResult<Teklif>> GetByIdAsync(int id)
    {
        var teklif = await _teklifRepo.GetByIdAsync(id);
        if (teklif is null)
            return ServiceResult<Teklif>.Fail($"Teklif bulunamadı. (Id: {id})");

        return ServiceResult<Teklif>.Ok(teklif);
    }

    public async Task<ServiceResult<Teklif>> CreateAsync(Teklif teklif)
    {
        if (!await _musteriRepo.ExistsAsync(teklif.MusteriId))
            return ServiceResult<Teklif>.Fail("Geçersiz müşteri.");

        var noResult = await GenerateTeklifNoAsync();
        teklif.TeklifNo = noResult.Data!;

        var hatalar = Validate(teklif);
        if (hatalar.Count > 0)
            return ServiceResult<Teklif>.Fail(hatalar);

        var kayit = await _teklifRepo.AddAsync(teklif);
        return ServiceResult<Teklif>.Ok(kayit, $"Teklif oluşturuldu. ({teklif.TeklifNo})");
    }

    public async Task<ServiceResult<Teklif>> UpdateAsync(Teklif teklif)
    {
        if (!await _teklifRepo.ExistsAsync(teklif.Id))
            return ServiceResult<Teklif>.Fail("Güncellenecek teklif bulunamadı.");

        var hatalar = Validate(teklif);
        if (hatalar.Count > 0)
            return ServiceResult<Teklif>.Fail(hatalar);

        var guncellenen = await _teklifRepo.UpdateAsync(teklif);
        return ServiceResult<Teklif>.Ok(guncellenen, "Teklif güncellendi.");
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        if (!await _teklifRepo.ExistsAsync(id))
            return ServiceResult.Fail("Silinecek teklif bulunamadı.");

        await _teklifRepo.DeleteAsync(id);
        return ServiceResult.Ok("Teklif silindi.");
    }

    public Task<ServiceResult<string>> GenerateTeklifNoAsync()
    {
        // Format: TKL-2026-00001
        var no = $"TKL-{DateTime.Now:yyyy}-{DateTime.Now.Ticks % 100000:D5}";
        return Task.FromResult(ServiceResult<string>.Ok(no));
    }

    private static List<string> Validate(Teklif t)
    {
        var hatalar = new List<string>();
        if (string.IsNullOrWhiteSpace(t.Baslik)) hatalar.Add("Teklif başlığı zorunludur.");
        if (t.Tutar <= 0) hatalar.Add("Teklif tutarı sıfırdan büyük olmalıdır.");
        return hatalar;
    }
}
