using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;

namespace nUpdate.Administration.Core.Update
{
    internal class FtpManager
    {
        public delegate void FailedEventHandler(Exception ex);
        public event FailedEventHandler UploadFailed;
        public event FailedEventHandler DeleteFailed;
        public event EventHandler<EventArgs> ServerConnected;
        public event ProgressChangedEventHandler ProgressChanged;

        private FtpWebRequest deleteRequest;

        private WebClient packageWebClient;
        private WebClient fileWebClient;

        /// <summary>
        /// Returns the adress that was created by the <see cref="FtpManager"/>-class during the call of the last function.
        /// </summary>
        public string ServerAdress { get; set; }

        /// <summary>
        /// Sets the protocol to use.
        /// </summary>
        public FtpProtocol Protocol { get; set; }

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

        /// <summary>
        /// Is fired when the upload of a file has failed.
        /// </summary>
        protected internal void OnUploadFailed(Exception ex)
        {
            FailedEventHandler handler = this.UploadFailed;
            if (handler != null)
            {
                handler(ex);
            }
        }

        /// <summary>
        /// Is fired when deleting a file has failed.
        /// </summary>
        protected internal void OnDeleteFailed(Exception ex)
        {
            FailedEventHandler handler = this.DeleteFailed;
            if (handler != null)
            {
                handler(ex);
            }
        }

        /// <summary>
        /// Is fired when the client is connected to the server.
        /// </summary>
        protected internal void OnServerConnected()
        {
            EventHandler<EventArgs> handler = this.ServerConnected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected internal void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = this.ProgressChanged;
            if (handler != null)
            {
                handler(this, e);
            }
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
            this.ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", this.FtpServer, this.FtpPort, this.Directory, directory);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(this.ServerAdress));
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);

            if (this.Protocol == FtpProtocol.SecureFtp)
            {
                request.EnableSsl = true;
            }
            var resp = (FtpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// Deletes a file on the server.
        /// </summary>
        public void DeleteFile(string fileName)
        {
            this.ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", this.FtpServer, this.FtpPort, this.Directory, fileName);

            this.deleteRequest = (FtpWebRequest)WebRequest.Create(new Uri(this.ServerAdress));
            this.deleteRequest.KeepAlive = false;
            this.deleteRequest.Method = WebRequestMethods.Ftp.DeleteFile;

            this.deleteRequest.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);
            this.deleteRequest.UsePassive = FtpModeUsePassive;

            if (Protocol == FtpProtocol.SecureFtp)
            {
                this.deleteRequest.EnableSsl = true;
            }

            var deleteResponse = this.deleteRequest.GetResponse();
        }

        /// <summary>
        /// Deletes a directory on the server.
        /// </summary>
        public void DeleteDirectory(string directoryName)
        {
            /* ------- List the files -------- */

            this.ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", this.FtpServer, this.FtpPort, this.Directory, directoryName);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(this.ServerAdress));
            request.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);

            request.Method = WebRequestMethods.Ftp.ListDirectory;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
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

            this.deleteRequest = (FtpWebRequest)WebRequest.Create(new Uri(this.ServerAdress));
            this.deleteRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;

            this.deleteRequest.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);
            this.deleteRequest.UsePassive = this.FtpModeUsePassive;

            if (Protocol == FtpProtocol.SecureFtp)
            {
                this.deleteRequest.EnableSsl = true;
            }

            var deleteResponse = this.deleteRequest.GetResponse();
        }

        /// <summary>
        /// Connects to the server and uploads a file.
        /// </summary>
        public void UploadFile(string filePath)
        {
            this.HasFinishedUploading = false;
            this.fileWebClient = new WebClient();
            this.fileWebClient.UploadFileCompleted += new UploadFileCompletedEventHandler(UploadFinishedEventHandler);
            this.fileWebClient.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);

            this.ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}", this.FtpServer, this.FtpPort, this.Directory, Path.GetFileName(filePath));

            this.fileWebClient.UploadFileAsync(new Uri(this.ServerAdress), filePath);
        }

        public bool HasFinishedUploading { get; set; }

        /// <summary>
        /// Connects to the server and uploads a package file.
        /// </summary>
        public void UploadPackage(string packagePath, string packageVersion)
        {
            this.HasFinishedUploading = false;
            this.CreateDirectory(packageVersion);

            this.packageWebClient = new WebClient();
            this.packageWebClient.UploadProgressChanged += new UploadProgressChangedEventHandler(UploadProgressChangedEventHandler);
            this.packageWebClient.UploadFileCompleted += new UploadFileCompletedEventHandler(UploadFinishedEventHandler);
            this.packageWebClient.Credentials = new NetworkCredential(this.FtpUserName, this.FtpPassword);

            this.ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}/{4}", this.FtpServer, this.FtpPort, this.Directory, packageVersion, Path.GetFileName(packagePath));

            this.packageWebClient.UploadFileAsync(new Uri(this.ServerAdress), packagePath);
        }

        private void UploadProgressChangedEventHandler(object sender, ProgressChangedEventArgs e)
        {
            this.OnProgressChanged(e);
        }

        private void UploadFinishedEventHandler(object sender, UploadFileCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.OnUploadFailed(e.Error);
            }
            this.HasFinishedUploading = true;
        }
    }
}
