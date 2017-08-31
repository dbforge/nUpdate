// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using nUpdate.Operations;

namespace nUpdate
{
    // TODO: Internal
    /// <summary>
    ///     Represents an update package.
    /// </summary>
    [Serializable]
    public class UpdatePackage : IDeepCopy<UpdatePackage>
    {
        /// <summary>
        ///     Gets or sets the <see cref="System.Guid"/> of the package.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        ///     Gets or sets the release date of the package.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        ///     Gets or sets the version of the package.
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        ///     Gets or sets the name of the channel that the update package is located in.
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the package should be included into the statistics, or not.
        /// </summary>
        public bool UseStatistics { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Uri"/> of the remote file that creates the statistics entries using the parameters that are handled over.
        /// </summary>
        public Uri UpdatePhpFileUri { get; set; }

        /// <summary>
        ///     Gets or sets the version ID of this package in the statistics database.
        /// </summary>
        public int VersionId { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Uri"/> of the update package.
        /// </summary>
        public Uri UpdatePackageUri { get; set; }

        /// <summary>
        ///     Gets or sets the changelogs of the update package accessed by their relating <see cref="CultureInfo"/>.
        /// </summary>
        public Dictionary<CultureInfo, string> Changelog { get; set; }

        /// <summary>
        ///     Gets or sets the signature of the update package represented as a Base64 string.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        ///     Gets or sets the literal versions of the update package that shouldn't be able to install this update package.
        /// </summary>
        public string[] UnsupportedVersions { get; set; }

        /// <summary>
        ///     Gets or sets the supported <see cref="nUpdate.Architecture"/>s of the update package.
        /// </summary>
        public Architecture Architecture { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Operation"/>s of the update package that should be executed during the installation process.
        /// </summary>
        public List<Operation> Operations { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the update package should be favored over other packages, even if they have a higher <see cref="UpdateVersion"/>, or not.
        /// </summary>
        public bool NecessaryUpdate { get; set; }
        
        /// <summary>
        ///     Gets or sets the description of the package.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the package is released, or not.
        /// </summary>
        public bool IsReleased { get; set; }

        /// <summary>
        ///     Performs a deep copy of the current <see cref="UpdatePackage" /> instance.
        /// </summary>
        public UpdatePackage DeepCopy()
        {
            return (UpdatePackage)MemberwiseClone();
        }

        /// <summary>
        ///     Gathers the available <see cref="UpdatePackage"/> data from the file located at the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="configFileUri">The <see cref="Uri"/> of the <see cref="UpdatePackage"/> data file.</param>
        /// <param name="proxy">The optional <see cref="WebProxy"/> to use.</param>
        public static async Task<IEnumerable<UpdatePackage>> GetRemotePackageData(Uri configFileUri, WebProxy proxy)
        {
            using (var wc = new WebClientEx(10000))
            {
                wc.Encoding = Encoding.UTF8;
                if (proxy != null)
                    wc.Proxy = proxy;

                // Check for SSL and ignore it
                ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };
                var source = await wc.DownloadStringTaskAsync(configFileUri);
                if (!string.IsNullOrEmpty(source))
                    return JsonSerializer.Deserialize<IEnumerable<UpdatePackage>>(source);
            }

            return Enumerable.Empty<UpdatePackage>();
        }

        // TODO: Change within changes in the versioning system
        /// <summary>
        ///     Loads all available <see cref="UpdatePackage"/>s from a local file at the specified path.
        /// </summary>
        /// <param name="filePath">The path of the local file.</param>
        public static IEnumerable<UpdatePackage> FromFile(string filePath)
        {
            return JsonSerializer.Deserialize<IEnumerable<UpdatePackage>>(File.ReadAllText(filePath)) ?? Enumerable.Empty<UpdatePackage>();
        }
    }
}