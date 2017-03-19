// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace nUpdate.Administration.UserInterface.Popups
{
    public class Popup
    {
        /// <summary>
        ///     Shows a new popup-window.
        /// </summary>
        /// <param name="popupIcon">The icons of the popup.</param>
        /// <param name="title">The title of the popup.</param>
        /// <param name="infoMessage">The info message of the popup.</param>
        /// <param name="buttons">The buttons to show for the user-interaction.</param>
        public static DialogResult ShowPopup(Icon popupIcon, string title, string infoMessage, PopupButtons buttons)
        {
            var popupWindow = new PopupDialog
            {
                PopupIcon = popupIcon,
                Title = title,
                InfoMessage = infoMessage,
                Buttons = buttons,
                StartPosition = FormStartPosition.CenterParent
            };

            return popupWindow.ShowDialog();
        }

        /// <summary>
        ///     Shows a new popup-window.
        /// </summary>
        /// <param name="popupIcon">The icons of the popup.</param>
        /// <param name="title">The title of the popup.</param>
        /// <param name="ex">The exception to handle in the popup-information.</param>
        /// <param name="buttons">The buttons to show for the user-interaction.</param>
        public static DialogResult ShowPopup(Icon popupIcon, string title, Exception ex, PopupButtons buttons)
        {
            var popupWindow = new PopupDialog
            {
                PopupIcon = popupIcon,
                Title = title,
                InfoMessage = ex.Message,
                Buttons = buttons,
                StartPosition = FormStartPosition.CenterParent,
                Exception = ex
            };

            return popupWindow.ShowDialog();
        }

        /// <summary>
        ///     Shows a new popup-window.
        /// </summary>
        /// <param name="owner">The owner of the modal popup dialog.</param>
        /// <param name="popupIcon">The icons of the popup.</param>
        /// <param name="title">The title of the popup.</param>
        /// <param name="infoMessage">The info message of the popup.</param>
        /// <param name="buttons">The buttons to show for the user-interaction.</param>
        public static TaskDialogResult ShowPopup(IWin32Window owner, Icon popupIcon, string title, string infoMessage,
            PopupButtons buttons)
        {
            /*var popupWindow = new PopupDialog
            {
                PopupIcon = popupIcon,
                Title = title,
                InfoMessage = infoMessage,
                Buttons = buttons,
                StartPosition = FormStartPosition.CenterParent
            };

            return popupWindow.ShowDialog(owner);*/
            var dialog = new TaskDialog
            {
                Caption = Program.VersionString,
                InstructionText = title,
                Text = infoMessage,
                Icon = TaskDialogStandardIcon.Error,
                Cancelable = false
            };
            return dialog.Show();
        }

        /// <summary>
        ///     Shows a new popup-window.
        /// </summary>
        /// <param name="owner">The owner of the modal popup dialog.</param>
        /// <param name="popupIcon">The icons of the popup.</param>
        /// <param name="title">The title of the popup.</param>
        /// <param name="exception">The exception to handle in the popup-information.</param>
        /// <param name="buttons">The buttons to show for the user-interaction.</param>
        public static TaskDialogResult ShowPopup(IWin32Window owner, Icon popupIcon, string title, Exception exception,
            PopupButtons buttons)
        {
            /*var popupWindow = new PopupDialog
            {
                PopupIcon = popupIcon,
                Title = title,
                InfoMessage = exception.Message,
                StartPosition = FormStartPosition.CenterParent,
                Buttons = buttons,
                Exception = exception
            };

            return popupWindow.ShowDialog();*/

            var dialog = new TaskDialog
            {
                Caption = Program.VersionString,
                InstructionText = title,
                Text = exception.Message,
                Icon = TaskDialogStandardIcon.Error,
                Cancelable = false,
                DetailsExpanded = false,
                DetailsCollapsedLabel = "Show error information",
                DetailsExpandedLabel = "Hide error information",
                DetailsExpandedText = exception.StackTrace
            };
            
            var copyMessageButton = new TaskDialogButton("copyMessageButton", "Copy error details");
            copyMessageButton.Click += (sender, args) => Clipboard.SetText(exception.StackTrace);

            var closeButton = new TaskDialogButton("closeButton", "Close") {Default = true};
            closeButton.Click += (sender, args) => dialog.Close();

            dialog.Controls.Add(copyMessageButton);
            dialog.Controls.Add(closeButton);

            return dialog.Show();
        }
    }
}