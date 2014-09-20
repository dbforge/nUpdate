// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
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
                String.Format("{0}.json", Settings.Default.Language.Name));
            LocalizationProperties ls;
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
            newTextBox.Text = ls.ProjectEditDialogRenameText;
        }

        private void ProjectProjectEditDialog_Load(object sender, EventArgs e)
        {
            SetLanguage();
            newTextBox.Text = Project.Name;
            updateUrlTextBox.Text = Project.UpdateUrl;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (newTextBox.Text != Project.Name)
                Project.Name = newTextBox.Text;

            if (updateUrlTextBox.Text != Project.UpdateUrl)
                Project.UpdateUrl = updateUrlTextBox.Text;

            string projectFilePath= Path.Combine(Path.GetDirectoryName(Project.Path), String.Format("{0}.nupdproj", newTextBox.Text));

            try
            {
                File.Move(projectFilePath, Project.Path);
                ApplicationInstance.SaveProject(Project.Path, Project);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving project data.", ex, PopupButtons.Ok);
            }

            DialogResult = DialogResult.OK;
        }

/*
        private void newTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                saveButton.PerformClick();
        }
*/

        private void updateUrlTextBox_Enter(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Warning, "Important information.",
                "Please note that changing the update-url will only affect new update packages. Older ones will furthermore be available under the old url and use the old configuration.",
                PopupButtons.Ok);
        }
    }
}