# nUpdate 5.0: format redesign, migrator and portable projects

Execution plan for the decisions agreed on 2026-10-07 (decisions 1 to 15.1). Branch: `db/formats-v2-and-portable-projects`.

## 1. Decisions

| # | Decision |
|---|---|
| 1A | The 5.0 feed is `nupdate.json`. A legacy `updates.json` on the server is never touched; 3.x/4.x clients keep working from it. |
| 2A | `nupdate.json` is a document: `format`, `projectId`, `packages[]`; camelCase; string enums; changelog keyed by culture name; relative `file.path` with `size` and `sha512`; `signature { algorithm, value }`; per-entry `statistics { url, enabled }`; `publishedAt`; `touches[]`. |
| 3A | `UpdateVersion` is written as one canonical string by a JSON converter everywhere; no `Literal…` twins. |
| 4A | One sealed class per operation with a `type` discriminator; typed registry values; `OperationTag`, `Value2` and `InstallerOptions.ConfigurationOperations` are gone; the feed never carries operations. |
| 5A+ | The package zip carries `manifest.json` (`format`, `projectId`, `version`, `createdAt`, `operations`); only non-empty root folders; `PackageRoot` shared; server path `packages/<version>.zip`. |
| 6A | Integer `format` at the root of every JSON document: feed 1, manifest 1, installer options 2, projconf 1, project file 6 (`"v5"` and older still readable). |
| 7A | The Administration migrates legacy projects: repack, re-sign and upload every released package to the new path, write the new feed and statistics script, convert local folders; old server files untouched; prompt on open; publish refused until migrated; settings action; compensating pipeline. |
| 8A− | Project file: typed versions, string enums, camelCase, format 6; `projconf.json` entries carry the Id. |
| 9X | Statistics: REST API v2 (`/v2`, `/v2/downloads`, `/v2/projects/{id}/statistics`, `PUT`/`DELETE /v2/projects/{id}/versions/{version}`, `DELETE /v2/projects/{id}`), Bearer auth, JSON errors, no compatibility. |
| 10A | PEM keys (same key pair, converted by the migrator), RSA-PSS with SHA-512 via `RSACng` on .NET Framework (`System.Security.Cryptography.Cng`), algorithm named in the feed. |
| 11A | `installer-options.json` format 2: `packages`, `application`, `host { processId, afterInstall }`, `arguments [{ value, when }]`, `customUi`, `texts`; enums `AfterInstall` and `ArgumentCondition`. |
| 12A+ | Client API renames (table below); helpers internal or replaced by BCL; `UpdateManagerServices.Logger` (`ILogger`) instead of the event; `SizeCalculationException` gone; `PackageDeleteException` logged; `InvalidFeedException`. |
| 13A | `PreReleasePolicy { None, ReleaseCandidates, Betas, All }` plus `AcceptedPreReleaseLabels`; stage classification internal; filter options object. |
| 14B | `UpdateVersion.ToString()` is the only string form; `FullText`, `SemanticVersion`, `FromFullText`, `DevelopmentBuild` removed; `BasicVersion` becomes `Release`. |
| 15X | A project is a folder: `project.nupdproj` plus `packages/<version>/`, openable from anywhere; the wizard asks for the folder; export/import removed. |
| 15.1A | Saved secrets are encrypted in the project file with a project password (PBKDF2-SHA256, AES-GCM); the password is remembered per user and machine; the migrator asks for it on first open. |

## 2. Formats

### 2.1 `nupdate.json` (feed, format 1)

```json
{
  "format": 1,
  "projectId": "8f3c0a2e-5b1d-4e8a-9c7f-2d6b1e4a9f10",
  "packages": [
    {
      "version": "2.1.0-beta.1",
      "publishedAt": "2026-10-05T14:12:00+00:00",
      "architecture": "x64",
      "necessary": false,
      "changelog": { "en": "…", "de-DE": "…" },
      "unsupportedVersions": ["1.0.0"],
      "rollout": { "mode": "all", "conditions": [{ "key": "Region", "value": "EU", "negated": false }] },
      "touches": ["files", "registry"],
      "file": { "path": "packages/2.1.0-beta.1.zip", "size": 1234567, "sha512": "base64" },
      "signature": { "algorithm": "rsa-pss-sha512", "value": "base64" },
      "statistics": { "url": "statistics.php", "enabled": true }
    }
  ]
}
```

`file.path` and `statistics.url` are resolved relative to the feed URL; absolute URLs are honoured. `touches` lists the `OperationArea`s of the package's operations.

### 2.2 `manifest.json` (inside the package zip and next to it locally, format 1)

```json
{ "format": 1, "projectId": "…", "version": "2.1.0-beta.1", "createdAt": "…", "operations": [ { "type": "deleteFiles", "runBeforeFileReplacement": true, "directory": "%program%", "files": ["old.dll"] } ] }
```

Operation types and fields: `deleteFiles { directory, files }`, `renameFile { path, newName }`, `createRegistryKeys { key, subKeys }`, `deleteRegistryKeys { key, subKeys }`, `setRegistryValues { key, values[{ name, kind, value }] }`, `deleteRegistryValues { key, names }`, `startProcess { path, arguments }`, `terminateProcess { processName }`, `startService { serviceName, arguments[] }`, `stopService { serviceName }`, `executeScript { source }`. Registry kinds: `string`, `expandString`, `dword`, `qword`, `multiString`, `binary` (value base64).

### 2.3 `installer-options.json` (format 2)

```json
{
  "format": 2,
  "packages": [{ "path": "…\\2.1.0-beta.1.zip" }],
  "application": { "name": "…", "directory": "…", "executablePath": "…" },
  "host": { "processId": 4711, "afterInstall": "restart" },
  "arguments": [{ "value": "--updated", "when": "succeeded" }],
  "customUi": { "assemblyPath": null },
  "texts": { "…": "…" }
}
```

### 2.4 `project.nupdproj` (format 6)

```json
{
  "format": 6,
  "id": "…", "name": "Trade Updater", "updateUrl": "https://…/",
  "assemblyVersionPath": null,
  "transfer": { "protocol": "sftp", "host": "…", "port": 22, "directory": "/updates", "username": "deploy", "usePassiveMode": true, "trustedCertificateFingerprint": null, "sftpPrivateKeyPath": null, "trustedHostKeyFingerprint": "…", "pluginAssemblyPath": null, "proxy": null },
  "httpAuthentication": null,
  "statistics": { "enabled": true, "endpointUrl": "https://…/statistics.php", "database": { "host": "mysql", "name": "nupdate", "username": "nupdate" } },
  "publicKey": "-----BEGIN PUBLIC KEY-----…",
  "secrets": "base64 PasswordProtectedData blob or null",
  "packages": [{ "version": "2.1.0-beta.1", "description": "…", "released": true, "createdAt": "…" }],
  "log": [{ "kind": "upload", "at": "…", "version": "2.1.0-beta.1", "user": "…" }]
}
```

`secrets` holds the serialized `ProjectSecrets` (transfer password, SFTP passphrase, proxy password, HTTP password, statistics admin secret, database password, private key PEM) encrypted with the project password. `null` means "credentials are not saved". The project folder is `<folder>/project.nupdproj` plus `<folder>/packages/<version>/{<version>.zip, manifest.json}`.

### 2.5 `projconf.json` (format 1)

`{ "format": 1, "projects": [{ "id": "…", "name": "…", "path": "…/project.nupdproj" }] }`. The remembered project passwords live in `passwords.json` next to it, protected with Data Protection per entry (`{ "<projectId>": "protected" }`).

### 2.6 Statistics API v2 (`statistics.php`)

Routing by `PATH_INFO`. Bodies and responses camelCase JSON. Admin routes need `Authorization: Bearer <secret>`. Errors `{ "error": { "code", "message" } }` with 400/401/404/405/415. Tables `nupdate_version (project_id, version, created_at)` and `nupdate_download (id, project_id, version, os, reported_at)`.

## 3. Type inventory

### nUpdate (client library and contracts)

- `Updating`: `UpdateVersion` (canonical `ToString`, `Release`, `PreReleaseStage` internal), `UpdateVersionJsonConverter`, `UpdateFeed`, `PackageInfo`, `PackageFile`, `PackageSignature`, `PackageStatistics`, `RolloutSettings`, `RolloutCondition`, `RolloutConditionMode { Any, All }`, `Architecture { Any, X86, X64 }`, `PreReleasePolicy`, `UpdateManager`, `UpdateManagerServices` (+ `Logger`), `UpdateDownloadProgress`, `InstallerArgument`, `ArgumentCondition`, `AfterInstall`; internal `FeedLoader`, `UpdateFilter` (+ `UpdateFilterOptions`), `RolloutConditionEvaluator`, `StatisticsReporter`.
- `Operations`: `Operation` (abstract, `RunBeforeFileReplacement`, `Area`), the eleven sealed classes, `OperationArea`, `RegistryValue`, `RegistryValueKind`, `OperationJsonConverter`.
- `Packaging`: `PackageManifest`, `PackageRoot { Program, AppData, Temp, Desktop }`, `PackageLayout` (file names, root folder names).
- `Installer`: `InstallerOptions` (format 2) with `ApplicationInfo`, `HostOptions`, `CustomUiOptions`; `IProgressReporter`, `LockedFileDecision`, `ServiceProviderAttribute`, `InstallerText`.
- `Security`: internal `PackageSigning` (PSS/SHA-512, `RSACng` on .NET Framework), `RsaKeyPem` (hand-written DER for SPKI and PKCS#8).
- `FormatVersion` helper (root namespace) (`Check(actual, current, documentName)` throws `UnsupportedFormatException`).
- `Exceptions`: `InvalidFeedException`, `UnsupportedFormatException`, `InvalidSignatureException` (existing ones renamed where needed).
- Internal: `Serializer` (camelCase, string enums, version and operation converters), `UriHelper` removed in favour of `new Uri(base, relative)`, `Ui` toolbox internal.

### nUpdate.UpdateInstaller.Core

- `InstallerOptionsReader` (format 2), `InstallEngine` reads the manifest, `OperationExecutor` with one handler per operation class, roots from `PackageLayout`.

### nUpdate.Administration.Core

- `Models`: `UpdateProject` (format 6, `Folder`, `PackagesDirectory`), `UpdatePackage` (typed), `LogEntry` (typed), `ProjectSecrets`, `ProjectRegistration` (id, name, path), `ProjectPasswordStore`.
- `Projects`: `ProjectStore` (format 6, legacy branch), `ProjectMigrator` (v3 and v5 → 6; keys XML → PEM; secrets returned in memory), `ProjectSecretsProtection` (project password), `ProjectService` (create in a chosen folder, rename changes the name only, `SaveMigratedAsync`, delete), `ProjectPasswordStore`.
- `Packages`: `PackageBuilder` (manifest, non-empty roots), `PackageSigner` (PSS, SHA-512 hash), `FeedStore` (remote `nupdate.json` and the local `feed-entry.json` of every package), `PackageContentReader` (manifest).
- `Publishing`: `PublishService` (new paths, feed entries with size, hash, touches, `MigrationRequiredException`); `Migration`: `LegacyFeed` reader, `LegacyOperationConverter`, `LegacyFeedMigrator` (7A pipeline: repack, sign, upload, feed, statistics, local folders) and `MigrationStatus` (legacy feed present, feed present). The statistics script is uploaded by `ProjectService.SetupStatisticsAsync` as before, not by the migrator.
- `Statistics`: `StatisticsApi` (typed v2 client), `StatisticsScript` (v2 PHP).
- Removed: `Export/*`, `LegacyAesCredentialDecryptor` stays (needed by the v3 migrator).

### nUpdate.Administration (Avalonia)

- Wizard: project folder page (default `~/Documents/nUpdate Projects/<name>`), project password when credentials are saved.
- Credentials dialog: two modes (project password, or individual secrets when nothing is saved).
- Project window: migration banner and prompt, "Migrate published packages…" in settings, legacy feed indicator with delete action on the overview, publish refused until migrated.
- Removed: export and import windows and view models.

### nUpdate.UI.WindowsForms / WPF

- `UpdaterUI.RunAsync`, canonical version strings, renamed texts, `AfterInstall`.

## 4. Phases

| Phase | Content | Verification |
|---|---|---|
| 0 | This plan | – |
| 1 | `nUpdate`: version, converters, serializer, operations, manifest, feed, installer options, signing, API renames, policy, logger, statistics client | `Tests/Library`, gate nUpdate 100/100 |
| 2 | Installer engine on the manifest and typed operations | `Tests/Installer`, gate 100/100 |
| 3 | Administration.Core: models, store, migrator, builder, signer, feed store, publish, statistics v2, legacy feed migrator, remove export | `Tests/Administration/Core`, gate 100/100 |
| 4 | Administration app: wizard, credentials, migration UI, overview, remove export/import | `Tests/Administration/App`, gate 90/90 |
| 5 | UI packages, PHP script, Docker scenarios (migration, portable project), end-to-end test | `Tests/Integration` |
| 6 | README, BUILDING, package README, CI | `dotnet format`, full run |
| 7 | pass-all-checks, self-review | all gates |

## 5. Migration flows

- **Project file**: `ProjectStore.LoadAsync` reads `format` (int) or `ConfigVersion` (string) and dispatches: 6 → load; `"v5"` → `ProjectMigrator.FromV5`; null/`1b2`/`3b2`/`v3` → `ProjectMigrator.FromV3`. Both legacy paths produce a format 6 project without `secrets`; the secrets they recovered are returned in memory and saved once the user sets a project password (prompted by the app). Keys are converted from XML to PEM. Local folders: v5 projects keep their folder (`<name>.nupdproj` is renamed to `project.nupdproj`, `<version>/` folders move to `packages/<version>/`), repacked by the legacy feed migrator's local half.
- **Server**: `MigrationStatus.Check` downloads `nupdate.json` (404 → missing) and `updates.json` (404 → missing). Legacy present and new missing → "needs migration". `LegacyFeedMigrator.RunAsync`: for every entry of the legacy feed, locate the zip (local `packages/<version>/` or `<version>/` folder, else download), read operations (`operations.json` in the zip or the legacy entry's `Operations`), convert to typed operations, write manifest, repack, sign (PSS), upload to `packages/<version>.zip`, build the entry (size, hash, touches), finally upload `nupdate.json` and `statistics.php` + config, rewrite local folders, set `project.MigratedAt`. Compensation deletes what was uploaded.
- **Publish guard**: `PublishService` refuses when `MigrationStatus` says "needs migration".

## 6. Client API rename table

| Old | New |
|---|---|
| `SearchForUpdatesAsync` | `CheckForUpdatesAsync` |
| `PackageConfigurations` / `UpdateConfiguration` | `AvailableUpdates` / `PackageInfo` (document `UpdateFeed`) |
| `UseDynamicUpdateUri` | removed |
| `HostApplicationOptions` | `AfterInstall { Restart, Close, KeepRunning }` |
| `UpdateArgument(Argument, ExecutionOptions)` | `InstallerArgument(Value, When)`, `ArgumentCondition { Succeeded, Failed, Always }` |
| `UpdateConfigurationFileUri` | `FeedUri` |
| `DownloadPackagesAsync` / `ValidatePackagesAsync` / `InstallPackage()` / `DeletePackages()` | `DownloadAsync` / `VerifyAsync` / `StartInstaller()` / `DeleteDownloads()` |
| `TotalSize` / `PackageFilePaths` / `UpdateDirectory` | `TotalDownloadSize` / `DownloadedPackages` / `DownloadDirectory` |
| `IncludeCurrentPcIntoStatistics` | `ReportDownloads` |
| `Conditions` | `RolloutConditions` |
| `CustomInstallerUiAssemblyPath` | `CustomUiAssemblyPath` |
| `LanguageCulture` / `LocalizationProperties` / `CultureFilePaths` | `Culture` / `Texts` / `TextFiles` |
| `IncludeAlpha` / `IncludeBeta` | `PreReleases` (`PreReleasePolicy`) + `AcceptedPreReleaseLabels` |
| `nUpdateVersionAttribute` | `ApplicationVersionAttribute` (`UpdateVersionAttribute` would clash with `UpdateVersion` in attribute context) |
| `RolloutCondition.IsNegativeCondition` / `RolloutConditionMode.AtLeastOne` | `Negated` / `Any` |
| `Architecture.Independent` | `Architecture.Any` |
| `StatisticsReportFailed` event | `UpdateManagerServices.Logger` |
| `UpdaterUI.ShowUserInterfaceAsync()` | `UpdaterUI.RunAsync()` |

## 7. Risks

- PSS on .NET Framework needs `RSACng`; `PackageSigning` picks it by `RuntimeInformation.FrameworkDescription`. Covered by a Windows-only test.
- Hand-written DER for RSA keys: round-trip tests against `RSA.ExportParameters` on .NET 10 and against OpenSSL-generated PEM vectors.
- The legacy feed migrator downloads zips over HTTP when the local copy is missing; large projects take a while, so it reports progress per package.

## 8. Follow-up: strict versions, nUpdate 4 and 5 side by side, migration assistant

Requested after the first review round: `UpdateVersion` no longer reads the old spellings, and the migration is an
interactive assistant that explains how to run both versions in parallel.

- **Strict `UpdateVersion`**: only the canonical form parses (`major.minor.patch`, a fourth number only when it is
  not 0, SemVer pre-release label and build metadata, no leading zeros). `LegacyVersion` in
  `Administration.Core.Migration` converts every spelling of nUpdate 3, 4 and the 5.0 pre-releases for the legacy feed
  and the old project files.
- **Side by side**: the statistics API v2 is uploaded as `nupdate-statistics.php` (+ `nupdate-statistics.config.php`)
  so it never replaces the `statistics.php` of nUpdate 4; converted projects use that default endpoint. The
  Administration keeps its list in `projects.json` and only reads the `projconf.json` of nUpdate Administration 4,
  which shares the data folder. The migration never deletes anything of nUpdate 3 and 4, locally or on the server.
- **Migrator API**: `PrepareAsync` builds a `MigrationPlan` (every legacy package with its new version, source, file
  count, converted operations, warnings, skipped zip entries or a problem; downloads kept in a temp folder until the
  plan is disposed), `RunAsync(plan)` migrates the included packages (or only starts an empty `nupdate.json`),
  `FindLegacyFilesAsync`/`DeleteLegacyFilesAsync(files)` retire the old setup after the user confirmed the list.
  `FeedChecker` checks the published feed like a client (size, hash, algorithm, signature, manifest, statistics API).
- **Assistant** (`MigrationViewModel`, `MigrationWindow`): What changes, Packages (selectable), Statistics (checks the
  settings, explains PHP/PATH_INFO needs), Migrate (summary, trust prompt), Side by side (feed check, client snippet,
  version mapping, bridge release through nUpdate Administration 4, keep the old files, retire). Opens by itself when
  only `updates.json` exists, and from the banner, the Overview tab and the settings.
- `UpdateProject.LegacyProjectFile` remembers the converted nUpdate 4 project file for the guide.
- Review round on the follow-up: old zips are only re-signed when they carry the nUpdate 4 signature from
  `updates.json` (RSA PKCS#1 v1.5, SHA-512; a local copy that does not match falls back to the download); duplicate
  spellings of one version, versions the project already has as its own package and unreadable entries are reported
  instead of migrated; a converted project never overwrites the old file and never lands in nUpdate Administration 4's
  data folder; retiring deletes only version folders holding this project's zip (never one containing the project) and
  removes `updates.json` last; the guide's version example is the release that moves the users over, and the client
  snippet no longer pins a version; the statistics script counts only registered versions; the assistant can read the
  server again, and its window cannot be closed while the migration runs.
