using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace nUpdate.Administration
{
    internal class ResourceHelper
    {
        private static string FormatResourceName(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetName().Name + "." + resourceName.Replace(" ", "_")
                                                               .Replace("\\", ".")
                                                               .Replace("/", ".");
        }
        
        public static async Task<string> GetEmbeddedResource(string resourceName)
        {
            resourceName = FormatResourceName(resourceName);
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    return null;

                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}