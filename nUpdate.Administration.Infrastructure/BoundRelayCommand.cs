// BoundRelayCommand.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace nUpdate.Administration.Infrastructure
{
    public class BoundRelayCommand : DependencyObject, ICommand
    {
        private static readonly DependencyProperty CanExecuteBindingProperty =
            DependencyProperty.Register("CanExecuteBinding", typeof(bool), typeof(BoundRelayCommand));

        private readonly Action _methodToExecute;

        public BoundRelayCommand(Action methodToExecute, BindingBase canExecuteBinding)
        {
            _methodToExecute = methodToExecute;
            BindingOperations.SetBinding(this, CanExecuteBindingProperty, canExecuteBinding);
            DependencyPropertyDescriptor.FromProperty(CanExecuteBindingProperty, typeof(BoundRelayCommand))
                .AddValueChanged(this, (sender, e) =>
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        private bool CanExecuteBinding => (bool) GetValue(CanExecuteBindingProperty);

        public bool CanExecute(object parameter)
        {
            return CanExecuteBinding;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _methodToExecute();
        }
    }
}