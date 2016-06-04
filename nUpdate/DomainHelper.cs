using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace nUpdate
{
    internal class DomainHelper
    {
        public static IEnumerable<_AppDomain> EnumerateAppDomains()
        {
            // Obtain ICLRMetaHost interface
            object objHost;
            int hr = CLRCreateInstance(ref CLSID_CLRMetaHost, ref IID_CLRMetaHost, out objHost);
            if (hr < 0) throw new COMException("Cannot create meta host", hr);
            var host = (ICLRMetaHost)objHost;

            // Obtain ICLRRuntimeInfo interface
            var vers = Environment.Version;
            var versString = $"v{vers.Major}.{vers.Minor}.{vers.Build}";
            var objRuntime = host.GetRuntime(versString, ref IID_CLRRuntimeInfo);
            var runtime = (ICLRRuntimeInfo)objRuntime;
            bool started;
            uint flags;
            runtime.IsStarted(out started, out flags);
            if (!started) throw new COMException("CLR not started??");

            // Obtain legacy ICorRuntimeHost interface and iterate appdomains
            var V2Host = (ICorRuntimeHost)runtime.GetInterface(ref CLSID_CorRuntimeHost, ref IID_CorRuntimeHost);
            IntPtr hDomainEnum;
            V2Host.EnumDomains(out hDomainEnum);
            for (;;)
            {
                _AppDomain domain;
                V2Host.NextDomain(hDomainEnum, out domain);
                if (domain == null) break;
                yield return domain;
            }
            V2Host.CloseEnum(hDomainEnum);
        }

        private static Guid CLSID_CLRMetaHost = new Guid(0x9280188d, 0xe8e, 0x4867, 0xb3, 0xc, 0x7f, 0xa8, 0x38, 0x84, 0xe8, 0xde);
        private static Guid IID_CLRMetaHost = new Guid(0xD332DB9E, 0xB9B3, 0x4125, 0x82, 0x07, 0xA1, 0x48, 0x84, 0xF5, 0x32, 0x16);
        private static Guid IID_CLRRuntimeInfo = new Guid(0xBD39D1D2, 0xBA2F, 0x486a, 0x89, 0xB0, 0xB4, 0xB0, 0xCB, 0x46, 0x68, 0x91);
        private static Guid CLSID_CorRuntimeHost = new Guid(0xcb2f6723, 0xab3a, 0x11d2, 0x9c, 0x40, 0x00, 0xc0, 0x4f, 0xa3, 0x0a, 0x3e);
        private static Guid IID_CorRuntimeHost = new Guid(0xcb2f6722, 0xab3a, 0x11d2, 0x9c, 0x40, 0x00, 0xc0, 0x4f, 0xa3, 0x0a, 0x3e);

        [DllImport("mscoree.dll")]
        private static extern int CLRCreateInstance(ref Guid clsid, ref Guid iid,
            [MarshalAs(UnmanagedType.Interface)] out object ptr);

        [ComImport, Guid("D332DB9E-B9B3-4125-8207-A14884F53216"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ICLRMetaHost
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            object GetRuntime(string version, ref Guid iid);
        }

        [ComImport, Guid("BD39D1D2-BA2F-486a-89B0-B4B0CB466891"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ICLRRuntimeInfo
        {
            void GetVersionString(char[] buffer, int bufferLength);
            void GetRuntimeDirectory(char[] buffer, int bufferLength);
            bool IsLoaded(IntPtr hProcess);
            void LoadErrorString(uint id, char[] buffer, int bufferLength, int lcid);
            void LoadLibrary(string path, out IntPtr hMdodule);
            void GetProcAddress(string name, out IntPtr addr);
            [return: MarshalAs(UnmanagedType.Interface)]
            object GetInterface(ref Guid clsid, ref Guid iid);
            bool IsLoadable();
            void SetDefaultStartupFlags(uint flags, string configFile);
            void GetDefaultStartupFlags(out uint flags, char[] configFile, int configFileLength);
            void BindAsLegacyV2Runtime();
            void IsStarted(out bool started, out uint flags);
        }

        [ComImport, Guid("CB2F6722-AB3A-11d2-9C40-00C04FA30A3E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ICorRuntimeHost
        {
            void CreateLogicalThreadState();
            void DeleteLogicalThreadState();
            void SwitchinLogicalThreadState(IntPtr cookie);
            void SwitchoutLogicalThreadState(out IntPtr cookie);
            void LocksHeldByLogicalThread(out int count);
            void MapFile(IntPtr hFile, out IntPtr address);
            void GetConfiguration(out IntPtr config);
            void Start();
            void Stop();
            void CreateDomain(string name, object identity, out _AppDomain domain);
            void GetDefaultDomain(out _AppDomain domain);
            void EnumDomains(out IntPtr hEnum);
            void NextDomain(IntPtr hEnum, out _AppDomain domain);
            void CloseEnum(IntPtr hEnum);
        }
    }
}