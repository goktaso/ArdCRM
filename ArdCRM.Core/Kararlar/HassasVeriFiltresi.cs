using System.Text.RegularExpressions;

namespace ArdCRM.Core.Kararlar;

/// <summary>
/// Dış karar sağlayıcısına (JEV vb.) gönderilecek özniteliklerde hassas veri
/// olup olmadığını denetler.
///
/// CLAUDE.md kuralı: "Hassas veri → dış bulut servise gönderme."
/// Bu filtre o kuralın makine tarafından uygulanan hâlidir ve TEK yerde durur;
/// her çağrı noktasında tekrar yazılmaz.
///
/// Filtre iki katmanda çalışır:
///   1. Anahtar adı denetimi  — "vergiNo", "vergi_no", "VERGİ NO" hepsi yakalanır
///   2. Değer örüntü denetimi — e-posta, 10-11 haneli kimlik/vergi no, TR IBAN
///
/// Amaç sızıntıyı imkânsız kılmak değil, kaza eseri sızıntıyı derleme/çalışma
/// zamanında yakalamaktır. Yeni bir hassas alan çıktığında listeye eklenir.
/// </summary>
public sealed class HassasVeriFiltresi
{
    private static readonly string[] VarsayilanYasakliAnahtarlar =
    {
        "vergino", "vergidairesi", "tckimlik", "tckimlikno", "tckn", "kimlikno",
        "iban", "hesapno", "kartno", "krediKarti",
        "email", "eposta", "mail", "telefon", "gsm", "cep", "faks",
        "adres", "acikadres",
        "cariadi", "cariunvan", "unvan", "firmaadi", "musteriadi", "musterikodu",
        "ad", "soyad", "adsoyad", "tamad", "isim",
        "sifre", "password", "parola", "token", "apikey", "secret",
        "connectionstring", "baglantidizesi", "notlar", "aciklama"
    };

    private static readonly Regex EpostaDeseni =
        new(@"[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}", RegexOptions.Compiled);

    private static readonly Regex KimlikVergiDeseni =
        new(@"\b\d{10,11}\b", RegexOptions.Compiled);

    private static readonly Regex IbanDeseni =
        new(@"\bTR\d{24}\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>Serbest metin sayılan ve bu nedenle reddedilen değer uzunluğu.</summary>
    private const int SerbestMetinEsigi = 120;

    private readonly HashSet<string> _yasakliAnahtarlar;

    public HassasVeriFiltresi(IEnumerable<string>? ekYasakliAnahtarlar = null)
    {
        _yasakliAnahtarlar = new HashSet<string>(StringComparer.Ordinal);

        foreach (var anahtar in VarsayilanYasakliAnahtarlar)
            _yasakliAnahtarlar.Add(Normalize(anahtar));

        if (ekYasakliAnahtarlar is not null)
            foreach (var anahtar in ekYasakliAnahtarlar)
                _yasakliAnahtarlar.Add(Normalize(anahtar));
    }

    /// <summary>
    /// İhlalleri döner. Boş liste = gönderilebilir.
    /// </summary>
    public IReadOnlyList<string> Dogrula(IReadOnlyDictionary<string, object?> ozellikler)
    {
        var ihlaller = new List<string>();

        foreach (var (anahtar, deger) in ozellikler)
        {
            if (_yasakliAnahtarlar.Contains(Normalize(anahtar)))
            {
                ihlaller.Add($"'{anahtar}' hassas alan adı — dış sağlayıcıya gönderilemez.");
                continue;
            }

            if (deger is not string metin || metin.Length == 0)
                continue;

            if (metin.Length > SerbestMetinEsigi)
                ihlaller.Add($"'{anahtar}' serbest metin görünüyor ({metin.Length} karakter) — türetilmiş öznitelik gönderin.");
            else if (EpostaDeseni.IsMatch(metin))
                ihlaller.Add($"'{anahtar}' e-posta adresi içeriyor.");
            else if (IbanDeseni.IsMatch(metin))
                ihlaller.Add($"'{anahtar}' IBAN içeriyor.");
            else if (KimlikVergiDeseni.IsMatch(metin))
                ihlaller.Add($"'{anahtar}' 10-11 haneli kimlik/vergi numarası içeriyor.");
        }

        return ihlaller;
    }

    /// <summary>Büyük/küçük harf, boşluk, alt çizgi ve Türkçe karakter farklarını siler.</summary>
    private static string Normalize(string anahtar)
    {
        var tampon = new char[anahtar.Length];
        var uzunluk = 0;

        foreach (var karakter in anahtar.ToLowerInvariant())
        {
            var duz = karakter switch
            {
                'ı' or 'i' or 'î' => 'i',
                'ö' => 'o',
                'ü' => 'u',
                'ş' => 's',
                'ğ' => 'g',
                'ç' => 'c',
                _   => karakter
            };

            if (char.IsLetterOrDigit(duz))
                tampon[uzunluk++] = duz;
        }

        return new string(tampon, 0, uzunluk);
    }
}
