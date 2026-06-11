-- ============================================================
-- ArdCRM — 001_InitialCreate.sql
-- Çalıştırma: SSMS'de ArdCRM veritabanı seçiliyken çalıştır
--             VEYA: dotnet ef database update
-- ============================================================

-- Veritabanı yoksa oluştur
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ArdCRM')
BEGIN
    CREATE DATABASE ArdCRM;
END
GO

USE ArdCRM;
GO

-- Musteriler tablosu
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Musteriler' AND xtype='U')
BEGIN
    CREATE TABLE Musteriler (
        Id                INT IDENTITY(1,1) PRIMARY KEY,
        Ad                NVARCHAR(100) NOT NULL,
        Soyad             NVARCHAR(100) NULL,
        FirmaAdi          NVARCHAR(200) NOT NULL,
        Telefon           NVARCHAR(20)  NULL,
        Email             NVARCHAR(150) NULL,
        Adres             NVARCHAR(500) NULL,
        Sehir             NVARCHAR(100) NULL,
        VergiNo           NVARCHAR(20)  NULL,
        VergiDairesi      NVARCHAR(100) NULL,
        Tip               INT NOT NULL DEFAULT 0,   -- 0:Potansiyel 1:Aktif 2:Pasif 3:VIP
        Notlar            NVARCHAR(MAX) NULL,
        OlusturmaTarihi   DATETIME NOT NULL DEFAULT GETDATE(),
        GuncellemeTarihi  DATETIME NULL,
        Aktif             BIT NOT NULL DEFAULT 1
    );
    PRINT 'Musteriler tablosu oluşturuldu.';
END
GO

-- Teklifler tablosu
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Teklifler' AND xtype='U')
BEGIN
    CREATE TABLE Teklifler (
        Id                INT IDENTITY(1,1) PRIMARY KEY,
        TeklifNo          NVARCHAR(20)  NOT NULL,
        MusteriId         INT NOT NULL,
        Baslik            NVARCHAR(300) NOT NULL,
        Aciklama          NVARCHAR(MAX) NULL,
        Tutar             DECIMAL(18,2) NOT NULL DEFAULT 0,
        Para              NVARCHAR(3)   NOT NULL DEFAULT 'TRY',
        TeklifTarihi      DATETIME NOT NULL DEFAULT GETDATE(),
        GecerlilikTarihi  DATETIME NULL,
        Durum             INT NOT NULL DEFAULT 0,   -- 0:Taslak 1:Gonderildi 2:Gorusuluyor 3:Onaylandi 4:Reddedildi 5:Iptal
        Notlar            NVARCHAR(MAX) NULL,
        OlusturmaTarihi   DATETIME NOT NULL DEFAULT GETDATE(),
        GuncellemeTarihi  DATETIME NULL,
        Aktif             BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Teklifler_Musteriler FOREIGN KEY (MusteriId)
            REFERENCES Musteriler(Id)
    );
    PRINT 'Teklifler tablosu oluşturuldu.';
END
GO

PRINT 'Migration 001_InitialCreate tamamlandı.';
GO
