using CommunityToolkit.Mvvm.ComponentModel;

namespace nUpdate.Administration.ViewModels;

/// <summary>One entry of a wizard's step rail.</summary>
public sealed partial class WizardStep(int number, string title) : ObservableObject
{
    [ObservableProperty] private bool _isCurrent;

    [ObservableProperty] private bool _isDone;

    public int Number { get; } = number;

    public string Title { get; } = title ?? throw new ArgumentNullException(nameof(title));

    /// <summary>The rail shows a check mark for finished steps and the number otherwise.</summary>
    public string Marker => IsDone ? "✓" : Number.ToString(System.Globalization.CultureInfo.InvariantCulture);

    partial void OnIsDoneChanged(bool value) => OnPropertyChanged(nameof(Marker));
}
