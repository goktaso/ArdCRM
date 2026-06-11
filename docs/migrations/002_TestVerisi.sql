-- ============================================================
-- ArdCRM — Test Verisi
-- Çalıştırma: SSMS'de ArdCRM veritabanı seçiliyken çalıştır
-- ============================================================

USE ArdCRM;
GO

-- Mevcut test verisini temizle (isteğe bağlı)
-- DELETE FROM Teklifler;
-- DELETE FROM Musteriler;

-- ============================================================
-- MÜŞTERİLER
-- Tip: 0=Potansiyel, 1=Aktif, 2=Pasif, 3=VIP
-- ============================================================

INSERT INTO Musteriler (Ad, Soyad, FirmaAdi, Telefon, Email, Adres, Sehir, VergiNo, VergiDairesi, Tip, Notlar, OlusturmaTarihi, Aktif)
VALUES
-- VIP Müşteriler
('Mehmet', 'Yılmaz', 'Yılmaz Ambalaj San. A.Ş.', '0232 456 78 90', 'mehmet.yilmaz@yilmazambalaj.com.tr', 'Atatürk Cad. No:45 Konak', 'İzmir', '1234567890', 'Konak V.D.', 3, 'FSC sertifikalı kraft torba üreticisi. Yıllık 500 ton kapasite.', DATEADD(month, -8, GETDATE()), 1),
('Ayşe', 'Kaya', 'Kaya Kağıt Ürünleri Ltd. Şti.', '0232 321 65 40', 'ayse@kayakagit.com', 'Organize San. Bölgesi 3. Cadde', 'İzmir', '9876543210', 'Gaziemir V.D.', 3, 'FSC CoC belgeli. ERP entegrasyonu öncelikli.', DATEADD(month, -6, GETDATE()), 1),

-- Aktif Müşteriler
('Ali', 'Demir', 'Artpack Kağıt Ambalaj', '0236 123 45 67', 'ali.demir@artpack.com.tr', 'Manisa OSB 5. Sokak No:12', 'Manisa', '1122334455', 'Manisa V.D.', 1, 'Kraft torba ihracatı yapıyor. FSC denetimi Haziran 2026.', DATEADD(month, -5, GETDATE()), 1),
('Fatma', 'Çelik', 'İzmir Kraft Torba Fab.', '0232 789 01 23', 'fcelik@izmirkraft.com', 'Kemalpaşa San. Sit. B Blok', 'İzmir', '5566778899', 'Kemalpaşa V.D.', 1, 'Yıllık FSC denetimi yapılıyor. Stok takip sistemi kurmak istiyor.', DATEADD(month, -4, GETDATE()), 1),
('Hasan', 'Öztürk', 'Batı Kraft Torba A.Ş.', '0232 654 32 10', 'hasan.ozturk@batikraft.com.tr', 'Torbalı OSB 2. Cadde No:8', 'İzmir', '3344556677', 'Torbalı V.D.', 1, 'Büyük müşteri. ERP entegrasyonu ve FSC danışmanlığı talep etti.', DATEADD(month, -3, GETDATE()), 1),
('Zeynep', 'Arslan', 'Ege Selüloz Sanayi', '0232 987 65 43', 'z.arslan@egeseluloz.com', 'Bornova San. Sitesi No:22', 'İzmir', '7788990011', 'Bornova V.D.', 1, 'Hammadde tedarikçisi. Depo yönetimi danışmanlığı görüşülüyor.', DATEADD(month, -2, GETDATE()), 1),
('Mustafa', 'Şahin', 'Marmara Ambalaj Ltd.', '0212 345 67 89', 'mshain@marmaraambalaj.com', 'İkitelli OSB Tekstil Blok', 'İstanbul', '2233445566', 'İkitelli V.D.', 1, 'İstanbul kökenli. FSC danışmanlığı için İzmir ofisimizle görüştü.', DATEADD(month, -2, GETDATE()), 1),
('Elif', 'Koç', 'Koç Kağıt San. Tic. Ltd.', '0224 567 89 01', 'elif.koc@kockagit.com', 'Bursa OSB 8. Cadde', 'Bursa', '4455667788', 'Bursa V.D.', 1, 'Yeni müşteri. ERP kurulum teklifi verildi.', DATEADD(month, -1, GETDATE()), 1),

-- Potansiyel Müşteriler
('Ahmet', 'Yıldız', 'Yıldız Karton Kutu', '0232 111 22 33', 'ahmet@yildizkarton.com', 'Buca San. Sitesi No:5', 'İzmir', '6677889900', 'Buca V.D.', 0, 'LinkedIn üzerinden iletişime geçti. FSC belgesi almak istiyor.', DATEADD(month, -1, GETDATE()), 1),
('Selin', 'Güler', 'Güler Ambalaj Plastik', '0232 444 55 66', 'selin@gulerambalaj.com', 'Gaziemir San. Bölgesi', 'İzmir', '8899001122', 'Gaziemir V.D.', 0, 'Rakip firmadan geçme ihtimali var. Fiyat teklifi bekleniyor.', DATEADD(day, -20, GETDATE()), 1),
('Burak', 'Aydın', 'Aydın Kağıt ve Ambalaj', '0256 789 01 23', 'burak@aydinkagit.com', 'Aydın OSB 4. Blok', 'Aydın', '0011223344', 'Aydın V.D.', 0, 'Fuar'da tanıştık. Tedarik zinciri danışmanlığı ilgileniyor.', DATEADD(day, -15, GETDATE()), 1),
('Deniz', 'Polat', 'Polat Endüstriyel Ambalaj', '0232 222 33 44', 'deniz.polat@polatambalaj.com', 'Pınarbaşı San. Sitesi', 'İzmir', '1100223344', 'Karşıyaka V.D.', 0, 'Web sitesi üzerinden form doldurdu. Geri aranacak.', DATEADD(day, -10, GETDATE()), 1),

-- Pasif Müşteriler
('Cengiz', 'Bakır', 'Bakır Selüloz Ltd.', '0232 333 44 55', NULL, 'Çiğli San. Bölgesi', 'İzmir', '9900112233', 'Çiğli V.D.', 2, '2025 yılında çalıştık. Şu an pasif. Tekrar aktifleşebilir.', DATEADD(month, -12, GETDATE()), 1);

GO

PRINT 'Musteriler eklendi: 13 kayıt';

-- ============================================================
-- TEKLİFLER
-- Durum: 0=Taslak, 1=Gönderildi, 2=Görüşülüyor, 3=Onaylandı, 4=Reddedildi, 5=İptal
-- ============================================================

INSERT INTO Teklifler (TeklifNo, MusteriId, Baslik, Aciklama, Tutar, Para, TeklifTarihi, GecerlilikTarihi, Durum, Notlar, OlusturmaTarihi, Aktif)
VALUES
-- Yılmaz Ambalaj (Id=1, VIP)
('TKL-2026-00001', 1, 'FSC CoC Denetim Danışmanlığı', 'Yıllık FSC Chain of Custody denetim hazırlık ve danışmanlık hizmeti. Süreç analizi, dokümantasyon hazırlama ve denetim gözlemleri dahil.', 45000, 'TRY', DATEADD(month, -3, GETDATE()), DATEADD(month, -1, GETDATE()), 3, 'Onaylandı ve başlandı. İkinci faz için yeni teklif hazırlanacak.', DATEADD(month, -3, GETDATE()), 1),
('TKL-2026-00002', 1, 'Netsis ERP Entegrasyonu - Faz 2', 'Mevcut Netsis ERP sistemine FSC mass balance modülü entegrasyonu. TBLSTHAR ve TBLSERITRA tablolarına özel raporlama.', 85000, 'TRY', DATEADD(month, -1, GETDATE()), DATEADD(month, 1, GETDATE()), 2, 'Teknik şartname inceleniyor.', DATEADD(month, -1, GETDATE()), 1),

-- Kaya Kağıt (Id=2, VIP)
('TKL-2026-00003', 2, 'Depo Yönetim Sistemi Kurulumu', 'Barkod tabanlı depo yönetim sistemi kurulum ve eğitim hizmeti. 3 depo, 2 kullanıcı lisansı.', 62000, 'TRY', DATEADD(month, -2, GETDATE()), DATEADD(day, 15, GETDATE()), 2, 'Demo yapıldı, müşteri memnun. Sözleşme aşamasında.', DATEADD(month, -2, GETDATE()), 1),
('TKL-2026-00004', 2, 'Yıllık Bakım ve Destek Anlaşması', 'Depo Yönetim Sistemi yıllık bakım, güncelleme ve teknik destek paketi.', 18000, 'TRY', DATEADD(month, -2, GETDATE()), DATEADD(month, 1, GETDATE()), 1, 'Kurulum teklifi onaylanırsa bu da onaylanacak.', DATEADD(month, -2, GETDATE()), 1),

-- Artpack (Id=3, Aktif)
('TKL-2026-00005', 3, 'FSC Sertifikasyon Danışmanlığı', 'Sıfırdan FSC CoC belgesi alım süreci danışmanlığı. Başvuru, denetim hazırlığı ve belgelendirme aşamaları.', 38000, 'TRY', DATEADD(month, -4, GETDATE()), DATEADD(month, -2, GETDATE()), 3, 'Proje tamamlandı. FSC belgesi alındı. Referans müşteri.', DATEADD(month, -4, GETDATE()), 1),
('TKL-2026-00006', 3, 'Logo ERP - Stok Modülü Entegrasyonu', 'Logo Tiger ERP stok modülüne FSC lot takip entegrasyonu.', 52000, 'TRY', DATEADD(month, -1, GETDATE()), DATEADD(month, 2, GETDATE()), 1, 'Teknik ekiple görüşme planlandı.', DATEADD(month, -1, GETDATE()), 1),

-- İzmir Kraft (Id=4, Aktif)
('TKL-2026-00007', 4, 'Stok Yönetim Sistemi Analiz ve Tasarım', 'Mevcut stok süreçlerinin analizi, iyileştirme önerileri ve yeni sistem tasarım raporu.', 15000, 'TRY', DATEADD(month, -3, GETDATE()), DATEADD(month, -1, GETDATE()), 3, 'Rapor teslim edildi. İkinci faz için bekleniyor.', DATEADD(month, -3, GETDATE()), 1),
('TKL-2026-00008', 4, 'Stok Yönetim Sistemi Geliştirme', 'Analiz raporuna dayalı özel stok yönetim yazılımı geliştirme ve kurulum.', 95000, 'TRY', DATEADD(month, -1, GETDATE()), DATEADD(month, 2, GETDATE()), 0, 'Analiz raporu tamamlandı, geliştirme teklifi taslak aşamasında.', DATEADD(month, -1, GETDATE()), 1),

-- Batı Kraft (Id=5, Aktif)
('TKL-2026-00009', 5, 'Tedarik Zinciri Danışmanlığı', 'Hammadde tedarikten ürün teslimine kadar tüm tedarik zinciri süreç optimizasyonu.', 75000, 'TRY', DATEADD(month, -2, GETDATE()), DATEADD(month, 1, GETDATE()), 2, 'İkinci toplantı yapıldı. Rakip firma da teklif vermiş.', DATEADD(month, -2, GETDATE()), 1),
('TKL-2026-00010', 5, 'FSC + Tedarik Zinciri Paket', 'FSC danışmanlığı ve tedarik zinciri optimizasyonu kombine paket teklifi.', 110000, 'TRY', DATEADD(day, -10, GETDATE()), DATEADD(month, 2, GETDATE()), 1, 'Paket teklif daha cazip. Yönlendirme yapılıyor.', DATEADD(day, -10, GETDATE()), 1),

-- Ege Selüloz (Id=6, Aktif)
('TKL-2026-00011', 6, 'Depo Optimizasyon Projesi', 'Mevcut depo düzeninin analizi ve optimizasyon önerileri.', 22000, 'TRY', DATEADD(month, -1, GETDATE()), DATEADD(month, 1, GETDATE()), 1, 'Saha ziyareti yapıldı.', DATEADD(month, -1, GETDATE()), 1),

-- Marmara Ambalaj (Id=7, Aktif)
('TKL-2026-00012', 7, 'Uzaktan FSC Danışmanlık Paketi', 'Online görüşme tabanlı FSC CoC danışmanlık hizmeti. Aylık 4 saat, 6 aylık paket.', 28000, 'TRY', DATEADD(day, -20, GETDATE()), DATEADD(month, 1, GETDATE()), 2, 'İstanbul müşterisi için uzaktan model uygun.', DATEADD(day, -20, GETDATE()), 1),

-- Koç Kağıt (Id=8, Aktif)
('TKL-2026-00013', 8, 'Netsis ERP Kurulum ve Eğitim', 'Yeni Netsis ERP kurulumu, veri aktarımı ve kullanıcı eğitimi. 5 kullanıcı lisansı.', 125000, 'TRY', DATEADD(day, -15, GETDATE()), DATEADD(month, 2, GETDATE()), 0, 'İlk görüşme yapıldı. İhtiyaç analizi bekleniyor.', DATEADD(day, -15, GETDATE()), 1),

-- Yıldız Karton (Id=9, Potansiyel)
('TKL-2026-00014', 9, 'FSC Ön Değerlendirme', 'FSC sertifikasyon sürecine hazırlık için ön değerlendirme ve yol haritası raporu.', 8500, 'TRY', DATEADD(day, -7, GETDATE()), DATEADD(month, 1, GETDATE()), 0, 'LinkedIn görüşmesi sonrası hazırlandı.', DATEADD(day, -7, GETDATE()), 1),

-- Güler Ambalaj (Id=10, Potansiyel)
('TKL-2026-00015', 10, 'Genel Danışmanlık Paketi', 'Tedarik zinciri ve depo yönetimi genel değerlendirme paketi.', 12000, 'TRY', DATEADD(day, -5, GETDATE()), DATEADD(month, 1, GETDATE()), 1, 'Rakip firma baskısı var. Hızlı hareket edilmeli.', DATEADD(day, -5, GETDATE()), 1),

-- Reddedilen/İptal teklifler (gerçekçilik için)
('TKL-2025-00001', 5, 'ERP Tam Entegrasyon - Eski Teklif', 'Eski versiyon ERP entegrasyon teklifi.', 180000, 'TRY', DATEADD(month, -8, GETDATE()), DATEADD(month, -6, GETDATE()), 4, 'Bütçe aşıldı gerekçesiyle reddedildi. Yeni teklifle geri dönüldü.', DATEADD(month, -8, GETDATE()), 1),
('TKL-2025-00002', 13, 'Bakır Selüloz FSC Danışmanlığı', 'FSC belgelendirme danışmanlığı teklifi.', 35000, 'TRY', DATEADD(month, -10, GETDATE()), DATEADD(month, -8, GETDATE()), 5, 'Müşteri pasif konuma geçti. İptal edildi.', DATEADD(month, -10, GETDATE()), 1);

GO

PRINT 'Teklifler eklendi: 17 kayıt';
PRINT '';
PRINT 'Özet:';
SELECT
    COUNT(*) AS ToplamMusteri,
    SUM(CASE WHEN Tip = 3 THEN 1 ELSE 0 END) AS VIP,
    SUM(CASE WHEN Tip = 1 THEN 1 ELSE 0 END) AS Aktif,
    SUM(CASE WHEN Tip = 0 THEN 1 ELSE 0 END) AS Potansiyel,
    SUM(CASE WHEN Tip = 2 THEN 1 ELSE 0 END) AS Pasif
FROM Musteriler;

SELECT
    COUNT(*) AS ToplamTeklif,
    SUM(CASE WHEN Durum = 3 THEN 1 ELSE 0 END) AS Onaylandi,
    SUM(CASE WHEN Durum IN (1,2) THEN 1 ELSE 0 END) AS Bekliyor,
    SUM(CASE WHEN Durum = 0 THEN 1 ELSE 0 END) AS Taslak,
    SUM(Tutar) AS ToplamTutar,
    SUM(CASE WHEN Durum = 3 THEN Tutar ELSE 0 END) AS OnaylananTutar
FROM Teklifler;
GO
