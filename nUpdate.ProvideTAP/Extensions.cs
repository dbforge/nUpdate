using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate
{
    public static class Extensions
    {
        public static async Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request, CancellationToken token)
        {
            using (token.Register(request.Abort, false))
            {
                try
                {
                    var response = await request.GetResponseAsync();
                    return (HttpWebResponse)response;
                }
                catch (WebException ex)
                {
                    if (token.IsCancellationRequested)
                        throw new OperationCanceledException(ex.Message, ex, token);
                    
                    throw;
                }
            }
        }
    }
}
