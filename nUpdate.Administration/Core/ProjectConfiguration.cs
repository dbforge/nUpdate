// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using nUpdate.Internal.Core;

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
        ///     The path of the project-file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     Loads the project configuration from the file in the application's main folder.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ProjectConfiguration> Load()
        {
            string content = File.ReadAllText(Program.ProjectsConfigFilePath);
            return !string.IsNullOrEmpty(content)
                ? Serializer.Deserialize<IEnumerable<ProjectConfiguration>>(content)
                : Enumerable.Empty<ProjectConfiguration>();
        }
    }
}