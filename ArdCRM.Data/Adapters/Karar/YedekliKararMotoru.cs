using ArdCRM.Core;
using ArdCRM.Core.Interfaces;

namespace ArdCRM.Data.Adapters.Karar;

/// <summary>
/// Birincil motoru dener, başarısız olursa yedeğe düşer.
///
/// Amaç: dış sağlayıcı (JEV) kapalı, yavaş veya hatalıyken sistemin durmaması.
/// NullErpAdapter ile aynı mantık: dış bağımlılık yoksa uygulama yine çalışır.
///
/// Yedeğe düşme sessiz değildir; sonuç mesajında birincil motorun hatası taşınır
/// ve karar kaynağı <see cref="KararKaynagi.Yedek"/> olarak işaretlenir.
/// </summary>
public sealed class YedekliKararMotoru : IKararMotoru
{
    private readonly IKararMotoru _birincil;
    private readonly IKararMotoru _yedek;

    public YedekliKararMotoru(IKararMotoru birincil, IKararMotoru yedek)
    {
        _birincil = birincil;
        _yedek    = yedek;
    }

    public async Task<bool> HazirMiAsync(CancellationToken iptal = default)
        => await _birincil.HazirMiAsync(iptal) || await _yedek.HazirMiAsync(iptal);

    public Task<ServiceResult<EvetHayirKarari>> EvetHayirAsync(KararIstegi istek, CancellationToken iptal = default)
        => CalistirAsync(
            () => _birincil.EvetHayirAsync(istek, iptal),
            () => _yedek.EvetHayirAsync(istek, iptal),
            karar => new EvetHayirKarari
            {
                Karar    = karar.Karar,
                Olasilik = karar.Olasilik,
                Kaynak   = KararKaynagi.Yedek,
                Gerekce  = karar.Gerekce
            });

    public Task<ServiceResult<SecimKarari>> SecimAsync(SecimIstegi istek, CancellationToken iptal = default)
        => CalistirAsync(
            () => _birincil.SecimAsync(istek, iptal),
            () => _yedek.SecimAsync(istek, iptal),
            karar => new SecimKarari
            {
                Secim   = karar.Secim,
                Guven   = karar.Guven,
                Kaynak  = KararKaynagi.Yedek,
                Dagilim = karar.Dagilim,
                Gerekce = karar.Gerekce
            });

    public Task<ServiceResult<PuanKarari>> PuanAsync(PuanIstegi istek, CancellationToken iptal = default)
        => CalistirAsync(
            () => _birincil.PuanAsync(istek, iptal),
            () => _yedek.PuanAsync(istek, iptal),
            karar => new PuanKarari
            {
                Puan    = karar.Puan,
                Guven   = karar.Guven,
                Kaynak  = KararKaynagi.Yedek,
                Gerekce = karar.Gerekce
            });

    private static async Task<ServiceResult<T>> CalistirAsync<T>(
        Func<Task<ServiceResult<T>>> birincil,
        Func<Task<ServiceResult<T>>> yedek,
        Func<T, T> yedekIsaretle)
    {
        string birincilHata;

        try
        {
            var sonuc = await birincil();
            if (sonuc.Success && sonuc.Data is not null)
                return sonuc;

            birincilHata = sonuc.Message;
        }
        catch (Exception ex)
        {
            birincilHata = ex.Message;
        }

        var yedekSonuc = await yedek();

        if (!yedekSonuc.Success || yedekSonuc.Data is null)
            return ServiceResult<T>.Fail(new List<string>
            {
                $"Birincil motor: {birincilHata}",
                $"Yedek motor: {yedekSonuc.Message}"
            });

        return ServiceResult<T>.Ok(
            yedekIsaretle(yedekSonuc.Data),
            $"Yedek motor kullanıldı. Birincil motor: {birincilHata}");
    }
}
