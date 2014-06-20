using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.Administration.Core
{
    internal class ValidationManager
    {
        /// <summary>
        /// Validates if all fields were filled out.
        /// </summary>
        /// <param name="owner">The form to validate.</param>
        /// <returns>This function returns a boolean.</returns>
        public static bool ValidateDialog(Form owner)
        {
            foreach (Control control in owner.Controls)
            {
                if (control.GetType() == typeof(TextBox) || control.GetType() == typeof(WatermarkTextBox))
                {
                    if (control.Enabled == true)
                    {
                        if (string.IsNullOrEmpty(control.Text) || control.ForeColor == Color.Gray)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Validates if all fields were filled out.
        /// </summary>
        /// <param name="owner">The panel to validate.</param>
        /// <returns>This function returns a boolean.</returns>
        public static bool ValidatePanel(Panel owner)
        {
            foreach (Control control in owner.Controls)
            {
                if (control.GetType() == typeof(TextBox) || control.GetType() == typeof(WatermarkTextBox))
                {
                    if (control.Enabled == true)
                    {
                        if (string.IsNullOrEmpty(control.Text) || control.ForeColor == Color.Gray)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Validates if all fields were filled out.
        /// </summary>
        public static bool ValidateDialog(Panel owner, TextBox unsupportedTextBox)
        {
            foreach (Control control in owner.Controls)
            {
                if (control.GetType() == typeof(TextBox) || control.GetType() == typeof(WatermarkTextBox))
                {
                    if (control.Enabled == true)
                    {
                        if (control != unsupportedTextBox)
                        {
                            if (string.IsNullOrEmpty(control.Text) || control.ForeColor == Color.Gray)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(control.Text) || control.Text == "/")
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Validates if all fields were filled out.
        /// </summary>
        public static bool ValidateDialog(Form owner, TextBox unsupportedTextBox)
        {
            foreach (Control control in owner.Controls)
            {
                if (control.GetType() == typeof(TextBox) || control.GetType() == typeof(WatermarkTextBox))
                {
                    if (control.Enabled == true)
                    {
                        if (control != unsupportedTextBox)
                        {
                            if (string.IsNullOrEmpty(control.Text) || control.ForeColor == Color.Gray)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(control.Text) || control.Text == "/")
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
