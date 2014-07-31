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
    public partial class ProjectEditDialog : BaseDialog
    {
        public ProjectEditDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The new name of the project.
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// Sets the language of the form.
        /// </summary>
        public void SetLanguage()
        {
            string languageFilePath = Path.Combine(Program.LanguagesDirectory, String.Format("{0}.json", Properties.Settings.Default.Language.LCID));
            LocalizationProperties ls = new LocalizationProperties();
            if (File.Exists(languageFilePath))
            {
                ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
            }
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            this.Text = ls.ProjectEditDialogTitle;
            this.headerLabel.Text = ls.ProjectEditDialogTitle;
            this.newNameLabel.Text = ls.ProjectEditDialogNewNameText;
            this.languageLabel.Text = ls.ProjectEditDialogLanguageText;
            this.newTextBox.Text = ls.ProjectEditDialogRenameText;
        }

        private void ProjectProjectEditDialog_Load(object sender, EventArgs e)
        {
            this.SetLanguage();
            
            switch (this.Project.ProgrammingLanguage)
            {
                case "VB.NET":
                    this.languageComboBox.SelectedIndex = 0;
                    break;
                case "C#":
                    this.languageComboBox.SelectedIndex = 1;
                    break;
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

            switch (languageComboBox.SelectedIndex)
            {
                case 0:
                    this.Project.ProgrammingLanguage = "VB.NET";
                    break;
                case 1:
                    this.Project.ProgrammingLanguage = "C#";
                    break;
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
