using nUpdate.WPFUserInterface.ViewModel.Interfaces;

// ReSharper disable once CheckNamespace
namespace nUpdate.WPFUserInterface.ServiceInterfaces
{
    public interface IDialogWindowService
    {
        void ShowDialog(string windowname, IDialogViewModel datacontext);
        void CloseDialog();
    }
}