// Author: Dominic Beger (Trade/ProgTrade) 2017

using System.Windows;
using System.Windows.Controls;

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
            ModeComboBox.ItemsSource = new[] {"Passive (recommended)", "Active"};
            ProtocolComboBox.ItemsSource = new[]
            {
                "FTP (insecure)", "FTPS (TLS1 Explicit)", "FTPS (TLS1 or SSL3 Explicit)", "FTPS (SSL3 explicit)",
                "FTPS (SSL2 explicit)", "FTPS (TLS1 implicit)", "FTPS (TLS1 or SSL3 implicit)", "FTPS (SSL3 implicit)",
                "FTPS (SSL2 implicit)"
            };
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
                ((dynamic)DataContext).Password = ((PasswordBox)sender).Password;
        }
    }
}