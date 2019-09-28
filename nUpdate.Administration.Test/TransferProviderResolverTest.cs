using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Administration.Common;
using nUpdate.Administration.Common.Exceptions;

namespace nUpdate.Administration.Test
{
    [TestClass]
    public class TransferProviderResolverTest
    {
        [TestMethod]
        public void CanCreateInternalHttpTransferProvider()
        {
            var transferProvider = TransferProviderResolver.ResolveInternal(UpdateProviderType.ServerOverHttp);
            Assert.IsTrue(transferProvider.GetType() == typeof(HttpServerUpdateProvider));
        }

        [TestMethod]
        public void CanCreateInternalFtpTransferProvider()
        {
            var transferProvider = TransferProviderResolver.ResolveInternal(UpdateProviderType.ServerOverFtp);
            Assert.IsTrue(transferProvider.GetType() == typeof(FtpServerUpdateProvider));
        }

        [TestMethod]
        public void CanCreateInternalGitHubTransferProvider()
        {
            var transferProvider = TransferProviderResolver.ResolveInternal(UpdateProviderType.GitHub);
            Assert.IsTrue(transferProvider.GetType() == typeof(GitHubUpdateProvider));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CanNotCreateCustomTransferProviderWithResolveInternalMethod()
        {
            TransferProviderResolver.ResolveInternal(UpdateProviderType.Custom);
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
