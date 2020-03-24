// UpdateInstaller.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using nUpdate.Actions;

namespace nUpdate.UpdateInstaller
{
    internal class UpdateInstaller
    {
        private static UpdateInstaller _instance;

        private UpdateInstaller()
        {
        }

        public static UpdateInstaller Instance => _instance ?? (_instance = new UpdateInstaller());

        private async Task CopyDirectoryContent(string sourcePath, string destPath)
        {
            var di = new DirectoryInfo(sourcePath);
            await di.GetFiles()
                .ForEachAsync(f => Task.Run(() => File.Copy(Path.Combine(sourcePath, f.Name),
                    Path.Combine(destPath, f.Name), true)));

            await di.GetDirectories().ForEachAsync(d =>
                CopyDirectoryContent(Path.Combine(sourcePath, d.Name), Path.Combine(destPath, d.Name)));
        }

        private async Task Install()
        {
            var appDirectory = Program.AppDirectory;
            var packageDirectory = Program.PackageDirectory;
            var newUpdatePackages = Program.NewUpdatePackages;

            await newUpdatePackages.ForEachAsync(async package =>
            {
                var identifier = package.Guid;
                var packagePath = Path.Combine(packageDirectory, identifier.ToString());

                var actionDataFilePath = Path.Combine(packagePath, "actions.json");
                var updateActions =
                    JsonSerializer.Deserialize<IEnumerable<IUpdateAction>>(File.ReadAllText(actionDataFilePath));

                var enumerable = updateActions as IUpdateAction[] ?? updateActions.ToArray();
                await enumerable.Where(a => a.ExecuteBeforeReplacingFiles).ForEachAsync(async a => await a.Execute());

                var directories = new DirectoryInfo(packagePath).GetDirectories();
                await directories.ForEachAsync(async d =>
                {
                    if (d.Name.Trim().Equals(Globals.AppExecutableDirectoryIdentifier))
                    {
                        await CopyDirectoryContent(d.FullName, appDirectory);
                    }
                    else
                    {
                        var specialFolderPath = Environment.GetFolderPath(
                            (Environment.SpecialFolder) Enum.Parse(typeof(Environment.SpecialFolder), d.Name.Trim()));
                        await CopyDirectoryContent(d.FullName, specialFolderPath);
                    }
                });

                await enumerable.Where(a => !a.ExecuteBeforeReplacingFiles).ForEachAsync(async a => await a.Execute());
            });
        }

        internal async void InstallUpdates()
        {
            await Install();
        }
    }
}