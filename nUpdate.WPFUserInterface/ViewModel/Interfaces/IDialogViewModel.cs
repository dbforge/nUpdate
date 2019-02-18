using System.Windows.Threading;

// ReSharper disable once CheckNamespace
namespace nUpdate.WPFUserInterface.ViewModel.Interfaces
{
    public interface IDialogViewModel
    {
        bool DialogResult { get; set; }
        string WindowTitle { get; set; }
        Dispatcher CurrentDispatcher { get; set; }
        void DialogLoaded();
        void DialogClosing();
    }
}