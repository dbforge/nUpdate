using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using nUpdate.Administration.Models;
using Renci.SshNet;

namespace nUpdate.Administration.BusinessLogic.Ssh
{
    public class SshTransferProvider : ITransferProvider
    {
        public ITransferData TransferData { get; set; }

        public Task DeleteDirectory(string relativeDirectoryPath)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFile(string relativeFilePath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DirectoryExists(string relativeDirectoryPath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FileExists(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IServerItem>> List(string relativeDirectoryPath, bool recursive)
        {
            throw new NotImplementedException();
        }

        public Task MakeDirectory(string relativePath)
        {
            throw new NotImplementedException();
        }

        public Task Rename(string relativePath, string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, Exception)> TestConnection()
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(string localFilePath, string remoteRelativePath,
            IProgress<ITransferProgressData> progress)
        {
            throw new NotImplementedException();
        }

        private SftpClient GetSftpClient()
        {
            byte[] expectedFingerPrint = {
                0x66, 0x31, 0xaf, 0x00, 0x54, 0xb9, 0x87, 0x31,
                0xff, 0x58, 0x1c, 0x31, 0xb1, 0xa2, 0x4c, 0x6b
            };
            var connectionInfo = new ConnectionInfo("sftp.foo.com",
                "guest",
                new PasswordAuthenticationMethod("guest", "pwd"),
                new PrivateKeyAuthenticationMethod("rsa.key"));
            using (var client = new SftpClient(connectionInfo))
            {
                client.HostKeyReceived += (sender, e) =>
                {
                    if (expectedFingerPrint.Length == e.FingerPrint.Length)
                    {
                        for (var i = 0; i < expectedFingerPrint.Length; i++)
                        {
                            if (expectedFingerPrint[i] != e.FingerPrint[i])
                            {
                                e.CanTrust = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        e.CanTrust = false;
                    }
                };

                client.Connect();
                return client;
            }
        }
    }
}