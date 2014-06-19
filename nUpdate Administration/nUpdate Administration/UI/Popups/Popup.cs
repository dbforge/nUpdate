using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using nUpdate.Administration.UI.Dialogs;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Popups
{
    internal class Popup
    {
        /// <summary>
        /// Shows a new popup-window.
        /// </summary>
        /// <param name="popupIcon">The icons of the popup.</param>
        /// <param name="title">The title of the popup.</param>
        /// <param name="infoMessage">The info message of the popup.</param>
        public static DialogResult ShowPopup(IWin32Window owner, Icon popupIcon, string title, string infoMessage, PopupButtons buttons) 
        {
            var popupWindow = new PopupDialog()
            {
                PopupIcon = popupIcon,
                Title = title,
                InfoMessage = infoMessage,
                Buttons = buttons,
                StartPosition = FormStartPosition.CenterParent,
            };
            
            return popupWindow.ShowDialog(owner);
        }

        /// <summary>
        /// Shows a new popup-window.
        /// </summary>
        /// <param name="popupIcon">The icons of the popup.</param>
        /// <param name="title">The title of the popup.</param>
        /// <param name="infoMessage">The info message of the popup.</param>
        public static DialogResult ShowPopup(IWin32Window owner, Icon popupIcon, string title, Exception exception, PopupButtons buttons)
        {
            var popupWindow = new PopupDialog()
            {
                PopupIcon = popupIcon,
                Title = title,
                InfoMessage = exception.Message,
                StartPosition = FormStartPosition.CenterParent,
                Buttons = buttons,
                Exception = exception,
            };

            return popupWindow.ShowDialog();
        }
    }
}
