# nUpdate 5.0: cross-platform installer

Execution plan for the decisions agreed on 2026-10-07 (decisions 1 to 19). Branch: `db/formats-v2-and-portable-projects`.

The update installer only ran on .NET Framework 4.6.2 on Windows. After this change it runs on Windows, Linux and macOS:
a base library with the engine, a built-in Avalonia installer shipped as seven self-contained executables, and custom
installers that are executables of their own built on the base library.

## 1. Decisions

| # | Decision |
|---|---|
| 1B | A custom installer UI is an installer executable of its own, built on the base library. The plugin mechanism (`CustomUiAssemblyPath`, `ServiceProviderAttribute`, `ProgressReporterFactory`) is removed. |
| 2A+ | Projects: `nUpdate.UpdateInstaller` is the base library (engine, `InstallerHost`, the windowless path; namespace `nUpdate.UpdateInstaller`, .NET Standard 2.0), renamed from `nUpdate.UpdateInstaller.Core`. `nUpdate.UpdateInstaller.UI.Avalonia` holds only the Avalonia window and the entry point (.NET 10) and is the package applications reference for the built-in installer. `samples/CustomInstaller` replaces `nUpdate.WPFUpdateInstaller`; the Windows Forms installer is removed. |
| 3B | One zip per platform and version. The feed entry has `files: [{ platform, path, size, sha512, signature, touches }]` instead of `file`/`signature`/`touches`. Feed format 1 is unreleased and stays 1. |
| 3.1A | Platforms are .NET runtime identifiers (`win-x64`, `win-x86`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, `osx-arm64`), operating systems (`win`, `linux`, `osx`) and `any`. The client prefers its exact RID, then its operating system, then `any`; a version without a matching file is not offered. `architecture` and the `Architecture` enum are removed. |
| 3.1.1A | Migrated nUpdate 4 packages: x86 → `win-x86`, x64 → `win-x64`, Independent → `win`. |
| 3.2A | The package editor has a platform list in General ("Add platform") and a platform switcher in Files and Operations. New packages start with `any`. Version, changelog, Necessary, unsupported versions and rollout are shared; files and operations are per platform; every zip has its own `manifest.json`. |
| 3.3A | Server: `packages/<version>/<platform>.zip`. Locally: `packages/<version>/feed-entry.json` plus `packages/<version>/<platform>/` with `<platform>.zip` and `manifest.json`. |
| 4A | The C# script operation is removed (and Roslyn from the Administration). *Start process* gains `waitForExit` and `failOnError`. Migrated scripts get the existing "left out" warning. |
| 5B | The built-in installer is published self-contained, single-file and trimmed; Newtonsoft.Json and the nUpdate assemblies are rooted. All RIDs are built on Linux. |
| 6A | One package `nUpdate.UpdateInstaller.UI.Avalonia` with all seven builds. Its build targets copy the app's `RuntimeIdentifier`, or the RIDs in `nUpdateInstallerRuntimes`, or all seven with a warning, to `nUpdate.Installer/<rid>/`, and set the Unix execute bit; the client re-checks it. |
| 7B | The seven RIDs: win-x64, win-x86, win-arm64, linux-x64, linux-arm64, osx-x64, osx-arm64. |
| 8A | No elevation on Linux and macOS; `RunInstallerAsAdmin` only applies on Windows. Before starting the installer elsewhere, the client checks write access to the application folder (for a macOS bundle, the bundle's parent folder). |
| 9A | Windowless automatically without a display (Windows non-interactive; Linux without `DISPLAY`/`WAYLAND_DISPLAY`) or when the window fails to open; `ShowInstallerWindow = false` forces it. `install.log` is always written to the installer's temp folder; failures also go to the event log on Windows. |
| 10A | macOS: for a bundled app, `Program` (`%program%`) is the whole `.app`. The package contains the complete signed bundle; the installer builds `X.app.new` next to it, swaps the two, deletes the old bundle and restarts. |
| 11B | The installer UI targets .NET 10; the base library and the client stay .NET Standard 2.0. |
| 12C | `nUpdate.UI.WindowsForms` and `nUpdate.UI.WPF` also target `net8.0-windows`. New `nUpdate.UI.Avalonia` (net8.0): search, no-updates, new-update, download and error dialogs over `UpdateFlow`, with the installer window's look. |
| 13B | The installer window: progress, status line, inline Retry/Skip/Abort for locked files, errors with the log path and a Close button. `UpdateManager.InstallerIcon` (PNG path) and `InstallerAccentColor`. Light and dark follow the system. |
| 14 | Registry and service operations exist only for `win…` platforms; the editor and the installer refuse them elsewhere. |
| 15B | Gates: `nUpdate.UpdateInstaller` 100/100, `nUpdate.UpdateInstaller.UI.Avalonia` and `nUpdate.UI.Avalonia` 90/90. CI runs a real windowless update with the published installer on Linux, Windows and a new macOS job (osx-arm64), including the bundle swap of an ad-hoc signed test `.app` that must still verify and start. |
| 16A | `UpdateManager.InstallerPath` is the exact path of the installer executable (chosen per platform in app code). It replaces `InstallerDirectory` and `CustomUiAssemblyPath`. Default: `<appdir>/nUpdate.Installer/<rid>/nUpdate.UpdateInstaller.UI.Avalonia(.exe)`. |
| 17A | macOS installers ship ad-hoc signed (the SDK signs the apphost; verified in the macOS job). Developers sign and notarize their app including the nested installer; the README explains how. |
| 18A | Unix permissions: the builder stores the source file's mode on Linux/macOS; on Windows it detects executables by content (ELF `7F 45 4C 46`, Mach-O `FEEDFACE`/`FEEDFACF`/`CAFEBABE` in both byte orders, `#!`) as 0755, other files 0644. Modes go into the zip `ExternalAttributes` and are restored on install. Add folder (and Add files) refuse symbolic links. |
| 19A | The installer executable is `nUpdate.UpdateInstaller.UI.Avalonia(.exe)`, so it cannot collide with the base library `nUpdate.UpdateInstaller.dll`. |

Spike results (2026-10-07, Linux host): a trimmed, compressed single-file Avalonia 12.1.3 app is about 23 MB per RID;
the osx-arm64 apphost carries an embedded ad-hoc signature (`LC_CODE_SIGNATURE`, `fade0cc0`) when published from
Linux; `DllImport("libc")` `chmod` works; without a display `StartWithClassicDesktopLifetime` throws
`System.Exception: XOpenDisplay failed`; `File.Copy`/`File.Move` keep Unix modes; `UseShellExecute = true` on Linux
runs an executable directly; Newtonsoft.Json needs its own and the option types' assemblies rooted. The win-x64
publish output also contains native `.pdb` files (100 MB), which must not be packed.

## 2. Formats

### 2.1 Feed entry (`nupdate.json`, format 1)

```json
{
  "version": "2.1.0",
  "publishedAt": "2026-10-07T14:12:00+00:00",
  "necessary": false,
  "changelog": { "en": "…" },
  "unsupportedVersions": [],
  "rollout": { "mode": "any", "conditions": [] },
  "files": [
    { "platform": "win-x64", "path": "packages/2.1.0/win-x64.zip", "size": 1234567, "sha512": "base64",
      "signature": { "algorithm": "rsa-pss-sha512", "value": "base64" }, "touches": ["files", "registry"] },
    { "platform": "linux", "path": "packages/2.1.0/linux.zip", "size": 1200000, "sha512": "base64",
      "signature": { "algorithm": "rsa-pss-sha512", "value": "base64" }, "touches": ["files"] }
  ],
  "statistics": { "url": "nupdate-statistics.php", "enabled": true }
}
```

The loader rejects a package without files, a file without platform, path or signature, and a platform listed twice.
Unknown platform names are allowed and simply never match.

### 2.2 `manifest.json` (format 1)

Gains `platform`. The client checks project, version and platform of the downloaded zip against the feed file.

### 2.3 `installer-options.json` (format 2, unreleased)

`customUi` is replaced by `ui { showWindow, iconPath, accentColor }`; `application` gains `bundle` (the `.app` path or
`null`). `%program%` and the `Program` root resolve to `application.bundle ?? application.directory`.

### 2.4 Operations

`startProcess { path, arguments, waitForExit, failOnError }`. `executeScript` and `OperationArea.Scripts` are removed.

## 3. Client library (`nUpdate`)

- `nUpdate.Updating.PackagePlatform`: constants and the list of the eleven names, `Current` (OS from
  `RuntimeInformation.IsOSPlatform`, architecture from `ProcessArchitecture`, so an x64 build under Rosetta or a
  32-bit .NET Framework process keeps its own build), `Rank(filePlatform, clientRid)` (3 exact, 2 OS, 1 any, 0 none),
  `IsWindows(platform)`. The display names live in the Administration (`PlatformChoice`).
- `PackageInfo.Files` (`PackageFile { Platform, Path, Size, Sha512, Signature, Touches }`) and
  `PackageInfo.FindFile(platform)`; `Architecture`, `File`, `Signature`, `Touches` leave `PackageInfo`.
- `UpdateFilter` drops a package without a matching file; `UpdateFilterOptions.Platform` replaces `Is64BitOperatingSystem`.
- `UpdateManager`: `Platform` (defaults to `PackagePlatform.Current`), `InstallerPath`, `ShowInstallerWindow`,
  `InstallerIcon`, `InstallerAccentColor` (`#RRGGBB` or `#AARRGGBB`, checked by the setter); `InstallerDirectory`,
  `CustomUiAssemblyPath`, `InstallerExecutableName` and `DefaultInstallerDirectoryName` are replaced by
  `DefaultInstallerPath()` and `InstallerFolderName`. Downloads, sizes, hashes, signatures and manifests use the
  selected file. `StartInstaller` copies the folder of `InstallerPath` to the temp folder, copies the icon next to it,
  makes the copy executable on Unix, refuses without write access on Unix (localized `NoWriteAccess`), detects a macOS
  bundle from the executable path (`…/X.app/Contents/MacOS/x`) and only asks for elevation on Windows.
- Platform services: `ISystemInformation` gains `IsWindows`; new `IFilePermissions` (`CanWrite(directory)`,
  `MakeExecutable(path)`; libc `chmod` P/Invoke) in `UpdateManagerServices`; `ProcessLauncher` elevates only on Windows.
- `TouchesFormatter.Describe(packages, platform, texts)`; Windows Forms and WPF dialogs pass `manager.Platform`.
- `InstallerOptions.Ui`, `ApplicationOptions.Bundle`, new `InstallerText` keys (Waiting, Retry, Skip, Abort, Close,
  LogFile, Finished, ProcessExitCode, …), `ScriptExecute` removed; `UpdateTexts` and the six language files follow.
- Removed: `Architecture`, `ServiceProviderAttribute`, `ExecuteScriptOperation`, `OperationArea.Scripts`.

## 4. Base library (`nUpdate.UpdateInstaller`)

- `git mv` of `nUpdate.UpdateInstaller.Core` into `nUpdate.UpdateInstaller` after the Windows Forms installer is
  deleted; namespaces `nUpdate.UpdateInstaller`, `.Abstractions`, `.Operations`, `.Platform` (cross-platform adapters)
  and `.Windows` (registry, services, event log). Packable, so custom installers reference it.
- `InstallerHost.Run(args, createWindow)`: reads the options, decides windowless (`ui.showWindow == false` or
  `IEnvironmentInfo.HasDisplay == false`), wraps the window in `FallbackProgressReporter` (switches to the windowless
  reporter when the window throws), wraps everything in `LoggingProgressReporter` (`install.log` next to the options
  file), runs the engine on a background thread and returns 0/1/2. Errors before the options are read go to the log of
  the options folder (when known), stderr and on Windows the event log.
- `WindowlessProgressReporter`: retries a locked file three times with two seconds pause, then aborts; failures to
  stderr and the Windows event log (`RegisterEventSource`/`ReportEvent` P/Invoke, no new package).
- Engine: `StartProcess` waits and fails on a non-zero exit code when asked; registry and service operations are
  refused off Windows before anything is touched; zip `ExternalAttributes` modes are restored through `IFilePermissions`
  on Unix; for a bundle, the `Program` root is copied into `X.app.new` and swapped in (macOS `renamex_np(RENAME_SWAP)`,
  elsewhere two renames with roll back), the old bundle is deleted; a bundle package must contain
  `Contents/Info.plist`.
- Removed: `ProgressReporterFactory`, `ScriptOperationHandler`, `IScriptRunner`, `CodeDomScriptRunner`,
  `ScriptReferences`, `ScriptCompilationException`, `System.CodeDom`.

## 5. Built-in installer (`nUpdate.UpdateInstaller.UI.Avalonia`)

- net10.0 WinExe: `Program.Main` → `InstallerHost.Run(args, options => new InstallerWindowReporter(options))`.
- `InstallerWindowViewModel` (status, percentage, indeterminate until the first report, locked-file question with
  Retry/Skip/Abort, error state with message, log path and Close), `InstallerWindow` (icon from `ui.iconPath`, accent
  from `ui.accentColor`, Fluent theme following the system), `InstallerWindowReporter` (marshals to the UI thread; calls
  before the window is shown wait for it; a failed start releases waiting callers with an exception so the fallback
  takes over).
- Publish: `SelfContained`, `PublishSingleFile`, `PublishTrimmed`, `EnableCompressionInSingleFile`,
  `IncludeNativeLibrariesForSelfExtract`, `TrimmerRootAssembly` for Newtonsoft.Json, nUpdate and
  nUpdate.UpdateInstaller, `DebugType none`.
- Pack: a target publishes the seven RIDs into `installer/<rid>/` (only the executable), plus
  `build/nUpdate.UpdateInstaller.UI.Avalonia.targets`: selects `$(RuntimeIdentifier)`, else `$(nUpdateInstallerRuntimes)`,
  else all seven with warning `NUPD001`, copies them to `$(nUpdateInstallerFolderName)/<rid>/` and runs `chmod +x` after
  the copy on Unix hosts.
- Headless tests of the view model, the window and the reporter (90/90).

## 6. Client UIs

- `nUpdate.UI.WindowsForms` and `nUpdate.UI.WPF`: `TargetFrameworks` `net462;net8.0-windows`.
- `nUpdate.UI.Avalonia` (net8.0, packable): `UpdaterUI` (same API shape as the other two: `RunAsync`), a presenter
  implementing `IUpdateFlowPresenter`, one dialog window with a view model per step, and the look of the installer
  window: the same layout and accent (`InstallerAccentColor` applies to both). The two do not share a resource file,
  so the installer does not depend on the client UI. The texts of the new-update dialog of all three UIs come from
  `nUpdate.Ui.UpdateSummary`. Headless tests 90/90.

## 7. Administration

- `PackageDefinition { Version, Platforms: List<PlatformPackage { Platform, Files, Operations }> }`;
  `PackageFileEntry.UnixMode`; `UnixModeDetector` (source mode on Unix, content sniffing on Windows); the builder writes
  `ExternalAttributes = mode << 16` and the manifest's `platform`, and refuses symbolic links.
- Layout per 3.3A in `UpdateProject` (`PackageDirectory`, `PlatformDirectory`, `PackageFilePath(version, platform)`)
  and `PackageLayout.RemotePackagePath(version, platform)`.
- `PublishService`: builds, hashes and signs one zip per platform, uploads each, deletes each (and the version folder)
  on removal; `PublishExistingAsync` reads the platforms from the entry.
- `FeedChecker` checks every file of every package.
- Migration: `LegacyFeedEntry.Platform` (3.1.1A) instead of `Architecture`; `LegacyFeedMigrator` and `ProjectMigrator`
  write the new layout; script operations are left out with a warning.
- Package editor: platform list with "Add platform" and remove; platform switcher above Files and Operations; the
  operation palette hides registry and service operations for non-Windows platforms and validation refuses them;
  `StartProcess` editor gets the two check boxes; the script editor, `ScriptValidator`, AvaloniaEdit and
  `Microsoft.CodeAnalysis.CSharp` go (AvaloniaEdit only if nothing else uses it).
- Scenarios and screenshots follow.

## 8. CI

- `packages` job (ubuntu): pack everything (publishes the seven installers), upload `nuget-packages` and
  `installer-builds`.
- `linux` job: format, build, tests with Docker, coverage gates with the new names, then the published-installer test
  (`NUPDATE_INSTALLER` = linux-x64 build) — needs `packages`.
- `windows` job (needs `packages`): unit tests, consumer smoke test (`nUpdate.Installer/win-x64/…exe` exists), the
  published-installer test with the win-x64 build, Administration publish.
- `macos` job (needs `packages`, macos-latest = arm64): `codesign --verify` of the osx-arm64 installer, the
  published-installer test including the bundle swap of an ad-hoc signed test `.app` and `codesign --verify --deep
  --strict` plus a start of the swapped bundle.
- The published-installer test lives in `nUpdate.Tests/Integration/PublishedInstallerTests.cs` with trait
  `Category=PublishedInstaller`; it skips unless `NUPDATE_INSTALLER` names an existing file; the regular runs exclude the
  trait.

## 9. Docs

README (installer section, platforms, custom installer, macOS signing and notarization, examples without
`InstallerDirectory`/`ServiceProvider`, Avalonia UI), `docs/nuget/README.md`, `BUILDING.md` (projects, gates, the
published-installer test, packing), `samples/CustomInstaller/README.md`.

## 10. Phases (one commit each, pushed)

| Phase | Content | Check |
|---|---|---|
| 0 | This plan | – |
| 1 | Client library: platforms, files list, manifest platform, operations, `UpdateManager` installer options, Unix services, texts; installer engine adapted just enough to compile (scripts removed) | Library and installer tests, nUpdate 100/100 |
| 2 | Base library rename, `InstallerHost`, reporters, log, event log, permissions, bundle swap, Windows-only guard; Windows Forms installer and WPF sample removed | Installer tests, 100/100 |
| 3 | `nUpdate.UpdateInstaller.UI.Avalonia` with window, publish and pack targets; `samples/CustomInstaller` | Headless tests 90/90, pack and consumer build locally |
| 4 | Administration: per-platform packages, layout, publish, checker, migration, editor; scripts and Roslyn removed | All tests, scenarios, gates |
| 5 | Client UIs: multi-targeting, `nUpdate.UI.Avalonia` | Headless tests 90/90 |
| 6 | CI jobs and the published-installer test | Local run of the test against a local publish; CI green on all jobs |
| 7 | Docs | Review |
| 8 | Full Release run with Docker, gates, format; self-review; fixes | – |
