using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Administration.Common;
using nUpdate.Administration.Common.Exceptions;
using nUpdate.Administration.Common.Ftp;
using nUpdate.Administration.Common.Http;

namespace nUpdate.Administration.Test
{
    [TestClass]
    public class TransferProviderResolverTest
    {
        [TestMethod]
        public void CanCreateInternalHttpTransferProvider()
        {
            var transferProvider = TransferProviderResolver.ResolveInternal(TransferProviderType.Http);
            Assert.IsTrue(transferProvider.GetType() == typeof(HttpTransferProvider));
        }

        [TestMethod]
        public void CanCreateInternalFtpTransferProvider()
        {
            var transferProvider = TransferProviderResolver.ResolveInternal(TransferProviderType.Ftp);
            Assert.IsTrue(transferProvider.GetType() == typeof(FtpTransferProvider));
        }

        [TestMethod]
        public void CanCreateInternalGitHubTransferProvider()
        {
            var transferProvider = TransferProviderResolver.ResolveInternal(TransferProviderType.GitHub);
            Assert.IsTrue(transferProvider.GetType() == typeof(GitHubTransferProvider));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CanNotCreateCustomTransferProviderWithResolveInternalMethod()
        {
            TransferProviderResolver.ResolveInternal(TransferProviderType.Custom);
        }

        [TestMethod]
        [ExpectedException(typeof(TransferProtocolException))]
        public void CanNotCreateCustomTransferProviderWithMissingAssemblyPath()
        {
            TransferProviderResolver.ResolveCustom(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(TransferProtocolException))]
        public void CanNotCreateCustomTransferProviderWithInvalidAssemblyPath()
        {
            TransferProviderResolver.ResolveCustom("+=this is not a path", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanNotCreateCustomTransferProviderWithMissingClassType()
        {
            TransferProviderResolver.ResolveCustom("D:\\SamplePath", null);
        }
    }
}
