using Avalonia.Headless.XUnit;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views.Controls;

namespace nUpdate.Tests.Administration.App;

public class BusyOverlayTests
{
    private readonly AppTestContext _context = new();

    [AvaloniaFact]
    public void BusyOverlay_IsHiddenWhileIdle()
    {
        var busy = new BusyOverlay { DataContext = _context.Factory.Create<ProjectPasswordViewModel>() };
        busy.IsVisible.ShouldBeFalse();
    }
}
