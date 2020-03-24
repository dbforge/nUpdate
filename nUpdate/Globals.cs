// Globals.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.IO;

namespace nUpdate
{
    public static class Globals
    {
        public static string AppExecutableDirectoryIdentifier => "ApplicationExecutablePath";

        public static string ApplicationUpdateDirectory => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nUpdate",
            "Updates", ApplicationParameters.ProductName);

        public static string PackageExtension => ".nupdpkg";
    }
}