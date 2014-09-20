// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
namespace nUpdate.Administration.Core.Application.Extension
{
    internal class ProgramIcon
    {
        /// <summary>
        ///     Represents and empty or nonexistent Program Icon
        /// </summary>
        public static readonly ProgramIcon None = new ProgramIcon();

        private int index;
        private string path;

        /// <summary>
        ///     Creates instance of ProgramIcon.
        /// </summary>
        /// <param name="path">Filename of file containing icon.</param>
        /// <param name="index">Index of icon within the file.</param>
        public ProgramIcon(string path, int index)
        {
            this.path = path;
            this.index = index;
        }

        /// <summary>
        ///     Creates instance of ProgramIcon.
        /// </summary>
        /// <param name="path">Filename of file containing icon.</param>
        public ProgramIcon(string path)
        {
            this.path = path;
            index = 0;
        }

        /// <summary>
        ///     Creates instance of ProgramIcon.
        /// </summary>
        public ProgramIcon()
        {
            path = string.Empty;
            index = 0;
        }

        /// <summary>
        ///     Gets or sets value that specifies icons index within a file.
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        ///     Gets or sets a value that specifies the file containing the icon.
        /// </summary>
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        ///     Returns string representation of current ProgramIcon.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return path + "," + index;
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
            {
                if (regString.Length > 3)
                    regString = regString.Substring(1, regString.Length - 2);
            }

            string path;
            int index = 0;

            int commaPos = regString.IndexOf(",");

            if (commaPos == -1)
                commaPos = regString.Length;
            else
                index = int.Parse(regString.Substring(commaPos + 1));

            path = regString.Substring(0, commaPos);


            return new ProgramIcon(path, index);
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
            if (lv.path == rv.path && lv.index == rv.index)
                return true;

            return false;
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
        ///     Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current System.Object.</param>
        /// <returns>true if the specified System.Object is equal to the current System.Object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            //Exists only to avoid compiler warning
            return this == (obj as ProgramIcon);
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
    }
}