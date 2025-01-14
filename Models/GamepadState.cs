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

        private bool IsPressed(string button)
        {
            if (_gamepad == null)
            {
                _gamepad = new Gamepad();
                _gamepad.ButtonChanged += (o, e) => OnPropertyChanged(e);
            }
            return _gamepad.IsPressed(button);
        }

        public GamepadState()
        {
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Start { get => IsPressed("Start"); }
        public bool Back { get => IsPressed("Back"); }
        public bool LeftStick { get => IsPressed("LeftStick"); }
        public bool RightStick { get => IsPressed("RightStick"); }
        public bool LeftShoulder { get => IsPressed("LeftShoulder"); }
        public bool RightShoulder { get => IsPressed("RightShoulder"); }
        public bool Guide { get => IsPressed("Guide"); }
        public bool A { get => IsPressed("A"); }
        public bool B { get => IsPressed("B"); }
        public bool X { get => IsPressed("X"); }
        public bool Y { get => IsPressed("Y"); }
        public bool DPadLeft { get => IsPressed("DPadLeft"); }
        public bool DPadRight { get => IsPressed("DPadRight"); }
        public bool DPadUp { get => IsPressed("DPadUp"); }
        public bool DPadDown { get => IsPressed("DPadDown"); }
        public bool TriggerLeft { get => IsPressed("TriggerLeft"); }
        public bool TriggerRight { get => IsPressed("TriggerRight"); }
        public bool LeftStickLeft { get => IsPressed("LeftStickLeft"); }
        public bool LeftStickRight { get => IsPressed("LeftStickRight"); }
        public bool LeftStickUp { get => IsPressed("LeftStickUp"); }
        public bool LeftStickDown { get => IsPressed("LeftStickDown"); }
        public bool RightStickLeft { get => IsPressed("RightStickLeft"); }
        public bool RightStickRight { get => IsPressed("RightStickRight"); }
        public bool RightStickUp { get => IsPressed("RightStickUp"); }
        public bool RightStickDown { get => IsPressed("RightStickDown"); }

    }
}