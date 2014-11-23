using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using nUpdate.UpdateInstaller.Client.GuiInterface;
using nUpdate.UpdateInstaller.Core;
using nUpdate.UpdateInstaller.Core.Operations;
using nUpdate.UpdateInstaller.Dialogs;

namespace nUpdate.UpdateInstaller
{
    public class Updater
    {
        /// <summary>
        ///     Runs the update installation process.
        /// </summary>
        public void RunUpdate()
        {
            IProgressReporter progressReporter = GetGui();
            progressReporter.Initialize();

            int percentComplete = 0;
            ThreadPool.QueueUserWorkItem(arg =>
            {
                using (var zf = Ionic.Zip.ZipFile.Read(Program.PackageFile))
                {
                    zf.ToList().ForEach(entry =>
                    {
                        int step = Program.Operations.Any() ? zf.Count/50 : zf.Count/100;

                        try
                        {
                            entry.FileName = Path.GetFileName(entry.FileName);
                            entry.Extract(Program.AimFolder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                        }
                        catch (Exception ex)
                        {
                            if (!progressReporter.Fail(ex))
                            {
                                progressReporter.Terminate();
                                Application.Exit();
                            }
                        }

                        percentComplete += step;
                        progressReporter.ReportOperationProgress(percentComplete, entry.FileName);
                    });
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
                                        percentComplete += operationsStep;
                                        progressReporter.ReportOperationProgress(percentComplete,
                                            String.Format(Program.FileDeletingOperationText, operation.Value));

                                        File.Delete(operation.Value.ToString());
                                        break;

                                    case OperationMethods.Rename:
                                        percentComplete += operationsStep;
                                        progressReporter.ReportOperationProgress(percentComplete,
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
                                            percentComplete += operationsStep;
                                            progressReporter.ReportOperationProgress(percentComplete,
                                                String.Format(Program.RegistryKeyCreateOperationText, operation.Value2));

                                            // Create key
                                        }
                                        break;

                                    case OperationMethods.Delete:
                                        foreach (var registryKey in (BindingList<string>) operation.Value2)
                                        {
                                            percentComplete += operationsStep;
                                            progressReporter.ReportOperationProgress(percentComplete,
                                                String.Format(Program.RegistryKeyDeleteOperationText, operation.Value2));

                                            // Delete key
                                        }
                                        break;

                                    case OperationMethods.SetValue:
                                        percentComplete += operationsStep;
                                        progressReporter.ReportOperationProgress(percentComplete,
                                                String.Format(Program.RegistryKeyValueSetOperationText, operation.Value.ToString().Split(new []{'\\'}).Last(), ((Tuple<string, string>)operation.Value2).Item2));

                                            // Set key value
                                        break;
                                }
                                break;

                            case OperationArea.Processes:
                                switch (operation.Method)
                                {
                                    case OperationMethods.Start:
                                        percentComplete += operationsStep;
                                        progressReporter.ReportOperationProgress(percentComplete,
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
                                        percentComplete += operationsStep;
                                        progressReporter.ReportOperationProgress(percentComplete,
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
                                        percentComplete += operationsStep;
                                        progressReporter.ReportOperationProgress(percentComplete,
                                            String.Format(Program.ServiceStartOperationText, operation.Value));

                                        ServiceManager.StartService(operation.Value.ToString());
                                        break;

                                    case OperationMethods.Stop:
                                        percentComplete += operationsStep;
                                        progressReporter.ReportOperationProgress(percentComplete,
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
                    if (!progressReporter.Fail(ex))
                    {
                        progressReporter.Terminate();
                        Application.Exit();
                    }
                }
            });

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
    }
}