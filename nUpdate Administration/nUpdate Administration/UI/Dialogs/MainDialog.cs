// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Application.Extension;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using ExceptionBase;
using Application = System.Windows.Forms.Application;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class MainDialog : BaseDialog
    {
        private LocalizationProperties _lp = new LocalizationProperties();

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
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            Text = _lp.ProductTitle;
            headerLabel.Text = _lp.ProductTitle;
            infoLabel.Text = _lp.MainDialogInfoText;

            sectionsListView.Groups[0].Header = _lp.MainDialogProjectsGroupText;
            sectionsListView.Groups[1].Header = _lp.MainDialogInformationGroupText;
            sectionsListView.Groups[2].Header = _lp.MainDialogPreferencesGroupText;

            sectionsListView.Items[0].Text = _lp.MainDialogNewProjectText;
            sectionsListView.Items[1].Text = _lp.MainDialogOpenProjectText;
            sectionsListView.Items[3].Text = _lp.MainDialogFeedbackText;
            sectionsListView.Items[4].Text = _lp.MainDialogPreferencesText;
            sectionsListView.Items[5].Text = _lp.MainDialogInformationText;
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

            foreach (string entry in File.ReadAllLines(Program.ProjectsConfigFilePath))
            {
                string[] entryParts = entry.Split(new[] {'-'});
                Program.ExisitingProjects.Add(entryParts[0], entryParts[1]);
            }

            SetLanguage();
            sectionsListView.DoubleBuffer();
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
                            try
                            {
                                Project = ApplicationInstance.LoadProject(fileDialog.FileName);
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

                            var projectDialog = new ProjectDialog { Project = Project };
                            projectDialog.ShowDialog();
                        }
                    }
                    break;

                case 2:
                    var projectManagementDialog = new ProjectManagementDialog();
                    projectManagementDialog.ShowDialog();
                    break;

                case 3:
                    var feedbackDialog = new FeedbackDialog();
                    feedbackDialog.ShowDialog();
                    break;

                case 4:
                    var preferencesDialog = new PreferencesDialog();
                    preferencesDialog.ShowDialog();
                    break;

                case 5:
                    var infoDialog = new InfoDialog();
                    infoDialog.ShowDialog();
                    break;
                case 6:
                    var statisticsServerDialog = new StatisticsServerDialog();
                    statisticsServerDialog.ReactsOnKeyDown = false;
                    statisticsServerDialog.ShowDialog();
                    break;
            }
        }
    }
}