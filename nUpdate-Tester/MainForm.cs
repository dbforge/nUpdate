// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using nUpdate.UpdateEventArgs;
using nUpdate.Updating;

namespace nUpdate.Tester
{
    public partial class MainForm : Form
    {
        private bool _updatesFound;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Handles the Click event of the button1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            var manager = new UpdateManager(new Uri("http://www.nupdate.net/test/updates.json"),
                "<RSAKeyValue><Modulus>obsQS8jN1bp2eaVlhH6JkWG/CdGNWYrQKJrz3+cOnVGxT9tGkP5Bw5lJQE0hR+k1Fq7NR69z7ExznRzU0gEz+Zz/iPnRZg6mowaHR6li7RBIz70V8ju2fk5IEYljSeIC+jbQcSfrZ6j+IdYy0EswkgbDvz2V2fIYnMzlZJ19hwPrIuPDUr7UlGpy6GkLVwMZI24NKlNcduHW/YIYgV/c8OSx8034ongIg+4ikxuxwTPpz7+QmSmIfvH4qb36GMJiUyykPECT1EQs1bGdvv38Uf7lbttX78L7Su1l0lXBZSs/zC8n/8vkCR9h+4VuBHtgKXVlMjno9mNtOda4F1dYd1MlGGzEW8yPqmMFxK0ureShngUF1+T5iUu/Cl7PYwXiQ8qgwClS4cVH2wlZ24afH7raKC7Nl2vfXAKxefJJAnjzb5UbBBCx8Svuuww4gfkMnAgKJQjINBxGWGIix4ng/bo9Ucfgb1LWrmnksHo/4Xcfbx/oGeGH0Eco5vG8MLPhpKprSYAULFC20W4gM+CIT8uf9k4avRJfLpbsj68H3GBGaPNAOYz9kh0FUwMda5fmICcBNhw/BAzWvpe/F3GuQkOriMqA2InlrGu48A2w9hKMiank6kifYVI+GLlL0m5RrqZFLkj+fbkr9MFN1Mp+aHjP81h9wUFYUMsHw1d4fefU66kapQg6OWK7IhqU5kwMfQfbsJ+UURmWsCwrdMTeG99a2j6A1MJ881HyFHNc/o5UmFUbPMRBILVVq7Ij6uC0ML78ea7OhfGb9nJ/A+1rvoiio/kgMPj8NXvJ3ZtW130J+DCZdl0K+IC4JVxzvVNBLsw4z/DIq0ni8JvlO1HMEvCW5lR+m9pbf+qgKXnmjpar3TcPLLx7B/X3jrpaF+8hX2xIWvq/ANzyUiZkdRVNSuk1BFz65BRKZhgVZkp4H7Ojs+lyXdoou7cFhIIYQeR51iWUeAlX4IsHPEoTINJegs8SpfD/Iz2CZWTWo2CtyUU1/GN7GkOnjDJRKGa70aIWcSh6A3CwXpH9iu9+AijUqxByD/gGCIX1Tl1VoCY/lA+iZ6tiwBHQeCfl5oROB+t8nKwwqWdvdTaFOdjXplBuo2aUYJwQsGGTz6cjaqxMb63VS1NAXmSFkINapVGDla6LGbjsH+qnRM7+lvJRyalf+TIrouC77e7PYxwhhFfZlvonqKTQjIQbkezEoD/68WWzN5C+5z9hMR8n7fnAO8NyBEZ3uBfJUNFKG/yI9GN7XfMOaaNSLRcpvIH58gCKbwWR9UwQYVWREbC8RHgjq1hvIIyd3QVmjSA5DUbFDTBFVoBLtPFPWJxX9/uiS3t+MfLebnkXmWUoZT/aq2RcTVc+gQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                new CultureInfo(comboBox1.GetItemText(comboBox1.SelectedItem)))
            {
                IncludeAlpha = checkBox1.Checked,
                IncludeBeta = checkBox2.Checked,
                //CloseHostApplication = false,
                //Arguments = new List<UpdateArgument>() { new UpdateArgument("suceeded", UpdateArgumentExecutionOptions.OnlyOnSucceeded), new UpdateArgument("failed", UpdateArgumentExecutionOptions.OnlyOnFaulted)}
            };

            var updaterUi = new UpdaterUI(manager, SynchronizationContext.Current, false);
            updaterUi.ShowUserInterface();

            //var dialog = new SearchForm();
            //manager.UpdateSearchFinished += dialog.Finished;
            //manager.UpdateSearchFinished += SearchFinished;
            //manager.UpdateSearchFailed += dialog.Fail;
            //manager.SearchForUpdatesAsync();

            //if (dialog.ShowDialog() == DialogResult.OK)
            //    dialog.Close();

            //if (!_updatesFound)
            //    return;

            //MessageBox.Show("Update gefunden: " +
            //                manager.PackageConfigurations.First(item => new UpdateVersion(item.LiteralVersion) == UpdateVersion.GetHighestUpdateVersion(manager.PackageConfigurations.Select(x => new UpdateVersion(x.LiteralVersion)))).LiteralVersion);
            //var downloadDialog = new DownloadDialog();
            //manager.PackagesDownloadProgressChanged += downloadDialog.ProgressChanged;
            //manager.PackagesDownloadFinished += downloadDialog.Finish;
            //manager.DownloadPackagesAsync();

            //if (downloadDialog.ShowDialog() == DialogResult.OK)
            //{
            //    downloadDialog.Close();
            //}

            //manager.InstallPackage();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            label1.Text = "1.0.0.0";
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //_updateConfigurationUri = comboBox2.SelectedIndex == 0 ? "http://www.nupdate.net/updates/updates.json" : "http://www.nupdate.net/test/updates.json";
        }

        #region "EAP"

        public void SearchFinished(object sender, UpdateSearchFinishedEventArgs e)
        {
            _updatesFound = e.UpdatesAvailable;
        }

        #endregion
    }
}