using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using nUpdate.Installer;
using nUpdate.UpdateInstaller;

namespace CustomInstaller;

/// <summary>
///     A WPF window reporting the installer's progress. Built in code rather than XAML so the sample stays short.
///     <see cref="WindowProgressReporter" /> takes care of the threads: it calls the members that show something on the
///     window's thread.
/// </summary>
public sealed class WpfProgressReporter(InstallerSession session) : WindowProgressReporter(session)
{
    private readonly TextBlock _status = new()
    { TextTrimming = TextTrimming.CharacterEllipsis, Margin = new Thickness(0, 0, 0, 8) };

    private readonly TextBlock _percentage = new()
    { HorizontalAlignment = HorizontalAlignment.Right, Visibility = Visibility.Collapsed };

    private readonly ProgressBar _progress = new() { Height = 22, Minimum = 0, Maximum = 100, IsIndeterminate = true };
    private Window? _window;
    private bool _allowClose;

    private InstallerOptions Options => Session.Options;

    protected override void RunWindow(Action shown)
    {
        _status.Text = Options.Text(InstallerText.ExtractingFiles);

        var layout = new StackPanel { Margin = new Thickness(16) };
        layout.Children.Add(_status);
        layout.Children.Add(_progress);
        layout.Children.Add(_percentage);

        _window = new Window
        {
            Title = Options.Text(InstallerText.WindowTitle, Options.Application.Name),
            Width = 460,
            SizeToContent = SizeToContent.Height,
            ResizeMode = ResizeMode.NoResize,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = layout,
        };
        _window.Closing += (_, e) => e.Cancel = !_allowClose;
        _window.Loaded += (_, _) => shown();
        _window.ShowDialog();
    }

    protected override void Post(Action action) => _window!.Dispatcher.InvokeAsync(action);

    protected override void ShowProgress(float progress, string text)
    {
        _progress.IsIndeterminate = false;
        _progress.Value = Math.Max(0, Math.Min(100, progress));
        _percentage.Visibility = Visibility.Visible;
        _percentage.Text = $"{Math.Round(progress).ToString(CultureInfo.CurrentCulture)} %";
        _status.Text = text;
    }

    protected override void AskAboutLockedFile(string filePath, Action<LockedFileDecision> answer)
    {
        var message = Options.Text(InstallerText.FileInUseError, filePath);
        var result = MessageBox.Show(_window!,
            message + Environment.NewLine + Environment.NewLine +
            "Yes retries, No skips the file, Cancel aborts the update.",
            _window!.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        answer(result switch
        {
            MessageBoxResult.Yes => LockedFileDecision.Retry,
            MessageBoxResult.No => LockedFileDecision.Skip,
            _ => LockedFileDecision.Abort,
        });
    }

    protected override void ShowError(Exception exception, Action closed)
    {
        var message = exception.Message;
        if (Session.LogFilePath is not null)
            message += Environment.NewLine + Environment.NewLine +
                       Options.Text(InstallerText.LogFileHint, Session.LogFilePath);
        MessageBox.Show(_window!, message, Options.Text(InstallerText.UpdatingErrorCaption), MessageBoxButton.OK,
            MessageBoxImage.Error);
        closed();
    }

    protected override void Finish()
    {
        _allowClose = true;
        _window!.Close();
    }
}
