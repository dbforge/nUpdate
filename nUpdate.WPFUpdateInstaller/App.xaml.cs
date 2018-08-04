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