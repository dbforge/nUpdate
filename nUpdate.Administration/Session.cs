using System.IO;
using System.Linq;
using nUpdate.Administration.Logging;
using nUpdate.Administration.Proxy;
using nUpdate.Administration.Sql;

// ReSharper disable InconsistentNaming

namespace nUpdate.Administration
{
    internal static class Session
    {
        static Session()
        {
            AvailableLocations = Serializer.Deserialize<TrulyObservableCollection<UpdateProjectLocation>>(File.ReadAllText(FilePathProvider.ProjectsConfigFilePath)) ?? new TrulyObservableCollection<UpdateProjectLocation>();
            AvailableLocations.CollectionChanged += (sender, args) => 
                File.WriteAllText(FilePathProvider.ProjectsConfigFilePath, Serializer.Serialize(AvailableLocations.ToList()));
        }

        /// <summary>
        ///     Gets the active <see cref="UpdateProject"/> of the <see cref="Session"/>.
        /// </summary>
        internal static UpdateProject ActiveProject { get; private set; }

        /// <summary>
        ///     Gets the <see cref="Administration.UpdateFactory"/> of the <see cref="Session"/> for managing updates.
        /// </summary>
        internal static UpdateFactory UpdateFactory { get; private set; }

        /// <summary>
        ///     Gets the <see cref="PackageActionLogger"/> of the <see cref="Session"/> for the package history.
        /// </summary>
        internal static PackageActionLogger Logger { get; private set; }

        /// <summary>
        ///     Gets the <see cref="Administration.TransferManager"/> of the <see cref="Session"/> for data transfers.
        /// </summary>
        internal static TransferManager TransferManager { get; private set; }

        /// <summary>
        ///     Gets the path of the file containing the <see cref="UpdateProject"/> data of the <see cref="Session"/>.
        /// </summary>
        internal static string ProjectFilePath
            => AvailableLocations.First(x => x.Guid == ActiveProject.Guid).LastSeenPath;

        /// <summary>
        ///     Gets the <see cref="Sql.SqlManager"/> of the <see cref="Session"/> for managing statistics entries.
        /// </summary>
        internal static SqlManager SQLManager { get; private set; }

        /// <summary>
        ///     Gets the <see cref="Proxy.ProxyManager"/> of the <see cref="Session"/> for managing proxies.
        /// </summary>
        internal static ProxyManager ProxyManager { get; set; }

        /// <summary>
        ///     Gets or sets the path of the local package data folders of the current <see cref="ActiveProject"/>.
        /// </summary>
        internal static string PackagesPath { get; set; }

        internal static TrulyObservableCollection<UpdateProjectLocation> AvailableLocations { get; set; }
        
        /// <summary>
        ///     Initializes the <see cref="Session"/> with the specified <see cref="UpdateProject"/>.
        /// </summary>
        /// <param name="project">The <see cref="UpdateProject"/> of the <see cref="Session"/>.</param>
        internal static void InitializeProject(UpdateProject project)
        {
            ActiveProject = project;
            UpdateFactory = new UpdateFactory(project);
            Logger = new PackageActionLogger(project);
            TransferManager = new TransferManager(project);
            SQLManager = new SqlManager(project);
            ProxyManager = new ProxyManager();
            PackagesPath = Path.Combine(FilePathProvider.Path, "Projects", project.Guid.ToString());

            ActiveProject.PropertyChanged += (sender, args) => ActiveProject.Save();
        }

        internal static void Terminate()
        {
            ActiveProject = default(UpdateProject);
            UpdateFactory = null;
            Logger = null;
            TransferManager?.Dispose();
            TransferManager = null;
        }
    }
}