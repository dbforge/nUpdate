// HttpData.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using nUpdate.Administration.PluginBase.Models;

namespace nUpdate.Administration.Models.Http
{
    public class HttpData : ITransferData
    {
        public string Password { get; set; }
        public Uri ScriptUri { get; set; }
        public string Username { get; set; }
    }
}