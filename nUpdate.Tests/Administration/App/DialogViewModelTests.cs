using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.ViewModels;

namespace nUpdate.Tests.Administration.App;

/// <summary>Behaviour every dialog shares: the busy state, cancelling and error descriptions.</summary>
public class DialogViewModelTests
{
    private readonly AppTestContext _context = new();

    private NewProjectViewModel ReadyWizard()
    {
        var viewModel = _context.Factory.Create<NewProjectViewModel>();
        viewModel.Name = "P";
        viewModel.UpdateUrl = "https://u.example.com/p";
        viewModel.Transfer.Host = "h";
        viewModel.Transfer.Username = "u";
        viewModel.Transfer.Password = "p";
        viewModel.SaveCredentials = false;
        viewModel.TestConnectionFirst = false;
        return viewModel;
    }

    [Fact]
    public async Task RunBusy_ReportsProgressAndErrors()
    {
        var viewModel = ReadyWizard();
        var accepted = new List<bool>();
        viewModel.CloseRequested += (_, a) => accepted.Add(a);
        viewModel.IsIdle.ShouldBeTrue();

        viewModel.CancelCommand.Execute(null);
        accepted.ShouldBe([false]);

        _context.Projects.CreateAsync(Arg.Any<NewProjectRequest>(), Arg.Any<IProgress<PipelineProgress>>(),
                Arg.Any<CancellationToken>())
            .ThrowsAsync(
                new PipelineException("step", new IOException("disk"), [new InvalidOperationException("undo")]));
        await viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("disk");
        viewModel.ErrorMessage!.ShouldContain("Rolling back failed too: undo");
        viewModel.IsBusy.ShouldBeFalse();
        DialogViewModel.Describe(new InvalidOperationException("plain")).ShouldBe("plain");
        DialogViewModel.Describe(new ArgumentException("A project named \"X\" already exists.", "newName"))
            .ShouldBe("A project named \"X\" already exists.");
        DialogViewModel.Describe(new ArgumentException("no parameter")).ShouldBe("no parameter");
        DialogViewModel.Describe(new PipelineException("s", new IOException("io"), []))
            .ShouldNotContain("Rolling back");
        Should.Throw<ArgumentNullException>(() => DialogViewModel.Describe(null!));
        Should.Throw<ArgumentNullException>(() => new ViewModelFactory(null!));
    }

    [Fact]
    public async Task CancelCommand_CannotExecuteWhileBusy()
    {
        var viewModel = ReadyWizard();
        var blocker = new TaskCompletionSource<ProjectLoadResult>();
        _context.Projects.CreateAsync(Arg.Any<NewProjectRequest>(), Arg.Any<IProgress<PipelineProgress>>(),
            Arg.Any<CancellationToken>()).Returns(blocker.Task);
        viewModel.CancelCommand.CanExecute(null).ShouldBeTrue();

        var create = viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.IsBusy.ShouldBeTrue();
        viewModel.CancelCommand.CanExecute(null).ShouldBeFalse();

        blocker.SetResult(AppTestContext.Loaded(AppTestContext.NewProject(), new ProjectSecrets()));
        await create;
        viewModel.IsBusy.ShouldBeFalse();
        viewModel.CancelCommand.CanExecute(null).ShouldBeTrue();
    }
}
