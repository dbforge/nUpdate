using CommunityToolkit.Mvvm.ComponentModel;

namespace nUpdate.Administration.ViewModels;

/// <summary>One entry of a wizard's step rail.</summary>
public sealed partial class WizardStep : ObservableObject
{
    [ObservableProperty]
    private bool _isCurrent;

    [ObservableProperty]
    private bool _isDone;

    public WizardStep(int number, string title)
    {
        Number = number;
        Title = title ?? throw new ArgumentNullException(nameof(title));
    }

    public int Number { get; }

    public string Title { get; }

    /// <summary>The rail shows a check mark for finished steps and the number otherwise.</summary>
    public string Marker => IsDone ? "✓" : Number.ToString(System.Globalization.CultureInfo.InvariantCulture);

    partial void OnIsDoneChanged(bool value) => OnPropertyChanged(nameof(Marker));
}
