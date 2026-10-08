#!/usr/bin/env bash
# Builds an application against the packages in artifacts/packages the way a user would and checks how the
# nUpdate.UpdateInstaller.UI.Avalonia build targets copy the installer:
#   - with nUpdateInstallerRuntimes=<rid> only that installer, executable on Linux and macOS,
#   - without, all seven and warning NUPD001.
# Exports NUPDATE_INSTALLER (the copied installer for <rid>) to GITHUB_ENV for the published-installer tests.
set -euo pipefail

rid="${1:?usage: consumer-smoke-test.sh <runtime identifier>}"
work="$(pwd)/artifacts/consumer"
rm -rf "$work"
mkdir -p "$work"
cd "$work"

cat > consumer.csproj <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup>
  <ItemGroup>
    <PackageReference Include="nUpdate" Version="5.0.0" />
    <PackageReference Include="nUpdate.UpdateInstaller.UI.Avalonia" Version="5.0.0" />
  </ItemGroup>
</Project>
EOF
echo 'System.Console.WriteLine(typeof(nUpdate.Updating.UpdateManager).FullName);' > Program.cs
# Keep the repository's build settings out of the consumer.
printf '<Project>\n</Project>\n' > Directory.Build.props
printf '<Project>\n</Project>\n' > Directory.Packages.props
printf '<Project>\n</Project>\n' > Directory.Build.targets
# Sources go into a nuget.config: Git Bash would turn an https:// argument into a Windows path. A package folder of its
# own, so a 5.0.0 package from an earlier run's cache cannot stand in for the new one; NUGET_PACKAGES would win over it.
cat > nuget.config <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="globalPackagesFolder" value=".nuget-packages" />
  </config>
  <packageSources>
    <clear />
    <add key="local" value="../packages" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
EOF
unset NUGET_PACKAGES

executable=nUpdate.UpdateInstaller.UI.Avalonia
[[ "$rid" == win* ]] && executable+=.exe

dotnet build consumer.csproj -p:nUpdateInstallerRuntimes="$rid" -o one
test -f "one/nUpdate.Installer/$rid/$executable"
test "$(ls one/nUpdate.Installer | wc -l)" -eq 1
if [[ "$rid" != win* ]]; then
  test -x "one/nUpdate.Installer/$rid/$executable"
fi
dotnet one/consumer.dll

dotnet build consumer.csproj -o all | tee all.log
grep -q NUPD001 all.log
test "$(ls all/nUpdate.Installer | wc -l)" -eq 7

# A self-contained single-file publish for the runtime identifier: only that installer, next to the application and
# not bundled into it, executable on Linux and macOS.
dotnet publish consumer.csproj -r "$rid" --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o published
test -f "published/nUpdate.Installer/$rid/$executable"
test "$(ls published/nUpdate.Installer | wc -l)" -eq 1
if [[ "$rid" != win* ]]; then
  test -x "published/nUpdate.Installer/$rid/$executable"
fi

installer="$work/one/nUpdate.Installer/$rid/$executable"
if command -v cygpath > /dev/null; then
  installer="$(cygpath -w "$installer")"
fi
echo "Installer for $rid: $installer"
echo "NUPDATE_INSTALLER=$installer" >> "${GITHUB_ENV:-/dev/null}"
