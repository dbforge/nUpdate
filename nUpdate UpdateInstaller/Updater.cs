using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace nUpdate.UpdateInstaller
{
    public class Updater
    {
        private int _doneTaskAmount;
        private int _totalTaskCount;
        private IProgressReporter _progressReporter;

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
                _progressReporter.InitializingFail(ex);
                _progressReporter.Terminate();
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
        ///     Loads the GUI either from a given external assembly, if one is set, or otherwise from the integrated GUI.
        /// </summary>
        /// <returns>Returns a new instance of the given object that implements the <see cref="nUpdate.UpdateInstaller.Client.GuiInterface.IProgressReporter"/>-interface.</returns>
        private IProgressReporter GetGui()
        {
            if (String.IsNullOrEmpty(Program.ExternalGuiAssemblyPath) || !File.Exists(Program.ExternalGuiAssemblyPath)) 
                return (IProgressReporter) Activator.CreateInstance(typeof (MainForm));

            Assembly assembly = Assembly.LoadFrom(Program.ExternalGuiAssemblyPath);
            List<Type> validTypes =
                assembly.GetTypes().Where(item => typeof (IProgressReporter).IsAssignableFrom(item)).ToList(); // If any class implements the interface, load it...
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
                    try
                    {
                        foreach (ZipEntry entry in zf)
                        {
                            entry.Extract(Directory.GetParent(Program.PackageFile).FullName);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_progressReporter.Fail(ex))
                            _progressReporter.Terminate();
                    }
                }

                var directories = Directory.GetParent(Program.PackageFile).GetDirectories();
                var simpleOperations = Program.Operations.Where(item =>
                    item.Area != OperationArea.Registry &&
                    (item.Area != OperationArea.Files && item.Method != OperationMethods.Delete)).ToArray();
                _totalTaskCount = directories.Sum(
                    directory => Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories).Length) +
                                  simpleOperations.Count()
                                  + Program.Operations.Count(item => simpleOperations.Any(x => x == item));

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
                if (_progressReporter.Fail(ex))
                    _progressReporter.Terminate();
                return;
            }

            try
            {
                foreach (var operation in Program.Operations)
                {
                    float percentage;
                    switch (operation.Area)
                    {
                        case OperationArea.Files:
                            switch (operation.Method)
                            {
                                case OperationMethods.Delete:
                                    string[] deleteFilePathParts = operation.Value.ToString().Split('\\');
                                    string deleteFileFullPath = Path.Combine(Operation.GetDirectory(deleteFilePathParts[0]),
                                        String.Join("\\", deleteFilePathParts.Where(item => item != deleteFilePathParts[0])));
                                    foreach (var fileToDelete in (BindingList<string>) operation.Value2)
                                    {
                                        _doneTaskAmount += 1;
                                        percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                                        _progressReporter.ReportOperationProgress(percentage,
                                            String.Format(Program.FileDeletingOperationText, fileToDelete));

                                        File.Delete(Path.Combine(deleteFileFullPath, fileToDelete));
                                    }
                                    break;

                                case OperationMethods.Rename:
                                    _doneTaskAmount += 1;
                                    percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                                    _progressReporter.ReportOperationProgress(percentage,
                                        String.Format(Program.FileRenamingOperationText, operation.Value,
                                            operation.Value2));

                                    string[] renameFilePathParts = operation.Value.ToString().Split('\\');
                                    string renameFileFullPath = Path.Combine(Operation.GetDirectory(renameFilePathParts[0]),
                                        String.Join("\\", renameFilePathParts.Where(item => item != renameFilePathParts[0])));
                                    File.Move(renameFileFullPath,
                                        Path.Combine(Directory.GetParent(operation.Value.ToString()).FullName,
                                            operation.Value2.ToString()));
                                    break;
                            }
                            break;
                        case OperationArea.Registry:
                            switch (operation.Method)
                            {
                                case OperationMethods.Create:
                                    foreach (var registryKey in (BindingList<string>) operation.Value2)
                                    {
                                        _doneTaskAmount += 1;
                                        percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                                        _progressReporter.ReportOperationProgress(percentage,
                                            String.Format(Program.RegistryKeyCreateOperationText, registryKey));
                                        RegistryManager.CreateSubKey(operation.Value.ToString(), registryKey);
                                    }
                                    break;

                                case OperationMethods.Delete:
                                    foreach (var registryKey in (BindingList<string>) operation.Value2)
                                    {
                                        _doneTaskAmount += 1;
                                        percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                                        _progressReporter.ReportOperationProgress(percentage,
                                            String.Format(Program.RegistryKeyDeleteOperationText, registryKey));
                                        RegistryManager.DeleteSubKey(operation.Value.ToString(), registryKey);
                                    }
                                    break;

                                case OperationMethods.SetValue:
                                    foreach (
                                        var nameValuePair in
                                            (BindingList<Tuple<string, object, RegistryValueKind>>) operation.Value2)
                                    {
                                        _doneTaskAmount += 1;
                                        percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                                        _progressReporter.ReportOperationProgress(percentage,
                                            String.Format(Program.RegistryNameValuePairSetValueOperationText, nameValuePair.Item1));
                                        RegistryManager.SetValue(operation.Value.ToString(), nameValuePair.Item1,
                                            nameValuePair.Item2, nameValuePair.Item3);
                                    }
                                    break;

                                case OperationMethods.DeleteValue:
                                    foreach (
                                        var valueName in
                                            (BindingList<string>)operation.Value2)
                                    {
                                        _doneTaskAmount += 1;
                                        percentage = ((float)_doneTaskAmount / _totalTaskCount) * 100f;
                                        _progressReporter.ReportOperationProgress(percentage,
                                            String.Format(Program.RegistryNameValuePairSetValueOperationText, valueName));
                                        RegistryManager.DeleteValue(operation.Value.ToString(), valueName);
                                    }
                                    break;
                            }
                            break;

                        case OperationArea.Processes:
                            _doneTaskAmount += 1;
                            percentage = ((float)_doneTaskAmount / _totalTaskCount) * 100f;
                            switch (operation.Method)
                            {
                                case OperationMethods.Start:
                                    _progressReporter.ReportOperationProgress(percentage,
                                        String.Format(Program.ProcessStartOperationText, operation.Value));

                                    string[] processFilePathParts = operation.Value.ToString().Split('\\');
                                    string processFileFullPath = Path.Combine(Operation.GetDirectory(processFilePathParts[0]),
                                        String.Join("\\", processFilePathParts.Where(item => item != processFilePathParts[0])));

                                    var process = new Process
                                    {
                                        StartInfo =
                                        {
                                            FileName = processFileFullPath,
                                            Arguments = operation.Value2.ToString()
                                        }
                                    };
                                    process.Start();
                                    break;

                                case OperationMethods.Stop:
                                    _progressReporter.ReportOperationProgress(percentage,
                                        String.Format(Program.ProcessStopOperationText, operation.Value));

                                    var processes = Process.GetProcessesByName(operation.Value.ToString());
                                    foreach (var foundProcess in processes)
                                        foundProcess.Kill();
                                    break;
                            }
                            break;

                        case OperationArea.Services:
                            _doneTaskAmount += 1;
                            percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                            switch (operation.Method)
                            {
                                case OperationMethods.Start:
                                    _progressReporter.ReportOperationProgress(percentage,
                                        String.Format(Program.ServiceStartOperationText, operation.Value));

                                    ServiceManager.StartService(operation.Value.ToString(), (string[])operation.Value2);
                                    break;

                                case OperationMethods.Stop:
                                    _progressReporter.ReportOperationProgress(percentage,
                                        String.Format(Program.ServiceStopOperationText, operation.Value));

                                    ServiceManager.StopService(operation.Value.ToString());
                                    break;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (_progressReporter.Fail(ex)) // If it should be terminated
                {
                    _progressReporter.Terminate();
                }
            }

            try
            {
                Directory.Delete(Directory.GetParent(Program.PackageFile).FullName);
            }
            catch (Exception ex)
            {
                _progressReporter.Fail(ex);
            }

            var p = new Process
            {
                StartInfo = { UseShellExecute = true, FileName = Program.ApplicationExecutablePath }
            };
            p.Start();

            _progressReporter.Terminate();
        }

        private void CopyDirectoryRecursively(string sourceDirName, string destDirName)
        {
            try
            {
                var dir = new DirectoryInfo(sourceDirName);
                DirectoryInfo[] sourceDirectories = dir.GetDirectories();

                if (!Directory.Exists(destDirName))
                    Directory.CreateDirectory(destDirName);

                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    _doneTaskAmount += 1;
                    float percentage = ((float) _doneTaskAmount/_totalTaskCount)*100f;
                    _progressReporter.ReportUnpackingProgress(percentage, file.Name);

                    string aimPath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(aimPath, true);
                }

                foreach (DirectoryInfo subDirectories in sourceDirectories)
                {
                    string aimDirectoryPath = Path.Combine(destDirName, subDirectories.Name);
                    CopyDirectoryRecursively(subDirectories.FullName, aimDirectoryPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}