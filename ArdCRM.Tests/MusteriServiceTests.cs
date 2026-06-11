using ArdCRM.Business.Services;
using ArdCRM.Core.Entities;
using ArdCRM.Core.Enums;
using ArdCRM.Core.Interfaces;
using Moq;

namespace ArdCRM.Tests;

public class MusteriServiceTests
{
    private readonly Mock<IRepository<Musteri>> _mockRepo;
    private readonly MusteriService _service;

    public MusteriServiceTests()
    {
        _mockRepo = new Mock<IRepository<Musteri>>();
        _service  = new MusteriService(_mockRepo.Object);
    }

    // ── GetAll ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ListeVarsa_BasariliDoner()
    {
        var liste = new List<Musteri>
        {
            new() { Id = 1, Ad = "Ali", FirmaAdi = "ARD Sistem", Aktif = true },
            new() { Id = 2, Ad = "Veli", FirmaAdi = "Test Firma", Aktif = true }
        };
        _mockRepo.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(liste);

        var result = await _service.GetAllAsync();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetAll_BosList_BasariliVeBosDoner()
    {
        _mockRepo.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(new List<Musteri>());

        var result = await _service.GetAllAsync();

        Assert.True(result.Success);
        Assert.Empty(result.Data!);
    }

    // ── GetById ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_VarlanId_MusteriDoner()
    {
        var musteri = new Musteri { Id = 1, Ad = "Ali", FirmaAdi = "ARD Sistem", Aktif = true };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(musteri);

        var result = await _service.GetByIdAsync(1);

        Assert.True(result.Success);
        Assert.Equal("ARD Sistem", result.Data!.FirmaAdi);
    }

    [Fact]
    public async Task GetById_YanliId_HataDoner()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Musteri?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.False(result.Success);
        Assert.Contains("99", result.Message);
    }

    // ── Create ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_GecerliMusteri_BasariliDoner()
    {
        var musteri = new Musteri { Ad = "Ali", FirmaAdi = "ARD Sistem", Email = "ali@ard.com" };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Musteri>())).ReturnsAsync(musteri);

        var result = await _service.CreateAsync(musteri);

        Assert.True(result.Success);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Musteri>()), Times.Once);
    }

    [Fact]
    public async Task Create_AdiBos_HataDoner()
    {
        var musteri = new Musteri { Ad = "", FirmaAdi = "ARD Sistem" };

        var result = await _service.CreateAsync(musteri);

        Assert.False(result.Success);
        Assert.Contains("Ad alanı zorunludur.", result.Errors);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Musteri>()), Times.Never);
    }

    [Fact]
    public async Task Create_FirmaAdiBos_HataDoner()
    {
        var musteri = new Musteri { Ad = "Ali", FirmaAdi = "" };

        var result = await _service.CreateAsync(musteri);

        Assert.False(result.Success);
        Assert.Contains("Firma adı zorunludur.", result.Errors);
    }

    [Fact]
    public async Task Create_GecersizEmail_HataDoner()
    {
        var musteri = new Musteri { Ad = "Ali", FirmaAdi = "ARD Sistem", Email = "gecersiz-email" };

        var result = await _service.CreateAsync(musteri);

        Assert.False(result.Success);
        Assert.Contains("Geçerli bir e-posta adresi giriniz.", result.Errors);
    }

    [Fact]
    public async Task Create_CokluHata_TumHatalarDoner()
    {
        var musteri = new Musteri { Ad = "", FirmaAdi = "" };

        var result = await _service.CreateAsync(musteri);

        Assert.False(result.Success);
        Assert.Equal(2, result.Errors.Count);
    }

    // ── Update ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_VarlanMusteri_BasariliDoner()
    {
        var musteri = new Musteri { Id = 1, Ad = "Ali", FirmaAdi = "ARD Güncel" };
        _mockRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Musteri>())).ReturnsAsync(musteri);

        var result = await _service.UpdateAsync(musteri);

        Assert.True(result.Success);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Musteri>()), Times.Once);
    }

    [Fact]
    public async Task Update_YokMusteriId_HataDoner()
    {
        var musteri = new Musteri { Id = 99, Ad = "Ali", FirmaAdi = "ARD Sistem" };
        _mockRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        var result = await _service.UpdateAsync(musteri);

        Assert.False(result.Success);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Musteri>()), Times.Never);
    }

    // ── Delete ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_VarlanId_BasariliDoner()
    {
        _mockRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.True(result.Success);
        _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Delete_YokId_HataDoner()
    {
        _mockRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        var result = await _service.DeleteAsync(99);

        Assert.False(result.Success);
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}
