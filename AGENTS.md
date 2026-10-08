# Notes for coding agents

[BUILDING.md](BUILDING.md) has the full build, test, coverage and packaging instructions. These are the pitfalls that
cost the most time:

- **.NET SDK:** when the SDK is installed in `~/.dotnet`, run `export DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$PATH`
  before any `dotnet` command.
- **Never pass `-nologo` to `dotnet test`.** The tests run on Microsoft.Testing.Platform, which forwards the flag to the
  test host, and the host then runs zero tests. Runner options go after `--`, for example
  `dotnet test nUpdate.Tests/nUpdate.Tests.csproj -- --filter-class nUpdate.Tests.Library.UpdateManagerTests`.
- **Coverage gates are checked in Release.** CI gates a Release build, and Debug coverage misses some compiler-generated
  branches. Before claiming the gates pass, build and test with `--configuration Release` and run the CoverageGate
  command from BUILDING.md on that report.
- **No hidden branches in the 100% assemblies.** `"text" + exception` compiles to
  `string.Concat("text", exception?.ToString())`, a null branch no test reaches. Write `"text" + exception.ToString()`.
- **`.gitignore` hides folders named `Packages`.** The Visual Studio rule `**/[Pp]ackages/*` once kept a source folder
  out of the repository unnoticed. Before pushing new folders, check `git status --short --ignored` or build a fresh
  clone of the branch.
