# Graph Engineering — Yontem

## Problem

Buyuk bir .NET cozumunde her yapay zeka oturumuna tum dokumani vermek uc sorun uretir:

1. **Baglam kirliligi** — ilgisiz kural ve gecmis, modelin dikkatini boler.
2. **Tekrar** — ayni bilgi CLAUDE.md, README, kod yorumu ve sohbet gecmisinde
   farkli surumlerde yasar; hangisinin guncel oldugu belirsizlesir.
3. **Dogrulanmamis is** — "yaptim" diyen ama build/test kapisindan gecmemis cikti.

## Cozum: bilgiyi grafik olarak kur, gorevle yuru

Dokumani duz bir yigin degil, **dugumler ve okuma kosullari** olan bir grafik
olarak kur. Her gorev bu grafikte yalnizca kendi yolunu yurur.

```text
                 +-- kararlar.md
                 |
Gorev --> Router +-- domains/<alan>.md --> Uygula --> Build/Test kapisi --> Sonuc
(sinif)          |                                         |
                 +-- testing.md                            +-- hata --> ilgili dugume don
```

### Dugumler

| Dugum | Sorumluluk | Okuma kosulu |
|---|---|---|
| `context/README.md` | Router. Hangi dosya ne zaman okunur. | **Her gorevde** |
| `context/proje-ozeti.md` | Amac, mimari, kapsam, mevcut durum | **Her gorevde** |
| `context/kurallar-ve-sinirlar.md` | Degismez kurallar, guvenlik/yetki sinirlari | **Her gorevde** |
| `context/kararlar.md` | Gerekceli, tarihli teknik kararlar | Karar aliniyor/sorgulaniyorsa |
| `context/testing.md` | Gercek build/test komutlari, kabul kriterleri | Kod degisikligi varsa |
| `context/domains/*.md` | Is alani detayi: sorumluluk, kurallar, kod konumlari | Gorev o alani etkiliyorsa |
| `context/hedefler.md` | Oncelik ve basari olcutu | Planlama yapiliyorsa |
| `context/kaynaklar/` | Kullanici sagladigi dis kaynak envanteri | Kaynak inceleniyorsa |
| `tasks/todo.md` | Tek CHECKPOINT: hedef, durum, sonraki adim, engel | Oturum basi/sonu |
| `tasks/lessons.md` | Tekrar edilebilir, kanitli dersler | Ders cikarildiginda |

### Kenarlar (okuma kosullari)

Router'daki tablo grafigin kenarlaridir. Gorev tipi kenari secer:

| Gorev tipi | Zorunlu | Ek |
|---|---|---|
| feature | proje-ozeti, kurallar | ilgili domain, testing, (hedefler) |
| bugfix | proje-ozeti, kurallar | hatanin domaini, testing |
| refactor | proje-ozeti, kurallar | kararlar, etkilenen domainler |
| review | proje-ozeti, kurallar | kararlar, testing |
| research | proje-ozeti | kararlar, kaynaklar |
| planning | proje-ozeti, kurallar | hedefler, kararlar, tasks/todo |
| documentation | proje-ozeti | degisen alanin domaini |

## Akisin kurallari

1. **Once siniflandir.** Gorev tipi belirlenmeden dosya okunmaz.
2. **Yalnizca gerekli dugumu yukle.** `context/` dizininin tamami varsayilan
   olarak okunmaz; okumak istisnadir ve gerekcesi soylenir.
3. **Build/test kapisi zorunludur.** Kod degisirse `context/testing.md` icindeki
   gercek komut calistirilir. Calistirilamayan kontrol "dogrulandi" diye raporlanmaz.
4. **Hatada dar donus.** Derleme/test hatasinda tum proje yeniden yorumlanmaz;
   yalnizca hatanin baglandigi domain, karar veya sozlesme dugumune donulur.
5. **Kalici degisiklik ayni gorevde yazilir.** Kural, hedef veya karar degistiyse
   ilgili `context/` kaydi o gorev icinde guncellenir; "sonra yazarim" yok.
6. **Cift kayit yasak.** Bir bilginin tek dogru yeri vardir; router bu yeri gosterir,
   icerigi tekrarlamaz.

## Neden CHECKPOINT ayri dosyada

`tasks/todo.md` tek bir CHECKPOINT tutar: hedef, durum, sonraki somut adim, engel.
Oturum degistiginde (is/ev makinesi, Codex/Claude Code) okunacak ilk yer burasidir.
Proje talimat dosyasi (CLAUDE.md) kural icindir; durum icin degil. Bu ayrim,
talimat dosyasinin oturum notlariyla sismesini engeller.

## Codex ve Claude Code uyumu

Iki aracin da kendi talimat dosyasi vardir:

- Codex → `AGENTS.md`
- Claude Code → `CLAUDE.md`

Kural metni iki dosyada cogaltilmaz. Ikisi de ayni router'a (`context/README.md`)
yonlendirir. Proje zaten bir talimat dosyasina sahipse o dosya **degistirilmez**;
router baglantisi ya ayri bir ek dosyayla (`*.graph-context.md`) ya da dosyanin
sonuna eklenen isaretli bir bolumle (`-AddPointer`) kurulur.

## Basari olcutu

Sistem su durumda ise yariyor demektir:

- Yeni bir oturum, projeyi bastan anlatmadan dogru dosyalari kendisi seciyor.
- "Neden boyle yapilmis?" sorusunun cevabi `kararlar.md`de tarihli duruyor.
- Bir hata sonrasi donus, tum projeyi degil tek bir dugumu etkiliyor.
- Build/test komutu her projede tek ve dogru yerde yaziyor.
