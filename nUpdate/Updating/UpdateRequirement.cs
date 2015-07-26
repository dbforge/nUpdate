using System;
using Microsoft.Win32;

namespace nUpdate.Updating
{
    [Serializable]
    public class UpdateRequirement
    {
        public enum RequirementType
        {
            netFramework,
            registry,
            assembly,
            osVersion,
        }


        /// <summary>
        ///     The type of the Requirement, like Framework, assembly and registry
        /// </summary>
        public RequirementType Type { get; set; }

        /// <summary>
        ///     The path of the assembly or registry key
        /// </summary>
        public string Path { get; set; }

        private Version _version;
        /// <summary>
        ///     The version of the assembly or Framework, the target machine has to have
        /// </summary>
        public Version Version {
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
        ///     The object the registry Value must have
        /// </summary>
        public Object RegistryValue { get; set; }
 

        /// <summary>
        ///     Instanciate a new Type of UpdateRequirement
        /// </summary>
        /// <param name="type">The type of the Requirement, like Framework, assembly and registry</param>
        /// <param name="path">The path of the assembly or registry key</param>
        /// <param name="version">The version of the assembly or Framework, the target machine has to have</param>
        /// <param name="registryValue">The object the registry Value must have</param>
        public UpdateRequirement(RequirementType type, string path, Version version, Object registryValue)
        {
            this.Type = type;
            this.Path = path;
            this.Version = version;
            this.RegistryValue = registryValue;
        }

        public Tuple<bool, string> CheckRequirement()
        {
            string message = "";
            bool meetRequirement = true;
            switch (Type)
            {
                case RequirementType.assembly:
                    string rootDirectory = Path.Split(new char[] { System.IO.Path.DirectorySeparatorChar })[0];

                    Path = Path.Replace(rootDirectory, "");

                    switch (rootDirectory)
                    {
                        case "%appdata%":
                            Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path;
                            break;
                        case "executable directory":
                        case "full path":
                            Path = Path.Remove(0, 1);
                            break;
                            
                    }
                    if (!System.IO.File.Exists(Path))
                    {
                        meetRequirement = false;
                        message += " The assembly" + Path + " is missing." + Environment.NewLine;
                        break;
                    }

                    System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(Path);
                    Version fileVersion = new Version(fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
                    if (fileVersion < Version)
                    {
                        meetRequirement = false;
                        message += " The assembly" + Path + " is version is lower than" + Version.ToString(4) + ". " + Environment.NewLine;
                    }
                    break;

                case RequirementType.netFramework:
                    bool frameworkOK = true;
                    switch (Version.ToString(3))
                    {
                        case "2.0.0":
                            if(Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v2.0.50727") != null && Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v2.0.50727").GetValue("Version") != "2.0.50727.4927")
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
                            if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full") != null && !(Int32.Parse(Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full").GetValue("Release").ToString()) >= 378758))
                            {
                                frameworkOK = false;
                            }
                            break;
                        case "4.5.2":
                            if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full") != null && !(Int32.Parse(Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full").GetValue("Release").ToString()) >= 379893))
                            {
                                frameworkOK = false;
                            }
                            break;
                        case "4.6":
                            if (Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full") != null && !(Int32.Parse(Registry.LocalMachine.OpenSubKey("SOFTWARE\"Microsoft\"NET Framework Setup\"NDP\v4\"Full").GetValue("Release").ToString()) >= 381029))
                            {
                                frameworkOK = false;
                            }
                            break;
                    }
                   
                    if (!frameworkOK)
                    {
                        meetRequirement = false;
                        message += "The .NET Framework version" + Version.ToString(3) + " is required. " + Environment.NewLine;
                    }
                    break;

                case RequirementType.registry:
                    string[] pathPieces = Path.Split(new string[] { "\\" },StringSplitOptions.RemoveEmptyEntries );
                    string keyPath = "";
                    for (int i = 1; i < pathPieces.Length - 1; i++)
                    {
                        keyPath += pathPieces[i] + "\\";
                    }

                    Object value = null;

                    if (pathPieces[0] == "HKEY_CLASSES_ROOT")
                    {
                       value = Registry.ClassesRoot.OpenSubKey(keyPath).GetValue(pathPieces[pathPieces.Length - 1]);
                    }
                    else if (pathPieces[0] == "HKEY_CURRENT_USER")
                    {
                        value = Registry.CurrentUser.OpenSubKey(keyPath).GetValue(pathPieces[pathPieces.Length - 1]);
                    }
                    else if (pathPieces[0] == "HKEY_LOCAL_MACHINE")
                    {
                        value = Registry.LocalMachine.OpenSubKey(keyPath).GetValue(pathPieces[pathPieces.Length - 1]);
                    }

                    if (!RegistryValue.Equals(value))
                    {
                        meetRequirement = false;
                        message += "The registry key " + Path + " has a wrong value or does not exist!" + Environment.NewLine;
                    }
                    break;

                case RequirementType.osVersion:
                    if (Environment.OSVersion.Version < Version)
                    {
                        meetRequirement = false;
                        message += "The OS Version" + Version.ToString(3) + " is required. " + Environment.NewLine;
                    }  
                    break;
            }
            return new Tuple<bool, string>(meetRequirement, message);
        }

        public override string ToString()
        {
            switch (Type)
            {
                case RequirementType.osVersion:
                    return "OS Version >= " + Version.ToString(2);
                case RequirementType.assembly:
                    string[] pieces = Path.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    string fileName = pieces[pieces.Length - 1];          
                    return "Assembly: " + fileName;
                case RequirementType.netFramework:
                    return "Framework Version >= " + Version.ToString(3);
                case RequirementType.registry:
                    string[] regPath = Path.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    string registryKey = regPath[regPath.Length - 1];
                   
                    return "Registry Key: " + registryKey + " Value: " + RegistryValue; // TODO: show name
                default:
                    return null;
            }
        }
    }
}
