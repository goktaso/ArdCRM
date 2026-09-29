#!/bin/bash
# ArdCRM — Claude Code on the web oturum baslangici
# .NET 8 SDK'yi kurar ve NuGet paketlerini geri yukler; boylece oturum acilir
# acilmaz "dotnet build" / "dotnet test" calisabilir.
#
# Yalnizca uzak (bulut) oturumlarda calisir; yerel makinede hicbir sey yapmaz.
# Idempotenttir: SDK zaten varsa kurulumu atlar.
set -euo pipefail

if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

echo 'export DOTNET_CLI_TELEMETRY_OPTOUT=1' >> "${CLAUDE_ENV_FILE:-/dev/null}"
echo 'export DOTNET_NOLOGO=1'               >> "${CLAUDE_ENV_FILE:-/dev/null}"

if ! command -v dotnet > /dev/null 2>&1; then
  echo "[session-start] .NET 8 SDK kuruluyor (apt)..."
  export DEBIAN_FRONTEND=noninteractive
  apt-get update -qq
  apt-get install -y -qq dotnet-sdk-8.0
else
  echo "[session-start] .NET SDK zaten kurulu: $(dotnet --version)"
fi

cd "${CLAUDE_PROJECT_DIR:-.}"

if [ -f ArdCRM.sln ]; then
  echo "[session-start] NuGet paketleri geri yukleniyor..."
  dotnet restore ArdCRM.sln
fi

echo "[session-start] Hazir. Dogrulama komutlari: context/testing.md"
