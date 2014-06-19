using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;
using System.Net.Sockets;
using System.ComponentModel;
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
        public FtpProtocol Protocol
        {
            get;
            set;
        }

        /// <summary>
        /// Sets if passive mode should be used.
        /// </summary>
        public bool FtpModeUsePassive
        {
            get;
            set;
        }

        /// <summary>
        /// The FTP-server.
        /// </summary>
        public string FtpServer
        {
            get;
            set;
        }

        /// <summary>
        /// The port.
        /// </summary>
        public int FtpPort
        {
            get;
            set;
        }

        /// <summary>
        /// The directory.
        /// </summary>
        public string Directory
        {
            get;
            set;
        }

        /// <summary>
        /// The username.
        /// </summary>
        public string FtpUserName
        {
            get;
            set;
        }

        /// <summary>
        /// The password.
        /// </summary>
        public SecureString FtpPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Is fired when the upload of a file has failed.
        /// </summary>
        protected internal void OnUploadFailed(Exception ex)
        {
            FailedEventHandler handler = UploadFailed;
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
            FailedEventHandler handler = DeleteFailed;
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
            EventHandler<EventArgs> handler = ServerConnected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected internal void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = ProgressChanged;
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

            byte[] _sent = new byte[content.Length];
            int _count = 0;

            foreach (byte b in System.Text.Encoding.Default.GetBytes(content))
            {
                _sent[_count] = b;
                _count++;
            }

            stream.Write(_sent, 0, _sent.Length);
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
            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}",
                            FtpServer, FtpPort, Directory, directory);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ServerAdress));
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(FtpUserName, FtpPassword);

            if (Protocol == FtpProtocol.SecureFtp)
                request.EnableSsl = true;
            var resp = (FtpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// Deletes a file on the server.
        /// </summary>
        public void DeleteFile(string fileName)
        {
            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}",
                    FtpServer, FtpPort, Directory, fileName);

            deleteRequest = (FtpWebRequest)WebRequest.Create(new Uri(ServerAdress));
            deleteRequest.KeepAlive = false;
            deleteRequest.Method = WebRequestMethods.Ftp.DeleteFile;

            deleteRequest.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
            deleteRequest.UsePassive = FtpModeUsePassive;

            if (Protocol == FtpProtocol.SecureFtp)
                deleteRequest.EnableSsl = true;

            var deleteResponse = deleteRequest.GetResponse();
        }

        /// <summary>
        /// Deletes a directory on the server.
        /// </summary>
        public void DeleteDirectory(string directoryName)
        {
            /* ------- List the files -------- */

            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}",
                    FtpServer, FtpPort, Directory, directoryName);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ServerAdress));
            request.Credentials = new NetworkCredential(FtpUserName, FtpPassword);

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
                    DeleteFile(entry);
            }

            /* ------- Delete the directory -------- */

            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}",
                    FtpServer, FtpPort, Directory, directoryName);

            deleteRequest = (FtpWebRequest)WebRequest.Create(new Uri(ServerAdress));
            deleteRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;

            deleteRequest.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
            deleteRequest.UsePassive = FtpModeUsePassive;

            if (Protocol == FtpProtocol.SecureFtp)
                deleteRequest.EnableSsl = true;

            var deleteResponse = deleteRequest.GetResponse();
        }

        /// <summary>
        /// Connects to the server and uploads a file.
        /// </summary>
        public void UploadFile(string filePath)
        {
            HasFinishedUploading = false;
            fileWebClient = new WebClient();
            fileWebClient.UploadFileCompleted += new UploadFileCompletedEventHandler(UploadFinishedEventHandler);
            fileWebClient.Credentials = new NetworkCredential(FtpUserName, FtpPassword);

            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}",
                    FtpServer, FtpPort, Directory, Path.GetFileName(filePath));

            fileWebClient.UploadFileAsync(new Uri(ServerAdress), filePath);
        }

        public bool HasFinishedUploading { get; set; }

        /// <summary>
        /// Connects to the server and uploads a package file.
        /// </summary>
        public void UploadPackage(string packagePath, string packageVersion)
        {
            HasFinishedUploading = false;
            CreateDirectory(packageVersion);

            packageWebClient = new WebClient();
            packageWebClient.UploadProgressChanged += new UploadProgressChangedEventHandler(UploadProgressChangedEventHandler);
            packageWebClient.UploadFileCompleted += new UploadFileCompletedEventHandler(UploadFinishedEventHandler);
            packageWebClient.Credentials = new NetworkCredential(FtpUserName, FtpPassword);

            ServerAdress = String.Format("ftp://{0}:{1}/{2}/{3}/{4}", 
                FtpServer, FtpPort, Directory, packageVersion, Path.GetFileName(packagePath));

            packageWebClient.UploadFileAsync(new Uri(ServerAdress), packagePath);
        }

        private void UploadProgressChangedEventHandler(object sender, ProgressChangedEventArgs e)
        {
            OnProgressChanged(e);
        }

        private void UploadFinishedEventHandler(object sender, UploadFileCompletedEventArgs e)
        {
            if (e.Error != null)
                OnUploadFailed(e.Error);
            HasFinishedUploading = true;
        }
    }
}
