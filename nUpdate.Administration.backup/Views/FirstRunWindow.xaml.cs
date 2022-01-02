﻿// FirstRunWindow.xaml.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.ComponentModel;
using System.Windows;
using nUpdate.Administration.ViewModels.FirstRun;
using nUpdate.Administration.Views.FirstRun;
using TaskDialogInterop;

namespace nUpdate.Administration.Views
{
    /// <summary>
    ///     Interaction logic for FirstRunWindow.xaml
    /// </summary>
    public partial class FirstRunWindow
    {
        private bool _finished;

        public FirstRunWindow()
        {
            InitializeComponent();
            DataContext = new FirstRunBase(FirstRunProvider.Instance);
        }

        public override void RequestClose()
        {
            // It's not the user who tries to close the window, but the application itself. Allow it then.
            _finished = true;
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_finished)
                return;

            e.Cancel = true;
            var taskDialog = new TaskDialogOptions
            {
                Owner = this,
                Title = "nUpdate Administration v4.0",
                MainIcon = VistaTaskDialogIcon.Warning,
                MainInstruction = "Cancel the first time configuration?",
                Content =
                    "Are you sure that you want to cancel the first time configuration? If you select \"Yes\", nUpdate Administration will exit and open the configuration dialog on the next start.",
                CommonButtons = TaskDialogCommonButtons.YesNo
            };

            if (TaskDialog.Show(taskDialog).Result == TaskDialogSimpleResult.Yes)
                Application.Current.Shutdown();
        }
    }
}