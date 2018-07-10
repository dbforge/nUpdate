using System.Windows;
using nUpdate.Internal.Core;
using nUpdate.ViewModel.Interfaces;
using nUpdate.WPFUserInterface.ViewModel;
using Application = System.Windows.Forms.Application;

namespace nUpdate.UI.Windows
{
    /// <summary>
    /// Interaktionslogik für DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow
    {
        public DialogWindow(IDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Title = vm.WindowTitle;
            Loaded += DialogWindow_Loaded;
            Closing += DialogWindow_Closing;
        }

        private void DialogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = (IDialogViewModel)DataContext;
            vm.DialogClosing();
            Loaded -= DialogWindow_Loaded;
            Closing -= DialogWindow_Closing;

            var updateUiVm = (UpdateUiBaseVm)DataContext;
            Icon = updateUiVm.GetIcon(IconHelper.ExtractAssociatedIcon(Application.ExecutablePath));
        }

        private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (IDialogViewModel)DataContext;
            vm.CurrentDispatcher = Dispatcher;
            vm.DialogLoaded();
        }


    }
}
