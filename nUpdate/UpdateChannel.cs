// Copyright © Dominic Beger 2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace nUpdate
{
    [Serializable]
    public class UpdateChannel
    {
        public UpdateChannel()
        {
        }

        public UpdateChannel(string name, Uri uri, Version latestVersion, IEnumerable<string> supportedSuffixes)
        {
            Name = name;
            Uri = uri;
            LatestVersion = latestVersion;
            SupportedSuffixes = supportedSuffixes;
        }

        /// <summary>
        ///     Gets or sets the latest version of the <see cref="UpdateChannel" />.
        /// </summary>
        public Version LatestVersion { get; set; }

        /// <summary>
        ///     Gets or sets the name of the <see cref="UpdateChannel" />.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the supported semantic version suffixes of the <see cref="UpdateChannel" />.
        /// </summary>
        public IEnumerable<string> SupportedSuffixes { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="System.Uri" /> of the <see cref="UpdateChannel" />.
        /// </summary>
        public Uri Uri { get; set; }

        public static IEnumerable<UpdateChannel> GetDefaultMasterChannel(Uri updateDirectoryUri)
        {
            return new[]
            {
                new UpdateChannel("Alpha", new Uri(updateDirectoryUri, "/channels/alpha.json"), null,
                    new[] {"alpha", "a"}),
                new UpdateChannel("Beta", new Uri(updateDirectoryUri, "/channels/beta.json"), null,
                    new[] {"beta", "b"}),
                new UpdateChannel("ReleaseCandidate", new Uri(updateDirectoryUri, "/channels/releasecandidate.json"),
                    null,
                    new[] {"rc"}),
                new UpdateChannel("Release", new Uri(updateDirectoryUri, "/channels/release.json"), null,
                    Enumerable.Empty<string>())
            };
        }

        /// <summary>
        ///     Gathers the master channel data from the file located at the specified <see cref="System.Uri" />.
        /// </summary>
        /// <param name="masterChannelUri">The <see cref="System.Uri" /> of the master channel file.</param>
        /// <param name="proxy">The optional <see cref="WebProxy" /> to use.</param>
        public static async Task<IEnumerable<UpdateChannel>> GetMasterChannel(Uri masterChannelUri, WebProxy proxy)
        {
            using (var wc = new WebClientEx(5000))
            {
                wc.Encoding = Encoding.UTF8;
                if (proxy != null)
                    wc.Proxy = proxy;

                // Check for SSL and ignore it
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var source = await wc.DownloadStringTaskAsync(masterChannelUri);
                if (!string.IsNullOrEmpty(source))
                    return JsonSerializer.Deserialize<IEnumerable<UpdateChannel>>(source);
            }

            return Enumerable.Empty<UpdateChannel>();
        }

        /// <summary>
        ///     Tries to gather the master channel data from the file located at the specified <see cref="System.Uri" /> and
        ///     returns <c>null</c>, if the file doesn't exist.
        /// </summary>
        /// <param name="masterChannelUri">The <see cref="System.Uri" /> of the master channel file.</param>
        /// <param name="proxy">The optional <see cref="WebProxy" /> to use.</param>
        public static async Task<IEnumerable<UpdateChannel>> TryGetMasterChannel(Uri masterChannelUri, WebProxy proxy)
        {
            try
            {
                return await GetMasterChannel(masterChannelUri, proxy);
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.ProtocolError ||
                    ((HttpWebResponse) ex.Response).StatusCode != HttpStatusCode.NotFound)
                    throw;
            }
            return null;
        }
    }
}