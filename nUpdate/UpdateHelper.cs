using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace nUpdate
{
    public static class UpdateHelper
    {
        public async static Task<double> GetTotalPackageSize(IEnumerable<UpdatePackage> packages)
        {
            double size = 0;
            await packages.ForEachAsync(async p =>
            {
                try
                {
                    var req = WebRequest.Create(p.UpdatePackageUri);
                    req.Method = "HEAD";
                    using (var resp = await req.GetResponseAsync())
                    {
                        double contentLength;
                        size = double.TryParse(resp.Headers.Get("Content-Length"), out contentLength)
                            ? contentLength
                            : double.NaN;
                    }
                }
                catch
                {
                    size = double.NaN;
                }
            });
            return size;
        }
    }
}