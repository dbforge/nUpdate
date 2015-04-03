// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.IO;
using nUpdate.Core;

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

        /// <summary>
        ///     Loads the project configuration from the file in the application's main folder.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ProjectConfiguration> Load()
        {
            return
                Serializer.Deserialize<IEnumerable<ProjectConfiguration>>(
                    File.ReadAllText(Program.ProjectsConfigFilePath));
        }
    }
}