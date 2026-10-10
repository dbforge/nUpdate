using nUpdate.Installer;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller;
using nUpdate.UpdateInstaller.Operations;

namespace nUpdate.Tests.Installer;

public class OperationContextTests
{
    private readonly TestInstallerServices _services = new();

    [Fact]
    public void OperationContext_ValidatesArgumentsAndFormatsTexts()
    {
        var options = _services.Options("/p/1.0.0.0.zip");
        options.Texts[nameof(InstallerText.ProcessStart)] = "Starte {0}";
        var resolver =
            new PathPlaceholderResolver(_services.FileSystem, _services.AppDirectory, _services.SpecialFolders);
        var tracker = new ProgressTracker();
        var context = new OperationContext(options, _services.Services, resolver, tracker, _services.Reporter);
        context.Options.ShouldBe(options);
        context.Services.ShouldBe(_services.Services);
        context.Paths.ShouldBe(resolver);
        context.Progress.ShouldBe(tracker);
        context.Reporter.ShouldBe(_services.Reporter);
        context.Report(InstallerText.ProcessStart, "x");
        _services.Reporter.Operations.Single().Text.ShouldBe("Starte x");

        Should.Throw<ArgumentNullException>(() =>
            new OperationContext(null!, _services.Services, resolver, tracker, _services.Reporter));
        Should.Throw<ArgumentNullException>(() =>
            new OperationContext(options, null!, resolver, tracker, _services.Reporter));
        Should.Throw<ArgumentNullException>(() =>
            new OperationContext(options, _services.Services, null!, tracker, _services.Reporter));
        Should.Throw<ArgumentNullException>(() =>
            new OperationContext(options, _services.Services, resolver, null!, _services.Reporter));
        Should.Throw<ArgumentNullException>(() =>
            new OperationContext(options, _services.Services, resolver, tracker, null!));
    }
}
