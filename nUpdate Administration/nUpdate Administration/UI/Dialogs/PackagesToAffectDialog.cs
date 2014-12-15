// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 13-08-2014 18:53
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using nUpdate.Administration.Core.Update;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackagesToAffectDialog : BaseDialog
    {
        /// <summary>
        ///     Sets the package versions to show for the selection.
        /// </summary>
        public List<UpdateVersion> AvailablePackageVersions = new List<UpdateVersion>();

        /// <summary>
        ///     Sets the versions that should be affected.
        /// </summary>
        public List<UpdateVersion> PackageVersionsToAffect = new List<UpdateVersion>();

        public PackagesToAffectDialog()
        {
            InitializeComponent();
        }

        private void PackagesToAffectDialog_Load(object sender, EventArgs e)
        {
            foreach (var packageVersion in AvailablePackageVersions)
            {
                checkedListBox1.Items.Add(packageVersion);
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            foreach (var item in checkedListBox1.CheckedItems)
            {
                PackageVersionsToAffect.Add(new UpdateVersion(item.ToString()));
            }

            DialogResult = DialogResult.OK;
        }
    }
}