using System;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Update;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageEditDialog : BaseDialog
    {
        public PackageEditDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The configuration of the package.
        /// </summary>
        internal UpdateConfiguration PackageConfiguration { get; set; }

        private void PackageEditDialog_Load(object sender, EventArgs e)
        {
            var packageVersion = new UpdateVersion(PackageConfiguration.Version);
            majorNumericUpDown.Value = packageVersion.Major;
            majorNumericUpDown.Minimum = packageVersion.Major;
            minorNumericUpDown.Value = packageVersion.Minor;
            buildNumericUpDown.Value = packageVersion.Build;
            revisionNumericUpDown.Value = packageVersion.Revision;
            switch (packageVersion.DevelopmentalStage)
            {
                case DevelopmentalStage.Alpha:
                    developmentalStageComboBox.SelectedIndex = 0;
                    break;
                case DevelopmentalStage.Beta:
                    developmentalStageComboBox.SelectedIndex = 1;
                    break;
                case DevelopmentalStage.Release:
                    developmentalStageComboBox.SelectedIndex = 2;
                    break;
            }

            developmentBuildNumericUpDown.Value = (packageVersion.DevelopmentBuild > 0)
                ? packageVersion.DevelopmentBuild
                : 1;
            developmentBuildNumericUpDown.Enabled = (packageVersion.DevelopmentalStage != DevelopmentalStage.Release);
            mustUpdateCheckBox.Checked = PackageConfiguration.MustUpdate;
            foreach (UpdatePackage package in Project.Packages)
            {
                if (package.Version == packageVersion)
                    descriptionTextBox.Text = package.Description;
            }

            switch (PackageConfiguration.Architecture)
            {
                case "x86":
                    architectureComboBox.SelectedIndex = 0;
                    break;
                case "x64":
                    architectureComboBox.SelectedIndex = 1;
                    break;
                case "AnyCPU":
                    architectureComboBox.SelectedIndex = 2;
                    break;
            }

            changelogTextBox.Text = PackageConfiguration.Changelog;
            if (PackageConfiguration.UnsupportedVersions != null)
                unsupportedVersionsList.Items.AddRange(PackageConfiguration.UnsupportedVersions);
        }

        private void categoryTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (categoryTreeView.SelectedNode.Parent == null)
            {
                switch (categoryTreeView.SelectedNode.Index)
                {
                    case 0:
                        generalPanel.BringToFront();
                        break;
                    case 1:
                        changelogPanel.BringToFront();
                        break;
                    case 2:
                        availabilityPanel.BringToFront();
                        break;
                    case 3:
                        operationsPanel.BringToFront();
                        break;
                }
            }
        }
    }
}