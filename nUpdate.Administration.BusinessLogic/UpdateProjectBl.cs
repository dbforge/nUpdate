// UpdateProjectBl.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System.IO;
using System.Linq;
using nUpdate.Administration.Models;
using nUpdate.Administration.Models.Logging;
using nUpdate.Administration.PluginBase.Models;

namespace nUpdate.Administration.BusinessLogic
{
    public class UpdateProjectBl
    {
        public UpdateProjectBl(string path)
        {
            var updateProject = JsonSerializer.Deserialize<UpdateProject>(File.ReadAllText(path));
            var currentProjectEntry =
                ProjectSession.AvailableLocations.FirstOrDefault(item => item.Guid == updateProject.Guid);
            if (currentProjectEntry == null)
                ProjectSession.AvailableLocations.Add(new UpdateProjectLocation(updateProject.Guid, path));
            else
                currentProjectEntry.LastSeenPath = path;

            UpdateProject = updateProject;
        }

        public UpdateProjectBl(UpdateProject project)
        {
            UpdateProject = project;
        }

        public UpdateProject UpdateProject { get; }

        public void AddLogEntry(string packageName, PackageActionType packageActionType)
        {
            var logData = new PackageActionLogData(packageActionType, packageName);
            // TODO: LOG
        }

        public void ClearLog()
        {
            // TODO: LOG
        }

        /// <summary>
        ///     Saves the current <see cref="UpdateProject" /> under the last known file path.
        /// </summary>
        public void Save()
        {
            var updateProjectLocation =
                ProjectSession.AvailableLocations.FirstOrDefault(loc => loc.Guid == UpdateProject.Guid);
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