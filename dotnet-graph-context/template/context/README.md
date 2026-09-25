# Proje Baglam Haritasi

Bu dosya bir bilgi router'idir: gorevin ihtiyaci olan dosyayi secer. Proje
bilgilerini tekrarlamak yerine bilginin nerede oldugunu ve ne zaman okunacagini
anlatir. **Varsayilan olarak `context/` dizininin tamami okunmaz.**

## Her gorevde oku

- `proje-ozeti.md`: amac, kapsam, mimari ve mevcut durum.
- `kurallar-ve-sinirlar.md`: degismez is kurallari, guvenlik ve yetki sinirlari.

## Goreve gore oku

| Dosya | Ne zaman okunur? |
|---|---|
| `hedefler.md` | Planlama, oncelik veya basari olcutu gerekiyorsa |
| `kararlar.md` | Mimari/urun karari aliniyor veya eski karar sorgulaniyorsa |
| `testing.md` | Kod degisikligi, kabul kriteri veya hata duzeltmesi varsa |
| `domains/<alan>.md` | Gorev ilgili is alanini etkiliyorsa |
| `kaynaklar/README.md` | Kullanici sagladigi kaynak veya referans inceleniyorsa |

## Gorev siniflandirma -> baglam secimi

| Gorev tipi | Zorunlu | Ek olarak |
|---|---|---|
| feature | proje-ozeti, kurallar-ve-sinirlar | ilgili `domains/*`, `testing.md`, gerekiyorsa `hedefler.md` |
| bugfix | proje-ozeti, kurallar-ve-sinirlar | hatanin domain dosyasi + `testing.md` |
| refactor | proje-ozeti, kurallar-ve-sinirlar | `kararlar.md` + etkilenen `domains/*` |
| review | proje-ozeti, kurallar-ve-sinirlar | `kararlar.md`, `testing.md` |
| research | proje-ozeti | `kararlar.md`, `kaynaklar/README.md` |
| planning | proje-ozeti, kurallar-ve-sinirlar | `hedefler.md`, `kararlar.md`, `../tasks/todo.md` |
| documentation | proje-ozeti | degistirilen alanin domain dosyasi |

## Domain haritasi

> Domain dosyasi ekledikce bu tabloyu guncelleyin. Bos birakmayin; bos tablo
> router'in ise yaramadigi anlamina gelir.

| Dosya | Ne zaman okunur? |
|---|---|
| `domains/[alan].md` | [hangi gorevlerde] |

## Harita kurali

- Yeni kalici baglam dosyasi eklendiginde buraya amaci ve okunma kosulu eklenir.
- Gecici notlar, ham kaynaklar ve dogrulanmamis varsayimlar bu haritaya eklenmez.
- Bir dosyanin yalnizca icerigi degisirse haritayi guncellemek gerekmez; amaci
  veya konumu degisirse ayni degisiklikte guncelle.
