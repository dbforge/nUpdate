![nUpdate Logo](https://www.nupdate.net/nupdate_header.png)

# nUpdate - .NET Update Solution

[![Release](https://img.shields.io/badge/release-v4.0-blue.svg)](https://github.com/dbforge/nUpdate/releases)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/ProgTrade/nUpdate/master/LICENSE)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=dominic%2ebeger%40hotmail%2ede&lc=DE&item_name=nUpdate&no_note=0&currency_code=EUR&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHostedGuest)

nUpdate is a powerful update system for .NET applications.
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

## Specs and requirements

The pure `nUpdate` library is based on .NET standard 2.0.
This means your project needs to fulfill one of these requirements:

- .NET Framework >= 4.6.1
- .NET Core >= 2.0
- UWP >= 10.0.16299

All other libraries in the solution use the .NET Framework 4.6.1.

## Installation

You can get the necessary libraries and applications from the current [releases](https://github.com/dbforge/nUpdate/releases). The nUpdate library itself can also be installed in your application using the NuGet package manager.

### Using NuGet

```
PM> Install-Package nUpdate -Version 4.0.0
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

The package `nUpdate.UI.WindowsForms` provides a built-in user interface for Windows Forms application. Install it via NuGet:

```
PM> Install-Package nUpdate.UI.WindowsForms -Version 1.0.0
```

To use the built-in WPF user interface, you need to install `nUpdate.UI.WPF`:

```
PM> Install-Package nUpdate.UI.WPF -Version 1.0.0
```

Both projects give you an `UpdaterUI` class that you can use as follows:

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

For asynchronous usage, each method (except `InstallPackage` and `ValidatePackages` which are synchronous in any case till now) has a corresponding asynchronous one defined inside the `UpdateManager`.

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

## Supported by

<img src="https://www.nupdate.net/jetbrains.png" width="96" height="96"/>
