using nUpdate.Client.GuiInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace nUpdate.UpdateInstaller
{
    public class Updater
    {
        private static readonly string GuiFilename;
        private const string DefaultUpdaterGui = "UpdaterGui.dll";

        static Updater()
        {
            try
            {
                GuiFilename = ConfigurationManager.AppSettings["UpdaterGuiAssembly"];
                if (String.IsNullOrWhiteSpace(GuiFilename))
                    GuiFilename = DefaultUpdaterGui;
            }
            catch(Exception)
            {
                GuiFilename = DefaultUpdaterGui;
            }
        }

        /// <summary>
        /// Runs the update installation process.
        /// </summary>
        public void RunUpdate()
        {
            var progressReporter = GetGUI();
            progressReporter.Initialize();
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (o, e) =>
            {
                using (var zf = Ionic.Zip.ZipFile.Read(Program.PackageFile))
                {
                    zf.ToList().ForEach(entry =>
                    {
                        int step = (zf.Count / 100);
                        int percentComplete = 0;

                        entry.FileName = Path.GetFileName(entry.FileName);
                        entry.Extract(Program.AimFolder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);

                        percentComplete += step;
                        worker.ReportProgress(percentComplete);
                        progressReporter.ReportProgress(percentComplete, entry.FileName);
                    });
                }
            };

            worker.RunWorkerAsync();
            progressReporter.Terminate();

            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = Program.ApplicationExecutablePath; 
            p.Start();
        }

        private IProgressReporter GetGUI()
        {
            if (File.Exists(GuiFilename))
            {
                var assembly = Assembly.LoadFrom(GuiFilename);

                var validTypes = assembly.GetTypes().Where(x => typeof(IProgressReporter).IsAssignableFrom(x)).ToList();
                if(validTypes.Count > 0)
                {
                    try
                    {
                        return (IProgressReporter)Activator.CreateInstance(validTypes.First());
                    }
                    catch(Exception) //No parameterless constructor
                    {
                        return (IProgressReporter)Activator.CreateInstance(typeof(MainForm)); // Show the default GUI
                    }
                }
            }

            return (IProgressReporter)Activator.CreateInstance(typeof(MainForm));
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
