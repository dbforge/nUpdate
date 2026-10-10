using nUpdate.Tests.Integration.Support;

namespace nUpdate.Tests.Integration.Scenarios.Support;

/// <summary>
///     Base of the scenario tests: one scenario per test, written as Given/When/Then steps that drive the real
///     windows against the container servers. Every scenario starts with an empty server and an empty data folder.
/// </summary>
[Collection(ServerCollectionFixture.Name)]
[Trait("Category", "Integration")]
public abstract class ScenarioTest(ServerFixture server)
{
    private readonly List<string> _steps = [];

    /// <summary>The independent view of the servers and the temp folder, used to verify what the application did.</summary>
    protected IntegrationContext Context { get; private set; } = null!;

    /// <summary>The application under test.</summary>
    protected AdministrationApp App { get; private set; } = null!;

    /// <summary>Runs a scenario: skips without Docker, starts the application, and tears it down afterwards.</summary>
    protected async Task Scenario(Func<Task> body)
    {
        Assert.SkipUnless(ServerFixture.DockerAvailable || ServerFixture.DockerRequired,
            "Docker is not available on this machine.");
        await server.ResetAsync();
        Context = new IntegrationContext(server);
        App = new AdministrationApp(Context);
        try
        {
            await App.StartAsync();
            await body();
        }
        finally
        {
            App.Dispose();
            Context.Dispose();
        }
    }

    protected Task Given(string text, Func<Task> step) => Step("Given", text, step);

    protected Task When(string text, Func<Task> step) => Step("When", text, step);

    protected Task Then(string text, Func<Task> step) => Step("Then", text, step);

    protected Task And(string text, Func<Task> step) => Step("And", text, step);

    protected Task Given(string text, Action step) => Step("Given", text, () =>
    {
        step();
        return Task.CompletedTask;
    });

    protected Task When(string text, Action step) => Step("When", text, () =>
    {
        step();
        return Task.CompletedTask;
    });

    protected Task Then(string text, Action step) => Step("Then", text, () =>
    {
        step();
        return Task.CompletedTask;
    });

    protected Task And(string text, Action step) => Step("And", text, () =>
    {
        step();
        return Task.CompletedTask;
    });

    private async Task Step(string keyword, string text, Func<Task> step)
    {
        var line = $"{keyword} {text}";
        _steps.Add(line);
        TestContext.Current.TestOutputHelper?.WriteLine(line);
        try
        {
            await step();
            User.Pump();
        }
        catch (Exception ex) when (ex is not ScenarioStepException)
        {
            throw new ScenarioStepException(line, _steps, ex);
        }
    }
}

/// <summary>Names the step that failed and the steps before it, so the failure reads like the scenario.</summary>
public sealed class ScenarioStepException(string step, IReadOnlyList<string> steps, Exception inner) : Exception(
    $"Failed at: {step}{Environment.NewLine}{Environment.NewLine}Scenario so far:{Environment.NewLine}  {string.Join(Environment.NewLine + "  ", steps)}{Environment.NewLine}{Environment.NewLine}{inner.Message}",
    inner);
