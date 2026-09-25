# ArdCRM — Ajan Çalışma Talimatı (Codex)

Bu dosya Codex için proje bazlı çalışma talimatıdır. Claude Code aynı kuralları
`CLAUDE.md` → "Graph Context Router" bölümünden okur. Proje kuralları, teknoloji
listesi ve geliştirme geçmişi `CLAUDE.md` dosyasındadır; burada tekrarlanmaz.

## Her görevde

1. Önce `context/README.md` dosyasını oku (bağlam haritası / router).
2. Haritada **Her görevde oku** olarak işaretli dosyaları oku:
   `context/proje-ozeti.md` ve `context/kurallar-ve-sinirlar.md`.
3. Görevi sınıflandır: feature, bugfix, refactor, review, research, planning,
   documentation.
4. Yalnızca görevle ilgili domain, karar, sözleşme ve test dosyalarını yükle.
   `context/` dizininin tamamını varsayılan olarak okuma.
5. Devam eden iş varsa `tasks/todo.md` içindeki CHECKPOINT'i oku.

## İş grafiği

```text
Sınıflandır -> ilgili bağlamı seç -> incele/planla -> uygula
-> build ve uygun testleri çalıştır -> sonuç
                                 ^          |
                                 +-- hata --+
```

- Birden fazla bağımsız alan etkileniyorsa onları ayrı alt işler olarak ele al.
- Test veya derleme hatasında tüm projeyi yeniden yorumlama; hatanın ilişkili
  olduğu domain, karar veya sözleşme dosyasına geri dön (`context/testing.md` →
  "Hata geri dönüş kuralı").
- Kalıcı bir kural, hedef veya teknik karar değişirse aynı görevde ilgili
  `context/` kaydını güncelle.

## Doğrulama kapısı

Kod değişikliğinde `context/testing.md` içindeki gerçek komutları çalıştır:

```bash
dotnet build ArdCRM.sln
dotnet test ArdCRM.Tests/ArdCRM.Tests.csproj
```

Çalıştırılamayan bir kontrolü doğrulanmış gibi raporlama; neyin
çalıştırılmadığını açıkça belirt.

## Güvenlik ve kalite

- Gizli bilgi, bağlantı dizesi, gerçek müşteri verisi veya erişim anahtarı bağlam
  dosyalarına, koda veya commit'e yazılmaz.
- ERP (Netsis/Logo) **salt okunurdur**; yazma yapılmaz.
- Mevcut dosyaları koru; görev dışı dosyaları değiştirme.
- Şema değişikliği, ERP sorgu değişikliği ve geri alınamaz işlemler kullanıcı
  onayı ister (`context/kurallar-ve-sinirlar.md`).
