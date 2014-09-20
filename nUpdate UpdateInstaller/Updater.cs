using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using nUpdate.Client.GuiInterface;

namespace nUpdate.UpdateInstaller
{
    public class Updater
    {
        private const string DefaultUpdaterGui = "UpdaterGui.dll";
        private readonly string GuiFilename;

        public Updater()
        {
            try
            {
                GuiFilename = ConfigurationManager.AppSettings["UpdaterGuiAssembly"];
                if (String.IsNullOrWhiteSpace(GuiFilename))
                    GuiFilename = DefaultUpdaterGui;
            }
            catch (Exception)
            {
                GuiFilename = DefaultUpdaterGui;
            }
        }

        /// <summary>
        ///     Runs the update installation process.
        /// </summary>
        public void RunUpdate()
        {
            IProgressReporter progressReporter = GetGUI();
            progressReporter.Initialize();
            var worker = new BackgroundWorker();
            //worker.WorkerReportsProgress = true;
            //worker.DoWork += (o, e) =>
            //{
            //    using (var zf = Ionic.Zip.ZipFile.Read(Program.PackageFile))
            //    {
            //        zf.ToList().ForEach(entry =>
            //        {
            //            int step = (zf.Count / 100);
            //            int percentComplete = 0;

            //            entry.FileName = Path.GetFileName(entry.FileName);
            //            entry.Extract(Program.AimFolder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);

            //            percentComplete += step;
            //            worker.ReportProgress(percentComplete);
            //            progressReporter.ReportProgress(percentComplete, entry.FileName);
            //        });
            //    }
            //};

            worker.RunWorkerAsync();
            progressReporter.Terminate();

            var p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = Program.ApplicationExecutablePath;
            p.Start();
        }

        private IProgressReporter GetGUI()
        {
            if (File.Exists(GuiFilename))
            {
                Assembly assembly = Assembly.LoadFrom(GuiFilename);

                List<Type> validTypes =
                    assembly.GetTypes().Where(x => typeof (IProgressReporter).IsAssignableFrom(x)).ToList();
                if (validTypes.Count > 0)
                {
                    try
                    {
                        return (IProgressReporter) Activator.CreateInstance(validTypes.First());
                    }
                    catch (Exception) //No parameterless constructor
                    {
                        return (IProgressReporter) Activator.CreateInstance(typeof (MainForm)); // Show the default GUI
                    }
                }
            }

            return (IProgressReporter) Activator.CreateInstance(typeof (MainForm));
        }
    }

    //internal class DefaultProgressReporter 
    //{
    //    MainForm mainForm = new MainForm();
    //    public DefaultProgressReporter()
    //    {
    //        mainForm.ShowDialog();
    //    }
    //}
}