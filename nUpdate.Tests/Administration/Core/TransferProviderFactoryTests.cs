using System.IO.Abstractions.TestingHelpers;
using nUpdate.Administration.Core.Transfer;
using nUpdate.Administration.TransferInterface;
using nUpdate.Tests.Administration.Support;

[assembly: ServiceProvider(typeof(nUpdate.Tests.Administration.Support.TestPluginProvider))]

namespace nUpdate.Tests.Administration.Core;

public class TransferProviderFactoryTests
{
    private readonly AdminTestContext _context = new();

    private TransferProviderFactory Factory => new(_context.FileSystem, _ => typeof(TransferProviderFactoryTests).Assembly);

    [Theory]
    [InlineData(TransferProtocol.Ftp)]
    [InlineData(TransferProtocol.FtpsExplicit)]
    [InlineData(TransferProtocol.FtpsImplicit)]
    public async Task Create_BuildsFtpProvidersWithoutConnecting(TransferProtocol protocol)
    {
        var settings = new TransferSettings { Protocol = protocol, Host = "ftp.example.com", Username = "u", UsePassiveMode = protocol == TransferProtocol.Ftp };
        await using var provider = Factory.Create(settings, new TransferCredentials { Password = "p" });
        provider.ShouldBeOfType<FtpTransferProvider>();
    }

    [Fact]
    public async Task Create_BuildsSftpProviderWithPasswordOrKey()
    {
        var settings = new TransferSettings { Protocol = TransferProtocol.Sftp, Host = "sftp.example.com", Port = 22, Username = "u" };
        await using (var withPassword = Factory.Create(settings, new TransferCredentials { Password = "p" }))
            withPassword.ShouldBeOfType<SftpTransferProvider>();
        await using (var withoutAnything = Factory.Create(settings, new TransferCredentials()))
            withoutAnything.ShouldBeOfType<SftpTransferProvider>();
        Should.Throw<ArgumentNullException>(() => new SftpTransferProvider(settings, new TransferCredentials(), null!));
        Should.Throw<ArgumentNullException>(() => new SftpTransferProvider(null!, new TransferCredentials(), _context.FileSystem));
        Should.Throw<ArgumentNullException>(() => new SftpTransferProvider(settings, null!, _context.FileSystem));
        Should.Throw<ArgumentNullException>(() => new FtpTransferProvider(null!, new TransferCredentials(), _context.FileSystem));
        Should.Throw<ArgumentNullException>(() => new FtpTransferProvider(settings, null!, _context.FileSystem));
        Should.Throw<ArgumentNullException>(() => new FtpTransferProvider(settings, new TransferCredentials(), null!));
    }

    [Fact]
    public async Task Create_LoadsPluginFactories()
    {
        _context.FileSystem.AddFile("/plugins/custom.dll", new MockFileData("not really"));
        var settings = new TransferSettings { Protocol = TransferProtocol.Plugin, PluginAssemblyPath = "/plugins/custom.dll" };
        await using var provider = Factory.Create(settings, new TransferCredentials());
        provider.ShouldNotBeNull();
        Factory.LoadPluginFactory(settings).ShouldBeOfType<TestPluginFactory>();
        Factory.SupportedProtocols.Count.ShouldBe(5);

        Should.Throw<InvalidOperationException>(() => Factory.LoadPluginFactory(new TransferSettings { Protocol = TransferProtocol.Plugin }));
        Should.Throw<FileNotFoundException>(() => Factory.LoadPluginFactory(new TransferSettings { Protocol = TransferProtocol.Plugin, PluginAssemblyPath = "/plugins/missing.dll" }));
        Should.Throw<InvalidOperationException>(() => TransferProviderFactory.FromAssembly(typeof(object).Assembly));
        Should.Throw<InvalidOperationException>(() => TransferProviderFactory.FromAssembly(typeof(nUpdate.Administration.Core.AdministrationPaths).Assembly));
        Should.Throw<InvalidOperationException>(() => TransferProviderFactory.FromProvider(Substitute.For<IServiceProvider>()));
        Should.Throw<ArgumentNullException>(() => TransferProviderFactory.FromProvider(null!));
        Should.Throw<ArgumentNullException>(() => TransferProviderFactory.FromAssembly(null!));
        Should.Throw<ArgumentNullException>(() => Factory.LoadPluginFactory(null!));
    }

    [Fact]
    public void Create_ValidatesArguments()
    {
        Should.Throw<ArgumentNullException>(() => Factory.Create(null!, new TransferCredentials()));
        Should.Throw<ArgumentNullException>(() => Factory.Create(new TransferSettings { Host = "h" }, null!));
        Should.Throw<ArgumentException>(() => Factory.Create(new TransferSettings { Host = " " }, new TransferCredentials()));
        Should.Throw<NotSupportedException>(() => Factory.Create(new TransferSettings { Host = "h", Protocol = (TransferProtocol)99 }, new TransferCredentials()));
        Should.Throw<ArgumentNullException>(() => new TransferProviderFactory(null!));
        Should.Throw<ArgumentNullException>(() => new TransferProviderFactory(_context.FileSystem, null!));
        new TransferProviderFactory(_context.FileSystem).SupportedProtocols.ShouldContain(TransferProtocol.Sftp);
    }
}
