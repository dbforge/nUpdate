// HttpBackendType.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System.ComponentModel;

namespace nUpdate.Administration.Models.Http
{
    public enum HttpBackendType
    {
        [Description("administration.php")] Php,
        [Description("index.js")] NodeJs,
        Custom
    }
}