// IUpdateActionPathProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

namespace nUpdate.Actions
{
    public interface IUpdateActionPathProvider
    {
        string AssignPathVariables(string path);
    }
}