using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace nUpdate.Administration.Infrastructure
{
    public class BoundRelayCommand : DependencyObject, ICommand
    {
        private static DependencyProperty CanExecuteBindingProperty = DependencyProperty.Register("CanExecuteBinding", typeof(bool), typeof(BoundRelayCommand));

        private bool CanExecuteBinding
        {
            get => (bool) GetValue(CanExecuteBindingProperty);
            set => SetValue(CanExecuteBindingProperty, value);
        }

        private readonly Action _methodToExecute;

        public event EventHandler CanExecuteChanged;

        public BoundRelayCommand(Action methodToExecute, BindingBase canExecuteBinding)
        {
            _methodToExecute = methodToExecute;
            BindingOperations.SetBinding(this, CanExecuteBindingProperty, canExecuteBinding);
            DependencyPropertyDescriptor.FromProperty(CanExecuteBindingProperty, typeof(BoundRelayCommand)).AddValueChanged(this, (sender, e) =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteBinding;
        }

        public void Execute(object parameter)
        {
            _methodToExecute();
        }
    }
}
