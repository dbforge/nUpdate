using System.Collections.Generic;
using System.Linq;

namespace nUpdate.Administration.Core
{
    public class Navigator<T>
    {
        private int _currentIndex;
        private readonly List<T> _itemList = new List<T>();

        /// <summary>
        ///     Adds a new item to the current navigator's list.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            _itemList.Add(item);
            ++_currentIndex;
        }

        /// <summary>
        ///     Gets a value indicating whether the navigator can go back or not.
        /// </summary>
        public bool CanGoBack
        {
            get { return _currentIndex != 0; }
        }

        /// <summary>
        ///     Gets a value indicating whether the navigator can go forward or not.
        /// </summary>
        public bool CanGoForward
        {
            get { return _currentIndex != _itemList.Count - 1; }
        }

        /// <summary>
        ///     Returns the current item selected.
        /// </summary>
        public T Current
        {
            get { return _itemList.ElementAt(_currentIndex); }
        }

        /// <summary>
        ///     Moves the naviagtor to the previous item.
        /// </summary>
        public void GoBack()
        {
            if (_currentIndex != 0)
                --_currentIndex;
        }

        /// <summary>
        ///     Moves the navigator to the next item.
        /// </summary>
        public void GoForward()
        {
            if (_currentIndex != _itemList.Count - 1)
                ++_currentIndex;
        }
    }
}