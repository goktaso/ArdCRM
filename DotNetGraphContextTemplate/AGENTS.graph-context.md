# Graph Context Router Eki

Bu dosyadaki kurallari mevcut `AGENTS.md` dosyanizla birlestirin; mevcut proje
talimatlarini silmeyin veya degistirmeyin.

## Baglam secimi

1. Her gorevde `context/README.md` dosyasini oku.
2. Haritanin **Her gorevde oku** bolumundeki dosyalari oku.
3. Gorevi feature, bugfix, refactor, review, research, planning veya
   documentation olarak siniflandir.
4. Yalnizca gorevle ilgili domain, karar, veri sozlesmesi ve test dosyalarini
   yukle. Tum `context/` dizinini varsayilan olarak okuma.

## Is akisi

```text
Siniflandir -> ilgili baglami sec -> incele/planla -> uygula
-> build ve uygun testleri calistir -> sonuc
                                 ^          |
                                 +-- hata --+
```

Bir test veya derleme hatasinda tum projeyi yeniden yorumlama; hatanin iliskili
oldugu domain, karar veya sozlesmeye geri don. Kalici bir kural, hedef veya
teknik karar degisirse ayni gorevde ilgili `context/` kaydini guncelle.
