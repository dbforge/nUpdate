#!/usr/bin/env bash
# Fails when website/api.html and the public API of the packages differ; see tools/ApiDocsCheck/Program.cs.
# Run it after a Release build of the solution: bash tools/ci/api-docs-check.sh
set -euo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/../.."

dotnet run --project tools/ApiDocsCheck --configuration Release --no-build -- website/api.html \
  nUpdate/bin/Release/netstandard2.0/nUpdate.dll \
  nUpdate.UpdateInstaller/bin/Release/netstandard2.0/nUpdate.UpdateInstaller.dll \
  nUpdate.UI.Avalonia/bin/Release/net8.0/nUpdate.UI.Avalonia.dll \
  nUpdate.UI.WPF/bin/Release/net8.0-windows/nUpdate.UI.WPF.dll \
  nUpdate.UI.WindowsForms/bin/Release/net8.0-windows/nUpdate.UI.WindowsForms.dll \
  nUpdate.Administration.TransferInterface/bin/Release/net10.0/nUpdate.Administration.TransferInterface.dll
