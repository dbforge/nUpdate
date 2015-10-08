// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using nUpdate.Administration.Application;

namespace nUpdate.Administration
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
        ///     Gets or sets the name of the <see cref="UpdateProject"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the path of the file of the <see cref="UpdateProject"/>.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier of the <see cref="UpdateProject"/>.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        ///     Loads the project configuration from the file in the application's main folder.
        /// </summary>
        public static IEnumerable<ProjectConfiguration> Load()
        {
            string content = File.ReadAllText(Program.ProjectsConfigFilePath);
            return !string.IsNullOrEmpty(content)
                ? Serializer.Deserialize<IEnumerable<ProjectConfiguration>>(content)
                : Enumerable.Empty<ProjectConfiguration>();
        }
    }
}