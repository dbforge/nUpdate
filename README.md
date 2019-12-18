![nUpdate Logo](https://www.nupdate.net/nupdate_header.png)

# nUpdate - .NET Update Solution

[![Release](https://img.shields.io/badge/release-v3.5.0-blue.svg)](https://github.com/dbforge/nUpdate/releases)
[![NuGet](https://img.shields.io/badge/nuget%20nUpdate.ProvideTAP-v3.5.0-red.svg)](https://www.nuget.org/packages/nUpdate.ProvideTAP/)
[![NuGet](https://img.shields.io/badge/nuget%20nUpdate.WithoutTAP-v3.5.0-red.svg)](https://www.nuget.org/packages/nUpdate.WithoutTAP/)  
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/ProgTrade/nUpdate/master/LICENSE)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=dominic%2ebeger%40hotmail%2ede&lc=DE&item_name=nUpdate&no_note=0&currency_code=EUR&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHostedGuest)

nUpdate is a modern update system for .NET applications.
Providing high security and an easy-to-use user interface, it is the perfect solution for your software.

![](https://www.nupdate.net/img/new-updates.png)
![](https://www.nupdate.net/img/updates-download.png)

## Features

- Remote update package control and management
- Many configuration settings
- Operations let you access the file system, the registry and add the possiblity to start or stop processes and services, when installing an update. You can even execute a small C#-script.
- Automated updating for all clients
- Secured update packages by signing with SHA512 and 8192 Bit RSA-keys
- Built-in user interface or a custom user interface can be used
- Interfaces, service providers and the Event-based/Task-based asynchronous pattern add a lot of flexiblity
- Smart statistics about the downloads of your published updates
- Supports very large update packages
- ...

## Installation

You can get the necessary libraries and applications from the current [releases](https://github.com/ProgTrade/nUpdate/releases). The nUpdate library itself can also be installed in your application using the NuGet package manager.

### Using NuGet

#### nUpdate.ProvideTAP

If you want to use nUpdate with the Taskbased Asynchronous Pattern including `async` and `await`, then install this package:

```
PM> Install-Package nUpdate.ProvideTAP -Version 3.5.0
```

##### Trouble installing nUpdate.ProvideTAP?

It may be that Visual Studio shows you a warning like `The primary reference "nUpdate.ProvideTAP, Version=..., Culture=neutral, PublicKeyToken=..., processorArchitecture=MSIL" could not be resolved because it has an indirect dependency on the .NET Framework assembly "System.Runtime, Version=..., Culture=neutral, PublicKeyToken=..." which has a higher version "..." than the version "..." in the current target framework.` and won't let you compile the project. You can fix this by going to the NuGet Package Manager and updating the Microsoft.Bcl.\* packages to the newest version.

#### nUpdate.WithoutTAP

Otherwise, if you want to use the Eventbased Asynchronous Pattern, make sure to install this package:

```
PM> Install-Package nUpdate.WithoutTAP -Version 3.5.0
```

## Usage example

### Simple integration

Specify the current client version inside Properties > AssemblyInfo.cs

``` c#
[assembly: nUpdateVersion("1.0.0.0")]
```

or inside the constructor of the `UpdateManager` class (is `null` by default):

``` c#
var manager = new UpdateManager(
    new Uri(...), "PublicKey", CultureInfo.Foo, new UpdateVersion("1.0.0.0"));
```

**Note**: One of the both needs to be specified. If both are specified, the version specified in the constructor parameter will be used.
When none of them is specified, an exception will be thrown.

### Using the integrated user interface (Windows Forms or WPF)

The projects `nUpdate.ProvideTAP` and `nUpdate.WithoutTAP` contain Windows Forms only. To use a WPF user interface, you need to reference `nUpdate.WPFUserInterface` (not yet available in NuGet). The codes for the usage are completely equal (except for having the WPF UI in nUpdate.UpdateInstaller as well, you'll have to provide the integrated `nUpdate.WPFUpdateInstaller`-DLL location using the `CustomUiAssemblyPath`-property).

``` c#
// Generated via nUpdate Administration > Open project > Overview > Copy source
var manager = new UpdateManager(
    new Uri(...), ...);

var updaterUI = new UpdaterUI(manager, SynchronizationContext.Current);
updaterUI.ShowUserInterface();
```

### Using no user interface (synchronously)

``` c#
// Generated via nUpdate Administration > Open project > Overview > Copy source
var manager = new UpdateManager(
    new Uri(...), ...);
    
if (manager.SearchForUpdates())
{
    manager.DownloadPackages();
    if (manager.ValidatePackages())
        manager.InstallPackage();
    return;
}
```

### Using no user interface (asynchronously)

For asynchronous usage, each method (except `InstallPackage` and `ValidatePackages` which are synchronous in any case till now) has a corresponding asynchronous one defined inside the `UpdateManager`. Depending on your reference, `nUpdate.ProvideTAP` uses the task-based asynchronous pattern with `async`, `await` and `Task<T>` whereas `nUpdate.WithoutTAP` uses the event-based asynchronous pattern (obsolete). You may have a look at each `UpdaterUI` implementation to see how it is used.

### Specifying an `UpdateVersion`

nUpdate uses a custom version implementation to describe application versions. Semantic versioning is not fully supported among the exact specification but an implementation for it is already included in v4.

You may specify versions as known with the ordinary parts `Major.Minor.Revision.Build`, e.g. `new UpdateVersion("1.2.0.0")`.
Pre-Releases may be noted by appending the leading, lowered letters of developmental stages and a development build. For example, 1.2 Beta 1 is noted as `1.2.0.0b1` or `1.2b1`, 1.3.1 Alpha 3 is noted as `1.3.1.0a3` or `1.3.1a3`. This means that trailing zero subversions may be ignored and have no need to be declared. The last subversion and the developmental stage may be separated by a dash: `1.2-a1`.

There are three developmental stages to this point:

Alpha: `a`,
Beta: `b`,
ReleaseCandidate: `rc`

**Note**: These need to be specified by using the abbrevations above. `1.2.3alpha1` is not valid in v3 of nUpdate.

In version 4 any type of pre-release descriptor, as well as build metadata, will be available. It will fulfill the complete semantic versioning specification and have an abstract version system.

## Web
* [nUpdate] - The official website of nUpdate.

## Roadmap

- .NET Standard and .NET Core port
- WPF port
- Differential updates
- Faster updating
- Fix remaining bugs
- Improvements
- Multilanguage everywhere
- Lots of new languages
- Code Signing Certificate
- PowerShell support
- Better integration in company networks
- ...

Have a look at the [develop]-branch for the newest changes.

[develop]:https://www.github.com/ProgTrade/nUpdate/tree/develop
[nUpdate]:http://www.nupdate.net/

## Supported by

<img src="https://www.nupdate.net/jetbrains.png" width="96" height="96"/>
