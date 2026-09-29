using ArdCRM.Core;
using ArdCRM.Core.Interfaces;

namespace ArdCRM.Data.Adapters.Karar;

/// <summary>
/// Deterministik, dış bağımlılığı olmayan karar motoru.
///
/// Sistemin temel motoru budur: JEV veya başka bir sağlayıcı devreye girse bile
/// bu motor yedek olarak kalır ve A/B karşılaştırmasının referans noktasıdır.
/// Dış servise hiçbir veri göndermez.
/// </summary>
public sealed class KuralTabanliKararMotoru : IKararMotoru
{
    private readonly KuralKatalogu _katalog;

    public KuralTabanliKararMotoru(KuralKatalogu katalog) => _katalog = katalog;

    public Task<bool> HazirMiAsync(CancellationToken iptal = default) => Task.FromResult(true);

    public Task<ServiceResult<EvetHayirKarari>> EvetHayirAsync(KararIstegi istek, CancellationToken iptal = default)
    {
        if (!_katalog.EvetHayirBul(istek.SoruAnahtari, out var kural))
            return Task.FromResult(ServiceResult<EvetHayirKarari>.Fail(
                $"'{istek.SoruAnahtari}' için evet/hayır kuralı tanımlı değil."));

        try
        {
            return Task.FromResult(ServiceResult<EvetHayirKarari>.Ok(kural(istek)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(ServiceResult<EvetHayirKarari>.Fail(
                $"Kural çalıştırılamadı ({istek.SoruAnahtari}): {ex.Message}"));
        }
    }

    public Task<ServiceResult<SecimKarari>> SecimAsync(SecimIstegi istek, CancellationToken iptal = default)
    {
        if (istek.Secenekler.Count == 0)
            return Task.FromResult(ServiceResult<SecimKarari>.Fail("Seçenek listesi boş olamaz."));

        if (!_katalog.SecimBul(istek.SoruAnahtari, out var kural))
            return Task.FromResult(ServiceResult<SecimKarari>.Fail(
                $"'{istek.SoruAnahtari}' için seçim kuralı tanımlı değil."));

        try
        {
            var karar = kural(istek);

            if (!istek.Secenekler.Contains(karar.Secim, StringComparer.Ordinal))
                return Task.FromResult(ServiceResult<SecimKarari>.Fail(
                    $"Kural, seçenek kümesi dışında bir değer döndü: '{karar.Secim}'."));

            return Task.FromResult(ServiceResult<SecimKarari>.Ok(karar));
        }
        catch (Exception ex)
        {
            return Task.FromResult(ServiceResult<SecimKarari>.Fail(
                $"Kural çalıştırılamadı ({istek.SoruAnahtari}): {ex.Message}"));
        }
    }

    public Task<ServiceResult<PuanKarari>> PuanAsync(PuanIstegi istek, CancellationToken iptal = default)
    {
        if (istek.EnDusuk >= istek.EnYuksek)
            return Task.FromResult(ServiceResult<PuanKarari>.Fail("Puan aralığı geçersiz."));

        if (!_katalog.PuanBul(istek.SoruAnahtari, out var kural))
            return Task.FromResult(ServiceResult<PuanKarari>.Fail(
                $"'{istek.SoruAnahtari}' için puan kuralı tanımlı değil."));

        try
        {
            var karar = kural(istek);

            if (karar.Puan < istek.EnDusuk || karar.Puan > istek.EnYuksek)
                return Task.FromResult(ServiceResult<PuanKarari>.Fail(
                    $"Kural, {istek.EnDusuk}-{istek.EnYuksek} aralığı dışında puan döndü: {karar.Puan}."));

            return Task.FromResult(ServiceResult<PuanKarari>.Ok(karar));
        }
        catch (Exception ex)
        {
            return Task.FromResult(ServiceResult<PuanKarari>.Fail(
                $"Kural çalıştırılamadı ({istek.SoruAnahtari}): {ex.Message}"));
        }
    }
}
