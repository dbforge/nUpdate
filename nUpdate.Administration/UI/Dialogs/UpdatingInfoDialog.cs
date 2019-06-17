// UpdatingInfoDialog.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

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