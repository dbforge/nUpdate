using nUpdate.Administration.Core.Transfer;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Administration.Core;

public class FtpsDataConnectionTests
{
    [Theory]
    [InlineData("522", "SSL connection failed: session reuse required")] // vsftpd
    [InlineData("425", "Unable to build data connection: Operation not permitted")] // ProFTPD
    [InlineData("450", "Data connection not available")]
    public void Describe_AdvisesWhenAnEncryptedDataConnectionIsRefused(string code, string reply)
    {
        var refused = new FluentFTP.Exceptions.FtpCommandException(code, reply);

        var message = FtpsDataConnection.Describe(refused, TransferProtocol.FtpsExplicit);

        message.ShouldStartWith(refused.Message + " "); // FluentFTP's own text, such as "Code: 522 Message: ..."
        message.ShouldContain(reply);
        message.ShouldEndWith(FtpsDataConnection.Advice);
        FtpsDataConnection.Describe(refused, TransferProtocol.FtpsImplicit).ShouldEndWith(FtpsDataConnection.Advice);
    }

    [Fact]
    public void Describe_AdvisesOnAFailedHandshakeAndOnAMentionedReuse()
    {
        var handshake = new IOException("The data connection failed.", new System.Security.Authentication.AuthenticationException("The remote party closed the stream."));
        FtpsDataConnection.Describe(handshake, TransferProtocol.FtpsExplicit).ShouldEndWith(FtpsDataConnection.Advice);
        FtpsDataConnection.Describe(new IOException("TLS session reuse is required"), TransferProtocol.FtpsExplicit).ShouldEndWith(FtpsDataConnection.Advice);
    }

    [Fact]
    public void Describe_LeavesOtherFailuresAsTheyAre()
    {
        var refused = new FluentFTP.Exceptions.FtpCommandException("522", "SSL connection failed: session reuse required");
        FtpsDataConnection.Describe(refused, TransferProtocol.Ftp).ShouldBe(refused.Message); // no TLS, no session to resume
        FtpsDataConnection.Describe(refused, TransferProtocol.Sftp).ShouldBe(refused.Message);
        var missing = new FluentFTP.Exceptions.FtpCommandException("550", "No such file or directory");
        FtpsDataConnection.Describe(missing, TransferProtocol.FtpsExplicit).ShouldBe(missing.Message);
        FtpsDataConnection.Describe(new IOException("broken pipe", new TimeoutException()), TransferProtocol.FtpsExplicit).ShouldBe("broken pipe");
        Should.Throw<ArgumentNullException>(() => FtpsDataConnection.Describe(null!, TransferProtocol.FtpsExplicit));
    }
}
