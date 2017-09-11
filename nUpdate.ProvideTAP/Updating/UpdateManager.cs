// Copyright © Dominic Beger 2017

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Exceptions;
using nUpdate.Internal.Core;
using nUpdate.UpdateEventArgs;

namespace nUpdate.Updating
{
    // PROVIDE TAP
    /// <summary>
    ///     Provides functionality to update .NET-applications.
    /// </summary>
    public partial class UpdateManager
    {
        /// <summary>
        ///     Releases all managed and unmanaged resources used by the current <see cref="UpdateManager" />-instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _searchCancellationTokenSource.Dispose();
            _downloadCancellationTokenSource.Dispose();
            _disposed = true;
        }

        /// <summary>
        ///     Downloads the available update packages from the server.
        /// </summary>
        /// <seealso cref="DownloadPackagesAsync" />
        public void DownloadPackages()
        {
            if (!Directory.Exists(_applicationUpdateDirectory))
                Directory.CreateDirectory(_applicationUpdateDirectory);

            foreach (var updateConfiguration in PackageConfigurations)
            {
                WebResponse webResponse = null;
                try
                {
                    var webRequest = WebRequest.Create(updateConfiguration.UpdatePackageUri);
                    if (HttpAuthenticationCredentials != null)
                        webRequest.Credentials = HttpAuthenticationCredentials;
                    using (webResponse = webRequest.GetResponse())
                    {
                        var buffer = new byte[1024];
                        _packageFilePaths.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                            Path.Combine(_applicationUpdateDirectory,
                                $"{updateConfiguration.LiteralVersion}.zip"));
                        using (var fileStream = File.Create(Path.Combine(_applicationUpdateDirectory,
                            $"{updateConfiguration.LiteralVersion}.zip")))
                        {
                            using (var input = webResponse.GetResponseStream())
                            {
                                if (input == null)
                                    throw new Exception("The response stream couldn't be read.");

                                var size = input.Read(buffer, 0, buffer.Length);
                                while (size > 0)
                                {
                                    fileStream.Write(buffer, 0, size);
                                    size = input.Read(buffer, 0, buffer.Length);
                                }

                                if (!updateConfiguration.UseStatistics || !IncludeCurrentPcIntoStatistics)
                                    continue;

                                var response =
                                    new WebClient {Credentials = HttpAuthenticationCredentials}.DownloadString(
                                        $"{updateConfiguration.UpdatePhpFileUri}?versionid={updateConfiguration.VersionId}&os={SystemInformation.OperatingSystemName}"); // Only for calling it

                                if (string.IsNullOrEmpty(response))
                                    return;
                            }
                        }
                    }
                }
                finally
                {
                    webResponse?.Close();
                }
            }
        }

        /// <summary>
        ///     Downloads the available update packages from the server, asynchronously.
        /// </summary>
        /// <exception cref="OperationCanceledException" />
        /// <exception cref="StatisticsException" />
        /// <seealso cref="DownloadPackages" />
        public Task DownloadPackagesAsync(IProgress<UpdateDownloadProgressChangedEventArgs> progress = null)
        {
            return TaskEx.Run(async () =>
            {
                _downloadCancellationTokenSource?.Dispose();
                _downloadCancellationTokenSource = new CancellationTokenSource();

                long received = 0;
                var total = PackageConfigurations.Select(config => GetUpdatePackageSize(config.UpdatePackageUri))
                    .Where(updatePackageSize => updatePackageSize != null)
                    .Sum(updatePackageSize => updatePackageSize.Value);

                if (!Directory.Exists(_applicationUpdateDirectory))
                    Directory.CreateDirectory(_applicationUpdateDirectory);

                foreach (var updateConfiguration in PackageConfigurations)
                {
                    WebResponse webResponse = null;
                    try
                    {
                        if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                        {
                            DeletePackages();
                            Cleanup();
                            throw new OperationCanceledException();
                        }

                        var webRequest = WebRequest.Create(updateConfiguration.UpdatePackageUri);
                        if (HttpAuthenticationCredentials != null)
                            webRequest.Credentials = HttpAuthenticationCredentials;
                        webResponse = await webRequest.GetResponseAsync();

                        var buffer = new byte[1024];
                        _packageFilePaths.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                            Path.Combine(_applicationUpdateDirectory,
                                $"{updateConfiguration.LiteralVersion}.zip"));
                        using (var fileStream = File.Create(Path.Combine(_applicationUpdateDirectory,
                            $"{updateConfiguration.LiteralVersion}.zip")))
                        {
                            using (var input = webResponse.GetResponseStream())
                            {
                                if (input == null)
                                    throw new Exception("The response stream couldn't be read.");

                                if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    DeletePackages();
                                    Cleanup();
                                    throw new OperationCanceledException();
                                }

                                var size = await input.ReadAsync(buffer, 0, buffer.Length);
                                while (size > 0)
                                {
                                    if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                                    {
                                        fileStream.Flush();
                                        fileStream.Close();
                                        DeletePackages();
                                        Cleanup();
                                        throw new OperationCanceledException();
                                    }

                                    await fileStream.WriteAsync(buffer, 0, size);
                                    received += size;
                                    progress?.Report(new UpdateDownloadProgressChangedEventArgs(received,
                                        (long) total, (float) (received / total) * 100));
                                    size = await input.ReadAsync(buffer, 0, buffer.Length);
                                }

                                if (!updateConfiguration.UseStatistics || !IncludeCurrentPcIntoStatistics)
                                    continue;

                                var response =
                                    new WebClient
                                    {
                                        Credentials =
                                            HttpAuthenticationCredentials
                                    }.DownloadString(
                                        $"{updateConfiguration.UpdatePhpFileUri}?versionid={updateConfiguration.VersionId}&os={SystemInformation.OperatingSystemName}"); // Only for calling it
                                if (!string.IsNullOrEmpty(response))
                                    throw new StatisticsException(string.Format(
                                        _lp.StatisticsScriptExceptionText, response));
                            }
                        }
                    }
                    finally
                    {
                        webResponse?.Close();
                    }
                }
            });
        }

        /// <summary>
        ///     Searches for updates.
        /// </summary>
        /// <returns>Returns <c>true</c> if updates were found; otherwise, <c>false</c>.</returns>
        /// <exception cref="SizeCalculationException">The calculation of the size of the available updates has failed.</exception>
        public bool SearchForUpdates()
        {
            // It may be that this is not the first search call and previously saved data needs to be disposed.
            Cleanup();

            if (!ConnectionManager.IsConnectionAvailable())
                return false;

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            var configuration =
                UpdateConfiguration.Download(UpdateConfigurationFileUri, HttpAuthenticationCredentials, Proxy);

            var result = new UpdateResult(configuration, CurrentVersion,
                IncludeAlpha, IncludeBeta);
            if (!result.UpdatesFound)
                return false;

            PackageConfigurations = result.NewestConfigurations;
            double updatePackageSize = 0;
            foreach (var updateConfiguration in PackageConfigurations)
            {
                var newPackageSize = GetUpdatePackageSize(updateConfiguration.UpdatePackageUri);
                if (newPackageSize == null)
                    throw new SizeCalculationException(_lp.PackageSizeCalculationExceptionText);

                updatePackageSize += newPackageSize.Value;
                _packageOperations.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                    updateConfiguration.Operations);
            }

            TotalSize = updatePackageSize;
            return true;
        }

        /// <summary>
        ///     Searches for updates, asynchronously.
        /// </summary>
        /// <seealso cref="SearchForUpdates" />
        /// <exception cref="SizeCalculationException" />
        /// <exception cref="OperationCanceledException" />
        public Task<bool> SearchForUpdatesAsync()
        {
            return TaskEx.Run(async () =>
            {
                // It may be that this is not the first search call and previously saved data needs to be disposed.
                Cleanup();
                _searchCancellationTokenSource?.Dispose();
                _searchCancellationTokenSource = new CancellationTokenSource();

                if (!ConnectionManager.IsConnectionAvailable())
                    return false;
                
                _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                // Check for SSL and ignore it
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var configuration =
                    await UpdateConfiguration.DownloadAsync(UpdateConfigurationFileUri, HttpAuthenticationCredentials, Proxy, _searchCancellationTokenSource);

                _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var result = new UpdateResult(configuration, CurrentVersion,
                    IncludeAlpha, IncludeBeta);
                if (!result.UpdatesFound)
                    return false;

                PackageConfigurations = result.NewestConfigurations;
                double updatePackageSize = 0;
                foreach (var updateConfiguration in PackageConfigurations)
                {
                    _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var newPackageSize = GetUpdatePackageSize(updateConfiguration.UpdatePackageUri);
                    if (newPackageSize == null)
                        throw new SizeCalculationException(_lp.PackageSizeCalculationExceptionText);

                    updatePackageSize += newPackageSize.Value;
                    _packageOperations.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                        updateConfiguration.Operations);
                }

                TotalSize = updatePackageSize;
                if (!_searchCancellationTokenSource.Token.IsCancellationRequested)
                    return true;
                throw new OperationCanceledException();
            });
        }
    }
}