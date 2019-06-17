// ConnectionCheckerTest.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Internal.Core;

namespace nUpdate.Test
{
    [TestClass]
    public class ConnectionCheckerTest
    {
        /// <summary>
        ///     Checks if the "wininet.dll" returns the right stats.
        /// </summary>
        [TestMethod]
        public void CanReturnConnectionStatus()
        {
            Assert.IsTrue(ConnectionManager.IsConnectionAvailable());
        }
    }
}