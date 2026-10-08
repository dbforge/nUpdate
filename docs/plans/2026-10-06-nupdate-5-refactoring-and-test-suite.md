# nUpdate 5.0: refactoring, modernisation and full test suite

Branch: `db/refactorings-and-test-suite`. Date: 2026-10-06.
This plan executes the 29 decisions agreed in the planning session (numbered 1 to 27, plus 10.1 and 26.1).
Every section references the decision it implements.

## 1. Target solution layout

| Project | Target | Role | Key packages |
|---|---|---|---|
| `nUpdate.Shared` | netstandard2.0 | Shared contract types: `UpdateVersion`, `DevelopmentalStage`, `Operation`, `OperationArea`, `OperationMethod`, `UpdateArgument`, `UpdateArgumentExecutionOptions`, `HostApplicationOptions`, `InstallerOptions`, `Serializer` (9A) | Newtonsoft.Json, PolySharp |
| `nUpdate` | netstandard2.0 | Client library 5.0 (11A, 12A, 17A) | Newtonsoft.Json, PolySharp |
| `nUpdate.UpdateInstaller.UIBase` | netstandard2.0 | `IProgressReporter`, `ServiceProviderAttribute` | PolySharp |
| `nUpdate.UpdateInstaller.Core` | netstandard2.0 | Options parsing, `InstallEngine`, operation handlers, abstractions, Windows adapters (7A, 8A) | nUpdate.Shared, UIBase, TestableIO.System.IO.Abstractions.Wrappers, Microsoft.Win32.Registry, System.ServiceProcess.ServiceController, System.CodeDom |
| `nUpdate.UpdateInstaller` | net461 WinExe | `Main`, WinForms reporter, event-log reporter | Core, Microsoft.NETFramework.ReferenceAssemblies |
| `nUpdate.WPFUpdateInstaller` | net461 library | Minimal WPF custom-UI sample (18B) | UIBase |
| `nUpdate.Administration.TransferInterface` | net10.0 | Async `ITransferProvider`, `TransferProgress`, `ServerItem`, `ServiceProviderAttribute` (25A) | - |
| `nUpdate.Administration.Core` | net10.0 | Services, models, transfer, statistics client, credentials, migration (6A, 15A, 16A, 26A) | nUpdate, TransferInterface, FluentFTP, SSH.NET, Microsoft.AspNetCore.DataProtection, TestableIO.System.IO.Abstractions.Wrappers, Microsoft.Extensions.Logging.Abstractions, Microsoft.CodeAnalysis.CSharp |
| `nUpdate.Administration` | net10.0 Avalonia | Views, view models, composition root (2D, 6A) | Avalonia 12, CommunityToolkit.Mvvm, Microsoft.Extensions.DependencyInjection, ScottPlot.Avalonia, Avalonia.AvaloniaEdit |
| `nUpdate.UI.WindowsForms` | net461 | Adapted dialogs (19A) | nUpdate |
| `nUpdate.UI.WPF` | net461 | Adapted dialogs (19A) | nUpdate |
| `nUpdate.Test` | net10.0 | Unit tests for `nUpdate` and `nUpdate.Shared` | xunit.v3, NSubstitute, Shouldly, coverlet |
| `nUpdate.UpdateInstaller.Test` | net10.0 | Unit tests for Installer.Core and UIBase | same |
| `nUpdate.Administration.Test` | net10.0 | Unit tests for Admin.Core, TransferInterface, view models, headless views | same + Avalonia.Headless.XUnit |
| `nUpdate.Administration.IntegrationTest` | net10.0 | Testcontainers FTP, SFTP, php:apache, MySQL; end-to-end (5A+) | same + Testcontainers |

Chart library note: LiveCharts2 (decision 14) only has a dev prerelease for Avalonia 12. ScottPlot.Avalonia 5.1.x, the agreed alternative, targets Avalonia 12 stable, so it is used.

Solution files: `nUpdate.sln` (everything) and `nUpdate.Linux.slnf` (everything except the net461 projects, used by the Linux CI job and local Linux runs; note that the net461 projects compile on Linux too, they are excluded only from test runs).

## 2. Repository baseline (1A, 13A, 22A)

- `global.json` pins SDK 10.0.x with `rollForward: latestFeature`.
- `Directory.Build.props`: `LangVersion` latest, `Nullable` enable, `ImplicitUsings` enable, `TreatWarningsAsErrors` true, `AnalysisLevel` latest-recommended, `EnforceCodeStyleInBuild` true, `Deterministic`, common package metadata (authors, licence MIT, repo URL, version 5.0.0), `GenerateDocumentationFile` for the library projects.
- `Directory.Packages.props`: central package management, every version pinned.
- `.editorconfig`: file-scoped namespaces, `var` preferences, naming rules mirroring the existing `.DotSettings`, severity for IDE rules.
- `.gitignore`: add `*.DotSettings.user`, `TestResults/`, `coverage/`, `.idea/`.
- Delete: `MigrationBackup/`, `.nuget/`, `packages/`, `nUpdate.sln.GhostDoc.xml`, `nUpdate.sln.DotSettings.user`, `nUpdate/Resources/*` binaries and their resx entries, `nUpdate.Test/` (24B), old-style csproj files.
- Package icon: `assets/nupdate-icon.png` in the repo (the old csproj pointed at a Downloads path).

## 3. Phase plan

Each phase ends with a green `dotnet build nUpdate.Linux.slnf` and `dotnet test` for the affected test projects, then a commit.

### Phase 0: baseline
Section 2 plus a new `nUpdate.sln` built with `dotnet sln`. Existing source is kept compiling by converting the old csproj files to SDK style with the frameworks in section 1; the net461 shells get `EnableWindowsTargeting` and `Microsoft.NETFramework.ReferenceAssemblies`.

### Phase 1: nUpdate.Shared (9A)
Move the shared types; `UpdateVersion` becomes immutable with a single `CompareTo` from which all operators derive, consistent `Equals`/`GetHashCode`, a `TryParse`, and a throwing constructor for invalid input. `SemanticVersion` is computed from the components (fixes the `.Replace(".0", "")` bug). `Operation` gets typed payload helpers; the tag mapping becomes a single dictionary. `InstallerOptions` is a plain DTO with `ContractVersion`, `PackagePaths`, `ApplicationDirectory`, `ApplicationExecutablePath`, `ApplicationName`, `HostProcessId`, `HostApplicationOptions`, `Arguments`, `CustomUiAssemblyPath`, `Texts` (dictionary of the localized installer strings) and `ShowErrorsInUi`.

### Phase 2: nUpdate library (11A, 12A, 17A)
- `UpdateManager(Uri configUri, string publicKey, CultureInfo? culture = null, UpdateVersion? currentVersion = null)` plus `UpdateManager(UpdateManagerOptions options, UpdateManagerServices? services = null)`. `UpdateManagerServices` bundles `HttpClient`, `IFileSystem`, `IProcessLauncher`, `IApplicationTerminator`, `IEnvironmentInfo` (architecture, OS name), `ISystemInformation`, `IClock`. Defaults are real implementations.
- Public async API: `SearchForUpdatesAsync(CancellationToken)`, `DownloadPackagesAsync(IProgress<UpdateDownloadProgress>?, CancellationToken)`, `ValidatePackagesAsync(CancellationToken)`, `InstallPackage()`, `DeletePackages()`, `ReportDownloadsAsync` is internal to the download step. Properties: `PackageConfigurations`, `TotalSize`, `IncludeAlpha`, `IncludeBeta`, `IncludeCurrentPcIntoStatistics`, `Conditions`, `LanguageCulture`, `CultureFilePaths`, `PublicKey`, `CurrentVersion`, `Proxy`, `HttpAuthenticationCredentials`, `HostApplicationOptions`, `RunInstallerAsAdmin`, `Arguments`, `CustomInstallerUiAssemblyPath`, `InstallerDirectory`, `UseDynamicUpdateUri`, `CloseHostApplication`.
- `UpdateConfiguration`: drop `VersionId`; keep the other fields; `UpdatePhpFileUri` keeps its JSON name for compatibility and gets a `StatisticsUri` alias in code. Static downloads replaced by `UpdateConfigurationLoader` (HttpClient-based, injectable).
- `UpdateResult` becomes `UpdateFilter` with injected architecture and a pure `Filter(IEnumerable<UpdateConfiguration>)`; `CheckConditions` becomes `RolloutConditionEvaluator`.
- `RsaManager` → `RsaSignatureProvider` (same XML key format, SHA512, PKCS#1); stream verification disposes correctly.
- `LocalizationHelper` → `LocalizationProvider` with embedded + file cultures via `IFileSystem`.
- `SystemInformation`: Windows 11 detection via build number, injectable.
- Statistics report: `POST {StatisticsUri}` JSON `{ "projectId", "version", "os" }`; treated as best effort (logged, never aborts the download) and only sent if `UseStatistics` and `IncludeCurrentPcIntoStatistics`.
- Installer launch: write `installer-options.json` to `%TEMP%/nUpdate Installer/<guid>/`, copy `InstallerDirectory` contents there, start with the json path as the single argument.
- Helpers moved in from the UI libraries (19A): `ChangelogFormatter`, `UpdateSizeCheck`, `UpdateErrorMessages`, `VersionRangeFormatter`.
- Bug list with regression tests: see section 5.

### Phase 3: UpdateInstaller (7A, 8A, 18B)
Core: `InstallerOptionsReader` (JSON file → options, validates `ContractVersion`), `InstallEngine` (`Task<InstallResult> RunAsync(InstallerOptions, IProgressReporter, CancellationToken)`), `ProgressTracker`, `IOperationHandler` with `FileOperationHandler`, `RegistryOperationHandler`, `ProcessOperationHandler`, `ServiceOperationHandler`, `ScriptOperationHandler`, `OperationDispatcher`, `PackageExtractor` (System.IO.Compression), `DirectoryCopier` with bounded retry via `ILockedFileResolver`, `HostProcessWaiter`, `HostRestarter`, `PathPlaceholderResolver` (`%program%` etc.), abstractions `IRegistry`, `IServiceController`, `IProcessService`, `IScriptRunner`, `ISpecialFolders`, `IDelay`. Windows adapters in `Windows/` with `[SupportedOSPlatform("windows")]` and `[ExcludeFromCodeCoverage]` plus Windows-only tests.
Shell: `Program.Main` reads the path, builds the engine with real adapters, chooses `WinFormsProgressReporter` or `EventLogProgressReporter`, runs. `IProgressReporter` gains `ReportLockedFile(path, attempt) → LockedFileDecision` (Retry, Skip, Abort).
WPF sample: `MainWindow : IProgressReporter`, `InstallerServiceProvider`, nothing else.

### Phase 4: Administration.Core (6A, 15A, 16A, 25A, 26A, 26.1A)
Models: `UpdateProject` (v5: `ConfigVersion`, `Guid`, `Name`, `Path`, `UpdateUrl`, `AssemblyVersionPath`, `Transfer` (`TransferSettings`: `Protocol`, `Host`, `Port`, `Directory`, `Username`, `ProtectedPassword`, `UsePassiveMode`, `FtpsEncryptionMode`, `TrustedCertificateFingerprint`, `SftpPrivateKeyPath`, `ProtectedSftpKeyPassphrase`, `TrustedHostKeyFingerprint`, `PluginAssemblyPath`), `Proxy` settings, `HttpAuthentication`, `Statistics` (`Enabled`, `EndpointUri`, `ProtectedAdminSecret`), `ProtectedPrivateKey`, `PublicKey`, `SaveCredentials`, `Packages`, `Log`), `UpdatePackage`, `ProjectConfiguration`, `LogEntry`.
Services (all interfaces + implementations, all async, `IFileSystem` injected): `IProjectStore`, `IProjectMigrator` (v3 → v5 with `LegacyCredentialDecryptor`), `ICredentialProtector` (DataProtection), `IProjectExporter` (password-encrypted zip, PBKDF2-SHA256 600k + AES-GCM), `IPackageBuilder`, `IPackageSigner`, `IUpdateConfigurationStore`, `IPublishService` (step list with compensation), `IProjectService`, `IStatisticsClient`, `IStatisticsScriptTemplate`, `ITransferProviderFactory` (+ `FtpTransferProvider`, `SftpTransferProvider`, plugin loading), `IScriptValidator` (Roslyn, C# 5), `IPackageContentReader`, `IChangelogService`.
PHP: `Resources/statistics.php` rewritten as the HTTP API (section 6).

### Phase 5: Integration environment (5A+)
`nUpdate.Administration.IntegrationTest`: `ServerFixture` (xUnit collection fixture) creating one Docker network and one volume, containers: `fauria/vsftpd` (FTP + FTPS with a generated self-signed cert), `atmoz/sftp`, `php:8.3-apache` with `mysqli`, `mysql:8.4`. Tests: FTP provider contract, SFTP provider contract, statistics API (register/report/read), end-to-end publish → client search/download/validate via `nUpdate` → `InstallEngine` apply to temp dir → statistics report → admin reads counts. Tests are skipped with a clear message when Docker is unavailable.

### Phase 6: Avalonia Administration (2D, 6A)
App: `App.axaml`, DI container, `MainWindow` with project list; dialogs as `Window`s hosted by `IDialogService`; views: `ProjectView` (overview, packages, statistics with ScottPlot), `NewProjectDialog` (wizard), `ProjectEditDialog`, `PackageEditorView` (add/edit, file tree, changelog per culture, operations panels, AvaloniaEdit for scripts), `ImportExportDialog`, `DirectoryBrowserDialog`, `AboutDialog`. View models with CommunityToolkit.Mvvm; services `IDialogService`, `IFilePickerService`, `IClipboardService`. Tests: view model tests with substituted services; headless view tests for each view (binds, loads, basic interaction).

### Phase 7: UI libraries (19A)
Adapt `nUpdate.UI.WindowsForms` and `nUpdate.UI.WPF` to the async API and the moved helpers; delete duplicated helper classes; fix the WPF error-text bug; remove the service locator in favour of constructor injection.

### Phase 8: CI and docs (21A)
`.github/workflows/ci.yml` with `linux` (build slnf, unit tests with coverage thresholds, integration tests, ReportGenerator artifact) and `windows` (full build, Windows-only tests, `dotnet pack` artifacts). `BUILDING.md` with local commands. README: 5.0 migration note, installer deployment (10.1A), statistics API, SFTP.

### Phase 9: pass-all-checks and self-review
Run `dotnet format --verify-no-changes`, full build, all unit tests with coverage gates, integration tests; then `/self-review` and fold in findings.

## 4. Coverage gate (20A)
*Superseded during implementation: coverage comes from Microsoft.Testing.Extensions.CodeCoverage and the thresholds are
checked by `tools/CoverageGate` (see BUILDING.md). The test names in section 5 are indicative; the regression tests
exist under the names in the test classes.*

`coverlet.collector` via `runsettings` with `Threshold=100` (line, branch) for assemblies `nUpdate`, `nUpdate.Shared`, `nUpdate.Administration.Core`, `nUpdate.Administration.TransferInterface`, `nUpdate.UpdateInstaller.Core`, `nUpdate.UpdateInstaller.UIBase`, and `Threshold=90` for `nUpdate.Administration` (view models). Exclusions: `[ExcludeFromCodeCoverage]` with justification on Windows adapters, `Program`/`App` composition roots, generated code (`*.g.cs`, `*.axaml.cs` partial glue), PolySharp output.

## 5. Bugs fixed with regression tests (17A)
| # | Location (old) | Bug | Test |
|---|---|---|---|
| B1 | UpdateManager ctor | throws when culture is null | `Ctor_DefaultsToEnglish` |
| B2 | UpdateManager ctor | custom-UI check before properties set | removed by design |
| B3 | LanguageCulture setter | checks old value | `LanguageCulture_UsesCustomFile` |
| B4 | InstallPackage | DotNetZip bytes written as nUpdate.dll | removed by 10C |
| B5 | Download sync/async | inconsistent statistics response handling | `Download_StatisticsFailureDoesNotAbort` |
| B6 | Download | duplicate version add throws | `Download_TwiceSameVersion` |
| B7 | DownloadAsync | NaN progress on zero total | `Progress_ZeroTotal` |
| B8 | UpdateVersion.SemanticVersion | strips ".0" everywhere | `SemanticVersion_1_10_0_0` |
| B9 | UpdateVersion | null operators, Equals vs GetHashCode | `Equality_Consistent`, `Operators_Null` |
| B10 | UpdateVersion | dead `Build = 0` branch | `Parse_TwoDigit_SetsMinor` |
| B11 | SystemInformation | Windows 11 reported as 10 | `OsName_Windows11` |
| B12 | UpdateResult | hard-wired architecture | `Filter_32BitOnly` |
| B13 | GetUpdatePackageSize | bare catch | `Search_HeadFailure_Throws` |
| B14 | ValidatePackages | invalid key swallowed | `Validate_InvalidKey_Throws` |
| B15 | WebClientWrapper | user-agent twice | removed by 11A, `Http_UserAgentOnce` |
| B16 | Installer path split | drops repeated segments | `PlaceholderResolver_RepeatedSegment` |
| B17 | Installer DeleteValue | wrong progress text | `Registry_DeleteValue_Text` |
| B18 | Installer service start | string[] cast on JArray | `Service_Start_Payload` |
| B19 | Installer | restart after failure continues loop | `Engine_StopsAfterFailure` |
| B20 | Installer | infinite locked-file loop | `Copier_BoundedRetry` |
| B21 | Installer | missing operations fallback NRE | `Engine_NoOperationsFile` |
| B22 | RegistryManager | disposes root keys | adapter rewritten; `Registry_Adapter` (Windows) |
| B23 | Navigator | first index out of range | `Navigator_FirstCurrent` |
| B24 | ProjectImport | load before extract | `Import_ExtractsFirst` |
| B25 | FtpTransferService.MoveContent | last-char compare | `Transfer_MoveContent` (integration) |
| B26 | UploadPackageFinished | race on exception | removed by async contract |
| B27 | statistics.php | invalid timezone, PHP 8 errors | `Statistics_Report` (integration) |
| B28 | UI.WPF UpdaterUI | install failure shown as signature error | `ErrorMessages_InstallFailure` |
| B29 | MainDialog_Shown | NRE on null passwords | removed by design |

## 6. Statistics HTTP API (26A, 26.1A)
`statistics.php`, configured by a `statistics.config.php` written next to it (DB host, user, password, name, admin secret hash).
- `POST ?action=report` body `{projectId, version, os}` → 204. Unauthenticated. Creates the application/version rows on demand.
- `POST ?action=register` header `Authorization: Bearer <secret>` body `{projectId, name, version}` → 204.
- `DELETE ?action=version&projectId=&version=` bearer → 204.
- `GET ?action=statistics&projectId=` bearer → `{ "versions": [{ "version", "downloads", "byOs": {"Windows 11": n} }], "total": n }`.
- `GET ?action=setup` bearer → creates tables, 204.
Tables: `nupdate_application(project_id CHAR(36) PK, name)`, `nupdate_download(id PK, project_id, version, os, downloaded_at)`.

## 7. Installer options file (8A)
```json
{ "contractVersion": 1, "packagePaths": [...], "applicationDirectory": "...", "applicationExecutablePath": "...", "applicationName": "...", "hostProcessId": 1234, "hostApplicationOptions": "CloseAndRestart", "arguments": [...], "customUiAssemblyPath": null, "texts": { "ExtractingFiles": "...", ... } }
```

## 8. Project file v5 (15A)
`ConfigVersion: "v5"`. Loader: if `ConfigVersion` is `v3` or missing → `ProjectMigrator.MigrateV3` (decrypt with legacy AES constants, protect with `ICredentialProtector`, map `FtpProtocol` int → `TransferProtocol`, derive `Statistics.EndpointUri` from `UpdateUrl` + `statistics.php`, drop SQL fields, keep packages and log). Unknown versions throw `UnsupportedProjectVersionException`.
