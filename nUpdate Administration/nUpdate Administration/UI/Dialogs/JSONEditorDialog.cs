using System;
using System.Drawing;
using System.IO;
using System.Text;
using FastColoredTextBoxNS;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class JSONEditorDialog : BaseDialog
    {
        private readonly Style argumentsStyle = new TextStyle(Brushes.Red, null, FontStyle.Bold);
        private readonly Style commentStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private readonly Style keyWordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        private Style stringStyle = new TextStyle(Brushes.DarkRed, null, FontStyle.Regular);

        public JSONEditorDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The content of the language file.
        /// </summary>
        public string LanguageContent { get; set; }

        /// <summary>
        ///     The name of the language/culture.
        /// </summary>
        public string CultureName { get; set; }

        private void JSONEditorDialog_Load(object sender, EventArgs e)
        {
            byte[] bytes = Encoding.Default.GetBytes(JsonHelper.FormatJson(LanguageContent));
            codeTextBox.Text = Encoding.UTF8.GetString(bytes);
        }

        private void codeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(argumentsStyle);
            e.ChangedRange.SetStyle(argumentsStyle, @"\{[0-9]+\}?.");
            e.ChangedRange.SetStyle(keyWordStyle, @"null");
            //e.ChangedRange.SetStyle(stringStyle, "(?<=(?<!\)").+?(?=(?<!\)")", RegexOptions.Global);
            e.ChangedRange.SetStyle(commentStyle, @".*//.*$");
            e.ChangedRange.SetFoldingMarkers("{", "}");
        }

        private void saveLanguageButton_Click(object sender, EventArgs e)
        {
            try
            {
                Serializer.Serialize(codeTextBox.Text);
            }
            catch (FormatException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "The JSON-code is invalid.", ex, PopupButtons.OK);
                return;
            }

            try
            {
                string filePath = Path.Combine(Program.Path, "Localization", String.Format("{0}.json", CultureName));
                using (FileStream fs = File.Create(filePath))
                {
                }
                File.WriteAllText(filePath, codeTextBox.Text);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the language-file.", ex, PopupButtons.OK);
            }
            finally
            {
                Close();
            }
        }
    }
}