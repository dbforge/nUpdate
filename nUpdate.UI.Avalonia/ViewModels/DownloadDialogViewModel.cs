using System.Globalization;
using System.Windows.Input;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.UI.Avalonia.ViewModels;

/// <summary>Runs the download while the dialog is open and shows its progress.</summary>
internal sealed class DownloadDialogViewModel : DialogViewModel, IDisposable
{
    private readonly Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task> _download;
    private readonly DialogOperation<bool> _operation = new();
    private double _progress;
    private string _infoText = string.Empty;

    internal DownloadDialogViewModel(UpdateManager updateManager,
        Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task> download)
        : base(updateManager)
    {
        _download = download ?? throw new ArgumentNullException(nameof(download));
        CancelCommand = new RelayCommand(_operation.Cancel);
        ShowProgress(0);
    }

    public override string Title => Texts.Downloading;

    public ICommand CancelCommand { get; }

    /// <summary>0 to 100.</summary>
    public double Progress
    {
        get => _progress;
        private set => Set(ref _progress, value);
    }

    public string InfoText
    {
        get => _infoText;
        private set => Set(ref _infoText, value);
    }

    /// <summary>Completes once the dialog has closed; cancelled or faulted like the download.</summary>
    internal Task Completion => _operation.Completion;

    public void Dispose() => _operation.Dispose();

    public override async Task OnOpenedAsync()
    {
        var progress = new Progress<UpdateDownloadProgress>(value => ShowProgress(value.Percentage));
        await _operation.RunAsync(async token =>
        {
            await _download(progress, token);
            return true;
        });
        RequestClose(_operation.Succeeded);
    }

    public override bool OnClosing() => _operation.TryClose();

    internal void ShowProgress(float percentage)
    {
        Progress = percentage;
        InfoText = string.Format(CultureInfo.CurrentCulture, Texts.DownloadingInfo, Math.Round(percentage, 1))
            .Replace("\n", " ", StringComparison.Ordinal);
    }
}
