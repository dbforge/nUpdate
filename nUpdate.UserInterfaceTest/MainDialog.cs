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

            var manager = new UpdateManager(new Uri("http://localhost/nupdate/updates.json"), "<RSAKeyValue><Modulus>sK3GI4HI8Qd+33LTWXdhSLs/DMX+GZw/Onsh7J71rPUR7UHabuzr03LaUvzQhZfLMvi8V+RKXYz3/6s/ILc0c44IUJ1e9j2Hwq4sgsguQmnOKEcJzOyqA/0lNuf7/2UeyM/478G5UksoFeop4tirMHF5zqQnlxSjcWbCIvzFpkqYKI9vTjgirtFwnGknbXcArN9a5FXDtdZXNUwSO0NP7SIifIKv8Rzwq4d4PnAqhlawafHpJCLyUOd2erUvchUD3h+Q7yUZUWd8/UN5M81wZqA/CthJx2yM2JuclpQgc+5fv7uvcGe0tEep1Fa0e71cXt8CtLNLlWv2lttATE4VxIOzhbiUfLz+BCQBE/dUmBFn3crSLmJHnaqOqIWFx6y5Q5wJReHSKPqRtskuV0Wqy0CwHglaCy1lY8fPJSITZbfXGLGaKstUZN53YWC4QDfQIA5wj6c+TOMvF1yI0IuwV/4M7MtO2veFTXPxHUcAmJMcubwDJnAeyH7bkxLPMTFzBSGGOMdDFs0vvs7z0ZXwn6NCdmgVlgDdD32wd704IiTuskjBKB2HwZxp8ppI7kbCmGuOQTdc5R6qW5IKcvEeEAyM7o/7qvbRXtWrVbfiD9uPE2n3Pt9DIYZbtAHObxs+7XU67yMYWn7KyAL3BE+/5g7V/PkLsWD4jynv23MZnxfQ0Lw70Dw2zZjiLK10pzXRVCJNC5DoZGyocF/JaAqyAxOYoVqleFHvV6G9JmawxtY9w2L++c3NkOVhpks8aiJkjiZ/KR+GHSXHHKFrtg80eyn0Nqmycpq40jEQWqoo3z5dLifFFCjwPG8R6gF7ZK1Kw4CB/sFp7qps5C84F6HRk22bG+YXmUhDOVE9Npagq7reL5dbsoiuoQPEbzuVc2ADI/6RzNLtlnE/RLD2Jl7QW0qc5sVJrdS45x5bQRnQFz6FgSjuZ6hMxOjRIrJHB9RIZoiFqaqZzWOKANvkpI6dlb/Vv01hc1ETsjdsqUFvqZ0ltlMs9BxP0Hl6tiQs15Q0u6HPOiJ50wnXInLQPlKX1kYas1rsChV3tNwzOl75+LNReWwj2HZgBV97dWvk6i5wZcnZJL4+PKf4r9a3Csqi1cygM4U6kESrxAjH4dDwTXfhFsZ0pwbxAfeatcLw9MWFdUc3ZEi2FbxbSOOUFD2HyIQrFR3aBQ9WmXJAmzNW96t47KGVoepPRlAQqwD3TJWkZVfh0bWaZtG9RsifNDz/BUabfqbzPvwEkQI9ShhPApip3lx8ryKgulLpAlVGYWmvtnfSxba1Ah0f8iSEoJIhfdFeahvRoNpxvsk9Bt/Y1TL08LulkYh17xusR4zR3T4i8j8qOrnm4bMTDGrORLQ+OQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new CultureInfo("en"));
            manager.UseDynamicUpdateUri = true;
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