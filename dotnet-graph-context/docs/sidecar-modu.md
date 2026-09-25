# Sidecar Modu — Projeye Hic Dokunmadan Baglam

## Ne ise yarar

Olgun, calisir durumda ve gecmisi onemli bir projeye (orn. PackErp) yeni bir
dosya yapisi sokmak risklidir: depo kirlenir, gozden gecirme gurultulenir,
ekip disindaki bir arac beklenmedik dosya gorur.

Sidecar modunda baglam kayitlari projenin **disinda** tutulur:

```text
D:\C#\
├─ PackErp\                     <- HIC DEGISMEZ
│  ├─ PackErp.sln
│  └─ ...
└─ _graph-context\
   └─ PackErp\
      ├─ AGENTS.md              <- Codex router'i
      ├─ CLAUDE.md              <- Claude Code router'i
      ├─ baglanti.md            <- proje yolu + nasil baslatilir
      ├─ baslat-claude.cmd      <- (-WithLauncher)
      ├─ baslat-codex.cmd       <- (-WithLauncher)
      ├─ context\
      └─ tasks\
```

## Kurulum

```powershell
.\tools\Initialize-GraphContext.ps1 -ProjectPath "D:\C#\PackErp" -WithLauncher
```

Varsayilan sidecar kok klasoru projenin ust klasorundeki `_graph-context`'tir;
`-SidecarRoot` ile degistirilebilir.

## Kullanim

### Claude Code

```powershell
cd "D:\C#\PackErp"
claude --add-dir "D:\C#\_graph-context\PackErp"
```

Ilk mesaj:

```text
Once D:\C#\_graph-context\PackErp\context\README.md router'ini oku,
gorevi siniflandir, yalnizca ilgili baglam dosyalarini yukle. Gorev: ...
```

`baslat-claude.cmd` bu iki adimi tek tikla yapar.

### Codex

Codex `AGENTS.md` dosyasini calisma dizininde arar. Sidecar modda proje koku
degismedigi icin router yolunu ilk mesajda verin:

```text
Once D:\C#\_graph-context\PackErp\AGENTS.md ve
D:\C#\_graph-context\PackErp\context\README.md dosyalarini oku. Gorev: ...
```

## Ara secenek: izlenmeyen isaret dosyasi

Her seferinde yol yazmak istemiyorsaniz:

```powershell
.\tools\Initialize-GraphContext.ps1 -ProjectPath "D:\C#\PackErp" -LinkPointer
```

Bu, proje kokune tek bir `.graph-context.md` dosyasi birakir **ve** onu
`.git/info/exclude` icine ekler. `.git/info/exclude` yerel bir dosyadir:
commit edilmez, push edilmez, ekibin deposunda gorunmez. Yani depo icerigi
yine degismez; yalnizca sizin makinenizde bir isaret kalir.

Geri almak icin dosyayi silmek yeterlidir:

```powershell
Remove-Item "D:\C#\PackErp\.graph-context.md"
```

## Iceri tasima

Sistem ise yaradiginda, baglami projenin icine alip ekiple paylasabilirsiniz:

```powershell
Copy-Item "D:\C#\_graph-context\PackErp\context" "D:\C#\PackErp\context" -Recurse
Copy-Item "D:\C#\_graph-context\PackErp\tasks"   "D:\C#\PackErp\tasks"   -Recurse
.\tools\Initialize-GraphContext.ps1 -ProjectPath "D:\C#\PackErp" -Mode InPlace -AddPointer
```

Son komut var olan `AGENTS.md` / `CLAUDE.md` dosyalarinin **sonuna** router
bolumu ekler; mevcut satirlarin hicbirine dokunmaz ve ikinci kez calistirildiginda
tekrar eklemez.

## Sinirlar

- Sidecar klasoru proje deposuyla birlikte versiyonlanmaz. Ekiple paylasmak
  istiyorsaniz ya iceri tasiyin ya da sidecar kokunu ayri bir depoya alin.
- Iki makinede calisiyorsaniz (is/ev) sidecar klasorunu de senkronlamaniz gerekir;
  aksi halde `tasks/todo.md` CHECKPOINT'i iki makinede ayrisir.
- Baglam dosyalari projeyle birlikte tasinmaz; proje klasorunu tasirsaniz
  `baglanti.md` icindeki yolu guncelleyin.
