![](http://www.nupdate.net/titlelogo.png)

What is nUpdate?
------
nUpdate is a modern update system for .NET applications.
Providing high security and an easy-to-use user interface, nUpdate is the perfect solution for your software.

nUpdate is in a very advanced state. Its development began in autumn 2013 and the project has grown up until now. It contains a lot of features and provides you with everything you'll need to manage your updates.

Also, nUpdate is designed to fit in with all operating systems since Windows Vista and its components can be used in a very flexible way. It offers you the possibility to completely customize the functionality and user interfaces. This flexiblity is based on interfaces, service providers and the Event-based or Task-based aynchronous pattern. nUpdate will also take care of your clients by using intelligent algorithms that will prevent your users from being stressed out by having to do a lot of work manually and repeatedly, such as installing every single update on its own. Instead, nUpdate connects all updates, installs them in one go and does still take care of the version order to avoid any conflicts between the packages and makes sure that the data is installed in exactly the way you wanted.

Features
------

- Remote update package control and management
- Many configuration settings
- Operations let you access the file system (for deleting and renaming files), the registry and add the possiblity to start or stop processes and services, when installing an update. You can even execute a small C#-script.
- Automated updating for all clients
- Secured update packages by signing with SHA512 and 8192 Bit RSA-keys
- Interfaces, service providers and the Event-based/Task-based asynchronous pattern add a lot of flexiblity
- Smart statistics about the downloads of your published updates
- Supports very large update packages
- ...
 
Version
------
v3.0-beta4 (Could contain some bugs)

Web
------
* [nUpdate] - The official website of the nUpdate-project.
* [@nUpdateLib] - The official twitter account of the nUpdate-project.

Installation
------

There is currently no real installation, just download the whole solution and build it. If you are using .NET 4.0 and use the binaries located in "Provide TAP", you will have to install Microsoft.Bcl (Microsoft Async) in order to use the library as it implements async-await for the TAP. With .NET 4.5, you won't have to install anything, neither in "No TAP", nor in "Providing TAP". Make sure to start nUpdate Administration with admin privileges the first time, so that the registry key for the extension can be created.
A documentation, on how to use nUpdate's components and how to customize it, will follow soon.

Roadmap
------

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

Have a look at the [v4.0]-branch for the newest changes.

License
------
SugarCRM

[v4.0]:https://www.github.com/ProgTrade/nUpdate/tree/v4.0
[nUpdate]:http://www.nupdate.net/
[@nUpdateLib]:http://twitter.com/nUpdateLib

Screenshots
------

![](https://www.nupdate.net/img/new-updates.png)
![](https://www.nupdate.net/img/updates-download.png)
