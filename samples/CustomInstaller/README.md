# CustomInstaller

A sample of an update installer of your own: a WPF window on top of the `nUpdate.UpdateInstaller` base library,
which does all the work (reading the options, waiting for the application, copying files, running operations,
restarting, writing `install.log`, the windowless mode).

The whole program is one line:

```csharp
[STAThread]
static int Main(string[] args) => InstallerHost.Run(args, session => new WpfProgressReporter(session));
```

`WpfProgressReporter` derives from `nUpdate.UpdateInstaller.WindowProgressReporter`, which takes care of the threads:
the engine reports from its own thread, and the window only sees calls on its UI thread, once it is open.

- `RunWindow` shows the window and returns once it has closed. It runs on the main thread.
- `Post` hands an action to the UI thread.
- `ShowProgress` shows the progress and what the installer is doing.
- `AskAboutLockedFile` asks what to do with a file that another process holds open.
- `ShowError` shows an error and calls back once the user has read it.
- `Finish` closes the window.

To use it, publish it into a folder of its own that you ship with your application and point the client at it:

```csharp
manager.InstallerPath = Path.Combine(AppContext.BaseDirectory, "MyInstaller", "CustomInstaller.exe");
```

nUpdate copies that folder to the temp folder and starts the installer from there, so it can replace the application's
files. A framework-dependent build needs the .NET 8 Desktop Runtime on the machine; publish it self-contained to ship
it without.
