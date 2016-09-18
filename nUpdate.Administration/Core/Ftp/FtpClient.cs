// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using nUpdate.Administration.Core.Ftp.EventArgs;
using nUpdate.Administration.Core.Ftp.Exceptions;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Core.Ftp
{

    #region Public Enums

    /// <summary>
    ///     Enumeration representing type of file transfer mode.
    /// </summary>
    public enum TransferType
    {
        /// <summary>
        ///     No transfer type.
        /// </summary>
        None,

        /// <summary>
        ///     Transfer mode of type 'A' (ascii).
        /// </summary>
        Ascii,

        /// <summary>
        ///     Transfer mode of type 'I' (image or binary)
        /// </summary>
        Binary // TYPE I
    }

    /// <summary>
    ///     Enumeration representing the three types of actions that FTP supports when
    ///     uploading or 'putting' a file on an FTP server from the FTP client.
    /// </summary>
    public enum FileAction
    {
        /// <summary>
        ///     No action.
        /// </summary>
        None,

        /// <summary>
        ///     Create a new file or overwrite an existing file.
        /// </summary>
        Create,

        /// <summary>
        ///     Create a new file or append an existing file.
        /// </summary>
        CreateNew,

        /// <summary>
        ///     Create a new file.  Do not overwrite an existing file.
        /// </summary>
        CreateOrAppend,

        /// <summary>
        ///     Resume a file transfer.
        /// </summary>
        Resume,

        /// <summary>
        ///     Resume a file transfer if the file already exists.  Otherwise, create a new file.
        /// </summary>
        ResumeOrCreate
    }

    #endregion

    /// <summary>
    ///     The FtpClient Component for .NET is a fully .NET coded RFC 959 compatible FTP object component that supports the
    ///     RFC 959, SOCKS and HTTP proxies, SSLv2, SSLv3, and TLSv1
    ///     as well as automatic file integrity checks on all data transfers.
    ///     The component library also supports a pluggable directory listing parser.  The Starksoft FtpClient Component for
    ///     .NET support most FTP servers.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The object implements and uses the following FTP commands and provides a simple to use component.
    ///         FTP RFC 959 commands (and extended) directly supported:
    ///         USER    RMD     CDUP    CWD     STOU    RETR    AUTH    SITE CHMOD
    ///         PASS    RETR    DELE    PORT    APPE    MDTM    PROT
    ///         QUIT    PWD     TYPE    PASV    REST    SIZE    MODE
    ///         MKD     SYST    MODE    STOR    RNFR    FEAT    XSHA1
    ///         NLST    HELP    RNTO    SITE    ALLO    QUIT    XMD5
    ///         ABORT   STAT    LIST    NOOP    PBSZ    XCRC
    ///     </para>
    ///     <para>
    ///         Custom FTP server commands can be executed using the Quote() method.  This allows the FtpClient object to
    ///         handle
    ///         certain custom commands that are not supported by the RFC 959 standard but are required by specific FTP server
    ///         implementations for various tasks.
    ///     </para>
    ///     <para>
    ///         The Starksoft FtpClient Component for .NET supports SOCKS v4, SOCKS v4a, SOCKS v5, and HTTP proxy servers.  The
    ///         proxy settings are not read
    ///         from the local web browser proxy settings so deployment issues are not a problem with using proxy connections.
    ///         In addition the library also
    ///         supports active and passive (firewall friendly) mode transfers.  The Starksoft FtpClient Component for .NET
    ///         supports data compression, bandwidth throttling,
    ///         and secure connections through SSL (Secure Socket Layer) and TLS.  The Starksoft FtpClient Component for .NET
    ///         also supports automatic transfer integrity checks via
    ///         CRC, MD5, and SHA1.  The FtpClient object can parse many different directory listings from various FTP server
    ///         implementations.  But for those servers that are difficult to
    ///         parse of produce strange directory listings you can write your own ftp item parser.  See the IFtpItemParser
    ///         interface
    ///         for more information and an example parser.  Finally, the Starksoft FtpClient Component for .NET  also provides
    ///         support for encrypting and decrypting PGP data files though a .NET wrapper
    ///         class that interfaces directly with the open source GNU Open PGP executable.
    ///     </para>
    ///     <para>
    ///         The FtpClient libary has been tested with the following FTP servers and file formats.
    ///         <list type="">
    ///             <item>IIS 6.0 under Microsoft Windows 2000 and Windows 2003 server, </item>
    ///             <item>Microsoft FTP server running IIS 5.0</item>
    ///             <item>Gene6FTP Server</item>
    ///             <item>ProFTPd</item>
    ///             <item>Wu-FTPd</item>
    ///             <item>WS_FTP Server (by Ipswitch)</item>
    ///             <item>Serv-U FTP Server</item>
    ///             <item>GNU FTP server</item>
    ///             <item>Many public FTP servers</item>
    ///         </list>
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code>
    /// FtpClient ftp = new FtpClient("ftp.microsoft.com");
    /// // note: DataTransferMode is actually passive mode (PASV) by default
    /// ftp.DataTransferMode = DataTransferMode.Passive; 
    /// ftp.Open("anonymous", "myemail@host.com");
    /// ftp.ChangeDirectory("Softlib");
    /// ftp.GetFile("README.TXT", "c:\\README.TXT"); 
    /// ftp.Close();
    /// </code>
    /// </example>
    public class FtpClient : FtpBase
    {
        #region Contructors

        /// <summary>
        ///     FtpClient default constructor.
        /// </summary>
        public FtpClient()
            : base(DEFAULT_FTP_PORT, FtpSecurityProtocol.None)
        {
        }

        /// <summary>
        ///     Constructor method for FtpClient.
        /// </summary>
        /// <param name="host">String containing the host name or ip address of the remote FTP server.</param>
        /// <remarks>
        ///     This method takes one parameter to specify
        ///     the host name (or ip address).
        /// </remarks>
        public FtpClient(string host)
            : this(host, DEFAULT_FTP_PORT, FtpSecurityProtocol.None)
        {
        }

        /// <summary>
        ///     Constructor method for FtpClient.
        /// </summary>
        /// <param name="host">String containing the host name or ip address of the remote FTP server.</param>
        /// <param name="port">Port number used to connect to the remote FTP server.</param>
        /// <remarks>
        ///     This method takes two parameters that specify
        ///     the host name (or ip address) and the port to connect to the host.
        /// </remarks>
        public FtpClient(string host, int port)
            : base(host, port, FtpSecurityProtocol.None)
        {
        }

        /// <summary>
        ///     Constructor method for FtpClient.
        /// </summary>
        /// <param name="host">String containing the host name or ip address of the remote FTP server.</param>
        /// <param name="port">Port number used to connect to the remote FTP server.</param>
        /// <param name="securityProtocol">
        ///     Enumeration value indicating what security protocol (such as SSL) should be enabled for
        ///     this connection.
        /// </param>
        /// <remarks>
        ///     This method takes three parameters that specify
        ///     the host name (or ip address), port to connect to and what security protocol should be used when establishing the
        ///     connection.
        /// </remarks>
        public FtpClient(string host, int port, FtpSecurityProtocol securityProtocol)
            : base(host, port, securityProtocol)
        {
        }

        #endregion

        #region Private Variables and Constants

        private bool _disposed;
        private const int DEFAULT_FTP_PORT = 21; // default port is 21
        private const int FXP_TRANSFER_TIMEOUT = 600000; // 10 minutes

        private TransferType _fileTransferType = TransferType.Binary;
        private string _user;
        private string _password;
        private bool _opened;

        // transfer log
        private Stream _log = new MemoryStream();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the file transfer item.
        /// </summary>
        public TransferType FileTransferType
        {
            get { return _fileTransferType; }
            set
            {
                _fileTransferType = value;

                if (IsConnected)
                {
                    //  update the server with the new file transfer type
                    SetFileTransferType();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the the directory item parser to use when parsing directory listing data from the FTP server.
        ///     This parser is used by the GetDirList() and GetDirList(string) methods.
        /// </summary>
        /// <remarks>
        ///     You can create your own custom directory listing parser by creating an object that implements the
        ///     IFtpItemParser interface.  This is particular useful when parsing exotic file directory listing
        ///     formats from specific FTP servers.
        /// </remarks>
        public IFtpItemParser ItemParser { get; set; }

        /// <summary>
        ///     Gets or sets logging of file transfers.
        /// </summary>
        /// <remarks>
        ///     All data transfer activity can be retrieved from the Log property.
        /// </remarks>
        public bool IsLoggingOn { get; set; }

        /// <summary>
        ///     Gets or sets the Stream object used for logging data transfer activity.
        /// </summary>
        /// <remarks>
        ///     By default a MemoryStream object is created to log all data transfer activity.  Any
        ///     Stream object that can be written to may be used in place of the MemoryStream object.
        /// </remarks>
        /// <seealso cref="IsLoggingOn" />
        public Stream Log
        {
            get { return _log; }
            set
            {
                if (value.CanWrite == false)
                    throw new ArgumentException(
                        "must be writable. The property CanWrite must have a value equals to 'true'.", nameof(value));
                _log = value;
            }
        }

        /// <summary>
        ///     Gets or sets the timeout value in miliseconds when waiting for an FXP server to server transfer to complete.
        /// </summary>
        /// <remarks>
        ///     By default this timeout value is set to 600000 (10 minutes).  For large FXP file transfers you may need to
        ///     adjust this number higher.
        /// </remarks>
        public int FxpTransferTimeout { get; set; } = FXP_TRANSFER_TIMEOUT;

        /// <summary>
        ///     Gets the current directory path without sending having to send a request to the server.
        /// </summary>
        /// <seealso cref="GetWorkingDirectory" />
        public string CurrentDirectory { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Opens a connection to the remote FTP server and log in with user name and password credentials.
        /// </summary>
        /// <param name="user">User name.  Many public ftp allow for this value to 'anonymous'.</param>
        /// <param name="password">Password.  Anonymous public ftp servers generally require a valid email address for a password.</param>
        /// <remarks>Use the Close() method to log off and close the connection to the FTP server.</remarks>
        /// <seealso cref="OpenAsync" />
        /// <seealso cref="Close" />
        /// <seealso cref="Reopen" />
        public void Open(string user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "must have a value");

            if (user.Length == 0)
                throw new ArgumentException("must have a value", nameof(user));

            if (password == null)
                throw new ArgumentNullException(nameof(password), "must have a value or an empty string");

            // if the command connection is no already open then open a new command connect
            if (!IsConnected)
                OpenCommandConn();

            // test to see if this is an asychronous operation and if so make sure 
            // the user has not requested the operation to be canceled
            if (AsyncWorker != null && AsyncWorker.CancellationPending)
            {
                CloseAllConnections();
                return;
            }

            _user = user;
            _password = password;
            CurrentDirectory = "/";

            SendRequest(new FtpRequest(FtpCmd.User, user));

            // wait for user to log into system and all response messages to be transmitted
            Thread.Sleep(500);

            // test to see if this is an asychronous operation and if so make sure 
            // the user has not requested the operation to be canceled
            if (AsyncWorker != null && AsyncWorker.CancellationPending)
            {
                CloseAllConnections();
                return;
            }

            // some ftp servers do not require passwords for users and will log you in immediately - no password command is required
            if (LastResponse.Code != FtpResponseCode.UserLoggedIn)
            {
                SendRequest(new FtpRequest(FtpCmd.Pass, password));

                if (LastResponse.Code == FtpResponseCode.NotLoggedIn)
                    throw new FtpLoginException(
                        "Unable to log into FTP destination with supplied username and password.");
            }

            // test to see if this is an asychronous operation and if so make sure 
            // the user has not requested the operation to be canceled
            if (AsyncWorker != null && AsyncWorker.CancellationPending)
            {
                CloseAllConnections();
                return;
            }

            // if the custom item parser is not set then set to use the built-in generic parser
            if (ItemParser == null)
                ItemParser = new FtpGenericParser();

            //  set the file type used for transfers
            SetFileTransferType();

            // if compression is indicated then send the compression command
            if (IsCompressionEnabled)
                CompressionOn();

            // test to see if this is an asychronous operation and if so make sure 
            // the user has not requested the operation to be canceled
            if (AsyncWorker != null && AsyncWorker.CancellationPending)
            {
                CloseAllConnections();
                return;
            }

            _opened = true;
        }

        /// <summary>
        ///     Reopens a lost ftp connection.
        /// </summary>
        /// <remarks>
        ///     If the connection is currently open or the connection has never been open and FtpException is thrown.
        /// </remarks>
        public void Reopen()
        {
            if (!_opened)
                throw new FtpException("You must use the Open() method before using the Reopen() method.");

            // reopen the connection with the same username and password
            Open(_user, _password);
        }

        /// <summary>
        ///     Change the currently logged in user to another user on the FTP server.
        /// </summary>
        /// <param name="user">The name of user.</param>
        /// <param name="password">The password for the user.</param>
        public void ChangeUser(string user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "must have a value");

            if (user.Length == 0)
                throw new ArgumentException("must have a value", nameof(user));

            if (password == null)
                throw new ArgumentNullException(nameof(password), "must have a value");

            SendRequest(new FtpRequest(FtpCmd.User, user));

            // wait for user to log into system and all response messages to be transmitted
            Thread.Sleep(500);

            // test to see if this is an asychronous operation and if so make sure 
            // the user has not requested the operation to be canceled
            if (AsyncWorker != null && AsyncWorker.CancellationPending)
            {
                CloseAllConnections();
                return;
            }

            // some ftp servers do not require passwords for users and will log you in immediately - no password command is required
            if (LastResponse.Code != FtpResponseCode.UserLoggedIn)
            {
                SendRequest(new FtpRequest(FtpCmd.Pass, password));

                if (LastResponse.Code == FtpResponseCode.NotLoggedIn)
                    throw new FtpLoginException(
                        "Unable to log into FTP destination with supplied username and password.");
            }
        }

        /// <summary>
        ///     Closes connection to the FTP server.
        /// </summary>
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.ConnectionClosed" />
        /// <seealso cref="Reopen" />
        /// <seealso cref="Open" />
        public void Close()
        {
            CloseAllConnections();
        }

        /// <summary>
        ///     Changes the current working directory on older FTP servers that cannot handle a full path containing
        ///     multiple subdirectories.  This method will separate the full path into separate change directory commands
        ///     to support such systems.
        /// </summary>
        /// <param name="path">Path of the new directory to change to.</param>
        /// <remarks>Accepts both foward slash '/' and back slash '\' path names.</remarks>
        /// <seealso cref="ChangeDirectory" />
        /// <seealso cref="GetWorkingDirectory" />
        public void ChangeDirectoryMultiPath(string path)
        {
            // the change working dir command can generally handle all the weird directory name spaces
            // which is nice but frustrating that the ftp server implementors did not fix it for other commands

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("must have a value", nameof(path));

            // replace the windows style directory delimiter with a unix style delimiter
            path = path.Replace("\\", "/");

            string[] dirs = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

            // issue a single CWD command for each directory
            // this is a very reliable method to change directories on all FTP servers
            // because some systems do not all a full path to be specified when changing directories
            foreach (string dir in dirs)
            {
                SendRequest(new FtpRequest(FtpCmd.Cwd, dir));
            }
            CurrentDirectory = GetWorkingDirectory();
        }

        /// <summary>
        ///     Changes the current working directory on the server.
        /// </summary>
        /// <param name="path">Path of the new directory to change to.</param>
        /// <remarks>Accepts both foward slash '/' and back slash '\' path names.</remarks>
        /// <seealso cref="ChangeDirectory" />
        /// <seealso cref="GetWorkingDirectory" />
        public void ChangeDirectory(string path)
        {
            // the change working dir command can generally handle all the weird directory name spaces
            // which is nice but frustrating that the ftp server implementors did not fix it for other commands

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("must have a value", nameof(path));

            // replace the windows style directory delimiter with a unix style delimiter
            path = path.Replace("\\", "/");

            SendRequest(new FtpRequest(FtpCmd.Cwd, path));
            CurrentDirectory = GetWorkingDirectory();
        }


        /// <summary>
        ///     Gets the current working directory.
        /// </summary>
        /// <returns>A string value containing the current full working directory path on the FTP server.</returns>
        /// <seealso cref="ChangeDirectory" />
        /// <seealso cref="ChangeDirectoryUp" />
        public string GetWorkingDirectory()
        {
            SendRequest(new FtpRequest(FtpCmd.Pwd));

            //  now we have to fix the directory due to formatting
            //  most ftp servers send something like this:  257 "/awg/inbound" is current directory.
            string dir = LastResponse.Text;

            //  if the pwd is in quotes, then extract it
            if (dir.Substring(0, 1) == "\"")
                dir = dir.Substring(1, dir.IndexOf("\"", 1, StringComparison.Ordinal) - 1);

            return dir;
        }

        /// <summary>
        ///     Deletes a file on the remote FTP server.
        /// </summary>
        /// <param name="path">The path name of the file to delete.</param>
        /// <remarks>
        ///     The file is deleted in the current working directory if no path information
        ///     is specified.  Otherwise a full absolute path name can be specified.
        /// </remarks>
        /// <seealso cref="DeleteDirectory" />
        public void DeleteFile(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("must have a value", nameof(path));

            SendRequest(new FtpRequest(FtpCmd.Dele, path));
        }

        /// <summary>
        ///     Aborts an action such as transferring a file to or from the server.
        /// </summary>
        /// <remarks>
        ///     The abort command is sent up to the server signaling the server to abort the current activity.
        /// </remarks>
        public void Abort()
        {
            SendRequest(new FtpRequest(FtpCmd.Abor));
        }

        /// <summary>
        ///     Creates a new directory on the remote FTP server.
        /// </summary>
        /// <param name="path">The name of a new directory or an absolute path name for a new directory.</param>
        /// <remarks>
        ///     If a directory name is given for path then the server will create a new subdirectory
        ///     in the current working directory.  Optionally, a full absolute path may be given.
        /// </remarks>
        public void MakeDirectory(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("must contain a value", nameof(path));

            SendRequest(new FtpRequest(FtpCmd.Mkd, path));
        }

        /// <summary>
        ///     Moves a file on the remote FTP server from one directory to another.
        /// </summary>
        /// <param name="fromPath">Path and/or file name to be moved.</param>
        /// <param name="toPath">Destination path specifying the directory where the file will be moved to.</param>
        /// <remarks>
        ///     This method actuall results
        ///     in server FTP commands being issued to the server to perform the move.  This method is available for
        ///     your convenience when performing common tasks such as moving processed files out of a pick directory
        ///     and into a archive directory.
        /// </remarks>
        public void MoveFile(string fromPath, string toPath)
        {
            if (fromPath == null)
                throw new ArgumentNullException(nameof(fromPath));

            if (fromPath.Length == 0)
                throw new ArgumentException("must contain a value", nameof(fromPath));

            if (toPath == null)
                throw new ArgumentNullException(nameof(toPath));

            if (fromPath.Length == 0)
                throw new ArgumentException("must contain a value", nameof(toPath));

            //  retrieve the server file from the current working directory
            var fileStream = new MemoryStream();
            GetFile(fromPath, fileStream, false);

            //  create the remote file in the new location
            PutFile(fileStream, toPath, FileAction.Create);

            //  delete the original file from the original location
            DeleteFile(fromPath);
        }

        /// <summary>
        ///     Deletes a directory from the FTP server.
        /// </summary>
        /// <param name="path">Directory to delete.</param>
        /// <remarks>
        ///     The path can be either a specific subdirectory relative to the
        ///     current working directory on the server or an absolute path to
        ///     the directory to remove.
        /// </remarks>
        /// <seealso cref="DeleteFile" />
        public void DeleteDirectory(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("must have a value", nameof(path));

            SendRequest(new FtpRequest(FtpCmd.Rmd, path));
        }

        /// <summary>
        ///     Executes the specific help dialog on the FTP server.
        /// </summary>
        /// <returns>
        ///     A string contains the help dialog from the FTP server.
        /// </returns>
        /// <remarks>
        ///     Every FTP server supports a different set of commands and this commands
        ///     can be obtained by the FTP HELP command sent to the FTP server.  The information sent
        ///     back is not parsed or processed in any way by the FtpClient object.
        /// </remarks>
        public string GetHelp()
        {
            SendRequest(new FtpRequest(FtpCmd.Help));
            return LastResponse.Text;
        }

        /// <summary>
        ///     Retrieves the data and time for a specific file on the ftp server as a Coordinated Universal Time (UTC) value
        ///     (formerly known as GMT).
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="adjustToLocalTime">
        ///     Specifies if modified date and time as reported on the FTP server should be adjusted to
        ///     the local time zone with daylight savings on the client.
        /// </param>
        /// <returns>
        ///     A date time value.
        /// </returns>
        /// <remarks>
        ///     This function uses the MDTM command which is an additional feature command and therefore not supported
        ///     by all FTP servers.
        /// </remarks>
        /// <seealso cref="GetFileSize" />
        public DateTime GetFileDateTime(string fileName, bool adjustToLocalTime)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            if (fileName.Length == 0)
                throw new ArgumentException("must contain a value", nameof(fileName));

            try
            {
                SendRequest(new FtpRequest(FtpCmd.Mdtm, fileName));
            }
            catch (FtpException fex)
            {
                throw new FtpException(
                    $"An error occurred when retrieving file date and time for '{fileName}'.  Reason: {LastResponse.Text}", fex);
            }

            string response = LastResponse.Text;

            int year = int.Parse(response.Substring(0, 4), CultureInfo.InvariantCulture);
            int month = int.Parse(response.Substring(4, 2), CultureInfo.InvariantCulture);
            int day = int.Parse(response.Substring(6, 2), CultureInfo.InvariantCulture);
            int hour = int.Parse(response.Substring(8, 2), CultureInfo.InvariantCulture);
            int minute = int.Parse(response.Substring(10, 2), CultureInfo.InvariantCulture);
            int second = int.Parse(response.Substring(12, 2), CultureInfo.InvariantCulture);

            var dateUtc = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

            return adjustToLocalTime ? new DateTime(dateUtc.ToLocalTime().Ticks) : new DateTime(dateUtc.Ticks);
        }

        /// <summary>
        ///     Set the date and time for a specific file or directory on the server.
        /// </summary>
        /// <param name="path">The path or name of the file or directory.</param>
        /// <param name="dateTime">New date to set on the file or directory.</param>
        /// <remarks>
        ///     This function uses the MDTM command which is an additional feature command and therefore not supported
        ///     by all FTP servers.
        /// </remarks>
        /// <seealso cref="Rename" />
        public void SetDateTime(string path, DateTime dateTime)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("must have a value", nameof(path));

            // MDTM [YYMMDDHHMMSS] [filename]

            string dateTimeArg = dateTime.ToString("yyyyMMddHHmmss");

            try
            {
                SendRequest(new FtpRequest(FtpCmd.Mdtm, dateTimeArg, path));
            }
            catch (FtpException fex)
            {
                throw new FtpException(
                    $"An error occurred when setting a file date and time for '{path}'.", fex);
            }
        }

        /// <summary>
        ///     Retrieves the specific status for the FTP server.
        /// </summary>
        /// <remarks>
        ///     Each FTP server may return different status dialog information.  The status information sent
        ///     back is not parsed or processed in any way by the FtpClient object.
        /// </remarks>
        /// <returns>
        ///     A string containing the status of the FTP server.
        /// </returns>
        public string GetStatus()
        {
            try
            {
                SendRequest(new FtpRequest(FtpCmd.Stat));
            }
            catch (FtpException fex)
            {
                throw new FtpException(
                    $"An error occurred while getting the system status.  Reason: {LastResponse.Text}", fex);
            }

            return LastResponse.Text;
        }

        /// <summary>
        ///     Changes the current working directory on the FTP server to the parent directory.
        /// </summary>
        /// <remarks>
        ///     If there is no parent directory then ChangeDirectoryUp() will not have
        ///     any affect on the current working directory.
        /// </remarks>
        /// <seealso cref="ChangeDirectory" />
        /// <seealso cref="GetWorkingDirectory" />
        public void ChangeDirectoryUp()
        {
            SendRequest(new FtpRequest(FtpCmd.Cdup));
        }

        /// <summary>
        ///     Get the file size for a file on the remote FTP server.
        /// </summary>
        /// <param name="path">The name and/or path to the file.</param>
        /// <returns>An integer specifying the file size.</returns>
        /// <remarks>
        ///     The path can be file name relative to the current working directory or an absolute path.  This command is an
        ///     additional feature
        ///     that is not supported by all FTP servers.
        /// </remarks>
        /// <seealso cref="GetFileDateTime" />
        public int GetFileSize(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("must contain a value", nameof(path));

            try
            {
                SendRequest(new FtpRequest(FtpCmd.Size, path));
            }
            catch (FtpException fex)
            {
                throw new FtpException(
                    $"An error occurred when retrieving file size for {path}.  Reason: {LastResponse.Text}", fex);
            }

            return int.Parse(LastResponse.Text.Substring(4), CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Get the additional features supported by the remote FTP server.
        /// </summary>
        /// <returns>A string containing the additional features beyond the RFC 959 standard supported by the FTP server.</returns>
        /// <remarks>
        ///     This command is an additional feature beyond the REF 959 standard and therefore is not supported by all FTP
        ///     servers.
        /// </remarks>
        public string GetFeatures()
        {
            try
            {
                SendRequest(new FtpRequest(FtpCmd.Feat));
            }
            catch (FtpException fex)
            {
                throw new FtpException(
                    $"An error occurred when retrieving destination features.  Reason: {LastResponse.Text}", fex);
            }

            return LastResponse.Text;
        }

        /// <summary>
        ///     Retrieves the specific status for a file on the FTP server.
        /// </summary>
        /// <param name="path">
        ///     The path to the file.
        /// </param>
        /// <returns>
        ///     A string containing the status for the file.
        /// </returns>
        /// <remarks>
        ///     Each FTP server may return different status dialog information.  The status information sent
        ///     back is not parsed or processed in any way by the FtpClient object.
        /// </remarks>
        public string GetStatus(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("must contain a value", nameof(path));

            try
            {
                SendRequest(new FtpRequest(FtpCmd.Stat, path));
            }
            catch (FtpException fex)
            {
                throw new FtpException(
                    $"An error occurred when retrieving file status for file '{path}'.  Reason: {LastResponse.Text}", fex);
            }

            return LastResponse.Text;
        }

        /// <summary>
        ///     Allocates storage for a file on the FTP server prior to data transfer from the FTP client.
        /// </summary>
        /// <param name="size">
        ///     The storage size to allocate on the FTP server.
        /// </param>
        /// <remarks>
        ///     Some FTP servers may return the client to specify the storage size prior to data transfer from the FTP client to
        ///     the FTP server.
        /// </remarks>
        public void AllocateStorage(int size)
        {
            try
            {
                SendRequest(new FtpRequest(FtpCmd.Stor, size.ToString()));
            }
            catch (FtpException fex)
            {
                throw new FtpException(
                    $"An error occurred when trying to allocate storage on the destination.  Reason: {LastResponse.Text}", fex);
            }
        }

        /// <summary>
        ///     Retrieves a string identifying the remote FTP system.
        /// </summary>
        /// <returns>
        ///     A string contains the server type.
        /// </returns>
        /// <remarks>
        ///     The string contains the word "Type:", and the default transfer type
        ///     For example a UNIX FTP server will return 'UNIX Type: L8'.  A Windows
        ///     FTP server will return 'WINDOWS_NT'.
        /// </remarks>
        public string GetSystemType()
        {
            SendRequest(new FtpRequest(FtpCmd.Syst));
            return LastResponse.Text;
        }

        /// <summary>
        ///     Uploads a local file specified in the path parameter to the remote FTP server.
        /// </summary>
        /// <param name="localPath">Path to a file on the local machine.</param>
        /// <remarks>
        ///     The file is uploaded to the current working directory on the remote server.
        ///     A unique file name is created by the server.
        /// </remarks>
        public void PutFileUnique(string localPath)
        {
            if (localPath == null)
                throw new ArgumentNullException(nameof(localPath));

            using (FileStream fileStream = File.OpenRead(localPath))
            {
                PutFileUnique(fileStream);
            }
        }

        /// <summary>
        ///     Uploads any stream object to the remote FTP server and stores the data under a unique file name assigned by the FTP
        ///     server.
        /// </summary>
        /// <param name="inputStream">Any stream object on the local client machine.</param>
        /// <remarks>
        ///     The stream is uploaded to the current working directory on the remote server.
        ///     A unique file name is created by the server to store the data uploaded from the stream.
        /// </remarks>
        /// <seealso cref="PutFile(string, string, FileAction)" />
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        /// <seealso cref="GetFile(string, string)" />
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void PutFileUnique(Stream inputStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            if (!inputStream.CanRead)
                throw new ArgumentException("must be readable.  The CanRead property must return a value of 'true'.",
                    nameof(inputStream));

            try
            {
                TransferData(TransferDirection.ToServer, new FtpRequest(FtpCmd.Stor), inputStream);
            }
            catch (Exception ex)
            {
                throw new FtpException(
                    $"An error occurred while executing PutFileUnique() on the remote FTP destination. {ex}");
            }
        }

        /// <summary>
        ///     Retrieves a remote file from the FTP server and writes the data to a local file
        ///     specfied in the localPath parameter.  If the local file already exists a System.IO.IOException is thrown.
        /// </summary>
        /// <remarks>
        ///     To retrieve a remote file that you need to overwrite an existing file with or append to an existing file
        ///     see the method GetFile(string, string, FileAction).
        /// </remarks>
        /// <param name="remotePath">A path of the remote file.</param>
        /// <param name="localPath">A fully qualified local path to a file on the local machine.</param>
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="PutFile(string, string, FileAction)" />
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void GetFile(string remotePath, string localPath)
        {
            GetFile(remotePath, localPath, FileAction.CreateNew);
        }

        /// <summary>
        ///     Retrieves a remote file from the FTP server and writes the data to a local file
        ///     specfied in the localPath parameter.
        /// </summary>
        /// <param name="remotePath">A path and/or file name to the remote file.</param>
        /// <param name="localPath">A fully qualified local path to a file on the local machine.</param>
        /// <param name="action">The type of action to take.</param>
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="PutFile(string, string, FileAction)" />
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void GetFile(string remotePath, string localPath, FileAction action)
        {
            if (remotePath == null)
                throw new ArgumentNullException(nameof(remotePath));

            if (remotePath.Length == 0)
                throw new ArgumentException("must contain a value", nameof(remotePath));

            if (localPath == null)
                throw new ArgumentNullException(nameof(localPath));

            if (localPath.Length == 0)
                throw new ArgumentException("must contain a value", nameof(localPath));

            if (action == FileAction.None)
                throw new ArgumentOutOfRangeException(nameof(action), "must contain a value other than 'Unknown'");

            localPath = CorrectLocalPath(localPath);
            var request = new FtpRequest(FtpCmd.Retr, remotePath);

            try
            {
                switch (action)
                {
                    case FileAction.CreateNew:
                        // create a file stream to stream the file locally to disk that only creates the file if it does not already exist
                        using (Stream localFile = File.Open(localPath, FileMode.CreateNew))
                        {
                            TransferData(TransferDirection.ToClient, request, localFile);
                        }
                        break;

                    case FileAction.Create:
                        // create a file stream to stream the file locally to disk
                        using (Stream localFile = File.Open(localPath, FileMode.Create))
                        {
                            TransferData(TransferDirection.ToClient, request, localFile);
                        }
                        break;
                    case FileAction.CreateOrAppend:
                        // open the local file
                        using (Stream localFile = File.Open(localPath, FileMode.OpenOrCreate))
                        {
                            // set the file position to the end so that any new data will be appended                        
                            localFile.Position = localFile.Length;
                            TransferData(TransferDirection.ToClient, request, localFile);
                        }
                        break;
                    case FileAction.Resume:
                        using (Stream localFile = File.Open(localPath, FileMode.Open))
                        {
                            //  get the size of the file already on the server (in bytes)
                            long remoteSize = GetFileSize(remotePath);

                            // if the files are the same size then there is nothing to transfer
                            if (localFile.Length == remoteSize)
                                return;

                            TransferData(TransferDirection.ToClient, request, localFile, localFile.Length - 1);
                        }
                        break;
                    case FileAction.ResumeOrCreate:
                        if (File.Exists(localPath) && (new FileInfo(localPath)).Length > 0)
                            GetFile(remotePath, localPath, FileAction.Resume);
                        else
                            GetFile(remotePath, localPath, FileAction.Create);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new FtpException(
                    $"An unexpected exception occurred while retrieving file '{remotePath}. Error: {ex.Message}");
            }
        }

        /// <summary>
        ///     Retrieves a remote file from the FTP server and writes the data to a local stream object
        ///     specfied in the outStream parameter.
        /// </summary>
        /// <param name="remotePath">A path and/or file name to the remote file.</param>
        /// <param name="outStream">An output stream object used to stream the remote file to the local machine.</param>
        /// <param name="restart">
        ///     A true/false value to indicate if the file download needs to be restarted due to a previous
        ///     partial download.
        /// </param>
        /// <remarks>
        ///     If the remote path is a file name then the file is downloaded from the FTP server current working directory.
        ///     Otherwise a fully qualified
        ///     path for the remote file may be specified.  The output stream must be writeable and can be any stream object.
        ///     Finally, the restart parameter
        ///     is used to send a restart command to the FTP server.  The FTP server is instructed to restart the download process
        ///     at the last position of
        ///     of the output stream.  Not all FTP servers support the restart command.  If the FTP server does not support the
        ///     restart (REST) command,
        ///     an FtpException error is thrown.
        /// </remarks>
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="PutFile(string, string, FileAction)" />
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void GetFile(string remotePath, Stream outStream, bool restart)
        {
            if (remotePath == null)
                throw new ArgumentNullException(nameof(remotePath));

            if (remotePath.Length == 0)
                throw new ArgumentException("must contain a value", nameof(remotePath));

            if (outStream == null)
                throw new ArgumentNullException(nameof(outStream));

            if (outStream.CanWrite == false)
                throw new ArgumentException("must be writable.  The CanWrite property must return the value 'true'.",
                    nameof(outStream));

            var request = new FtpRequest(FtpCmd.Retr, remotePath);

            if (restart)
            {
                //  get the size of the file already on the server (in bytes)
                long remoteSize = GetFileSize(remotePath);

                // if the files are the same size then there is nothing to transfer
                if (outStream.Length == remoteSize)
                    return;

                TransferData(TransferDirection.ToClient, request, outStream, outStream.Length - 1);
            }
            else
            {
                TransferData(TransferDirection.ToClient, request, outStream);
            }
        }

        /// <summary>
        ///     Tests to see if a file or directory exists on the remote server.
        /// </summary>
        /// <param name="path">The full path to the remote file or directory.</param>
        /// <returns>A string containing the file listing from the current working directory.</returns>
        /// <remarks>
        ///     This method will execute a change working directory (CWD) command prior to testing to see if the file or
        ///     directory exits.  The original working directory will be changed back to the original value
        ///     after this method has completed.
        /// </remarks>
        public bool Exists(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("must have a value", nameof(path));

            // In order to test that a file exists we have to look for one of two conditions.  Some FTP servers will send a 550 error after
            // they go ahead and transmit zero length data.  This results in not FtpException being thrown.  Other servers will send a 450 error that the file
            // was not found and this will cause an FtpException to be thrown.

            string result;
            try
            {
                string origDir = CurrentDirectory;
                ChangeDirectory(path);
                result = GetNameList(path);
                ChangeDirectory(origDir);
            }
            catch (FtpException)
            {
                return false;
            }

            if (result.Length > 0)
                return true;
            return false;
        }


        /// <summary>
        ///     Retrieves a file name listing of the current working directory from the
        ///     remote FTP server using the NLST command.
        /// </summary>
        /// <returns>A string containing the file listing from the current working directory.</returns>
        /// <seealso cref="GetDirList(string)" />
        /// <seealso cref="GetDirListAsync(string)" />
        /// <seealso cref="GetDirListAsText(string)" />
        /// <seealso cref="GetDirListDeep" />
        /// <seealso cref="GetDirListDeepAsync(string)" />
        public string GetNameList()
        {
            return TransferText(new FtpRequest(FtpCmd.Nlst));
        }

        /// <summary>
        ///     Retrieves a file name listing of the current working directory from the
        ///     remote FTP server using the NLST command.
        /// </summary>
        /// <param name="path">The path to a directory on the remote FTP server.</param>
        /// <returns>A string containing the file listing from the current working directory.</returns>
        /// <seealso cref="GetDirListAsText(string)" />
        /// <seealso cref="GetDirList(string)" />
        /// <seealso cref="GetDirListAsync(string)" />
        /// <seealso cref="GetDirListAsText(string)" />
        /// <seealso cref="GetDirListDeep" />
        /// <seealso cref="GetDirListDeepAsync(string)" />
        public string GetNameList(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return TransferText(new FtpRequest(FtpCmd.Nlst, path));
        }


        /// <summary>
        ///     Retrieves a directory listing of the current working directory from the
        ///     remote FTP server using the LIST command.
        /// </summary>
        /// <returns>A string containing the directory listing of files from the current working directory.</returns>
        /// <seealso cref="GetDirList(string)" />
        /// <seealso cref="GetDirListAsync(string)" />
        /// <seealso cref="GetDirListDeep" />
        /// <seealso cref="GetDirListDeepAsync(string)" />
        /// <seealso cref="GetNameList(string)" />
        public string GetDirListAsText()
        {
            return TransferText(new FtpRequest(FtpCmd.List, "-al"));
        }

        /// <summary>
        ///     Retrieves a directory listing of the current working directory from the
        ///     remote FTP server using the LIST command.
        /// </summary>
        /// <param name="path">The path to a directory on the remote FTP server.</param>
        /// <returns>A string containing the directory listing of files from the current working directory.</returns>
        /// <seealso cref="GetDirList(string)" />
        /// <seealso cref="GetDirListAsync(string)" />
        /// <seealso cref="GetDirListDeep" />
        /// <seealso cref="GetDirListDeepAsync()" />
        /// <seealso cref="GetNameList()" />
        /// <seealso cref="GetNameList(string)" />
        public string GetDirListAsText(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return TransferText(new FtpRequest(FtpCmd.List, "-al", path));
        }

        /// <summary>
        ///     Retrieves a list of the files from current working directory on the remote FTP
        ///     server using the LIST command.
        /// </summary>
        /// <returns>FtpItemList collection object.</returns>
        /// <remarks>
        ///     This method returns a FtpItemList collection of FtpItem objects.
        /// </remarks>
        /// <seealso cref="GetDirListAsync(string)" />
        /// <seealso cref="GetDirListAsText(string)" />
        /// <seealso cref="GetDirListDeep" />
        /// <seealso cref="GetDirListDeepAsync(string)" />
        /// <seealso cref="GetNameList(string)" />
        public FtpItemCollection GetDirList()
        {
            return new FtpItemCollection(CurrentDirectory, TransferText(new FtpRequest(FtpCmd.List, "-al")),
                ItemParser);
        }

        /// <summary>
        ///     Retrieves a list of the files from a specified path on the remote FTP
        ///     server using the LIST command.
        /// </summary>
        /// <param name="path">The path to a directory on the remote FTP server.</param>
        /// <returns>FtpFileCollection collection object.</returns>
        /// <remarks>
        ///     This method returns a FtpFileCollection object containing a collection of
        ///     FtpItem objects.
        /// </remarks>
        /// <seealso cref="GetDirListAsync(string)" />
        /// <seealso cref="GetDirListAsText(string)" />
        /// <seealso cref="GetDirListDeep" />
        /// <seealso cref="GetDirListDeepAsync(string)" />
        /// <seealso cref="GetNameList(string)" />
        public FtpItemCollection GetDirList(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return new FtpItemCollection(path, TransferText(new FtpRequest(FtpCmd.List, "-al", path)), ItemParser);
        }

        /// <summary>
        ///     Deeply retrieves a list of all files and all sub directories from a specified path on the remote FTP
        ///     server using the LIST command.
        /// </summary>
        /// <param name="path">The path to a directory on the remote FTP server.</param>
        /// <returns>FtpFileCollection collection object.</returns>
        /// <remarks>
        ///     This method returns a FtpFileCollection object containing a collection of
        ///     FtpItem objects.
        /// </remarks>
        /// <seealso cref="GetDirListDeepAsync(string)" />
        /// <seealso cref="GetDirList(string)" />
        /// <seealso cref="GetDirListAsync(string)" />
        /// <seealso cref="GetDirListAsText(string)" />
        /// <seealso cref="GetNameList(string)" />
        public FtpItemCollection GetDirListDeep(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var deepCol = new FtpItemCollection();
            ParseDirListDeep(path, deepCol);
            return deepCol;
        }

        private void ParseDirListDeep(string path, FtpItemCollection deepCol)
        {
            FtpItemCollection itemCol = GetDirList(path);
            deepCol.Merge(itemCol);

            foreach (FtpItem item in itemCol)
            {
                // if the this call is being completed asynchronously and the user requests a cancellation
                // then stop processing the items and return
                if (AsyncWorker != null && AsyncWorker.CancellationPending)
                    return;

                // if the item is of type Directory then parse the directory list recursively
                if (item.ItemType == FtpItemType.Directory)
                    ParseDirListDeep(item.FullPath, deepCol);
            }
        }

        /// <summary>
        ///     Renames a file or directory on the remote FTP server.
        /// </summary>
        /// <param name="name">The name or absolute path of the file or directory you want to rename.</param>
        /// <param name="newName">The new name or absolute path of the file or directory.</param>
        /// <seealso cref="SetDateTime" />
        public void Rename(string name, string newName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "must have a value");

            if (name.Length == 0)
                throw new ArgumentException("must have a value", nameof(name));

            if (newName == null)
                throw new ArgumentNullException(nameof(newName), "must have a value");

            if (newName.Length == 0)
                throw new ArgumentException("must have a value", nameof(newName));

            SendRequest(new FtpRequest(FtpCmd.Rnfr, name));
            SendRequest(new FtpRequest(FtpCmd.Rnto, newName));
        }

        /// <summary>
        ///     Send a raw FTP command to the server.
        /// </summary>
        /// <param name="command">A string containing a valid FTP command value such as SYST.</param>
        /// <returns>The raw textual response from the server.</returns>
        /// <remarks>
        ///     This is an advanced feature of the FtpClient class that allows for any custom or specialized
        ///     FTP command to be sent to the FTP server.  Some FTP server support custom commands outside of
        ///     the standard FTP command list.  The following commands are not supported: PASV, RETR, STOR, and STRU.
        /// </remarks>
        /// <example>
        ///     <code>
        /// FtpClient ftp = new FtpClient("ftp.microsoft.com");
        /// ftp.Open("anonymous", "myemail@server.com");
        /// string response = ftp.Quote("SYST");
        /// System.Diagnostics.Debug.WriteLine(response);
        /// ftp.Close();
        /// </code>
        /// </example>
        public string Quote(string command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.Length < 3)
            {
                throw new ArgumentException($"Invalid command '{command}'.", nameof(command));
            }

            char[] separator = {' '};
            string[] values = command.Split(separator);

            // extract just the code value
            var code = values.Length == 0 ? command : values[0];

            // extract the arguments
            string args = string.Empty;
            if (command.Length > code.Length)
            {
                args = command.Replace(code, "").TrimStart();
            }

            FtpCmd ftpCmd;
            // Try to parse out the command if we can 
            if (Enum.TryParse(command, true, out ftpCmd))
            {
                if (ftpCmd == FtpCmd.Pasv || ftpCmd == FtpCmd.Retr || ftpCmd == FtpCmd.Stor || ftpCmd == FtpCmd.Stou ||
                    ftpCmd == FtpCmd.Erpt || ftpCmd == FtpCmd.Epsv)
                    throw new ArgumentException($"Command '{code}' not supported by Quote() method.",
                        nameof(command));

                if (ftpCmd == FtpCmd.List || ftpCmd == FtpCmd.Nlst)
                    return TransferText(new FtpRequest(ftpCmd, args));
            }
            else
            {
                ftpCmd = FtpCmd.Unknown;
            }

            SendRequest(ftpCmd == FtpCmd.Unknown ? new FtpRequest(ftpCmd, command) : new FtpRequest(ftpCmd, args));
            return LastResponseList.GetRawText();
        }

        /// <summary>
        ///     Sends a NOOP or no operation command to the FTP server.  This can be used to prevent some servers from logging out
        ///     the
        ///     interactive session during file transfer process.
        /// </summary>
        public void NoOperation()
        {
            SendRequest(new FtpRequest(FtpCmd.Noop));
        }

        /// <summary>
        ///     Issues a site specific change file mode (CHMOD) command to the server.  Not all servers implement this command.
        /// </summary>
        /// <param name="path">The path to the file or directory you want to change the mode on.</param>
        /// <param name="octalValue">The CHMOD octal value.</param>
        /// <remarks>
        ///     Common CHMOD values used on web servers.
        ///     Value 	User 	Group 	Other
        ///     755 	rwx 	r-x 	r-x
        ///     744 	rwx 	r--	    r--
        ///     766 	rwx 	rw- 	rw-
        ///     777 	rwx 	rwx 	rwx
        /// </remarks>
        /// <seealso cref="SetDateTime" />
        /// <seealso cref="Rename" />
        public void ChangeMode(string path, int octalValue)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("must have a value", nameof(path));

            SendRequest(new FtpRequest(FtpCmd.Site, "CHMOD", octalValue.ToString(), path));

            if (LastResponse.Code == FtpResponseCode.CommandNotImplementedSuperfluousAtThisSite)
                throw new FtpException($"Unable to the change file mode for file {path}.  Reason: {LastResponse.Text}");
        }

        /// <summary>
        ///     Issue a SITE command to the FTP server for site specific implementation commands.
        /// </summary>
        /// <param name="argument">One or more command arguments</param>
        /// <remarks>
        ///     For example, the CHMOD command is issued as a SITE command.
        /// </remarks>
        public void Site(string argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument), "must have a value");

            if (argument.Length == 0)
                throw new ArgumentException("must have a value", nameof(argument));

            SendRequest(new FtpRequest(FtpCmd.Site, argument));
        }

        /// <summary>
        ///     Uploads a local file specified in the path parameter to the remote FTP server.
        /// </summary>
        /// <param name="localPath">Path to a file on the local machine.</param>
        /// <param name="remotePath">Filename or full path to file on the remote FTP server.</param>
        /// <param name="action">The type of put action taken.</param>
        /// <remarks>
        ///     The file is uploaded to the current working directory on the remote server.  The remotePath
        ///     parameter is used to specify the path and file name used to store the file on the remote server.
        /// </remarks>
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        /// <seealso cref="PutFileUnique(string)" />
        /// <seealso cref="GetFile(string, string, FileAction)" />
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void PutFile(string localPath, string remotePath, FileAction action)
        {
            using (FileStream fileStream = File.OpenRead(localPath))
            {
                PutFile(fileStream, remotePath, action);
            }
        }

        /// <summary>
        ///     Uploads a local file specified in the path parameter to the remote FTP server.
        ///     An FtpException is thrown if the file already exists.
        /// </summary>
        /// <param name="localPath">Path to a file on the local machine.</param>
        /// <param name="remotePath">Filename or full path to file on the remote FTP server.</param>
        /// <remarks>
        ///     The file is uploaded to the current working directory on the remote server.  The remotePath
        ///     parameter is used to specify the path and file name used to store the file on the remote server.
        ///     To overwrite an existing file see the method PutFile(string, string, FileAction) and specify the
        ///     FileAction Create.
        /// </remarks>
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        /// <seealso cref="PutFileUnique(string)" />
        /// <seealso cref="GetFile(string, string, FileAction)" />
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void PutFile(string localPath, string remotePath)
        {
            using (FileStream fileStream = File.OpenRead(localPath))
            {
                PutFile(fileStream, remotePath, FileAction.CreateNew);
            }
        }

        /// <summary>
        ///     Uploads a local file specified in the path parameter to the remote FTP server.
        /// </summary>
        /// <param name="localPath">Path to a file on the local machine.</param>
        /// <param name="action">The type of put action taken.</param>
        /// <remarks>
        ///     The file is uploaded to the current working directory on the remote server.
        /// </remarks>
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        /// <seealso cref="PutFileUnique(string)" />
        /// <seealso cref="GetFile(string, string, FileAction)" />
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void PutFile(string localPath, FileAction action)
        {
            using (FileStream fileStream = File.OpenRead(localPath))
            {
                PutFile(fileStream, ExtractPathItemName(localPath), action);
            }
        }

        /// <summary>
        ///     Uploads a local file specified in the path parameter to the remote FTP server.
        ///     An FtpException is thrown if the file already exists.
        /// </summary>
        /// <param name="localPath">Path to a file on the local machine.</param>
        /// <remarks>
        ///     The file is uploaded to the current working directory on the remote server.
        /// </remarks>
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        /// <seealso cref="PutFileUnique(string)" />
        /// <seealso cref="GetFile(string, string, FileAction)" />
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void PutFile(string localPath)
        {
            using (FileStream fileStream = File.OpenRead(localPath))
            {
                PutFile(fileStream, ExtractPathItemName(localPath), FileAction.CreateNew);
            }
        }

        /// <summary>
        ///     Uploads stream data specified in the inputStream parameter to the remote FTP server.
        /// </summary>
        /// <param name="inputStream">Any open stream object on the local client machine.</param>
        /// <param name="remotePath">Filename or path and filename of the file stored on the remote FTP server.</param>
        /// <param name="action">The type of put action taken.</param>
        /// <remarks>
        ///     The stream is uploaded to the current working directory on the remote server.  The remotePath
        ///     parameter is used to specify the path and file name used to store the file on the remote server.
        /// </remarks>
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        /// <seealso cref="PutFileUnique(string)" />
        /// <seealso cref="GetFile(string, string, FileAction)" />
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void PutFile(Stream inputStream, string remotePath, FileAction action)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            if (!inputStream.CanRead)
                throw new ArgumentException("must be readable", nameof(inputStream));

            if (remotePath == null)
                throw new ArgumentNullException(nameof(remotePath));

            if (remotePath.Length == 0)
                throw new ArgumentException("must contain a value", nameof(remotePath));

            if (action == FileAction.None)
                throw new ArgumentOutOfRangeException(nameof(action), "must contain a value other than 'Unknown'");

            switch (action)
            {
                case FileAction.CreateOrAppend:
                    TransferData(TransferDirection.ToServer, new FtpRequest(FtpCmd.Appe, remotePath),
                        inputStream);
                    break;
                case FileAction.CreateNew:
                    if (Exists(remotePath))
                    {
                        throw new FtpException("Can not overwrite existing file.");
                    }
                    TransferData(TransferDirection.ToServer, new FtpRequest(FtpCmd.Stor, remotePath),
                        inputStream);
                    break;
                case FileAction.Create:
                    TransferData(TransferDirection.ToServer, new FtpRequest(FtpCmd.Stor, remotePath),
                        inputStream);
                    break;
                case FileAction.Resume:
                    //  get the size of the file already on the server (in bytes)
                    long remoteSize = GetFileSize(remotePath);

                    //  if the files are the same size then there is nothing to transfer
                    if (remoteSize == inputStream.Length)
                        return;

                    //  transfer file to the server
                    TransferData(TransferDirection.ToServer, new FtpRequest(FtpCmd.Stor, remotePath),
                        inputStream, remoteSize);
                    break;
                case FileAction.ResumeOrCreate:
                    PutFile(inputStream, remotePath, Exists(remotePath) ? FileAction.Resume : FileAction.Create);
                    break;
            }
        }

        /// <summary>
        ///     File Exchange Protocol (FXP) allows server-to-server transfer which can greatly speed up file transfers.
        /// </summary>
        /// <param name="fileName">The name of the file to transfer.</param>
        /// <param name="destination">The destination FTP server which must be supplied as an open and connected FtpClient object.</param>
        /// <remarks>
        ///     Both servers must support and have FXP enabled before you can transfer files between two remote servers using FXP.
        ///     One FTP server must support PASV mode and the other server must allow PORT commands from a foreign address.
        ///     Finally, firewall settings may interfer with the ability of one server to access the other server.
        ///     Starksoft FtpClient will coordinate the FTP negoitaion and necessary PORT and PASV transfer commands.
        /// </remarks>
        /// <seealso cref="FxpTransferTimeout" />
        /// <seealso cref="FxpCopyAsync" />
        public void FxpCopy(string fileName, FtpClient destination)
        {
            if (IsConnected == false)
                throw new FtpException(
                    "The connection must be open before a transfer between servers can be intitiated.");

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (destination.IsConnected == false)
                throw new FtpException(
                    "The destination object must be open and connected before a transfer between servers can be intitiated.");

            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            if (fileName.Length == 0)
                throw new ArgumentException("must have a value", nameof(fileName));


            //  send command to destination FTP server to get passive port to be used from the source FTP server
            try
            {
                destination.SendRequest(new FtpRequest(FtpCmd.Pasv));
            }
            catch (FtpException fex)
            {
                throw new FtpException(
                    string.Format(
                        "An error occurred when trying to set up the passive connection on '{1}' for a destination to destination copy between '{0}' and '{1}'.",
                        Host, destination.Host), fex);
            }

            //  get the begin and end positions to extract data from the response string
            int startIdx = destination.LastResponse.Text.IndexOf("(", StringComparison.Ordinal) + 1;
            int endIdx = destination.LastResponse.Text.IndexOf(")", StringComparison.Ordinal);
            string dataPortInfo = destination.LastResponse.Text.Substring(startIdx, endIdx - startIdx);

            //  send a command to the source server instructing it to connect to
            //  the local ip address and port that the destination server will be bound to
            try
            {
                SendRequest(new FtpRequest(FtpCmd.Port, dataPortInfo));
            }
            catch (FtpException fex)
            {
                throw new FtpException(
                    $"Command instructing '{Host}' to open connection failed.", fex);
            }

            // send command to tell the source server to retrieve the file from the destination server
            try
            {
                SendRequest(new FtpRequest(FtpCmd.Retr, fileName));
            }
            catch (FtpException fex)
            {
                throw new FtpException(
                    $"An error occurred transfering on a server to server copy between '{Host}' and '{destination.Host}'.", fex);
            }

            // send command to tell the destination to store the file
            try
            {
                destination.SendRequest(new FtpRequest(FtpCmd.Stor, fileName));
            }
            catch (FtpException fex)
            {
                throw new FtpException(
                    $"An error occurred transfering on a server to server copy between '{Host}' and '{destination.Host}'.", fex);
            }

            // wait until we get a file completed response back from the destination server and the source server
            destination.WaitForHappyCodes(FxpTransferTimeout, FtpResponseCode.RequestedFileActionOkayAndCompleted,
                FtpResponseCode.ClosingDataConnection);
            WaitForHappyCodes(FxpTransferTimeout, FtpResponseCode.RequestedFileActionOkayAndCompleted,
                FtpResponseCode.ClosingDataConnection);
        }

        #endregion

        #region Private Methods

        private string CorrectLocalPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("must have a value", nameof(path));

            string[] fileName = {ExtractPathItemName(path)};
            string pathOnly = path.Substring(0, path.Length - fileName[0].Length - 1);

            // if the pathOnly portion contains the root node then we need to add the 
            // a directory slash back otherwise the final combined path will be something
            // like c:myfile.txt and this will result
            if (pathOnly.EndsWith(":") && pathOnly.IndexOf("\\", StringComparison.Ordinal) == -1)
            {
                pathOnly += "\\";
            }

            char[] invalidPath = Path.GetInvalidPathChars();
            if (path.IndexOfAny(invalidPath) != -1)
            {
                foreach (char c in invalidPath)
                {
                    if (pathOnly.IndexOf(c) != -1)
                        pathOnly = pathOnly.Replace(c, '_');
                }
            }

            char[] invalidFile = Path.GetInvalidFileNameChars();
            if (fileName[0].IndexOfAny(invalidFile) == -1)
                return Path.Combine(pathOnly, fileName[0]);
            foreach (char c in invalidFile.Where(c => fileName[0].IndexOf(c) != -1))
            {
                fileName[0] = fileName[0].Replace(c, '_');
            }

            return Path.Combine(pathOnly, fileName[0]);
        }

        private void SetFileTransferType()
        {
            switch (_fileTransferType)
            {
                case TransferType.Binary:
                    SendRequest(new FtpRequest(FtpCmd.Type, "I"));
                    break;
                case TransferType.Ascii:
                    SendRequest(new FtpRequest(FtpCmd.Type, "A"));
                    break;
            }
        }


        private string ExtractPathItemName(string path)
        {
            if (path.IndexOf("\\", StringComparison.Ordinal) != -1)
                return path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            if (path.IndexOf("/", StringComparison.Ordinal) != -1)
                return path.Substring(path.LastIndexOf("/", StringComparison.Ordinal) + 1);
            if (path.Length > 0)
                return path;
            throw new FtpException(string.Format(CultureInfo.InvariantCulture, "Item name not found in path {0}.",
                path));
        }

        #endregion

        #region  Asynchronous Methods and Events

        private Exception _asyncException;

        ////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     Event handler for GetDirListAsync method.
        /// </summary>
        /// <seealso cref="GetDirListAsync(string)" />
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        public event EventHandler<GetDirListAsyncCompletedEventArgs> GetDirListAsyncCompleted;

        /// <summary>
        ///     Asynchronously retrieves a list of the files from current working directory on the remote FTP
        ///     server using the LIST command.
        /// </summary>
        /// <remarks>
        ///     This method returns a FtpItemList collection of FtpItem objects through the GetDirListAsyncCompleted event.
        /// </remarks>
        /// <seealso cref="GetDirListAsyncCompleted" />
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        /// <seealso cref="GetDirList(string)" />
        /// <seealso cref="GetDirListDeep" />
        /// <seealso cref="GetDirListDeepAsync(string)" />
        /// <seealso cref="GetDirListAsText(string)" />
        /// <seealso cref="GetNameList(string)" />
        public void GetDirListAsync()
        {
            GetDirListAsync(string.Empty);
        }

        /// <summary>
        ///     Asynchronously retrieves a list of the files from a specified path on the remote FTP
        ///     server using the LIST command.
        /// </summary>
        /// <param name="path">The path to a directory on the remote FTP server.</param>
        /// <remarks>
        ///     This method returns a FtpFileCollection object containing a collection of
        ///     FtpItem objects.  The FtpFileCollection is returned though the GetDirListAsyncCompleted event.
        /// </remarks>
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        /// <seealso cref="GetDirList(string)" />
        /// <seealso cref="GetDirListDeep" />
        /// <seealso cref="GetDirListDeepAsync(string)" />
        /// <seealso cref="GetDirListAsText(string)" />
        /// <seealso cref="GetNameList(string)" />
        public void GetDirListAsync(string path)
        {
            if (AsyncWorker != null && AsyncWorker.IsBusy)
                throw new InvalidOperationException(
                    "The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            if (AsyncWorker == null)
                throw new ArgumentException("AsyncWorker");

            AsyncWorker.WorkerSupportsCancellation = true;
            AsyncWorker.DoWork += GetDirListAsync_DoWork;
            AsyncWorker.RunWorkerCompleted += GetDirListAsync_RunWorkerCompleted;
            AsyncWorker.RunWorkerAsync(path);
        }

        private void GetDirListAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var path = (string) e.Argument;
                e.Result = GetDirList(path);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        private void GetDirListAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (GetDirListAsyncCompleted != null)
                GetDirListAsyncCompleted(this,
                    new GetDirListAsyncCompletedEventArgs(_asyncException, IsAsyncCanceled,
                        (FtpItemCollection) e.Result));
            _asyncException = null;
        }

        /// <summary>
        ///     Event handler for GetDirListDeepAsync method.
        /// </summary>
        public event EventHandler<GetDirListDeepAsyncCompletedEventArgs> GetDirListDeepAsyncCompleted;

        /// <summary>
        ///     Asynchronous deep retrieval of a list of all files and all sub directories from the current path on the remote FTP
        ///     server using the LIST command.
        /// </summary>
        /// <remarks>
        ///     This method returns a FtpFileCollection object containing a collection of FtpItem objects through the
        ///     GetDirListDeepAsyncCompleted event.
        /// </remarks>
        /// <seealso cref="GetDirListDeepAsyncCompleted" />
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        /// <seealso cref="GetDirListDeep" />
        /// <seealso cref="GetDirList(string)" />
        /// <seealso cref="GetDirListAsync(string)" />
        /// <seealso cref="GetDirListAsText(string)" />
        /// <seealso cref="GetNameList(string)" />
        public void GetDirListDeepAsync()
        {
            GetDirListDeepAsync(GetWorkingDirectory());
        }

        /// <summary>
        ///     Asynchronous deep retrieval of a list of all files and all sub directories from a specified path on the remote FTP
        ///     server using the LIST command.
        /// </summary>
        /// <param name="path">The path to a directory on the remote FTP server.</param>
        /// <remarks>
        ///     This method returns a FtpFileCollection object containing a collection of
        ///     FtpItem objects the GetDirListDeepAsyncCompleted event.
        /// </remarks>
        /// <seealso cref="GetDirListDeepAsyncCompleted" />
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        /// <seealso cref="GetDirListDeep" />
        /// <seealso cref="GetDirList(string)" />
        /// <seealso cref="GetDirListAsync(string)" />
        /// <seealso cref="GetDirListAsText(string)" />
        public void GetDirListDeepAsync(string path)
        {
            if (AsyncWorker != null && AsyncWorker.IsBusy)
                throw new InvalidOperationException(
                    "The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            if (AsyncWorker == null)
                throw new ArgumentException("AsyncWorker");

            AsyncWorker.WorkerSupportsCancellation = true;
            AsyncWorker.DoWork += GetDirListDeepAsync_DoWork;
            AsyncWorker.RunWorkerCompleted +=
                GetDirListDeepAsync_RunWorkerCompleted;
            AsyncWorker.RunWorkerAsync(path);
        }

        private void GetDirListDeepAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var path = (string) e.Argument;
                e.Result = GetDirList(path);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        private void GetDirListDeepAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (GetDirListDeepAsyncCompleted != null)
                GetDirListDeepAsyncCompleted(this,
                    new GetDirListDeepAsyncCompletedEventArgs(_asyncException, IsAsyncCanceled,
                        (FtpItemCollection) e.Result));
            _asyncException = null;
        }


        ////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     Event that fires when the GetFileAsync method is invoked.
        /// </summary>
        public event EventHandler<GetFileAsyncCompletedEventArgs> GetFileAsyncCompleted;

        /// <summary>
        ///     Asynchronously retrieves a remote file from the FTP server and writes the data to a local file
        ///     specfied in the localPath parameter.
        /// </summary>
        /// <param name="remotePath">A path and/or file name to the remote file.</param>
        /// <param name="localPath">A fully qualified local path to a file on the local machine.</param>
        /// <param name="action">The type of action to take.</param>
        /// <seealso cref="GetFileAsyncCompleted" />
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        /// <seealso cref="GetFile(string, string, FileAction)" />
        /// <seealso cref="PutFile(string)" />
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void GetFileAsync(string remotePath, string localPath, FileAction action)
        {
            if (AsyncWorker != null && AsyncWorker.IsBusy)
                throw new InvalidOperationException(
                    "The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            if (AsyncWorker == null)
                throw new ArgumentException("AsyncWorker");

            AsyncWorker.WorkerSupportsCancellation = true;
            AsyncWorker.DoWork += GetFileAsync_DoWork;
            AsyncWorker.RunWorkerCompleted += GetFileAsync_RunWorkerCompleted;
            var args = new object[3];
            args[0] = remotePath;
            args[1] = localPath;
            args[2] = action;
            AsyncWorker.RunWorkerAsync(args);
        }

        private void GetFileAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var args = (object[]) e.Argument;
                GetFile((string) args[0], (string) args[1], (FileAction) args[2]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        private void GetFileAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (GetFileAsyncCompleted != null)
                GetFileAsyncCompleted(this, new GetFileAsyncCompletedEventArgs(_asyncException, IsAsyncCanceled));
            _asyncException = null;
        }

        /// <summary>
        ///     Asynchronously retrieves a remote file from the FTP server and writes the data to a local stream object
        ///     specfied in the outStream parameter.
        /// </summary>
        /// <param name="remotePath">A path and/or file name to the remote file.</param>
        /// <param name="outStream">An output stream object used to stream the remote file to the local machine.</param>
        /// <param name="restart">
        ///     A true/false value to indicate if the file download needs to be restarted due to a previous
        ///     partial download.
        /// </param>
        /// <remarks>
        ///     If the remote path is a file name then the file is downloaded from the FTP server current working directory.
        ///     Otherwise a fully qualified
        ///     path for the remote file may be specified.  The output stream must be writeable and can be any stream object.
        ///     Finally, the restart parameter
        ///     is used to send a restart command to the FTP server.  The FTP server is instructed to restart the download process
        ///     at the last position of
        ///     of the output stream.  Not all FTP servers support the restart command.  If the FTP server does not support the
        ///     restart (REST) command,
        ///     an FtpException error is thrown.
        /// </remarks>
        /// <seealso cref="GetFileAsyncCompleted" />
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        /// <seealso cref="GetFile(string, string)" />
        /// <seealso cref="PutFile(string, string, FileAction)" />
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void GetFileAsync(string remotePath, Stream outStream, bool restart)
        {
            if (AsyncWorker != null && AsyncWorker.IsBusy)
                throw new InvalidOperationException(
                    "The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            if (AsyncWorker == null)
                throw new ArgumentException("AsyncWorker");

            AsyncWorker.WorkerSupportsCancellation = true;
            AsyncWorker.DoWork += GetFileStreamAsync_DoWork;
            AsyncWorker.RunWorkerCompleted += GetFileAsync_RunWorkerCompleted;
            var args = new object[3];
            args[0] = remotePath;
            args[1] = outStream;
            args[2] = restart;
            AsyncWorker.RunWorkerAsync(args);
        }

        private void GetFileStreamAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var args = (object[]) e.Argument;
                GetFile((string) args[0], (Stream) args[1], (bool) args[2]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     Asynchronous event for PutFileAsync method.
        /// </summary>
        /// <seealso cref="PutFileAsync(string, string, FileAction)" />
        public event EventHandler<PutFileAsyncCompletedEventArgs> PutFileAsyncCompleted;

        /// <summary>
        ///     Asynchronously uploads a local file specified in the path parameter to the remote FTP server.
        /// </summary>
        /// <param name="localPath">Path to a file on the local machine.</param>
        /// <param name="remotePath">Filename or full path to file on the remote FTP server.</param>
        /// <param name="action">The type of put action taken.</param>
        /// <remarks>
        ///     The file is uploaded to the current working directory on the remote server.  The remotePath
        ///     parameter is used to specify the path and file name used to store the file on the remote server.
        /// </remarks>
        /// <seealso cref="PutFileAsyncCompleted" />
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        /// <seealso cref="PutFile(string, string, FileAction)" />
        /// <seealso cref="PutFileUnique(string)" />
        /// <seealso cref="GetFile(string, string, FileAction)" />
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void PutFileAsync(string localPath, string remotePath, FileAction action)
        {
            if (AsyncWorker != null && AsyncWorker.IsBusy)
                throw new InvalidOperationException(
                    "The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            if (AsyncWorker == null)
                throw new ArgumentException("AsyncWorker");

            AsyncWorker.WorkerSupportsCancellation = true;
            AsyncWorker.DoWork += PutFileAsync_DoWork;
            AsyncWorker.RunWorkerCompleted += PutFileAsync_RunWorkerCompleted;
            var args = new object[3];
            args[0] = localPath;
            args[1] = remotePath;
            args[2] = action;
            AsyncWorker.RunWorkerAsync(args);
        }

        private void PutFileAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var args = (object[]) e.Argument;
                PutFile((string) args[0], (string) args[1], (FileAction) args[2]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        private void PutFileAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (PutFileAsyncCompleted != null)
                PutFileAsyncCompleted(this, new PutFileAsyncCompletedEventArgs(_asyncException, IsAsyncCanceled));
            _asyncException = null;
        }

        /// <summary>
        ///     Asynchronously uploads stream data specified in the inputStream parameter to the remote FTP server.
        /// </summary>
        /// <param name="inputStream">Any open stream object on the local client machine.</param>
        /// <param name="remotePath">Filename or path and filename of the file stored on the remote FTP server.</param>
        /// <param name="action">The type of put action taken.</param>
        /// <remarks>
        ///     The stream is uploaded to the current working directory on the remote server.  The remotePath
        ///     parameter is used to specify the path and file name used to store the file on the remote server.
        /// </remarks>
        /// <seealso cref="PutFileAsyncCompleted" />
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        /// <seealso cref="PutFile(string, string, FileAction)" />
        /// <seealso cref="GetFile(string, string, FileAction)" />
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void PutFileAsync(Stream inputStream, string remotePath, FileAction action)
        {
            if (AsyncWorker != null && AsyncWorker.IsBusy)
                throw new InvalidOperationException(
                    "The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            if (AsyncWorker == null)
                throw new ArgumentException("AsyncWorker");

            AsyncWorker.WorkerSupportsCancellation = true;
            AsyncWorker.DoWork += PutFileStreamAsync_DoWork;
            AsyncWorker.RunWorkerCompleted += PutFileAsync_RunWorkerCompleted;
            var args = new object[3];
            args[0] = inputStream;
            args[1] = remotePath;
            args[2] = action;
            AsyncWorker.RunWorkerAsync(args);
        }

        private void PutFileStreamAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var args = (object[]) e.Argument;
                PutFile((Stream) args[0], (string) args[1], (FileAction) args[2]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        /// <summary>
        ///     Asynchronously uploads a local file specified in the path parameter to the remote FTP server.
        /// </summary>
        /// <param name="localPath">Path to a file on the local machine.</param>
        /// <param name="action">The type of put action taken.</param>
        /// <remarks>
        ///     The file is uploaded to the current working directory on the remote server.
        /// </remarks>
        /// <seealso cref="PutFileAsyncCompleted" />
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        /// <seealso cref="PutFile(string, string, FileAction)" />
        /// <seealso cref="PutFileUnique(string)" />
        /// <seealso cref="GetFile(string, string, FileAction)" />
        /// <seealso cref="GetFileAsync(string, string, FileAction)" />
        /// <seealso cref="MoveFile" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="FxpCopyAsync" />
        public void PutFileAsync(string localPath, FileAction action)
        {
            if (AsyncWorker != null && AsyncWorker.IsBusy)
                throw new InvalidOperationException(
                    "The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            if (AsyncWorker == null)
                throw new ArgumentException("AsyncWorker");

            AsyncWorker.WorkerSupportsCancellation = true;
            AsyncWorker.DoWork += PutFileLocalAsync_DoWork;
            AsyncWorker.RunWorkerCompleted += PutFileAsync_RunWorkerCompleted;
            var args = new object[2];
            args[0] = localPath;
            args[1] = action;
            AsyncWorker.RunWorkerAsync(args);
        }

        private void PutFileLocalAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var args = (object[]) e.Argument;
                PutFile((string) args[0], (FileAction) args[1]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     Event handler for OpenAsync method.
        /// </summary>
        public event EventHandler<OpenAsyncCompletedEventArgs> OpenAsyncCompleted;

        /// <summary>
        ///     Asynchronously opens a connection to the remote FTP server and log in with user name and password credentials.
        /// </summary>
        /// <param name="user">User name.  Many public ftp allow for this value to 'anonymous'.</param>
        /// <param name="password">Password.  Anonymous public ftp servers generally require a valid email address for a password.</param>
        /// <remarks>Use the Close() method to log off and close the connection to the FTP server.</remarks>
        /// <seealso cref="OpenAsyncCompleted" />
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        /// <seealso cref="Open" />
        /// <seealso cref="Reopen" />
        /// <seealso cref="Close" />
        public void OpenAsync(string user, string password)
        {
            if (AsyncWorker != null && AsyncWorker.IsBusy)
                throw new InvalidOperationException(
                    "The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            if (AsyncWorker == null)
                throw new ArgumentException("AsyncWorker");

            AsyncWorker.WorkerSupportsCancellation = true;
            AsyncWorker.DoWork += OpenAsync_DoWork;
            AsyncWorker.RunWorkerCompleted += OpenAsync_RunWorkerCompleted;
            var args = new object[2];
            args[0] = user;
            args[1] = password;
            AsyncWorker.RunWorkerAsync(args);
        }

        private void OpenAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var args = (object[]) e.Argument;
                Open((string) args[0], (string) args[1]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        private void OpenAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OpenAsyncCompleted != null)
                OpenAsyncCompleted(this, new OpenAsyncCompletedEventArgs(_asyncException, IsAsyncCanceled));
            _asyncException = null;
        }


        ////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     Asynchronous event for FxpCopyAsync method.
        /// </summary>
        public event EventHandler<FxpCopyAsyncCompletedEventArgs> FxpCopyAsyncCompleted;

        /// <summary>
        ///     Asynchronous File Exchange Protocol (FXP) allows server-to-server transfer which can greatly speed up file
        ///     transfers.
        /// </summary>
        /// <param name="fileName">The name of the file to transfer.</param>
        /// <param name="destination">The destination FTP server which must be supplied as an open and connected FtpClient object.</param>
        /// <remarks>
        ///     Both servers must support and have FXP enabled before you can transfer files between two remote servers using FXP.
        ///     One FTP server must support PASV mode and the other server must allow PORT commands from a foreign address.
        ///     Finally, firewall settings may interfer with the ability of one server to access the other server.
        ///     Starksoft FtpClient will coordinate the FTP negoitaion and necessary PORT and PASV transfer commands.
        /// </remarks>
        /// <seealso cref="FxpCopyAsyncCompleted" />
        /// <seealso cref="FxpTransferTimeout" />
        /// <seealso cref="FxpCopy" />
        /// <seealso cref="nUpdate.Administration.Core.Ftp.FtpBase.CancelAsync" />
        public void FxpCopyAsync(string fileName, FtpClient destination)
        {
            if (AsyncWorker != null && AsyncWorker.IsBusy)
                throw new InvalidOperationException(
                    "The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            if (AsyncWorker == null)
                throw new ArgumentException("AsyncWorker");

            AsyncWorker.WorkerSupportsCancellation = true;
            AsyncWorker.DoWork += FxpCopyAsync_DoWork;
            AsyncWorker.RunWorkerCompleted += FxpCopyAsync_RunWorkerCompleted;
            var args = new object[2];
            args[0] = fileName;
            args[1] = destination;
            AsyncWorker.RunWorkerAsync(args);
        }

        private void FxpCopyAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var args = (object[]) e.Argument;
                FxpCopy((string) args[0], (FtpClient) args[1]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        private void FxpCopyAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (FxpCopyAsyncCompleted != null)
                FxpCopyAsyncCompleted(this, new FxpCopyAsyncCompletedEventArgs(_asyncException, IsAsyncCanceled));
            _asyncException = null;
        }

        #endregion

        #region Destructors

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _log.Dispose();
            _disposed = true;
        }

        /// <summary>
        ///     Dispose deconstructor.
        /// </summary>
        ~FtpClient()
        {
            Dispose();
        }

        #endregion
    }
}