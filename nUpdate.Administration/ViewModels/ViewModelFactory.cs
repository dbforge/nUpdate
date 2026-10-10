using Microsoft.Extensions.DependencyInjection;

namespace nUpdate.Administration.ViewModels;

/// <summary>Creates view models with their dependencies resolved from the container.</summary>
public sealed class ViewModelFactory(IServiceProvider services)
{
    private readonly IServiceProvider _services = services ?? throw new ArgumentNullException(nameof(services));

    public T Create<T>(params object[] parameters) where T : ViewModelBase =>
        ActivatorUtilities.CreateInstance<T>(_services, parameters);
}
