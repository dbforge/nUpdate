using System;
using System.Net;
using System.Text;
using nUpdate.Win32;

namespace nUpdate
{
    public class SizeHelper
    {
        public static string ToAdequateSizeString(long fileSize)
        {
            var sb = new StringBuilder(20);
            NativeMethods.StrFormatByteSize(fileSize, sb, 20);
            return sb.ToString();
        }

        public static double? GetRemoteFileSize(Uri packageUri)
        {
            try
            {
                var req = WebRequest.Create(packageUri);
                req.Method = "HEAD";
                using (var resp = req.GetResponse())
                {
                    double contentLength;
                    if (double.TryParse(resp.Headers.Get("Content-Length"), out contentLength))
                        return contentLength;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
