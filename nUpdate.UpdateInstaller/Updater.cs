// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Ionic.Zip;
using Microsoft.Win32;
using nUpdate.UpdateInstaller.Client.GuiInterface;
using nUpdate.UpdateInstaller.Core;
using nUpdate.UpdateInstaller.Core.Operations;
using nUpdate.UpdateInstaller.UI.Popups;
using Newtonsoft.Json.Linq;

namespace nUpdate.UpdateInstaller
{
    public class Updater
    {
        private int _doneTaskAmount;
        private IProgressReporter _progressReporter;
        private int _totalTaskCount;

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
                    bool continueCopyLoop = true;
                    var aimPath = Path.Combine(destDirName, file.Name);
                    while (continueCopyLoop)
                        try
                        {
                            file.CopyTo(aimPath, true);
                            continueCopyLoop = false;
                        }
                        catch (IOException ex)
                        {
                            if (FileHelper.IsFileLocked(ex))
                                _progressReporter.Fail(new Exception(string.Format(Program.FileInUseError, aimPath)));
                            else
                                throw;
                        }

                    _doneTaskAmount += 1;
                    var percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
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
                _progressReporter.Terminate();
                if (!Program.IsHostApplicationClosed)
                    return;

                var process = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = true,
                        FileName = Program.ApplicationExecutablePath,
                        Arguments =
                            string.Join("|",
                                Program.Arguments.Where(
                                    item =>
                                        item.ExecutionOptions ==
                                        UpdateArgumentExecutionOptions.OnlyOnFaulted).Select(item => item.Argument))
                    }
                };
                process.Start();
            }
        }

        /// <summary>
        ///     Loads the GUI either from a given external assembly, if one is set, or otherwise, uses the integrated GUI.
        /// </summary>
        /// <returns>
        ///     Returns a new instance of the given object that implements the
        ///     <see cref="IProgressReporter" />-interface.
        /// </returns>
        private IProgressReporter GetProgressReporter()
        {
            Assembly assembly = string.IsNullOrEmpty(Program.ExternalGuiAssemblyPath) ||
                                !File.Exists(Program.ExternalGuiAssemblyPath)
                ? Assembly.GetExecutingAssembly()
                : Assembly.LoadFrom(Program.ExternalGuiAssemblyPath);
            IServiceProvider provider = ServiceProviderHelper.CreateServiceProvider(assembly);
            return (IProgressReporter) provider.GetService(typeof(IProgressReporter));
        }

        /// <summary>
        ///     Runs the updating process.
        /// </summary>
        public void RunUpdate()
        {
            try
            {
                _progressReporter = GetProgressReporter();
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(SystemIcons.Error, "Error while initializing the graphical user interface.", ex,
                    PopupButtons.Ok);
                return;
            }

            try
            {
                _progressReporter.Initialize();
            }
            catch (Exception ex)
            {
                _progressReporter.InitializingFail(ex);
                _progressReporter.Terminate();
            }

            ThreadPool.QueueUserWorkItem(arg => RunUpdateAsync());
        }

        /// <summary>
        ///     Runs the updating process. This method does not block the calling thread.
        /// </summary>
        private void RunUpdateAsync()
        {
            string parentPath = Directory.GetParent(Program.PackageFilePaths.First()).FullName;
            /* Extract and count for the progress */
            foreach (var packageFilePath in Program.PackageFilePaths)
            {
                var version = new UpdateVersion(Path.GetFileNameWithoutExtension(packageFilePath));
                string extractedDirectoryPath =
                    Path.Combine(parentPath, version.ToString());
                Directory.CreateDirectory(extractedDirectoryPath);
                using (var zf = ZipFile.Read(packageFilePath))
                {
                    zf.ParallelDeflateThreshold = -1;
                    try
                    {
                        foreach (var entry in zf) entry.Extract(extractedDirectoryPath);
                    }
                    catch (Exception ex)
                    {
                        _progressReporter.Fail(ex);
                        _progressReporter.Terminate();
                        if (!Program.IsHostApplicationClosed || !Program.RestartHostApplication)
                            return;

                        var process = new Process
                        {
                            StartInfo =
                            {
                                UseShellExecute = true,
                                FileName = Program.ApplicationExecutablePath,
                                Arguments =
                                    string.Join("|",
                                        Program.Arguments.Where(
                                                item =>
                                                    item.ExecutionOptions ==
                                                    UpdateArgumentExecutionOptions.OnlyOnFaulted)
                                            .Select(item => item.Argument))
                            }
                        };
                        process.Start();
                        return;
                    }

                    _totalTaskCount += new DirectoryInfo(extractedDirectoryPath).GetDirectories().Sum(
                        directory => Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories).Length);
                }
            }

            foreach (var operationEnumerable in Program.Operations.Select(item => item.Value))
                _totalTaskCount +=
                    operationEnumerable.Count(
                        item => item.Area != OperationArea.Registry && item.Method != OperationMethod.Delete);

            foreach (
                var array in
                Program.Operations.Select(entry => entry.Value)
                    .Select(operationEnumerable => operationEnumerable.Where(
                            item =>
                                item.Area == OperationArea.Registry && item.Method != OperationMethod.SetValue)
                        .Select(registryOperation => registryOperation.Value2)
                        .OfType<JArray>()).SelectMany(entries =>
                    {
                        var entryEnumerable = entries as JArray[] ?? entries.ToArray();
                        return entryEnumerable;
                    }))
                _totalTaskCount += array.ToObject<IEnumerable<string>>().Count();

            foreach (
                var array in
                Program.Operations.Select(entry => entry.Value)
                    .Select(operationEnumerable => operationEnumerable.Where(
                            item => item.Area == OperationArea.Files && item.Method == OperationMethod.Delete)
                        .Select(registryOperation => registryOperation.Value2)
                        .OfType<JArray>()).SelectMany(entries =>
                    {
                        var entryEnumerable = entries as JArray[] ?? entries.ToArray();
                        return entryEnumerable;
                    }))
                _totalTaskCount += array.ToObject<IEnumerable<string>>().Count();

            foreach (
                var array in
                Program.Operations.Select(entry => entry.Value)
                    .Select(operationEnumerable => operationEnumerable.Where(
                            item =>
                                item.Area == OperationArea.Registry && item.Method == OperationMethod.SetValue)
                        .Select(registryOperation => registryOperation.Value2)
                        .OfType<JArray>()).SelectMany(entries =>
                    {
                        var entryEnumerable = entries as JArray[] ?? entries.ToArray();
                        return entryEnumerable;
                    }))
                _totalTaskCount += array.ToObject<IEnumerable<Tuple<string, object, RegistryValueKind>>>().Count();

            foreach (
                var packageFilePath in
                Program.PackageFilePaths)
            {
                var version = new UpdateVersion(Path.GetFileNameWithoutExtension(packageFilePath));
                string extractedDirectoryPath =
                    Path.Combine(parentPath, version.ToString());
                foreach (var directory in new DirectoryInfo(extractedDirectoryPath).GetDirectories())
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

                try
                {
                    IEnumerable<Operation> currentVersionOperations =
                        Program.Operations.Any(item => new UpdateVersion(item.Key) == version)
                            ? Program.Operations.First(item => new UpdateVersion(item.Key) == version).Value
                            : Enumerable.Empty<Operation>();
                    foreach (var operation in currentVersionOperations)
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
                                            string.Join("\\",
                                                deleteFilePathParts.Where(item => item != deleteFilePathParts[0])));
                                        secondValueAsArray = operation.Value2 as JArray;
                                        if (secondValueAsArray != null)
                                            foreach (
                                                var fileToDelete in secondValueAsArray.ToObject<IEnumerable<string>>())
                                            {
                                                string path = Path.Combine(deleteFileFullPath, fileToDelete);
                                                if (File.Exists(path))
                                                    File.Delete(path);

                                                _doneTaskAmount += 1;
                                                percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
                                                _progressReporter.ReportOperationProgress(percentage,
                                                    string.Format(Program.FileDeletingOperationText, fileToDelete));
                                            }

                                        break;

                                    case OperationMethod.Rename:
                                        var renameFilePathParts = operation.Value.Split('\\');
                                        var renameFileFullPath = Path.Combine(
                                            Operation.GetDirectory(renameFilePathParts[0]),
                                            string.Join("\\",
                                                renameFilePathParts.Where(item => item != renameFilePathParts[0])));
                                        if (File.Exists(renameFileFullPath))
                                            File.Move(renameFileFullPath,
                                                Path.Combine(Directory.GetParent(renameFileFullPath).FullName,
                                                    operation.Value2.ToString()));

                                        _doneTaskAmount += 1;
                                        percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
                                        _progressReporter.ReportOperationProgress(percentage,
                                            string.Format(Program.FileRenamingOperationText,
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
                                            foreach (
                                                var registryKey in secondValueAsArray.ToObject<IEnumerable<string>>())
                                            {
                                                RegistryManager.CreateSubKey(operation.Value, registryKey);

                                                _doneTaskAmount += 1;
                                                percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
                                                _progressReporter.ReportOperationProgress(percentage,
                                                    string.Format(Program.RegistrySubKeyCreateOperationText,
                                                        registryKey));
                                            }

                                        break;

                                    case OperationMethod.Delete:
                                        secondValueAsArray = operation.Value2 as JArray;
                                        if (secondValueAsArray != null)
                                            foreach (
                                                var registryKey in secondValueAsArray.ToObject<IEnumerable<string>>())
                                            {
                                                RegistryManager.DeleteSubKey(operation.Value, registryKey);

                                                _doneTaskAmount += 1;
                                                percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
                                                _progressReporter.ReportOperationProgress(percentage,
                                                    string.Format(Program.RegistrySubKeyDeleteOperationText,
                                                        registryKey));
                                            }

                                        break;

                                    case OperationMethod.SetValue:
                                        secondValueAsArray = operation.Value2 as JArray;
                                        if (secondValueAsArray != null)
                                            foreach (
                                                var nameValuePair in
                                                secondValueAsArray
                                                    .ToObject<IEnumerable<Tuple<string, object, RegistryValueKind>>>
                                                        ())
                                            {
                                                RegistryManager.SetValue(operation.Value, nameValuePair.Item1,
                                                    nameValuePair.Item2, nameValuePair.Item3);

                                                _doneTaskAmount += 1;
                                                percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
                                                _progressReporter.ReportOperationProgress(percentage,
                                                    string.Format(Program.RegistryNameValuePairSetValueOperationText,
                                                        nameValuePair.Item1, nameValuePair.Item2));
                                            }

                                        break;

                                    case OperationMethod.DeleteValue:
                                        secondValueAsArray = operation.Value2 as JArray;
                                        if (secondValueAsArray != null)
                                            foreach (var valueName in secondValueAsArray.ToObject<IEnumerable<string>>()
                                            )
                                            {
                                                RegistryManager.DeleteValue(operation.Value, valueName);

                                                _doneTaskAmount += 1;
                                                percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
                                                _progressReporter.ReportOperationProgress(percentage,
                                                    string.Format(Program.RegistryNameValuePairDeleteValueOperationText,
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
                                                string.Join("\\",
                                                    processFilePathParts.Where(item =>
                                                        item != processFilePathParts[0])));

                                        var process = new Process
                                        {
                                            StartInfo =
                                            {
                                                FileName = processFileFullPath,
                                                Arguments = operation.Value2.ToString()
                                            }
                                        };
                                        try
                                        {
                                            process.Start();
                                        }
                                        catch (Win32Exception ex)
                                        {
                                            if (ex.NativeErrorCode != 1223)
                                                throw;
                                        }

                                        _doneTaskAmount += 1;
                                        percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
                                        _progressReporter.ReportOperationProgress(percentage,
                                            string.Format(Program.ProcessStartOperationText, operation.Value));
                                        break;

                                    case OperationMethod.Stop:
                                        var processes = Process.GetProcessesByName(operation.Value);
                                        foreach (var foundProcess in processes)
                                            foundProcess.Kill();

                                        _doneTaskAmount += 1;
                                        percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
                                        _progressReporter.ReportOperationProgress(percentage,
                                            string.Format(Program.ProcessStopOperationText, operation.Value));
                                        break;
                                }

                                break;

                            case OperationArea.Services:
                                switch (operation.Method)
                                {
                                    case OperationMethod.Start:
                                        ServiceManager.StartService(operation.Value,
                                            ((JArray) operation.Value2).ToObject<string[]>());

                                        _doneTaskAmount += 1;
                                        percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
                                        _progressReporter.ReportOperationProgress(percentage,
                                            string.Format(Program.ServiceStartOperationText, operation.Value));
                                        break;

                                    case OperationMethod.Stop:
                                        ServiceManager.StopService(operation.Value);

                                        _doneTaskAmount += 1;
                                        percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
                                        _progressReporter.ReportOperationProgress(percentage,
                                            string.Format(Program.ServiceStopOperationText, operation.Value));
                                        break;
                                }

                                break;
                            case OperationArea.Scripts:
                                switch (operation.Method)
                                {
                                    case OperationMethod.Execute:
                                        var helper = new CodeDomHelper();
                                        helper.ExecuteScript(operation.Value);
                                        break;
                                }

                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _progressReporter.Fail(ex);
                    _progressReporter.Terminate();
                    if (!Program.IsHostApplicationClosed || !Program.RestartHostApplication)
                        return;

                    var process = new Process
                    {
                        StartInfo =
                        {
                            UseShellExecute = true,
                            FileName = Program.ApplicationExecutablePath,
                            Arguments =
                                string.Join("|",
                                    Program.Arguments.Where(
                                        item =>
                                            item.ExecutionOptions ==
                                            UpdateArgumentExecutionOptions.OnlyOnFaulted).Select(item => item.Argument))
                        }
                    };
                    process.Start();
                    return;
                }
            }

            if (Program.IsHostApplicationClosed && Program.RestartHostApplication)
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = true,
                        FileName = Program.ApplicationExecutablePath,
                        Arguments =
                            string.Join("|",
                                Program.Arguments.Where(
                                        item =>
                                            item.ExecutionOptions == UpdateArgumentExecutionOptions.OnlyOnSucceeded)
                                    .Select(item => item.Argument))
                    }
                };
                p.Start();
            }

            _progressReporter.Terminate();
        }
    }
}