# nUpdate - The easy update-solution

nUpdate is a modern update system providing high security and an easy-to-use interface.

- Easy management of update packages
- Easy usable library with a lot of possibilities
- Automated updating for all clients
- Many configuration settings which let you control your updates easily
- Secured update packages by signing with SHA512 and 8912 Bit RSA-keys
- Smart error handling with automated resetting data if an operation failed
- Interfaces, TAP and EAP provide the possibility to use custom Graphic User Interfaces that can be shown to the client
- Many operations that let you access the registry, file system, processes and services during the installation of an update
- Excellent integration into your applications
- Well designed integrated user interfaces

### Version
2.0.0.0 Beta Build 1 (Could contain bugs)

### Notes
Help and tips are appreciated. Current Beta-version could contain some problems and bugs which will be fixed.

### Website
* [nUpdate] - The official website of the nUpdate-project.
* [@nUpdateLib] - The official twitter account of the nUpdate-project.

### Installation

Download the whole solution and build it. If you are using .NET 4.0 and use the binaries located in "Provide TAP" you will have to install Microsoft.Bcl (Microsoft Async) in order to use the library as it implements async-await for the TAP. With .NET 4.5 you won't have to install anything, neither in "No TAP", nor in "Providing TAP". Make sure to start the Administration with admin privileges the first time, so that the registry key for the extension can be created.
A documentation on how to use nUpdate's components and how to customize it will follow soon.

### Development

Want to contribute as said above? Great!
Just use GitHub's functions for creating issues on problems so that I could have a look at them and decide what to do etc. If you already have an idea on what to do, make sure to fork nUpdate and create a PullRequest.

### Todo's
- Differential updates
- Delta Patching

###License
SugarCRM


[nUpdate]:http://www.nupdate.net/
[@nUpdateLib]:http://twitter.com/nUpdateLib
