using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Playnite.SDK.Data;
using ThemeOptions.Tools;

namespace ThemeOptions.Models
{
    public class GamepadState: Dictionary<string, bool>, INotifyPropertyChanged
    {
        Gamepad _gamepad;

        public new object this[string button]
        {
            get
            {
                if (_gamepad == null)
                {
                    _gamepad = new Gamepad();
                    _gamepad.ButtonChanged += (o, e) => OnPropertyChanged(e);
                }
                return _gamepad.IsPressed(button);
            }
            set { }
        }
        public GamepadState()
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}