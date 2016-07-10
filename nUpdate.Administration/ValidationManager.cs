// Author: Dominic Beger (Trade/ProgTrade)

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.UserInterface.Controls;

namespace nUpdate.Administration
{
    internal class ValidationManager
    {
        internal static bool Validate(Control owner)
        {
            return (from Control control in owner.Controls
                    where
                        control.GetType() == typeof(TextBox) || control.GetType() == typeof(CueTextBox) ||
                        control.GetType() == typeof(ButtonTextBox)
                    where control.Enabled
                    select !string.IsNullOrWhiteSpace(control.Text) && control.ForeColor != Color.Gray).FirstOrDefault();
        }

        internal static bool ValidateTabPage(TabPage owner)
        {
            foreach (Control control in owner.Controls.Cast<Control>().Where(control => (control.GetType() == typeof (TextBox) || control.GetType() == typeof (CueTextBox) ||
                                                                                        control.GetType() == typeof (ButtonTextBox)) && control.Enabled))
            {
                return !string.IsNullOrWhiteSpace(control.Text) && control.ForeColor != Color.Gray;
            }

            return true;
        }

        public static bool ValidateDialogWithIgnoring(Control owner, IEnumerable<TextBox> fieldsToIgnore)
        {
            return (from Control control in owner.Controls
                    where control.GetType() == typeof(TextBox) || control.GetType() == typeof(CueTextBox)
                    where control.Enabled
                    where fieldsToIgnore.All(item => item != control)
                    select control).Select(
                    control => !string.IsNullOrWhiteSpace(control.Text) && control.ForeColor != Color.Gray)
                .FirstOrDefault();
        }
    }
}