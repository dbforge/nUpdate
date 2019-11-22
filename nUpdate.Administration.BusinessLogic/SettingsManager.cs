using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.BusinessLogic
{
    public sealed class SettingsManager : Singleton<SettingsManager>
    {
        private bool _initialized;
        private Dictionary<string, object> _values = new Dictionary<string, object>();

        public void CreateDefault()
        {
            _values.Add("FirstRun", true);
            _values.Add("TransferServiceAssemblyPaths", Enumerable.Empty<string>());
            Save();
        }

        private void EnsureObjectState()
        {
            if (!_initialized)
                throw new InvalidOperationException("SettingsManager");
        }

        public bool CheckExistence(string key)
        {
            return _values.ContainsKey(key);
        }

        public object this[string identifier]
        {
            get
            {
                EnsureObjectState();

                if (identifier == null)
                    throw new ArgumentNullException(nameof(identifier));
                return _values[identifier];
            }
            set
            {
                EnsureObjectState();
                
                if (identifier == null)
                    throw new ArgumentNullException(nameof(identifier));
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (!value.GetType().IsSerializable)
                    throw new ArgumentException("Must be serializable.", nameof(value));

                if (_values.ContainsKey(identifier))
                    _values[identifier] = value;
                else
                    _values.Add(identifier, value);
                Save();
            }
        }

        public void Initialize()
        {
            if (!File.Exists(PathProvider.SettingsFilePath))
                CreateDefault();
            else
            {
                byte[] data = File.ReadAllBytes(PathProvider.SettingsFilePath);
                _values = BinarySerializer.DeserializeType<Dictionary<string, object>>(data);
            }

            _initialized = true;
        }

        public void Save()
        {
            byte[] data = BinarySerializer.Serialize(_values);
            File.WriteAllBytes(PathProvider.SettingsFilePath, data);
        }
    }
}