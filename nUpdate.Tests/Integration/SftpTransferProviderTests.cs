using nUpdate.Administration.TransferInterface;
using nUpdate.Tests.Integration.Support;

namespace nUpdate.Tests.Integration;

[Collection(ServerCollectionFixture.Name)]
[Trait("Category", "Integration")]
public sealed class SftpTransferProviderTests : IDisposable
{
    private readonly IntegrationContext _context;

    public SftpTransferProviderTests(ServerFixture server)
    {
        _context = new IntegrationContext(server);
    }

    public void Dispose() => _context.Dispose();

    [DockerFact]
    public async Task SftpTransferProvider_SupportsTheWholeContractAfterTrustingTheHostKey()
    {
        await _context.Server.ResetAsync();
        var settings = _context.SftpSettings();
        await using (var untrusted = _context.TransferFactory.Create(settings, IntegrationContext.SftpCredentials))
        {
            var ex = await Should.ThrowAsync<UntrustedServerException>(() => untrusted.ConnectAsync());
            ex.Fingerprint.ShouldNotBeNullOrEmpty();
            settings.TrustedHostKeyFingerprint = ex.Fingerprint;
        }

        await using var sftp = _context.TransferFactory.Create(settings, IntegrationContext.SftpCredentials);
        await sftp.ConnectAsync();
        await TransferContract.ExerciseAsync(_context, sftp, "sftp");

        settings.TrustedHostKeyFingerprint = "0000";
        await using var changed = _context.TransferFactory.Create(settings, IntegrationContext.SftpCredentials);
        await Should.ThrowAsync<UntrustedServerException>(() => changed.ConnectAsync());
    }

    [DockerFact]
    public async Task Connect_RejectsWrongCredentials()
    {
        var settings = _context.SftpSettings();
        await using (var learn = _context.TransferFactory.Create(settings, IntegrationContext.SftpCredentials))
        {
            try
            {
                await learn.ConnectAsync();
            }
            catch (UntrustedServerException ex)
            {
                settings.TrustedHostKeyFingerprint = ex.Fingerprint;
            }
        }

        await using var wrong = _context.TransferFactory.Create(settings, new TransferCredentials { Password = "nope" });
        (await Should.ThrowAsync<TransferException>(() => wrong.ConnectAsync())).Message.ShouldContain("rejected");
    }

    [DockerFact]
    public async Task UploadFile_UploadIsServedOverHttp()
    {
        await _context.Server.ResetAsync();
        await using (var sftp = await _context.ConnectTrustedAsync(_context.SftpSettings(), IntegrationContext.SftpCredentials))
            await sftp.UploadFileAsync(_context.WriteFile("via-sftp.txt", "sftp"), "via-sftp.txt");

        (await _context.HttpClient.GetStringAsync(_context.Server.HttpBaseUrl + "via-sftp.txt")).ShouldBe("sftp");
    }
}
