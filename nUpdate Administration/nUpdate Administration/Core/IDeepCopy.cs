// Author: Dominic Beger (Trade/ProgTrade)\nLicense: Creative Commons Attribution NoDerivs (CC-ND)\nCreated: 22-12-2014 19:56

namespace nUpdate.Administration.Core
{
    /// <summary>
    ///     Offers methods for cloning/copying class instances.
    /// </summary>
    /// <typeparam name="T">The class type to clone/copy.</typeparam>
    public interface IDeepCopy<out T> where T : class
    {
        T DeepCopy();
    }
}