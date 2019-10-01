using System.Windows;
using System.Windows.Controls;

namespace nUpdate.Administration.Views.NewProject
{
    /// <summary>
    /// Interaktionslogik für HttpDataPage.xaml
    /// </summary>
    public partial class HttpDataPage
    {
        public HttpDataPage()
        {
            InitializeComponent();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
                ((dynamic)DataContext).Password = ((PasswordBox)sender).Password;
        }

        private void ConfirmationPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
                ((dynamic)DataContext).ConfirmationPassword = ((PasswordBox)sender).Password;
        }
    }
}
