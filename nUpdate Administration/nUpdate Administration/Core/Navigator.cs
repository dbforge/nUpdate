using System.Collections.Generic;

namespace nUpdate.Administration.Core
{
    internal class Navigator<T>
    {
        private readonly List<T> navs = new List<T>();
        private T Last;
        private int count;

        /// <summary>
        ///     Sets if the navigator can go back.
        /// </summary>
        public bool CanGoBack
        {
            get { return (count <= 0) == false; }
        }

        /// <summary>
        ///     Sets if the navigator can go forward.
        /// </summary>
        public bool CanGoForward
        {
            get { return navs.Count != count; }
        }

        /// <summary>
        ///     Returns the current item.
        /// </summary>
        public T Current
        {
            get
            {
                if ((count - 1 == -1) == false)
                    return navs[count - 1];
                return default(T);
            }
        }

        /// <summary>
        ///     Goes back.
        /// </summary>
        public void Back()
        {
            if (CanGoBack)
                count--;
        }

        /// <summary>
        ///     Goes forward.
        /// </summary>
        public void Forward()
        {
            if (CanGoForward)
                count++;
        }

        /// <summary>
        ///     Sets an item.
        /// </summary>
        public void Set(T value)
        {
            if (navs.Count == 0)
            {
                navs.Add(value);
                Last = value;
                count++;
            }
            else if (!Last.Equals(value))
            {
                navs.Add(value);
                Last = value;
                count++;
            }
        }
    }
}