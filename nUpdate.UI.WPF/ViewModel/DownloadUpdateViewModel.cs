using System.Globalization;
using System.Windows.Input;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.UI.WPF.ViewModel;

/// <summary>Runs the download while the dialog is open and shows its progress.</summary>
public sealed class DownloadUpdateViewModel : DialogViewModel, IDisposable
{
    private readonly Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task> _download;
    private readonly DialogOperation<bool> _operation = new();
    private double _progressPercentage;
    private string _infoText = string.Empty;

    internal DownloadUpdateViewModel(UpdateManager updateManager,
        Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task> download)
        : base(updateManager)
    {
        _download = download ?? throw new ArgumentNullException(nameof(download));
        CancelCommand = new RelayCommand(_operation.Cancel);
        ShowProgress(0);
    }

    public override string WindowTitle => LocProperties.Downloading;

    public ICommand CancelCommand { get; }

    public double ProgressPercentage
    {
        get => _progressPercentage;
        private set => SetProperty(ref _progressPercentage, value);
    }

    public string InfoText
    {
        get => _infoText;
        private set => SetProperty(ref _infoText, value);
    }

    /// <summary>Completes once the dialog has closed; cancelled or faulted like the download.</summary>
    internal Task Completion => _operation.Completion;

    public void Dispose() => _operation.Dispose();

    internal override async Task OnLoadedAsync()
    {
        var progress = new Progress<UpdateDownloadProgress>(value => ShowProgress(value.Percentage));
        await _operation.RunAsync(async token =>
        {
            await _download(progress, token);
            return true;
        });
        RequestClose(_operation.Succeeded);
    }

    internal override bool OnClosing() => _operation.TryClose();

    private void ShowProgress(float percentage)
    {
        ProgressPercentage = percentage;
        InfoText = string.Format(CultureInfo.CurrentCulture, LocProperties.DownloadingInfo, Math.Round(percentage, 1));
    }
}
