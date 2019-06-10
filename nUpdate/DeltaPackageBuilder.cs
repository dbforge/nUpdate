using DeltaCompressionDotNet.MsDelta;

namespace nUpdate
{
    internal class DeltaPackageBuilder
    {
        internal static void BuildDelta(string currentFile, string newFile, string deltaOutputFile)
        {
            var msDelta = new MsDeltaCompression();
            msDelta.CreateDelta(currentFile, newFile, deltaOutputFile);
        }
    }
}
