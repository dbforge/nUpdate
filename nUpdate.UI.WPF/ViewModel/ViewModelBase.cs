// ViewModelBase.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using nUpdate.UI.WPF.Properties;

// ReSharper disable once CheckNamespace
namespace nUpdate.UI.WPF.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        private static readonly List<string> HostProcesses = new List<string> {"XDesProc", "devenv", "WDExpress"};

        private bool _disposedValue;

        public bool IsInDesignMode
        {
            get
            {
                var ret = HostProcesses.Contains(Process.GetCurrentProcess().ProcessName);
                return ret;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
                if (disposing)
                {
                }

            _disposedValue = true;
        }
    }
}