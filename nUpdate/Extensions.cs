// Extensions.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.IO;

namespace nUpdate
{
    internal static class Extensions
    {
        public static void Empty(this DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles())
                file.Delete();
            foreach (var subDirectory in directory.GetDirectories())
                subDirectory.Delete(true);
        }
    }
}