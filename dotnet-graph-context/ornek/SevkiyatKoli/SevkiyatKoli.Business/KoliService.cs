using SevkiyatKoli.Core;

namespace SevkiyatKoli.Business;

/// <summary>
/// Koli iş kuralları. Veri bellekte tutulur (kapsam dışı: veritabanı).
///
/// Uygulanan kurallar — bkz. context/kurallar-ve-sinirlar.md:
///   1. Numara formatı {IrsaliyeNo}-{Sira:000}
///   2. Sıra 1'den başlar, üretimde atlanmaz
///   3. Kapalı/iptal koliye satır eklenemez
///   4. Brüt ağırlık MaksBrutAgirlikKg'ı aşamaz
///   5. Üretilen numara tekrar kullanılmaz
/// </summary>
public sealed class KoliService : IKoliService
{
    public const decimal MaksBrutAgirlikKg = 30m;

    private readonly Dictionary<string, Sevkiyat> _sevkiyatlar = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Koli>     _koliler     = new(StringComparer.Ordinal);

    public Sonuc<Koli> KoliAc(string irsaliyeNo)
    {
        if (string.IsNullOrWhiteSpace(irsaliyeNo))
            return Sonuc<Koli>.Hata("İrsaliye numarası zorunludur.");

        if (!_sevkiyatlar.TryGetValue(irsaliyeNo, out var sevkiyat))
        {
            sevkiyat = new Sevkiyat { IrsaliyeNo = irsaliyeNo };
            _sevkiyatlar[irsaliyeNo] = sevkiyat;
        }

        var siraNo = sevkiyat.SonrakiSiraNoUret();
        var koliNo = KoliNumaratoru.Uret(irsaliyeNo, siraNo);

        // Kural 5: numara tekrar kullanılmaz. Sayaç geri alınmadığı için bu
        // çakışma normalde imkânsızdır; yine de sessizce üzerine yazmayız.
        if (_koliler.ContainsKey(koliNo))
            return Sonuc<Koli>.Hata($"'{koliNo}' numarası daha önce üretilmiş — tekrar kullanılamaz.");

        var koli = new Koli
        {
            KoliNo     = koliNo,
            IrsaliyeNo = irsaliyeNo,
            SiraNo     = siraNo
        };

        _koliler[koliNo] = koli;
        sevkiyat.KoliEkle(koli);

        return Sonuc<Koli>.Ok(koli, $"{koliNo} açıldı.");
    }

    public Sonuc SatirEkle(string koliNo, KoliSatiri satir)
    {
        if (!_koliler.TryGetValue(koliNo, out var koli))
            return Sonuc.Hata($"'{koliNo}' bulunamadı.");

        // Kural 3
        if (koli.Durum != KoliDurumu.Acik)
            return Sonuc.Hata(
                $"'{koliNo}' {koli.Durum} durumunda; satır eklenemez. " +
                "Düzeltme yolu: koliyi iptal edip yeni koli açın.");

        if (satir.Miktar <= 0)
            return Sonuc.Hata("Miktar sıfırdan büyük olmalıdır.");

        if (satir.Agirlik < 0)
            return Sonuc.Hata("Ağırlık negatif olamaz.");

        // Kural 4 — satır eklendiğinde brüt ağırlık sınırı aşılıyor mu?
        var yeniBrut = koli.BrutAgirlik + satir.Agirlik;
        if (yeniBrut > MaksBrutAgirlikKg)
            return Sonuc.Hata(
                $"Brüt ağırlık sınırı aşılıyor: {yeniBrut:0.##} kg > {MaksBrutAgirlikKg:0.##} kg. " +
                $"Satır reddedildi ('{satir.StokKod}', {satir.Agirlik:0.##} kg).");

        koli.SatirEkle(satir);
        return Sonuc.Ok($"'{satir.StokKod}' eklendi. Brüt: {koli.BrutAgirlik:0.##} kg.");
    }

    public Sonuc KoliKapat(string koliNo)
    {
        if (!_koliler.TryGetValue(koliNo, out var koli))
            return Sonuc.Hata($"'{koliNo}' bulunamadı.");

        if (koli.Durum == KoliDurumu.Iptal)
            return Sonuc.Hata($"'{koliNo}' iptal edilmiş; kapatılamaz.");

        if (koli.Durum == KoliDurumu.Kapali)
            return Sonuc.Hata($"'{koliNo}' zaten kapalı.");

        if (koli.Satirlar.Count == 0)
            return Sonuc.Hata($"'{koliNo}' boş; kapatılamaz.");

        koli.Kapat();
        return Sonuc.Ok($"{koliNo} kapatıldı. Brüt: {koli.BrutAgirlik:0.##} kg.");
    }

    public Sonuc KoliIptal(string koliNo)
    {
        if (!_koliler.TryGetValue(koliNo, out var koli))
            return Sonuc.Hata($"'{koliNo}' bulunamadı.");

        if (koli.Durum == KoliDurumu.Iptal)
            return Sonuc.Hata($"'{koliNo}' zaten iptal.");

        koli.IptalEt();
        return Sonuc.Ok($"{koliNo} iptal edildi. Numara yeniden kullanılmayacak.");
    }

    public Sonuc<Koli> KoliGetir(string koliNo)
        => _koliler.TryGetValue(koliNo, out var koli)
            ? Sonuc<Koli>.Ok(koli)
            : Sonuc<Koli>.Hata($"'{koliNo}' bulunamadı.");

    public Sonuc<Sevkiyat> SevkiyatGetir(string irsaliyeNo)
        => _sevkiyatlar.TryGetValue(irsaliyeNo, out var sevkiyat)
            ? Sonuc<Sevkiyat>.Ok(sevkiyat)
            : Sonuc<Sevkiyat>.Hata($"'{irsaliyeNo}' bulunamadı.");
}
