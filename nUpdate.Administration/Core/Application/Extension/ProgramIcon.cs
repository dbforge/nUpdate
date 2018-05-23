// Copyright © Dominic Beger 2018

using System;

namespace nUpdate.Administration.Core.Application.Extension
{
    public class ProgramIcon
    {
        /// <summary>
        ///     Represents and empty or nonexistent Program Icon
        /// </summary>
        public static readonly ProgramIcon None = new ProgramIcon();

        /// <summary>
        ///     Creates instance of ProgramIcon.
        /// </summary>
        /// <param name="path">Filename of file containing icon.</param>
        /// <param name="index">Index of icon within the file.</param>
        public ProgramIcon(string path, int index)
        {
            Path = path;
            Index = index;
        }

        /// <summary>
        ///     Creates instance of ProgramIcon.
        /// </summary>
        /// <param name="path">Filename of file containing icon.</param>
        public ProgramIcon(string path)
        {
            Path = path;
            Index = 0;
        }

        /// <summary>
        ///     Creates instance of ProgramIcon.
        /// </summary>
        public ProgramIcon()
        {
            Path = string.Empty;
            Index = 0;
        }

        /// <summary>
        ///     Gets or sets value that specifies icons index within a file.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///     Gets or sets a value that specifies the file containing the icon.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current System.Object.</param>
        /// <returns>true if the specified System.Object is equal to the current System.Object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            //Exists only to avoid compiler warning
            return this == obj as ProgramIcon;
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current System.Object.</returns>
        public override int GetHashCode()
        {
            //Exists only to avoid compiler warning
            return base.GetHashCode();
        }

        /// <summary>
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="rv"></param>
        /// <returns></returns>
        public static bool operator ==(ProgramIcon lv, ProgramIcon rv)
        {
            if (ReferenceEquals(lv, null) && ReferenceEquals(rv, null))
                return true;
            if (ReferenceEquals(lv, null) || ReferenceEquals(rv, null))
                return false;
            return lv.Path == rv.Path && lv.Index == rv.Index;
        }

        /// <summary>
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="rv"></param>
        /// <returns></returns>
        public static bool operator !=(ProgramIcon lv, ProgramIcon rv)
        {
            return !(lv == rv);
        }

        /// <summary>
        ///     Parses string to create and instance of ProgramIcon.
        /// </summary>
        /// <param name="regString">String specifying file path. Icon can be included as well.</param>
        /// <returns>ProgramIcon based on input string.</returns>
        public static ProgramIcon Parse(string regString)
        {
            if (regString == string.Empty)
                return new ProgramIcon("");

            if (regString.StartsWith("\"") && regString.EndsWith("\""))
                if (regString.Length > 3)
                    regString = regString.Substring(1, regString.Length - 2);

            var index = 0;

            var commaPos = regString.IndexOf(",", StringComparison.Ordinal);

            if (commaPos == -1)
                commaPos = regString.Length;
            else
                index = int.Parse(regString.Substring(commaPos + 1));

            var path = regString.Substring(0, commaPos);


            return new ProgramIcon(path, index);
        }

        /// <summary>
        ///     Returns string representation of current ProgramIcon.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Path + "," + Index;
        }
    }
}