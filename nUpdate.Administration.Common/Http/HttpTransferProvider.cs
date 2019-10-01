// HttpTransferProvider.cs, 27.07.2019
// Copyright (C) Dominic Beger 18.09.2019

using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using nUpdate.Administration.Common.Http.JsonRpc;

namespace nUpdate.Administration.Common.Http
{
    public class HttpTransferProvider : ITransferProvider
    {
        internal HttpData Data => TransferData as HttpData;

        public async Task DeleteDirectory(string relativeDirectoryPath)
        {
            var response = await SendRequest(relativeDirectoryPath);
            if (response.Error != null)
                throw response.Error;
        }

        public async Task DeleteFile(string relativeFilePath)
        {
            var response = await SendRequest(relativeFilePath);
            if (response.Error != null)
                throw response.Error;
        }

        public async Task<bool> DirectoryExists(string relativeDirectoryPath)
        {
            var response = await SendRequest<bool>(relativeDirectoryPath);
            if (response.Error != null)
                throw response.Error;
            return response.Result;
        }

        public async Task<bool> FileExists(string relativeFilePath)
        {
            var response = await SendRequest<bool>(relativeFilePath);
            if (response.Error != null)
                throw response.Error;
            return response.Result;
        }

        public async Task<IEnumerable<IServerItem>> List(string relativeDirectoryPath, bool recursive)
        {
            var response = await SendRequest<IEnumerable<IServerItem>>(new object[] {relativeDirectoryPath, recursive});
            if (response.Error != null)
                throw response.Error;
            return response.Result;
        }

        public async Task MakeDirectory(string relativePath)
        {
            var response = await SendRequest(relativePath);
            if (response.Error != null)
                throw response.Error;
        }

        public async Task Rename(string relativePath, string oldName, string newName)
        {
            var response = await SendRequest(new[] {relativePath, oldName, newName});
            if (response.Error != null)
                throw response.Error;
        }

        public async Task<(bool, Exception)> TestConnection()
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            var response = await SendRequest(null);
            return response.Error != null ? (false, response.Error) : (true, null);
        }

        public ITransferData TransferData { get; set; }

        public async Task UploadFile(string localFilePath, string remoteRelativePath,
            IProgress<ITransferProgressData> progress)
        {
            // TODO: Implement progress handling
            var response = await SendRequest(new[] {localFilePath, remoteRelativePath});
            if (response.Error != null)
                throw response.Error;
        }

        private async Task<JsonResponse> SendRequest(object parameter, [CallerMemberName] string callerMethod = "")
        {
            return JsonSerializer.Deserialize<JsonResponse>(await SendRequestInternal(parameter, callerMethod));
        }

        private async Task<JsonResponse<T>> SendRequest<T>(object parameter,
            [CallerMemberName] string callerMethod = "")
        {
            return JsonSerializer.Deserialize<JsonResponse<T>>(await SendRequestInternal(parameter, callerMethod));
        }

        private async Task<string> SendRequestInternal(object parameter, string callerMethod)
        {
            using (var client = new WebClientEx())
            {
                client.Credentials = new NetworkCredential(Data.Username, Data.Password);
                var jsonRequest = new JsonRequest(callerMethod, parameter, Guid.NewGuid());
                return await client.UploadStringTaskAsync(Data.ScriptUri, "POST", jsonRequest.ToString());
            }
        }
    }
}