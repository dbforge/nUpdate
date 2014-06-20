using nUpdate.Dialogs;
using System.Windows.Forms;

namespace nUpdate.Core
{
    internal class ErrorHandler
    {
        public static void ShowErrorDialog(int errorCode, string infoMessage, string errorMessage)
        {
            ShowErrorDialog(null, errorCode, infoMessage, errorMessage);
        }

        public static void ShowErrorDialog(IWin32Window owner, int errorCode, string infoMessage, string errorMessage)
        {
            var errorDialog = new UpdateErrorDialog()
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                InfoMessage = infoMessage,
            };

            if (owner != null)
            {
                errorDialog.ShowDialog(owner);
            }
        }
    }
}
