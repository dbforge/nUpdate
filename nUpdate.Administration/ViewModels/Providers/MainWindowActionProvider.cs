using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using nUpdate.Administration.Common;
using nUpdate.Administration.Common.Providers;
using nUpdate.Administration.Views;
using nUpdate.Administration.Views.Dialogs;
using TaskDialogInterop;

namespace nUpdate.Administration.ViewModels.Providers
{
    internal class MainWindowActionProvider : Singleton<MainWindowActionProvider>, IMainViewActionProvider
    {
        public MainWindowActionProvider()
        {
            if (!Directory.Exists(PathProvider.SettingsDirectoryFilePath))
                Directory.CreateDirectory(PathProvider.SettingsDirectoryFilePath);
            SettingsManager.Instance.Initialize();

        }

        public void Load()
        {
            if ((bool)SettingsManager.Instance["FirstRun"])
            {
                WindowManager.ShowModalWindow<FirstRunWindow>();
                return;
            }

            if (!(bool)SettingsManager.Instance["UseEncryptedKeyDatabase"])
                return;

            bool correctPassword = false;
            while (!correctPassword)
            {
                var passwordDialog = new PasswordInputDialog();
                var showDialog = WindowManager.ShowModalWindow(passwordDialog);
                if (showDialog == null || !showDialog.Value)
                {
                    Application.Current.Shutdown();
                    return;
                }

                try
                {
                    KeyManager.Instance.Initialize(passwordDialog.Password);
                    GlobalSession.MasterPassword = passwordDialog.Password;
                    correctPassword = true;
                }
                catch (CryptographicException)
                {
                    var taskDialog = new TaskDialogOptions
                    {
                        Owner = WindowManager.GetCurrentWindow(),
                        Title = "nUpdate Administration v4.0",
                        MainIcon = VistaTaskDialogIcon.Error,
                        MainInstruction = "Decrypting the key database failed.",
                        Content = "Invalid password."
                    };

                    TaskDialog.Show(taskDialog);
                }
            }
        }

        public bool CanEditMasterPassword()
        {
            return SettingsManager.Instance.CheckExistance("UseEncryptedKeyDatabase") && (bool)SettingsManager.Instance["UseEncryptedKeyDatabase"];
        }

        public List<MainMenuItem> GetCollectionView()
        {
            var view = new List<MainMenuItem>(new List<MainMenuItem>
                {
                    new MainMenuItem("New project", "Creates a new project linked with your application.", MainMenuGroup.Projects, /*new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/NewSolutionFolder_64x.png", UriKind.Relative)),*/ new RelayCommand(
                        () =>
                        {
                            WindowManager.ShowModalWindow<NewProjectWindow>();
                        })),

                    new MainMenuItem("Open project", "Opens an existing project for managing its updates.", MainMenuGroup.Projects, /*new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/ProjectFolderOpen_64x.png", UriKind.Relative)),*/ new RelayCommand(
                        () => { })),

                    new MainMenuItem("Manage key database", "Import, export or edit your passwords or private keys.", MainMenuGroup.KeyDatabase, /*new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/Key_64x.png", UriKind.Relative)),*/ new RelayCommand(
                        () => { })),

                    new MainMenuItem("Change master password", "Change the password used for encrypting the key database.", MainMenuGroup.KeyDatabase, /*new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/Lock_64x.png", UriKind.Relative)),*/ new RelayCommand(
                        () => {}, CanEditMasterPassword)),

                    new MainMenuItem("Manage statistics servers", "Add, edit or delete statistics servers.", MainMenuGroup.Statistics, /*new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/ServerReport_64x.png", UriKind.Relative)),*/ new RelayCommand(
                        () => { })),

                    new MainMenuItem("Preferences", "Customize nUpdate Administration.", MainMenuGroup.Application, /*new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/Settings_64x.png", UriKind.Relative)),*/ new RelayCommand(
                        () => { })),

                    new MainMenuItem("Information", "About nUpdate Administration.", MainMenuGroup.Application, /*new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/CodeInformation_64x.png", UriKind.Relative)),*/ new RelayCommand(
                        () => { })),

                    new MainMenuItem("Feedback", "Send your feedback right here.", MainMenuGroup.Application, /*new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/feedbacktoolicon_64x.png", UriKind.Relative)),*/ new RelayCommand(
                        () => { }))
                });
            return view;
        }
    }
}
