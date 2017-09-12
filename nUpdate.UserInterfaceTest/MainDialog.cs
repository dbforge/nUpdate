using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Updating;

namespace nUpdate.UserInterfaceTest
{
    public partial class MainDialog : Form
    {
        private readonly UpdaterUI _updaterUI;

        public MainDialog()
        {
            InitializeComponent();
            var manager = new UpdateManager(new Uri("https://www.nupdate.net/test/updates.json"),
                "<RSAKeyValue><Modulus>xhHXp+QCvWb8+W6TR/hkkhEy9h9WwdAMBMDJbsfn3ObF6U5K2KQCKQ6Alr4iXqlwvcYHKt2CG2M4m9So+rA/N3gg/AETbd74MYXaMRtDqLjkTQuG8EI9JqnlKAR3JS4zWkZghtPsYHtzox7J7Z94iDLAP899r+n9yeafYWwXOsRiTOxKhO9oHHQAK5JlRe9yHMM9F+WanEOGTqfI5FwvldH1Wnahs4xJMhDSB9m92D2zu9nruCmA+7l12hoQOIpsF4hKT6cm4Hm+TcZURForopS4I/lU9W+FDv+GMBrf46+tX2QfmBN9x/NUciyYJvFEAVA1JUXhYT19v+2tW1paASlHhzDGrEFpruWK/NNXPNOlEfdjpZRrvdsRGRYwt5bwt8H7+9n8MAI9ERXO627MIm9wrhvsCuZDDStJNMK05AbOfK/4KwVUUxCn0LO7/aWkhEkljz8fz+cS3lA2yX0Scl8r/z1iRyDjLQJ08bnZJrc3LsVOwQVef1/86IpBBOSkco4g6iaZzgfFSKHNpJvah5Gpa5rt9dq9omcNTANGh5KMNjD7zLM0sXQd3Te1vj5bsWXdBrqFdtWJt627/NTUIEvfKmI4V+hvc3VGnXmBIAPUyodC2pxoDrqWElIXMMgaYl2ifLVQdP7Dw6Z9lZ0CSjW5//Juq/vTANMPBjf6LazWxYto+hTQGjBUYYCaGQKNk3lHkFCBIIMvpGANPjWuLUn0Vbh2PI5KkPElHbJ0CAYRHRG2brNP8Zw95NhaZYMoec3cMLokeU31M1TgkCHeqZspnYlKcGxgQj8xCCPmSwriB1jjGJRBrhC2CK7izrn8Dq7uu0Epg2M8nKeYzhFuiBUashVB/YFUmdA9LALUihv4XN0yXQ4C8UJvsGo+vtpdfWx51smMZxSLcjgfDgnNPR2ZKlJbG7bEowUduZWGurGhRNREC+9E1BZCRylI6c0B+7pgeiOexDzWgQlNnslQWtQs7M1ctsic+8EtOYXd//a4PO1bBhpL9u+FPB+Ek8kSSaGhDUsv0ZkNFdyqKVeP+EmGY5UaQ+b0lCdYmoQjp07Yd3lEb+T7vT/AntBSDcvUITSeP4imgYE7bw5Qo3nWWv3dHz4ubGxrumqQD1I/x7X/QU9SMVfMtcswCLx8inJICaw8jJ5E/mH5lFWY8IOjI5211CkjepKc2wtoeSYxFSVw+896vG3n0dlqerddaBr6kBRSOz5gXS4cucYPXZwiHHaFOqRBdVA84yKn9YNMiVSPdbSIYZk3GEjvQTz9dizOVdQKV0c+HyjrXfXsnwOhMFN8x1iBOD0oeUSMjiV2WidETK117Wnz36lFkUmf7TwOVhaHCI0YC6w9zm9rrcpPWQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                new CultureInfo("de-DE"), new UpdateVersion("0.1"));
            // manager.HttpAuthenticationCredentials = new NetworkCredential("trade", "test123");
            _updaterUI = new UpdaterUI(manager, SynchronizationContext.Current);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            _updaterUI.ShowUserInterface();
        }

        private void hiddenSearchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _updaterUI.UseHiddenSearch = true;
        }
    }
}