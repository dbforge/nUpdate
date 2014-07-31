using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectEditDialog : BaseDialog
    {
        public ProjectEditDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The new name of the project.
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        ///     Sets the language of the form.
        /// </summary>
        public void SetLanguage()
        {
            string languageFilePath = Path.Combine(Program.LanguagesDirectory,
                String.Format("{0}.json", Settings.Default.Language.LCID));
            var ls = new LocalizationProperties();
            if (File.Exists(languageFilePath))
                ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            Text = ls.ProjectEditDialogTitle;
            headerLabel.Text = ls.ProjectEditDialogTitle;
            newNameLabel.Text = ls.ProjectEditDialogNewNameText;
            languageLabel.Text = ls.ProjectEditDialogLanguageText;
            newTextBox.Text = ls.ProjectEditDialogRenameText;
        }

        private void ProjectProjectEditDialog_Load(object sender, EventArgs e)
        {
            SetLanguage();

            switch (Project.ProgrammingLanguage)
            {
                case "VB.NET":
                    languageComboBox.SelectedIndex = 0;
                    break;
                case "C#":
                    languageComboBox.SelectedIndex = 1;
                    break;
            }

            newTextBox.Text = Project.Name;
            updateUrlTextBox.Text = Project.UpdateUrl;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (newTextBox.Text != Project.Name)
                Project.Name = newTextBox.Text;

            if (updateUrlTextBox.Text != Project.UpdateUrl)
                Project.UpdateUrl = updateUrlTextBox.Text;

            switch (languageComboBox.SelectedIndex)
            {
                case 0:
                    Project.ProgrammingLanguage = "VB.NET";
                    break;
                case 1:
                    Project.ProgrammingLanguage = "C#";
                    break;
            }

            Project.Path = Path.Combine(Path.GetDirectoryName(Project.Path),
                String.Format("{0}.nupdproj", newTextBox.Text));
            Project.HasUnsavedChanges = true;

            DialogResult = DialogResult.OK;
        }

        private void newTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                saveButton.PerformClick();
        }

        private void updateUrlTextBox_Enter(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Warning, "Important information.",
                "Please note that changing the update-url will only affect new update packages. Older ones will furthermore be available under the old url and use the old configuration.",
                PopupButtons.OK);
        }
    }
}