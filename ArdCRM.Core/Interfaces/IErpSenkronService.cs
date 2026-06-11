using ArdCRM.Core.Interfaces;

namespace ArdCRM.Core.Interfaces;

public interface IErpSenkronService
{
    /// <summary>ERP'den müşterileri önizleme için getirir — DB'ye yazmaz.</summary>
    Task<ServiceResult<IEnumerable<ErpMusteri>>> OnizlemeGetirAsync();

    /// <summary>Seçilen ERP müşterilerini ArdCRM'e aktarır.</summary>
    Task<ServiceResult<ErpSenkronSonuc>> MusterileriAktarAsync(IEnumerable<string> cariKodlar);
}

public class ErpSenkronSonuc
{
    public int YeniEklenen    { get; set; }
    public int Guncellenen    { get; set; }
    public int Atlanan        { get; set; }
    public List<string> Hatalar { get; set; } = new();
}
