using System.Security.Authentication;
using FluentFTP.Exceptions;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Core.Transfer;

/// <summary>
///     Explains a refused FTPS data connection. Servers such as vsftpd and ProFTPD require by default that every data
///     connection resumes the TLS session of the control connection. The TLS of .NET, which FluentFTP uses, cannot do
///     that, so the login works but listing and uploading fail (issue #86).
/// </summary>
internal static class FtpsDataConnection
{
    /// <summary>What to do about it: the server setting that allows fresh sessions, or SFTP.</summary>
    public const string Advice =
        "If the server requires data connections to resume the TLS session of the control connection, as vsftpd and ProFTPD do by default, " +
        "nUpdate Administration cannot use it: allow data connections without that on the server (vsftpd: require_ssl_reuse=NO, " +
        "ProFTPD: TLSOptions NoSessionReuseRequired) or publish over SFTP.";

    /// <summary>The replies of a server that refuses a data connection: cannot open it (425), not available (450), TLS failed (522).</summary>
    private static readonly string[] RefusalCodes = ["425", "450", "522"];

    /// <summary>The message of a failed transfer: the server's own, followed by <see cref="Advice" /> when an FTPS data connection looks refused.</summary>
    public static string Describe(Exception exception, TransferProtocol protocol)
    {
        ArgumentNullException.ThrowIfNull(exception);
        var encrypted = protocol is TransferProtocol.FtpsExplicit or TransferProtocol.FtpsImplicit;
        return encrypted && LooksRefused(exception) ? exception.Message + " " + Advice : exception.Message;
    }

    private static bool LooksRefused(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is AuthenticationException
                || (current is FtpCommandException command && RefusalCodes.Contains(command.CompletionCode))
                || current.Message.Contains("reuse", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
