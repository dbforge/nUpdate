using nUpdate.ViewModel.Interfaces;

namespace nUpdate.ServiceInterfaces
{
    public interface IDialogWindowService
    {
        void ShowDialog(string windowname, IDialogViewModel datacontext);
        void CloseDialog();
        
    }
}
