using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.Win32;

namespace nUpdate.Administration.UserInterface.Controls
{
    public class ButtonTextBox : CueTextBox
    {
        private Button _button;
        private string _text;

        public string ButtonText
        {
            get { return _text; }
            set { _text = value; }
        }

        public event EventHandler<EventArgs> ButtonClicked;

        protected virtual void OnButtonClicked()
        {
            ButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        public void Initialize()
        {
            _button = new Button {Size = new Size(25, ClientSize.Height + 2)};
            _button.Click += ButtonClickedHandler;
            _button.Location = new Point(ClientSize.Width - _button.Width, -1);
            _button.Cursor = Cursors.Default;
            _button.FlatStyle = FlatStyle.System;
            _button.Text = _text;
            Controls.Add(_button);
            NativeMethods.SendMessage(Handle, 0xd3, (IntPtr) 2, (IntPtr) (_button.Width << 16));
        }

        private void ButtonClickedHandler(object sender, EventArgs e)
        {
            OnButtonClicked();
        }
    }
}