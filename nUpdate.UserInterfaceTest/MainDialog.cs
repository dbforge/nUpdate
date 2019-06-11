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

            UpdateManager manager = new UpdateManager(new Uri("https://www.nupdate.net/test/updates.json"), "<RSAKeyValue><Modulus>uCL1HpwhnKdP9F3jptfpvnedszCsoVpPv1LPwg8bGouORN2bMHw8JCNZ4Wp5NJ/mVJT5bAs9xlXDSeZgReprrvuLX65oiKxZKX9akC60d+LeVN8+lin/CijXuHZFIj5xRHLQOPUaidYDLN9He3V1fEcZwCe/Ir+4ayd34YpTCTPcSHnLOePHMWqTjsfYDx8gAFmGhP2VLKABbyhM1y9iCkyuVDy7DrvWjxadzL6QAtJ/BV3UFHRbpT17W11Xf1li0WyqO4YolAa34XL3MCBS0STclfqsy+EFArBMr2x54oJ61Kd3pRq1wK9UTCFhIySgtJDVpq7wB24t2Zvc3VeyR8mChObulTBTCSOpeTdnGEyLdHjCIvC6zRRQPZwUvhHr+WP1UbnPe1jw9a0Clv5EthD6cozdMkF0DlRVAHgQr6xuLtAbi+ADwlX98ARkDJl30SO8vkK3hfg6W37YYFPbVWMp20nQei7vwKfRycJbRHnX8injhrdyMkXMSm/ScIV5HEosarKfqcLYY9G6t6VKo0aZeHXiz3VOcXbFe2j0nm6eoSbZwwDpF2S+Jxg/REluXJL2tt/bAFvNeMYRZIHpgvS5kyQM+VVuepBWwdPSqgjPz3c5whN+fSzly7PLpS9JZajg6dE9fsXlEMV9jWPsDeo3GnnfN6gh6U4S7a95YDVgprAyY1DCOeXTxVimIy2l7BlZYirN+XFCU31+wMnvpVdC04hWu5T4XXx9PhCCUOdDzf3MvFUswg8dDBEtZW/EJhyoalQuHFEb7blyKfrEkQ63cVlMIQilvmU+HiRya6mH/gPiL7dDW5MQ84ba2Sue6XFdBHXuTNTD5vxgSDJf+VjQsjESbFmILPUM2KrhxD06g1L0NDfRpQ1ApvT9qwettlsncN1RZMO+ZqGKBAybyz18QdjPEG50WVVQ0nqtfLb0o+ydAmMVekxx6CJzIXtsZbSPsgF01IhvMkYqva4Rk07c1QBEVl+ZDFKqqJR20y1L4k/qAixaG4goracmjTaGqGx8Yj7vdqvcxJB4GOtb1lyx6HRWUyllwLk1FJnyO6Sst67+CNq0cuoTQABglpZov42J5g8vA+He0m+vHGAFU8KsvBvqaVTF+/c/x1rt/XB3qtxWruXBfyIR3LvyT1GN7Ef+grn3Bo9Xhdwf/DPWiC1iE1KjcY/0ZIpSBP29LRo8/SukYg6pPyuNZ/RqqB62nSs1QJgUNmz+P/zsF2ywAPzP/OuNymZ6x0qdazswObs+vqjX7fHm5a1d2lVqhCKsw1zWKR9MNqcCrskmeha0bJSkZIcxEzk1Jrd0NAfSrXJ3GZgTAlRY96rnWzU6/p/cz0K2UEm7IrkLYfbeCAQKFQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new CultureInfo("en"));
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