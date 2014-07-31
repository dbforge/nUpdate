using nUpdate.Internal;
using System;
using System.Windows.Forms;

namespace nUpdate_Tester
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateManager manager = new UpdateManager(new Uri("http://www.nupdate.net/administration/updates.json"), "<RSAKeyValue><Modulus>w57rnIk2AgPHUSoaSHNd6mdv58R8E7TOyCSnI6xFdLZCXvV3O7KNDg6I4ee26VQZvbwaMb63LZwpuI6MtSJ92XxFcCVmzImwF6bYUFCFk46//eS6YpxDkEwlKD6e0oiNu8tgfE8T3zeVqSGRM4tcyQqpZY13xBnqW+KECTstCdVP/F3T+1HLWFa1gZtvl3jtVzQlbbHEc3bHnHiatUHDwa/fORyR0DXWcBEg1h7oacFoc+lVtYg7muNIpsUTXpAKV8PpAHRUssMVXBIePF8Jp4uCqf+rpXNWA1ujsleZYmoN37/CBVtjAlNSMA1sHmA2hm1TZ3pNAt2+p+VuASXR1xlzp8BmOlPKR80Ltq+dZv5UfetrQw4kFBQc52lEeimsSIH/kCtRqBUIviVYJ2/VMY8Rs8H04T174SI415NzQBkFxRTC6LzjbCOrIOwE4yWk9hlIaqL/yhhKa0g98Y0g109txTIiX6MH6rN0r8fO1+A8AsiRUBHi/YXVQZ3Ud7rNCkkgQOCPAxqk/V0X14caqVx17aHt/HCiKDgTqLIGcBPEF7BTcpbOurfhwC2uJiKi1+8agf+8C+bSIwdQ5HQQQNW0fdQieYkzx00qgeJnYTe1e34/65DfhbPpHtX+/+ZImuNHofz8jZzBhZ0Jox+/I26bOBxWdgkw5MLhhemG2Ac=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new UpdateVersion("1.0.0.0"));
            manager.IncludeAlpha = checkBox1.Checked; 
            manager.IncludeBeta = checkBox2.Checked;
            //manager.UseHiddenSearch = true;
            //manager.Language = nUpdate.Core.Language.Language.English;
            //manager.LanguageCulture = nUpdate.Core.Language.Language.English;

            var updaterUI = new UpdaterUI(manager);
            updaterUI.ShowUserInterface();

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
            label1.Text = new UpdateVersion("1.5.888.9a1").ToString();
        }
    }
}
