// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.UI.Controls;

namespace nUpdate.Administration.Core
{
    public class ValidationManager
    {
        /// <summary>
        ///     Validates if all fields were filled out.
        /// </summary>
        /// <param name="owner">The form to validate.</param>
        /// <returns>This function returns a boolean.</returns>
        public static bool ValidateDialog(Form owner)
        {
            return (from Control control in owner.Controls
                where control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox)
                where control.Enabled
                select !string.IsNullOrEmpty(control.Text) && control.ForeColor != Color.Gray).FirstOrDefault();
        }

        /// <summary>
        ///     Validates if all fields were filled out.
        /// </summary>
        /// <param name="owner">The panel to validate.</param>
        /// <returns>This function returns a boolean.</returns>
        public static bool ValidatePanel(Panel owner)
        {
            foreach (Control control in from Control control in owner.Controls
                where control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox)
                where control.Enabled
                select control)
            {
                return !string.IsNullOrEmpty(control.Text) && control.ForeColor != Color.Gray;
            }

            return true;
        }

        /// <summary>
        ///     Validates if all fields were filled out with ignoring the given text field.
        /// </summary>
        public static bool ValidateDialogWithIgnoring(Form owner, TextBox textBoxToIgnore)
        {
            return (from Control control in owner.Controls
                where control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox)
                where control.Enabled
                where control != textBoxToIgnore
                select control).Select(control => !string.IsNullOrEmpty(control.Text) && control.ForeColor != Color.Gray)
                .FirstOrDefault();
        }

        /// <summary>
        ///     Validates if all fields were filled out with ignoring the given text field.
        /// </summary>
        public static bool ValidatePanelWithIgnoring(Panel owner, TextBox textBoxToIgnore)
        {
            return (from Control control in owner.Controls
                where control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox)
                where control.Enabled
                where control != textBoxToIgnore
                select control).Select(control => !string.IsNullOrEmpty(control.Text) && control.ForeColor != Color.Gray)
                .FirstOrDefault();
        }
    }
}