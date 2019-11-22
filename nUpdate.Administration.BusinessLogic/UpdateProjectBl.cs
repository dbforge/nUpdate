using System.IO;
using System.Linq;
using nUpdate.Administration.Models;
using nUpdate.Administration.Models.Logging;

namespace nUpdate.Administration.BusinessLogic
{
    public class UpdateProjectBl
    {
        private readonly UpdateProject _updateProject;

        public UpdateProjectBl(string path)
        {
            var updateProject = JsonSerializer.Deserialize<UpdateProject>(File.ReadAllText(path));
            var currentProjectEntry =
                ProjectSession.AvailableLocations.FirstOrDefault(item => item.Guid == updateProject.Guid);
            if (currentProjectEntry == null)
                ProjectSession.AvailableLocations.Add(new UpdateProjectLocation(updateProject.Guid, path));
            else
                currentProjectEntry.LastSeenPath = path;

            _updateProject = updateProject;
        }

        public UpdateProjectBl(UpdateProject project)
        {
            _updateProject = project;
        }

        public UpdateProject UpdateProject => _updateProject;

        public void AddLogEntry(string packageName, PackageActionType packageActionType)
        {
            var logData = new PackageActionLogData(packageActionType, packageName);
            _updateProject.LogData.Add(logData);
        }

        public void ClearLog()
        {
            _updateProject.LogData.Clear();
        }

        /// <summary>
        ///     Saves the current <see cref="UpdateProject" /> under the last known file path.
        /// </summary>
        public void Save()
        {
            var updateProjectLocation = ProjectSession.AvailableLocations.FirstOrDefault(loc => loc.Guid == _updateProject.Guid);
            if (updateProjectLocation != null)
                File.WriteAllText(updateProjectLocation.LastSeenPath, JsonSerializer.Serialize(this));
            // TODO: Handle case that path is null
        }

        public void Save(string path)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(this));
        }
    }
}
