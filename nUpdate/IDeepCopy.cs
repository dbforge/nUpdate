// Copyright © Dominic Beger 2017

namespace nUpdate
{
    /// <summary>
    ///     Offers methods for cloning/copying class instances.
    /// </summary>
    /// <typeparam name="T">The class or struct type to clone/copy.</typeparam>
    public interface IDeepCopy<out T>
    {
        T DeepCopy();
    }
}