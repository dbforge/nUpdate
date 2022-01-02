﻿// MainWindowActionProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.IO;
using System.Security.Cryptography;
using System.Windows;
using nUpdate.Administration.BusinessLogic;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views.Dialogs;
using TaskDialogInterop;

namespace nUpdate.Administration.Views
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
            if ((bool) SettingsManager.Instance["FirstRun"])
            {
                WindowManager.ShowModalWindow<FirstRunWindow>();
                return;
            }

            if (!(bool) SettingsManager.Instance["UseEncryptedKeyDatabase"])
                return;

            var correctPassword = false;
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
            return SettingsManager.Instance.CheckExistence("UseEncryptedKeyDatabase") &&
                   (bool) SettingsManager.Instance["UseEncryptedKeyDatabase"];
        }

        public void CreateNewProject()
        {
            WindowManager.ShowModalWindow(new NewProjectWindow());
        }
    }
}