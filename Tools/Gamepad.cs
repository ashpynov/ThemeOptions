using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Playnite.SDK.Events;

namespace ThemeOptions.Tools
{
    public class Gamepad
    {
        dynamic Model;

        static Dictionary<ControllerInput, ControllerInputState> ControllerState = new Dictionary<ControllerInput, ControllerInputState>();
        public static ControllerInputState GetState(ControllerInput button)
        {
            return ControllerState.ContainsKey(button) ? ControllerState[button] : ControllerInputState.Released;
        }

        public static ControllerInput GetButton(string buttonName)
        {
            return Enum.TryParse(buttonName, ignoreCase: true, out ControllerInput button) ? button : ControllerInput.None;
        }
        public static ControllerInputState GetState(string buttonName)
        {
            ControllerInput button = GetButton(buttonName);
            return ControllerState.ContainsKey(button)
                    ? ControllerState[button] : ControllerInputState.Released;
        }

        public static void SetState(OnControllerButtonStateChangedArgs args)
        {
            ControllerState[args.Button] = args.State;
            if (args.State == ControllerInputState.Released)
            {
                ButtonUp?.Invoke(null, args.Button);
                ButtonChanged?.Invoke(null, args.Button);
            }
            else
            {
                ButtonDown?.Invoke(null, args.Button);
                ButtonChanged?.Invoke(null, args.Button);
            }
        }

        public Gamepad()
        {
            dynamic model = Application.Current.MainWindow?.DataContext;
            if (model != null && model.GetType().Name == "FullscreenAppViewModel")
            {
                Model = model;
            }
        }

        static public EventHandler<ControllerInput> ButtonUp;
        static public EventHandler<ControllerInput> ButtonDown;
        static public EventHandler<ControllerInput> ButtonChanged;

        int GetControllesStateHash()
        {
            int combinedHash = 17;
            foreach (ControllerInputState v in  ControllerState.Values)
            {
                unchecked // Allow arithmetic overflow, numbers will "wrap around"
                {
                    combinedHash = combinedHash * 23 + v.GetHashCode();
                }
            }

            return combinedHash;
        }
        public void LongPressCommand(ICommand shortCommand, ICommand longCommand, object shortParameter, object longParameter)
        {
            var startState = GetControllesStateHash();
            DateTime startTime = DateTime.Now;

            bool shortPress = false;
            bool cancelPress = false;

            void onButtonUp(object sender, ControllerInput e)
            {
                shortPress = true;
            }

            void onButtonDown(object sender, ControllerInput e)
            {
                cancelPress = true;
            }

            void removeHandlers()
            {
                ButtonDown -= onButtonDown;
                ButtonUp -= onButtonUp;
            }

            System.Timers.Timer timer = new System.Timers.Timer(50)
            {
                AutoReset = false,
                Enabled = true
            };

            timer.Elapsed += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (cancelPress)
                    {
                        removeHandlers();
                        return;
                    }
                    else if (shortPress)
                    {
                        removeHandlers();
                        timer.Stop();
                        timer.Dispose();
                        if (shortCommand?.CanExecute(shortParameter) == true)
                        {
                            shortCommand?.Execute(shortParameter);
                        }
                    }
                    else if ((DateTime.Now - startTime).TotalMilliseconds > 500)
                    {
                        removeHandlers();
                        timer.Stop();
                        timer.Dispose();
                        if (longCommand?.CanExecute(longParameter) == true)
                        {
                            longCommand?.Execute(longParameter);
                        }
                    }
                    else
                    {
                        ButtonUp += onButtonUp;
                        ButtonDown += onButtonDown;
                        timer.Start();
                    }
                });
            };
            timer.Start();

        }

        public bool IsPressed(string buttonName)
        {
            return EnableGameControllerSupport && GetState(buttonName) == ControllerInputState.Pressed;
        }

        private bool altProcessing = false;
        public bool AltProcessing
        {
            get
            {
                return EnableGameControllerSupport && altProcessing && !Model?.App?.GameController?.StandardProcessingEnabled;
            }
            set
            {
                if (Model != null)
                {
                    altProcessing = value;
                    Model.App.GameController.StandardProcessingEnabled = !value && EnableGameControllerSupport;
                }
            }
        }

        public bool EnableGameControllerSupport
        {
            get
            {
                return Model != null && Model.AppSettings.Fullscreen.EnableGameControllerSupport;
            }
        }
        public void DefaultProcess(string buttonName, bool pressed)
        {
            if (Model == null || InputManager.Current.PrimaryKeyboardDevice?.ActiveSource == null)
            {
                return;
            }

            Assembly assembly = Assembly.LoadFrom(Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "playnite.dll"));;
            Type eventArgs = assembly.GetType("Playnite.Input.GameControllerInputEventArgs");
            ControllerInputState inputState = pressed ? ControllerInputState.Pressed : ControllerInputState.Released;
            ControllerInput controllerInput = GetButton(buttonName);

            InputEventArgs args = Activator.CreateInstance( eventArgs, new object[] { Key.None, inputState, controllerInput }) as InputEventArgs;

            if (Model.App.GameController.GetType().GetField("inputManager", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Model.App.GameController) is InputManager manager)
            {
                MethodInfo MapPadToKeyboard = Model.App.GameController.GetType().GetMethod("MapPadToKeyboard", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo SimulateKeyInput = Model.App.GameController.GetType().GetMethod("SimulateKeyInput", BindingFlags.Instance | BindingFlags.NonPublic);

                manager.ProcessInput(args);

                var keyboard = MapPadToKeyboard.Invoke(Model.App.GameController, new object[] { controllerInput });

                Model.App.GameController.StandardProcessingEnabled = EnableGameControllerSupport;
                SimulateKeyInput.Invoke(Model.App.GameController, new object[] { keyboard, true });
                Model.App.GameController.StandardProcessingEnabled = false;
            }
        }
    }
}