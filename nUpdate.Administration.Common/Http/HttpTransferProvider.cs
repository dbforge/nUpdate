using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using nUpdate.Administration.Common.Http.JsonRpc;

namespace nUpdate.Administration.Common.Http
{
    public class HttpTransferProvider : ITransferProvider
    {
        internal HttpData Data => TransferData as HttpData;

        private async Task<string> SendRequestInternal(object parameter, string callerMethod)
        {
            using (var client = new WebClientEx())
            {
                if (Data.MustAuthenticate)
                    client.Credentials = new NetworkCredential(Data.Username, Data.Password);

                var jsonRequest = new JsonRequest(callerMethod, parameter, Guid.NewGuid());
                return await client.UploadStringTaskAsync(Data.ScriptUri, "POST", jsonRequest.ToString());
            }
        }

        private async Task<JsonResponse> SendRequest(object parameter, [CallerMemberName] string callerMethod = "")
        {
            return JsonSerializer.Deserialize<JsonResponse>(await SendRequestInternal(parameter, callerMethod));
        }

        private async Task<JsonResponse<T>> SendRequest<T>(object parameter, [CallerMemberName] string callerMethod = "")
        {
            return JsonSerializer.Deserialize<JsonResponse<T>>(await SendRequestInternal(parameter, callerMethod));
        }

        public Task DeleteDirectoryInWorkingDirectory(string directoryName)
        {
            return DeleteDirectory(Path.Combine(Data.Directory, directoryName));
        }

        public async Task DeleteDirectory(string directoryPath)
        {
            var response = await SendRequest(directoryPath);
            if (response.Error != null)
                throw response.Error;
        }

        public Task DeleteFileInWorkingDirectory(string fileName)
        {
            return DeleteFile(Path.Combine(Data.Directory, fileName));
        }

        public async Task<(bool, Exception)> TestConnection()
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            var response = await SendRequest(null);
            return response.Error != null ? (false, response.Error) : (true, null);
        }

        public async Task DeleteFile(string filePath)
        {
            var response = await SendRequest(filePath);
            if (response.Error != null)
                throw response.Error;
        }

        public async Task<IEnumerable<IServerItem>> List(string path, bool recursive)
        {
            var response = await SendRequest<IEnumerable<IServerItem>>(new object[] {path, recursive});
            if (response.Error != null)
                throw response.Error;
            return response.Result;
        }

        public Task RenameInWorkingDirectory(string oldName, string newName)
        {
            return Rename(Data.Directory, oldName, newName);
        }

        public async Task Rename(string path, string oldName, string newName)
        {
            var response = await SendRequest(new[] {path, oldName, newName});
            if (response.Error != null)
                throw response.Error;
        }

        public Task MakeDirectoryInWorkingDirectory(string name)
        {
            return MakeDirectory(Path.Combine(Data.Directory, name));
        }

        public async Task<bool> FileExists(string filePath)
        {
            var response = await SendRequest<bool>(filePath);
            if (response.Error != null)
                throw response.Error;
            return response.Result;
        }

        public Task<bool> FileExistsInWorkingDirectory(string fileName)
        {
            return FileExists(Path.Combine(Data.Directory, fileName));
        }

        public async Task<bool> DirectoryExists(string directoryPath)
        {
            var response = await SendRequest<bool>(directoryPath);
            if (response.Error != null)
                throw response.Error;
            return response.Result;
        }

        public Task<bool> DirectoryExistsInWorkingDirectory(string destinationName)
        {
            return DirectoryExists(Path.Combine(Data.Directory, destinationName));
        }

        public async Task MakeDirectory(string directoryPath)
        {
            var response = await SendRequest(directoryPath);
            if (response.Error != null)
                throw response.Error;
        }

        public ITransferData TransferData { get; set; }

        public async Task UploadFile(string filePath, IProgress<ITransferProgressData> progress)
        {
            // TODO: Implement progress handling
            var response = await SendRequest(filePath);
            if (response.Error != null)
                throw response.Error;
        }
    }
}