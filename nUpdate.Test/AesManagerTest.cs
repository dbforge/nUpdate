// Author: Dominic Beger (Trade/ProgTrade)

using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Administration;

namespace nUpdate.Test
{
    [TestClass]
    public class AesManagerTest
    {
        [TestMethod]
        public void CanEncryptAndDecrypt()
        {
            byte[] plain = Encoding.UTF8.GetBytes("test");
            var cipherText = AesCryptoProvider.Encrypt(plain, "master");
            Assert.AreEqual(AesCryptoProvider.Decrypt(cipherText, "master"), "test");
        }
    }
}