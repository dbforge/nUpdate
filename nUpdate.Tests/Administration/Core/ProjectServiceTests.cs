using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.TransferInterface;
using nUpdate.Tests.Administration.Support;
using nUpdate.Tests.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public class ProjectServiceTests
{
    private readonly AdminTestContext _context = new();
    private readonly ProjectService _service;
    private readonly List<string> _uploads = [];

    public ProjectServiceTests()
    {
        _service = new ProjectService(_context.FileSystem, _context.Paths, _context.Store, _context.Passwords, _context.TransferFactory, _context.Statistics, _context.Migrator, _context.Logger,
            _ => (TestKeys.PublicKey, TestKeys.PrivateKey));
        _context.Transfer.UploadFileAsync(Arg.Any<string>(), Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                _uploads.Add(call.ArgAt<string>(1) + "=" + _context.FileSystem.File.ReadAllText(call.ArgAt<string>(0)));
                return Task.CompletedTask;
            });
        _context.ServeLegacyFeed(null);
    }

    private NewProjectRequest Request(bool statistics = false) => new()
    {
        Name = "New Project",
        Folder = _context.ProjectFolder("New Project"),
        UpdateUrl = "https://updates.example.com/new",
        Transfer = new TransferSettings { Host = "ftp.example.com", Username = "u" },
        Secrets = new ProjectSecrets { TransferPassword = "pw", StatisticsDatabasePassword = "dbpw" },
        Statistics = statistics ? new StatisticsSettings { Enabled = true, Database = new StatisticsDatabaseSettings { Name = "db", Username = "dbu" } } : new StatisticsSettings(),
        ProjectPassword = "project-pw",
    };

    [Fact]
    public async Task Create_WritesProjectKeysSecretsAndStatistics()
    {
        var result = await _service.CreateAsync(Request(statistics: true));

        var project = result.Project;
        project.Name.ShouldBe("New Project");
        project.UpdateUrl.ShouldBe("https://updates.example.com/new/");
        project.PublicKey.ShouldBe(TestKeys.PublicKey);
        project.Statistics.EndpointUrl.ShouldBeNull();
        project.Path.ShouldBe(_context.FileSystem.Path.Combine(_context.ProjectFolder("New Project"), "project.nupdproj"));
        _context.FileSystem.Directory.Exists(project.PackagesDirectory).ShouldBeTrue();
        result.Secrets.PrivateKey.ShouldBe(TestKeys.PrivateKey);
        result.Secrets.StatisticsAdminSecret.ShouldNotBeNullOrEmpty();
        result.SecretsState.ShouldBe(SecretsState.Loaded);
        project.Secrets.ShouldNotBeNull();
        project.Log.Single().Kind.ShouldBe(LogEntryKind.Create);

        (await _context.Store.ListAsync()).Single().Name.ShouldBe("New Project");
        (await _context.Passwords.GetAsync(project.Id)).ShouldBe("project-pw");
        var loaded = await _context.Store.LoadAsync(project.Path, "project-pw");
        loaded.Secrets.PrivateKey.ShouldBe(TestKeys.PrivateKey);
        loaded.Secrets.TransferPassword.ShouldBe("pw");
        _uploads.Select(u => u.Split('=')[0]).ShouldBe(["nupdate-statistics.php", "nupdate-statistics.config.php"]);
        _uploads[0].ShouldContain("version 2");
        _uploads[1].ShouldContain("$nupdateDbPassword = 'dbpw';");
        // Script and config are uploaded from temp files and never kept locally.
        _context.FileSystem.AllFiles.Any(f => f.EndsWith(".php", StringComparison.Ordinal)).ShouldBeFalse();
        await _context.Statistics.Received().VerifyAsync(Arg.Is<StatisticsEndpoint>(e => e.AdminSecret == result.Secrets.StatisticsAdminSecret && e.Uri.ToString() == "https://updates.example.com/new/nupdate-statistics.php"), Arg.Any<CancellationToken>());
        await _context.Transfer.Received().ListAsync("", false, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WithoutStatisticsConnectionTestOrPassword()
    {
        var request = Request();
        request.UpdateUrl = "https://updates.example.com/new/";
        request.TestConnection = false;
        request.ProjectPassword = null;
        request.Folder = "/elsewhere/p";
        var result = await _service.CreateAsync(request);
        result.Project.Path.ShouldBe(_context.FileSystem.Path.Combine(_context.FileSystem.Path.GetFullPath("/elsewhere/p"), "project.nupdproj"));
        result.Project.Secrets.ShouldBeNull();
        result.SecretsState.ShouldBe(SecretsState.NotSaved);
        (await _context.Passwords.GetAsync(result.Project.Id)).ShouldBeNull();
        await _context.Transfer.DidNotReceive().ConnectAsync(Arg.Any<CancellationToken>());
        await _context.Statistics.DidNotReceive().VerifyAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<CancellationToken>());
        _context.FileSystem.Directory.Exists("/elsewhere/p/packages").ShouldBeTrue();
    }

    [Fact]
    public async Task Create_RollsBackWhenStatisticsSetupFails()
    {
        _context.Statistics.VerifyAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<CancellationToken>()).Returns(_ => throw new StatisticsException("db down"));
        var ex = await Should.ThrowAsync<PipelineException>(() => _service.CreateAsync(Request(statistics: true)));
        ex.FailedStep.ShouldBe("Setting up the statistics");
        _context.FileSystem.Directory.Exists(_context.ProjectFolder("New Project")).ShouldBeFalse();
        (await _context.Store.ListAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Create_KeepsAnExistingFolderWhenSavingFails()
    {
        var store = Substitute.For<IProjectStore>();
        store.ListAsync(Arg.Any<CancellationToken>()).Returns([]);
        store.SaveAsync(Arg.Any<UpdateProject>(), Arg.Any<CancellationToken>()).Returns(_ => throw new IOException("disk"));
        var service = new ProjectService(_context.FileSystem, _context.Paths, store, _context.Passwords, _context.TransferFactory, _context.Statistics, _context.Migrator, _context.Logger, _ => ("pub", "priv"));
        _context.FileSystem.AddDirectory(_context.ProjectFolder("New Project"));

        var ex = await Should.ThrowAsync<PipelineException>(() => service.CreateAsync(Request()));

        ex.FailedStep.ShouldBe("Saving the project");
        _context.FileSystem.Directory.Exists(_context.ProjectFolder("New Project")).ShouldBeTrue();
    }

    [Fact]
    public async Task Create_ValidatesRequests()
    {
        await Should.ThrowAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
        var empty = Request();
        empty.Name = " ";
        await Should.ThrowAsync<ArgumentException>(() => _service.CreateAsync(empty));
        var invalidName = Request();
        invalidName.Name = "a/b";
        await Should.ThrowAsync<ArgumentException>(() => _service.CreateAsync(invalidName));
        var padded = Request();
        padded.Name = " x ";
        await Should.ThrowAsync<ArgumentException>(() => _service.CreateAsync(padded));
        var noFolder = Request();
        noFolder.Folder = " ";
        await Should.ThrowAsync<ArgumentException>(() => _service.CreateAsync(noFolder));
        var badUrl = Request();
        badUrl.UpdateUrl = "not a url";
        await Should.ThrowAsync<ArgumentException>(() => _service.CreateAsync(badUrl));
        var noDb = Request(statistics: true);
        noDb.Statistics.Database = null;
        await Should.ThrowAsync<ArgumentException>(() => _service.CreateAsync(noDb));
        var taken = Request();
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(taken.Folder, "project.nupdproj"), new System.IO.Abstractions.TestingHelpers.MockFileData("{}"));
        (await Should.ThrowAsync<ArgumentException>(() => _service.CreateAsync(taken))).Message.ShouldContain("already holds");

        await _context.Store.RegisterAsync(new ProjectRegistration(Guid.NewGuid(), "new project", "/x"));
        await Should.ThrowAsync<ArgumentException>(() => _service.CreateAsync(Request()));
    }

    [Fact]
    public async Task TestConnection_ConnectsAndLists()
    {
        await _service.TestConnectionAsync(new TransferSettings { Host = "h" }, new TransferCredentials());
        await _context.Transfer.Received().ConnectAsync(Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DisposeAsync();
        await Should.ThrowAsync<ArgumentNullException>(() => _service.TestConnectionAsync(null!, new TransferCredentials()));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.TestConnectionAsync(new TransferSettings(), null!));
    }

    [Fact]
    public async Task Rename_ChangesNameAndRegistrationOnly()
    {
        var project = _context.NewProject();
        await _context.Store.SaveAsync(project);
        var path = project.Path;

        await _service.RenameAsync(project, "Renamed");

        project.Name.ShouldBe("Renamed");
        project.Path.ShouldBe(path);
        _context.FileSystem.Directory.Exists(_context.ProjectFolder("Demo")).ShouldBeTrue();
        (await _context.Store.ListAsync()).Single().Name.ShouldBe("Renamed");
        project.Log.Last().Kind.ShouldBe(LogEntryKind.Edit);

        await _service.RenameAsync(project, "Renamed");
        await Should.ThrowAsync<ArgumentException>(() => _service.RenameAsync(project, "bad/name"));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.RenameAsync(null!, "x"));
        await _context.Store.RegisterAsync(new ProjectRegistration(Guid.NewGuid(), "Other", "/o"));
        await Should.ThrowAsync<ArgumentException>(() => _service.RenameAsync(project, "other"));
        await _service.RenameAsync(project, "RENAMED");
        project.Name.ShouldBe("RENAMED");
    }

    [Fact]
    public async Task Save_ProtectsSecretsUnderThePasswordOrDropsThem()
    {
        var project = _context.NewProject();
        await _service.SaveAsync(project, new ProjectSecrets { TransferPassword = "pw", PrivateKey = "k" }, "project-pw");
        (await _context.Store.LoadAsync(project.Path, "project-pw")).Secrets.TransferPassword.ShouldBe("pw");
        (await _context.Passwords.GetAsync(project.Id)).ShouldBe("project-pw");

        await _service.SaveAsync(project, new ProjectSecrets { TransferPassword = "pw" }, null);
        (await _context.Store.LoadAsync(project.Path)).SecretsState.ShouldBe(SecretsState.NotSaved);
        (await _context.Passwords.GetAsync(project.Id)).ShouldBeNull();

        await Should.ThrowAsync<ArgumentNullException>(() => _service.SaveAsync(null!, new ProjectSecrets(), null));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.SaveAsync(project, null!, null));
    }

    [Fact]
    public async Task SaveMigrated_WritesProjectFileIntoTheFolderAndReplacesTheOldRegistration()
    {
        var path = _context.FileSystem.Path;
        var old = path.Combine(path.GetTempPath(), "old");
        var legacyFile = path.Combine(old, "Legacy.nupdproj");
        var otherFile = path.Combine(old, "Other.nupdproj");
        var project = _context.NewProject();
        project.Path = legacyFile;
        _context.FileSystem.AddFile(project.Path, new System.IO.Abstractions.TestingHelpers.MockFileData("{legacy}"));
        await _context.Store.RegisterAsync(new ProjectRegistration(Guid.Empty, "Legacy", legacyFile));
        await _context.Store.RegisterAsync(new ProjectRegistration(Guid.Empty, "Other", otherFile));
        var secrets = AdminTestContext.NewSecrets();

        await _service.SaveMigratedAsync(project, secrets, "pw");

        // "old" may be a shared folder like Documents, so the project gets a folder of its own name.
        project.Path.ShouldBe(path.Combine(old, "Demo", "project.nupdproj"));
        project.LegacyProjectFile.ShouldBe(legacyFile);
        (await _context.Store.LoadAsync(project.Path, "pw")).Project.LegacyProjectFile.ShouldBe(legacyFile);
        _context.FileSystem.File.Exists(legacyFile).ShouldBeTrue();
        _context.FileSystem.Directory.Exists(path.Combine(old, "Demo", "packages")).ShouldBeTrue();
        (await _context.Store.LoadAsync(project.Path, "pw")).Secrets.PrivateKey.ShouldBe(TestKeys.PrivateKey);
        (await _context.Store.ListAsync()).Select(r => $"{r.Name}={r.Path}").ShouldBe(["Demo=" + project.Path, "Other=" + otherFile]);
        project.Log.Single().Kind.ShouldBe(LogEntryKind.Migrate);

        // An old file that is called project.nupdproj already is not overwritten: nUpdate Administration 4 keeps using it.
        var current = _context.NewProject("Current");
        var currentOld = current.Path;
        await _context.Store.SaveAsync(current);
        var oldContent = _context.FileSystem.File.ReadAllText(currentOld);
        await _service.SaveMigratedAsync(current, secrets, null);
        current.Path.ShouldBe(_context.FileSystem.Path.Combine(_context.ProjectFolder("Current"), "Current", "project.nupdproj"));
        current.LegacyProjectFile.ShouldBe(currentOld);
        current.Secrets.ShouldBeNull();
        _context.FileSystem.File.ReadAllText(currentOld).ShouldBe(oldContent);
        (await _context.Store.ListAsync()).Single(r => r.Name == "Current").Path.ShouldBe(current.Path);

        // A file in the data folder of nUpdate Administration 4, which deletes that folder with the project, moves to the projects folder.
        var inData = _context.NewProject("Data");
        inData.Path = _context.FileSystem.Path.Combine(_context.Paths.LegacyProjectDataDirectory("Data"), "Data.nupdproj");
        _context.FileSystem.AddFile(inData.Path, new System.IO.Abstractions.TestingHelpers.MockFileData("{legacy}"));
        await _service.SaveMigratedAsync(inData, secrets, null);
        inData.Path.ShouldBe(_context.FileSystem.Path.Combine(_context.ProjectFolder("Data"), "project.nupdproj"));

        // A file in a folder of the project's name (the 5.0 pre-release layout) is converted in place.
        var inPlace = _context.NewProject("Inplace");
        var inPlaceFolder = path.Combine(path.GetTempPath(), "projects", "Inplace");
        inPlace.Path = path.Combine(inPlaceFolder, "Inplace.nupdproj");
        _context.FileSystem.AddFile(inPlace.Path, new System.IO.Abstractions.TestingHelpers.MockFileData("{legacy}"));
        await _service.SaveMigratedAsync(inPlace, secrets, null);
        inPlace.Path.ShouldBe(path.Combine(inPlaceFolder, "project.nupdproj"));

        // The target folder already holds a different project: refused, nothing is written.
        var clash = _context.NewProject("Current");
        clash.Id = Guid.NewGuid();
        clash.Path = _context.FileSystem.Path.Combine(_context.ProjectFolder("Current"), "Old.nupdproj");
        _context.FileSystem.AddFile(clash.Path, new System.IO.Abstractions.TestingHelpers.MockFileData("{legacy}"));
        (await Should.ThrowAsync<InvalidOperationException>(() => _service.SaveMigratedAsync(clash, secrets, null))).Message.ShouldContain("another project");
        (await _context.Store.LoadAsync(currentOld)).Project.Id.ShouldBe(current.Id);
        // The same project id in the target is fine (the conversion ran before and was interrupted).
        var again = _context.NewProject("Current");
        again.Path = clash.Path;
        await _service.SaveMigratedAsync(again, secrets, null);
        again.Path.ShouldBe(currentOld);

        await Should.ThrowAsync<ArgumentNullException>(() => _service.SaveMigratedAsync(null!, secrets, null));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.SaveMigratedAsync(project, null!, null));
        await Should.ThrowAsync<ArgumentException>(() => _service.SaveMigratedAsync(new UpdateProject(), secrets, null));
    }

    [Fact]
    public async Task SetupStatistics_GeneratesSecretAndUsesEndpoint()
    {
        var project = _context.NewProject(statistics: true);
        project.Statistics.Database = new StatisticsDatabaseSettings { Name = "db" };
        var secrets = AdminTestContext.NewSecrets();
        await _service.SetupStatisticsAsync(project, secrets);
        secrets.StatisticsAdminSecret.ShouldNotBeNullOrEmpty();
        _uploads.Count.ShouldBe(2);
        await _context.Statistics.Received().VerifyAsync(Arg.Is<StatisticsEndpoint>(e => e.Uri.ToString() == "https://updates.example.com/demo/nupdate-statistics.php"), Arg.Any<CancellationToken>());

        var preset = _context.NewProject("Preset", statistics: true);
        preset.Statistics.Database = new StatisticsDatabaseSettings();
        preset.Statistics.EndpointUrl = "https://stats.example.com/x.php";
        await _service.SetupStatisticsAsync(preset, AdminTestContext.NewSecrets(statistics: true));
        await _context.Statistics.Received().VerifyAsync(Arg.Is<StatisticsEndpoint>(e => e.Uri.ToString() == "https://stats.example.com/x.php"), Arg.Any<CancellationToken>());

        var invalid = _context.NewProject("Invalid", statistics: true);
        invalid.Statistics.Database = new StatisticsDatabaseSettings();
        invalid.UpdateUrl = "not a url";
        await Should.ThrowAsync<InvalidOperationException>(() => _service.SetupStatisticsAsync(invalid, AdminTestContext.NewSecrets(statistics: true)));

        project.Statistics.Database = null;
        await Should.ThrowAsync<InvalidOperationException>(() => _service.SetupStatisticsAsync(project, secrets));
        project.Statistics.Enabled = false;
        await Should.ThrowAsync<InvalidOperationException>(() => _service.SetupStatisticsAsync(project, secrets));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.SetupStatisticsAsync(null!, secrets));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.SetupStatisticsAsync(project, null!));
    }

    [Fact]
    public async Task Delete_RemovesLocalAndRemoteFilesIncludingLegacyOnes()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.0.0"), Released = true });
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.1.0"), Released = false });
        await _service.SaveAsync(project, secrets, "pw");
        _context.ServeLegacyFeed("""[{"LiteralVersion":"0.9.0.0","UpdatePackageUri":"https://updates.example.com/demo/0.9.0.0/p.zip"}]""");

        await _service.DeleteAsync(project, secrets, deleteLocalFiles: true, deleteServerFiles: true);

        await _context.Statistics.Received().DeleteProjectAsync(Arg.Any<StatisticsEndpoint>(), project.Id, Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteDirectoryAsync("0.9.0.0", Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteFileAsync("updates.json", Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteDirectoryAsync("packages/1.0.0", Arg.Any<CancellationToken>());
        await _context.Transfer.DidNotReceive().DeleteFileAsync("packages/1.1.0.zip", Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteFileAsync("nupdate.json", Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteFileAsync("nupdate-statistics.php", Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteFileAsync("nupdate-statistics.config.php", Arg.Any<CancellationToken>());
        (await _context.Store.ListAsync()).ShouldBeEmpty();
        (await _context.Passwords.GetAsync(project.Id)).ShouldBeNull();
        _context.FileSystem.Directory.Exists(project.Folder).ShouldBeFalse();
    }

    [Fact]
    public async Task Delete_LeavesFilesTheProjectDoesNotOwn()
    {
        var project = _context.NewProject();
        await _service.SaveAsync(project, AdminTestContext.NewSecrets(), null);
        var foreign = _context.FileSystem.Path.Combine(project.Folder, "notes.txt");
        _context.FileSystem.AddFile(foreign, new System.IO.Abstractions.TestingHelpers.MockFileData("mine"));
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(project.PackagesDirectory, "1.0.0", "package.zip"), new System.IO.Abstractions.TestingHelpers.MockFileData("zip"));

        await _service.DeleteAsync(project, AdminTestContext.NewSecrets(), deleteLocalFiles: true, deleteServerFiles: false);

        _context.FileSystem.File.Exists(project.Path).ShouldBeFalse();
        _context.FileSystem.Directory.Exists(project.PackagesDirectory).ShouldBeFalse();
        _context.FileSystem.File.Exists(foreign).ShouldBeTrue();
        _context.FileSystem.Directory.Exists(project.Folder).ShouldBeTrue();
    }

    [Fact]
    public async Task Delete_CanKeepFiles()
    {
        var project = _context.NewProject(statistics: true);
        await _context.Store.SaveAsync(project);
        await _service.DeleteAsync(project, AdminTestContext.NewSecrets(), deleteLocalFiles: false, deleteServerFiles: false);
        _context.FileSystem.File.Exists(project.Path).ShouldBeTrue();
        await _context.Transfer.DidNotReceive().ConnectAsync(Arg.Any<CancellationToken>());
        (await _context.Store.ListAsync()).ShouldBeEmpty();

        var unsaved = _context.NewProject("Ghost", statistics: true);
        await _service.DeleteAsync(unsaved, AdminTestContext.NewSecrets(), deleteLocalFiles: true, deleteServerFiles: true);
        await _context.Statistics.DidNotReceive().DeleteProjectAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await Should.ThrowAsync<ArgumentNullException>(() => _service.DeleteAsync(null!, AdminTestContext.NewSecrets(), false, false));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.DeleteAsync(project, null!, false, false));
    }

    [Fact]
    public void Constructor_ValidatesArguments()
    {
        var c = _context;
        Should.Throw<ArgumentNullException>(() => new ProjectService(null!, c.Paths, c.Store, c.Passwords, c.TransferFactory, c.Statistics, c.Migrator, c.Logger));
        Should.Throw<ArgumentNullException>(() => new ProjectService(c.FileSystem, null!, c.Store, c.Passwords, c.TransferFactory, c.Statistics, c.Migrator, c.Logger));
        Should.Throw<ArgumentNullException>(() => new ProjectService(c.FileSystem, c.Paths, null!, c.Passwords, c.TransferFactory, c.Statistics, c.Migrator, c.Logger));
        Should.Throw<ArgumentNullException>(() => new ProjectService(c.FileSystem, c.Paths, c.Store, null!, c.TransferFactory, c.Statistics, c.Migrator, c.Logger));
        Should.Throw<ArgumentNullException>(() => new ProjectService(c.FileSystem, c.Paths, c.Store, c.Passwords, null!, c.Statistics, c.Migrator, c.Logger));
        Should.Throw<ArgumentNullException>(() => new ProjectService(c.FileSystem, c.Paths, c.Store, c.Passwords, c.TransferFactory, null!, c.Migrator, c.Logger));
        Should.Throw<ArgumentNullException>(() => new ProjectService(c.FileSystem, c.Paths, c.Store, c.Passwords, c.TransferFactory, c.Statistics, null!, c.Logger));
        Should.Throw<ArgumentNullException>(() => new ProjectService(c.FileSystem, c.Paths, c.Store, c.Passwords, c.TransferFactory, c.Statistics, c.Migrator, null!));
        Should.Throw<ArgumentNullException>(() => new ProjectService(c.FileSystem, c.Paths, c.Store, c.Passwords, c.TransferFactory, c.Statistics, c.Migrator, c.Logger, null!));
        new ProjectService(c.FileSystem, c.Paths, c.Store, c.Passwords, c.TransferFactory, c.Statistics, c.Migrator, c.Logger).ShouldNotBeNull();
    }

    [Fact]
    public async Task Create_GeneratesRealKeysByDefault()
    {
        var service = new ProjectService(_context.FileSystem, _context.Paths, _context.Store, _context.Passwords, _context.TransferFactory, _context.Statistics, _context.Migrator, _context.Logger);
        var request = Request();
        request.KeySize = 1024;
        request.TestConnection = false;
        var result = await service.CreateAsync(request);
        result.Project.PublicKey.ShouldStartWith("-----BEGIN PUBLIC KEY-----");
        result.Secrets.PrivateKey!.ShouldStartWith("-----BEGIN PRIVATE KEY-----");
    }
}
