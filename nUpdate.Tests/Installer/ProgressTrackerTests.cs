using nUpdate.UpdateInstaller;

namespace nUpdate.Tests.Installer;

public class ProgressTrackerTests
{
    [Fact]
    public void ProgressTracker_ComputesPercentages()
    {
        var tracker = new ProgressTracker();
        tracker.Percentage.ShouldBe(100f);
        tracker.AddTasks(4);
        tracker.Total.ShouldBe(4);
        tracker.Advance().ShouldBe(25f);
        tracker.Advance().ShouldBe(50f);
        tracker.Done.ShouldBe(2);
        tracker.AddTasks(0);
        tracker.Advance();
        tracker.Advance();
        tracker.Advance().ShouldBe(100f);
        Should.Throw<ArgumentOutOfRangeException>(() => tracker.AddTasks(-1));
    }
}
