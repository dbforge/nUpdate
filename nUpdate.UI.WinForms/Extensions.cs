// Extensions.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Text;
using nUpdate.UI.WinForms.Win32;

namespace nUpdate.UI.WinForms
{
    internal static class Extensions
    {
        public static string ToAdequateSizeString(this long fileSize)
        {
            var sb = new StringBuilder(20);
            NativeMethods.StrFormatByteSize(fileSize, sb, 20);
            return sb.ToString();
        }
    }
}