using System.ComponentModel;
using System.Windows;
using nUpdate.Internal.Core;
using nUpdate.WPFUserInterface.ViewModel;
using nUpdate.WPFUserInterface.ViewModel.Interfaces;
using Application = System.Windows.Forms.Application;

namespace nUpdate.UI.Windows
{
    /// <summary>
    ///     Interaktionslogik für DialogWindow.xaml
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

        private void DialogWindow_Closing(object sender, CancelEventArgs e)
        {
            var vm = (IDialogViewModel) DataContext;
            vm.DialogClosing();
            Loaded -= DialogWindow_Loaded;
            Closing -= DialogWindow_Closing;

            var updateUiVm = (UpdateUiBaseViewModel) DataContext;
            Icon = updateUiVm.GetIcon(IconHelper.ExtractAssociatedIcon(Application.ExecutablePath));
        }

        private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (IDialogViewModel) DataContext;
            vm.CurrentDispatcher = Dispatcher;
            vm.DialogLoaded();
        }
    }
}