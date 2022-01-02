// FtpDataPage.xaml.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Security.Authentication;
using System.Windows;
using System.Windows.Controls;
using FluentFTP;

namespace nUpdate.Administration.Views.NewProject
{
    /// <summary>
    ///     Interaktionslogik für FtpDataPage.xaml
    /// </summary>
    public partial class FtpDataPage
    {
        public FtpDataPage()
        {
            InitializeComponent();
        }

        private void FtpDataPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            ModeComboBox.ItemsSource = Enum.GetValues(typeof(FtpEncryptionMode));
            ProtocolComboBox.ItemsSource = Enum.GetValues(typeof(SslProtocols));
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
                ((dynamic) DataContext).Password = ((PasswordBox) sender).Password;
        }
    }
}