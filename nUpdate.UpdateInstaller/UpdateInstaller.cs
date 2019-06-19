using System;
using System.IO;
using System.Threading.Tasks;

namespace nUpdate.UpdateInstaller
{
    internal class UpdateInstaller
    {
        private static UpdateInstaller _instance;

        private UpdateInstaller()
        { }

        public static UpdateInstaller Instance => _instance ?? (_instance = new UpdateInstaller());

        internal async Task Install()
        {
            var packageDirectory = Program.PackageDirectory;
            var newUpdatePackages = Program.NewUpdatePackages;

            throw new NotImplementedException();
            /*await newUpdatePackages.ForEachAsync(package =>
            {
                var identifier = package.Guid;
                string packagePath = Path.Combine(packageDirectory, identifier.ToString(), ".zip");
            });*/
        }
    }
}
