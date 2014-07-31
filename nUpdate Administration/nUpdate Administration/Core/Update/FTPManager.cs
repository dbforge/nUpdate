using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;

namespace nUpdate.Administration.Core.Update
{
    internal class FTPManager
    {
        private static int ReceiveBufferSize = 0;
        private FtpWebRequest deleteDirectoryRequest;
        private FtpWebRequest deleteFileRequest;
        private FtpWebRequest directoryRequest;
        private WebClient fileWebClient;
        private bool hasAlreadyFixedStrings;
        private WebClient packageWebClient;

        /// <summary>
        ///     Returns the adress that was created by the <see cref="FTPManager" />-class during the call of the last function.
        /// </summary>
        public string ServerAdress { get; set; }

        /// <summary>
        ///     Sets the protocol to use.
        /// </summary>
        public FTPProtocol Protocol { get; set; }

        /// <summary>
        ///     Sets if passive mode should be used.
        /// </summary>
        public bool FtpModeUsePassive { get; set; }

        /// <summary>
        ///     The FTP-server.
        /// </summary>
        public string FtpServer { get; set; }

        /// <summary>
        ///     The port.
        /// </summary>
        public int FtpPort { get; set; }

        /// <summary>
        ///     The directory.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        ///     The username.
        /// </summary>
        public string FtpUserName { get; set; }

        /// <summary>
        ///     The password.
        /// </summary>
        public SecureString FtpPassword { get; set; }

        public bool HasFinishedUploading { get; set; }
        public Exception PackageUploadException { get; set; }
        public event ProgressChangedEventHandler ProgressChanged;

        protected internal void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = ProgressChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        ///     Edits the properties if they are not automatically suitable for the server adress.
        /// </summary>
        private void FixProperties()
        {
            if (FtpServer.EndsWith("/"))
                FtpServer = FtpServer.Remove(FtpServer.Length - 1);

            if (Directory.StartsWith("/"))
                Directory = Directory.Substring(1);

            if (Directory.EndsWith("/"))
                Directory = Directory.Remove(Directory.Length - 1);

            hasAlreadyFixedStrings = true;
        }

        private static int GetChatCountInString(string haystack, char needle)
        {
            int count = 0;

            foreach (char c in haystack)
            {
                if (c == needle)
                    count++;
            }

            return count;
        }

        /// <summary>
        ///     Sends a content to a network stream.
        /// </summary>
        private static void Send(NetworkStream stream, string content)
        {
            content = content + "\r\n";

            var sent = new byte[content.Length];
            int count = 0;

            foreach (byte b in Encoding.Default.GetBytes(content))
            {
                sent[count] = b;
                count++;
            }

            stream.Write(sent, 0, sent.Length);
        }

        /// <summary>
        ///     Reads a network stream.
        /// </summary>
        private static string Read(NetworkStream stream)
        {
            while (stream.DataAvailable == false)
            {
            }
            var bytes = new byte[ReceiveBufferSize];
            string read = "";
            while (stream.DataAvailable)
            {
                read += Encoding.UTF8.GetString(bytes, 0, stream.Read(bytes, 0, bytes.Length));
            }
            return read;
        }

        /// <summary>
        ///     Creates a directory on the server.
        /// </summary>
        /// <param name="directory"></param>
        private void CreateDirectory(string directory)
        {
            if (!hasAlreadyFixedStrings)
                FixProperties();

            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", FtpServer, FtpPort, Directory, directory);

            directoryRequest = (FtpWebRequest) WebRequest.Create(new Uri(ServerAdress));
            directoryRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            directoryRequest.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
            directoryRequest.UsePassive = FtpModeUsePassive;

            if (Protocol == FTPProtocol.SecureFtp)
                directoryRequest.EnableSsl = true;
            var resp = (FtpWebResponse) directoryRequest.GetResponse();
        }

        /// <summary>
        ///     Deletes a file on the server.
        /// </summary>
        public void DeleteFile(string fileName)
        {
            if (!hasAlreadyFixedStrings)
                FixProperties();

            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", FtpServer, FtpPort, Directory, fileName);

            deleteFileRequest = (FtpWebRequest) WebRequest.Create(new Uri(ServerAdress));
            deleteFileRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            deleteFileRequest.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
            deleteFileRequest.UsePassive = FtpModeUsePassive;

            if (Protocol == FTPProtocol.SecureFtp)
                deleteFileRequest.EnableSsl = true;

            WebResponse deleteResponse = deleteFileRequest.GetResponse();
        }

        /// <summary>
        ///     Deletes a directory on the server.
        /// </summary>
        public void DeleteDirectory(string directoryName)
        {
            /* ------- List the files -------- */

            if (!hasAlreadyFixedStrings)
                FixProperties();

            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", FtpServer, FtpPort, Directory, directoryName);

            deleteDirectoryRequest = (FtpWebRequest) WebRequest.Create(new Uri(ServerAdress));
            deleteDirectoryRequest.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
            deleteDirectoryRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            deleteDirectoryRequest.UsePassive = FtpModeUsePassive;

            var response = (FtpWebResponse) deleteDirectoryRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            var reader = new StreamReader(responseStream);

            var result = new List<string>();

            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine());
            }

            reader.Close();
            response.Close();

            /* ------- Delete the files -------- */

            foreach (string entry in result)
            {
                if (!entry.EndsWith("."))
                    DeleteFile(entry);
            }

            /* ------- Delete the directory -------- */

            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", FtpServer, FtpPort, Directory, directoryName);

            deleteFileRequest = (FtpWebRequest) WebRequest.Create(new Uri(ServerAdress));
            deleteFileRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
            deleteFileRequest.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
            deleteFileRequest.UsePassive = FtpModeUsePassive;

            if (Protocol == FTPProtocol.SecureFtp)
                deleteFileRequest.EnableSsl = true;

            WebResponse deleteResponse = deleteFileRequest.GetResponse();
        }

        /// <summary>
        ///     Connects to the server and uploads a file.
        /// </summary>
        public void UploadFile(string filePath)
        {
            HasFinishedUploading = false;
            fileWebClient = new WebClient();
            fileWebClient.Credentials = new NetworkCredential(FtpUserName, FtpPassword);

            FixProperties();
            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", FtpServer, FtpPort, Directory,
                Path.GetFileName(filePath));
            fileWebClient.UploadFile(new Uri(ServerAdress), filePath);
        }

        /// <summary>
        ///     Connects to the server and uploads a package file.
        /// </summary>
        public void UploadPackage(string packagePath, string packageVersion)
        {
            CreateDirectory(packageVersion);

            packageWebClient = new WebClient();
            packageWebClient.UploadProgressChanged += UploadProgressChangedEventHandler;
            packageWebClient.UploadFileCompleted += UploadFinishedEventHandler;
            packageWebClient.Credentials = new NetworkCredential(FtpUserName, FtpPassword);

            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}/{4}", FtpServer, FtpPort, Directory, packageVersion,
                Path.GetFileName(packagePath));
            packageWebClient.UploadFileAsync(new Uri(ServerAdress), packagePath);
        }

        /// <summary>
        ///     Terminates the package upload process.
        /// </summary>
        public void CancelPackageUpload()
        {
            packageWebClient.CancelAsync();
        }

        private void UploadProgressChangedEventHandler(object sender, ProgressChangedEventArgs e)
        {
            OnProgressChanged(e);
        }

        private void UploadFinishedEventHandler(object sender, UploadFileCompletedEventArgs e)
        {
            HasFinishedUploading = true;
            if (e.Error != null)
            {
                if (e.Error.InnerException != null)
                    PackageUploadException = e.Error.InnerException;
            }
        }
    }
}