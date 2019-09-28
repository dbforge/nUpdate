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
            var transferProvider = UpdateProviderResolver.ResolveInternal(UpdateProviderType.ServerOverHttp);
            Assert.IsTrue(transferProvider.GetType() == typeof(HttpServerUpdateProvider));
        }

        [TestMethod]
        public void CanCreateInternalFtpTransferProvider()
        {
            var transferProvider = UpdateProviderResolver.ResolveInternal(UpdateProviderType.ServerOverFtp);
            Assert.IsTrue(transferProvider.GetType() == typeof(FtpServerUpdateProvider));
        }

        [TestMethod]
        public void CanCreateInternalGitHubTransferProvider()
        {
            var transferProvider = UpdateProviderResolver.ResolveInternal(UpdateProviderType.GitHub);
            Assert.IsTrue(transferProvider.GetType() == typeof(GitHubUpdateProvider));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CanNotCreateCustomTransferProviderWithResolveInternalMethod()
        {
            UpdateProviderResolver.ResolveInternal(UpdateProviderType.Custom);
        }

        [TestMethod]
        [ExpectedException(typeof(TransferProtocolException))]
        public void CanNotCreateCustomTransferProviderWithMissingAssemblyPath()
        {
            UpdateProviderResolver.ResolveCustom(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(TransferProtocolException))]
        public void CanNotCreateCustomTransferProviderWithInvalidAssemblyPath()
        {
            UpdateProviderResolver.ResolveCustom("+=this is not a path", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanNotCreateCustomTransferProviderWithMissingClassType()
        {
            UpdateProviderResolver.ResolveCustom("D:\\SamplePath", null);
        }
    }
}
