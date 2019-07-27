// MainDialog.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

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
            UpdateManager manager = new UpdateManager(new Uri("http://localhost/nupdate/updates.json"), "<RSAKeyValue><Modulus>vRt6o2QsPy9P7NnMoa2iOVXcJhVDUCXEbOFOxrAZAcKmKVr/9JEOicpiAiWXUqm1jqH0WAgP0NpQL5p1EGFRbfdqlTnHgflqmg9TzY6Y3eR0rNDtefxKhoPd2gKJDTGXBICbBqALpUwaMyNHCzk9XbuT3HorVd+ZHHi/SaMOpNLhXUKmFpgfkUpj35PG9v4CDM0i3YntgRv6BwF8gNBftbmQxp0riGs+blLYM9IlBHhfWWW6hBDCdPNLlDJ53EM0LQB4BYnH/+PtjcPwiFrZC0gtIEHcGA/k/Es/KBSYGrRsxYfUCarfqdu2pYFyTTRXXdJrap9dtx7a443XGEQ2vXd425QKRJiDViHDP5jAVLDji+4lIsB1iJTmENhTF26Zng305/GqKCKRXyJpjh93MjrUIVG5uPSl+oxA33luH00ISI3ivPN4YBTQkwPjTp7gP6Kh3uj02DCsELZSeyY6tP/IVuc3REFV6mhguSO0MeSRjUAx61i6t1LgPVr+qEn/yCAz3jLlxwLfBfL0RA1vESiD6i6kvidNHiv6Sa/T7Wy4J0PrwiLy3y3jax4pwL2n2uDTVPJWXFdDWJV4Ud8He58KL6ItNn1K7L3n0i5mZaQZjU5EgzObdLEaH/icvxit6TOAJ5FC0Ktq1J4IzzR4JH36Cfn+izoslO5hWIdun2gcQmODB8JNdw2/fXxffwjDCYN6ue/loZsSxGvcFQvtenNs/vY9JGwJU3bYPmvXsHOfn06w0r42IUYmbx/OgoTI9TBbhNCuHWJ8h9vAE+k56HYDz/mbKOlrGdHZvhWBkG96x9wFyM22ppw1VazP01rjCB9UQD3NW+jFfhUWYx5j0vX6I8QEBuzvTXm5HX36OUHPixmgQMdUFUiirVijYWRyAI96zvFTL4SmWY8p8XTErlV4SFMaNfZKGHQnsADwF2MyGPTzZGpZcXtNjDuzQmUYUhnVuJvd+ZVb1gCZkVIiQ5wM+xPWUcLzbBjl6XYL5YXbDvorW+kUiziGkM9FAViEbnYPqVQnNYx6h0heopqGkLLVFA85cMxXJAoLHyMxGEQNh0TqcIVvSQ7y5u28NKlP6rMa+beIuUZp+2Fm+LdjIEsBRQdoE5BgRM9rGqdClGirsqtBcXhpCnxcOJCJqpvkPahInDmkr8Mw/UZ/CbCFEDXy18c8L9owsb8LLp2xxl0P63QDbpVYi+IjhQKshgb/UtYxSQi5hfZp6LljqV5oRZx1FIg8mjxp2XUWNRZKvQrM9KIG6kwXB9lYihmmPNkLMAz1pZgg8VnLeaVlD+DcK6BIH6+QHZFLDIn60TlRQgtf16j4iQ4gtfd2YXtydgISw/WJctaFYPNHuQXTWgjLQQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new CultureInfo("en"));
            manager.RunInstallerAsAdmin = false;
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