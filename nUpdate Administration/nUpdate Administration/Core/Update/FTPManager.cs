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
        public event ProgressChangedEventHandler ProgressChanged;

        private bool hasAlreadyFixedStrings = false;
        private FtpWebRequest directoryRequest;
        private FtpWebRequest deleteFileRequest;
        private FtpWebRequest deleteDirectoryRequest;
        private WebClient packageWebClient;
        private WebClient fileWebClient;

        /// <summary>
        /// Returns the adress that was created by the <see cref="FTPManager"/>-class during the call of the last function.
        /// </summary>
        public string ServerAdress { get; set; }

        /// <summary>
        /// Sets the protocol to use.
        /// </summary>
        public FTPProtocol Protocol { get; set; }

        /// <summary>
        /// Sets if passive mode should be used.
        /// </summary>
        public bool FtpModeUsePassive { get; set; }

        /// <summary>
        /// The FTP-server.
        /// </summary>
        public string FtpServer { get; set; }

        /// <summary>
        /// The port.
        /// </summary>
        public int FtpPort { get; set; }

        /// <summary>
        /// The directory.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// The username.
        /// </summary>
        public string FtpUserName { get; set; }

        /// <summary>
        /// The password.
        /// </summary>
        public SecureString FtpPassword { get; set; }

        protected internal void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = this.ProgressChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Edits the properties if they are not automatically suitable for the server adress.
        /// </summary>
        private void FixProperties()
        {
            if (this.FtpServer.EndsWith("/"))
            {
                this.FtpServer = this.FtpServer.Remove(this.FtpServer.Length - 1);
            }
            
            if (this.Directory.StartsWith("/"))
            {
                this.Directory = this.Directory.Substring(1);
            }

            if (this.Directory.EndsWith("/"))
            {
                this.Directory = this.Directory.Remove(this.Directory.Length - 1);
            }

            this.hasAlreadyFixedStrings = true;
        }

        static int ReceiveBufferSize = 0;
        private static int GetChatCountInString(string haystack, char needle)
        {
            int count = 0;

            foreach (char c in haystack)
            {
                if (c == needle) count++;
            }

            return count;
        }

        /// <summary>
        /// Sends a content to a network stream.
        /// </summary>
        private static void Send(NetworkStream stream, string content)
        {
            content = content + "\r\n";

            byte[] sent = new byte[content.Length];
            int count = 0;

            foreach (byte b in System.Text.Encoding.Default.GetBytes(content))
            {
                sent[count] = b;
                count++;
            }

            stream.Write(sent, 0, sent.Length);
        }

        /// <summary>
        /// Reads a network stream.
        /// </summary>
        private static string Read(NetworkStream stream)
        {
            while (stream.DataAvailable == false)
            {
            }
            byte[] bytes = new byte[ReceiveBufferSize];
            string read = "";
            while (stream.DataAvailable)
            {
                read += System.Text.Encoding.UTF8.GetString(bytes, 0, stream.Read(bytes, 0, bytes.Length));
            }
            return read;
        }

        /// <summary>
        /// Creates a directory on the server.
        /// </summary>
        /// <param name="directory"></param>
        private void CreateDirectory(string directory)
        {
            if (!this.hasAlreadyFixedStrings)
            {
                this.FixProperties();
            }

            this.ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", this.FtpServer, this.FtpPort, this.Directory, directory);

            this.directoryRequest = (FtpWebRequest)WebRequest.Create(new Uri(this.ServerAdress));
            this.directoryRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            this.directoryRequest.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);
            this.directoryRequest.UsePassive = this.FtpModeUsePassive;

            if (this.Protocol == FTPProtocol.SecureFtp)
            {
                this.directoryRequest.EnableSsl = true;
            }
            var resp = (FtpWebResponse)this.directoryRequest.GetResponse();
        }

        /// <summary>
        /// Deletes a file on the server.
        /// </summary>
        public void DeleteFile(string fileName)
        {
            if (!this.hasAlreadyFixedStrings)
            {
                this.FixProperties();
            }

            this.ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", this.FtpServer, this.FtpPort, this.Directory, fileName);

            this.deleteFileRequest = (FtpWebRequest)WebRequest.Create(new Uri(this.ServerAdress));
            this.deleteFileRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            this.deleteFileRequest.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);
            this.deleteFileRequest.UsePassive = FtpModeUsePassive;

            if (Protocol == FTPProtocol.SecureFtp)
            {
                this.deleteFileRequest.EnableSsl = true;
            }

            var deleteResponse = this.deleteFileRequest.GetResponse();
        }

        /// <summary>
        /// Deletes a directory on the server.
        /// </summary>
        public void DeleteDirectory(string directoryName)
        {
            /* ------- List the files -------- */

            if (!this.hasAlreadyFixedStrings)
            {
                this.FixProperties();
            }

            this.ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", this.FtpServer, this.FtpPort, this.Directory, directoryName);

            this.deleteDirectoryRequest = (FtpWebRequest)WebRequest.Create(new Uri(this.ServerAdress));
            this.deleteDirectoryRequest.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);
            this.deleteDirectoryRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            this.deleteDirectoryRequest.UsePassive = this.FtpModeUsePassive;

            FtpWebResponse response = (FtpWebResponse)this.deleteDirectoryRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            List<string> result = new List<string>();

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
                {
                    this.DeleteFile(entry);
                }
            }

            /* ------- Delete the directory -------- */

            this.ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", this.FtpServer, this.FtpPort, this.Directory, directoryName);

            this.deleteFileRequest = (FtpWebRequest)WebRequest.Create(new Uri(this.ServerAdress));
            this.deleteFileRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
            this.deleteFileRequest.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);
            this.deleteFileRequest.UsePassive = this.FtpModeUsePassive;

            if (Protocol == FTPProtocol.SecureFtp)
            {
                this.deleteFileRequest.EnableSsl = true;
            }

            var deleteResponse = this.deleteFileRequest.GetResponse();
        }

        public bool HasFinishedUploading { get; set; }

        /// <summary>
        /// Connects to the server and uploads a file.
        /// </summary>
        public void UploadFile(string filePath)
        {
            this.HasFinishedUploading = false;
            this.fileWebClient = new WebClient();
            this.fileWebClient.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);

            this.FixProperties();
            this.ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", this.FtpServer, this.FtpPort, this.Directory, Path.GetFileName(filePath));
            this.fileWebClient.UploadFile(new Uri(this.ServerAdress), filePath);
        }

        public Exception PackageUploadException { get; set; }

        /// <summary>
        /// Connects to the server and uploads a package file.
        /// </summary>
        public void UploadPackage(string packagePath, string packageVersion)
        {
            this.CreateDirectory(packageVersion);

            this.packageWebClient = new WebClient();
            this.packageWebClient.UploadProgressChanged += new UploadProgressChangedEventHandler(UploadProgressChangedEventHandler);
            this.packageWebClient.UploadFileCompleted += new UploadFileCompletedEventHandler(UploadFinishedEventHandler);
            this.packageWebClient.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);

            this.ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}/{4}", this.FtpServer, this.FtpPort, this.Directory, packageVersion, Path.GetFileName(packagePath));
            this.packageWebClient.UploadFileAsync(new Uri(this.ServerAdress), packagePath);
        }

        /// <summary>
        /// Terminates the package upload process.
        /// </summary>
        public void CancelPackageUpload()
        {
            packageWebClient.CancelAsync();
        }

        private void UploadProgressChangedEventHandler(object sender, ProgressChangedEventArgs e)
        {
            this.OnProgressChanged(e);
        }

        private void UploadFinishedEventHandler(object sender, UploadFileCompletedEventArgs e)
        {
            this.HasFinishedUploading = true;
            if (e.Error != null)
            {
                if (e.Error.InnerException != null)
                    PackageUploadException = e.Error.InnerException;
            }
        }
    }
}
