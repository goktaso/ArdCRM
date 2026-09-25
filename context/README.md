# Proje Bağlam Haritası — ArdCRM

Bu dosya bir bilgi router'ıdır: görevin ihtiyacı olan dosyayı seçer. Proje
bilgilerini tekrarlamak yerine bilginin nerede olduğunu ve ne zaman okunacağını
anlatır. **Varsayılan olarak `context/` dizininin tamamı okunmaz.**

## Her görevde oku

- `proje-ozeti.md`: amaç, kapsam, mimari ve mevcut durum.
- `kurallar-ve-sinirlar.md`: değişmez iş kuralları, güvenlik ve yetki sınırları.

## Göreve göre oku

| Dosya | Ne zaman okunur? |
|---|---|
| `hedefler.md` | Planlama, öncelik veya başarı ölçütü gerekiyorsa |
| `kararlar.md` | Mimari/ürün kararı alınıyor veya eski karar sorgulanıyorsa |
| `testing.md` | Kod değişikliği, kabul kriteri veya hata düzeltmesi varsa |
| `domains/musteri.md` | Müşteri/cari kaydı, müşteri listesi, müşteri detayı, MusteriService/MusteriController |
| `domains/teklif.md` | Teklif CRUD, teklif no üretimi, durum akışı, tutar/para birimi |
| `domains/erp-senkron.md` | Netsis/Logo okuma, ERP adapter, önizleme veya aktarım akışı |
| `domains/veri-ve-migration.md` | Şema değişikliği, EF Core migration, SQL script, DbContext |
| `kaynaklar/README.md` | Kullanıcı sağladığı kaynak veya referans inceleniyorsa |

## Görev sınıflandırma → bağlam seçimi

| Görev tipi | Zorunlu | Ek olarak |
|---|---|---|
| feature | proje-ozeti, kurallar-ve-sinirlar | ilgili `domains/*`, `testing.md`, gerekiyorsa `hedefler.md` |
| bugfix | proje-ozeti, kurallar-ve-sinirlar | hatanın domain dosyası + `testing.md` |
| refactor | proje-ozeti, kurallar-ve-sinirlar | `kararlar.md` + etkilenen `domains/*` |
| review | proje-ozeti, kurallar-ve-sinirlar | `kararlar.md`, `testing.md` |
| research | proje-ozeti | `kararlar.md`, `kaynaklar/README.md` |
| planning | proje-ozeti, kurallar-ve-sinirlar | `hedefler.md`, `kararlar.md`, `tasks/todo.md` |
| documentation | proje-ozeti | değiştirilen alanın domain dosyası |

## Diğer talimat dosyalarıyla ilişki

- `CLAUDE.md` → projenin ana talimat dosyasıdır (Claude Code). Kritik kurallar,
  teknoloji listesi ve geliştirme durumu orada tutulur; bu harita onu tekrar etmez.
- `AGENTS.md` → Codex için aynı router kurallarının karşılığıdır.
- `tasks/todo.md` → güncel CHECKPOINT ve devam eden iş kaydı.
- `tasks/lessons.md` → tekrar edilebilir, doğrulanmış dersler.

## Harita kuralı

- Yeni kalıcı bağlam dosyası eklendiğinde buraya amacı ve okunma koşulu eklenir.
- Geçici notlar, ham kaynaklar ve doğrulanmamış varsayımlar bu haritaya eklenmez.
- Bir dosyanın yalnızca içeriği değişirse haritayı güncellemek gerekmez; amacı
  veya konumu değişirse aynı değişiklikte güncelle.
