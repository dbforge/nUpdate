using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Core.Update.Extension;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Dialogs;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration
{
    public partial class MainDialog : BaseDialog
    {
        private LocalizationProperties lp = new LocalizationProperties();

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
                lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            Text = lp.ProductTitle;
            headerLabel.Text = lp.ProductTitle;
            infoLabel.Text = lp.MainDialogInfoText;

            sectionsListView.Groups[0].Header = lp.MainDialogProjectsGroupText;
            sectionsListView.Groups[1].Header = lp.MainDialogInformationGroupText;
            sectionsListView.Groups[2].Header = lp.MainDialogPreferencesGroupText;

            sectionsListView.Items[0].Text = lp.MainDialogNewProjectText;
            sectionsListView.Items[1].Text = lp.MainDialogOpenProjectText;
            sectionsListView.Items[2].Text = lp.MainDialogFeedbackText;
            sectionsListView.Items[3].Text = lp.MainDialogPreferencesText;
            sectionsListView.Items[4].Text = lp.MainDialogInformationText;
        }

        private void MainDialog_Load(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                DialogResult dr = MessageBox.Show(lp.OperatingSystemNotSupportedWarn, String.Empty, MessageBoxButtons.OK,
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
                    Popup.ShowPopup(this, SystemIcons.Error, lp.MissingRightsWarnCaption, lp.MissingRightsWarnText,
                        PopupButtons.OK);
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
                using (FileStream fs = File.Create(Program.ProjectsConfigFilePath))
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
                                var projectDialog = new ProjectDialog();
                                projectDialog.Project = Project;
                                projectDialog.ShowDialog();
                            }
                            catch (Exception ex)
                            {
                                Popup.ShowPopup(this, SystemIcons.Error, lp.ProjectReadingErrorCaption, ex,
                                    PopupButtons.OK);
                            }
                        }
                    }
                    break;

                case 2:
                    var feedbackDialog = new FeedbackDialog();
                    feedbackDialog.ShowDialog();
                    break;

                case 3:
                    var preferencesDialog = new PreferencesDialog();
                    preferencesDialog.ShowDialog();
                    break;

                case 4:
                    var infoDialog = new InfoDialog();
                    infoDialog.ShowDialog();
                    break;
                    //case 5:
                    //    StatisticsServerAddDialog stats = new StatisticsServerAddDialog();
                    //    stats.ShowDialog();
                    //    break; // Internal preparements
            }
        }
    }
}