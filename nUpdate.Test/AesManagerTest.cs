// Author: Dominic Beger (Trade/ProgTrade) 2016

using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Administration.Core;

namespace nUpdate.Test
{
    [TestClass]
    public class AesManagerTest
    {
        [TestMethod]
        public void CanEncryptAndDecrypt()
        {
            const string password = "ILikeTrains";
            byte[] encrypted = AesManager.Encrypt(password, "testKey", "testIV");
            var decrypted = AesManager.Decrypt(encrypted, "testKey", "testIV");
            Assert.IsTrue(decrypted.ConvertToUnsecureString() == password);
            Assert.IsFalse(decrypted.ConvertToUnsecureString() == "NotThePassword");
        }
    }
}