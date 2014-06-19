using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nUpdate.Administration.Core
{
    internal class Navigator<T>
    {
        private List<T> navs = new List<T>();
        private T Last;
        private int count = 0;

        /// <summary>
        /// Sets if the navigator can go back.
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                return (count <= 0) == false;
            }
        }

        /// <summary>
        /// Sets if the navigator can go forward.
        /// </summary>
        public bool CanGoForward
        {
            get
            {
                return navs.Count != count;
            }
        }

        /// <summary>
        /// Goes back.
        /// </summary>
        public void Back()
        {
            if (CanGoBack) count--;
        }

        /// <summary>
        /// Goes forward.
        /// </summary>
        public void Forward()
        {
            if (CanGoForward) count++;
        }

        /// <summary>
        /// Returns the current item.
        /// </summary>
        public T Current
        {
            get
            {
                if ((count - 1 == -1) == false)
                {
                    return navs[count - 1];
                }
                else
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Sets an item.
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
