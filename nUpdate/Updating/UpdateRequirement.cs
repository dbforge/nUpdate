using System;
using Microsoft.Win32;

namespace nUpdate.Updating
{
    [Serializable]
    public class UpdateRequirement
    {
        public enum RequirementType
        {
            DotNetFramework,
            OSVersion,
        }

        /// <summary>
        ///     Contains the error message, if the requirement is missing
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        ///     The type of the Requirement, like Framework, assembly and registry
        /// </summary>
        public RequirementType Type { get; set; }


        private Version _version;

        /// <summary>
        ///     The version of the assembly or Framework, the target machine has to have
        /// </summary>
        public Version Version
        {
            set
            {
                _version = value;
                if (value == null)
                    return;
                if (_version.Build == -1 && _version.Revision == -1)
                {
                    _version = new Version(_version.Major, _version.Minor, 0, 0);
                }
                if (_version.Build == -1)
                    _version = new Version(_version.Major, _version.Minor, 0, _version.Revision);
                if (_version.Revision == -1)
                    _version = new Version(_version.Major, _version.Minor, _version.Build, 0);
            }
            get
            {
                return _version;
            }
        }


        /// <summary>
        ///     Instanciate a new Type of UpdateRequirement
        /// </summary>
        /// <param name="type">The type of the Requirement, like Framework, assembly and registry</param>
        /// <param name="path">The path of the assembly or registry key</param>
        /// <param name="version">The version of the assembly or Framework, the target machine has to have</param>
        /// <param name="registryValue">The object the registry Value must have</param>
        public UpdateRequirement(RequirementType type, Version version)
        {
            this.Type = type;
            this.Version = version;
        }

        public Tuple<bool, string> CheckRequirement()
        {
            string message = "";
            bool meetRequirement = true;
            switch (Type)
            {
                case RequirementType.DotNetFramework:
                    bool frameworkOK = true;
                    switch (Version.ToString(3))
                    {
                        case "2.0.0":
                            if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v2.0.50727") != null && Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v2.0.50727").GetValue("Version") != "2.0.50727.4927")
                            {
                                frameworkOK = false;
                            }
                            break;
                        case "3.0.0":
                            if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v3.0") != null && Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v3.0").GetValue("Version") != "3.0.30729.4926")
                            {
                                frameworkOK = false;
                            }
                            break;
                        case "3.5.0":
                            if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v3.5") != null && Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v3.5").GetValue("Version") != "3.5.30729.4926")
                            {
                                frameworkOK = false;
                            }
                            break;
                        case "4.0.0":
                            if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4.0\"Client") != null && Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4.0\"Client").GetValue("Version") != "4.0.0.0")
                            {
                                frameworkOK = false;
                            }
                            break;
                        case "4.5.0":
                            if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full") != null && !(Int32.Parse(Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full").GetValue("Release").ToString()) >= 378389))
                            {
                                frameworkOK = false;
                            }
                            break;
                        case "4.5.1":
                            if(Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 3)
                            {
                                if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full") != null && !(Int32.Parse(Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full").GetValue("Release").ToString()) >= 378675))
                                {
                                    frameworkOK = false;
                                }
                            }
                            else
                            {
                                if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full") != null && !(Int32.Parse(Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full").GetValue("Release").ToString()) >= 378758))
                                {
                                    frameworkOK = false;
                                }
                            }
                           
                            break;
                        case "4.5.2":
                            if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full") != null && !(Int32.Parse(Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full").GetValue("Release").ToString()) >= 379893))
                            {
                                frameworkOK = false;
                            }
                            break;
                        case "4.6":
                            if(Environment.OSVersion.Version.Major == 10)
                            {
                                if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full") != null && !(Int32.Parse(Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full").GetValue("Release").ToString()) >= 393295))
                                {
                                    frameworkOK = false;
                                }
                            }
                            else
                            {
                                if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full") != null && !(Int32.Parse(Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full").GetValue("Release").ToString()) >= 393297))
                                {
                                    frameworkOK = false;
                                }
                            }
                            
                            break;
                    }

                    if (!frameworkOK)
                    {
                        meetRequirement = false;
                        message += "The .NET Framework version" + Version.ToString(3) + " is required. " + Environment.NewLine;
                    }
                    break;
                case RequirementType.OSVersion:
                    if (Environment.OSVersion.Version < Version)
                    {
                        meetRequirement = false;
                        message += "The OS Version " + Version.ToString(3) + " is required. " + Environment.NewLine;
                    }
                    break;
            }
            ErrorMessage = message;
            return new Tuple<bool, string>(meetRequirement, message);
        }

        public override string ToString()
        {
            switch (Type)
            {
                case RequirementType.OSVersion:
                    return "OS Version >= " + Version.ToString(2);
                case RequirementType.DotNetFramework:
                    return "Framework Version >= " + Version.ToString(3);
                default:
                    return null;
            }
        }
    }
}