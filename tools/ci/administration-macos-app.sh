#!/usr/bin/env bash
# Builds the macOS application bundle of nUpdate Administration; runs on macOS only (plutil, codesign, ditto):
#   - publishes nUpdate.Administration self-contained for <rid> (osx-arm64 or osx-x64), so users need no .NET,
#   - assembles "<output folder>/nUpdate Administration.app": the published files in Contents/MacOS, the icon in
#     Contents/Resources and an Info.plist with the version from Directory.Build.props,
#   - signs the bundle ad hoc, verifies it and zips it to <output folder>/nUpdate-Administration-<rid>.zip.
# The bundle is not notarized, so macOS asks before it opens a downloaded copy.
set -euo pipefail

usage="usage: administration-macos-app.sh <osx-arm64|osx-x64> <output folder>"
rid="${1:?$usage}"
output="${2:?$usage}"
[[ "$rid" == osx-* ]] || { echo "$usage" >&2; exit 1; }
root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
project="$root/nUpdate.Administration/nUpdate.Administration.csproj"
app="$output/nUpdate Administration.app"
zip="$output/nUpdate-Administration-$rid.zip"
version="$(dotnet msbuild "$project" -getProperty:Version)"
# Info.plist takes only numbers: 5.0.0-beta.1 is stored as 5.0.0.
bundle_version="${version%%-*}"

rm -rf "$app" "$zip"
dotnet publish "$project" --configuration Release --runtime "$rid" --self-contained --output "$app/Contents/MacOS"
test -x "$app/Contents/MacOS/nUpdate.Administration"
mkdir -p "$app/Contents/Resources"
cp "$root/nUpdate.Administration/Assets/nUpdate.icns" "$app/Contents/Resources/"

# LSMinimumSystemVersion is the deployment target of .NET 10's native binaries (12.0); Avalonia's need less.
cat > "$app/Contents/Info.plist" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleName</key><string>nUpdate Administration</string>
  <key>CFBundleDisplayName</key><string>nUpdate Administration</string>
  <key>CFBundleIdentifier</key><string>net.nupdate.administration</string>
  <key>CFBundleExecutable</key><string>nUpdate.Administration</string>
  <key>CFBundleIconFile</key><string>nUpdate</string>
  <key>CFBundlePackageType</key><string>APPL</string>
  <key>CFBundleShortVersionString</key><string>$bundle_version</string>
  <key>CFBundleVersion</key><string>$bundle_version</string>
  <key>LSMinimumSystemVersion</key><string>12.0</string>
  <key>NSHighResolutionCapable</key><true/>
  <key>NSPrincipalClass</key><string>NSApplication</string>
</dict>
</plist>
EOF
plutil -lint "$app/Contents/Info.plist"

codesign --force --deep --sign - "$app"
codesign --verify --deep --strict --verbose=2 "$app"
ditto -c -k --keepParent "$app" "$zip"

# Check the bundle as users get it: codesign keeps the signatures of the files in Contents/MacOS that are not Mach-O
# in extended attributes, which the zip must carry.
rm -rf "$output/unpacked"
ditto -x -k "$zip" "$output/unpacked"
codesign --verify --deep --strict "$output/unpacked/nUpdate Administration.app"
rm -rf "$output/unpacked"
echo "nUpdate Administration for $rid: $zip"
