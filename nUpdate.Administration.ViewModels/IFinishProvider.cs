// IFinishProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;

namespace nUpdate.Administration.ViewModels
{
    public interface IFinishProvider
    {
        void SetFinishAction(out Action finishAction);
    }
}