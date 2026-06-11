using ArdCRM.Core.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Text;

namespace ArdCRM.Data.Adapters;

/// <summary>
/// Netsis ERP (ACORE23) veritabanından READ-ONLY veri okur.
/// KURAL: Bu adapter asla INSERT/UPDATE/DELETE yapmaz.
/// Encoding: Netsis Windows-1254 (Turkish) kullanır, okurken düzeltilir.
/// </summary>
public class NetsisAdapter : IErpAdapter
{
    private readonly string _connectionString;

    public NetsisAdapter(string connectionString)
    {
        _connectionString = connectionString;
        // Windows-1254 encoding desteği
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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
                CARI_KOD                                AS CariKod,
                RTRIM(LTRIM(CARI_ISIM))                 AS CariAdi,
                RTRIM(LTRIM(ISNULL(CARI_TEL, '')))      AS Telefon,
                RTRIM(LTRIM(ISNULL(EMAIL, '')))         AS Email,
                RTRIM(LTRIM(ISNULL(CARI_ADRES, '')))    AS Adres,
                RTRIM(LTRIM(ISNULL(CARI_IL, '')))       AS Sehir,
                RTRIM(LTRIM(ISNULL(VERGI_NUMARASI, ''))) AS VergiNo,
                RTRIM(LTRIM(ISNULL(VERGI_DAIRESI, ''))) AS VergiDairesi,
                CASE WHEN ISNULL(UPDATE_KODU,'') = 'P' THEN 0 ELSE 1 END AS Aktif
            FROM TBLCASABIT WITH (NOLOCK)
            WHERE CARI_KOD IS NOT NULL
              AND LEN(RTRIM(LTRIM(CARI_ISIM))) > 0
              AND CARI_TIP = 'A'
              AND ISNULL(UPDATE_KODU,'') != 'S'
            ORDER BY CARI_ISIM
            """;

        await using var conn = new SqlConnection(_connectionString);
        var result = await conn.QueryAsync<ErpMusteri>(sql);

        // Encoding düzeltme — Netsis bazı sürümlerde Türkçe karakterleri bozuk saklar
        return result.Select(DuzeltEncoding).ToList();
    }

    public async Task<ErpMusteri?> GetMusteriByKodAsync(string cariKod)
    {
        const string sql = """
            SELECT
                CARI_KOD                                AS CariKod,
                RTRIM(LTRIM(CARI_ISIM))                 AS CariAdi,
                RTRIM(LTRIM(ISNULL(CARI_TEL, '')))      AS Telefon,
                RTRIM(LTRIM(ISNULL(EMAIL, '')))         AS Email,
                RTRIM(LTRIM(ISNULL(CARI_ADRES, '')))    AS Adres,
                RTRIM(LTRIM(ISNULL(CARI_IL, '')))       AS Sehir,
                RTRIM(LTRIM(ISNULL(VERGI_NUMARASI, ''))) AS VergiNo,
                RTRIM(LTRIM(ISNULL(VERGI_DAIRESI, ''))) AS VergiDairesi,
                CASE WHEN ISNULL(UPDATE_KODU,'') = 'P' THEN 0 ELSE 1 END AS Aktif
            FROM TBLCASABIT WITH (NOLOCK)
            WHERE CARI_KOD = @CariKod
            """;

        await using var conn = new SqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<ErpMusteri>(sql, new { CariKod = cariKod });
        return result is null ? null : DuzeltEncoding(result);
    }

    public async Task<IEnumerable<ErpStokHareketi>> GetStokHareketleriAsync(DateTime baslangic, DateTime bitis)
    {
        const string sql = """
            SELECT
                STOK_KODU       AS StokKod,
                STOK_ADI        AS StokAdi,
                MIKTAR          AS Miktar,
                BIRIM_ADI       AS Birim,
                TARIH           AS Tarih,
                EVRAK_NO        AS BelgeNo,
                HAREKET_TURU    AS HareketTip
            FROM TBLSTHAR WITH (NOLOCK)
            WHERE TARIH BETWEEN @Baslangic AND @Bitis
            ORDER BY TARIH DESC
            """;

        await using var conn = new SqlConnection(_connectionString);
        var result = await conn.QueryAsync<ErpStokHareketi>(sql, new { Baslangic = baslangic, Bitis = bitis });
        return result;
    }

    /// <summary>
    /// Netsis Windows-1254 encoding bozukluklarını düzeltir.
    /// Örnek: "ÝZMÝR" → "İZMİR", "ÞÝRKET" → "ŞİRKET"
    /// </summary>
    private static ErpMusteri DuzeltEncoding(ErpMusteri m)
    {
        m.CariAdi       = DuzeltStr(m.CariAdi);
        m.Adres         = DuzeltStr(m.Adres);
        m.Sehir         = DuzeltStr(m.Sehir);
        m.VergiDairesi  = DuzeltStr(m.VergiDairesi);
        return m;
    }

    private static string? DuzeltStr(string? s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        // Windows-1254 → UTF-8 dönüşüm tablosu (Netsis'in bilinen bozuk karakterleri)
        return s
            .Replace("Ý", "İ").Replace("ý", "ı")
            .Replace("Þ", "Ş").Replace("þ", "ş")
            .Replace("Ð", "Ğ").Replace("ð", "ğ")
            .Trim();
    }
}
