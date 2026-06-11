using ArdCRM.Business.Validators;
using ArdCRM.Core;
using ArdCRM.Core.Entities;
using ArdCRM.Core.Interfaces;

namespace ArdCRM.Business.Services;

public class TeklifService : ITeklifService
{
    private readonly IRepository<Teklif>  _teklifRepo;
    private readonly IRepository<Musteri> _musteriRepo;
    private readonly TeklifValidator _validator = new();

    public TeklifService(IRepository<Teklif> teklifRepo, IRepository<Musteri> musteriRepo)
    {
        _teklifRepo  = teklifRepo;
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
        return ServiceResult<IEnumerable<Teklif>>.Ok(tumListe.Where(t => t.MusteriId == musteriId));
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

        var validation = await _validator.ValidateAsync(teklif);
        if (!validation.IsValid)
            return ServiceResult<Teklif>.Fail(validation.Errors.Select(e => e.ErrorMessage).ToList());

        var kayit = await _teklifRepo.AddAsync(teklif);
        return ServiceResult<Teklif>.Ok(kayit, $"Teklif oluşturuldu. ({teklif.TeklifNo})");
    }

    public async Task<ServiceResult<Teklif>> UpdateAsync(Teklif teklif)
    {
        if (!await _teklifRepo.ExistsAsync(teklif.Id))
            return ServiceResult<Teklif>.Fail("Güncellenecek teklif bulunamadı.");

        var validation = await _validator.ValidateAsync(teklif);
        if (!validation.IsValid)
            return ServiceResult<Teklif>.Fail(validation.Errors.Select(e => e.ErrorMessage).ToList());

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
        var no = $"TKL-{DateTime.Now:yyyy}-{DateTime.Now.Ticks % 100000:D5}";
        return Task.FromResult(ServiceResult<string>.Ok(no));
    }
}
