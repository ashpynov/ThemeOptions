
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Playnite.SDK;
using Playnite.SDK.Controls;

namespace ThemeOptions.Controls
{
    public partial class CommandControl : PluginUserControl, INotifyPropertyChanged
    {
        private static readonly ILogger Logger = LogManager.GetLogger();

        public CommandControl()
        {
            Application.Current.Deactivated += OnApplicationDeactivate;
            Application.Current.Activated += OnApplicationActivate;
        }

        private void OnApplicationDeactivate(object sender, EventArgs e)
        {
            IsActive = false;
        }

        private void OnApplicationActivate(object sender, EventArgs e)
        {
            IsActive = true;
        }

        private bool isActive = true;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}