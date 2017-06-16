using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nUpdate.Administration.ViewModels.Base
{
    /// <summary>
    ///     Provides an abstract view manager class that offers basic convertion methods for finding the corresponding view for a specified view model by using an <see cref="IValueConverter"/>. 
    /// </summary>
    public abstract class ViewManagerBase : IValueConverter
    {
        // Here we save the types of the view models and their corresponding views. This will be set by the inheriting class.
        private readonly Dictionary<Type, Type> _viewModelViewDictionary;
        // Here we save the types of the view models and the instances of their views.
        private readonly Dictionary<Type, FrameworkElement> _views = new Dictionary<Type, FrameworkElement>();

        protected ViewManagerBase(Dictionary<Type, Type> v)
        {
            _viewModelViewDictionary = v;
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
            return !_viewModelViewDictionary.ContainsKey(viewModelType) ? null : GetViewInstance(viewModelType, value, _viewModelViewDictionary[viewModelType]);
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This is not used in any case, so just leave it empty and throw a NotImplementedException.
            throw new NotImplementedException();
        }
    }
}
