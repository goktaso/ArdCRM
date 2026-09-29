namespace SevkiyatKoli.Core;

public interface IKoliService
{
    /// <summary>Sevkiyat için yeni bir koli açar ve numarasını üretir.</summary>
    Sonuc<Koli> KoliAc(string irsaliyeNo);

    /// <summary>Açık koliye satır ekler. Kapalı/iptal koliye eklenemez; brüt 30 kg'ı aşamaz.</summary>
    Sonuc SatirEkle(string koliNo, KoliSatiri satir);

    /// <summary>Koliyi kapatır. Kapandıktan sonra içeriği değişmez.</summary>
    Sonuc KoliKapat(string koliNo);

    /// <summary>Koliyi iptal eder. Numara serbest kalmaz, yeniden kullanılmaz.</summary>
    Sonuc KoliIptal(string koliNo);

    Sonuc<Koli> KoliGetir(string koliNo);
    Sonuc<Sevkiyat> SevkiyatGetir(string irsaliyeNo);
}
