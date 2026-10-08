using Avalonia;
using Avalonia.Controls;
using nUpdate.Administration.ViewModels;

namespace nUpdate.Administration.Views.Controls;

/// <summary>The step list on the left of a wizard window.</summary>
public partial class WizardRail : UserControl
{
    public static readonly StyledProperty<IEnumerable<WizardStep>?> StepsProperty = AvaloniaProperty.Register<WizardRail, IEnumerable<WizardStep>?>(nameof(Steps));

    public WizardRail()
    {
        InitializeComponent();
    }

    public IEnumerable<WizardStep>? Steps
    {
        get => GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
    }
}
