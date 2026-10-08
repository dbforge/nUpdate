namespace nUpdate.Administration.TransferInterface;

/// <summary>
///     Marks the <see cref="IServiceProvider" /> of a plugin assembly. nUpdate Administration instantiates it and asks it
///     for an <see cref="ITransferProviderFactory" />.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ServiceProviderAttribute : Attribute
{
    public ServiceProviderAttribute(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        if (!typeof(IServiceProvider).IsAssignableFrom(serviceType))
            throw new ArgumentException("The type must implement System.IServiceProvider.", nameof(serviceType));
        ServiceType = serviceType;
    }

    public Type ServiceType { get; }
}
