using ArdCRM.Core.Interfaces;

namespace ArdCRM.Data.Adapters;

/// <summary>
/// NetsisConnection tanımlı değilken devreye giren boş adapter.
/// Uygulama ERP olmadan da çalışmaya devam eder.
/// </summary>
public class NullErpAdapter : IErpAdapter
{
    public Task<bool> TestConnectionAsync() =>
        Task.FromResult(false);

    public Task<IEnumerable<ErpMusteri>> GetMusterilerAsync() =>
        Task.FromResult(Enumerable.Empty<ErpMusteri>());

    public Task<ErpMusteri?> GetMusteriByKodAsync(string cariKod) =>
        Task.FromResult<ErpMusteri?>(null);

    public Task<IEnumerable<ErpStokHareketi>> GetStokHareketleriAsync(DateTime baslangic, DateTime bitis) =>
        Task.FromResult(Enumerable.Empty<ErpStokHareketi>());
}
