// Copyright © Dominic Beger 2018

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Internal.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
{
    public partial class ServiceStartOperationPanel : UserControl, IOperationPanel
    {
        public ServiceStartOperationPanel()
        {
            InitializeComponent();
        }

        public IEnumerable<string> Arguments
        {
            get
            {
                return string.IsNullOrWhiteSpace(argumentTextBox.Text)
                    ? Enumerable.Empty<string>()
                    : argumentTextBox.Text.Split(',').Select(t => t.Trim());
            }
            set { argumentTextBox.Text = string.Join(",", value.Select(t => t.Trim())); }
        }

        public string ServiceName
        {
            get => serviceNameTextBox.Text;
            set => serviceNameTextBox.Text = value;
        }

        public bool IsValid
            =>
                !string.IsNullOrEmpty(serviceNameTextBox.Text) && (string.IsNullOrWhiteSpace(argumentTextBox.Text) ||
                                                                   !argumentTextBox.Text.Trim()
                                                                       .Split(',')
                                                                       .Contains(string.Empty));

        public Operation Operation
            =>
                new Operation(OperationArea.Services, OperationMethod.Start, ServiceName,
                    Arguments);
    }
}