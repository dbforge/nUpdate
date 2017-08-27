using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.Properties;
using nUpdate.Administration.Views;
using nUpdate.Administration.Views.Dialogs;
using TaskDialogInterop;

// ReSharper disable MemberCanBeMadeStatic.Local

namespace nUpdate.Administration.ViewModels
{
    public class MainWindowViewModel : WindowViewModel
    {
        private ICommand _loadCommand;

        public MainWindowViewModel()
        {
            LoadCommand = new RelayCommand(OnLoad);
            CollectionView =
                new ListCollectionView(new List<MainMenuItemViewModel>
                {
                    new MainMenuItemViewModel("New project", "Creates a new project linked with your application.", MainMenuGroup.Projects, new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/NewSolutionFolder_64x.png", UriKind.Relative)), new RelayCommand(
                        () =>
                        {
                            WindowManager.ShowDialog(new NewProjectWindow());
                        })),

                    new MainMenuItemViewModel("Open project", "Opens an existing project for managing its updates.", MainMenuGroup.Projects, new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/ProjectFolderOpen_64x.png", UriKind.Relative)), new RelayCommand(
                        () => { })),

                    new MainMenuItemViewModel("Manage key database", "Import, export or edit your passwords or private keys.", MainMenuGroup.KeyDatabase, new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/Key_64x.png", UriKind.Relative)), new RelayCommand(
                        () => { })),

                    new MainMenuItemViewModel("Change master password", "Change the password used for encrypting the key database.", MainMenuGroup.KeyDatabase, new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/Lock_64x.png", UriKind.Relative)), new RelayCommand(() => {}, CanEditMasterPassword)),

                    new MainMenuItemViewModel("Manage statistics servers", "Add, edit or delete statistics servers.", MainMenuGroup.Statistics, new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/ServerReport_64x.png", UriKind.Relative)), new RelayCommand(
                        () => { })),

                    new MainMenuItemViewModel("Preferences", "Customize nUpdate Administration.", MainMenuGroup.Application, new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/Settings_64x.png", UriKind.Relative)), new RelayCommand(
                        () => { })),

                    new MainMenuItemViewModel("Information", "About nUpdate Administration.", MainMenuGroup.Application, new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/CodeInformation_64x.png", UriKind.Relative)), new RelayCommand(
                        () => { })),

                    new MainMenuItemViewModel("Feedback", "Send your feedback right here.", MainMenuGroup.Application, new BitmapImage(new Uri(@"/nUpdate.Administration;component/Icons/feedbacktoolicon_64x.png", UriKind.Relative)), new RelayCommand(
                        () => { }))
                });
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
        }

        public ICollectionView CollectionView { get; }


        public ICommand LoadCommand
        {
            get => _loadCommand;
            set => SetProperty(value, ref _loadCommand);
        }

        private bool CanEditMasterPassword()
        {
            return Settings.Default.UseEncryptedKeyDatabase;
        }

        private void OnLoad()
        {
            if (Settings.Default.FirstRun)
            {
                WindowManager.ShowDialog(new FirstRunWindow());
                return;
            }

            if (!Settings.Default.UseEncryptedKeyDatabase)
                return;
            
            bool correctPassword = false;
            while (!correctPassword)
            {
                var passwordDialog = new PasswordInputDialog();
                var showDialog = WindowManager.ShowDialog(passwordDialog);
                if (showDialog == null || !showDialog.Value)
                {
                    Application.Current.Shutdown();
                    return;
                }

                try
                {
                    KeyManager.Instance.Initialize(passwordDialog.Password);
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
    }
}