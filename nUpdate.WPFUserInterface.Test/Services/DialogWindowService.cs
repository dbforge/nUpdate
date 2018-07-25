using System.Windows;
using nUpdate.ServiceInterfaces;

namespace nUpdate.WPFUserInterface.Test.Services
{
    class DialogWindowService : IDialogWindowService
    {
        private DialogWindow _currentWindow;
        public void ShowDialog(string windowname, object datacontext)
        {
            _currentWindow = new DialogWindow()
            {
                Name = windowname,
                DataContext = datacontext,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
            _currentWindow.ShowDialog();
        }

        public void CloseDialog()
        {
            _currentWindow.Close();
        }
        
    }
}
