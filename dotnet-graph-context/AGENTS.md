# dotnet-graph-context — Codex Talimati

Bu depo .NET projelerine baglam sistemi kuran bir aractir. Kurallar Claude Code
ile ayni; tek kaynak `CLAUDE.md` dosyasidir.

Calismaya baslamadan once `CLAUDE.md` dosyasini okuyun. Ozet:

- Hicbir betik var olan dosyayi ezmez (`Write-GcFile` -> "varsa koru").
- `Install-GraphContextAll.ps1` varsayilan olarak plan modundadir (`-Apply` gerekir).
- Varsayilan kurulum modu `Sidecar`; proje klasoru degismez.
- Build/test komutu uydurulmaz; yalnizca diskteki dosyalardan tespit edilir.
- Sablon metinleri ASCII Turkce yazilir.
- Degisiklik sonrasi: sozdizimi kontrolu + gecici agac uzerinde
  `-WhatIf` / ilk calistirma / ikinci calistirma (idempotency) dogrulamasi.
