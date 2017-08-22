using System;
using System.Windows;
using Starksoft.Aspen.Ftps;

namespace nUpdate.Administration.Views.NewProject
{
    /// <summary>
    /// Interaktionslogik für FtpDataPage.xaml
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
            ProtocolComboBox.ItemsSource = Enum.GetValues(typeof(FtpsSecurityProtocol));
        }
    }
}
