using System.Media;

namespace nUpdate.UI.WindowsForms.Popups;

internal sealed partial class PopupDialog : Form
{
    public PopupDialog()
    {
        InitializeComponent();
    }

    public PopupButtons Buttons { get; set; } = PopupButtons.Ok;

    /// <summary>The exception whose full text can be copied from the context menu.</summary>
    public Exception? Exception { get; set; }

    public string InfoMessage { get; set; } = string.Empty;

    public Icon PopupIcon { get; set; } = SystemIcons.Information;

    public string Title { get; set; } = string.Empty;

    private void closeButton_Click(object sender, EventArgs e) => Close();

    private void copyEntireMessageToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (Exception is not null)
            Clipboard.SetText(Exception.ToString());
    }

    private void noButton_Click(object sender, EventArgs e) => DialogResult = DialogResult.No;

    private void yesButton_Click(object sender, EventArgs e) => DialogResult = DialogResult.Yes;

    private void PopupDialog_Shown(object sender, EventArgs e)
    {
        iconPictureBox.Image = PopupIcon.ToBitmap();
        headerLabel.Text = Title;
        messageLabel.Text = InfoMessage;

        if (headerLabel.Height > 20)
            headerLabel.Location = new Point(headerLabel.Location.X, headerLabel.Location.Y - 7);

        if (messageLabel.Height > 41)
        {
            var difference = messageLabel.Height - 41;
            messageLabel.Height += difference;
            Height += difference;
            controlPanel1.Location = new Point(controlPanel1.Location.X, controlPanel1.Location.Y + difference);
        }

        if (Buttons == PopupButtons.Ok)
        {
            closeButton.Visible = true;
            AcceptButton = closeButton;
        }
        else
        {
            noButton.Visible = true;
            yesButton.Visible = true;
            AcceptButton = noButton;
        }

        contextMenu.Enabled = Exception is not null;

        if (ReferenceEquals(PopupIcon, SystemIcons.Error))
            SystemSounds.Hand.Play();
        else if (ReferenceEquals(PopupIcon, SystemIcons.Warning))
            SystemSounds.Exclamation.Play();
        else if (ReferenceEquals(PopupIcon, SystemIcons.Question))
            SystemSounds.Question.Play();
    }
}
