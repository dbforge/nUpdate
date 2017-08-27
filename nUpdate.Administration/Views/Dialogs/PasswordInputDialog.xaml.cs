// Author: Dominic Beger (Trade/ProgTrade) 2017

using System.Windows;
using System.Windows.Interop;
using nUpdate.Administration.Win32;
using TaskDialogInterop;

namespace nUpdate.Administration.Views.Dialogs
{
    public partial class PasswordInputDialog
    {
        public PasswordInputDialog()
        {
            InitializeComponent();
        }

        public string Password { get; set; }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            var taskDialog = new TaskDialogOptions
            {
                Owner = this,
                Title = "nUpdate Administration v4.0",
                MainIcon = VistaTaskDialogIcon.Warning,
                MainInstruction = "Are you sure that you want to cancel?",
                Content =
                    "The master password needs to be entered in order to use nUpdate Administration. If you cancel this operation, nUpdate Administration will be terminated. Are you sure?",
                CommonButtons = TaskDialogCommonButtons.YesNo
            };

            if (TaskDialog.Show(taskDialog).Result == TaskDialogSimpleResult.Yes)
                DialogResult = false;
        }

        private void ContinueButton_OnClick(object sender, RoutedEventArgs e)
        {
            Password = PasswordBox.Password;
            DialogResult = true;
        }

        private void PasswordInputDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_STYLE,
                NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_STYLE) & ~NativeMethods.WS_SYSMENU);
        }
    }
}