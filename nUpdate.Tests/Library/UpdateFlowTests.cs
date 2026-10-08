using System.IO.Abstractions.TestingHelpers;
using System.Net;
using nUpdate.Tests.Library.Support;
using nUpdate.Tests.Support;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public sealed class UpdateFlowTests : IDisposable
{
    private const string FeedUri = "https://h/u/nupdate.json";
    private readonly TestServices _services = new();
    private readonly IUpdateFlowPresenter _presenter = Substitute.For<IUpdateFlowPresenter>();
    private readonly UpdateManager _manager;
    private readonly UpdateFlow _flow;

    public UpdateFlowTests()
    {
        _manager = new UpdateManager(new Uri(FeedUri), TestKeys.PublicKey, services: _services.Build());
        _flow = new UpdateFlow(_manager, _presenter, _services.FileSystem);
        var root = _services.FileSystem.Path.GetPathRoot(_services.FileSystem.Path.GetTempPath())!;
        _services.FileSystem.AddDrive(root, new MockDriveData { AvailableFreeSpace = 10_000 });

        // By default the presenter runs the search and the download straight away and the user accepts the install.
        _presenter.RunSearchAsync(Arg.Any<Func<CancellationToken, Task<bool>>>()).Returns(call => call.Arg<Func<CancellationToken, Task<bool>>>()(CancellationToken.None));
        _presenter.RunDownloadAsync(Arg.Any<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>())
            .Returns(call => call.Arg<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>()(new Progress<UpdateDownloadProgress>(), CancellationToken.None));
        _presenter.ConfirmInstallAsync().Returns(true);
    }

    public void Dispose() => _manager.Dispose();

    private static byte[] Package(int seed, int size = 300)
    {
        var data = new byte[size];
        new Random(seed).NextBytes(data);
        return data;
    }

    private void Publish(int size = 300, bool tampered = false, bool brokenDownload = false)
    {
        var package = TestPackages.Build("1.1.0", Guid.Empty, Package(1, size));
        var served = tampered ? TestPackages.Build("1.1.0", Guid.Empty, Package(2, size)) : package;
        var uri = "https://h/u/packages/1.1.0/any.zip";
        _services.Http.Bytes(uri, served);
        if (brokenDownload)
            _services.Http.On(HttpMethod.Get, uri, _ => new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var info = new PackageInfo
        {
            Version = new UpdateVersion("1.1.0"),
            Files = [new PackageFile { Path = "packages/1.1.0/any.zip", Size = served.Length, Sha512 = TestKeys.Sha512(served), Signature = new PackageSignature { Value = TestKeys.Sign(package) } }],
        };
        _services.Http.Text(HttpMethod.Get, FeedUri, Serializer.Serialize(new UpdateFeed { Packages = [info] }));
    }

    private void ServeEmptyFeed() => _services.Http.Text(HttpMethod.Get, FeedUri, Serializer.Serialize(new UpdateFeed()));

    private UpdateErrorMessage ShownError() =>
        (UpdateErrorMessage)_presenter.ReceivedCalls().Single(c => c.GetMethodInfo().Name == nameof(IUpdateFlowPresenter.ShowErrorAsync)).GetArguments()[0]!;

    [Fact]
    public void Constructor_ValidatesArguments()
    {
        Should.Throw<ArgumentNullException>(() => new UpdateFlow(null!, _presenter));
        Should.Throw<ArgumentNullException>(() => new UpdateFlow(_manager, null!));
        new UpdateFlow(_manager, _presenter).IsRunning.ShouldBeFalse();
    }

    [Fact]
    public async Task Run_NoUpdates_AreReportedOnlyForVisibleSearches()
    {
        ServeEmptyFeed();
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.NoUpdates);
        await _presenter.Received(1).ShowNoUpdatesAsync();

        _presenter.ClearReceivedCalls();
        _flow.UseHiddenSearch = true;
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.NoUpdates);
        await _presenter.DidNotReceive().ShowNoUpdatesAsync();
        await _presenter.DidNotReceive().RunSearchAsync(Arg.Any<Func<CancellationToken, Task<bool>>>());
    }

    [Fact]
    public async Task Run_SearchCancelledByDialogOrCaller_IsReportedAsCancelled()
    {
        Publish();
        _presenter.RunSearchAsync(Arg.Any<Func<CancellationToken, Task<bool>>>()).Returns(call =>
        {
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            return call.Arg<Func<CancellationToken, Task<bool>>>()(cancellation.Token);
        });
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.Cancelled);

        _flow.UseHiddenSearch = true;
        using var outer = new CancellationTokenSource();
        outer.Cancel();
        (await _flow.RunAsync(outer.Token)).ShouldBe(UpdateFlowResult.Cancelled);
        await _presenter.DidNotReceive().ShowErrorAsync(Arg.Any<UpdateErrorMessage>(), Arg.Any<Exception?>());
    }

    [Fact]
    public async Task Run_SearchFailure_IsShownAsSearchError()
    {
        _services.Http.On(HttpMethod.Get, FeedUri, _ => new HttpResponseMessage(HttpStatusCode.InternalServerError));
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.Failed);
        ShownError().Caption.ShouldBe(_manager.Texts.SearchError);
        await _presenter.Received().ShowErrorAsync(Arg.Any<UpdateErrorMessage>(), Arg.Is<Exception?>(e => e is HttpRequestException));
    }

    [Fact]
    public async Task Run_SearchTimeout_IsAnErrorNotACancellation()
    {
        _services.Http.On(HttpMethod.Get, FeedUri, (_, _) => throw new TaskCanceledException("timed out"));
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.Failed);
        ShownError().Caption.ShouldBe(_manager.Texts.SearchError);
    }

    [Fact]
    public async Task Run_DiskSpaceCheck_UsesTheManagersFileSystemByDefault()
    {
        Publish(size: 6_000);
        (await new UpdateFlow(_manager, _presenter).RunAsync()).ShouldBe(UpdateFlowResult.InsufficientDiskSpace);
    }

    [Fact]
    public async Task Run_Declined_StopsBeforeTheDownload()
    {
        Publish();
        _presenter.ConfirmInstallAsync().Returns(false);
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.Declined);
        await _presenter.DidNotReceive().RunDownloadAsync(Arg.Any<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>());
    }

    [Fact]
    public async Task Run_InsufficientDiskSpace_IsReportedBeforeTheDownload()
    {
        Publish(size: 6_000);
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.InsufficientDiskSpace);
        var error = ShownError();
        error.Caption.ShouldBe(_manager.Texts.NotEnoughDiskSpaceTitle);
        error.Text.ShouldContain(nUpdate.Ui.ByteSizeFormatter.Format(_manager.TotalDownloadSize * 2 - 10_000, System.Globalization.CultureInfo.CurrentCulture));
        await _presenter.Received().ShowErrorAsync(Arg.Any<UpdateErrorMessage>(), null);
        await _presenter.DidNotReceive().RunDownloadAsync(Arg.Any<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>());
    }

    [Fact]
    public async Task Run_Download_ForwardsProgressAndHonoursCancellation()
    {
        Publish();
        var reports = new List<UpdateDownloadProgress>();
        _presenter.RunDownloadAsync(Arg.Any<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>())
            .Returns(call => call.Arg<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>()(new SynchronousProgress(reports), CancellationToken.None));
        _services.AddInstaller();
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.InstallerStarted);
        reports.Last().Percentage.ShouldBe(100f);
        _services.ApplicationTerminator.Received(1).Terminate();

        _presenter.RunDownloadAsync(Arg.Any<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>()).Returns(call =>
        {
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            return call.Arg<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>()(new Progress<UpdateDownloadProgress>(), cancellation.Token);
        });
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.Cancelled);
    }

    [Fact]
    public async Task Run_DownloadFailure_IsShownAsDownloadError()
    {
        Publish(brokenDownload: true);
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.Failed);
        ShownError().Caption.ShouldBe(_manager.Texts.DownloadError);
    }

    [Fact]
    public async Task Run_Verification_RejectsTamperedPackages()
    {
        // The feed hash matches the served bytes, the signature does not: the download is fine, the verification fails.
        Publish(tampered: true);
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.InvalidSignature);
        ShownError().Caption.ShouldBe(_manager.Texts.InvalidSignatureTitle);
    }

    [Fact]
    public async Task Run_VerificationCancelledByTheCaller_IsReportedAsCancelled()
    {
        Publish();
        using var cancellation = new CancellationTokenSource();
        _presenter.RunDownloadAsync(Arg.Any<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>()).Returns(async call =>
        {
            await call.Arg<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>()(new Progress<UpdateDownloadProgress>(), CancellationToken.None);
            cancellation.Cancel();
        });
        (await _flow.RunAsync(cancellation.Token)).ShouldBe(UpdateFlowResult.Cancelled);
        await _presenter.DidNotReceive().ShowErrorAsync(Arg.Any<UpdateErrorMessage>(), Arg.Any<Exception?>());
    }

    [Fact]
    public async Task Run_VerificationFailure_IsShownAsVerificationError()
    {
        Publish();
        _presenter.RunDownloadAsync(Arg.Any<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>()).Returns(async call =>
        {
            await call.Arg<Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task>>()(new Progress<UpdateDownloadProgress>(), CancellationToken.None);
            _services.FileSystem.File.Delete(_manager.DownloadedPackages.Values.Single());
        });
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.Failed);
        ShownError().Text.ShouldBe(_manager.Texts.PackageNotFound);
    }

    [Fact]
    public async Task Run_Install_ReportsMissingInstallerAndDeclinedElevation()
    {
        Publish();
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.Failed);
        ShownError().Caption.ShouldBe(_manager.Texts.InstallerInitializingError);

        _services.AddInstaller();
        _services.ProcessLauncher.Start(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(false);
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.ElevationDeclined);
    }

    [Fact]
    public async Task Run_RejectsOverlappingRuns()
    {
        Publish();
        var started = new TaskCompletionSource<bool>();
        var release = new TaskCompletionSource<bool>();
        _presenter.RunSearchAsync(Arg.Any<Func<CancellationToken, Task<bool>>>()).Returns(async _ =>
        {
            started.SetResult(true);
            await release.Task;
            return false;
        });

        var first = _flow.RunAsync();
        await started.Task;
        _flow.IsRunning.ShouldBeTrue();
        (await _flow.RunAsync()).ShouldBe(UpdateFlowResult.AlreadyRunning);
        release.SetResult(true);
        (await first).ShouldBe(UpdateFlowResult.NoUpdates);
        _flow.IsRunning.ShouldBeFalse();
    }
}
