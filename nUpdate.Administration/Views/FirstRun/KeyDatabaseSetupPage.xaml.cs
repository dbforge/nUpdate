using System.Windows;
using System.Windows.Controls;

namespace nUpdate.Administration.Views.FirstRun
{
    /// <summary>
    /// Interaction logic for KeyDatabaseSetupPage.xaml
    /// </summary>
    public partial class KeyDatabaseSetupPage
    {
        public KeyDatabaseSetupPage()
        {
            InitializeComponent();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
                ((dynamic)DataContext).MasterPassword = ((PasswordBox)sender).Password;
        }

        private void PasswordVerifyChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
                ((dynamic)DataContext).MasterPasswordVerify = ((PasswordBox)sender).Password;
        }
    }
}
