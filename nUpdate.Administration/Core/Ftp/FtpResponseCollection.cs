using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace nUpdate.Administration.Core.Ftp
{
    /// <summary>
    ///     Ftp response collection.
    /// </summary>
    public class FtpResponseCollection : IEnumerable<FtpResponse>
    {
        private readonly List<FtpResponse> _list = new List<FtpResponse>();

        /// <summary>
        ///     Gets the number of elements actually contained in the FtpResponseCollection list.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        ///     Gets an FtpResponse from the FtpResponseCollection list based on index value.
        /// </summary>
        /// <param name="index">Numeric index of item to retrieve.</param>
        /// <returns>FtpResponse object.</returns>
        public FtpResponse this[int index]
        {
            get { return _list[index]; }
        }

        IEnumerator<FtpResponse> IEnumerable<FtpResponse>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        ///     Searches for the specified object and returns the zero-based index of the
        ///     first occurrence within the entire FtpResponseCollection list.
        /// </summary>
        /// <param name="item">The FtpResponse object to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire if found; otherwise, -1.</returns>
        public int IndexOf(FtpResponse item)
        {
            return _list.IndexOf(item);
        }


        /// <summary>
        ///     Adds an FtpResponse to the end of the FtpResponseCollection list.
        /// </summary>
        /// <param name="item">FtpResponse object to add.</param>
        public void Add(FtpResponse item)
        {
            _list.Add(item);
        }

        /// <summary>
        ///     Remove all elements from the FtpResponseCollection list.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        ///     Get the raw FTP server supplied reponse text.
        /// </summary>
        /// <returns>A string containing the FTP server response.</returns>
        public string GetRawText()
        {
            var builder = new StringBuilder();
            foreach (FtpResponse item in _list)
            {
                builder.Append(item.RawText);
                builder.Append("\r\n");
            }
            return builder.ToString();
        }

        /// <summary>
        ///     Get the last server response from the FtpResponseCollection list.
        /// </summary>
        /// <returns>FtpResponse object.</returns>
        public FtpResponse GetLast()
        {
            if (_list.Count == 0)
                return new FtpResponse();
            return _list[_list.Count - 1];
        }
    }
}