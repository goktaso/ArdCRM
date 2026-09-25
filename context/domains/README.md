# Domain Notları — ArdCRM

Bir domain tekrar tekrar bağlam gerektiriyorsa burada bir dosya açılır ve
`../README.md` haritasındaki tabloya eklenir.

## Mevcut domain dosyaları

| Dosya | Sorumluluk |
|---|---|
| `musteri.md` | Müşteri/cari kaydı, liste, detay, soft delete |
| `teklif.md` | Teklif CRUD, teklif no üretimi, durum akışı |
| `erp-senkron.md` | ERP (Netsis) okuma, önizleme ve seçerek aktarım |
| `veri-ve-migration.md` | DbContext, repository, şema ve migration disiplini |

Her domain dosyası şunları içerir: sorumluluk, giriş/çıkışlar, temel kurallar,
ilgili kod konumları, bağımlılıklar ve doğrulama yöntemi.
