using nUpdate.Actions;

namespace nUpdate.UpdateInstaller
{
    internal class UpdateActionAppPathProvider : IUpdateActionAppPathProvider
    {
        public string GetAppPath()
        {
            return Program.AppDirectory;
        }
    }
}
