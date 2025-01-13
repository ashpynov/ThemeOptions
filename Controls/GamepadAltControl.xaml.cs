
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Playnite.SDK;
using Playnite.SDK.Controls;
using ThemeOptions.Models;
using ThemeOptions.Tools;

namespace ThemeOptions.Controls
{
    public partial class GamepadAltControl : PluginUserControl
    {
        //private IInputElement _previousFocusedElement;
        public Gamepad controller;

        private static readonly ILogger Logger = LogManager.GetLogger();
        private readonly bool suppressDefaults;

        static GamepadAltControl()
        {
            TagProperty.OverrideMetadata(typeof(GamepadAltControl), new FrameworkPropertyMetadata(-1, OnTagChanged));
        }

        public GamepadAltControl(bool suppressDefaults = false)
        {
            ((IComponentConnector)this).InitializeComponent();
            DataContext = this;
            controller = new Gamepad();
            this.suppressDefaults = suppressDefaults;
            controller.ButtonDown += OnButtonDown;

        }

        private static void OnTagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GamepadAltControl gamepadAltControl)
            {
                bool active = gamepadAltControl.Tag.ToString().Equals("True", StringComparison.OrdinalIgnoreCase);
                gamepadAltControl.controller.AltProcessing = active;
            }
        }

        private void OnButtonDown(object o, string buttonName)
        {
            if (controller.AltProcessing)
            {
                foreach (var binding in (Parent as ContentControl).InputBindings)
                {
                    if (binding.GetType().Name == "GameControllerInputBinding")
                    {
                        dynamic gamePadBinding = binding;
                        if (gamePadBinding.Button.ToString().Equals(buttonName))
                        {

                            if (gamePadBinding.Command?.CanExecute(gamePadBinding.CommandParameter) == true)
                            {
                                gamePadBinding.Command?.Execute(gamePadBinding.CommandParameter);
                            }
                            return;
                        }
                    }
                }
                // button not found, default processing
                if (!suppressDefaults)
                {
                    controller.DefaultProcess(buttonName, pressed: true);
                }
            }
        }
    }
}