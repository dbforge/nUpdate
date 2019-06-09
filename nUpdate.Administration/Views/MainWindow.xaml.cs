using nUpdate.Administration.ViewModels;
using nUpdate.Administration.ViewModels.Providers;

namespace nUpdate.Administration.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(MainWindowActionProvider.Instance);
        }
    }
}
