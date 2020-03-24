// IUpdateAction.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public interface IUpdateAction
    {
        string Description { get; }
        bool ExecuteBeforeReplacingFiles { get; set; }
        string Name { get; }
        Task Execute();
    }
}