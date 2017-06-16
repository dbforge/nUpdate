using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace nUpdate.Administration
{
    // ReSharper disable once InconsistentNaming
    internal static class ShaManager
    {
        internal static string Hash(string plain)
        {
            using (var sha = new SHA1Managed())
                return new ByteConverter().ConvertToString(sha.ComputeHash(Encoding.UTF8.GetBytes(plain)));
        }

        /*
        public static byte[] HashDirectory(string path)
        {
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                .OrderBy(p => p).ToList();

            var sha = new SHA1Managed();

            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                string relativePath = file.Substring(path.Length + 1);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                sha.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                byte[] contentBytes = File.ReadAllBytes(file);
                if (i == files.Count - 1)
                    sha.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                else
                    sha.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
            }

            return sha.Hash;
        }

        public static byte[] HashFile(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var sha = new SHA1Managed();
                return sha.ComputeHash(stream);
            }
        }*/
    }
}