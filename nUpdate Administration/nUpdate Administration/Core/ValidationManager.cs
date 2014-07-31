using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.Administration.Core
{
    internal class ValidationManager
    {
        /// <summary>
        ///     Validates if all fields were filled out.
        /// </summary>
        /// <param name="owner">The form to validate.</param>
        /// <returns>This function returns a boolean.</returns>
        public static bool ValidateDialog(Form owner)
        {
            foreach (Control control in owner.Controls)
            {
                if (control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox))
                {
                    if (control.Enabled)
                    {
                        if (string.IsNullOrEmpty(control.Text) || control.ForeColor == Color.Gray)
                            return false;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     Validates if all fields were filled out.
        /// </summary>
        /// <param name="owner">The panel to validate.</param>
        /// <returns>This function returns a boolean.</returns>
        public static bool ValidatePanel(Panel owner)
        {
            foreach (Control control in owner.Controls)
            {
                if (control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox))
                {
                    if (control.Enabled)
                    {
                        if (string.IsNullOrEmpty(control.Text) || control.ForeColor == Color.Gray)
                            return false;
                        return true;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Validates if all fields were filled out with checking a directory field.
        /// </summary>
        public static bool ValidatePanel(Panel owner, TextBox directoryTextBox)
        {
            foreach (Control control in owner.Controls)
            {
                if (control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox))
                {
                    if (control.Enabled)
                    {
                        if (control != directoryTextBox)
                        {
                            if (string.IsNullOrEmpty(control.Text) || control.ForeColor == Color.Gray)
                                return false;
                            return true;
                        }
                        if (string.IsNullOrEmpty(control.Text) || control.Text == "/")
                            return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     Validates if all fields were filled out with checking a directory field.
        /// </summary>
        public static bool ValidateDialog(Form owner, TextBox directoryTextBox)
        {
            foreach (Control control in owner.Controls)
            {
                if (control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox))
                {
                    if (control.Enabled)
                    {
                        if (control != directoryTextBox)
                        {
                            if (string.IsNullOrEmpty(control.Text) || control.ForeColor == Color.Gray)
                                return false;
                            return true;
                        }
                        if (string.IsNullOrEmpty(control.Text) || control.Text == "/")
                            return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     Validates if all fields were filled out with ignoring the given text field.
        /// </summary>
        public static bool ValidateDialogWithIgnoring(Form owner, TextBox textBoxToIgnore)
        {
            foreach (Control control in owner.Controls)
            {
                if (control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox))
                {
                    if (control.Enabled)
                    {
                        if (control != textBoxToIgnore)
                        {
                            if (string.IsNullOrEmpty(control.Text) || control.ForeColor == Color.Gray)
                                return false;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     Validates if all fields were filled out with ignoring the given text field.
        /// </summary>
        public static bool ValidatePanelWithIgnoring(Panel owner, TextBox textBoxToIgnore)
        {
            foreach (Control control in owner.Controls)
            {
                if (control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox))
                {
                    if (control.Enabled)
                    {
                        if (control != textBoxToIgnore)
                        {
                            if (string.IsNullOrEmpty(control.Text) || control.ForeColor == Color.Gray)
                                return false;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}