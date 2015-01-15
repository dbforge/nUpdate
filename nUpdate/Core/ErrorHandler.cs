// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Windows.Forms;
using nUpdate.UI.Dialogs;

namespace nUpdate.Core
{
    internal class ErrorHandler
    {
        public static void ShowErrorDialog(int errorCode, string infoMessage, Exception error)
        {
            ShowErrorDialog(null, errorCode, infoMessage, error);
        }

        public static void ShowErrorDialog(IWin32Window owner, int errorCode, string infoMessage, Exception error)
        {
            var errorDialog = new UpdateErrorDialog
            {
                ErrorCode = errorCode,
                Error = error,
                InfoMessage = infoMessage
            };

            if (owner != null)
                errorDialog.ShowDialog(owner);
        }
    }
}