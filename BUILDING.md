# Building nUpdate

## Prerequisites

- .NET SDK 10.0 (see `global.json`; `rollForward` accepts any 10.0.x feature band). If you installed it with
  `dotnet-install.sh` into `~/.dotnet`, export `DOTNET_ROOT=$HOME/.dotnet` and put it on your `PATH`.
- Docker for the integration tests, including the UI scenarios. They start pure-ftpd, atmoz/sftp, a php:apache image built from
  `nUpdate.Tests/Integration/Docker/php.Dockerfile` and mysql:8.4 and are skipped when Docker is not reachable. CI sets
  `NUPDATE_REQUIRE_DOCKER=1`, so there a Docker that does not answer within a minute fails them instead.
- Windows is only needed to *run* the Windows Forms/WPF user interfaces and the Windows-only adapter tests. Everything,
  including the net462 and net8.0-windows projects, *builds* on Linux and macOS (`EnableWindowsTargeting`), and the
  installer for all seven runtime identifiers is published from any of them.

## Everyday commands

```bash
dotnet restore nUpdate.sln && dotnet tool restore
dotnet build nUpdate.sln
dotnet test nUpdate.sln                     # everything; the Docker integration tests skip themselves without Docker
dotnet test nUpdate.Tests/nUpdate.Tests.csproj -- --filter-not-trait "Category=Integration"   # unit tests only
dotnet format nUpdate.sln                   # fix style; CI runs --verify-no-changes
dotnet run --project nUpdate.Administration # start the administration app
```

While a change breaks the compilation of a downstream project, the test project can be built with only a part of the
tree: `-p:LibraryOnly=true` (the `nUpdate` library and `Tests/Library`), `-p:InstallerOnly=true` (plus the installer
base library and `Tests/Installer`), `-p:AdminCoreOnly=true` (everything but the Administration app and the Docker tests) or
`-p:SkipIntegration=true` (everything but the Docker tests). Pass the same switch to `dotnet build`, `dotnet test` and
`dotnet format` of `nUpdate.Tests/nUpdate.Tests.csproj`. The `--filter-namespace` option of the test runner matches a
namespace exactly, so `nUpdate.Tests.Integration.Scenarios` runs the scenarios and `nUpdate.Tests.Integration` the
end-to-end and transfer tests.

Do not pass `-nologo` to `dotnet test`: with the Microsoft.Testing.Platform runner the flag is forwarded to the test
host, which then runs zero tests.

## Coverage

The test project writes a Cobertura report when asked to:

```bash
dotnet test nUpdate.Tests/nUpdate.Tests.csproj --no-build --results-directory TestResults/nUpdate.Tests -- \
  --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml
```

Then check the thresholds per assembly:

```bash
dotnet run --project tools/CoverageGate -- TestResults/*/coverage.cobertura.xml \
  --require nUpdate=100,100 --require nUpdate.UpdateInstaller=100,100 \
  --require nUpdate.Administration.Core=100,100 --require nUpdate.Administration.TransferInterface=100,100 \
  --require nUpdate.Administration=90,90 --require nUpdate.UpdateInstaller.UI.Avalonia=90,90 \
  --require nUpdate.UI.Avalonia=90,90 --show-uncovered
```

`--show-uncovered` lists the lines and branches that are missing. An HTML report:

```bash
dotnet reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:TestResults/report
```

Code that cannot run in a Linux unit test is excluded from coverage, each exclusion with a comment saying why: the
Windows adapters of the installer (registry, services, event log; exercised by the Windows-only tests), the real
process launchers, the macOS bundle swap system call and the start of the Avalonia platform (both exercised by the
published-installer tests), the FTP/SFTP providers (covered by the integration tests instead), composition roots and
the Windows Forms/WPF shells.

## The published installer

`nUpdate.UpdateInstaller.UI.Avalonia` is published self-contained, single-file and trimmed per runtime identifier:

```bash
dotnet publish nUpdate.UpdateInstaller.UI.Avalonia/nUpdate.UpdateInstaller.UI.Avalonia.csproj -c Release -r linux-x64 -o /tmp/installer
```

`PublishedInstallerTests` (trait `Category=PublishedInstaller`) builds real packages with the Administration's builder
and runs such an installer windowless: files, Unix modes, operations, a failed update, the log, and on macOS the swap of
an ad-hoc signed bundle that must still verify and start. They are skipped unless `NUPDATE_INSTALLER` names the
executable:

```bash
NUPDATE_INSTALLER=/tmp/installer/nUpdate.UpdateInstaller.UI.Avalonia \
  dotnet test nUpdate.Tests/nUpdate.Tests.csproj -- --filter-trait "Category=PublishedInstaller"
```

CI merges all reports with ReportGenerator (`-reporttypes:Cobertura`) and gates on the merged file; when the gate is
given several reports it adds line hits but takes the best branch value per line, which can under-count a branch that
two test projects cover half each.

## Packages

```bash
dotnet pack nUpdate.sln --configuration Release --output artifacts/packages
```

produces `nUpdate`, `nUpdate.UpdateInstaller` (the base library for installers of your own),
`nUpdate.UpdateInstaller.UI.Avalonia` (the built-in installer for all seven runtime identifiers plus the MSBuild targets
that copy one or more of them to `nUpdate.Installer/<rid>/` in the consuming application's output),
`nUpdate.UI.WindowsForms`, `nUpdate.UI.WPF`, `nUpdate.UI.Avalonia` and `nUpdate.Administration.TransferInterface`.
Packing restores the runtime packs of all seven runtime identifiers and publishes each, which takes a few minutes the
first time. `tools/ci/consumer-smoke-test.sh <rid>` builds an application against the packages and checks what the
targets copy.

## Screenshots

`WindowScreenshots` renders every window of nUpdate Administration, the installer window and the Avalonia update
dialog with sample data for an application called Aurora, and the main windows of nUpdate Administration once more in
dark mode (`*-dark.png`). Headless rendering has no window decorations, so it adds
them: a Windows 11 frame for nUpdate Administration, a GNOME frame for the installer and the update dialog. Set
`NUPDATE_SCREENSHOTS` to a folder to get the PNGs, then copy the ones the README shows into `docs/images`
(`project-packages.png`, `package-operations.png`, `project-statistics.png` and `migration-packages.png`, renamed to
`administration-*.png`; `installer.png`, `installer-locked-file.png`, and `update-dialog.png` as
`avalonia-update-dialog.png`):

```
NUPDATE_SCREENSHOTS=/tmp/screenshots dotnet test nUpdate.Tests/nUpdate.Tests.csproj -- --filter-class nUpdate.Tests.Administration.App.WindowScreenshots
```

The update dialog of nUpdate.UI.WindowsForms can only be rendered on Windows. `tools/WinFormsScreenshots` shows the real
dialog for Aurora, captures it from the screen and adds the same Windows 11 frame. The Windows job of CI runs it and
uploads `winforms-update-dialog.png` as the artifact `screenshots-windows`; copy it into `docs/images`. On a Windows
machine, `dotnet run --project tools/WinFormsScreenshots -- <folder>` does the same.

## The macOS bundle of nUpdate Administration

`tools/ci/administration-macos-app.sh <rid> <folder>` runs on a Mac. It publishes nUpdate Administration
self-contained for `osx-arm64` or `osx-x64` and builds `<folder>/nUpdate Administration.app`: the published files in
`Contents/MacOS`, `nUpdate.Administration/Assets/nUpdate.icns` as the icon and an `Info.plist` with the version from
`Directory.Build.props`. It signs the bundle ad hoc, verifies the signature and zips the bundle to
`<folder>/nUpdate-Administration-<rid>.zip`. The macOS job of CI uploads that zip for osx-arm64 as the artifact
`administration-osx-arm64`.

```bash
bash tools/ci/administration-macos-app.sh osx-arm64 artifacts/administration-osx-arm64
```

Unpack the zip with Finder or `ditto -x -k`: the signatures of the files in `Contents/MacOS` that are not Mach-O live in
extended attributes, which `unzip` drops. The bundle is not notarized, so macOS blocks a downloaded copy until you
allow it under System Settings > Privacy & Security or remove the quarantine with
`xattr -dr com.apple.quarantine "nUpdate Administration.app"`. The icon is made from `nUpdate.png` with Pillow, in
`nUpdate.Administration/Assets`: `python3 -c "from PIL import Image; Image.open('nUpdate.png').save('nUpdate.icns')"`.
It is not an Avalonia resource of the app.

## CI

`.github/workflows/ci.yml` runs on every push and on pull requests from forks. The Linux job checks the formatting,
runs every test including the Docker integration tests and the headless scenarios, merges the coverage reports and
applies the coverage gate. The packages job packs the NuGet packages on Linux (publishing the installer for all seven
runtime identifiers), builds a consumer project against them and runs the published-installer tests with the
linux-x64 installer. The Windows job runs the tests without Docker (including the Windows-only adapter tests), the
consumer project and the published-installer tests with the win-x64 installer, and publishes nUpdate Administration
for win-x64. The macOS job (Apple silicon) checks the signature of the osx-arm64 installer, runs the
published-installer tests, including the bundle swap, and builds the macOS bundle of nUpdate Administration for
osx-arm64. Coverage, test results, packages and the published administration are uploaded as artifacts.

## Layout

| Project | Target | Purpose |
|---|---|---|
| `nUpdate` | netstandard2.0 | The client library (`UpdateManager`, `UpdateFlow`) and the contracts shared with the installer and the administration: `UpdateFeed` (`nupdate.json`), `PackageManifest` (`manifest.json`), typed operations, `InstallerOptions`, `IProgressReporter` for installer windows, PEM keys and RSA-PSS signing. |
| `nUpdate.UpdateInstaller` | netstandard2.0 | The installer base library: the engine, `InstallerHost`, `WindowProgressReporter` as the base of installer windows, the windowless and logging reporters, the system adapters. |
| `nUpdate.UpdateInstaller.UI.Avalonia` | net10.0 | The built-in installer executable (Avalonia window), published self-contained per runtime identifier. |
| `nUpdate.UI.WindowsForms`, `nUpdate.UI.WPF` | net462, net8.0-windows | The built-in client user interfaces for Windows. |
| `nUpdate.UI.Avalonia` | net8.0 | The built-in client user interface for Avalonia. |
| `samples/CustomInstaller` | net8.0-windows | Sample of an installer of your own with a WPF window. |
| `samples/Aurora` | net10.0 | Sample application for trying updates against your own web and FTP server; `samples/Aurora/publish.sh <version>` publishes it with the installer (a signed `Aurora.app` on macOS). See its README. |
| `nUpdate.Administration.TransferInterface` | net10.0 | Transfer provider contract, also for plugins. |
| `nUpdate.Administration.Core` | net10.0 | Project folders and format 6 files (with the migration of every earlier format), project passwords, packages and feeds, publishing, the statistics API v2 client and script, the migration of legacy feeds, security. |
| `nUpdate.Administration` | net10.0 | The Avalonia administration app. |
| `nUpdate.Tests` | net10.0 | All tests: `Library/`, `Installer/`, `InstallerUi/` and `ClientUi/` (headless), `Administration/` (unit, view models, headless views), `Integration/` (Docker, trait `Category=Integration`; `Scenarios/` drives the real windows headlessly, one Given/When/Then class per dialog; `PublishedInstallerTests`, trait `Category=PublishedInstaller`), `Support/`. xUnit v3, NSubstitute, Shouldly, Avalonia.Headless, Testcontainers. |
| `tools/CoverageGate` | net10.0 | Cobertura threshold checker used by CI. |
| `tools/WinFormsScreenshots` | net8.0-windows | Renders the Windows Forms update dialog for the README (Windows only). |
