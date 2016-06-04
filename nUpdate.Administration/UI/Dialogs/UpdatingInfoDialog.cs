// Author: Dominic Beger (Trade/ProgTrade)

using System;

namespace nUpdate.Administration.UI.Dialogs
{
    internal partial class UpdatingInfoDialog : BaseDialog
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