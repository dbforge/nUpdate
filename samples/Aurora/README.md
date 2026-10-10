# Aurora

A small Avalonia application for trying nUpdate on your own web and FTP server: it checks a feed with the dialogs of
`nUpdate.UI.Avalonia`, downloads and verifies what it finds and starts the built-in installer, which replaces Aurora
and starts the new version. The window shows the version Aurora runs as, so you see the update happen.

Aurora references the nUpdate projects of this repository, so whatever you change in the library, the dialogs or the
installer is what you test. Your own application references the packages instead
(`nUpdate`, `nUpdate.UI.Avalonia`, `nUpdate.UpdateInstaller.UI.Avalonia`).

## How it is built

`publish.sh <version> [runtime identifier]` publishes Aurora as that version, self-contained and as a single file, and
publishes the installer for the same runtime identifier into `nUpdate.Installer/<rid>/` next to it, where
`UpdateManager` looks for it. The runtime identifier defaults to the one of your machine.

- **macOS:** `artifacts/<version>/Aurora.app`, a bundle signed ad hoc. Every file in `Contents/MacOS` is Mach-O code
  with its signature embedded, so the bundle still verifies after nUpdate replaced it.
- **Linux and Windows** (run the script in Git Bash): the folder `artifacts/<version>/Aurora`.

The version comes from `-p:Version`, which `Aurora.csproj` turns into the `[assembly: ApplicationVersion]` that
`UpdateManager` reads. `artifacts/` is not committed.

## Trying an update on a Mac

1. **Create the project.** Start nUpdate Administration (`dotnet run --project nUpdate.Administration`) and create a
   project called Aurora:
   - Update URL: the folder your web server serves for the updates, for example `http://localhost/aurora/`.
   - Transfer: your FTP server, with the directory that is this same folder seen from FTP. Test the connection.

2. **Install Aurora 1.0.0.** Publish it and copy it to where it should live. The installer runs as you, so you need
   write access to the folder that contains the bundle; `~/Applications` is fine. Copy it with `ditto`, which keeps
   the signature:

   ```bash
   samples/Aurora/publish.sh 1.0.0
   mkdir -p ~/Applications
   ditto samples/Aurora/artifacts/1.0.0/Aurora.app ~/Applications/Aurora.app
   open ~/Applications/Aurora.app
   ```

3. **Point Aurora to the project.** Enter the feed URL (the update URL followed by `nupdate.json`, for example
   `http://localhost/aurora/nupdate.json`) and paste the public key (Administration, Overview, Copy public key).
   Aurora keeps both in `~/.config/Aurora/settings.json`, outside the bundle, so every version reads them.

4. **Publish Aurora 1.1.0.**

   ```bash
   samples/Aurora/publish.sh 1.1.0
   ```

   In the Administration, create a package 1.1.0:
   - General: add the platform macOS Apple silicon (osx-arm64) and remove "Any platform". An Intel Mac needs
     osx-x64 and `publish.sh 1.1.0 osx-x64`.
   - Changelog: a line in English.
   - Files: into Program, without a sub folder, Add folder… and choose `samples/Aurora/artifacts/1.1.0/Aurora.app`.
     On a macOS platform the bundle becomes Program itself.
   - Save and publish.

5. **Update.** In Aurora 1.0.0, click Check for updates and install. The installer shows its progress, replaces the
   bundle and starts Aurora again, which now shows 1.1.0. `codesign --verify --deep --strict ~/Applications/Aurora.app`
   confirms that the replaced bundle is intact.

Things to try next:

- **Edit the published package:** open 1.1.0 in the Administration and change a file or add an operation; saving
  builds the macOS package again under a new name and switches the feed. Start Aurora 1.0.0 again
  (`ditto` the 1.0.0 bundle back) to install the rebuilt package.
- **Pre-releases:** publish `1.2.0-beta.1` and set "Least stable version to install" to Beta.
- **Operations:** add a Delete files or a Start a process operation; the Placeholders box next to it shows where
  `%program%` (on macOS the whole `Aurora.app`), `%appdata%`, `%temp%` and `%desktop%` point.

## On Linux and Windows

`publish.sh` publishes the folder `artifacts/<version>/Aurora`, containing `Aurora` (`Aurora.exe`) and
`nUpdate.Installer`. Copy the folder of version 1.0.0 to where Aurora should live, start it from there, and create the
package for linux-x64 or win-x64. In its Files section, Add files… the executable of version 1.1.0 into Program, and Add
folder… its `nUpdate.Installer` folder.

## When something goes wrong

- The Diagnostics section shows the executable Aurora runs from, the installer it would start and whether that
  exists, the platform and the settings file.
- The installer writes `install.log` into `<temp>/nUpdate Installer/Aurora`; on a Mac `<temp>` is `$TMPDIR`
  (`open "$TMPDIR/nUpdate Installer/Aurora"`).
- In the Administration, Overview, Check shows whether the feed is reachable at the update URL.
