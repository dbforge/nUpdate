# nUpdate

nUpdate is an update system for .NET applications: a client library that searches, downloads and verifies signed
update packages, an installer for Windows, Linux and macOS that applies them, and a cross-platform administration app
that builds, signs and publishes packages over FTP, FTPS or SFTP.

- `nUpdate` - the client library (`UpdateManager`, `UpdateFlow`) for .NET Standard 2.0.
- `nUpdate.UpdateInstaller.UI.Avalonia` - the built-in installer, self-contained for seven runtime identifiers; copies
  the one of your application to `nUpdate.Installer/<rid>/` on build (set `nUpdateInstallerRuntimes` to choose).
- `nUpdate.UpdateInstaller` - the base library for an installer of your own (`InstallerHost`).
- `nUpdate.UI.WindowsForms` / `nUpdate.UI.WPF` / `nUpdate.UI.Avalonia` - built-in update dialogs.
- `nUpdate.Administration.TransferInterface` - the contract for transfer plugins of nUpdate Administration.

```csharp
var manager = new UpdateManager(new Uri("https://example.com/updates/nupdate.json"), publicKey,
    CultureInfo.CurrentUICulture, new UpdateVersion("1.0.0"));
var updaterUI = new UpdaterUI(manager, this);
await updaterUI.RunAsync();
```

Documentation, the 5.0 migration notes and the source are at https://github.com/dbforge/nUpdate.
