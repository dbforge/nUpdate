namespace nUpdate.Tests.Library.Support;

/// <summary>Facts shared by the tests of the feed and the package manifest.</summary>
public static class TestFeed
{
    /// <summary>The project id the feed and manifest JSON of these tests is written for.</summary>
    public static Guid ProjectId { get; } = Guid.Parse("8f3c0a2e-5b1d-4e8a-9c7f-2d6b1e4a9f10");
}
