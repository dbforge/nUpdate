// Copyright © Dominic Beger 2018

using System;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class UpdatingInfoDialog : BaseDialog
    {
        public UpdatingInfoDialog()
        {
            InitializeComponent();
        }

        private void UpdatingInfoDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Program.VersionString);
        }
    }
}