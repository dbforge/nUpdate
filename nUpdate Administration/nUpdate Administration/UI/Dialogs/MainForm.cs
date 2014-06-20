#define DEBUG

using Microsoft.WindowsAPICodePack.Taskbar;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Application.Extension;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.UI.Dialogs;
using nUpdate.Administration.UI.Popups;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace nUpdate.Administration
{
    public partial class MainForm : BaseForm
    {
        private string notSupportedWarning;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the language
        /// </summary>
        public void SetLanguage()
        {
            LocalizationProperties ls = new LocalizationProperties();
            if (File.Exists(Program.LanguageSerializerFilePath))
            {
                ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(Program.LanguageSerializerFilePath));
            }
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            this.Text = ls.ProductTitle;
            this.headerLabel.Text = ls.ProductTitle;
            this.infoLabel.Text = ls.MainFormInfoText;

            this.sectionsListView.Groups[0].Header = ls.MainFormProjectsGroupText;
            this.sectionsListView.Groups[1].Header = ls.MainFormOptionGroupText;
            this.sectionsListView.Groups[2].Header = ls.MainFormInformationGroupText;

            this.sectionsListView.Items[0].Text = ls.MainFormNewProjectText;
            this.sectionsListView.Items[1].Text = ls.MainFormOpenProjectText;
            this.sectionsListView.Items[2].Text = ls.MainFormSettingsText;
            this.sectionsListView.Items[3].Text = ls.MainFormInformationText;
            this.sectionsListView.Items[4].Text = ls.MainFormFeedbackText;

            this.notSupportedWarning = ls.MainFormNotSupportedWarn;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                DialogResult dr = MessageBox.Show(this.notSupportedWarning, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                {
                    Application.Exit();
                }
            }

            FileAssociationInfo fai = new FileAssociationInfo(".nupdproj");
            if (!fai.Exists)
            {
                try
                {
                    fai.Create("nUpdate Administration");

                    ProgramAssociationInfo pai = new ProgramAssociationInfo(fai.ProgID);
                    if (!pai.Exists)
                    {
                        pai.Create("nUpdate Administration Project File", new ProgramVerb("Open", Application.ExecutablePath));
                        pai.DefaultIcon = new ProgramIcon(Application.ExecutablePath);
                    }
                }
                catch
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing rights.", "You do not own the admin rights to create the extension's registry entry.", PopupButtons.OK);
                }
            }

            Program.LanguageSerializerFilePath = Path.Combine(Program.Path, "lang.xml");

            // Create program folder
            if (!Directory.Exists(Program.Path))
            {
                Directory.CreateDirectory(Program.Path);
            }

            LocalizationProperties lang = new LocalizationProperties();
            string content = Serializer.Serialize(lang);
            File.WriteAllText(Program.LanguageSerializerFilePath, content);

            this.SetLanguage();
            this.sectionsListView.DoubleBuffer();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            JumpList list = JumpList.CreateJumpList();
            list.ClearAllUserTasks();

            JumpListLink taskLink = new JumpListLink(Assembly.GetEntryAssembly().Location, "Create new project");
            taskLink.Arguments = "-1";

            list.AddUserTasks(taskLink);
            list.Refresh();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WindowsMessageHelper.NewProjectArg)
            {
                //NewProjectForm projectForm = new NewProjectForm();
                //projectForm.ShowDialog();
                MessageBox.Show("lol");
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void sectionsListView_Click(object sender, EventArgs e)
        {
            switch (this.sectionsListView.FocusedItem.Index)
            {
                case 0:
                    NewProjectForm projectForm = new NewProjectForm();
                    projectForm.ShowDialog();
                    break;

                case 1:
                    OpenFileDialog fileDialog = new OpenFileDialog();
                    fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
                    fileDialog.Multiselect = false;
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            this.Project = ApplicationInstance.LoadProject(fileDialog.FileName);
                            ShowDialog<ProjectForm>(this, this.Project);
                        }
                        catch (Exception ex)
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while reading project data.", ex, PopupButtons.OK);
                        }
                    }
                    break;

                case 2:
                    OptionsForm optionForm = new OptionsForm();
                    optionForm.ShowDialog();
                    break;

                case 3:
                    FeedbackForm feedbackForm = new FeedbackForm();
                    feedbackForm.ShowDialog();
                    break;

                case 4:
                    InfoForm infoForm = new InfoForm();
                    infoForm.ShowDialog();
                    break;
            }
        }
    }
}
