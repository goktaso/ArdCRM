namespace ArdCRM.Core.Interfaces;

/// <summary>
/// Küçük, tekrarlayan kararlar için soyutlama katmanı.
/// Kural tabanlı motor, JEV (TypeSafe System One) veya başka bir sağlayıcı
/// bu arayüzü implement eder.
///
/// KURAL: Bu arayüzden geçen <see cref="KararIstegi.Ozellikler"/> sözlüğüne
/// hassas veri konulmaz. Yalnızca türetilmiş/anonim öznitelik gönderilir
/// (örn. "adBenzerlik: 0.82", "vergiNoEslesti: true"). Ham cari adı, vergi no,
/// e-posta, telefon, adres gibi alanlar dış sağlayıcıya çıkamaz.
/// Bkz. ArdCRM.Core.Kararlar.HassasVeriFiltresi
/// </summary>
public interface IKararMotoru
{
    /// <summary>Bu motorun kullanılabilir olup olmadığını bildirir.</summary>
    Task<bool> HazirMiAsync(CancellationToken iptal = default);

    /// <summary>Evet/hayır kararı + kalibre olasılık.</summary>
    Task<ServiceResult<EvetHayirKarari>> EvetHayirAsync(KararIstegi istek, CancellationToken iptal = default);

    /// <summary>Önceden tanımlı seçenekler arasından seçim.</summary>
    Task<ServiceResult<SecimKarari>> SecimAsync(SecimIstegi istek, CancellationToken iptal = default);

    /// <summary>Tanımlı ölçekte puanlama.</summary>
    Task<ServiceResult<PuanKarari>> PuanAsync(PuanIstegi istek, CancellationToken iptal = default);
}

/// <summary>Kararın hangi motordan geldiği. Loglama ve A/B karşılaştırması için.</summary>
public enum KararKaynagi
{
    Kural = 0,
    Jev   = 1,
    Yedek = 2
}

public class KararIstegi
{
    /// <summary>Kural kataloğundaki anahtar. Örn. "erp.cari-eslesme".</summary>
    public required string SoruAnahtari { get; init; }

    /// <summary>İnsan ve model tarafından okunabilir soru metni.</summary>
    public required string Soru { get; init; }

    /// <summary>Türetilmiş, hassas olmayan öznitelikler.</summary>
    public IReadOnlyDictionary<string, object?> Ozellikler { get; init; }
        = new Dictionary<string, object?>();
}

public sealed class SecimIstegi : KararIstegi
{
    /// <summary>Kapalı seçenek kümesi. Boş olamaz.</summary>
    public required IReadOnlyList<string> Secenekler { get; init; }
}

public sealed class PuanIstegi : KararIstegi
{
    public int EnDusuk  { get; init; } = 1;
    public int EnYuksek { get; init; } = 5;
}

public sealed class EvetHayirKarari
{
    public bool          Karar     { get; init; }
    /// <summary>0..1 aralığında kalibre olasılık.</summary>
    public double        Olasilik  { get; init; }
    public KararKaynagi  Kaynak    { get; init; }
    public string?       Gerekce   { get; init; }
}

public sealed class SecimKarari
{
    public string        Secim   { get; init; } = string.Empty;
    public double        Guven   { get; init; }
    public KararKaynagi  Kaynak  { get; init; }
    /// <summary>Seçenek başına olasılık dağılımı (sağlayıcı veriyorsa).</summary>
    public IReadOnlyDictionary<string, double>? Dagilim { get; init; }
    public string?       Gerekce { get; init; }
}

public sealed class PuanKarari
{
    public int           Puan    { get; init; }
    public double        Guven   { get; init; }
    public KararKaynagi  Kaynak  { get; init; }
    public string?       Gerekce { get; init; }
}
