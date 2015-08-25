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
            byte[] encrypted = AESManager.Encrypt(password, "testKey", "testIV");
            var decrypted = AESManager.Decrypt(encrypted, "testKey", "testIV");
            Assert.IsTrue(decrypted.ConvertToUnsecureString() == password);
            Assert.IsFalse(decrypted.ConvertToUnsecureString() == "NotThePassword");
        }
    }
}