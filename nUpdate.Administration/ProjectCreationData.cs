// Author: Dominic Beger (Trade/ProgTrade) 2017

namespace nUpdate.Administration
{
    public class ProjectCreationData
    {
        /// <summary>
        ///     Gets or sets the location (directory path) of the project.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        ///     Gets or sets the private key associated with the project.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        ///     Gets or sets the project data.
        /// </summary>
        public UpdateProject Project { get; set; } = new UpdateProject();
    }
}