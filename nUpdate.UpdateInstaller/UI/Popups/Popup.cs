// Popup.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.UpdateInstaller.UI.Popups
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
            if (LoggedIntoEventlog(popupIcon, infoMessage)) return DialogResult.OK;


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
            if (LoggedIntoEventlog(popupIcon, ex.ToString())) return DialogResult.OK;
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
        public static DialogResult ShowPopup(IWin32Window owner, Icon popupIcon, string title, string infoMessage,
            PopupButtons buttons)
        {
            if (LoggedIntoEventlog(popupIcon, infoMessage)) return DialogResult.OK;
            var popupWindow = new PopupDialog
            {
                PopupIcon = popupIcon,
                Title = title,
                InfoMessage = infoMessage,
                Buttons = buttons,
                StartPosition = FormStartPosition.CenterParent
            };

            return popupWindow.ShowDialog(owner);
        }

        /// <summary>
        ///     Shows a new popup-window.
        /// </summary>
        /// <param name="owner">The owner of the modal popup dialog.</param>
        /// <param name="popupIcon">The icons of the popup.</param>
        /// <param name="title">The title of the popup.</param>
        /// <param name="exception">The exception to handle in the popup-information.</param>
        /// <param name="buttons">The buttons to show for the user-interaction.</param>
        public static DialogResult ShowPopup(IWin32Window owner, Icon popupIcon, string title, Exception exception,
            PopupButtons buttons)
        {
            if (LoggedIntoEventlog(popupIcon, exception.ToString())) return DialogResult.OK;
            var popupWindow = new PopupDialog
            {
                PopupIcon = popupIcon,
                Title = title,
                InfoMessage = exception.Message,
                StartPosition = FormStartPosition.CenterParent,
                Buttons = buttons,
                Exception = exception
            };

            return popupWindow.ShowDialog();
        }


        private static bool LoggedIntoEventlog(Icon popupIcon, string infoMessage)
        {
            if (WindowsServiceHelper.IsRunningInServiceContext)
            {
                if (popupIcon == SystemIcons.Error)
                    WindowsEventLog.LogError(infoMessage);
                else if (popupIcon == SystemIcons.Warning)
                    WindowsEventLog.LogWarning(infoMessage);
                else
                    WindowsEventLog.LogInformation(infoMessage);
                return true;
            }

            return false;
        }
    }
}