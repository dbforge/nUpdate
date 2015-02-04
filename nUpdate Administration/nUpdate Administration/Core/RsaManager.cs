// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Security.Cryptography;

namespace nUpdate.Administration.Core
{
    /// <summary>
    ///     Provides methods and properties to sign and verify data with the RSACryptoServiceProvider.
    /// </summary>
    public class RsaManager : IDisposable
    {
        /// <summary>
        ///     The default key size in bits.
        /// </summary>
        public const int DEFAULT_KEY_SIZE = 8192;

        private bool _disposed;
        private readonly RSACryptoServiceProvider _rsa;

        /// <summary>
        ///     Creates a new instance of the <see cref="RsaManager" />-class.
        /// </summary>
        /// <param name="rsaKey">The public key to use.</param>
        public RsaManager(string rsaKey)
        {
            if (string.IsNullOrEmpty(rsaKey))
                throw new ArgumentNullException("rsaKey");

            _rsa = new RSACryptoServiceProvider();
            _rsa.FromXmlString(rsaKey);
            _rsa.PersistKeyInCsp = false;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="RsaManager" />-class and creates a new key pair.
        /// </summary>
        public RsaManager()
        {
            _rsa = new RSACryptoServiceProvider(DEFAULT_KEY_SIZE);
            _rsa.ToXmlString(true);
            _rsa.PersistKeyInCsp = false;
        }

        /// <summary>
        ///     Returns the public key.
        /// </summary>
        public string PublicKey
        {
            get { return _rsa.ToXmlString(false); }
        }

        /// <summary>
        ///     Returns the private key.
        /// </summary>
        public string PrivateKey
        {
            get { return _rsa.ToXmlString(true); }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Calculates the signature for the given data.
        /// </summary>
        /// <param name="data">The data to calculate the signature for.</param>
        /// <returns>The calculated signature.</returns>
        public byte[] SignData(byte[] data)
        {
            return _rsa.SignData(data, typeof (SHA512));
        }

        /// <summary>
        ///     Checks if the signature for the given data is valid.
        /// </summary>
        /// <param name="data">The data to check.</param>
        /// <param name="signature">The signature to check.</param>
        /// <returns>Returns "true" if the signature is correct, otherwise "false".</returns>
        public bool VerifyData(byte[] data, byte[] signature)
        {
            return _rsa.VerifyData(data, typeof (SHA512), signature);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _rsa.Dispose();
            _disposed = true;
        }
    }
}