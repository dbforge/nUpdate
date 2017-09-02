// Copyright © Dominic Beger 2017

using nUpdate.Core.Win32;

namespace nUpdate.Core
{
    public class ConnectionManager
    {
        public static bool IsConnectionAvailable()
        {
            int desc;
            return NativeMethods.InternetGetConnectedState(out desc, 0);
        }
    }
}