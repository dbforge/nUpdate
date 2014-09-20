// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System.Collections.Generic;

namespace nUpdate.Administration.Core
{
    internal class Navigator<T>
    {
        private readonly List<T> _navs = new List<T>();
        private T _last;
        private int _count;

        /// <summary>
        ///     Sets if the navigator can go back.
        /// </summary>
        public bool CanGoBack
        {
            get { return (_count <= 0) == false; }
        }

        /// <summary>
        ///     Sets if the navigator can go forward.
        /// </summary>
        public bool CanGoForward
        {
            get { return _navs.Count != _count; }
        }

        /// <summary>
        ///     Returns the current item.
        /// </summary>
        public T Current
        {
            get
            {
                if ((_count - 1 == -1) == false)
                    return _navs[_count - 1];
                return default(T);
            }
        }

        /// <summary>
        ///     Goes back.
        /// </summary>
        public void Back()
        {
            if (CanGoBack)
                _count--;
        }

        /// <summary>
        ///     Goes forward.
        /// </summary>
        public void Forward()
        {
            if (CanGoForward)
                _count++;
        }

        /// <summary>
        ///     Sets an item.
        /// </summary>
        public void Set(T value)
        {
            if (_navs.Count == 0)
            {
                _navs.Add(value);
                _last = value;
                _count++;
            }
            else if (!_last.Equals(value))
            {
                _navs.Add(value);
                _last = value;
                _count++;
            }
        }
    }
}