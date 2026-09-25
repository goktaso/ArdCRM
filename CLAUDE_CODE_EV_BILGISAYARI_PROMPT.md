# Ev Bilgisayarinda Graph Engineering Kurulum Promptu

Ev bilgisayarinda Claude Code'u `ArdCRM` depo kokunde acin ve asagidaki istemi
verin. Bu depo icindeki `DotNetGraphContextTemplate` klasoru referansinizdir.

```text
Bu bilgisayarda .NET projelerim icin Graph Engineering tabanli, Codex ve Claude
Code ile uyumlu bir proje-baglam sistemi kur.

Referans sablon: Bu deponun kokundeki DotNetGraphContextTemplate.

Hedef:
- Her projede gorev once siniflandirilsin.
- Tum dokumanlar korlemesine okunmasin; context/README.md yalniz gerekli baglam
  dosyalarini secen router olsun.
- Akis su sekilde calissin:
  gorev -> siniflandirma -> ilgili context secimi -> inceleme/plan -> uygulama
  -> build/test dogrulamasi -> hata varsa yalniz ilgili domain veya karar dosyasina donus.
- Codex icin AGENTS.md, Claude Code icin CLAUDE.md uyumlu olsun.

Yapilacaklar:
1. D:\C# altindaki .NET cozumu olan projeleri listele.
2. Kucuk, tek katmanli veya egitim projelerini degistirme.
3. Oncelikle PackErp, ArdCRM, ProductionPlanning ve EtiketTasarim projelerini degerlendir.
4. Her uygun proje icin DotNetGraphContextTemplate/tools/Initialize-GraphContext.ps1
   betigini kullan veya ayni guvenli davranisi uygula.
5. Var olan context/, tasks/, AGENTS.md ve CLAUDE.md dosyalarini asla ezme.
   Var olan talimat dosyasi varsa gerekli router kurallarini cakisma yaratmadan
   ek dosya veya acik bir yonlendirme ile entegre et.
6. context/proje-ozeti.md, kurallar-ve-sinirlar.md ve kararlar.md varsa koru;
   yalniz eksik dosyalari olustur.
7. Her projedeki gercek build ve test komutlarini tespit edip context/testing.md
   dosyasina yaz. Komut uydurma.
8. Degisiklik yapmadan once her proje icin kisa plan goster; sonra uygula.
9. En sonda proje bazinda eklenen dosyalari, korunmus dosyalari, talimat dosyasi
   entegrasyonunu, build/test komutlarini ve elle doldurulacak alanlari raporla.

Gizli bilgi, connection string, gercek musteri verisi veya erisim anahtarini
hicbir context dosyasina kopyalama.
```
