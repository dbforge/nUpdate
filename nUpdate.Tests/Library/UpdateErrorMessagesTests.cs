using nUpdate.Exceptions;
using nUpdate.Localization;
using nUpdate.Ui;

namespace nUpdate.Tests.Library;

public class UpdateErrorMessagesTests
{
    [Fact]
    public void UpdateErrorMessages_MapExceptionsToTexts()
    {
        var texts = new UpdateTexts();
        var generic = new InvalidOperationException("boom");

        UpdateErrorMessages.ForSearch(generic, texts).ShouldSatisfyAllConditions(m => m.Caption.ShouldBe(texts.SearchError), m => m.Text.ShouldBe("boom"));
        UpdateErrorMessages.ForDownload(generic, texts).Caption.ShouldBe(texts.DownloadError);
        UpdateErrorMessages.ForVerification(new FileNotFoundException(), texts).Text.ShouldBe(texts.PackageNotFound);
        UpdateErrorMessages.ForVerification(new ArgumentException(), texts).Text.ShouldBe(texts.InvalidSignatureData);
        UpdateErrorMessages.ForVerification(new InvalidFeedException(), texts).Text.ShouldBe(texts.InvalidSignatureData);
        UpdateErrorMessages.ForVerification(generic, texts).Text.ShouldBe("boom");
        UpdateErrorMessages.ForInvalidSignature(texts).ShouldSatisfyAllConditions(m => m.Caption.ShouldBe(texts.InvalidSignatureTitle), m => m.Text.ShouldBe(texts.InvalidSignatureInfo));
        UpdateErrorMessages.ForInstall(generic, texts).Caption.ShouldBe(texts.InstallerInitializingError);
        var space = UpdateErrorMessages.ForInsufficientDiskSpace(2048, 1024, texts);
        space.Caption.ShouldBe(texts.NotEnoughDiskSpaceTitle);
        space.Text.ShouldContain("2.00 KB");
        space.Text.ShouldContain("1.00 KB");

        Should.Throw<ArgumentNullException>(() => UpdateErrorMessages.ForSearch(null!, texts));
        Should.Throw<ArgumentNullException>(() => UpdateErrorMessages.ForSearch(generic, null!));
        Should.Throw<ArgumentNullException>(() => UpdateErrorMessages.ForInvalidSignature(null!));
        Should.Throw<ArgumentNullException>(() => UpdateErrorMessages.ForInsufficientDiskSpace(1, 1, null!));
        Should.Throw<ArgumentNullException>(() => new UpdateErrorMessage(null!, "t"));
        Should.Throw<ArgumentNullException>(() => new UpdateErrorMessage("c", null!));
    }
}
