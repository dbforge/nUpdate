using System;
using System.Threading.Tasks;

namespace nUpdate
{
    public interface ICustomWebClient
    {
        int Timeout { get; set; }
        Task<string> DownloadStringFrom(Uri resourceUri);
        Task<string> UploadStringTo(Uri targetUri, string method, string data);
    }
}
