using ArdCRM.Business.Services;
using ArdCRM.Core.Entities;
using ArdCRM.Core.Enums;
using ArdCRM.Core.Interfaces;
using Moq;

namespace ArdCRM.Tests;

public class TeklifServiceTests
{
    private readonly Mock<IRepository<Teklif>>  _mockTeklifRepo;
    private readonly Mock<IRepository<Musteri>> _mockMusteriRepo;
    private readonly TeklifService _service;

    public TeklifServiceTests()
    {
        _mockTeklifRepo  = new Mock<IRepository<Teklif>>();
        _mockMusteriRepo = new Mock<IRepository<Musteri>>();
        _service = new TeklifService(_mockTeklifRepo.Object, _mockMusteriRepo.Object);
    }

    // ── GetAll ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ListeVarsa_BasariliDoner()
    {
        var liste = new List<Teklif>
        {
            new() { Id = 1, TeklifNo = "TKL-2026-00001", Baslik = "Danışmanlık", Tutar = 10000, MusteriId = 1 },
            new() { Id = 2, TeklifNo = "TKL-2026-00002", Baslik = "ERP Kurulum",  Tutar = 25000, MusteriId = 1 }
        };
        _mockTeklifRepo.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(liste);

        var result = await _service.GetAllAsync();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count());
    }

    // ── GetByMusteri ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByMusteri_VarlanMusteriId_FiltreliListeDoner()
    {
        var liste = new List<Teklif>
        {
            new() { Id = 1, TeklifNo = "TKL-001", Baslik = "Teklif A", Tutar = 5000,  MusteriId = 1 },
            new() { Id = 2, TeklifNo = "TKL-002", Baslik = "Teklif B", Tutar = 8000,  MusteriId = 2 },
            new() { Id = 3, TeklifNo = "TKL-003", Baslik = "Teklif C", Tutar = 12000, MusteriId = 1 }
        };
        _mockTeklifRepo.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(liste);

        var result = await _service.GetByMusteriAsync(1);

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count());
        Assert.All(result.Data!, t => Assert.Equal(1, t.MusteriId));
    }

    // ── GetById ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_VarlanId_TeklifDoner()
    {
        var teklif = new Teklif { Id = 1, TeklifNo = "TKL-001", Baslik = "Test", Tutar = 5000, MusteriId = 1 };
        _mockTeklifRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(teklif);

        var result = await _service.GetByIdAsync(1);

        Assert.True(result.Success);
        Assert.Equal("TKL-001", result.Data!.TeklifNo);
    }

    [Fact]
    public async Task GetById_YanliId_HataDoner()
    {
        _mockTeklifRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Teklif?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.False(result.Success);
    }

    // ── Create ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_GecerliTeklif_BasariliDoner()
    {
        var teklif = new Teklif { Baslik = "Danışmanlık Hizmeti", Tutar = 15000, MusteriId = 1 };
        _mockMusteriRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockTeklifRepo.Setup(r => r.AddAsync(It.IsAny<Teklif>())).ReturnsAsync(teklif);

        var result = await _service.CreateAsync(teklif);

        Assert.True(result.Success);
        _mockTeklifRepo.Verify(r => r.AddAsync(It.IsAny<Teklif>()), Times.Once);
    }

    [Fact]
    public async Task Create_GecersizMusteri_HataDoner()
    {
        var teklif = new Teklif { Baslik = "Test", Tutar = 5000, MusteriId = 99 };
        _mockMusteriRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        var result = await _service.CreateAsync(teklif);

        Assert.False(result.Success);
        Assert.Contains("Geçersiz müşteri", result.Message);
        _mockTeklifRepo.Verify(r => r.AddAsync(It.IsAny<Teklif>()), Times.Never);
    }

    [Fact]
    public async Task Create_BaslikBos_HataDoner()
    {
        var teklif = new Teklif { Baslik = "", Tutar = 5000, MusteriId = 1 };
        _mockMusteriRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        var result = await _service.CreateAsync(teklif);

        Assert.False(result.Success);
        Assert.Contains("Teklif başlığı zorunludur.", result.Errors);
    }

    [Fact]
    public async Task Create_TutarSifir_HataDoner()
    {
        var teklif = new Teklif { Baslik = "Test Teklif", Tutar = 0, MusteriId = 1 };
        _mockMusteriRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        var result = await _service.CreateAsync(teklif);

        Assert.False(result.Success);
        Assert.Contains("Teklif tutarı sıfırdan büyük olmalıdır.", result.Errors);
    }

    [Fact]
    public async Task Create_TutarNegatif_HataDoner()
    {
        var teklif = new Teklif { Baslik = "Test", Tutar = -100, MusteriId = 1 };
        _mockMusteriRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        var result = await _service.CreateAsync(teklif);

        Assert.False(result.Success);
    }

    // ── Delete ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_VarlanId_BasariliDoner()
    {
        _mockTeklifRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockTeklifRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.True(result.Success);
        _mockTeklifRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Delete_YokId_HataDoner()
    {
        _mockTeklifRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        var result = await _service.DeleteAsync(99);

        Assert.False(result.Success);
        _mockTeklifRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    // ── TeklifNo ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GenerateTeklifNo_HerZaman_TKLIleBaslar()
    {
        var result = await _service.GenerateTeklifNoAsync();

        Assert.True(result.Success);
        Assert.StartsWith("TKL-", result.Data);
    }

    [Fact]
    public async Task GenerateTeklifNo_HerZaman_YilIcerir()
    {
        var result = await _service.GenerateTeklifNoAsync();

        Assert.Contains(DateTime.Now.Year.ToString(), result.Data);
    }
}
