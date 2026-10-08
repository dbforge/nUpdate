using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Services;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;

namespace nUpdate.Tests.Administration.App;

/// <summary>The transfer settings editor used by the wizard and the settings dialog.</summary>
public class TransferSettingsEditorTests
{
    private readonly AppTestContext _context = new();

    private TransferSettingsEditorViewModel Transfer => _context.Factory.Create<TransferSettingsEditorViewModel>();

    [Fact]
    public void TransferEditor_AdjustsDefaultPortsPerProtocol()
    {
        var editor = Transfer;
        editor.Protocol.ShouldBe(TransferProtocol.Sftp); // the recommended default
        editor.Port.ShouldBe(22);
        editor.Protocols[0].ShouldBe(TransferProtocol.Sftp);
        editor.Protocol = TransferProtocol.Ftp;
        editor.Port.ShouldBe(21);
        editor.Protocol = TransferProtocol.Sftp;
        editor.Port.ShouldBe(22);
        editor.Protocol = TransferProtocol.FtpsImplicit;
        editor.Port.ShouldBe(990);
        editor.Protocol = TransferProtocol.FtpsExplicit;
        editor.Port.ShouldBe(21);
        editor.Port = 8021;
        editor.Protocol = TransferProtocol.Sftp;
        editor.Port.ShouldBe(8021);
        editor.Protocol = TransferProtocol.Plugin;
        editor.Port.ShouldBe(8021);
        editor.IsPlugin.ShouldBeTrue();
    }

    [Fact]
    public void TransferEditor_RoundTripsSettingsAndSecrets()
    {
        var editor = Transfer;
        var settings = new TransferSettings
        {
            Protocol = TransferProtocol.Sftp,
            Host = "h",
            Port = 2222,
            Directory = "/d",
            Username = "u",
            UsePassiveMode = false,
            TrustedCertificateFingerprint = "cert",
            SftpPrivateKeyPath = "/key",
            TrustedHostKeyFingerprint = "host",
            PluginAssemblyPath = null,
            Proxy = new ProxySettings { Address = "http://proxy", Username = "pu" },
        };
        editor.Load(settings, new ProjectSecrets { TransferPassword = "pw", SftpKeyPassphrase = "pass", ProxyPassword = "pp" });

        editor.IsSftp.ShouldBeTrue();
        editor.IsFtp.ShouldBeFalse();
        editor.IsPlugin.ShouldBeFalse();
        editor.Port.ShouldBe(2222);
        editor.UseProxy.ShouldBeTrue();
        var back = editor.ToSettings();
        back.Protocol.ShouldBe(TransferProtocol.Sftp);
        back.Host.ShouldBe("h");
        back.Directory.ShouldBe("/d");
        back.SftpPrivateKeyPath.ShouldBe("/key");
        back.TrustedHostKeyFingerprint.ShouldBe("host");
        back.Proxy!.Address.ShouldBe("http://proxy");
        back.Proxy.Username.ShouldBe("pu");
        var secrets = new ProjectSecrets();
        editor.ApplySecrets(secrets);
        secrets.TransferPassword.ShouldBe("pw");
        secrets.SftpKeyPassphrase.ShouldBe("pass");
        secrets.ProxyPassword.ShouldBe("pp");

        editor.Directory = " ";
        editor.SftpPrivateKeyPath = " ";
        editor.PluginAssemblyPath = " ";
        editor.ProxyUsername = "";
        editor.Password = "";
        editor.SftpKeyPassphrase = "";
        editor.ProxyPassword = "";
        var cleared = editor.ToSettings();
        cleared.Directory.ShouldBe("/");
        cleared.SftpPrivateKeyPath.ShouldBeNull();
        cleared.PluginAssemblyPath.ShouldBeNull();
        cleared.Proxy!.Username.ShouldBeNull();
        editor.UseProxy = false;
        editor.ToSettings().Proxy.ShouldBeNull();
        editor.ApplySecrets(secrets);
        secrets.TransferPassword.ShouldBeNull();
        secrets.SftpKeyPassphrase.ShouldBeNull();
        secrets.ProxyPassword.ShouldBeNull();
        editor.Protocols.Count.ShouldBe(5);
        Should.Throw<ArgumentNullException>(() => editor.Load(null!, new ProjectSecrets()));
        Should.Throw<ArgumentNullException>(() => editor.Load(settings, null!));
        Should.Throw<ArgumentNullException>(() => editor.ApplySecrets(null!));
    }

    [Fact]
    public void TransferEditor_LoadsSettingsWithoutOptionalParts()
    {
        var editor = _context.Factory.Create<TransferSettingsEditorViewModel>();
        editor.Load(new TransferSettings { Protocol = TransferProtocol.Plugin, PluginAssemblyPath = "/p.dll", Proxy = new ProxySettings { Address = "http://p" } }, new ProjectSecrets());
        editor.PluginAssemblyPath.ShouldBe("/p.dll");
        editor.SftpPrivateKeyPath.ShouldBe("");
        editor.Password.ShouldBe("");
        editor.ProxyUsername.ShouldBe("");
        editor.UseProxy.ShouldBeTrue();
        editor.ProxyAddress = " ";
        editor.ToSettings().Proxy.ShouldBeNull();

        editor.Port = 21;
        editor.Protocol = TransferProtocol.FtpsImplicit;
        editor.Port.ShouldBe(990);
        editor.Protocol = TransferProtocol.Ftp;
        editor.Port.ShouldBe(21);
        editor.Port = 22;
        editor.Protocol = TransferProtocol.FtpsImplicit;
        editor.Port.ShouldBe(990);
        editor.Protocol = TransferProtocol.Sftp;
        editor.Port.ShouldBe(22);
        editor.Port = 990;
        editor.Protocol = TransferProtocol.FtpsImplicit;
        editor.Port.ShouldBe(990);
        editor.Protocol = TransferProtocol.Sftp;
        editor.Port.ShouldBe(22);
        editor.Protocol = TransferProtocol.Plugin;
        editor.Port.ShouldBe(22);
    }

    [Fact]
    public void TransferEditor_Validates()
    {
        var editor = Transfer;
        editor.Protocol = TransferProtocol.Ftp;
        editor.Validate().ShouldBe("Enter the server host name.");
        editor.Host = "h";
        editor.Port = 0;
        editor.Validate().ShouldBe("The port must be between 1 and 65535.");
        editor.Port = 21;
        editor.Validate().ShouldBe("Enter the user name.");
        editor.Username = "u";
        editor.Validate().ShouldBe("Enter the password.");
        editor.Password = "p";
        editor.Validate().ShouldBeNull();
        editor.Protocol = TransferProtocol.Sftp;
        editor.Password = "";
        editor.Validate().ShouldBe("Enter a password or choose a private key file.");
        editor.SftpPrivateKeyPath = "/key";
        editor.Validate().ShouldBeNull();
        editor.Protocol = TransferProtocol.Plugin;
        editor.Validate().ShouldBe("Choose the plugin assembly.");
        editor.PluginAssemblyPath = "/p.dll";
        editor.Validate().ShouldBeNull();
    }

    [Fact]
    public async Task TransferEditor_BrowsesFiles()
    {
        var editor = Transfer;
        _context.Files.PickFileAsync(Arg.Any<string>(), Arg.Any<FileTypeFilter[]>()).Returns("/picked");
        await editor.BrowseKeyFileCommand.ExecuteAsync(null);
        editor.SftpPrivateKeyPath.ShouldBe("/picked");
        await editor.BrowsePluginCommand.ExecuteAsync(null);
        editor.PluginAssemblyPath.ShouldBe("/picked");
        _context.Files.PickFileAsync(Arg.Any<string>(), Arg.Any<FileTypeFilter[]>()).Returns((string?)null);
        editor.SftpPrivateKeyPath = "keep";
        await editor.BrowseKeyFileCommand.ExecuteAsync(null);
        editor.SftpPrivateKeyPath.ShouldBe("keep");
        await editor.BrowsePluginCommand.ExecuteAsync(null);
        editor.PluginAssemblyPath.ShouldBe("/picked");
    }

    [Fact]
    public async Task TransferEditor_TestConnection_HandlesTrustAndErrors()
    {
        var editor = Transfer;
        editor.Protocol = TransferProtocol.Ftp;
        (await editor.TestConnectionAsync()).ShouldBeFalse();
        editor.TestResult.ShouldBe("Enter the server host name.");

        editor.Host = "h";
        editor.Username = "u";
        editor.Password = "p";
        (await editor.TestConnectionAsync()).ShouldBeTrue();
        editor.TestResult.ShouldBe("Connection successful.");
        editor.IsTesting.ShouldBeFalse();

        _context.Projects.TestConnectionAsync(Arg.Any<TransferSettings>(), Arg.Any<TransferCredentials>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new TransferException("refused"));
        (await editor.TestConnectionAsync()).ShouldBeFalse();
        editor.TestResult.ShouldBe("refused");

        var calls = 0;
        _context.Projects.TestConnectionAsync(Arg.Any<TransferSettings>(), Arg.Any<TransferCredentials>(), Arg.Any<CancellationToken>())
            .Returns(_ => calls++ == 0 ? Task.FromException(new UntrustedServerException("unknown", "abc", "CN=h")) : Task.CompletedTask);
        _context.Dialogs.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        (await editor.TestConnectionAsync()).ShouldBeTrue();
        editor.TrustedCertificateFingerprint.ShouldBe("abc");

        editor.Protocol = TransferProtocol.Sftp;
        calls = 0;
        (await editor.TestConnectionAsync()).ShouldBeTrue();
        editor.TrustedHostKeyFingerprint.ShouldBe("abc");

        calls = 0;
        _context.Dialogs.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(false);
        (await editor.TestConnectionAsync()).ShouldBeFalse();
        editor.TestResult.ShouldBe("The host key was not trusted.");

        // Without a fingerprint there is nothing to trust; the exception is reported like any other transfer error.
        _context.Projects.TestConnectionAsync(Arg.Any<TransferSettings>(), Arg.Any<TransferCredentials>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new UntrustedServerException("no fingerprint"));
        (await editor.TestConnectionAsync()).ShouldBeFalse();
        editor.TestResult.ShouldBe("no fingerprint");
    }

    [Fact]
    public async Task TransferEditor_ReportsInvalidSettingsInsteadOfThrowing()
    {
        var editor = _context.Factory.Create<TransferSettingsEditorViewModel>();
        editor.Host = "h";
        editor.Username = "u";
        editor.Password = "p";
        _context.Projects.TestConnectionAsync(Arg.Any<TransferSettings>(), Arg.Any<TransferCredentials>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ArgumentException("bad proxy"));
        (await editor.TestConnectionAsync()).ShouldBeFalse();
        editor.TestResult.ShouldBe("bad proxy");
    }
}
