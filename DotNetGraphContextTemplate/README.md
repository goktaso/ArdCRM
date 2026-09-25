# .NET Graph Context Template

Bu klasor, buyuk veya cok katmanli .NET projelerinde yapay zeka ile calisirken
baglami secerek yuklemek icin tekrar kullanilabilir bir baslangic setidir.

Amac tum dokumani her istege vermek degil, gorevi siniflandirip yalnizca gerekli
bilgiyi yuklemektir. Akis su sekildedir:

```text
Gorev -> Router (AGENTS.md) -> context/README.md -> ilgili bilgi dosyalari
      -> uygula -> build/test kapisi -> basarisizsa ilgili alana don
```

## Hizli kurulum

Bir projeye dosya eklemek icin PowerShell'de su komutu calistirin:

```powershell
& ".\DotNetGraphContextTemplate\tools\Initialize-GraphContext.ps1" `
  -ProjectPath "D:\C#\ProjeAdi"
```

Betik var olan dosyalari **ezmez**. `context/` ve `tasks/` dosyalarini ekler.
Projede `AGENTS.md` yoksa router talimatini ana dosya olarak olusturur; varsa
onun yerine incelemeniz icin `AGENTS.graph-context.md` olusturur.

## Ilk 15 dakika

1. `context/proje-ozeti.md` icindeki koseli parantezli alanlari doldurun.
2. `context/kurallar-ve-sinirlar.md` icine degismemesi gereken is kurallarini yazin.
3. Mimari veya urun karari alindiginda `context/kararlar.md`ye ekleyin.
4. Bir domain birden fazla kez baglam gerektiriyorsa `context/domains/` altinda
   bir dosya acin ve `context/README.md` haritasina ekleyin.
5. Gorev sonunda kalici ders varsa `tasks/lessons.md`ye, devam eden is varsa
   `tasks/todo.md`ye kisa kayit ekleyin.
