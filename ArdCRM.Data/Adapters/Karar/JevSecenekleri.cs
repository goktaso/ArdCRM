namespace ArdCRM.Data.Adapters.Karar;

/// <summary>
/// JEV (TypeSafe System One) bağlantı ayarları.
/// Değerler appsettings.{Ortam}.json (gitignore'da) veya ortam değişkeninden gelir;
/// API anahtarı hiçbir koşulda kaynak koda veya bağlam dosyalarına yazılmaz.
/// </summary>
public sealed class JevSecenekleri
{
    public const string BolumAdi = "Jev";

    /// <summary>Boşsa adapter devre dışıdır; sistem kural motoruyla çalışır.</summary>
    public string? ApiAnahtari { get; set; }

    public string TabanAdres { get; set; } = "https://api.typesafe.ai";

    public string UcNokta { get; set; } = "/v1/systemone";

    public string Model { get; set; } = "jev-latest";

    public int ZamanAsimiSaniye { get; set; } = 10;

    /// <summary>
    /// GÜVENLİK KİLİDİ. Varsayılan false.
    ///
    /// /v1/systemone istek ve yanıt şeması resmî TypeSafe dokümanıyla birebir
    /// doğrulanmadan true yapılmaz. false iken adapter hiçbir ağ çağrısı yapmaz;
    /// açık bir hata döner ve sistem kural motoruna düşer.
    ///
    /// Doğrulama adımları JevKararMotoru sınıfının başındaki nota yazılmıştır.
    /// </summary>
    public bool SemaDogrulandi { get; set; }
}
