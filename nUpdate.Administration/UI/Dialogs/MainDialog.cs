// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using nUpdate.Administration.Application;
using nUpdate.Administration.Application.Extension;
using nUpdate.Administration.Localization;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using static System.String;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class MainDialog : BaseDialog
    {
        public MainDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the path of the project that is stored in the file that was opened.
        /// </summary>
        public string ProjectPath { get; set; }

        private void MainDialog_Load(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                var dr = MessageBox.Show("Your operating system is not supported.", Empty,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                    System.Windows.Forms.Application.Exit();
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
                            new ProgramVerb("Open",
                                $"\"{System.Windows.Forms.Application.ExecutablePath} %1\""));
                        pai.DefaultIcon = new ProgramIcon(System.Windows.Forms.Application.ExecutablePath);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Popup.ShowPopup(this, SystemIcons.Warning, "Missing rights.",
                    "The registry entry for the extension (.nupdproj) couldn't be created. Without that file extension nUpdate Administration won't work correctly. Please make sure to start the administration with admin privileges the first time.",
                    PopupButtons.Ok);
            }

            if (IsNullOrWhiteSpace(Settings.Default.ProgramPath))
                Settings.Default.ProgramPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "nUpdate Administration");
            if (!Directory.Exists(Program.LanguagesDirectory))
            {
                Directory.CreateDirectory(Program.LanguagesDirectory); // Create the directory

                // Save the language content
                var lang = new LocalizationProperties();
                var content = Serializer.Serialize(lang);
                File.WriteAllText(Path.Combine(Program.LanguagesDirectory, "en.json"), content);
            }

            if (!File.Exists(Program.ProjectsConfigFilePath))
                File.Create(Program.ProjectsConfigFilePath).Close();

            if (!File.Exists(Program.StatisticServersFilePath))
                File.Create(Program.StatisticServersFilePath).Close();

            var projectsPath = Path.Combine(Program.Path, "Projects");
            if (!Directory.Exists(projectsPath))
                Directory.CreateDirectory(projectsPath);

            sectionsListView.DoubleBuffer();
            Text = Format(Text, Program.VersionString);
            headerLabel.Text = Format(Text, Program.VersionString);

            /* Since 4.0.0 */
            var projectConfiguration = ProjectConfiguration.Load(); 
            foreach (var configuration in projectConfiguration.Where(item => item.Guid == Guid.Empty))
            {
                var project = UpdateProject.LoadProject(configuration.Path);
                configuration.Guid = project.Guid;

                string oldPath = Path.Combine(Program.Path, "Projects", configuration.Name);
                try
                {
                    Directory.Move(oldPath, Path.Combine(Program.Path, "Projects", configuration.Guid.ToString()));
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error,
                        $"Error while updating the path for project {configuration.Name}.",
                        ex, PopupButtons.Ok);
                    break;
                }
            }

            try
            {
                File.WriteAllText(Program.ProjectsConfigFilePath, Serializer.Serialize(projectConfiguration));
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the new project configuration.",
                       ex, PopupButtons.Ok);
            }
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
                {
                    try
                    {
                        project.RuntimeFtpPassword =
                            AESManager.Decrypt(Convert.FromBase64String(project.FtpPassword),
                                credentialsDialog.Password.Trim(), credentialsDialog.Username.Trim());

                        if (project.Proxy != null)
                            project.RuntimeProxyPassword =
                                AESManager.Decrypt(Convert.FromBase64String(project.ProxyPassword),
                                    credentialsDialog.Password.Trim(), credentialsDialog.Username.Trim());

                        if (project.UseStatistics)
                            project.RuntimeSqlPassword =
                                AESManager.Decrypt(Convert.FromBase64String(project.SqlPassword),
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
                }
                else
                {
                    return null;
                }

                if (project.FtpUsername == credentialsDialog.Username)
                    return project;

                Popup.ShowPopup(this, SystemIcons.Error, "Invalid credentials.",
                    "The entered credentials are invalid.", PopupButtons.Ok);
                return null;
            }

            try
            {
                project.RuntimeFtpPassword =
                    AESManager.Decrypt(Convert.FromBase64String(project.FtpPassword),
                        Program.AesKeyPassword, Program.AesIvPassword);

                if (project.Proxy != null)
                    project.RuntimeProxyPassword =
                        AESManager.Decrypt(Convert.FromBase64String(project.ProxyPassword),
                            Program.AesKeyPassword, Program.AesIvPassword);

                if (project.UseStatistics)
                    project.RuntimeSqlPassword =
                        AESManager.Decrypt(Convert.FromBase64String(project.SqlPassword),
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
                            };
                            projectDialog.ShowDialog();
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
                            };
                            projectEditDialog.ShowDialog();
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
                    var infoDialog = new InformationDialog();
                    infoDialog.ShowDialog();
                    break;
                case 8:
                    var statisticsServerDialog = new StatisticsServerDialog {ReactsOnKeyDown = false};
                    statisticsServerDialog.ShowDialog();
                    break;
            }
        }

        private void MainDialog_Shown(object sender, EventArgs e)
        {
            if (IsNullOrEmpty(ProjectPath))
                return;

            Project = OpenProject(ProjectPath);
            if (Project == null)
                return;

            var projectDialog = new ProjectDialog
            {
                Project = Project,
            };
            projectDialog.ShowDialog();
        }
    }
}