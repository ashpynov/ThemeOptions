using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace ThemeOptions.Tools
{
    public class Gamepad
    {
        dynamic Controllers;
        dynamic Model;

        void OnButtonUp(object sender, dynamic e)
        {
            ButtonUp?.Invoke(null, e?.Button.ToString());
            ButtonChanged?.Invoke(null, e?.Button.ToString());
        }
        void OnButtonDown(object sender, dynamic e)
        {
            ButtonDown?.Invoke(null, e?.Button.ToString());
            ButtonChanged?.Invoke(null, e?.Button.ToString());
        }

        public Gamepad()
        {
            dynamic model = Application.Current.MainWindow?.DataContext;
            if (model != null && model.GetType().Name == "FullscreenAppViewModel")
            {
                Model = model;
                Controllers = model.App.GameController.Controllers;

                var gameController = model.App.GameController;

                DynamicEventHandler.AddEventHandler(gameController, "ButtonUp", this, nameof(OnButtonUp));
                DynamicEventHandler.AddEventHandler(gameController, "ButtonDown", this, nameof(OnButtonDown));
            }
        }

        public EventHandler<string> ButtonUp;
        public EventHandler<string> ButtonDown;
        public EventHandler<string> ButtonChanged;

        int GetControllesStateHash()
        {
            int combinedHash = 17;

            try
            {
                foreach (var controller in Controllers)
                {
                    foreach (var kvp in controller.LastInputState)
                    {
                        int keyHash = kvp.Key.GetHashCode();
                        int valueHash = kvp.Value.GetHashCode();
                        unchecked // Allow arithmetic overflow, numbers will "wrap around"
                        {
                            combinedHash = combinedHash * 23 + keyHash;
                            combinedHash = combinedHash * 23 + valueHash;
                        }
                    }
                }
            }
            catch { };

            return combinedHash;
        }
        public void LongPressCommand(ICommand shortCommand, ICommand longCommand, object shortParameter, object longParameter)
        {
            var startState = GetControllesStateHash();
            DateTime startTime = DateTime.Now;

            bool shortPress = false;
            bool cancelPress = false;

            void onButtonUp(object sender, string e)
            {
                shortPress = true;
            }

            void onButtonDown(object sender, string e)
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
            if (!EnableGameControllerSupport)
            {
                return false;
            }

            try
            {
                foreach (var controller in Controllers)
                {
                    foreach (var kvp in controller.LastInputState)
                    {
                        if (kvp.Key.ToString().Equals(buttonName, StringComparison.OrdinalIgnoreCase))
                        {
                            if (kvp.Value.ToString().Equals("Pressed", StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public bool AltProcessing {
            get { return EnableGameControllerSupport && !Model.App.GameController.StandardProcessingEnabled; }
            set { if (Model != null) Model.App.GameController.StandardProcessingEnabled = !value && EnableGameControllerSupport; }
        }

        public bool EnableGameControllerSupport
        {
            get
            {
                return Model != null && Model.AppSettings.Fullscreen.EnableGameControllerSupport;
            }
        }
        public void DefaultProcess(string button, bool pressed)
        {
            if (Model == null || InputManager.Current.PrimaryKeyboardDevice?.ActiveSource == null)
            {
                return;
            }

            Assembly assembly = Assembly.LoadFrom(Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "playnite.dll"));;
            Type eventArgs = assembly.GetType("Playnite.Input.GameControllerInputEventArgs");
            object inputState = Enum.Parse(assembly.GetType("Playnite.Input.ControllerInputState"), pressed ? "Pressed" : "Released");
            object controllerInput = Enum.Parse(assembly.GetType("Playnite.Input.ControllerInput"), button);

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