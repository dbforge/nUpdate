namespace nUpdate
{
    internal interface IUpdateDeliveryEndpoint
    {
        DefaultUpdatePackage GetPackage(string versionData);
    }
}
