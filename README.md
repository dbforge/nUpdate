![nUpdate Logo](https://www.nupdate.net/nupdate_header.png)

# nUpdate - .NET Update Solution

[![Release](https://img.shields.io/badge/release-v3.3.1-blue.svg)](https://github.com/ProgTrade/nUpdate/releases)
[![NuGet](https://img.shields.io/badge/nuget%20nUpdate.ProvideTAP-v3.3.1-red.svg)](https://www.nuget.org/packages/nUpdate.ProvideTAP/)
[![NuGet](https://img.shields.io/badge/nuget%20nUpdate.WithoutTAP-v3.3.1-red.svg)](https://www.nuget.org/packages/nUpdate.WithoutTAP/)  
[![Issues](https://img.shields.io/github/issues/ProgTrade/nUpdate.svg)](https://github.com/ProgTrade/nUpdate/issues)
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
PM> Install-Package nUpdate.ProvideTAP -Version 3.3.1
```

##### Trouble installing nUpdate.ProvideTAP?

It may be that Visual Studio shows you a warning like `The primary reference "nUpdate.ProvideTAP, Version=..., Culture=neutral, PublicKeyToken=..., processorArchitecture=MSIL" could not be resolved because it has an indirect dependency on the .NET Framework assembly "System.Runtime, Version=..., Culture=neutral, PublicKeyToken=..." which has a higher version "..." than the version "..." in the current target framework.` and won't let you compile the project. You can fix this by going to the NuGet Package Manager and updating the Microsoft.Bcl.\* packages to the newest version.

#### nUpdate.WithoutTAP

Otherwise, if you want to use the Eventbased Asynchronous Pattern, make sure to install this package:

```
PM> Install-Package nUpdate.WithoutTAP -Version 3.3.1
```

## Web
* [nUpdate] - The official website of nUpdate.
* [@nUpdateLib] - The official twitter account of nUpdate.

## Roadmap

- Differential updates
- Delta Patching
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
[@nUpdateLib]:http://twitter.com/nUpdateLib

## Supported by

<img src="https://www.nupdate.net/jetbrains.png" width="96" height="96"/>
