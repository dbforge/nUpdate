// IDeepCopy.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

namespace nUpdate
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