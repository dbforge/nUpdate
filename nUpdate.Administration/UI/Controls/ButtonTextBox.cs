// Copyright © Dominic Beger 2018

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.Core.Win32;

namespace nUpdate.Administration.UI.Controls
{
    public class ButtonTextBox : CueTextBox
    {
        private Button _button;
        public string ButtonText { get; set; }
        public event EventHandler<EventArgs> ButtonClicked;

        private void ButtonClickedHandler(object sender, EventArgs e)
        {
            OnButtonClicked();
        }

        public void Initialize()
        {
            _button = new Button {Size = new Size(25, ClientSize.Height + 2)};
            _button.Click += ButtonClickedHandler;
            _button.Location = new Point(ClientSize.Width - _button.Width, -1);
            _button.Cursor = Cursors.Default;
            _button.FlatStyle = FlatStyle.System;
            _button.Text = ButtonText;
            Controls.Add(_button);
            NativeMethods.SendMessage(Handle, 0xd3, (IntPtr) 2, (IntPtr) (_button.Width << 16));
        }

        protected virtual void OnButtonClicked()
        {
            if (ButtonClicked != null)
                ButtonClicked(this, EventArgs.Empty);
        }
    }
}