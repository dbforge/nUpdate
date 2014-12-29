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
using nUpdate.UpdateInstaller.Client.GuiInterface;
using nUpdate.UpdateInstaller.Core;
using nUpdate.UpdateInstaller.Core.Operations;
using nUpdate.UpdateInstaller.UI.Dialogs;
using nUpdate.UpdateInstaller.UI.Popups;

namespace nUpdate.UpdateInstaller
{
    public class Updater
    {
        private int _currentFileAmount;
        private int _totalFilesCount;
        private int _percentageMultiplier = 50;
        private int _percentComplete;
        private IProgressReporter _progressReporter;
        private readonly ManualResetEvent _unpackingResetEvent = new ManualResetEvent(false);

        /// <summary>
        ///     Runs the update installation process.
        /// </summary>
        public void RunUpdate()
        {
            try
            {
                _progressReporter = GetGui();
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(SystemIcons.Error, "Updating the application has failed.", String.Format("Error while loading the user interface: {0}", ex.Message), PopupButtons.Ok);
                return;
            }

            if (_progressReporter == null)
                return;

            try
            {
                _progressReporter.Initialize();
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(SystemIcons.Error, "Updating the application has failed.", String.Format("Error while loading the user interface: {0}", ex.Message), PopupButtons.Ok);
                return;
            }

            ThreadPool.QueueUserWorkItem(arg =>
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
                _totalFilesCount = directories.Sum(directory => Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories).Length);

                if (!Program.Operations.Any())
                    _percentageMultiplier = 100;

                foreach (var directory in directories)
                {
                    CopyDirectoryRecursively(directory.FullName, Program.AimFolder);
                }

                int operationsStep = (Program.Operations.Count()/50);
                try
                {
                    foreach (var operation in Program.Operations)
                    {
                        switch (operation.Area)
                        {
                            case OperationArea.Files:
                                switch (operation.Method)
                                {
                                    case OperationMethods.Delete:
                                        _percentComplete += operationsStep;
                                        _progressReporter.ReportOperationProgress(_percentComplete,
                                            String.Format(Program.FileDeletingOperationText, operation.Value));

                                        File.Delete(operation.Value.ToString());
                                        break;

                                    case OperationMethods.Rename:
                                        _percentComplete += operationsStep;
                                        _progressReporter.ReportOperationProgress(_percentComplete,
                                            String.Format(Program.FileRenamingOperationText, operation.Value,
                                                operation.Value2));

                                        File.Move(operation.Value.ToString(),
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
                                            _percentComplete += operationsStep;
                                            _progressReporter.ReportOperationProgress(_percentComplete,
                                                String.Format(Program.RegistryKeyCreateOperationText, operation.Value2));

                                            // Create key
                                        }
                                        break;

                                    case OperationMethods.Delete:
                                        foreach (var registryKey in (BindingList<string>) operation.Value2)
                                        {
                                            _percentComplete += operationsStep;
                                            _progressReporter.ReportOperationProgress(_percentComplete,
                                                String.Format(Program.RegistryKeyDeleteOperationText, operation.Value2));

                                            // Delete key
                                        }
                                        break;

                                    case OperationMethods.SetValue:
                                        _percentComplete += operationsStep;
                                        _progressReporter.ReportOperationProgress(_percentComplete,
                                                String.Format(Program.RegistryKeyValueSetOperationText, operation.Value.ToString().Split(new []{'\\'}).Last(), ((Tuple<string, string>)operation.Value2).Item2));

                                            // Set key value
                                        break;
                                }
                                break;

                            case OperationArea.Processes:
                                switch (operation.Method)
                                {
                                    case OperationMethods.Start:
                                        _percentComplete += operationsStep;
                                        _progressReporter.ReportOperationProgress(_percentComplete,
                                            String.Format(Program.ProcessStartOperationText, operation.Value));

                                        var process = new Process
                                        {
                                            StartInfo =
                                            {
                                                FileName = operation.Value.ToString(),
                                                Arguments = operation.Value2.ToString()
                                            }
                                        };
                                        process.Start();
                                        break;

                                    case OperationMethods.Stop:
                                        _percentComplete += operationsStep;
                                        _progressReporter.ReportOperationProgress(_percentComplete,
                                            String.Format(Program.ProcessStopOperationText, operation.Value));

                                        var processes = Process.GetProcessesByName(operation.Value.ToString());
                                        foreach (var foundProcess in processes)
                                            foundProcess.Kill();
                                        break;
                                }
                                break;

                            case OperationArea.Services:
                                switch (operation.Method)
                                {
                                    case OperationMethods.Start:
                                        _percentComplete += operationsStep;
                                        _progressReporter.ReportOperationProgress(_percentComplete,
                                            String.Format(Program.ServiceStartOperationText, operation.Value));

                                        ServiceManager.StartService(operation.Value.ToString());
                                        break;

                                    case OperationMethods.Stop:
                                        _percentComplete += operationsStep;
                                        _progressReporter.ReportOperationProgress(_percentComplete,
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

                _unpackingResetEvent.Set();
            });

            _unpackingResetEvent.WaitOne();

            var p = new Process
            {
                StartInfo = {UseShellExecute = true, FileName = Program.ApplicationExecutablePath}
            };
            p.Start();
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

        private void CopyDirectoryRecursively(string sourceDirName, string destDirName)
        {
            var dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] sourceDirectories = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                _currentFileAmount += 1;
                _percentComplete = (_currentFileAmount / _totalFilesCount) * _percentageMultiplier;
                _progressReporter.ReportUnpackingProgress(_percentComplete, file.Name);

                string aimPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(aimPath, true);
            }

            foreach (DirectoryInfo subDirectories in sourceDirectories)
            {
                string aimDirectoryPath = Path.Combine(destDirName, subDirectories.Name);
                CopyDirectoryRecursively(subDirectories.FullName, aimDirectoryPath);
            }
        }
    }
}