
using System;
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
            set => SetValue(ref isActive, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            property = value;
            OnPropertyChanged(propertyName);
        }
    }
}