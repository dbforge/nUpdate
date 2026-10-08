using nUpdate.Administration.TransferInterface;
using nUpdate.Tests.Integration.Support;

namespace nUpdate.Tests.Integration;

[Collection(ServerCollectionFixture.Name)]
[Trait("Category", "Integration")]
public sealed class FtpTransferProviderTests : IDisposable
{
    private readonly IntegrationContext _context;

    public FtpTransferProviderTests(ServerFixture server)
    {
        _context = new IntegrationContext(server);
    }

    public void Dispose() => _context.Dispose();

    [DockerFact]
    public async Task FtpTransferProvider_SupportsTheWholeContract()
    {
        await _context.Server.ResetAsync();
        await using var ftp = await _context.ConnectTrustedAsync(_context.FtpSettings(), IntegrationContext.FtpCredentials);
        await TransferContract.ExerciseAsync(_context, ftp, "ftp");
    }

    [DockerFact]
    public async Task Connect_FtpsExplicit_RequiresTrustingTheSelfSignedCertificate()
    {
        await _context.Server.ResetAsync();
        var settings = _context.FtpSettings(TransferProtocol.FtpsExplicit);
        await using (var untrusted = _context.TransferFactory.Create(settings, IntegrationContext.FtpCredentials))
        {
            var ex = await Should.ThrowAsync<UntrustedServerException>(() => untrusted.ConnectAsync());
            ex.Fingerprint.ShouldNotBeNullOrEmpty();
            ex.Subject!.ShouldContain("localhost");
            settings.TrustedCertificateFingerprint = ex.Fingerprint;
        }

        await using var trusted = _context.TransferFactory.Create(settings, IntegrationContext.FtpCredentials);
        await trusted.ConnectAsync();
        await TransferContract.ExerciseAsync(_context, trusted, "ftps");

        settings.TrustedCertificateFingerprint = "0000";
        await using var wrong = _context.TransferFactory.Create(settings, IntegrationContext.FtpCredentials);
        await Should.ThrowAsync<UntrustedServerException>(() => wrong.ConnectAsync());
    }

    [DockerFact]
    public async Task Connect_RejectsWrongCredentialsAndUnreachableHosts()
    {
        await using (var wrongPassword = _context.TransferFactory.Create(_context.FtpSettings(), new TransferCredentials { Password = "nope" }))
            (await Should.ThrowAsync<TransferException>(() => wrongPassword.ConnectAsync())).Message.ShouldContain("rejected");

        var unreachable = _context.FtpSettings();
        unreachable.Port = 1;
        await using var noServer = _context.TransferFactory.Create(unreachable, IntegrationContext.FtpCredentials);
        await Should.ThrowAsync<TransferException>(() => noServer.ConnectAsync());
    }

    [DockerFact]
    public async Task UploadFile_UploadIsServedOverHttp()
    {
        await _context.Server.ResetAsync();
        await using (var ftp = await _context.ConnectTrustedAsync(_context.FtpSettings(), IntegrationContext.FtpCredentials))
            await ftp.UploadFileAsync(_context.WriteFile("via-ftp.txt", "ftp"), "via-ftp.txt");

        (await _context.HttpClient.GetStringAsync(_context.Server.HttpBaseUrl + "via-ftp.txt")).ShouldBe("ftp");
    }
}
