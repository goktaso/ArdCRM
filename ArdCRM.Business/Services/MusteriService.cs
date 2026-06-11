using ArdCRM.Business.Validators;
using ArdCRM.Core;
using ArdCRM.Core.Entities;
using ArdCRM.Core.Interfaces;

namespace ArdCRM.Business.Services;

public class MusteriService : IMusteriService
{
    private readonly IRepository<Musteri> _musteriRepo;
    private readonly MusteriValidator _validator = new();

    public MusteriService(IRepository<Musteri> musteriRepo)
    {
        _musteriRepo = musteriRepo;
    }

    public async Task<ServiceResult<IEnumerable<Musteri>>> GetAllAsync()
    {
        var liste = await _musteriRepo.GetAllActiveAsync();
        return ServiceResult<IEnumerable<Musteri>>.Ok(liste);
    }

    public async Task<ServiceResult<Musteri>> GetByIdAsync(int id)
    {
        var musteri = await _musteriRepo.GetByIdAsync(id);
        if (musteri is null)
            return ServiceResult<Musteri>.Fail($"Müşteri bulunamadı. (Id: {id})");
        return ServiceResult<Musteri>.Ok(musteri);
    }

    public async Task<ServiceResult<Musteri>> CreateAsync(Musteri musteri)
    {
        var validation = await _validator.ValidateAsync(musteri);
        if (!validation.IsValid)
            return ServiceResult<Musteri>.Fail(validation.Errors.Select(e => e.ErrorMessage).ToList());

        var kayit = await _musteriRepo.AddAsync(musteri);
        return ServiceResult<Musteri>.Ok(kayit, "Müşteri başarıyla oluşturuldu.");
    }

    public async Task<ServiceResult<Musteri>> UpdateAsync(Musteri musteri)
    {
        if (!await _musteriRepo.ExistsAsync(musteri.Id))
            return ServiceResult<Musteri>.Fail("Güncellenecek müşteri bulunamadı.");

        var validation = await _validator.ValidateAsync(musteri);
        if (!validation.IsValid)
            return ServiceResult<Musteri>.Fail(validation.Errors.Select(e => e.ErrorMessage).ToList());

        var guncellenen = await _musteriRepo.UpdateAsync(musteri);
        return ServiceResult<Musteri>.Ok(guncellenen, "Müşteri güncellendi.");
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        if (!await _musteriRepo.ExistsAsync(id))
            return ServiceResult.Fail("Silinecek müşteri bulunamadı.");

        await _musteriRepo.DeleteAsync(id);
        return ServiceResult.Ok("Müşteri silindi.");
    }
}
