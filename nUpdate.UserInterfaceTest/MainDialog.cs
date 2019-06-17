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

            UpdateManager manager = new UpdateManager(new Uri("http://localhost/test/updates.json"),
                "<RSAKeyValue><Modulus>x41wUnWNlTcRPi2xE+ZrIY2h/xReeBtWtXEHrlhD/SgzLHTqVZPXikW/dCXrTXUXCS2ur81OFW01JnNcvLmWBVv92jmC9028uYr4yMYXcxQRpARbZJ3MFQ4s3XZw5nVLQFyhi95SqKIMOu2bOHQLUD0IlJ4EZYCAu86Mx6c36Xu9UAWbn7yWeJk1ZjgD+DrO3ar9NqaM0zWT8lbiR6mQwA55LCkolFq7HNbVXQIIgjd3VmRKcI0eOl7Lc901lllWTnzWtE7uXVo9HY5JF+3eQ5MVrrytBoPH2ZbTYPurRV9zwSQwW2ouAFvMPJQxrE4X0kY5sdTUq5P6Joe2GHXqjr2VIAMoUtJDIbFb7eaX9BOZJo+BsGy0iF5L0O4hmXXl2dlwDt3QnctHYYnIrqOHEGSYWzUeFO63shh691v+JvaBhKltzi1kUEqJ7nfvYySxjLt/DyBtyCCbX3QB/bC8rt2N7fpfbE/E6yo2SyG9VMwDWhViSKLZrScgsFC/Sn67EY07RvnMkD2HXHaiYnZdu14X2HmoqtmQt0QP1H2O/B7S/rU8NY6yS9rh/X5gdQKPTI3eBtdW5Ip5fJJzygtA5JmsgIaLxGbO8iPk5dr11es1yxK+QmrNFScbK7fBQYYgRgNMc+OgnsYQSwb0ZM2FoHo5k56hrexcF9itPksyM7jH2py+kCIgc34YsfylJQrdCCbg7lO6BQf/mUHke/PF4N8+NGnumJa3JudiQy6lyLJgili65hBY32CN4oAsg/8UwO/cKyvn6OKHvDTFwXB4OvPyj/arzNIcSMkBWnfBLVVs+kM6KlGzD4y6nRQUZyncz49jy8rBapI1rfS3wkTBdGcmU69WB6xIDhaPiKzH9zmqmDOBQLsU4ZwrZ9D49qFJ8GPTZPF7CISSMrK/oqL4cSIxb2vv2GfF8YC2n23XczjBBTorlOoBzMFedmT7jNP6HL2BTVcjkXVF18mKZExzvI+vzwHhCN9u/1/5H2N4i5gd2qUwz+fYDQAfsYQr4pjbgBmnuIUhVRVVHWvTiqYv3irTQWqvVdWQfyuV99QgHS1lp9F+Y3sZmUQMnhUCdQ38kAlY0OR+sQaZQ11H6fCroqwze+S0HKpoFxKo2nELb/DKWdePkTXEoe6xlbJ3tsK8c2Ut+d2Lb25/iYrTlAe2gsBAypU59NAhhHnBOJYGK1fiRmLit7LkzxOnrw5mlA5gyEj9yTDAWIxwmaGH2hC5vFThb4vJ4Xdo5a2yVtpzY25vNbY5RrZ7GvWZa6K82Oayv7i+u4JjARxmN/dseKqylp1RooiMjyFCut14g4qQk4QAJrZvLDnMEc5SSwIFKsdnK7t246X4PniWy7U6mVCLvQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                new CultureInfo("en"));
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