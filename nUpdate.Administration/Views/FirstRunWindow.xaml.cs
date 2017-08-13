using System.ComponentModel;
using nUpdate.Administration.ViewModels;

namespace nUpdate.Administration.Views
{
    /// <summary>
    /// Interaction logic for FirstRunWindow.xaml
    /// </summary>
    public partial class FirstRunWindow
    {
        public FirstRunWindow()
        {
            InitializeComponent();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (DataContext == null || DataContext.GetType() != typeof(PagedWindowViewModel))
                return;

            if (!((dynamic) DataContext).AllowClosing)
                e.Cancel = true;
        }
    }
}
