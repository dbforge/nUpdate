using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Media;
using nUpdate.Installer;

namespace nUpdate.UpdateInstaller.UI.Avalonia;

/// <summary>What the installer window shows: progress while installing, a question for a locked file, or an error.</summary>
public sealed class InstallerWindowViewModel : INotifyPropertyChanged
{
    private readonly InstallerOptions _options;
    private string _status;
    private double _progress;
    private bool _isIndeterminate = true;
    private bool _isAskingAboutLockedFile;
    private bool _hasFailed;
    private string _lockedFileMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private Action<LockedFileDecision>? _answer;
    private Action? _acknowledge;

    public InstallerWindowViewModel(InstallerSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _options = session.Options;
        LogFileText = session.LogFilePath is null ? null : Text(InstallerText.LogFileHint, session.LogFilePath);
        Title = Text(InstallerText.WindowTitle, _options.Application.Name);
        _status = Text(InstallerText.ExtractingFiles);
        IconPath = _options.Ui.IconPath;
        AccentColor = Color.TryParse(_options.Ui.AccentColor, out var accent) ? accent : null;
        RetryCommand = new RelayCommand(() => Answer(LockedFileDecision.Retry));
        SkipCommand = new RelayCommand(() => Answer(LockedFileDecision.Skip));
        AbortCommand = new RelayCommand(() => Answer(LockedFileDecision.Abort));
        CloseCommand = new RelayCommand(Acknowledge);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Raised when the window should close: the installer is done.</summary>
    public event EventHandler? CloseRequested;

    public string Title { get; }

    /// <summary>The PNG the application gave as installer icon, or <c>null</c> for the nUpdate icon.</summary>
    public string? IconPath { get; }

    /// <summary>The accent color the application asked for, or <c>null</c> for the default.</summary>
    public Color? AccentColor { get; }

    public string Status
    {
        get => _status;
        private set => Set(ref _status, value);
    }

    /// <summary>0 to 100.</summary>
    public double Progress
    {
        get => _progress;
        private set => Set(ref _progress, value);
    }

    /// <summary>True until the first task is done, while the installer waits for the application to close.</summary>
    public bool IsIndeterminate
    {
        get => _isIndeterminate;
        private set => Set(ref _isIndeterminate, value);
    }

    public string PercentageText =>
        IsIndeterminate ? string.Empty : Math.Round(Progress).ToString(CultureInfo.CurrentCulture) + " %";

    public bool IsAskingAboutLockedFile
    {
        get => _isAskingAboutLockedFile;
        private set => Set(ref _isAskingAboutLockedFile, value);
    }

    public string LockedFileMessage
    {
        get => _lockedFileMessage;
        private set => Set(ref _lockedFileMessage, value);
    }

    public bool HasFailed
    {
        get => _hasFailed;
        private set => Set(ref _hasFailed, value);
    }

    public string ErrorCaption => Text(InstallerText.UpdatingErrorCaption);

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => Set(ref _errorMessage, value);
    }

    /// <summary>Where the log is, for the error view; <c>null</c> when none is written.</summary>
    public string? LogFileText { get; }

    public string RetryText => Text(InstallerText.RetryButton);

    public string SkipText => Text(InstallerText.SkipButton);

    public string AbortText => Text(InstallerText.AbortButton);

    public string CloseText => Text(InstallerText.CloseButton);

    public ICommand RetryCommand { get; }

    public ICommand SkipCommand { get; }

    public ICommand AbortCommand { get; }

    public ICommand CloseCommand { get; }

    /// <summary>Whether the window may close: only once the installer is done, never in the middle of an update.</summary>
    public bool CanClose { get; private set; }

    /// <summary>Shows the progress; 0 keeps the bar moving without a value.</summary>
    public void Report(float progress, string text)
    {
        Status = text;
        if (progress > 0)
            IsIndeterminate = false;
        Progress = Math.Max(0, Math.Min(100, progress));
        OnPropertyChanged(nameof(PercentageText));
    }

    /// <summary>Asks what to do with a locked file; <paramref name="answer" /> receives the button the user pressed.</summary>
    public void AskAboutLockedFile(string filePath, Action<LockedFileDecision> answer)
    {
        _answer = answer ?? throw new ArgumentNullException(nameof(answer));
        LockedFileMessage = Text(InstallerText.FileInUseError, filePath);
        IsAskingAboutLockedFile = true;
    }

    /// <summary>Shows the error; <paramref name="acknowledged" /> runs when the user closes it.</summary>
    public void ShowError(Exception exception, Action acknowledged)
    {
        ArgumentNullException.ThrowIfNull(exception);
        _acknowledge = acknowledged ?? throw new ArgumentNullException(nameof(acknowledged));
        ErrorMessage = exception.Message;
        IsAskingAboutLockedFile = false;
        HasFailed = true;
    }

    /// <summary>The installer is done: the window closes.</summary>
    public void Finish()
    {
        CanClose = true;
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void Answer(LockedFileDecision decision)
    {
        var answer = _answer;
        _answer = null;
        IsAskingAboutLockedFile = false;
        answer?.Invoke(decision);
    }

    private void Acknowledge()
    {
        // The error stays visible until the installer closes the window; Close only answers once.
        var acknowledge = _acknowledge;
        _acknowledge = null;
        acknowledge?.Invoke();
    }

    private string Text(InstallerText key, params object[] arguments) => _options.Text(key, arguments);

    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;
        field = value;
        OnPropertyChanged(name);
    }

    private void OnPropertyChanged(string? name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
