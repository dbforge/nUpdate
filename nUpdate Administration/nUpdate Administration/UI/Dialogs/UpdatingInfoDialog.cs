// Author: Dominic Beger (Trade/ProgTrade)

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
            Text = String.Format(Text, Program.VersionString);
        }
    }
}