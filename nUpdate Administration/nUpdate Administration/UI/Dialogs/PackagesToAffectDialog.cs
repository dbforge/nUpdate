// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 13-08-2014 18:53
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackagesToAffectDialog : BaseDialog
    {
        /// <summary>
        ///     Sets the package versions to show for the selection.
        /// </summary>
        public List<string> PackageVersionLiterals = new List<string>();

        /// <summary>
        ///     Sets the versions that should be affected.
        /// </summary>
        public List<string> PackageVersionsToAffect = new List<string>();

        public PackagesToAffectDialog()
        {
            InitializeComponent();
        }

        private void PackagesToAffectDialog_Load(object sender, EventArgs e)
        {
            foreach (string packageVersionLiteral in PackageVersionLiterals)
            {
                checkedListBox1.Items.Add(packageVersionLiteral);
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            foreach (object item in checkedListBox1.CheckedItems)
            {
                PackageVersionsToAffect.Add(item.ToString());
            }

            DialogResult = DialogResult.OK;
        }
    }
}