// App.xaml.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.Windows;

namespace nUpdate.WPFUpdateInstaller
{
    /// <summary>
    ///     Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Program.Main(e.Args);
        }
    }
}