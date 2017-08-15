// Copyright © Dominic Beger 2017

namespace nUpdate.Core
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