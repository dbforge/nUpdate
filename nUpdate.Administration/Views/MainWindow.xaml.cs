// MainWindow.xaml.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Windows;
using ModernWpf;
using nUpdate.Administration.ViewModels;

namespace nUpdate.Administration.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(MainWindowActionProvider.Instance);
        }

        private void ToggleTheme(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark ? ApplicationTheme.Light : ApplicationTheme.Dark;
        }
    }
}