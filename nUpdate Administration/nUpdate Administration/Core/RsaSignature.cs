/**
 *  Class to sign data with the RSACryptoServiceProvider-class.
 *  
 *  Author: Tim Schiewe (timmi31061)
 *  License: GPL v3
 *  Created: 09th December 2013
 *  Modified: Dominic B. (Trade) - 15.12.13 - Improved design of constructor
 *  Modified: Tim Schiewe (timmi31061) - 04.01.14 - Parameter validation added; more IntelliSense.
 *  Modified: Tim Schiewe (timmi31061) - 04.01.14 - Critical security update
 *  Modified: Dominic B. (Trade) - 23.01.2014 - Changed comments
 *  Modified: Dominic B. (Trade) - 23.12.2014 - Implemented IDisposable
 */

using System;
using System.Security.Cryptography;

namespace nUpdate.Administration.Core
{
    /// <summary>
    ///     Provides methods and properties to sign data with the RSACryptoServiceProvider.
    /// </summary>
    public class RsaSignature : IDisposable
    {
        /// <summary>
        ///     The default key size in bits.
        /// </summary>
        public const int DefaultKeySize = 8192;

        private bool _disposed;
        private readonly RSACryptoServiceProvider _rsa;

        /// <summary>
        ///     Creates a new instance of the <see cref="RsaSignature"/>-class.
        /// </summary>
        /// <param name="rsaKey">The public key to use.</param>
        public RsaSignature(string rsaKey)
        {
            if (string.IsNullOrEmpty(rsaKey))
                throw new ArgumentNullException("rsaKey");

            _rsa = new RSACryptoServiceProvider();
            _rsa.FromXmlString(rsaKey);
            _rsa.PersistKeyInCsp = false;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="RsaSignature"/>-class and creates a new key pair.
        /// </summary>
        public RsaSignature()
        {
            _rsa = new RSACryptoServiceProvider(DefaultKeySize);
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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