using nUpdate.Localization;

namespace nUpdate.Ui;

/// <summary>A caption and a text, as the built-in user interfaces show errors.</summary>
public sealed class UpdateErrorMessage
{
    public UpdateErrorMessage(string caption, string text)
    {
        Caption = caption ?? throw new ArgumentNullException(nameof(caption));
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public string Caption { get; }

    public string Text { get; }
}

internal static class UpdateErrorMessages
{
    public static UpdateErrorMessage ForSearch(Exception exception, UpdateTexts texts)
    {
        Check(exception, texts);
        return new UpdateErrorMessage(texts.SearchError, exception.Message);
    }

    public static UpdateErrorMessage ForDownload(Exception exception, UpdateTexts texts)
    {
        Check(exception, texts);
        return new UpdateErrorMessage(texts.DownloadError, exception.Message);
    }

    public static UpdateErrorMessage ForVerification(Exception exception, UpdateTexts texts)
    {
        Check(exception, texts);
        return exception switch
        {
            FileNotFoundException => new UpdateErrorMessage(texts.VerificationError, texts.PackageNotFound),
            ArgumentException or Exceptions.InvalidFeedException => new UpdateErrorMessage(texts.VerificationError, texts.InvalidSignatureData),
            _ => new UpdateErrorMessage(texts.VerificationError, exception.Message),
        };
    }

    public static UpdateErrorMessage ForInvalidSignature(UpdateTexts texts)
    {
        if (texts is null)
            throw new ArgumentNullException(nameof(texts));
        return new UpdateErrorMessage(texts.InvalidSignatureTitle, texts.InvalidSignatureInfo);
    }

    public static UpdateErrorMessage ForInstall(Exception exception, UpdateTexts texts)
    {
        Check(exception, texts);
        return new UpdateErrorMessage(texts.InstallerInitializingError, exception.Message);
    }

    public static UpdateErrorMessage ForInsufficientDiskSpace(long packageSize, long bytesToFree, UpdateTexts texts)
    {
        if (texts is null)
            throw new ArgumentNullException(nameof(texts));
        return new UpdateErrorMessage(texts.NotEnoughDiskSpaceTitle,
            string.Format(System.Globalization.CultureInfo.CurrentCulture, texts.NotEnoughDiskSpaceInfo,
                ByteSizeFormatter.Format(packageSize), ByteSizeFormatter.Format(bytesToFree)));
    }

    private static void Check(Exception exception, UpdateTexts texts)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));
        if (texts is null)
            throw new ArgumentNullException(nameof(texts));
    }
}
