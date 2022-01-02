// IResettable.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

namespace nUpdate.Administration
{
    /// <summary>
    ///     Offers methods for resetting data when asynchronous operations failed.
    /// </summary>
    public interface IResettable
    {
        void Reset();
    }
}