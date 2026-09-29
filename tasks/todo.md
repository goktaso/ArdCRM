# Görev Durumu — ArdCRM

> Bu dosya projenin tek CHECKPOINT kaydıdır. `CLAUDE.md` içindeki
> "Devam Edilecekler" bölümü geçmiş kayıt olarak korunur; yeni WIP notu buraya yazılır.

## CHECKPOINT

- Hedef: Karar motoru altyapısı (`IKararMotoru`) kuruldu; JEV değerlendirme aşamasında.
- Durum: tamamlandı ve doğrulandı — 2026-09-29, build 0 hata, 50/50 test yeşil.
- Sonraki adım: JEV anahtarı alınırsa `/v1/systemone` şemasını doğrula ve gölge modda
  kural motoruyla karşılaştır (`context/domains/karar-motoru.md`).
- Engeller: yok. (JEV ücretsiz kredisi askıda; karar motoru JEV olmadan çalışıyor.)

## Görevler

- [x] 2026-09-29 — Karar motoru: build + test doğrulaması yapıldı (50/50 yeşil).
- [ ] `context/hedefler.md` → aktif hedef ve başarı ölçütü doldurulacak.
- [ ] JEV: `console.typesafe.ai` üzerinden anahtar alınırsa `/v1/systemone` şeması
      doğrulanacak; gölge modda kural motoruyla karşılaştırılacak (bkz.
      `context/domains/karar-motoru.md` → "JEV adapter'ını açma adımları").
- [ ] `context/proje-ozeti.md` → "Son doğrulama" satırı, ilk gerçek build/test
      çalıştırmasından sonra güncellenecek.
- [ ] Üretim ortamı için `app.UseDeveloperExceptionPage()` ortam ayrımı değerlendirilecek
      (`ArdCRM.Web/Program.cs`, risk kaydı `context/proje-ozeti.md`).

## Tamamlananlar (özet)

Detaylı geliştirme geçmişi `CLAUDE.md` → "Aktif Geliştirme Durumu" bölümündedir.

- [x] 2026-09-25 — `context/` + `tasks/` bağlam sistemi ve router kuralları eklendi.
