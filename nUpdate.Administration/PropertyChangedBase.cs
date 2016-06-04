using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
// ReSharper disable ExplicitCallerInfoArgument

namespace nUpdate.Administration
{
    [Serializable]
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(T value, ref T field, Expression<Func<object>> property)
        {
            return SetProperty(value, ref field, GetPropertyName(property));
        }

        protected virtual bool SetProperty<T>(T value, ref T field, [CallerMemberName]string propertyName = null)
        {
            if (!field.Equals(default(T)) && field.Equals(value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged(Expression<Func<object>> property)
        {
            OnPropertyChanged(GetPropertyName(property));
        }

        protected string GetPropertyName(Expression<Func<object>> property)
        {
            var lambda = property as LambdaExpression;
            MemberExpression memberExpression;
            var body = lambda.Body as UnaryExpression;
            if (body != null)
            {
                var unaryExpression = body;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }
            var propertyInfo = memberExpression?.Member as PropertyInfo;

            return propertyInfo?.Name ?? string.Empty;
        }
    }
}