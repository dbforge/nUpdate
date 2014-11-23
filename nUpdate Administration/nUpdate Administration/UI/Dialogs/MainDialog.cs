// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Application.Extension;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using Application = System.Windows.Forms.Application;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class MainDialog : BaseDialog
    {
        private LocalizationProperties _lp = new LocalizationProperties();

        /// <summary>
        ///     The path of the project that is stored in a file that was opened.
        /// </summary>
        public string ProjectPath { get; set; }

        public MainDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Sets the language
        /// </summary>
        public void SetLanguage()
        {
            string languageFilePath = Path.Combine(Program.LanguagesDirectory,
                String.Format("{0}.json", Settings.Default.Language.Name));
            if (File.Exists(languageFilePath))
                _lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
            else
            {
                File.WriteAllBytes(Path.Combine(Program.LanguagesDirectory, "en.json"), Resources.en);
                Settings.Default.Language = new CultureInfo("en");
                Settings.Default.Save();
                Settings.Default.Reload();
                _lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
            }

            Text = _lp.ProductTitle;
            headerLabel.Text = _lp.ProductTitle;
            infoLabel.Text = _lp.MainDialogInfoText;

            sectionsListView.Groups[0].Header = _lp.MainDialogProjectsGroupText;
            sectionsListView.Groups[1].Header = _lp.MainDialogInformationGroupText;
            sectionsListView.Groups[2].Header = _lp.MainDialogPreferencesGroupText;

            sectionsListView.Items[0].Text = _lp.MainDialogNewProjectText;
            sectionsListView.Items[1].Text = _lp.MainDialogOpenProjectText;
            sectionsListView.Items[4].Text = _lp.MainDialogFeedbackText;
            sectionsListView.Items[5].Text = _lp.MainDialogPreferencesText;
            sectionsListView.Items[6].Text = _lp.MainDialogInformationText;
        }

        private void MainDialog_Load(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                DialogResult dr = MessageBox.Show(_lp.OperatingSystemNotSupportedWarn, String.Empty, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                    Application.Exit();
            }
            
            var fai = new FileAssociationInfo(".nupdproj");
            if (!fai.Exists)
            {
                try
                {
                    fai.Create("nUpdate Administration");

                    var pai = new ProgramAssociationInfo(fai.ProgID);
                    if (!pai.Exists)
                    {
                        pai.Create("nUpdate Administration Project File",
                            new ProgramVerb("Open", Application.ExecutablePath));
                        pai.DefaultIcon = new ProgramIcon(Application.ExecutablePath);
                    }
                }
                catch
                {
                    Popup.ShowPopup(this, SystemIcons.Error, _lp.MissingRightsWarnCaption, _lp.MissingRightsWarnText,
                        PopupButtons.Ok);
                }

                if (!String.IsNullOrEmpty(ProjectPath))
                    OpenProject(ProjectPath);
            }

            Program.LanguagesDirectory = Path.Combine(Program.Path, "Localization");
            if (!Directory.Exists(Program.LanguagesDirectory))
            {
                Directory.CreateDirectory(Program.LanguagesDirectory); // Create the directory

                // Save the language content
                var lang = new LocalizationProperties();
                string content = Serializer.Serialize(lang);
                File.WriteAllText(Path.Combine(Program.LanguagesDirectory, "en.json"), content);
            }

            if (!File.Exists(Program.ProjectsConfigFilePath))
            {
                using (File.Create(Program.ProjectsConfigFilePath))
                {
                }
            }

            if (!File.Exists(Program.StatisticServersFilePath))
            {
                using (File.Create(Program.StatisticServersFilePath))
                {
                }
            }

            string projectsPath = Path.Combine(Program.Path, "Projects");
            if (!Directory.Exists(projectsPath))
                Directory.CreateDirectory(projectsPath);

            foreach (var entryParts in File.ReadAllLines(Program.ProjectsConfigFilePath).Select(entry => entry.Split(new[] {'-'})))
            {
                Program.ExisitingProjects.Add(entryParts[0], entryParts[1]);
            }

            SetLanguage();
            sectionsListView.DoubleBuffer();
        }

        public void OpenProject(string projectPath)
        {
            try
            {
                Project = ApplicationInstance.LoadProject(projectPath);
                var credentialsDialog = new CredentialsDialog();
                if (credentialsDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Program.FtpPassword =
                            AesManager.Decrypt(Convert.FromBase64String(Project.FtpPassword),
                                credentialsDialog.Password.Trim(), credentialsDialog.Username.Trim());

                        if (Project.Proxy != null)
                            Program.ProxyPassword =
                                AesManager.Decrypt(Convert.FromBase64String(Project.ProxyPassword),
                                    credentialsDialog.Password.Trim(), credentialsDialog.Username.Trim());

                        if (Project.UseStatistics)
                            Program.SqlPassword =
                                AesManager.Decrypt(Convert.FromBase64String(Project.SqlPassword),
                                    credentialsDialog.Password.Trim(), credentialsDialog.Username.Trim());
                    }
                    catch (CryptographicException)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Invalid credentials.",
                            "The entered credentials are invalid.", PopupButtons.Ok);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "The decryption progress has failed.",
                            ex, PopupButtons.Ok);
                        return;
                    }
                }
                else
                {
                    return;
                }

                if (Project.FtpUsername != credentialsDialog.Username)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid credentials.",
                        "The entered credentials are invalid.", PopupButtons.Ok);
                    return;
                }
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, _lp.ProjectReadingErrorCaption, ex,
                    PopupButtons.Ok);
                return;
            }

            var projectDialog = new ProjectDialog {Project = Project};
            projectDialog.ShowDialog();
        }

        private void sectionsListView_Click(object sender, EventArgs e)
        {
            switch (sectionsListView.FocusedItem.Index)
            {
                case 0:
                    var newProjectDialog = new NewProjectDialog();
                    newProjectDialog.ShowDialog();
                    break;

                case 1:
                    using (var fileDialog = new OpenFileDialog())
                    {
                        fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
                        fileDialog.Multiselect = false;
                        if (fileDialog.ShowDialog() == DialogResult.OK)
                        {
                            OpenProject(fileDialog.FileName);
                        }
                    }
                    break;

                case 2:
                    var projectManagementDialog = new ProjectRemovalDialog();
                    projectManagementDialog.ShowDialog();
                    break;

                case 4:
                    var feedbackDialog = new FeedbackDialog();
                    feedbackDialog.ShowDialog();
                    break;

                case 5:
                    var preferencesDialog = new PreferencesDialog();
                    preferencesDialog.ShowDialog();
                    break;

                case 6:
                    var infoDialog = new InfoDialog();
                    infoDialog.ShowDialog();
                    break;
                case 7:
                    var statisticsServerDialog = new StatisticsServerDialog();
                    statisticsServerDialog.ReactsOnKeyDown = false;
                    statisticsServerDialog.ShowDialog();
                    break;
            }
        }
    }
}