using System.Collections.Generic;

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
                return (this.count <= 0) == false;
            }
        }

        /// <summary>
        /// Sets if the navigator can go forward.
        /// </summary>
        public bool CanGoForward
        {
            get
            {
                return this.navs.Count != this.count;
            }
        }

        /// <summary>
        /// Goes back.
        /// </summary>
        public void Back()
        {
            if (this.CanGoBack)
            {
                this.count--;
            }
        }

        /// <summary>
        /// Goes forward.
        /// </summary>
        public void Forward()
        {
            if (this.CanGoForward)
            { 
                this.count++; 
            }
        }

        /// <summary>
        /// Returns the current item.
        /// </summary>
        public T Current
        {
            get
            {
                if ((this.count - 1 == -1) == false)
                {
                    return this.navs[this.count - 1];
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
            if (this.navs.Count == 0)
            {
                this.navs.Add(value);
                this.Last = value;
                count++;
            }
            else if (!this.Last.Equals(value))
            {
                this.navs.Add(value);
                this.Last = value;
                this.count++;
            }
        }

    }
}
