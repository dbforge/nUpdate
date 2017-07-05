![](http://www.nupdate.net/titlelogo.png)

nUpdate is a modern update system for .NET applications.
Providing high security and an easy-to-use user interface, nUpdate is the perfect solution for your software.

nUpdate is in a very advanced state. Its development began in autumn 2013 and the project has grown up until now. It contains a lot of features and provides you with everything you'll need to manage your updates.

Also nUpdate is designed to fit in with all operating systems since Windows Vista and its components can be used in a very flexible way. It even allows you to use a custom UI in the client. This is the result of the usage of interfaces, service providers and event-based or task-based aynchronous patterns. nUpdate will also take care of your clients and use intelligent, logical processes that will prevent your users from being stressed out by having to do a lot of things manually and repeatedly. The best example for such a case would be installing every single update on its own. Instead, nUpdate connects all updates, installs them in one go and does still take care of the version order to avoid any conflicts between versions and make sure that the data is installed in exactly the way you wanted.

A small overview over some of the most important features of nUpdate:

- Easy management of update packages
- Easy to use
- Automated updating for all clients
- Many configuration settings which let you control your updates easily
- Secured update packages by signing with SHA512 and 8192 Bit RSA-keys
- Smart error handling with automatically resetting data if an operation failed
- Interfaces, service providers and event-based/task-based asynchronous patterns provide the possibility to use own Graphical User Interfaces that can be shown to the client
- Smart statistics about the downloads of your updates
- Supports very large update packages
 
### Notes
Help and tips are appreciated. Current Beta-version could contain some problems and bugs which will be fixed.

### Website
* [nUpdate] - The official website of the nUpdate-project.
* [@nUpdateLib] - The official twitter account of the nUpdate-project.

### Installation

There is currently no real installation, just download the whole solution and build it. If you are using .NET 4.0 and use the binaries located in "Provide TAP" you will have to install Microsoft.Bcl (Microsoft Async) in order to use the library as it implements async-await for the TAP. With .NET 4.5 you won't have to install anything, neither in "No TAP", nor in "Providing TAP". Make sure to start the Administration with admin privileges the first time, so that the registry key for the extension can be created.
A documentation on how to use nUpdate's components and how to customize it will follow soon.

### Development

Want to contribute as said above? Great!
Just use GitHub's functions for creating issues on problems so that I could have a look at them and decide what to do etc. If you already have an idea on what to do, make sure to fork nUpdate and create a PullRequest.

### Roadmap
I'm not happy with everything, yet. It's possible that you find some small things that need to be improved. Nevermind, everything is in work!

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

### License
MIT

[nUpdate]:http://www.nupdate.net/
[@nUpdateLib]:http://twitter.com/nUpdateLib

### Screenshots

![](https://www.nupdate.net/img/new-updates.png)
![](https://www.nupdate.net/img/updates-download.png)
