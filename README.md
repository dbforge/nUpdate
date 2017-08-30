![nUpdate Logo](https://www.nupdate.net/nupdate_header.png)

# nUpdate - .NET Update Solution

[![Release](https://img.shields.io/badge/release-v3.1.2-blue.svg)](https://github.com/ProgTrade/nUpdate/releases)
[![NuGet](https://img.shields.io/badge/nuget%20nUpdate.ProvideTAP-v3.1.2-red.svg)](https://www.nuget.org/packages/nUpdate.ProvideTAP/)
[![NuGet](https://img.shields.io/badge/nuget%20nUpdate.WithoutTAP-v3.1.2-red.svg)](https://www.nuget.org/packages/nUpdate.WithoutTAP/)  
[![Issues](https://img.shields.io/github/issues/ProgTrade/nUpdate.svg)](https://github.com/ProgTrade/nUpdate/issues)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/ProgTrade/nUpdate/master/LICENSE)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=dominic%2ebeger%40hotmail%2ede&lc=DE&item_name=nUpdate&no_note=0&currency_code=EUR&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHostedGuest)

nUpdate is a modern update system for .NET applications.
Providing high security and an easy-to-use user interface, it is the perfect solution for your software.

nUpdate is in a very advanced state. Its development began in autumn 2013 and the project has grown up until now. It contains a lot of features and provides you with everything you'll need to manage your updates.

Also, nUpdate is designed to fit in with all operating systems since Windows Vista and its components can be used in a very flexible way. It offers you the possibility to completely customize the functionality and user interfaces. This flexiblity is based on interfaces, service providers and the Event-based or Task-based aynchronous pattern. nUpdate will also take care of your clients by using intelligent algorithms that will prevent your users from being stressed out by having to do a lot of work manually and repeatedly, such as installing every single update on its own. Instead, nUpdate connects all updates, installs them in one go and does still take care of the version order to avoid any conflicts between the packages and makes sure that the data is installed in exactly the way you wanted.

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

#### Using NuGet

If you want to use nUpdate with the Taskbased Asynchronous Pattern including `async` and `await`, then install this package:

```
PM> Install-Package nUpdate.ProvideTAP -Version 3.1.2 
```

Otherwise, if you want to use the Eventbased Asynchronous Pattern only, make sure to install this package:

```
PM> Install-Package nUpdate.WithoutTAP -Version 3.1.2
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

## Screenshots

![](https://www.nupdate.net/img/new-updates.png)
![](https://www.nupdate.net/img/updates-download.png)
