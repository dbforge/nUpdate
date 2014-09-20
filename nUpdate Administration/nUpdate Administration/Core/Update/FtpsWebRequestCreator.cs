using System;
using System.Net;

namespace nUpdate.Administration.Core.Update
{
    internal class FtpsWebRequestCreator : IWebRequestCreate
    {
        public WebRequest Create(Uri uri)
        {
            var webRequest = (FtpWebRequest)WebRequest.Create(uri.AbsoluteUri.Remove(3, 1));
            webRequest.EnableSsl = true;
            return webRequest;
        }
    }
}
