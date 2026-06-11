using ArdCRM.Core.Entities;

namespace ArdCRM.Core.Interfaces;

public interface IMusteriService
{
    Task<ServiceResult<IEnumerable<Musteri>>> GetAllAsync();
    Task<ServiceResult<Musteri>> GetByIdAsync(int id);
    Task<ServiceResult<Musteri>> CreateAsync(Musteri musteri);
    Task<ServiceResult<Musteri>> UpdateAsync(Musteri musteri);
    Task<ServiceResult> DeleteAsync(int id);
}

public interface ITeklifService
{
    Task<ServiceResult<IEnumerable<Teklif>>> GetAllAsync();
    Task<ServiceResult<IEnumerable<Teklif>>> GetByMusteriAsync(int musteriId);
    Task<ServiceResult<Teklif>> GetByIdAsync(int id);
    Task<ServiceResult<Teklif>> CreateAsync(Teklif teklif);
    Task<ServiceResult<Teklif>> UpdateAsync(Teklif teklif);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult<string>> GenerateTeklifNoAsync();
}
