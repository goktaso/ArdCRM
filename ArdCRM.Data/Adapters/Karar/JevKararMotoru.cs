using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ArdCRM.Core;
using ArdCRM.Core.Interfaces;
using ArdCRM.Core.Kararlar;

namespace ArdCRM.Data.Adapters.Karar;

/// <summary>
/// JEV (TypeSafe System One) adapter'ı.
///
/// DURUM: İSTEK/YANIT ŞEMASI DOĞRULANMADI — ADAPTER KAPALI DOĞAR.
/// Bilinen tek gerçek: uç nokta POST https://api.typesafe.ai/v1/systemone ve
/// model üç soru tipi kabul ediyor (Noul = evet/hayır olasılık, Choice = kapalı
/// küme seçimi, Score = sıralı puan). Alan adları, kimlik doğrulama başlığının
/// biçimi ve yanıt gövdesi resmî dokümanla doğrulanmamıştır; bu yüzden
/// <see cref="JevSecenekleri.SemaDogrulandi"/> varsayılan olarak false'tur ve
/// adapter hiçbir ağ çağrısı yapmaz.
///
/// AÇMA ADIMLARI (API anahtarı alındığında):
///   1. Resmî dokümandaki istek gövdesiyle CagirAsync içindeki govde sözlüğünü eşleştir.
///   2. Yanıt gövdesiyle Yanit*Coz metotlarını eşleştir.
///   3. Kimlik doğrulama başlığını doğrula (Bearer varsayıldı).
///   4. Tek bir gerçek çağrı ile doğrula, çıktıyı context/kararlar.md'ye yaz.
///   5. appsettings.{Ortam}.json içinde Jev:SemaDogrulandi = true yap.
///
/// Şema doğrulanana kadar bu sınıf üretimde kural motorunun önüne konmamalıdır.
/// </summary>
public sealed class JevKararMotoru : IKararMotoru
{
    private const string SemaUyarisi =
        "JEV adapter'ı kapalı: /v1/systemone şeması resmî dokümanla doğrulanmadı " +
        "(Jev:SemaDogrulandi = false). Sistem kural motoruyla çalışıyor.";

    private readonly HttpClient _istemci;
    private readonly JevSecenekleri _secenekler;
    private readonly HassasVeriFiltresi _filtre;

    public JevKararMotoru(HttpClient istemci, JevSecenekleri secenekler, HassasVeriFiltresi filtre)
    {
        _istemci    = istemci;
        _secenekler = secenekler;
        _filtre     = filtre;

        _istemci.Timeout = TimeSpan.FromSeconds(Math.Max(1, secenekler.ZamanAsimiSaniye));

        if (_istemci.BaseAddress is null && !string.IsNullOrWhiteSpace(secenekler.TabanAdres))
            _istemci.BaseAddress = new Uri(secenekler.TabanAdres);

        if (!string.IsNullOrWhiteSpace(secenekler.ApiAnahtari))
            _istemci.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", secenekler.ApiAnahtari);
    }

    public Task<bool> HazirMiAsync(CancellationToken iptal = default)
        => Task.FromResult(_secenekler.SemaDogrulandi && !string.IsNullOrWhiteSpace(_secenekler.ApiAnahtari));

    public async Task<ServiceResult<EvetHayirKarari>> EvetHayirAsync(KararIstegi istek, CancellationToken iptal = default)
    {
        var onKontrol = OnKontrol(istek);
        if (onKontrol is not null)
            return ServiceResult<EvetHayirKarari>.Fail(onKontrol);

        var yanit = await CagirAsync("noul", istek, ekAlanlar: null, iptal);
        if (!yanit.Success || yanit.Data is null)
            return ServiceResult<EvetHayirKarari>.Fail(yanit.Errors);

        try
        {
            var kok = yanit.Data.RootElement;
            var olasilik = OndalikOku(kok, "probability", "olasilik", "value");

            return ServiceResult<EvetHayirKarari>.Ok(new EvetHayirKarari
            {
                Karar    = olasilik >= 0.5d,
                Olasilik = olasilik,
                Kaynak   = KararKaynagi.Jev
            });
        }
        catch (Exception ex)
        {
            return ServiceResult<EvetHayirKarari>.Fail($"JEV yanıtı çözümlenemedi: {ex.Message}");
        }
        finally
        {
            yanit.Data.Dispose();
        }
    }

    public async Task<ServiceResult<SecimKarari>> SecimAsync(SecimIstegi istek, CancellationToken iptal = default)
    {
        var onKontrol = OnKontrol(istek);
        if (onKontrol is not null)
            return ServiceResult<SecimKarari>.Fail(onKontrol);

        if (istek.Secenekler.Count == 0)
            return ServiceResult<SecimKarari>.Fail("Seçenek listesi boş olamaz.");

        var yanit = await CagirAsync("choice", istek,
            ekAlanlar: new Dictionary<string, object?> { ["options"] = istek.Secenekler }, iptal);

        if (!yanit.Success || yanit.Data is null)
            return ServiceResult<SecimKarari>.Fail(yanit.Errors);

        try
        {
            var kok   = yanit.Data.RootElement;
            var secim = MetinOku(kok, "choice", "secim", "value");

            if (secim is null || !istek.Secenekler.Contains(secim, StringComparer.Ordinal))
                return ServiceResult<SecimKarari>.Fail(
                    $"JEV, seçenek kümesi dışında bir değer döndü: '{secim ?? "(boş)"}'.");

            return ServiceResult<SecimKarari>.Ok(new SecimKarari
            {
                Secim  = secim,
                Guven  = OndalikOku(kok, "confidence", "guven", "probability"),
                Kaynak = KararKaynagi.Jev
            });
        }
        catch (Exception ex)
        {
            return ServiceResult<SecimKarari>.Fail($"JEV yanıtı çözümlenemedi: {ex.Message}");
        }
        finally
        {
            yanit.Data.Dispose();
        }
    }

    public async Task<ServiceResult<PuanKarari>> PuanAsync(PuanIstegi istek, CancellationToken iptal = default)
    {
        var onKontrol = OnKontrol(istek);
        if (onKontrol is not null)
            return ServiceResult<PuanKarari>.Fail(onKontrol);

        var yanit = await CagirAsync("score", istek,
            ekAlanlar: new Dictionary<string, object?>
            {
                ["min"] = istek.EnDusuk,
                ["max"] = istek.EnYuksek
            }, iptal);

        if (!yanit.Success || yanit.Data is null)
            return ServiceResult<PuanKarari>.Fail(yanit.Errors);

        try
        {
            var kok  = yanit.Data.RootElement;
            var puan = (int)Math.Round(OndalikOku(kok, "score", "puan", "value"));

            if (puan < istek.EnDusuk || puan > istek.EnYuksek)
                return ServiceResult<PuanKarari>.Fail(
                    $"JEV, {istek.EnDusuk}-{istek.EnYuksek} aralığı dışında puan döndü: {puan}.");

            return ServiceResult<PuanKarari>.Ok(new PuanKarari
            {
                Puan   = puan,
                Guven  = OndalikOku(kok, "confidence", "guven"),
                Kaynak = KararKaynagi.Jev
            });
        }
        catch (Exception ex)
        {
            return ServiceResult<PuanKarari>.Fail($"JEV yanıtı çözümlenemedi: {ex.Message}");
        }
        finally
        {
            yanit.Data.Dispose();
        }
    }

    /// <summary>Şema kilidi, anahtar ve hassas veri denetimi. Sorun yoksa null döner.</summary>
    private string? OnKontrol(KararIstegi istek)
    {
        if (!_secenekler.SemaDogrulandi)
            return SemaUyarisi;

        if (string.IsNullOrWhiteSpace(_secenekler.ApiAnahtari))
            return "JEV API anahtarı tanımlı değil.";

        var ihlaller = _filtre.Dogrula(istek.Ozellikler);
        if (ihlaller.Count > 0)
            return "Hassas veri dış sağlayıcıya gönderilemez: " + string.Join(" | ", ihlaller);

        return null;
    }

    /// <summary>
    /// ŞEMA DOĞRULANMADI — alan adları resmî dokümanla eşleştirilecek.
    /// </summary>
    private async Task<ServiceResult<JsonDocument>> CagirAsync(
        string soruTipi,
        KararIstegi istek,
        IDictionary<string, object?>? ekAlanlar,
        CancellationToken iptal)
    {
        var govde = new Dictionary<string, object?>
        {
            ["model"]    = _secenekler.Model,
            ["type"]     = soruTipi,
            ["question"] = istek.Soru,
            ["input"]    = istek.Ozellikler
        };

        if (ekAlanlar is not null)
            foreach (var (anahtar, deger) in ekAlanlar)
                govde[anahtar] = deger;

        try
        {
            using var icerik = new StringContent(
                JsonSerializer.Serialize(govde), Encoding.UTF8, "application/json");

            using var cevap = await _istemci.PostAsync(_secenekler.UcNokta, icerik, iptal);
            var metin = await cevap.Content.ReadAsStringAsync(iptal);

            if (!cevap.IsSuccessStatusCode)
                return ServiceResult<JsonDocument>.Fail(
                    $"JEV çağrısı başarısız ({(int)cevap.StatusCode}): {Kisalt(metin)}");

            return ServiceResult<JsonDocument>.Ok(JsonDocument.Parse(metin));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<JsonDocument>.Fail(
                $"JEV çağrısı {_secenekler.ZamanAsimiSaniye} saniyede yanıt vermedi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<JsonDocument>.Fail($"JEV çağrısı hata verdi: {ex.Message}");
        }
    }

    private static double OndalikOku(JsonElement kok, params string[] adaylar)
    {
        foreach (var ad in adaylar)
            if (kok.TryGetProperty(ad, out var deger) && deger.ValueKind == JsonValueKind.Number)
                return deger.GetDouble();

        return 0d;
    }

    private static string? MetinOku(JsonElement kok, params string[] adaylar)
    {
        foreach (var ad in adaylar)
            if (kok.TryGetProperty(ad, out var deger) && deger.ValueKind == JsonValueKind.String)
                return deger.GetString();

        return null;
    }

    private static string Kisalt(string metin)
        => metin.Length <= 300 ? metin : metin[..300] + "...";
}
