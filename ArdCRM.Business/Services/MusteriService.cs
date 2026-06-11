using ArdCRM.Core;
using ArdCRM.Core.Entities;
using ArdCRM.Core.Interfaces;

namespace ArdCRM.Business.Services;

public class MusteriService : IMusteriService
{
    private readonly IRepository<Musteri> _musteriRepo;

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
        var hatalar = Validate(musteri);
        if (hatalar.Count > 0)
            return ServiceResult<Musteri>.Fail(hatalar);

        var kayit = await _musteriRepo.AddAsync(musteri);
        return ServiceResult<Musteri>.Ok(kayit, "Müşteri başarıyla oluşturuldu.");
    }

    public async Task<ServiceResult<Musteri>> UpdateAsync(Musteri musteri)
    {
        if (!await _musteriRepo.ExistsAsync(musteri.Id))
            return ServiceResult<Musteri>.Fail("Güncellenecek müşteri bulunamadı.");

        var hatalar = Validate(musteri);
        if (hatalar.Count > 0)
            return ServiceResult<Musteri>.Fail(hatalar);

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

    private static List<string> Validate(Musteri m)
    {
        var hatalar = new List<string>();
        if (string.IsNullOrWhiteSpace(m.Ad)) hatalar.Add("Ad alanı zorunludur.");
        if (string.IsNullOrWhiteSpace(m.FirmaAdi)) hatalar.Add("Firma adı zorunludur.");
        if (!string.IsNullOrWhiteSpace(m.Email) && !m.Email.Contains('@'))
            hatalar.Add("Geçerli bir e-posta adresi giriniz.");
        return hatalar;
    }
}
