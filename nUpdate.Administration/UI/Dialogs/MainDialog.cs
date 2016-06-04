// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using nUpdate.Administration.Application;
using nUpdate.Administration.Application.Extension;
using nUpdate.Administration.Localization;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using static System.String;

namespace nUpdate.Administration.UI.Dialogs
{
    internal partial class MainDialog : BaseDialog
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
            if (!Directory.Exists(FilePathProvider.LanguagesDirectory))
            {
                Directory.CreateDirectory(FilePathProvider.LanguagesDirectory); // Create the directory

                // Save the language content
                var lang = new LocalizationProperties();
                var content = Serializer.Serialize(lang);
                File.WriteAllText(Path.Combine(FilePathProvider.LanguagesDirectory, "en.json"), content);
            }

            if (!File.Exists(FilePathProvider.ProjectsConfigFilePath))
                File.Create(FilePathProvider.ProjectsConfigFilePath).Close();

            if (!File.Exists(FilePathProvider.StatisticServersFilePath))
                File.Create(FilePathProvider.StatisticServersFilePath).Close();

            var projectsPath = Path.Combine(FilePathProvider.Path, "Projects");
            if (!Directory.Exists(projectsPath))
                Directory.CreateDirectory(projectsPath);

            sectionsListView.DoubleBuffer();
            Text = Format(Text, Program.VersionString);
            headerLabel.Text = Format(Text, Program.VersionString);
        }

        public void OpenProject(string projectPath)
        {
            UpdateProject project;
            try
            {
                project = UpdateProject.Load(projectPath);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while reading the project data.", ex,
                    PopupButtons.Ok);
                return;
            }

            // Initialize our application session, so that we can access all data of this project
            Session.InitializeProject(project);
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

                            var projectDialog = new ProjectDialog();
                            projectDialog.ShowDialog();
                            Session.Terminate();
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
                            OpenProject(fileDialog.FileName);

                            var projectEditDialog = new ProjectEditDialog();
                            projectEditDialog.ShowDialog();
                            Session.Terminate();
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
                    var statisticsServerDialog = new StatisticalServerDialog {ReactsOnKeyDown = false};
                    statisticsServerDialog.ShowDialog();
                    break;
            }
        }

        private void MainDialog_Shown(object sender, EventArgs e)
        {
            if (IsNullOrEmpty(ProjectPath))
                return;

            OpenProject(ProjectPath);

            var projectDialog = new ProjectDialog();
            projectDialog.ShowDialog();
            Session.Terminate();
        }
    }
}