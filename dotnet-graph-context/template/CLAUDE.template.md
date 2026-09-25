# Proje baglam router'i (Claude Code)

Bu dosya Claude Code icin proje bazli calisma talimatidir. Codex ayni kurallari
kok dizindeki `AGENTS.md` dosyasindan okur. Kural metnini iki dosyada cogaltmayin;
ikisi de `context/README.md` router'ina yonlendirir.

## Her gorevde

1. Once `context/README.md` dosyasini oku (baglam haritasi).
2. Haritada **Her gorevde oku** olarak isaretli dosyalari oku:
   `context/proje-ozeti.md` ve `context/kurallar-ve-sinirlar.md`.
3. Gorevi siniflandir: feature, bugfix, refactor, review, research, planning,
   documentation.
4. Yalnizca gorevle ilgili domain, karar, sozlesme ve test dosyalarini yukle.
   `context/` dizininin tamamini varsayilan olarak okuma.
5. Devam eden is varsa `tasks/todo.md` icindeki CHECKPOINT'i oku.

## Is grafigi

```text
Siniflandir -> ilgili baglami sec -> incele/planla -> uygula
-> build ve uygun testleri calistir -> sonuc
                                 ^          |
                                 +-- hata --+
```

- Birden fazla bagimsiz alan etkileniyorsa onlari ayri alt isler olarak ele al.
- Test veya derleme hatasinda tum projeyi yeniden yorumlama; hatanin iliskili
  oldugu domain, karar veya sozlesmeye geri don.
- Kalici bir kural, hedef veya teknik karar degisirse ayni gorevde ilgili
  `context/` kaydini guncelle.

## Dogrulama kapisi

Kod degisikliginde `context/testing.md` icindeki gercek komutlari calistir.
Calistirilamayan bir kontrolu dogrulanmis gibi raporlama; neyin calistirilmadigini
acikca belirt.

## Guvenlik ve kalite

- Gizli bilgi, baglanti dizesi, gercek musteri verisi veya erisim anahtari baglam
  dosyalarina, koda veya commit'e yazilmaz.
- Mevcut dosyalari koru; gorev disi dosyalari degistirme.
- Geri alinamaz islemler (sema degisikligi, yayinlama, veri silme) kullanici onayi ister.
