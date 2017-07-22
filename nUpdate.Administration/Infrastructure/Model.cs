using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace nUpdate.Administration.Infrastructure
{
    [Serializable]
    public class Model : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(T value, ref T field, string propertyName = null)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (field != null && field.Equals(value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void OnPropertyChanged(string propertyName = null)
        {
            if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(propertyName));
            else
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}