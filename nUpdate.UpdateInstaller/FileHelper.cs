// FileHelper.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Runtime.InteropServices;

namespace nUpdate.UpdateInstaller
{
    public class FileHelper
    {
        private const int ERROR_SHARING_VIOLATION = 32;
        private const int ERROR_LOCK_VIOLATION = 33;

        public static bool IsFileLocked(Exception exception)
        {
            var errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION;
        }
    }
}