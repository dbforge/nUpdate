// Copyright © Dominic Beger 2017

using System;
using System.IO;
using System.Security.Cryptography;

namespace nUpdate
{
    /// <summary>
    ///     Provides methods and properties to sign and verify data with a <see cref="RSACryptoServiceProvider" />.
    /// </summary>
    public class RsaManager : IDisposable
    {
        /// <summary>
        ///     The default key size in bits.
        /// </summary>
        public const int DefaultKeySize = 8192;

        private readonly RSACryptoServiceProvider _rsa;

        private bool _disposed;

        /// <summary>
        ///     Creates a new instance of the <see cref="RsaManager" /> class.
        /// </summary>
        /// <param name="rsaKey">The public key to use.</param>
        public RsaManager(string rsaKey)
        {
            if (string.IsNullOrEmpty(rsaKey))
                throw new ArgumentNullException(nameof(rsaKey));

            _rsa = new RSACryptoServiceProvider();
            _rsa.FromXmlString(rsaKey);
            _rsa.PersistKeyInCsp = false;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="RsaManager" /> class and creates a new RSA-key pair.
        /// </summary>
        public RsaManager()
        {
            _rsa = new RSACryptoServiceProvider(DefaultKeySize);
            _rsa.ToXmlString(true);
            _rsa.PersistKeyInCsp = false;
        }

        /// <summary>
        ///     Gets the private key.
        /// </summary>
        public string PrivateKey => _rsa.ToXmlString(true);

        /// <summary>
        ///     Gets the public key.
        /// </summary>
        public string PublicKey => _rsa.ToXmlString(false);

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _rsa.Dispose();
            _disposed = true;
        }

        /// <summary>
        ///     Calculates the signature of the given data.
        /// </summary>
        /// <param name="data">The data to calculate the signature from.</param>
        /// <returns>The calculated signature.</returns>
        public byte[] SignData(byte[] data)
        {
            return _rsa.SignData(data, typeof(SHA512));
        }

        /// <summary>
        ///     Calculates the signature of the data provided in the <see cref="Stream" />.
        /// </summary>
        /// <param name="stream">The stream containing the data.</param>
        /// <returns>The calculated signature.</returns>
        public byte[] SignData(Stream stream)
        {
            return _rsa.SignData(stream, typeof(SHA512));
        }

        /// <summary>
        ///     Determines whether the signature of the given data is valid, or not.
        /// </summary>
        /// <param name="data">The data to check.</param>
        /// <param name="signature">The signature to check.</param>
        /// <returns>Returns <c>true</c> if the signature is valid, otherwise <c>false</c>.</returns>
        public bool VerifyData(byte[] data, byte[] signature)
        {
            return _rsa.VerifyData(data, typeof(SHA512), signature);
        }

        /// <summary>
        ///     Determines whether the signature of the data provided in the <see cref="Stream" /> is valid, or not.
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /> containing the data to check.</param>
        /// <param name="signature">The signature to check.</param>
        /// <returns>Returns <c>true</c> if the signature is valid, otherwise <c>false</c>.</returns>
        public bool VerifyData(Stream stream, byte[] signature)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var data = ms.ToArray();
                return _rsa.VerifyData(data, typeof(SHA512), signature);
            }
        }
    }
}