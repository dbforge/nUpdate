using System.IO;

namespace nUpdate.Internal.Core
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
