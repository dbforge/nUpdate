#!/usr/bin/env bash
# Builds nUpdate Administration for Linux:
#   - publishes nUpdate.Administration self-contained for <rid> (linux-x64 or linux-arm64), so users need no .NET,
#   - packs the folder nUpdate-Administration into <output folder>/nUpdate-Administration-<rid>.tar.gz. A tarball keeps
#     the executable bit, which the zip of a GitHub Actions artifact would drop.
set -euo pipefail

usage="usage: administration-linux.sh <linux-x64|linux-arm64> <output folder>"
rid="${1:?$usage}"
output="${2:?$usage}"
[[ "$rid" == linux-* ]] || { echo "$usage" >&2; exit 1; }
root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
project="$root/nUpdate.Administration/nUpdate.Administration.csproj"
folder="$output/nUpdate-Administration"
tarball="$output/nUpdate-Administration-$rid.tar.gz"

rm -rf "$folder" "$tarball"
dotnet publish "$project" --configuration Release --runtime "$rid" --self-contained --output "$folder"
test -x "$folder/nUpdate.Administration"
tar -czf "$tarball" -C "$output" nUpdate-Administration
rm -rf "$folder"
echo "nUpdate Administration for $rid: $tarball"
