using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace nUpdate.Administration.Infrastructure
{
    [Serializable]
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(T value, ref T field, [CallerMemberName]string propertyName = null)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (field != null && field.Equals(value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}