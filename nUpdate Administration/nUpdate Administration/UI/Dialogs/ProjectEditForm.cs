using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.UI.Popups;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectEditForm : BaseForm
    {
        public ProjectEditForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The new name of the project.
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// The programming language of the project.
        /// </summary>
        public ProgrammingLanguage Language { get; set; }

        /// <summary>
        /// Sets the language of the form.
        /// </summary>
        public void SetLanguage()
        {
            LocalizationProperties ls = new LocalizationProperties();
            if (File.Exists(Program.LanguageSerializerFilePath))
            {
                ls = Serializer.Deserialize<LocalizationProperties>(Program.LanguageSerializerFilePath);
            }
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            this.Text = ls.EditFormTitle;
            this.headerLabel.Text = ls.EditFormTitle;
            this.newNameLabel.Text = ls.EditFormNewNameText;
            this.languageLabel.Text = ls.EditFormLanguageText;
            this.newTextBox.Text = ls.EditFormRenameText;
        }

        private void ProjectEditForm_Load(object sender, EventArgs e)
        {
            this.SetLanguage();
            
            if (this.Language == ProgrammingLanguage.VB)
            {
                this.languageComboBox.SelectedIndex = 0;
            }
            else if (this.Language == ProgrammingLanguage.CSharp)
            {
                this.languageComboBox.SelectedIndex = 1;
            }

            this.newTextBox.Text = this.Project.Name;
            this.updateUrlTextBox.Text = this.Project.UpdateUrl;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (this.newTextBox.Text != this.Project.Name)
            {
                this.Project.Name = this.newTextBox.Text;
            }

            if (this.updateUrlTextBox.Text != this.Project.UpdateUrl)
            {
                this.Project.UpdateUrl = this.updateUrlTextBox.Text;
            }

            if (languageComboBox.SelectedIndex == 0)
            {
                this.Project.ProgrammingLanguage = "VB.NET";
            }
            else
            {
                this.Project.ProgrammingLanguage = "C#";
            }

            this.Project.Path = Path.Combine(Path.GetDirectoryName(this.Project.Path), String.Format("{0}.nupdproj", this.newTextBox.Text));
            this.Project.HasUnsavedChanges = true;

            this.DialogResult = DialogResult.OK;
        }

        private void newTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.saveButton.PerformClick();
            }
        }

        private void updateUrlTextBox_Enter(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Warning, "Important information.", "Please note that changing the update-url will only affect new update packages. Older ones will furthermore be available under the old url and use the old configuration.", PopupButtons.OK);
        }
    }
}
