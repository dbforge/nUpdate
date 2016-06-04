using System;
using Microsoft.Win32;

namespace nUpdate
{
    [Serializable]
    public class UpdateRequirement
    {
        private Version _version;
        
        /// <summary>
        ///     Gets or sets the type of the <see cref="UpdateRequirement"/>.
        /// </summary>
        public UpdateRequirementType Type { get; set; }

        /// <summary>
        ///     The version of the assembly or Framework, the target machine has to have
        /// </summary>
        public Version Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                if (_version.Build == -1 && _version.Revision == -1)
                {
                    _version = new Version(_version.Major, _version.Minor, 0, 0);
                }
                if (_version.Build == -1)
                    _version = new Version(_version.Major, _version.Minor, 0, _version.Revision);
                if (_version.Revision == -1)
                    _version = new Version(_version.Major, _version.Minor, _version.Build, 0);
            }
        }
        
        public UpdateRequirement(UpdateRequirementType type, Version version)
        {
            Type = type;
            Version = version;
        }

        public bool CheckRequirement()
        {
            switch (Type)
            {
                case UpdateRequirementType.DotNetFramework:
                    RegistryKey subKey;
                    switch (Version.ToString(3))
                    {
                        case "2.0.0":
                            subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v2.0.50727");
                            if ((string)subKey?.GetValue("Version") != "2.0.50727.4927")
                                return false;
                            break;
                        case "3.0.0":
                            subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v3.0");
                            if ((string)subKey?.GetValue("Version") != "3.0.30729.4926")
                                return false;
                            break;
                        case "3.5.0":
                            subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v3.5");
                            if ((string)subKey?.GetValue("Version") != "3.5.30729.4926")
                                return false;
                            break;
                        case "4.0.0":
                            subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4.0\"Client");
                            if ((string)subKey?.GetValue("Version") != "4.0.0.0")
                                return false;
                            break;
                        case "4.5.0":
                            subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full");
                            if (subKey != null && !(int.Parse(subKey.GetValue("Release").ToString()) >= 378389))
                                return false;
                            break;
                        case "4.5.1":
                            if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 3)
                            {
                                subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full");
                                if (subKey != null && (!(int.Parse(subKey.GetValue("Release").ToString()) >= 378675) || !(int.Parse(subKey.GetValue("Release").ToString()) >= 378758)))
                                    return false;
                            }
                            break;
                        case "4.5.2":
                            subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full");
                            if (subKey != null && !(int.Parse(subKey.GetValue("Release").ToString()) >= 379893))
                                return false;
                            break;
                        case "4.6":
                            if (Environment.OSVersion.Version.Major == 10)
                            {
                                subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full");
                                if (subKey != null && !(int.Parse(subKey.GetValue("Release").ToString()) >= 393295))
                                    return false;
                            }
                            else
                            {
                                subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full");
                                if (subKey != null && !(int.Parse(subKey.GetValue("Release").ToString()) >= 393297))
                                    return false;
                            }

                            break;
                    }
                    break;
                case UpdateRequirementType.OSVersion:
                    return (Environment.OSVersion.Version < Version);
            }

            return true;
        }
    }
}