using nUpdate.Administration.ViewModels;
using nUpdate.Administration.ViewModels.Providers;

namespace nUpdate.Administration.Views
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow
    {
        public NewProjectWindow()
        {
            InitializeComponent();
            DataContext = new NewProjectViewModel(NewProjectProvider.Instance);
        }

        public override void RequestClose()
        {
            Close();
        }
    }
}
