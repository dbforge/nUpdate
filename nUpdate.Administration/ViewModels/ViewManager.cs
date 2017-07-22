using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nUpdate.Administration.ViewModels
{
    /// <summary>
    ///     Provides an abstract view manager class that offers basic convertion methods for finding the corresponding view for a specified view model by using an <see cref="IValueConverter"/>. 
    /// </summary>
    public abstract class ViewManager : IValueConverter
    {
        /// <summary>
        ///     Saves the types of the view models and their corresponding views.
        /// </summary>
        private readonly Dictionary<Type, Type> _viewTypes = new Dictionary<Type, Type>();

        /// <summary>
        ///      Saves the types of the view models and an instance of their view.
        /// </summary>
        private readonly Dictionary<Type, FrameworkElement> _views = new Dictionary<Type, FrameworkElement>();

        protected ViewManager()
        { }

        protected ViewManager(Dictionary<Type, Type> viewTypes)
        {
            AddViewTypes(viewTypes);
        }

        protected ViewManager(Dictionary<Type, FrameworkElement> views)
        {
            AddViews(views);
        }

        protected void AddViewType(Type viewModelType, Type viewType)
        {
            _viewTypes.Add(viewModelType, viewType);
        }

        protected void AddViewTypes(Dictionary<Type, Type> viewTypes)
        {
            foreach (var entry in viewTypes)
            {
                AddViewType(entry.Key, entry.Value);
            }
        }

        protected void AddView(Type viewModelType, FrameworkElement view)
        {
            AddViewType(viewModelType, view.GetType());
            _views.Add(viewModelType, view);
        }

        protected void AddViews(Dictionary<Type, FrameworkElement> views)
        {
            foreach (var entry in views)
            {
                AddView(entry.Key, entry.Value);
            }
        }

        /// <summary>
        ///     Gets an instance of the view that corresponds to the specified view model type.
        /// </summary>
        /// <param name="viewModelType">The type of the view model.</param>
        /// <param name="viewModelInstance">An existing instance of the view model.</param>
        /// <param name="viewType">The type of the view.</param>
        /// <returns>An instance of the corresponding view.</returns>
        private object GetViewInstance(Type viewModelType, object viewModelInstance, Type viewType)
        {
            // There is already a cached instance available.
            if (_views.ContainsKey(viewModelType))
                return _views[viewModelType];
            
            // No instance, yet. Let's create one and set the view model instance as data context.
            var view = (FrameworkElement) Activator.CreateInstance(viewType);
            view.DataContext = viewModelInstance;
            _views.Add(viewModelType, view);
            return view;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var viewModelType = value.GetType();
            return !_viewTypes.ContainsKey(viewModelType) ? null : GetViewInstance(viewModelType, value, _viewTypes[viewModelType]);
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This is not used in any case, so just leave it empty and throw a NotImplementedException.
            throw new NotImplementedException();
        }
    }
}
