using System;

namespace nUpdate.Administration.Common.Http
{
    public class HttpData : ITransferData
    {
        public Uri ScriptUri { get; set; }
        public bool MustAuthenticate { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}