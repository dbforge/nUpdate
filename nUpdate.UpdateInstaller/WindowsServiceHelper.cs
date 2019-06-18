// WindowsServiceHelper.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;

namespace nUpdate.UpdateInstaller
{
    public static class WindowsServiceHelper
    {
        private static bool? _isRunningInServiceContext;

        /// <summary>
        ///     Gets a value indicating, if the application runs in service context
        /// </summary>
        public static bool IsRunningInServiceContext => _isRunningInServiceContext ??
                                                        (_isRunningInServiceContext =
                                                            DetermineIfRunningInServiceContext()).Value;

        private static bool DetermineIfRunningInServiceContext()
        {
            return !Environment.UserInteractive;
        }
    }
}