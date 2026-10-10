using System.Windows.Input;
using nUpdate.Updating;

namespace nUpdate.UI.Avalonia.ViewModels;

/// <summary>A message with a Close button: that the application is up to date, or an error with its details.</summary>
internal sealed class MessageDialogViewModel : DialogViewModel
{
    internal MessageDialogViewModel(UpdateManager updateManager, string caption, string text,
        Exception? exception = null)
        : base(updateManager)
    {
        Caption = caption ?? throw new ArgumentNullException(nameof(caption));
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Details = exception?.ToString();
        CloseCommand = new RelayCommand(() => RequestClose(true));
    }

    public override string Title => UpdateManager.ApplicationName;

    public string Caption { get; }

    public string Text { get; }

    /// <summary>The exception for a details section, or <c>null</c>.</summary>
    public string? Details { get; }

    public ICommand CloseCommand { get; }
}
