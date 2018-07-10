using System.Windows.Threading;

namespace nUpdate.ViewModel.Interfaces
{
    public interface IDialogViewModel
    {
        bool DialogResult {get; set;}
        string WindowTitle {get; set;}
        Dispatcher CurrentDispatcher { get; set; }
        void DialogLoaded();
        void DialogClosing();
    }
}
