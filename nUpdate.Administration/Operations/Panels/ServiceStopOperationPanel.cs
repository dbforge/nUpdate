// ServiceStopOperationPanel.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.Windows.Forms;
using nUpdate.Operations;

namespace nUpdate.Administration.Operations.Panels
{
    public partial class ServiceStopOperationPanel : UserControl, IOperationPanel
    {
        public ServiceStopOperationPanel()
        {
            InitializeComponent();
        }

        public string ServiceName
        {
            get => serviceNameTextBox.Text;
            set => serviceNameTextBox.Text = value;
        }

        public bool IsValid => !string.IsNullOrEmpty(serviceNameTextBox.Text);
        public Operation Operation => new Operation(OperationArea.Services, OperationMethod.Stop, ServiceName);
    }
}