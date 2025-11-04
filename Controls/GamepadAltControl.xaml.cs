
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Playnite.SDK;
using Playnite.SDK.Controls;
using Playnite.SDK.Events;
using ThemeOptions.Tools;

namespace ThemeOptions.Controls
{
    public partial class GamepadAltControl : PluginUserControl
    {
        public Gamepad controller;

        private static readonly ILogger Logger = LogManager.GetLogger();
        private readonly bool suppressDefaults;
        private readonly bool global;

        InputBindingCollection actualBindings;
        public new InputBindingCollection InputBindings
        {
             set
             {
                if (value != actualBindings)
                {
                    (Parent as ContentControl).InputBindings.Clear();
                    (Parent as ContentControl).InputBindings.AddRange(value);
                    actualBindings = value;
                    Logger.Debug("Binding is changed");
                }
             }
        }

        static GamepadAltControl()
        {
            TagProperty.OverrideMetadata(typeof(GamepadAltControl), new FrameworkPropertyMetadata(null, OnTagChanged));
        }

        public GamepadAltControl(bool suppressDefaults = false, bool global = false)
        {
            this.suppressDefaults = suppressDefaults;
            this.global = global;

            ((IComponentConnector)this).InitializeComponent();
            DataContext = this;
            controller = new Gamepad();

            Gamepad.ButtonDown += OnButtonDown;

            dynamic model = Application.Current.MainWindow.DataContext;
            if (model != null && model.GetType().Name == "FullscreenAppViewModel")
            {
                (model.AppSettings.Fullscreen as INotifyPropertyChanged).PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "EnableGameControllerSupport")
                    {
                        UpdateAltProcessing();
                    }
                };
                (model.App as INotifyPropertyChanged).PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "IsActive")
                    {
                        UpdateAltProcessing();
                    }
                };
                EventManager.RegisterClassHandler(
                    typeof(UIElement),
                    Keyboard.GotKeyboardFocusEvent,
                        new KeyboardFocusChangedEventHandler((sender, args) =>
                    {
                        UpdateAltProcessing();
                    })
                );

            }
        }

        bool _scheduled = false;
        void UpdateAltProcessing()
        {
            if (!_scheduled)
            {
                _scheduled = true;
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _scheduled = false;
                    DoUpdateAltProcessing();
                }), DispatcherPriority.DataBind);
            }
        }
        void DoUpdateAltProcessing()
        {
            bool active = Tag?.ToString()?.Equals("True", StringComparison.OrdinalIgnoreCase) ?? false;
            if (Tag is InputBindingCollection collection)
            {
                active = true;
                InputBindings = collection;
            }
            controller.AltProcessing = active && (global || IsParentFocused(this));
        }

        private static void OnTagChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is GamepadAltControl gamepadAltControl)
            {
                gamepadAltControl.UpdateAltProcessing();
            }
        }

        private void OnButtonDown(object o, ControllerInput button)
        {
            if (controller.AltProcessing)
            {
                foreach (var binding in (Parent as ContentControl).InputBindings)
                {
                    if (binding.GetType().Name == "GameControllerInputBinding")
                    {
                        dynamic gamePadBinding = binding;
                        if (gamePadBinding.Button.ToString().Equals(button.ToString()))
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
                    controller.DefaultProcess(button.ToString(), pressed: true);
                }
            }
        }


        static bool IsParentFocused(DependencyObject current)
        {
            var parent = VisualTreeHelper.GetParent(current);
            if (parent == null)
            {
                return false;
            }
            else if (parent is UIElement parentElement && parentElement.IsKeyboardFocusWithin)
            {
                return true;
            }
            return IsParentFocused(parent);
        }
    }
}