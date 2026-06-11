using ArdCRM.Core;
using ArdCRM.Core.Entities;
using ArdCRM.Core.Enums;
using ArdCRM.Core.Interfaces;

namespace ArdCRM.Business.Services;

public class ErpSenkronService : IErpSenkronService
{
    private readonly IErpAdapter _erp;
    private readonly IRepository<Musteri> _musteriRepo;

    public ErpSenkronService(IErpAdapter erp, IRepository<Musteri> musteriRepo)
    {
        _erp         = erp;
        _musteriRepo = musteriRepo;
    }

    public async Task<ServiceResult<IEnumerable<ErpMusteri>>> OnizlemeGetirAsync()
    {
        try
        {
            var baglantiOk = await _erp.TestConnectionAsync();
            if (!baglantiOk)
                return ServiceResult<IEnumerable<ErpMusteri>>.Fail("ERP bağlantısı kurulamadı.");

            var musteriler = await _erp.GetMusterilerAsync();
            return ServiceResult<IEnumerable<ErpMusteri>>.Ok(musteriler,
                $"{musteriler.Count()} müşteri bulundu.");
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<ErpMusteri>>.Fail($"ERP okuma hatası: {ex.Message}");
        }
    }

    public async Task<ServiceResult<ErpSenkronSonuc>> MusterileriAktarAsync(IEnumerable<string> cariKodlar)
    {
        var sonuc = new ErpSenkronSonuc();

        try
        {
            var baglantiOk = await _erp.TestConnectionAsync();
            if (!baglantiOk)
                return ServiceResult<ErpSenkronSonuc>.Fail("ERP bağlantısı kurulamadı.");

            var mevcutlar = (await _musteriRepo.GetAllActiveAsync()).ToList();

            foreach (var kod in cariKodlar)
            {
                try
                {
                    var erpMusteri = await _erp.GetMusteriByKodAsync(kod);
                    if (erpMusteri is null)
                    {
                        sonuc.Atlanan++;
                        sonuc.Hatalar.Add($"{kod}: ERP'de bulunamadı.");
                        continue;
                    }

                    // Mevcut kayıt var mı? (VergiNo ile eşleştir)
                    var mevcut = mevcutlar.FirstOrDefault(m =>
                        !string.IsNullOrEmpty(m.VergiNo) && m.VergiNo == erpMusteri.VergiNo);

                    if (mevcut is not null)
                    {
                        // Güncelle
                        mevcut.FirmaAdi      = erpMusteri.CariAdi;
                        mevcut.Telefon       = erpMusteri.Telefon;
                        mevcut.Email         = erpMusteri.Email;
                        mevcut.Adres         = erpMusteri.Adres;
                        mevcut.Sehir         = erpMusteri.Sehir;
                        mevcut.VergiDairesi  = erpMusteri.VergiDairesi;
                        await _musteriRepo.UpdateAsync(mevcut);
                        sonuc.Guncellenen++;
                    }
                    else
                    {
                        // Yeni ekle
                        var yeni = new Musteri
                        {
                            Ad           = erpMusteri.CariAdi.Split(' ').FirstOrDefault() ?? erpMusteri.CariAdi,
                            FirmaAdi     = erpMusteri.CariAdi,
                            Telefon      = erpMusteri.Telefon,
                            Email        = erpMusteri.Email,
                            Adres        = erpMusteri.Adres,
                            Sehir        = erpMusteri.Sehir,
                            VergiNo      = erpMusteri.VergiNo,
                            VergiDairesi = erpMusteri.VergiDairesi,
                            Tip          = MusteriTipi.Aktif,
                            Notlar       = $"ERP'den aktarıldı. Cari Kodu: {kod}",
                            Aktif        = erpMusteri.Aktif
                        };
                        await _musteriRepo.AddAsync(yeni);
                        sonuc.YeniEklenen++;
                    }
                }
                catch (Exception ex)
                {
                    sonuc.Atlanan++;
                    sonuc.Hatalar.Add($"{kod}: {ex.Message}");
                }
            }

            var mesaj = $"Tamamlandı — Yeni: {sonuc.YeniEklenen}, Güncellenen: {sonuc.Guncellenen}, Atlanan: {sonuc.Atlanan}";
            return ServiceResult<ErpSenkronSonuc>.Ok(sonuc, mesaj);
        }
        catch (Exception ex)
        {
            return ServiceResult<ErpSenkronSonuc>.Fail($"Senkronizasyon hatası: {ex.Message}");
        }
    }
}
