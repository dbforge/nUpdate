// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using nUpdate.Administration.Core.Ftp.Exceptions;

namespace nUpdate.Administration.Core.Ftp
{
    /// <summary>
    ///     FTP server commands.
    /// </summary>
    public enum FtpCmd
    {
        /// <summary>
        ///     Unknown command issued.
        /// </summary>
        Unknown,

        /// <summary>
        ///     The USER command.
        /// </summary>
        User,

        /// <summary>
        ///     The PASS command.
        /// </summary>
        Pass,

        /// <summary>
        ///     The MKD command.  Make new directory.
        /// </summary>
        Mkd,

        /// <summary>
        ///     The RMD command.  Remove directory.
        /// </summary>
        Rmd,

        /// <summary>
        ///     The RETR command.  Retrieve file.
        /// </summary>
        Retr,

        /// <summary>
        ///     The PWD command.  Print working directory.
        /// </summary>
        Pwd,

        /// <summary>
        ///     The SYST command.  System status.
        /// </summary>
        Syst,

        /// <summary>
        ///     The CDUP command.  Change directory up.
        /// </summary>
        Cdup,

        /// <summary>
        ///     The DELE command.  Delete file or directory.
        /// </summary>
        Dele,

        /// <summary>
        ///     The TYPE command.  Transfer type.
        /// </summary>
        Type,

        /// <summary>
        ///     The CWD command.  Change working directory.
        /// </summary>
        Cwd,

        /// <summary>
        ///     The PORT command.  Data port.
        /// </summary>
        Port,

        /// <summary>
        ///     The PASV command.  Passive port.
        /// </summary>
        Pasv,

        /// <summary>
        ///     The STOR command.  Store file.
        /// </summary>
        Stor,

        /// <summary>
        ///     The STOU command.  Store file unique.
        /// </summary>
        Stou,

        /// <summary>
        ///     The APPE command.  Append file.
        /// </summary>
        Appe,

        /// <summary>
        ///     The RNFR command.  Rename file from.
        /// </summary>
        Rnfr,

        /// <summary>
        ///     The RFTO command.  Rename file to.
        /// </summary>
        Rnto,

        /// <summary>
        ///     The ABOR command.  Abort current operation.
        /// </summary>
        Abor,

        /// <summary>
        ///     The LIST command.  List files.
        /// </summary>
        List,

        /// <summary>
        ///     The NLST command.  Namelist files.
        /// </summary>
        Nlst,

        /// <summary>
        ///     The SITE command.  Site.
        /// </summary>
        Site,

        /// <summary>
        ///     The STAT command.  Status.
        /// </summary>
        Stat,

        /// <summary>
        ///     The NOOP command.  No operation.
        /// </summary>
        Noop,

        /// <summary>
        ///     The HELP command.  Help.
        /// </summary>
        Help,

        /// <summary>
        ///     The ALLO command.  Allocate space.
        /// </summary>
        Allo,

        /// <summary>
        ///     The QUIT command.  Quite session.
        /// </summary>
        Quit,

        /// <summary>
        ///     The REST command.  Restart transfer.
        /// </summary>
        Rest,

        /// <summary>
        ///     The AUTH command.  Initialize authentication.
        /// </summary>
        Auth,

        /// <summary>
        ///     The PBSZ command.
        /// </summary>
        Pbsz,

        /// <summary>
        ///     The PROT command.  Security protocol.
        /// </summary>
        Prot,

        /// <summary>
        ///     The MODE command.  Data transfer mode.
        /// </summary>
        Mode,

        /// <summary>
        ///     The MDTM command.  Month, date, and time.
        /// </summary>
        Mdtm,

        /// <summary>
        ///     The SIZE command.  File size.
        /// </summary>
        Size,

        /// <summary>
        ///     The FEAT command.  Supported features.
        /// </summary>
        Feat,

        /// <summary>
        ///     The XCRC command.  CRC file integrity.
        /// </summary>
        Xcrc,

        /// <summary>
        ///     The XMD5 command.  MD5 file integrity.
        /// </summary>
        Xmd5,

        /// <summary>
        ///     The XSHA1 command.  SHA1 file integerity.
        /// </summary>
        Xsha1,

        /// <summary>
        ///     The EPSV command.
        /// </summary>
        Epsv,

        /// <summary>
        ///     The ERPT command.
        /// </summary>
        Erpt
    }

    /// <summary>
    ///     FTP request object which contains the command, arguments and text or an FTP request.
    /// </summary>
    public class FtpRequest
    {
        private readonly string[] _arguments;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public FtpRequest()
        {
            Command = new FtpCmd();
            Text = string.Empty;
        }

        /// <summary>
        ///     FTP request constructor.
        /// </summary>
        /// <param name="command">FTP request command.</param>
        /// <param name="arguments">Parameters for the request</param>
        internal FtpRequest(FtpCmd command, params string[] arguments)
        {
            Command = command;
            _arguments = arguments;
            Text = BuildCommandText();
        }

        /// <summary>
        ///     FTP request constructor.
        /// </summary>
        /// <param name="command">FTP request command.</param>
        internal FtpRequest(FtpCmd command)
            : this(command, null)
        {
        }

        /// <summary>
        ///     Get the FTP command enumeration value.
        /// </summary>
        public FtpCmd Command { get; }

        /// <summary>
        ///     Get the FTP command arguments (if any).
        /// </summary>
        public List<string> Arguments
        {
            get
            {
                if (_arguments != null)
                {
                    return new List<string>(_arguments);
                }

                return new List<string>();
            }
        }

        /// <summary>
        ///     Get the FTP command text with any arguments.
        /// </summary>
        public string Text { get; }

        /// <summary>
        ///     Gets a boolean value indicating if the command is a file transfer or not.
        /// </summary>
        public bool IsFileTransfer
        {
            get
            {
                if (Command == FtpCmd.Stou || Command == FtpCmd.Stor || Command == FtpCmd.Retr)
                    return true;
                return false;
            }
        }

        internal bool HasHappyCodes => GetHappyCodes().Length != 0;

        internal string BuildCommandText()
        {
            string commandText = Command.ToString().ToUpper(CultureInfo.InvariantCulture);

            if (_arguments == null)
            {
                return commandText;
            }
            StringBuilder builder = new StringBuilder();
            foreach (string arg in _arguments)
            {
                builder.Append(arg);
                builder.Append(" ");
            }
            string argText = builder.ToString().TrimEnd();

            if (Command == FtpCmd.Unknown)
                return argText;
            return $"{commandText} {argText}".TrimEnd();
        }

        internal byte[] GetBytes()
        {
            return Encoding.ASCII.GetBytes($"{Text}\r\n");
        }

        internal FtpResponseCode[] GetHappyCodes()
        {
            switch (Command)
            {
                case FtpCmd.Unknown:
                    return BuildResponseArray();
                case FtpCmd.Allo:
                    return BuildResponseArray(FtpResponseCode.CommandOkay,
                        FtpResponseCode.CommandNotImplementedSuperfluousAtThisSite);
                case FtpCmd.User:
                    return BuildResponseArray(FtpResponseCode.UserNameOkayButNeedPassword,
                        FtpResponseCode.ServiceReadyForNewUser, FtpResponseCode.UserLoggedIn);
                case FtpCmd.Pass:
                    return BuildResponseArray(FtpResponseCode.UserLoggedIn, FtpResponseCode.ServiceReadyForNewUser,
                        FtpResponseCode.NotLoggedIn);
                case FtpCmd.Cwd:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Pwd:
                    return BuildResponseArray(FtpResponseCode.PathNameCreated);
                case FtpCmd.Dele:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Abor:
                    return BuildResponseArray();
                case FtpCmd.Mkd:
                    return BuildResponseArray(FtpResponseCode.PathNameCreated);
                case FtpCmd.Rmd:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Help:
                    return BuildResponseArray(FtpResponseCode.SystemStatusOrHelpReply, FtpResponseCode.HelpMessage,
                        FtpResponseCode.FileStatus);
                case FtpCmd.Mdtm:
                    return BuildResponseArray(FtpResponseCode.FileStatus);
                case FtpCmd.Stat:
                    return BuildResponseArray(FtpResponseCode.SystemStatusOrHelpReply, FtpResponseCode.DirectoryStatus,
                        FtpResponseCode.FileStatus);
                case FtpCmd.Cdup:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.Size:
                    return BuildResponseArray(FtpResponseCode.FileStatus);
                case FtpCmd.Feat:
                    return BuildResponseArray(FtpResponseCode.SystemStatusOrHelpReply);
                case FtpCmd.Syst:
                    return BuildResponseArray(FtpResponseCode.NameSystemType);
                case FtpCmd.Rnfr:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionPendingFurtherInformation);
                case FtpCmd.Rnto:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Noop:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.Site:
                    return BuildResponseArray(FtpResponseCode.CommandOkay,
                        FtpResponseCode.CommandNotImplementedSuperfluousAtThisSite);
                case FtpCmd.Pasv:
                    return BuildResponseArray(FtpResponseCode.EnteringPassiveMode);
                case FtpCmd.Port:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.Type:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.Rest:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionPendingFurtherInformation);
                case FtpCmd.Mode:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.Quit:
                    return BuildResponseArray();
                case FtpCmd.Auth:
                    return BuildResponseArray(FtpResponseCode.AuthenticationCommandOkay);
                case FtpCmd.Pbsz:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.Prot:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.List:
                case FtpCmd.Nlst:
                    return BuildResponseArray(FtpResponseCode.DataConnectionAlreadyOpenSoTransferStarting,
                        FtpResponseCode.FileStatusOkaySoAboutToOpenDataConnection,
                        FtpResponseCode.ClosingDataConnection,
                        FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Appe:
                case FtpCmd.Stor:
                case FtpCmd.Stou:
                case FtpCmd.Retr:
                    return BuildResponseArray(FtpResponseCode.DataConnectionAlreadyOpenSoTransferStarting,
                        FtpResponseCode.FileStatusOkaySoAboutToOpenDataConnection,
                        FtpResponseCode.ClosingDataConnection,
                        FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Xcrc:
                case FtpCmd.Xmd5:
                case FtpCmd.Xsha1:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Epsv:
                    return BuildResponseArray();
                case FtpCmd.Erpt:
                    return BuildResponseArray();

                default:
                    throw new FtpException($"No response code(s) defined for FtpCmd {Command}.");
            }
        }

        private FtpResponseCode[] BuildResponseArray(params FtpResponseCode[] codes)
        {
            return codes;
        }
    }
}