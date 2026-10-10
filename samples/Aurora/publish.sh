#!/usr/bin/env bash
# Publishes Aurora as <version> for trying updates locally, with the installer in nUpdate.Installer/<rid>/:
#   - on macOS as artifacts/<version>/Aurora.app, a bundle signed ad hoc, the way you ship a Mac app,
#   - elsewhere as the folder artifacts/<version>/Aurora.
# The runtime identifier defaults to the one of this machine.
set -euo pipefail

usage="usage: publish.sh <version> [runtime identifier]"
version="${1:?$usage}"
here="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if [[ -n "${2:-}" ]]; then
  rid="$2"
else
  case "$(uname -s)-$(uname -m)" in
    Darwin-arm64) rid=osx-arm64 ;;
    Darwin-x86_64) rid=osx-x64 ;;
    Linux-aarch64 | Linux-arm64) rid=linux-arm64 ;;
    Linux-*) rid=linux-x64 ;;
    *) rid=win-x64 ;;
  esac
fi

output="$here/artifacts/$version"
rm -rf "$output"

if [[ "$rid" == osx-* ]]; then
  app="$output/Aurora.app"
  dotnet publish "$here/Aurora.csproj" --configuration Release --runtime "$rid" -p:Version="$version" \
    --output "$app/Contents/MacOS"
  # Info.plist takes only numbers: 1.1.0-beta.1 is stored as 1.1.0.
  bundle_version="${version%%-*}"
  cat > "$app/Contents/Info.plist" <<PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleName</key><string>Aurora</string>
  <key>CFBundleDisplayName</key><string>Aurora</string>
  <key>CFBundleIdentifier</key><string>net.nupdate.samples.aurora</string>
  <key>CFBundleExecutable</key><string>Aurora</string>
  <key>CFBundlePackageType</key><string>APPL</string>
  <key>CFBundleShortVersionString</key><string>$bundle_version</string>
  <key>CFBundleVersion</key><string>$bundle_version</string>
  <key>LSMinimumSystemVersion</key><string>12.0</string>
  <key>NSHighResolutionCapable</key><true/>
  <key>NSPrincipalClass</key><string>NSApplication</string>
</dict>
</plist>
PLIST
  plutil -lint "$app/Contents/Info.plist" > /dev/null
  codesign --force --deep --sign - "$app"
  codesign --verify --deep --strict "$app"
  echo "Aurora $version for $rid: $app"
else
  dotnet publish "$here/Aurora.csproj" --configuration Release --runtime "$rid" -p:Version="$version" \
    --output "$output/Aurora"
  echo "Aurora $version for $rid: $output/Aurora"
fi
