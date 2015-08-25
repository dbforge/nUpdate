// Author: Dominic Beger (Trade/ProgTrade)

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.UI.Controls;

namespace nUpdate.Administration
{
    public class ValidationManager
    {
        /// <summary>
        ///     Validates if all fields were filled out.
        /// </summary>
        /// <param name="owner">The form to validate.</param>
        /// <returns>This function returns a boolean.</returns>
        public static bool Validate(Control owner)
        {
            return (from Control control in owner.Controls
                where
                    control.GetType() == typeof (TextBox) || control.GetType() == typeof (CueTextBox) ||
                    control.GetType() == typeof (ButtonTextBox)
                where control.Enabled
                select !string.IsNullOrWhiteSpace(control.Text) && control.ForeColor != Color.Gray).FirstOrDefault();
        }

        /// <summary>
        ///     Validates if all fields were filled out.
        /// </summary>
        /// <param name="owner">The panel to validate.</param>
        /// <returns>This function returns a boolean.</returns>
        public static bool ValidateTabPage(TabPage owner)
        {
            foreach (var control in from Control control in owner.Controls
                where control.GetType() == typeof (TextBox) || control.GetType() == typeof (CueTextBox)
                where control.Enabled
                select control)
            {
                return !string.IsNullOrWhiteSpace(control.Text) && control.ForeColor != Color.Gray;
            }

            return true;
        }

        /// <summary>
        ///     Validates if all fields were filled out with ignoring the given text fields.
        /// </summary>
        public static bool ValidateDialogWithIgnoring(Form owner, IEnumerable<TextBox> fieldsToIgnore)
        {
            return (from Control control in owner.Controls
                where control.GetType() == typeof (TextBox) || control.GetType() == typeof (CueTextBox)
                where control.Enabled
                where fieldsToIgnore.All(item => item != control)
                select control).Select(
                    control => !string.IsNullOrWhiteSpace(control.Text) && control.ForeColor != Color.Gray)
                .FirstOrDefault();
        }

        /// <summary>
        ///     Validates if all fields were filled out with ignoring the given text field.
        /// </summary>
        public static bool ValidateWithIgnoring(Control owner, TextBox textBoxToIgnore)
        {
            return (from Control control in owner.Controls
                where control.GetType() == typeof (TextBox) || control.GetType() == typeof (CueTextBox)
                where control.Enabled
                where control != textBoxToIgnore
                select control).Select(
                    control => !string.IsNullOrWhiteSpace(control.Text) && control.ForeColor != Color.Gray)
                .FirstOrDefault();
        }
    }
}