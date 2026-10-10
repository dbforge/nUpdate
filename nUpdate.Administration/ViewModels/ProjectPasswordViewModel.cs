using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace nUpdate.Administration.ViewModels;

/// <summary>Asks how the credentials of a converted project are stored: encrypted under a project password, or not at all.</summary>
public partial class ProjectPasswordViewModel : DialogViewModel
{
    public const int MinimumLength = 8;

    [ObservableProperty] private bool _saveCredentials = true;

    [ObservableProperty] private string _password = string.Empty;

    [ObservableProperty] private string _passwordConfirmation = string.Empty;

    public ProjectPasswordViewModel()
    {
        Title = "Protect the project";
    }

    /// <summary>The chosen password, or <c>null</c> when the credentials are not saved. Set when the dialog was accepted.</summary>
    public string? Result { get; private set; }

    /// <summary>A validation message for a password choice, shared with the wizard and the settings.</summary>
    public static string? Validate(bool saveCredentials, string password, string confirmation)
    {
        if (!saveCredentials)
            return null;
        if (password.Length < MinimumLength)
            return $"The project password must have at least {MinimumLength} characters.";
        return password == confirmation ? null : "The passwords do not match.";
    }

    [RelayCommand]
    private void Accept()
    {
        ErrorMessage = Validate(SaveCredentials, Password, PasswordConfirmation);
        if (ErrorMessage is not null)
            return;
        Result = SaveCredentials ? Password : null;
        Close(true);
    }
}
