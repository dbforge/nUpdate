using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;

namespace nUpdate.Administration.Core
{
    public static class Extensions
    {
        #region "Control"

        /// <summary>
        /// Sets the possibility to apply double buffering on the listview-control.
        /// </summary>
        /// <param name="control">The control to apply the double buffering on.</param>
        public static void DoubleBuffer(this Control control)
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession) return;
            System.Reflection.PropertyInfo dbProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            dbProp.SetValue(control, true, null);
        }

        #endregion

        #region "Other"

        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        public static T Remove<T>(this Stack<T> stack, T element)
        {
            T obj = stack.Pop();
            if (obj.Equals(element))
            {
                return obj;
            }
            else
            {
                T toReturn = stack.Remove(element);
                stack.Push(obj);
                return toReturn;
            }
        }

        #endregion
    }
}
