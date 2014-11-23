using System;
using System.Globalization;
using System.Windows.Forms;
using nUpdate.Internal;

namespace nUpdate.Tester
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var manager = new UpdateManager(new Uri("http://www.nupdate.net/updates.json"),
                "<RSAKeyValue><Modulus>0LW19YTHmpuWglyrvmgOKH3xmdKJU27mqIRtgtdfDcl1hVLenOgk14CH7qOUehUCBMaXdvLVdeRfVbb0f/J/4lK0PKPMp5s2SlGl6xxIE0u4/bw7r2lzyHmZjJ8Log2WMNWr3IRf+fok026omUKVp0AeoMoNH668vTGDYSMNsvxw61KEd0Nv6kaZo3pOWKo70BtEnjdP9FobjOqSxWdLBcBCWvJO1TSe1NQeVDzRNepoUXoL5/Olcw/pyQvrJg4F1uI+4mSuk/6lyY6YdvKdRWeqKkLfNGo8Z3Mo9nA5hFPwEXeWp2fO4RlS2hcw87Lx41pjdKD0tovyf8lhaYzLzMg/nmKrgflIcHoAJKC49/x90XJWgM252JdSmAQRVuoUWRZBcvUbaMVc1qMROwn269N+UmJfukq0COQAjvc3iIRAr2W8S9HV/qunKFNoETBPjyJDXKDaJfQzgISJB1qjZ422nwuc+qWFUhmHvz+I4NnX/7uSvPChawC4RWB5zQHyN4AGLhB90QKjRq1x9U6PupEJdbYtRuwGygBvsKRlE7QNs3jgFcqBcM8jzs4WAa4y+Iy8ynAmPuDz5imbKhzgrQutMHfcUElCix2YBaInaNTwFYG35aurUMOS+zpX6k5CCFbyUNhQ5XFHWH45/5X/uFfQ0SUlaRynxaQcf9eaS/HKXq2+CpIanZiOfhBnwB9KYlXdJ/82EkmWV5NgdN5biluGGKZEig/Xz0rr9BPeDygfAoEYh5HyGXc44xh4zFzHf/8KCOp5SA1xfj9or0RE/9zv3kDm7g0RQ1fECRRxX/MKfV9Cc7plQ1cqgEr1u0nUrtmgM4JSCyp/TW9n1c4bX0N+wdiBbVr0ohmQxF9k7w5Z/BjU/LHzAEWBS4UqMkMvb3iVTuwi18vc82ot7rVBxC6lIM1TqDCBDGoFIeNpSwwETwqaa2GK9z0TpO296wcCXpq9pPy0GyASC8l13qb07FeAdb6SzEO+uY25UmhWY+tXFeIsWc+ly3iIuznAwlCLK42QvSfGPnXBrBP2Qvz5yjC+vjj7gLN9HEUhB/IjVxU2EPp3yHimWWGBZQosGE0KKWc9dDBFr7aIomqK0lNoXavAcig73FT0D6WV520UDDRYddADDe6Rp7+AgNoMH/+avkMJXxl0LEQQ37PrIaXYSlEsfT9IDXyDWJSypGQco0hH810XF07bjknKl05LI/DQd1F9USLCWAvkkPGcZXIQKUULS6ViuLl3zwH+Y1E8DVePmPwz26gj7+K0tO9gywN7oLIVoEHTaxhTK+zQwzElLu+zNHQiP/1Y+85AGKUfVVHsGXNruELoO49RGBvOLmDaPcuwzfmIaDgJ+aBWxBCCKw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                new UpdateVersion("0.1.0.0"), new CultureInfo("ru"))
            {
                IncludeAlpha = checkBox1.Checked,
                IncludeBeta = checkBox2.Checked,
            };
            //manager.UseHiddenSearch = true;

            var updaterUi = new UpdaterUI(manager);
            updaterUi.ShowUserInterface();

            //    var dialog = new SearchForm();
            //    manager.UpdateSearchFinished += new UpdateManager.UpdateSearchFinishedEventHandler(dialog.Finished);
            //    manager.CheckForUpdatesAsync();
            //    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //        dialog.Close();
            //    MessageBox.Show(manager.UpdateVersion.ToString());
            //}
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            label1.Text = new UpdateVersion("1.5.888.0").ToString();
        }
    }
}