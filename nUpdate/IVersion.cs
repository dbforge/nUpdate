// IVersion.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;

namespace nUpdate
{
    public interface IVersion : IComparable
    {
        bool HasPreRelease { get; }
        string PreRelease { get; }
        bool IsValid();
    }

    public interface IVersion<T> : IVersion, IComparable<T>, IEquatable<T>
    {
    }
}