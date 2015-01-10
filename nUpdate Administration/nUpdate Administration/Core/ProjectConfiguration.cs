using System;

namespace nUpdate.Administration.Core
{
    [Serializable]
    public class ProjectConfiguration
    {
        public ProjectConfiguration(string name, string path)
        {
            Name = name;
            Path = path;
        }

        /// <summary>
        ///     The name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The path o´f the project-file.
        /// </summary>
        public string Path { get; set; }
    }
}
