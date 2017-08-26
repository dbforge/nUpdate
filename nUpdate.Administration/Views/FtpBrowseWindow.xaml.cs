namespace nUpdate.Administration.Views
{
    /// <summary>
    /// Interaktionslogik für FtpBrowseWindow.xaml
    /// </summary>
    public partial class FtpBrowseWindow
    {
        public FtpBrowseWindow(ITransferData data)
        {
            InitializeComponent();
            ((dynamic) DataContext).TransferManager = new TransferManager(TransferProtocol.FTP, data);
        }

        public string Directory => ((dynamic) DataContext).Directory;
    }
}
