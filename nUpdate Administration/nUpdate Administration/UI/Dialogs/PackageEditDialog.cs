using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Update;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageEditDialog : BaseDialog
    {
        /// <summary>
        /// The configuration of the package.
        /// </summary>
        internal UpdateConfiguration PackageConfiguration { get; set; }

        public PackageEditDialog()
        {
            InitializeComponent();
        }

        private void PackageEditDialog_Load(object sender, System.EventArgs e)
        {
            var packageVersion = new UpdateVersion(this.PackageConfiguration.Version);
            this.majorNumericUpDown.Value = packageVersion.Major;
            this.majorNumericUpDown.Minimum = packageVersion.Major;
            this.minorNumericUpDown.Value = packageVersion.Minor;
            this.buildNumericUpDown.Value = packageVersion.Build;
            this.revisionNumericUpDown.Value = packageVersion.Revision;
            switch (packageVersion.DevelopmentalStage)
            {
                case DevelopmentalStage.Alpha:
                    this.developmentalStageComboBox.SelectedIndex = 0;
                    break;
                case DevelopmentalStage.Beta:
                    this.developmentalStageComboBox.SelectedIndex = 1;
                    break;
                case DevelopmentalStage.Release:
                    this.developmentalStageComboBox.SelectedIndex = 2;
                    break;
            }

            this.developmentBuildNumericUpDown.Value = (packageVersion.DevelopmentBuild > 0) ? packageVersion.DevelopmentBuild : 1;
            this.developmentBuildNumericUpDown.Enabled = (packageVersion.DevelopmentalStage != DevelopmentalStage.Release);
            this.mustUpdateCheckBox.Checked = this.PackageConfiguration.MustUpdate;
            foreach (UpdatePackage package in this.Project.Packages)
            {
                if (package.Version == packageVersion)
                {
                    this.descriptionTextBox.Text = package.Description;
                }
            }

            switch (this.PackageConfiguration.Architecture)
            {
                case "x86":
                    this.architectureComboBox.SelectedIndex = 0;
                    break;
                case "x64":
                    this.architectureComboBox.SelectedIndex = 1;
                    break;
                case "AnyCPU":
                    this.architectureComboBox.SelectedIndex = 2;
                    break;
            }

            this.changelogTextBox.Text = this.PackageConfiguration.Changelog;
            if (this.PackageConfiguration.UnsupportedVersions != null)
            {
                this.unsupportedVersionsList.Items.AddRange(this.PackageConfiguration.UnsupportedVersions);
            }
        }

        private void categoryTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.categoryTreeView.SelectedNode.Parent == null)
            {
                switch (this.categoryTreeView.SelectedNode.Index)
                {
                    case 0:
                        this.generalPanel.BringToFront();
                        break;
                    case 1:
                        this.changelogPanel.BringToFront();
                        break;
                    case 2:
                        this.availabilityPanel.BringToFront();
                        break;
                    case 3:
                        this.operationsPanel.BringToFront();
                        break;
                }
            }
        }
    }
}
