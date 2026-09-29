using System.Diagnostics.CodeAnalysis;
using ArdCRM.Core.Interfaces;

namespace ArdCRM.Data.Adapters.Karar;

/// <summary>
/// Kural tabanlı karar motorunun sözlüğü. Her karar, soru anahtarına bağlı
/// deterministik bir fonksiyondur.
///
/// Neden delege sözlüğü: kurallar kod içinde, test edilebilir ve sürüm kontrolünde
/// kalır; yeni bir karar eklemek yeni bir sınıf hiyerarşisi gerektirmez.
/// </summary>
public sealed class KuralKatalogu
{
    private readonly Dictionary<string, Func<KararIstegi, EvetHayirKarari>> _evetHayir = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Func<SecimIstegi, SecimKarari>>     _secim     = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Func<PuanIstegi, PuanKarari>>       _puan      = new(StringComparer.OrdinalIgnoreCase);

    public KuralKatalogu EvetHayirKaydet(string soruAnahtari, Func<KararIstegi, EvetHayirKarari> kural)
    {
        _evetHayir[soruAnahtari] = kural;
        return this;
    }

    public KuralKatalogu SecimKaydet(string soruAnahtari, Func<SecimIstegi, SecimKarari> kural)
    {
        _secim[soruAnahtari] = kural;
        return this;
    }

    public KuralKatalogu PuanKaydet(string soruAnahtari, Func<PuanIstegi, PuanKarari> kural)
    {
        _puan[soruAnahtari] = kural;
        return this;
    }

    public bool EvetHayirBul(
        string soruAnahtari,
        [MaybeNullWhen(false)] out Func<KararIstegi, EvetHayirKarari> kural)
        => _evetHayir.TryGetValue(soruAnahtari, out kural);

    public bool SecimBul(
        string soruAnahtari,
        [MaybeNullWhen(false)] out Func<SecimIstegi, SecimKarari> kural)
        => _secim.TryGetValue(soruAnahtari, out kural);

    public bool PuanBul(
        string soruAnahtari,
        [MaybeNullWhen(false)] out Func<PuanIstegi, PuanKarari> kural)
        => _puan.TryGetValue(soruAnahtari, out kural);

    /// <summary>Öznitelik sözlüğünden tip güvenli okuma yardımcıları.</summary>
    public static double SayiOku(KararIstegi istek, string anahtar, double varsayilan = 0d)
    {
        if (!istek.Ozellikler.TryGetValue(anahtar, out var deger) || deger is null)
            return varsayilan;

        return deger switch
        {
            double d  => d,
            float f   => f,
            decimal m => (double)m,
            int i     => i,
            long l    => l,
            string s when double.TryParse(s, System.Globalization.NumberStyles.Any,
                                          System.Globalization.CultureInfo.InvariantCulture, out var p) => p,
            _ => varsayilan
        };
    }

    public static bool MantikOku(KararIstegi istek, string anahtar, bool varsayilan = false)
    {
        if (!istek.Ozellikler.TryGetValue(anahtar, out var deger) || deger is null)
            return varsayilan;

        return deger switch
        {
            bool b => b,
            string s when bool.TryParse(s, out var p) => p,
            _ => varsayilan
        };
    }
}
