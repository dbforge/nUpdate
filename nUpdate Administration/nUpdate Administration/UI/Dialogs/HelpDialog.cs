// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class HelpDialog : BaseDialog
    {
        private bool _allowCancel;

        public HelpDialog()
        {
            InitializeComponent();
        }

        private void HelpDialog_Load(object sender, EventArgs e)
        {
            if (!ConnectionChecker.IsConnectionAvailable())
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No network connection available.",
                    "The entries couldn't be loaded as no network connection is available.", PopupButtons.Ok);
                Close();
                return;
            }

            using (var helpClient = new WebClientWrapper())
            {
                helpClient.DownloadStringCompleted += (o, args) =>
                {
                    if (args.Error != null)
                    {
                        BeginInvoke(
                            new Action(
                                () =>
                                {
                                    Popup.ShowPopup(this, SystemIcons.Error,
                                        "Error while downloading the help data.",
                                        args.Error.InnerException ?? args.Error, PopupButtons.Ok);
                                    _allowCancel = true;
                                    Close();
                                }));
                    }
                    else
                    {
                        var helpData = args.Result;
                        if (String.IsNullOrEmpty(helpData))
                            BeginInvoke(new Action(() => informationLabel.Text = "No entries available."));
                        else
                        {
                            HelpContent helpContent = null;
                            try
                            {
                                helpContent = Serializer.Deserialize<HelpContent>(helpData);
                            }
                            catch (Exception ex)
                            {
                                BeginInvoke(
                                    new Action(
                                        () =>
                                        {
                                            Popup.ShowPopup(this, SystemIcons.Error,
                                                "Error while loading the help data.",
                                                ex, PopupButtons.Ok);
                                            _allowCancel = true;
                                            Close();
                                        }));
                            }

                            if (helpContent != null)
                                foreach (
                                    var actionListItem in
                                        helpContent.HelpEntries.Select(contentPart => new ActionListItem
                                        {
                                            HeaderText = contentPart.Question,
                                            ItemText = contentPart.Answer,
                                            ItemImage = Resources.Question
                                        }))
                                {
                                    var item = actionListItem;
                                    BeginInvoke(new Action(() => helpEntryActionList.Items.Add(item)));
                                }

                            BeginInvoke(new Action(() => informationLabel.Visible = false));
                        }

                        _allowCancel = true;
                    }
                };

                helpClient.DownloadStringAsync(new Uri("http://www.nupdate.net/help.json"));
            }
        }

        private void HelpDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }
    }
}