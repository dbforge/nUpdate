// CustomWebClient.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Net;
using System.Threading.Tasks;

namespace nUpdate
{
    public class CustomWebClient : WebClient, ICustomWebClient
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomWebClient" />-class.
        /// </summary>
        public CustomWebClient() : this(5000)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomWebClient" />-class.
        /// </summary>
        /// <param name="timeout">The timeout to use.</param>
        public CustomWebClient(int timeout)
        {
            Timeout = timeout;
        }

        public int Timeout { get; set; }

        public async Task<T> DownloadFromJson<T>(Uri resourceUri)
        {
            var result = await DownloadStringFrom(resourceUri);
            return JsonSerializer.Deserialize<T>(result);
        }

        public Task<string> DownloadStringFrom(Uri resourceUri)
        {
            return DownloadStringTaskAsync(resourceUri);
        }

        public Task<string> UploadStringTo(Uri targetUri, string method, string data)
        {
            return UploadStringTaskAsync(targetUri, method, data);
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request != null)
                request.Timeout = Timeout;

            return request;
        }
    }
}