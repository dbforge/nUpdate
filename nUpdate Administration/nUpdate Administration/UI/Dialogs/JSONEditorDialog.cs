using FastColoredTextBoxNS;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;
using nUpdate.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class JSONEditorDialog : BaseDialog
    {
        /// <summary>
        /// The content of the language file.
        /// </summary>
        public string LanguageContent { get; set; }

        /// <summary>
        /// The name of the language/culture.
        /// </summary>
        public string CultureName { get; set; }
            
        public JSONEditorDialog()
        {
            InitializeComponent();
        }

        private void JSONEditorDialog_Load(object sender, EventArgs e)
        {
            byte[] bytes = Encoding.Default.GetBytes(JsonHelper.FormatJson(this.LanguageContent));
            this.codeTextBox.Text = Encoding.UTF8.GetString(bytes);
        }

        Style argumentsStyle = new TextStyle(Brushes.Red, null, FontStyle.Bold);
        Style stringStyle = new TextStyle(Brushes.DarkRed, null, FontStyle.Regular);
        Style commentStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        Style keyWordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
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
                string filePath = Path.Combine(Program.Path, "Localization", String.Format("{0}.json", this.CultureName));
                using (FileStream fs = File.Create(filePath)) { }
                File.WriteAllText(filePath, codeTextBox.Text);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the language-file.", ex, PopupButtons.OK);
                return;
            }
            finally
            {
                this.Close();
            }
        }
    }
}
