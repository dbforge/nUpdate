using System.Windows;
using nUpdate.ServiceInterfaces;

// ReSharper disable once CheckNamespace
namespace nUpdate.WPFUserInterface.Services
{
    class MessageBoxService : IMessageboxService
    {
        public EnuMessageBoxResult Show(string message, string title, EnuMessageBoxButton buttons = EnuMessageBoxButton.Ok,
            EnuMessageBoxImage image = EnuMessageBoxImage.None)
        {
            return (EnuMessageBoxResult) MessageBox.Show(message, title, (MessageBoxButton) buttons,(MessageBoxImage) image);
        }
    }
}
