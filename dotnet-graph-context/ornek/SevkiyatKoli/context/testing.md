# Dogrulama ve Kabul Kriterleri - SevkiyatKoli

Bu dosya 2026-09-29 tarihinde proje dosyalarindan otomatik tespit edilmistir.
Komutlar uydurulmaz; tespit edilemeyen alan "[tespit edilemedi]" olarak isaretlenir.
Komutlar proje kok dizininde calistirilir.

## Standart kontroller

| Amac | Komut |
|---|---|
| Derleme | `dotnet build "SevkiyatKoli.sln"` |
| Test | `dotnet test "SevkiyatKoli.Tests/SevkiyatKoli.Tests.csproj"` |

## Tespit edilen yapi

- Cozum dosyasi: SevkiyatKoli.sln
- Proje sayisi: 3
- Test projeleri: SevkiyatKoli.Tests/SevkiyatKoli.Tests.csproj
- Hedef framework: net8.0
- Statik analiz: yapilandirilmis lint/analyzer kurali bulunamadi
- CI tanimi: bulunamadi

## Goreve ozel kabul kriterleri

- Her is kurali icin en az bir test bulunur ve testin adi `Kural{N}_` ile baslar.
- Sinir degerler ayrica test edilir (brut tam 30.0 kg kabul, 30.01 kg red).
- Kural degisikligi once `context/kurallar-ve-sinirlar.md`ye yazilir, sonra kodlanir.
- Son dogrulama: **2026-09-29 — `dotnet test SevkiyatKoli.sln` 21/21 yesil (65 ms).**

## Hata geri donus kurali

Bir test basarisiz olursa once hata mesaji ile degisen alanin baglantisini kurun;
ardindan yalnizca ilgili domain, sozlesme veya mimari karar dosyasina donun.
Iki denemede cozulmeyen hatayi `tasks/todo.md` CHECKPOINT bolumune engel olarak yazin.

