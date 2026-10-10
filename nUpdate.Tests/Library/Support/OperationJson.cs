using nUpdate.Operations;

namespace nUpdate.Tests.Library.Support;

/// <summary>Sends operations through the JSON the package manifest uses.</summary>
public static class OperationJson
{
    /// <summary>Serializes the operation and reads it back as an <see cref="Operation" />, so the type discriminator decides the result.</summary>
    public static Operation RoundTrip(Operation operation) =>
        Serializer.Deserialize<Operation>(Serializer.Serialize(operation))!;
}
