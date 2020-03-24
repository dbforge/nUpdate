// Extensions.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable once IdentifierTypo

namespace nUpdate.Administration.BusinessLogic
{
    internal static class Extensions
    {
        internal static async Task<bool> AllAsync<T>(this IEnumerable<T> items, Func<T, Task<bool>> predicate)
        {
            var itemTaskList = items.Select(item => new {Item = item, PredTask = predicate.Invoke(item)}).ToList();
            await Task.WhenAll(itemTaskList.Select(x => x.PredTask));
            return itemTaskList.All(x => x.PredTask.Result);
        }

        internal static string ConvertToInsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException(nameof(securePassword));

            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        internal static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie) action(i);
        }

        internal static bool IsValidPath(this string path)
        {
            var driveCheckRegEx = new Regex(@"^[a-zA-Z]:\\$");
            if (!driveCheckRegEx.IsMatch(path.Substring(0, 3)))
                return false;

            var invalidPathChars = new string(Path.GetInvalidPathChars());
            invalidPathChars += @":/?*" + "\"";
            var containsBadCharacterRegEx = new Regex("[" + Regex.Escape(invalidPathChars) + "]");
            return !containsBadCharacterRegEx.IsMatch(path.Substring(3, path.Length - 3));
        }

        internal static T Remove<T>(this Stack<T> stack, T element)
        {
            var obj = stack.Pop();
            if (obj.Equals(element))
                return obj;
            var toReturn = stack.Remove(element);
            stack.Push(obj);
            return toReturn;
        }

        internal static T[] RemoveAt<T>(this T[] source, int index)
        {
            var destination = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, destination, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, destination, index, source.Length - index - 1);

            return destination;
        }
    }
}