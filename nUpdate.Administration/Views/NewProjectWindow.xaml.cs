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
        }

        public override void RequestClose()
        {
            Close();
        }
    }
}
