// Copyright © Dominic Beger 2018

using System;
using System.Drawing;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Application.Extension;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class MainDialog : BaseDialog
    {
        private SecureString _ftpPassword = new SecureString();
        private SecureString _proxyPassword = new SecureString();

        private SecureString _sqlPassword = new SecureString();
        //private readonly LocalizationProperties _lp = new LocalizationProperties();

        public MainDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The path of the project that is stored in a file that was opened.
        /// </summary>
        public string ProjectPath { get; set; }

        ///// <summary>
        /////     Sets the language
        ///// </summary>
        //public void SetLanguage()
        //{
        //    string languageFilePath = Path.Combine(Program.LanguagesDirectory,
        //        String.Format("{0}.json", Settings.Default.Language.Name));
        //    if (File.Exists(languageFilePath))
        //        _lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
        //    else
        //    {
        //        File.WriteAllBytes(Path.Combine(Program.LanguagesDirectory, "en.json"), Resources.en);
        //        Settings.Default.Language = new CultureInfo("en");
        //        Settings.Default.Save();
        //        Settings.Default.Reload();
        //        _lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
        //    }

        //    Text = _lp.ProductTitle;
        //    headerLabel.Text = _lp.ProductTitle;
        //    infoLabel.Text = _lp.MainDialogInfoText;

        //    sectionsListView.Groups[0].Header = _lp.MainDialogProjectsGroupText;
        //    sectionsListView.Groups[1].Header = _lp.MainDialogInformationGroupText;
        //    sectionsListView.Groups[2].Header = _lp.MainDialogPreferencesGroupText;

        //    sectionsListView.Items[0].Text = _lp.MainDialogNewProjectText;
        //    sectionsListView.Items[1].Text = _lp.MainDialogOpenProjectText;
        //    sectionsListView.Items[4].Text = _lp.MainDialogFeedbackText;
        //    sectionsListView.Items[5].Text = _lp.MainDialogPreferencesText;
        //    sectionsListView.Items[6].Text = _lp.MainDialogInformationText;
        //}

        private void MainDialog_Load(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                var dr = MessageBox.Show("Your operating system is not supported.", string.Empty,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                    Application.Exit();
            }

            try
            {
                var fai = new FileAssociationInfo(".nupdproj");
                if (!fai.Exists)
                {
                    fai.Create("nUpdate Administration");

                    var pai = new ProgramAssociationInfo(fai.ProgId);
                    if (!pai.Exists)
                    {
                        pai.Create("nUpdate Administration Project File",
                            new ProgramVerb("Open", $"\"{Application.ExecutablePath} %1\""));
                        pai.DefaultIcon = new ProgramIcon(Application.ExecutablePath);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Popup.ShowPopup(this, SystemIcons.Warning, "Missing rights.",
                    "The registry entry for the extension (.nupdproj) couldn't be created. Without that file extension nUpdate Administration won't work correctly. Please make sure to start the administration with admin privileges the first time.",
                    PopupButtons.Ok);
            }

            if (string.IsNullOrWhiteSpace(Settings.Default.ProgramPath))
                Settings.Default.ProgramPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "nUpdate Administration");
            Program.LanguagesDirectory = Path.Combine(Program.Path, "Localization");
            if (!Directory.Exists(Program.LanguagesDirectory))
            {
                Directory.CreateDirectory(Program.LanguagesDirectory); // Create the directory

                // Save the language content
                var lang = new LocalizationProperties();
                var content = Serializer.Serialize(lang);
                File.WriteAllText(Path.Combine(Program.LanguagesDirectory, "en.json"), content);
            }

            if (!File.Exists(Program.ProjectsConfigFilePath))
                using (File.Create(Program.ProjectsConfigFilePath))
                {
                }

            if (!File.Exists(Program.StatisticServersFilePath))
                using (File.Create(Program.StatisticServersFilePath))
                {
                }

            var projectsPath = Path.Combine(Program.Path, "Projects");
            if (!Directory.Exists(projectsPath))
                Directory.CreateDirectory(projectsPath);

            //SetLanguage();
            sectionsListView.DoubleBuffer();
            Text = string.Format(Text, Program.VersionString);
            headerLabel.Text = string.Format(Text, Program.VersionString);
        }

        private void MainDialog_Shown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProjectPath))
                return;

            Project = OpenProject(ProjectPath);
            if (Project == null)
                return;

            var projectDialog = new ProjectDialog
            {
                Project = Project,
                FtpPassword = _ftpPassword.Copy(),
                ProxyPassword = _proxyPassword.Copy(),
                SqlPassword = _sqlPassword.Copy()
            };
            if (projectDialog.ShowDialog() != DialogResult.OK)
                return;

            _ftpPassword.Dispose();
            _proxyPassword.Dispose();
            _sqlPassword.Dispose();
        }

        public UpdateProject OpenProject(string projectPath)
        {
            UpdateProject project;
            try
            {
                project = UpdateProject.LoadProject(projectPath);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while reading the project.", ex,
                    PopupButtons.Ok);
                return null;
            }

            if (!project.SaveCredentials)
            {
                var credentialsDialog = new CredentialsDialog();
                if (credentialsDialog.ShowDialog() == DialogResult.OK)
                    try
                    {
                        _ftpPassword =
                            AesManager.Decrypt(Convert.FromBase64String(project.FtpPassword),
                                credentialsDialog.Password.Trim(), credentialsDialog.Username.Trim());

                        if (project.Proxy != null)
                            _proxyPassword =
                                AesManager.Decrypt(Convert.FromBase64String(project.ProxyPassword),
                                    credentialsDialog.Password.Trim(), credentialsDialog.Username.Trim());

                        if (project.UseStatistics)
                            _sqlPassword =
                                AesManager.Decrypt(Convert.FromBase64String(project.SqlPassword),
                                    credentialsDialog.Password.Trim(), credentialsDialog.Username.Trim());
                    }
                    catch (CryptographicException)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Invalid credentials.",
                            "The entered credentials are invalid.", PopupButtons.Ok);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "The decryption progress has failed.",
                            ex, PopupButtons.Ok);
                        return null;
                    }
                else
                    return null;

                if (project.FtpUsername == credentialsDialog.Username)
                    return project;

                Popup.ShowPopup(this, SystemIcons.Error, "Invalid credentials.",
                    "The entered credentials are invalid.", PopupButtons.Ok);
                return null;
            }

            try
            {
                _ftpPassword =
                    AesManager.Decrypt(Convert.FromBase64String(project.FtpPassword),
                        Program.AesKeyPassword, Program.AesIvPassword);

                if (project.Proxy != null)
                    _proxyPassword =
                        AesManager.Decrypt(Convert.FromBase64String(project.ProxyPassword),
                            Program.AesKeyPassword, Program.AesIvPassword);

                if (project.UseStatistics)
                    _sqlPassword =
                        AesManager.Decrypt(Convert.FromBase64String(project.SqlPassword),
                            Program.AesKeyPassword, Program.AesIvPassword);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "The decryption progress has failed.",
                    ex, PopupButtons.Ok);
                return null;
            }

            return project;
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
                            Project = OpenProject(fileDialog.FileName);
                            if (Project == null)
                                return;

                            var projectDialog = new ProjectDialog
                            {
                                Project = Project,
                                FtpPassword = _ftpPassword.Copy(),
                                ProxyPassword = _proxyPassword.Copy(),
                                SqlPassword = _sqlPassword.Copy()
                            };
                            if (projectDialog.ShowDialog() == DialogResult.OK)
                            {
                                _ftpPassword.Dispose();
                                _proxyPassword.Dispose();
                                _sqlPassword.Dispose();
                            }
                        }
                    }

                    break;

                case 2:
                    var projectRemovalDialog = new ProjectRemovalDialog();
                    projectRemovalDialog.ShowDialog();
                    break;

                case 3:
                    using (var fileDialog = new OpenFileDialog())
                    {
                        fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
                        fileDialog.Multiselect = false;
                        if (fileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Project = OpenProject(fileDialog.FileName);
                            if (Project == null)
                                return;

                            var projectEditDialog = new ProjectEditDialog
                            {
                                Project = Project,
                                FtpPassword = _ftpPassword,
                                ProxyPassword = _proxyPassword,
                                SqlPassword = _sqlPassword
                            };
                            if (projectEditDialog.ShowDialog() == DialogResult.OK)
                            {
                                _ftpPassword.Dispose();
                                _proxyPassword.Dispose();
                                _sqlPassword.Dispose();
                            }
                        }
                    }

                    break;

                case 4:
                    var projectImportDialog = new ProjectImportDialog();
                    projectImportDialog.ShowDialog();
                    break;

                case 5:
                    var feedbackDialog = new FeedbackDialog();
                    feedbackDialog.ShowDialog();
                    break;

                case 6:
                    var preferencesDialog = new PreferencesDialog();
                    preferencesDialog.ShowDialog();
                    break;

                case 7:
                    var infoDialog = new InfoDialog();
                    infoDialog.ShowDialog();
                    break;
                case 8:
                    var statisticsServerDialog = new StatisticsServerDialog {ReactsOnKeyDown = false};
                    statisticsServerDialog.ShowDialog();
                    break;
            }
        }
    }
}