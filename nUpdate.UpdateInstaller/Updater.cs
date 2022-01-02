// Updater.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Ionic.Zip;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using nUpdate.UpdateInstaller.Operations;
using nUpdate.UpdateInstaller.UIBase;
using UpdateInstaller;

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
            _progressReporter = GetProgressReporter();

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
        ///     <see cref="IProgressReporter" />-interface.
        /// </returns>
        private IProgressReporter GetProgressReporter()
        {
            var assembly = string.IsNullOrEmpty(Program.ExternalGuiAssemblyPath) ||
                           !File.Exists(Program.ExternalGuiAssemblyPath)
                ? Assembly.GetExecutingAssembly()
                : Assembly.LoadFrom(Program.ExternalGuiAssemblyPath);
            var provider = ServiceProviderHelper.CreateServiceProvider(assembly);
            return (IProgressReporter) provider.GetService(typeof(IProgressReporter));
        }

        /// <summary>
        ///     Runs the updating process. This method does not block the calling thread.
        /// </summary>
        private void RunUpdateAsync()
        {
            Thread.Sleep(500);
            Process hostProcess = null;
            try
            { 
                hostProcess = Process.GetProcessById(Program.HostProcessId);
                hostProcess.WaitForExit();
            }
            catch (Win32Exception)
            {
                try
                {
                    if (hostProcess != null)
                    {
                        while (!hostProcess.HasExited)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                catch (SystemException)
                { }
            }
            catch (SystemException) // Process already killed
            { }

            var parentPath = Directory.GetParent(Program.PackageFilePaths.First())?.FullName;
            if (parentPath == null)
            {
                _progressReporter.Fail(new Exception("The packages directory could not be determined."));
                CleanUp();
                _progressReporter.Terminate();
                return;
            }

            /* Extract and count for the progress */
            foreach (var packageFilePath in Program.PackageFilePaths)
            {
                var versionString = Path.GetFileNameWithoutExtension(packageFilePath);
                var extractedDirectoryPath =
                    Path.Combine(parentPath, versionString ?? throw new InvalidOperationException());
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
                        CleanUp();
                        _progressReporter.Terminate();
                        if (Program.HostApplicationOptions != HostApplicationOptions.CloseAndRestart)
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

                    string operationsFile = Path.Combine(extractedDirectoryPath, "operations.json");
                    var currentVersionOperations = !File.Exists(operationsFile)
                        ? Program.Operations[versionString].ToList()
                        : Serializer.Deserialize<IEnumerable<Operation>>(
                            File.ReadAllText(operationsFile)).ToList();

                    // Multiple entries for:
                    /* 
                     * Files: Delete operation
                     * Registry: All operations
                     */

                    _totalTaskCount +=
                        currentVersionOperations.Count(
                            item => item.Area != OperationArea.Registry &&
                                    (item.Area != OperationArea.Files || item.Method != OperationMethod.Delete));

                    foreach (var op in currentVersionOperations.Where(o => o.Area == OperationArea.Registry
                                                                           || o.Area == OperationArea.Files &&
                                                                           o.Method == OperationMethod.Delete))
                        _totalTaskCount += ((JArray) op.Value2).Count;
                }
            }

            foreach (
                var packageFilePath in
                Program.PackageFilePaths.OrderBy(item => new UpdateVersion(Path.GetFileNameWithoutExtension(item))))
            {
                var versionString = Path.GetFileNameWithoutExtension(packageFilePath);
                var extractedDirectoryPath =
                    Path.Combine(parentPath, versionString);

                List<Operation> currentVersionOperations;
                try
                {
                    string operationsFile = Path.Combine(extractedDirectoryPath, "operations.json");
                    currentVersionOperations = !File.Exists(operationsFile)
                        ? Program.Operations[versionString].ToList()
                        : Serializer.Deserialize<IEnumerable<Operation>>(
                            File.ReadAllText(operationsFile)).ToList();
                    ExecuteOperations(currentVersionOperations.Where(o => o.ExecuteBeforeReplacingFiles));
                }
                catch (Exception ex)
                {
                    _progressReporter.Fail(ex);
                    CleanUp();
                    _progressReporter.Terminate();
                    if (Program.HostApplicationOptions != HostApplicationOptions.CloseAndRestart)
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
                            if (WindowsServiceHelper.IsRunningInServiceContext) continue;
                            CopyDirectoryRecursively(directory.FullName,
                                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                            break;
                    }

                try
                {
                    ExecuteOperations(currentVersionOperations.Where(o => !o.ExecuteBeforeReplacingFiles));
                }
                catch (Exception ex)
                {
                    _progressReporter.Fail(ex);
                    CleanUp();
                    _progressReporter.Terminate();
                    if (Program.HostApplicationOptions != HostApplicationOptions.CloseAndRestart)
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

            CleanUp();
            if (Program.HostApplicationOptions == HostApplicationOptions.CloseAndRestart)
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

        private void ExecuteOperations(IEnumerable<Operation> operations)
        {
            foreach (var operation in operations)
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
                                    PathDetector.GetDirectory(deleteFilePathParts[0]),
                                    string.Join("\\",
                                        deleteFilePathParts.Where(item => item != deleteFilePathParts[0])));
                                secondValueAsArray = operation.Value2 as JArray;
                                if (secondValueAsArray != null)
                                    foreach (
                                        var fileToDelete in secondValueAsArray.ToObject<IEnumerable<string>>())
                                    {
                                        var path = Path.Combine(deleteFileFullPath, fileToDelete);
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
                                    PathDetector.GetDirectory(renameFilePathParts[0]),
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
                                            string.Format(Program.RegistrySubKeyCreateOperationText, registryKey));
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
                                            string.Format(Program.RegistrySubKeyDeleteOperationText, registryKey));
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
                                            string.Format(Program.RegistryNameValuePairSetValueOperationText,
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
                                    Path.Combine(PathDetector.GetDirectory(processFilePathParts[0]),
                                        string.Join("\\",
                                            processFilePathParts.Where(item => item != processFilePathParts[0])));

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
                                ServiceManager.StartService(operation.Value, (string[]) operation.Value2);

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

                                _doneTaskAmount += 1;
                                percentage = (float) _doneTaskAmount / _totalTaskCount * 100f;
                                _progressReporter.ReportOperationProgress(percentage, "Executing operation.");
                                break;
                        }

                        break;
                }
            }
        }

        /// <summary>
        ///     Cleans up all resources.
        /// </summary>
        private void CleanUp()
        {
            try
            {
                Directory.Delete(Directory.GetParent(Program.PackageFilePaths.First()).FullName, true);
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
                    var continueCopyLoop = true;
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
                CleanUp();
                _progressReporter.Terminate();
                if (Program.HostApplicationOptions != HostApplicationOptions.CloseAndRestart)
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
    }
}