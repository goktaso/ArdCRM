using ArdCRM.Core.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ArdCRM.Data.Adapters;

/// <summary>
/// Netsis ERP veritabanından READ-ONLY veri okur.
/// Tablo yapısı: TBLCASABIT (cari), TBLSTHAR (stok hareketleri)
/// KURAL: Bu adapter asla INSERT/UPDATE/DELETE yapmaz.
/// </summary>
public class NetsisAdapter : IErpAdapter
{
    private readonly string _connectionString;

    public NetsisAdapter(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<ErpMusteri>> GetMusterilerAsync()
    {
        const string sql = """
            SELECT
                CARI_KOD       AS CariKod,
                CARI_ISIM      AS CariAdi,
                CARI_TEL1      AS Telefon,
                CARI_EMAIL     AS Email,
                CARI_ADRES1    AS Adres,
                CARI_SEHIR     AS Sehir,
                CARI_VERGINO   AS VergiNo,
                CARI_VERGIDAIRE AS VergiDairesi,
                CASE WHEN PASIF = 0 THEN 1 ELSE 0 END AS Aktif
            FROM TBLCASABIT WITH (NOLOCK)
            WHERE CARI_KOD IS NOT NULL
              AND CARI_ISIM IS NOT NULL
            ORDER BY CARI_ISIM
            """;

        await using var conn = new SqlConnection(_connectionString);
        var result = await conn.QueryAsync<ErpMusteri>(sql);
        return result;
    }

    public async Task<ErpMusteri?> GetMusteriByKodAsync(string cariKod)
    {
        const string sql = """
            SELECT
                CARI_KOD        AS CariKod,
                CARI_ISIM       AS CariAdi,
                CARI_TEL1       AS Telefon,
                CARI_EMAIL      AS Email,
                CARI_ADRES1     AS Adres,
                CARI_SEHIR      AS Sehir,
                CARI_VERGINO    AS VergiNo,
                CARI_VERGIDAIRE AS VergiDairesi,
                CASE WHEN PASIF = 0 THEN 1 ELSE 0 END AS Aktif
            FROM TBLCASABIT WITH (NOLOCK)
            WHERE CARI_KOD = @CariKod
            """;

        await using var conn = new SqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<ErpMusteri>(sql, new { CariKod = cariKod });
    }

    public async Task<IEnumerable<ErpStokHareketi>> GetStokHareketleriAsync(DateTime baslangic, DateTime bitis)
    {
        const string sql = """
            SELECT
                STOK_KOD    AS StokKod,
                STOK_ADI    AS StokAdi,
                MIKTAR      AS Miktar,
                BIRIM       AS Birim,
                TARIH       AS Tarih,
                BELGE_NO    AS BelgeNo,
                HAREKET_TIP AS HareketTip
            FROM TBLSTHAR WITH (NOLOCK)
            WHERE TARIH BETWEEN @Baslangic AND @Bitis
            ORDER BY TARIH DESC
            """;

        await using var conn = new SqlConnection(_connectionString);
        var result = await conn.QueryAsync<ErpStokHareketi>(sql, new { Baslangic = baslangic, Bitis = bitis });
        return result;
    }
}
