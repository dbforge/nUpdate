// Copyright © Dominic Beger 2019

using System.IO;

namespace nUpdate
{
    internal class DeltaPackageBuilder
    {
        internal static void ApplyDelta(string currentFile, string deltaFile, string targetFile)
        {
            File.WriteAllBytes(targetFile,
                Delta.Delta.Apply(File.ReadAllBytes(currentFile), File.ReadAllBytes(deltaFile)));
        }

        internal static void BuildDelta(string currentFile, string newFile, string deltaOutputFile)
        {
            File.WriteAllBytes(deltaOutputFile,
                Delta.Delta.Create(File.ReadAllBytes(currentFile), File.ReadAllBytes(newFile)));
        }
    }
}