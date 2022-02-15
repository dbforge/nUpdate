// Globals.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.IO;

namespace nUpdate
{
    public static class Globals
    {
        public static string AppExecutableDirectoryIdentifier => "ApplicationExecutablePath";

        public static string ApplicationFilesDirectory => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nUpdate");

        public static string ApplicationUpdateDirectory => Path.Combine(ApplicationFilesDirectory,
            "Updates", ApplicationParameters.ProductName);

        public static string PackageExtension => ".nupdpkg";
    }
}