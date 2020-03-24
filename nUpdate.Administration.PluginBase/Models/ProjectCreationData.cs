// ProjectCreationData.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

namespace nUpdate.Administration.PluginBase.Models
{
    public class ProjectCreationData
    {
        /// <summary>
        ///     Gets or sets the location (directory path) of the project.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        ///     Gets or sets the private key of the project.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        ///     Gets or sets the update project data.
        /// </summary>
        public UpdateProject Project { get; set; } = new UpdateProject();
    }
}