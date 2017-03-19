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
            return owner.Controls.Cast<Control>().Where(control => (control.GetType() == typeof(TextBox) || control.GetType() == typeof(CueTextBox) || control.GetType() == typeof(ButtonTextBox)) && control.Enabled).All(control => !string.IsNullOrWhiteSpace(control.Text));
        }

        public static bool ValidateDialogWithIgnoring(Control owner, IEnumerable<TextBox> fieldsToIgnore)
        {
            return owner.Controls.Cast<Control>().Where(control => (control.GetType() == typeof (TextBox) || control.GetType() == typeof (CueTextBox) || control.GetType() == typeof (ButtonTextBox)) && control.Enabled && fieldsToIgnore.All(c => c != control)).All(control => !string.IsNullOrWhiteSpace(control.Text));
        }
    }
}