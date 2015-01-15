// Author: Dominic Beger (Trade/ProgTrade)

using System.IO;

namespace nUpdate.Administration.Core.Application
{
    public class ApplicationInstance
    {
        /// <summary>
        ///     Loads an update project.
        /// </summary>
        /// <param name="path">The path of the project file.</param>
        /// <returns>Returns the read update project.</returns>
        public static UpdateProject LoadProject(string path)
        {
            return Serializer.Deserialize<UpdateProject>(File.ReadAllText(path));
        }

        /// <summary>
        ///     Saves an update project.
        /// </summary>
        /// <param name="path">The path of the project file.</param>
        /// <param name="project">The project to save.</param>
        public static void SaveProject(string path, UpdateProject project)
        {
            var serializedContent = Serializer.Serialize(project);
            File.WriteAllText(path, serializedContent);
        }
    }
}