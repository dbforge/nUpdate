// Author: Dominic Beger (Trade/ProgTrade)

using nUpdate.Administration.Application;

namespace nUpdate.Administration.Logging
{
    internal class PackageActionLogger
    {
        private readonly UpdateProject _project;

        internal PackageActionLogger(UpdateProject project)
        {
            _project = project;
        }

        public UpdateProject Clear()
        {
            _project.LogData.Clear();
            _project.Save();

            return _project;
        }

        public UpdateProject AppendEntry(PackageActionType type, IUpdateVersion packageVersion)
        {
            var logData = new PackageActionLogData(type, packageVersion);
            _project.LogData.Add(logData);
            _project.Save();

            return _project;
        }
    }
}