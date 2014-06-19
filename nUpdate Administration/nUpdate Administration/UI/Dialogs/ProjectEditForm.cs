using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.UI.Popups;
using System.IO;
using System.Reflection;

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
        public string NewName
        {
            get;
            set;
        }

        /// <summary>
        /// The programming language of the project.
        /// </summary>
        public ProgrammingLanguage Language
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the language of the form.
        /// </summary>
        public void SetLanguage()
        {
            LocalizationProperties ls = new LocalizationProperties();
            if (File.Exists(Program.LanguageSerializerFilePath))
                ls = Serializer.Deserialize<LocalizationProperties>(Program.LanguageSerializerFilePath);
            else
            {

                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            Text = ls.EditFormTitle;
            headerLabel.Text = ls.EditFormTitle;
            newNameLabel.Text = ls.EditFormNewNameText;
            languageLabel.Text = ls.EditFormLanguageText;
            newTextBox.Text = ls.EditFormRenameText;
        }

        private void ProjectEditForm_Load(object sender, EventArgs e)
        {
            SetLanguage();
            
            if (this.Language == ProgrammingLanguage.VB)
                languageComboBox.SelectedIndex = 0;

            else if (this.Language == ProgrammingLanguage.CSharp)
                languageComboBox.SelectedIndex = 1;

            newTextBox.Text = Project.Name;
            updateUrlTextBox.Text = Project.UpdateUrl;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (newTextBox.Text != Project.Name)
                Project.Name = newTextBox.Text;
            
            if (updateUrlTextBox.Text != Project.UpdateUrl)
                Project.UpdateUrl = updateUrlTextBox.Text;

            if (languageComboBox.SelectedIndex == 0)
                Project.ProgrammingLanguage = "VB.NET";
            else
                Project.ProgrammingLanguage = "C#";

            Project.Path = Path.Combine(Path.GetDirectoryName(Project.Path), String.Format("{0}.nupdproj", newTextBox.Text));
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
                "Please note that changing the update-url will only affect new update packages. Older ones will furthermore be available under the old url and use the old configuration.", PopupButtons.OK);
        }
    }
}
