# Graph Context Router Eki (Claude Code)

Bu dosyadaki kurallari mevcut `CLAUDE.md` dosyanizla birlestirin; mevcut proje
talimatlarini silmeyin veya degistirmeyin. Catisma olursa mevcut `CLAUDE.md`
icindeki proje kurallari baglayicidir.

Entegrasyon icin iki secenek vardir:

1. `CLAUDE.md` icine tek satirlik bir yonlendirme eklemek:
   "Her gorevde once `context/README.md` router'ini oku."
2. `tools/Initialize-GraphContext.ps1 -AddPointer` calistirmak: betik mevcut
   `CLAUDE.md` / `AGENTS.md` dosyasinin **sonuna** isaretli bir router bolumu
   ekler, hicbir mevcut satiri degistirmez, ikinci calistirmada tekrar eklemez.

## Baglam secimi

1. Her gorevde `context/README.md` dosyasini oku.
2. Haritanin **Her gorevde oku** bolumundeki dosyalari oku.
3. Gorevi feature, bugfix, refactor, review, research, planning veya
   documentation olarak siniflandir.
4. Yalnizca gorevle ilgili domain, karar, veri sozlesmesi ve test dosyalarini
   yukle. Tum `context/` dizinini varsayilan olarak okuma.
5. Devam eden is icin `tasks/todo.md` CHECKPOINT'ini oku.

## Is akisi

```text
Siniflandir -> ilgili baglami sec -> incele/planla -> uygula
-> build ve uygun testleri calistir -> sonuc
                                 ^          |
                                 +-- hata --+
```

Bir test veya derleme hatasinda tum projeyi yeniden yorumlama; hatanin iliskili
oldugu domain, karar veya sozlesmeye geri don.

## Cift kayit yasagi

| Konu | Tek dogru yer |
|---|---|
| Proje kurallari ve teknoloji listesi | mevcut `CLAUDE.md` |
| Hangi dosya ne zaman okunur | `context/README.md` |
| Mimari, kapsam, mevcut durum | `context/proje-ozeti.md` |
| Gerekceli teknik kararlar | `context/kararlar.md` |
| Gercek build/test komutlari | `context/testing.md` |
| Guncel WIP / CHECKPOINT | `tasks/todo.md` |
