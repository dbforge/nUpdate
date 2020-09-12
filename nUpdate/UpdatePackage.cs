// UpdatePackage.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace nUpdate
{
    /// <summary>
    ///     Represents an update package.
    /// </summary>
    [Serializable]
    public class UpdatePackage
    {
        /// <summary>
        ///     Gets or sets the supported <see cref="nUpdate.Architecture" />s of the update package.
        /// </summary>
        public Architecture Architecture { get; set; }

        /// <summary>
        ///     Gets or sets the changelog of the update package.
        /// </summary>
        public Dictionary<CultureInfo, string> Changelog { get; set; }

        /// <summary>
        ///     Gets or sets the name of the channel that the update package is located in.
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the update package should be installed, even if there are newer versions
        ///     available (follows <see cref="UpdateStrategy.AllNewest" />), or not (follows
        ///     <see cref="UpdateStrategy.OnlyLatest" />).
        /// </summary>
        public bool Compulsory { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="System.Guid" /> of the package.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the package is released, or not.
        /// </summary>
        public bool IsReleased { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Uri" /> of the update package.
        /// </summary>
        public Uri PackageUri { get; set; }

        /// <summary>
        ///     Gets or sets the release date of the package.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        public UpdateRolloutConditionMode RolloutConditionMode { get; set; }


        public IEnumerable<UpdateRolloutCondition> RolloutConditions { get; set; }

        /// <summary>
        ///     Gets or sets the signature of the update package represented as a Base64 string.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        ///     Gets or sets the size of the package.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        ///     Gets or sets the versions that shouldn't be able to install this update package.
        /// </summary>
        public IEnumerable<SemanticVersion> UnsupportedVersions { get; set; }

        /// <summary>
        ///     Gets or sets the version of the package.
        /// </summary>
        public IVersion Version { get; set; }

        public static async Task<IEnumerable<UpdatePackage>> GetPackageEnumerable(Uri packageDataFileUri,
            WebProxy proxy)
        {
            if (Utility.IsHttpUri(packageDataFileUri))
                using (var wc = new CustomWebClient(10000))
                {
                    wc.Encoding = Encoding.UTF8;
                    if (proxy != null)
                        wc.Proxy = proxy;

                    var source = await wc.DownloadStringFrom(packageDataFileUri);
                    if (!string.IsNullOrEmpty(source))
                        return JsonSerializer.Deserialize<IEnumerable<UpdatePackage>>(source);
                }
            else
                using (var reader = File.OpenText(packageDataFileUri.ToString()))
                {
                    var content = await reader.ReadToEndAsync();
                    return JsonSerializer.Deserialize<IEnumerable<UpdatePackage>>(content);
                }

            return Enumerable.Empty<UpdatePackage>();
        }
    }
}