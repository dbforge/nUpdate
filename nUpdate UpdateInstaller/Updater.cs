// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Ionic.Zip;
using Microsoft.Win32;
using nUpdate.UpdateInstaller.Client.GuiInterface;
using nUpdate.UpdateInstaller.Core;
using nUpdate.UpdateInstaller.Core.Operations;
using nUpdate.UpdateInstaller.UI.Dialogs;
using Newtonsoft.Json.Linq;
using nUpdate.UpdateInstaller.UI.Popups;

namespace nUpdate.UpdateInstaller
{
    public class Updater
    {
        private int _doneTaskAmount;
        private IProgressReporter _progressReporter;
        private int _totalTaskCount;

        /// <summary>
        ///     Runs the updating process.
        /// </summary>
        public void RunUpdate()
        {
            try
            {
                _progressReporter = GetGui();
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(SystemIcons.Error, "Error while initializing the graphic user interface.", ex,
                    PopupButtons.Ok);
                return;
            }

            ThreadPool.QueueUserWorkItem(arg => RunUpdateAsync());
            try
            {
                _progressReporter.Initialize();
            }
            catch (Exception ex)
            {
                _progressReporter.InitializingFail(ex);
                _progressReporter.Terminate();
            }
        }

        /// <summary>
        ///     Loads the GUI either from a given external assembly, if one is set, or otherwise, from the integrated GUI.
        /// </summary>
        /// <returns>
        ///     Returns a new instance of the given object that implements the
        ///     <see cref="nUpdate.UpdateInstaller.Client.GuiInterface.IProgressReporter"/>-interface.
        /// </returns>
        private IProgressReporter GetGui()
        {
            if (String.IsNullOrEmpty(Program.ExternalGuiAssemblyPath) || !File.Exists(Program.ExternalGuiAssemblyPath))
                return (IProgressReporter) Activator.CreateInstance(typeof (MainForm));

            var assembly = Assembly.LoadFrom(Program.ExternalGuiAssemblyPath);
            var validTypes =
                assembly.GetTypes().Where(item => typeof (IProgressReporter).IsAssignableFrom(item)).ToList();
            // If any class implements the interface, load it...
            if (validTypes.Count <= 0)
                return (IProgressReporter) Activator.CreateInstance(typeof (MainForm));

            try
            {
                return (IProgressReporter) Activator.CreateInstance(validTypes.First());
            }
            catch (Exception) // No parameterless constructor as the "CreateInstance"-method wants a default constructor
            {
                return (IProgressReporter) Activator.CreateInstance(typeof (MainForm));
            }
        }

        /// <summary>
        ///     Runs the updating process. This method does not block the calling thread.
        /// </summary>
        private void RunUpdateAsync()
        {
            try
            {
                using (var zf = ZipFile.Read(Program.PackageFile))
                {
                    zf.ParallelDeflateThreshold = -1;
                    try
                    {
                        foreach (var entry in zf)
                        {
                            entry.Extract(Directory.GetParent(Program.PackageFile).FullName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _progressReporter.Fail(ex);
                        CleanUp();
                        _progressReporter.Terminate();
                        return;
                    }
                }

                var directories = Directory.GetParent(Program.PackageFile).GetDirectories();
                _totalTaskCount += directories.Sum(
                    directory => Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories).Length);
                
                _totalTaskCount += Program.Operations.Count(item => item.Area != OperationArea.Registry &&  item.Method != OperationMethod.Delete);

                foreach (
                    var secondValueAsArray in
                        Program.Operations.Where(
                            item => item.Area == OperationArea.Registry && item.Method != OperationMethod.SetValue)
                            .Select(registryOperation => registryOperation.Value2)
                            .OfType<JArray>())
                    _totalTaskCount += secondValueAsArray.ToObject<IEnumerable<string>>().Count();

                foreach (
                    var secondValueAsArray in
                        Program.Operations.Where(
                            item => item.Area == OperationArea.Files && item.Method == OperationMethod.Delete)
                            .Select(registryOperation => registryOperation.Value2)
                            .OfType<JArray>())
                    _totalTaskCount += secondValueAsArray.ToObject<IEnumerable<string>>().Count();

                foreach (
                    var secondValueAsArray in
                        Program.Operations.Where(
                            item => item.Area == OperationArea.Registry && item.Method == OperationMethod.SetValue)
                            .Select(registryOperation => registryOperation.Value2)
                            .OfType<JArray>())
                    _totalTaskCount +=
                        secondValueAsArray.ToObject<IEnumerable<Tuple<string, object, RegistryValueKind>>>().Count();

                foreach (var directory in directories)
                {
                    switch (directory.Name)
                    {
                        case "Program":
                            CopyDirectoryRecursively(directory.FullName, Program.AimFolder);
                            break;
                        case "AppData":
                            CopyDirectoryRecursively(directory.FullName,
                                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                            break;
                        case "Temp":
                            CopyDirectoryRecursively(directory.FullName, Path.GetTempPath());
                            break;
                        case "Desktop":
                            CopyDirectoryRecursively(directory.FullName,
                                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _progressReporter.Fail(ex);
                CleanUp();
                _progressReporter.Terminate();
                return;
            }

            try
            {
                foreach (var operation in Program.Operations)
                {
                    float percentage;
                    JArray secondValueAsArray;
                    switch (operation.Area)
                    {
                        case OperationArea.Files:
                            switch (operation.Method)
                            {
                                case OperationMethod.Delete:
                                    var deleteFilePathParts = operation.Value.Split('\\');
                                    var deleteFileFullPath = Path.Combine(
                                        Operation.GetDirectory(deleteFilePathParts[0]),
                                        String.Join("\\",
                                            deleteFilePathParts.Where(item => item != deleteFilePathParts[0])));
                                    secondValueAsArray = operation.Value2 as JArray;
                                    if (secondValueAsArray != null)
                                        foreach (var fileToDelete in secondValueAsArray.ToObject<IEnumerable<string>>())
                                        {
                                            string path = Path.Combine(deleteFileFullPath, fileToDelete);
                                            if (File.Exists(path))
                                                File.Delete(path);

                                            _doneTaskAmount += 1;
                                            percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                                            _progressReporter.ReportOperationProgress(percentage,
                                                String.Format(Program.FileDeletingOperationText, fileToDelete));
                                        }
                                    break;

                                case OperationMethod.Rename:
                                    var renameFilePathParts = operation.Value.Split('\\');
                                    var renameFileFullPath = Path.Combine(
                                        Operation.GetDirectory(renameFilePathParts[0]),
                                        String.Join("\\",
                                            renameFilePathParts.Where(item => item != renameFilePathParts[0])));
                                    if (File.Exists(renameFileFullPath))
                                        File.Move(renameFileFullPath,
                                            Path.Combine(Directory.GetParent(renameFileFullPath).FullName,
                                            operation.Value2.ToString()));

                                    _doneTaskAmount += 1;
                                    percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                                    MessageBox.Show(percentage.ToString());
                                    _progressReporter.ReportOperationProgress(percentage,
                                        String.Format(Program.FileRenamingOperationText,
                                            Path.GetFileName(operation.Value),
                                            operation.Value2));
                                    break;
                            }
                            break;
                        case OperationArea.Registry:
                            switch (operation.Method)
                            {
                                case OperationMethod.Create:
                                    secondValueAsArray = operation.Value2 as JArray;
                                    if (secondValueAsArray != null)
                                        foreach (var registryKey in secondValueAsArray.ToObject<IEnumerable<string>>())
                                        {
                                            RegistryManager.CreateSubKey(operation.Value, registryKey);

                                            _doneTaskAmount += 1;
                                            percentage = ((float)_doneTaskAmount / _totalTaskCount) * 100f;
                                            _progressReporter.ReportOperationProgress(percentage,
                                                String.Format(Program.RegistrySubKeyCreateOperationText, registryKey));
                                        }
                                    break;

                                case OperationMethod.Delete:
                                    secondValueAsArray = operation.Value2 as JArray;
                                    if (secondValueAsArray != null)
                                        foreach (var registryKey in secondValueAsArray.ToObject<IEnumerable<string>>())
                                        {
                                            RegistryManager.DeleteSubKey(operation.Value, registryKey);

                                            _doneTaskAmount += 1;
                                            percentage = ((float)_doneTaskAmount / _totalTaskCount) * 100f;
                                            _progressReporter.ReportOperationProgress(percentage,
                                                String.Format(Program.RegistrySubKeyDeleteOperationText, registryKey));
                                        }
                                    break;

                                case OperationMethod.SetValue:
                                    secondValueAsArray = operation.Value2 as JArray;
                                    if (secondValueAsArray != null)
                                        foreach (
                                            var nameValuePair in
                                                secondValueAsArray
                                                    .ToObject<IEnumerable<Tuple<string, object, RegistryValueKind>>>())
                                        {
                                            RegistryManager.SetValue(operation.Value, nameValuePair.Item1,
                                                nameValuePair.Item2, nameValuePair.Item3);

                                            _doneTaskAmount += 1;
                                            percentage = ((float)_doneTaskAmount / _totalTaskCount) * 100f;
                                            _progressReporter.ReportOperationProgress(percentage,
                                                String.Format(Program.RegistryNameValuePairSetValueOperationText,
                                                    nameValuePair.Item1, nameValuePair.Item2));
                                        }
                                    break;

                                case OperationMethod.DeleteValue:
                                    secondValueAsArray = operation.Value2 as JArray;
                                    if (secondValueAsArray != null)
                                        foreach (var valueName in secondValueAsArray.ToObject<IEnumerable<string>>())
                                        {
                                            RegistryManager.DeleteValue(operation.Value, valueName);

                                            _doneTaskAmount += 1;
                                            percentage = ((float)_doneTaskAmount / _totalTaskCount) * 100f;
                                            _progressReporter.ReportOperationProgress(percentage,
                                                String.Format(Program.RegistryNameValuePairSetValueOperationText,
                                                    valueName));
                                        }
                                    break;
                            }
                            break;

                        case OperationArea.Processes:
                            switch (operation.Method)
                            {
                                case OperationMethod.Start:
                                    var processFilePathParts = operation.Value.Split('\\');
                                    var processFileFullPath =
                                        Path.Combine(Operation.GetDirectory(processFilePathParts[0]),
                                            String.Join("\\",
                                                processFilePathParts.Where(item => item != processFilePathParts[0])));

                                    var process = new Process
                                    {
                                        StartInfo =
                                        {
                                            FileName = processFileFullPath,
                                            Arguments = operation.Value2.ToString()
                                        }
                                    };
                                    process.Start();

                                    _doneTaskAmount += 1;
                            percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                            _progressReporter.ReportOperationProgress(percentage,
                                String.Format(Program.ProcessStartOperationText, operation.Value));
                                    break;

                                case OperationMethod.Stop:
                                    var processes = Process.GetProcessesByName(operation.Value);
                                    foreach (var foundProcess in processes)
                                        foundProcess.Kill();

                                    _doneTaskAmount += 1;
                            percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                                    _progressReporter.ReportOperationProgress(percentage,
                                        String.Format(Program.ProcessStopOperationText, operation.Value));
                                    break;
                            }
                            break;

                        case OperationArea.Services:
                            switch (operation.Method)
                            {
                                case OperationMethod.Start:
                                    ServiceManager.StartService(operation.Value, (string[]) operation.Value2);

                                    _doneTaskAmount += 1;
                            percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                                    _progressReporter.ReportOperationProgress(percentage,
                                        String.Format(Program.ServiceStartOperationText, operation.Value));
                                    break;

                                case OperationMethod.Stop:
                                    ServiceManager.StopService(operation.Value);

                                    _doneTaskAmount += 1;
                            percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                                    _progressReporter.ReportOperationProgress(percentage,
                                        String.Format(Program.ServiceStopOperationText, operation.Value));
                                    break;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _progressReporter.Fail(ex);
                CleanUp();
                _progressReporter.Terminate();
                return;
            }

            CleanUp();

            var p = new Process
            {
                StartInfo = {UseShellExecute = true, FileName = Program.ApplicationExecutablePath}
            };
            p.Start();

            _progressReporter.Terminate();
        }

        /// <summary>
        ///     Cleans up all resources.
        /// </summary>
        private void CleanUp()
        {
            try
            {
                Directory.Delete(Directory.GetParent(Program.PackageFile).FullName, true);
            }
            catch (Exception ex)
            {
                _progressReporter.Fail(ex);
            }
        }

        /// <summary>
        ///     Performs a recursive copy of a given directory.
        /// </summary>
        /// <param name="sourceDirName">The path of the source directory.</param>
        /// <param name="destDirName">The path of the destination directory.</param>
        private void CopyDirectoryRecursively(string sourceDirName, string destDirName)
        {
            try
            {
                var dir = new DirectoryInfo(sourceDirName);
                var sourceDirectories = dir.GetDirectories();

                if (!Directory.Exists(destDirName))
                    Directory.CreateDirectory(destDirName);

                var files = dir.GetFiles();
                foreach (var file in files)
                {
                    var aimPath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(aimPath, true);

                    _doneTaskAmount += 1;
                    var percentage = ((float)_doneTaskAmount / _totalTaskCount) * 100f;
                    _progressReporter.ReportUnpackingProgress(percentage, file.Name);
                }

                foreach (var subDirectories in sourceDirectories)
                {
                    var aimDirectoryPath = Path.Combine(destDirName, subDirectories.Name);
                    CopyDirectoryRecursively(subDirectories.FullName, aimDirectoryPath);
                }
            }
            catch (Exception ex)
            {
                _progressReporter.Fail(ex);
                CleanUp();
                _progressReporter.Terminate();
            }
        }
    }
}