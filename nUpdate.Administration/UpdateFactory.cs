using nUpdate.Administration.Application;
using nUpdate.Administration.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration
{
    internal class UpdateFactory
    {
        private readonly UpdateProject _project;

        internal UpdateFactory(UpdateProject project)
        {
            _project = project;
        }

        internal IEnumerable<UpdatePackage> LoadPackageData()
        {
            return _project.Packages;
        }

        internal async Task RemoveUpdate(IUpdateVersion updateVersion, CancellationToken cancellationToken, IProgress<TransferProgressEventArgs> progress)
        {
            // First, remove the package data from the relating file. Background: If removing the package file fails, we can still be sure that the package won't be downloaded any longer.
            await RemovePackageData(updateVersion, cancellationToken, progress);

            if (await Session.TransferManager.Exists(updateVersion.ToString()))
                await Session.TransferManager.DeleteDirectory(updateVersion.ToString());

            var destinationPackage =
                _project.Packages.FirstOrDefault(item => new UpdateVersion(item.LiteralVersion).Equals(updateVersion));
            if (destinationPackage != null)
            {
                _project.Packages.Remove(destinationPackage);
                _project.Save();
            }
        }

        internal async Task PushUpdateLocally(UpdateFactoryPackage factoryPackage)
        {
            await Task.Run(() =>
            {
                _project.Packages.Add(factoryPackage.PackageData);
                _project.Save();

                File.WriteAllText(
                    Path.Combine(FilePathProvider.Path, "Projects", _project.Guid.ToString(),
                        factoryPackage.PackageData.LiteralVersion, "updates.json"),
                    Serializer.Serialize(factoryPackage.PackageData));
            });
        }

        internal async Task PushUpdate(UpdateFactoryPackage factoryPackage, CancellationToken cancellationToken,
            IProgress<TransferProgressEventArgs> progress)
        {
            var package = factoryPackage.PackageData;
            package.IsReleased = true;
            _project.Packages.Add(package);
            _project.Save();

            // TODO: Statistics stuff, if enabled
            //if (Project.UseStatistics)
            //{
            //    Invoke(new Action(() => loadingLabel.Text = "Connecting to SQL-server..."));
            //    try
            //    {
            //        var connectionString = $"SERVER={Project.SqlWebUrl};" +
            //                               $"DATABASE={Project.SqlDatabaseName};" +
            //                               $"UID={Project.SqlUsername};" +
            //                               $"PASSWORD={Project.RuntimeSqlPassword.ConvertToInsecureString()};";

            //        _insertConnection = new MySqlConnection(connectionString);
            //        _insertConnection.Open();
            //    }
            //    catch (MySqlException ex)
            //    {
            //        Invoke(
            //            new Action(
            //                () =>
            //                    Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
            //                        ex, PopupButtons.Ok)));
            //        _insertConnection.Close();
            //        Reset();
            //        return;
            //    }
            //    catch (Exception ex)
            //    {
            //        Invoke(
            //            new Action(
            //                () =>
            //                    Popup.ShowPopup(this, SystemIcons.Error,
            //                        "Error while connecting to the database.",
            //                        ex, PopupButtons.Ok)));
            //        _insertConnection.Close();
            //        Reset();
            //        return;
            //    }

            //    Invoke(new Action(() => loadingLabel.Text = "Executing SQL-commands..."));

            //    var command = _insertConnection.CreateCommand();
            //    command.CommandText =
            //        $"INSERT INTO `Version` (`ID`, `Version`, `Application_ID`) VALUES ({Settings.Default.VersionID}, \"{_packageVersion}\", {Project.ApplicationId});";
            //    // SQL-injections are impossible as conversions to the relating datatype would already fail if any injection statements were attached (would have to be a string then)

            //    try
            //    {
            //        command.ExecuteNonQuery();
            //    }
            //    catch (Exception ex)
            //    {
            //        Invoke(
            //            new Action(
            //                () =>
            //                    Popup.ShowPopup(this, SystemIcons.Error, "Error while executing the commands.",
            //                        ex, PopupButtons.Ok)));
            //        _insertConnection.Close();
            //        Reset();
            //        return;
            //    }
            //}

            // Upload the package
            await Session.TransferManager.UploadPackage(factoryPackage.ArchivePath, factoryPackage.PackageData.LiteralVersion, cancellationToken, progress);
            await PushPackageData(factoryPackage.PackageData, cancellationToken, progress);

            Session.Logger.AppendEntry(PackageActionType.UploadPackage,
                new UpdateVersion(factoryPackage.PackageData.LiteralVersion));
        }

        internal async Task RemovePackageData(IUpdateVersion updateVersion, CancellationToken cancellationToken, IProgress<TransferProgressEventArgs> progress)
        {
            var updateDataList = UpdatePackage.GetRemotePackageData(new Uri(_project.UpdateDirectoryUri, "updates.json"), _project.ProxyData.Proxy).ToList();
            var destinationPackage =
                updateDataList.FirstOrDefault(item => new UpdateVersion(item.LiteralVersion).Equals(updateVersion));
            if (destinationPackage != null)
                updateDataList.Remove(destinationPackage);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(Serializer.Serialize(updateDataList))))
            {
                // TODO: Upload the package data
                await Session.TransferManager.UploadFile(stream, "updates.json", progress);
            }
        }

        internal async Task PushPackageData(UpdatePackage updatePackage, CancellationToken cancellationToken, IProgress<TransferProgressEventArgs> progress)
        {
            var updateDataList = UpdatePackage.GetRemotePackageData(new Uri(_project.UpdateDirectoryUri, "updates.json"), _project.ProxyData.Proxy).ToList();
            updateDataList.Add(updatePackage);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(Serializer.Serialize(updateDataList))))
            {
                // TODO: Upload the package data
                await Session.TransferManager.UploadFile(stream, "updates.json", progress);
            }
        }

        internal async Task PushDefaultPackageData(CancellationToken cancellationToken, IProgress<TransferProgressEventArgs> progress)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(Serializer.Serialize(Enumerable.Empty<UpdatePackage>()))))
            {
                await Session.TransferManager.UploadFile(stream, "updates.json", progress);
            }
        }
    }
}