# Proje baglam router'i

Bu dosya Codex icin proje bazli calisma talimatidir. Claude Code kullaniliyorsa
ayni kurallari `CLAUDE.md` icinde veya ona yonlendiren bir katmanda tutun.

## Her gorevde

1. Once `context/README.md` dosyasini oku.
2. Ardindan haritada **Her gorevde oku** olarak belirtilen dosyalari oku.
3. Gorevi feature, bugfix, refactor, review, research, planning veya
   documentation olarak siniflandir.
4. Yalnizca gorevle ilgili domain, karar, veri sozlesmesi ve test dosyalarini
   yukle. Tum `context/` dizinini varsayilan olarak okuma.

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

## Guvenlik ve kalite

- Gizli bilgi, baglanti dizesi, gercek musteri verisi veya erisim anahtari
  baglam dosyalarina yazilmaz.
- Mevcut dosyalari koru; gorev disi dosyalari degistirme.
- Kod degisikliginde proje tarafindan belirtilen build ve test komutlarini
  calistir. Kontrol edilemeyen sonucu dogrulanmis gibi bildirme.
