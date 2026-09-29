namespace SevkiyatKoli.Core;

/// <summary>
/// Koli numarası üretimi ve biçim doğrulaması.
///
/// Kural 1: format {IrsaliyeNo}-{Sira:000}   örn. SEV2026001-003
/// Kural 2: sıra numarası sevkiyat içinde 1'den başlar ve üretimde atlanmaz
/// Kural 5: üretilen numara tekrar kullanılmaz (iptal edilse bile)
/// </summary>
public static class KoliNumaratoru
{
    public const int SiraBasamakSayisi = 3;

    public static string Uret(string irsaliyeNo, int siraNo)
    {
        if (string.IsNullOrWhiteSpace(irsaliyeNo))
            throw new ArgumentException("İrsaliye numarası boş olamaz.", nameof(irsaliyeNo));

        if (siraNo < 1)
            throw new ArgumentOutOfRangeException(nameof(siraNo), "Sıra numarası 1'den küçük olamaz.");

        return $"{irsaliyeNo}-{siraNo.ToString($"D{SiraBasamakSayisi}")}";
    }

    /// <summary>Biçim denetimi. Sıra 999'u aşarsa basamak sayısı büyür, bu geçerlidir.</summary>
    public static bool BicimGecerli(string? koliNo, string irsaliyeNo)
    {
        if (string.IsNullOrWhiteSpace(koliNo))
            return false;

        var onEk = irsaliyeNo + "-";
        if (!koliNo.StartsWith(onEk, StringComparison.Ordinal))
            return false;

        var siraMetni = koliNo[onEk.Length..];

        return siraMetni.Length >= SiraBasamakSayisi
               && siraMetni.All(char.IsDigit)
               && int.TryParse(siraMetni, out var sira)
               && sira >= 1;
    }
}
