// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using System.IO;
using nUpdate.Administration.Properties;
using Splat;

namespace nUpdate.Administration
{
    internal sealed class KeyManager : Singleton<KeyManager>, IEnableLogger
    {
        private bool _initialized;
        private Dictionary<Uri, object> _secrets = new Dictionary<Uri, object>();

        private void EnsureObjectState()
        {
            if (!_initialized)
                throw new InvalidOperationException();
        }
        
        internal object this[Uri identifier]
        {
            get
            {
                EnsureObjectState();

                if (identifier == null)
                    throw new ArgumentNullException(nameof(identifier));
                return _secrets[identifier];
            }
            set
            {
                EnsureObjectState();

                var secret = value;
                if (identifier == null)
                    throw new ArgumentNullException(nameof(identifier));
                if (secret == null)
                    throw new ArgumentNullException(nameof(secret));
                if (!secret.GetType().IsSerializable)
                    throw new ArgumentException("Must be serializable.", nameof(secret));

                if (_secrets.ContainsKey(identifier))
                    _secrets[identifier] = secret;
                else
                    _secrets.Add(identifier, secret);
                Save();
            }
        }

        internal void Initialize(string password)
        {
            if (!File.Exists(PathProvider.KeyDatabaseFilePath))
            {
                this.Log().Warn("Could not load the key database because it does not yet exist. Consider calling the \"Save\" method first to set an optional password and create it.");
                return;
            }

            byte[] data = File.ReadAllBytes(PathProvider.KeyDatabaseFilePath);
            if (Settings.Default.UseEncryptedKeyDatabase)
                data = AesCryptoProvider.Decrypt(data, password);
            _secrets = BinarySerializer.DeserializeType<Dictionary<Uri, object>>(data);
            _initialized = true;
        }

        internal void Save()
        {
            byte[] data = BinarySerializer.Serialize(_secrets);
            if (Settings.Default.UseEncryptedKeyDatabase)
                data = AesCryptoProvider.Encrypt(data, GlobalSession.MasterPassword);
            File.WriteAllBytes(PathProvider.KeyDatabaseFilePath, data);
        }
    }
}