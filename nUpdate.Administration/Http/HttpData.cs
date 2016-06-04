using System;

namespace nUpdate.Administration.Http
{
    internal struct HttpData : ITransferData
    {
        internal Uri ScriptUri { get; set; }
        internal bool MustAuthenticate { get; set; }
        internal string Username { get; set; }
        internal string Password { get; set; }
    }
}