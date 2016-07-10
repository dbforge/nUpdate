// Author: Dominic Beger (Trade/ProgTrade)

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
            const string password = "ILikeTrains";
            byte[] encrypted = AesManager.Encrypt(password, "testKey", "testIV");
            var decrypted = AesManager.Decrypt(encrypted, "testKey", "testIV");
            Assert.IsTrue(password == decrypted.ConvertToInsecureString());
            Assert.IsFalse("NotThePassword" == decrypted.ConvertToInsecureString());
        }
    }
}