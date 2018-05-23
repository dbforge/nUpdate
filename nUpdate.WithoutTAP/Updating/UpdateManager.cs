// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
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
    // WITHOUT TAP
    /// <summary>
    ///     Provides functionality to update .NET-applications.
    /// </summary>
    public partial class UpdateManager
    {
        private readonly ManualResetEvent _searchManualResetEvent = new ManualResetEvent(false);

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
            _searchManualResetEvent.Dispose();
            _disposed = true;
        }

        /// <summary>
        ///     Downloads the available update packages from the server.
        /// </summary>
        /// <seealso cref="DownloadPackagesAsync" />
        public void DownloadPackages()
        {
            OnUpdateDownloadStarted(this, EventArgs.Empty);

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
                                    received += size;
                                    OnUpdateDownloadProgressChanged(received,
                                        (long) total, (float) (received / total) * 100);
                                    size = input.Read(buffer, 0, buffer.Length);
                                }

                                if (_downloadCancellationTokenSource.IsCancellationRequested)
                                    return;

                                if (!updateConfiguration.UseStatistics || !IncludeCurrentPcIntoStatistics)
                                    continue;

                                var response =
                                    new WebClient {Credentials = HttpAuthenticationCredentials}.DownloadString(
                                        $"{updateConfiguration.UpdatePhpFileUri}?versionid={updateConfiguration.VersionId}&os={SystemInformation.OperatingSystemName}"); // Only for calling it

                                if (string.IsNullOrEmpty(response))
                                    return;
                                OnStatisticsEntryFailed(new StatisticsException(string.Format(
                                    _lp.StatisticsScriptExceptionText, response)));
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
        public void DownloadPackagesAsync()
        {
            OnUpdateDownloadStarted(this, EventArgs.Empty);

            Task.Factory.StartNew(() =>
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
                    if (_downloadCancellationTokenSource.IsCancellationRequested)
                        break;

                    WebResponse webResponse = null;
                    try
                    {
                        // TODO: Implement cancellation for this method in case of e.g. a timeout
                        // Thread will currently continue until the timeout message is thrown and then cancel in the background
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
                                        if (_downloadCancellationTokenSource.IsCancellationRequested)
                                            break;

                                        fileStream.Write(buffer, 0, size);
                                        received += size;
                                        OnUpdateDownloadProgressChanged(received,
                                            (long) total, (float) (received / total) * 100);
                                        size = input.Read(buffer, 0, buffer.Length);
                                    }

                                    if (_downloadCancellationTokenSource.IsCancellationRequested)
                                        return;

                                    if (!updateConfiguration.UseStatistics || !IncludeCurrentPcIntoStatistics)
                                        continue;

                                    var response =
                                        new WebClient {Credentials = HttpAuthenticationCredentials}.DownloadString(
                                            $"{updateConfiguration.UpdatePhpFileUri}?versionid={updateConfiguration.VersionId}&os={SystemInformation.OperatingSystemName}"); // Only for calling it

                                    if (string.IsNullOrEmpty(response))
                                        return;
                                    OnStatisticsEntryFailed(new StatisticsException(string.Format(
                                        _lp.StatisticsScriptExceptionText, response)));
                                }
                            }
                        }
                    }
                    finally
                    {
                        webResponse?.Close();
                    }
                }
            }).ContinueWith(DownloadTaskCompleted);
        }

        private void DownloadTaskCompleted(Task task)
        {
            if (_downloadCancellationTokenSource.IsCancellationRequested)
                return;

            var exception = task.Exception;
            if (exception != null)
                OnUpdateDownloadFailed(exception.InnerException ?? exception);
            else
                OnUpdateDownloadFinished(this, EventArgs.Empty);
        }

        protected virtual void OnStatisticsEntryFailed(Exception exception)
        {
            StatisticsEntryFailed?.Invoke(this, new FailedEventArgs(exception));
        }

        protected virtual void OnUpdateDownloadFailed(Exception exception)
        {
            PackagesDownloadFailed?.Invoke(this, new FailedEventArgs(exception));
        }

        protected virtual void OnUpdateDownloadFinished(object sender, EventArgs e)
        {
            PackagesDownloadFinished?.Invoke(sender, e);
        }

        protected virtual void OnUpdateDownloadProgressChanged(long bytesReceived, long totalBytesToReceive,
            float percentage)
        {
            PackagesDownloadProgressChanged?.Invoke(this,
                new UpdateDownloadProgressChangedEventArgs(bytesReceived, totalBytesToReceive, percentage));
        }

        protected virtual void OnUpdateDownloadStarted(object sender, EventArgs e)
        {
            PackagesDownloadStarted?.Invoke(sender, e);
        }

        protected virtual void OnUpdateSearchFailed(Exception exception)
        {
            UpdateSearchFailed?.Invoke(this, new FailedEventArgs(exception));
        }

        protected virtual void OnUpdateSearchFinished(bool updateAvailable)
        {
            UpdateSearchFinished?.Invoke(this, new UpdateSearchFinishedEventArgs(updateAvailable));
        }

        protected virtual void OnUpdateSearchStarted(object sender, EventArgs e)
        {
            UpdateSearchStarted?.Invoke(sender, e);
        }

        /// <summary>
        ///     Occurs when the download of the packages fails.
        /// </summary>
        public event EventHandler<FailedEventArgs> PackagesDownloadFailed;

        /// <summary>
        ///     Occurs when the download of the packages has finished.
        /// </summary>
        public event EventHandler<EventArgs> PackagesDownloadFinished;

        /// <summary>
        ///     Occurs when the download progress of the packages has changed.
        /// </summary>
        public event EventHandler<UpdateDownloadProgressChangedEventArgs> PackagesDownloadProgressChanged;

        /// <summary>
        ///     Occurs when the download of the packages begins.
        /// </summary>
        public event EventHandler<EventArgs> PackagesDownloadStarted;

        /// <summary>
        ///     Searches for updates.
        /// </summary>
        /// <returns>Returns <c>true</c> if updates were found; otherwise, <c>false</c>.</returns>
        public bool SearchForUpdates()
        {
            // It may be that this is not the first search call and previously saved data needs to be disposed.
            Cleanup();

            OnUpdateSearchStarted(this, EventArgs.Empty);
            if (!ConnectionManager.IsConnectionAvailable())
                return false;

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            var configuration =
                UpdateConfiguration.Download(UpdateConfigurationFileUri, HttpAuthenticationCredentials, Proxy,
                    SearchTimeout);

            var result = new UpdateResult(configuration, CurrentVersion,
                IncludeAlpha, IncludeBeta);
            if (!result.UpdatesFound)
                return false;

            PackageConfigurations = result.NewestConfigurations;
            double updatePackageSize = 0;
            foreach (var updateConfiguration in PackageConfigurations)
            {
                updateConfiguration.UpdatePackageUri = ConvertPackageUri(updateConfiguration.UpdatePackageUri);
                updateConfiguration.UpdatePhpFileUri = ConvertStatisticsUri(updateConfiguration.UpdatePhpFileUri);

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
        public void SearchForUpdatesAsync()
        {
            OnUpdateSearchStarted(this, EventArgs.Empty);

            Task.Factory.StartNew(() =>
            {
                // It may be that this is not the first search call and previously saved data needs to be disposed.
                Cleanup();

                // Reinitialize the cancellation tokens and wait handles
                _searchCancellationTokenSource?.Dispose();
                _searchCancellationTokenSource = new CancellationTokenSource();
                _searchManualResetEvent.Reset();

                if (!ConnectionManager.IsConnectionAvailable())
                    return false;

                // Check for SSL and ignore it
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

                IEnumerable<UpdateConfiguration> configurations = null;
                Exception exception = null;
                UpdateConfiguration.DownloadAsync(UpdateConfigurationFileUri, HttpAuthenticationCredentials,
                    Proxy,
                    (c, e) =>
                    {
                        configurations = c;
                        exception = e;
                        _searchManualResetEvent.Set();
                    }, _searchCancellationTokenSource, SearchTimeout);
                _searchManualResetEvent.WaitOne();

                // Check for cancellation before throwing any errors
                if (_searchCancellationTokenSource.IsCancellationRequested)
                    return false;
                if (exception != null)
                    throw exception;

                var result = new UpdateResult(configurations, CurrentVersion,
                    IncludeAlpha, IncludeBeta);
                if (!result.UpdatesFound)
                    return false;

                PackageConfigurations = result.NewestConfigurations;
                double updatePackageSize = 0;
                foreach (var updateConfiguration in PackageConfigurations)
                {
                    updateConfiguration.UpdatePackageUri = ConvertPackageUri(updateConfiguration.UpdatePackageUri);
                    updateConfiguration.UpdatePhpFileUri = ConvertStatisticsUri(updateConfiguration.UpdatePhpFileUri);

                    if (_searchCancellationTokenSource.IsCancellationRequested)
                        return false;

                    var newPackageSize = GetUpdatePackageSize(updateConfiguration.UpdatePackageUri);
                    if (newPackageSize == null)
                        throw new SizeCalculationException(_lp.PackageSizeCalculationExceptionText);

                    updatePackageSize += newPackageSize.Value;
                    _packageOperations.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                        updateConfiguration.Operations);
                }

                TotalSize = updatePackageSize;
                return true;
            }).ContinueWith(SearchTaskCompleted);
        }

        private void SearchTaskCompleted(Task<bool> task)
        {
            if (_searchCancellationTokenSource.IsCancellationRequested)
                return;

            var exception = task.Exception;
            if (exception != null)
                OnUpdateSearchFailed(exception.InnerException ?? exception);
            OnUpdateSearchFinished(task.Result);
        }

        /// <summary>
        ///     Occurs when the statistics entry failed.
        /// </summary>
        /// <remarks>
        ///     This event is meant to provide the user with a warning, if the statistic server entry fails. The update process
        ///     should not be canceled as this does not cause any problems that could affect it.
        /// </remarks>
        public event EventHandler<FailedEventArgs> StatisticsEntryFailed;

        /// <summary>
        ///     Occurs when an update search has failed.
        /// </summary>
        public event EventHandler<FailedEventArgs> UpdateSearchFailed;

        /// <summary>
        ///     Occurs when an active update search has finished.
        /// </summary>
        public event EventHandler<UpdateSearchFinishedEventArgs> UpdateSearchFinished;

        /// <summary>
        ///     Occurs when an update search is started.
        /// </summary>
        public event EventHandler<EventArgs> UpdateSearchStarted;
    }
}