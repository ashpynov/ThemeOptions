using Playnite.SDK;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ThemeOptions.Controls
{

    public partial class CommandControl
    {
        /********************************************************************************************************
          Coomand to Assign action on Gamepad longPress button
         ********************************************************
            @Name: Content.GameControllerHoldCommand
            @Parameter: Array of 2 Buttons to handle command on Short and Long press

            Warning: Usage of commands require to use Binding Proxy (from Playnite namespace)

            <Grid.Resources>
                <p:BindingProxy x:Key="MAIN_Commands" Data="{Binding}" />
                <p:BindingProxy x:Key="THEMEOPTIONS_Commands" Data="{Binding Content, ElementName=ThemeOptions_Command}" />
            </Grid.Resources>
            <Grid.InputBindings>
                <pin:GameControllerInputBinding
                    Button="Back"
                    Command="{Binding Content.GameControllerHoldCommand, ElementName=ThemeOptions_Command}">
                    <pin:GameControllerInputBinding.CommandParameter>
                        <x:Array Type="{x:Type Button}">
                            <Button
                                Command="{Binding Data.OpenMainMenuCommand, Source={StaticResource MAIN_Commands}}"
                            />
                            <Button
                                Command="{Binding Data.TouchTag, Source={StaticResource THEMEOPTIONS_Commands}}"
                                CommandParameter="FILTER_Show"/>
                        </x:Array>
                    </pin:GameControllerInputBinding.CommandParameter>
                </pin:GameControllerInputBinding>
            <Grid.InputBindings>

        ********************************************************************************************************/


        public RelayCommand<object> GameControllerHoldCommand => new RelayCommand<object>(o =>
        {
            if (o is Button[] buttons && buttons.Length == 2 )
            {
                Controller.LongPressCommand(buttons[0]?.Command, buttons[1]?.Command, buttons[0]?.CommandParameter, buttons[1]?.CommandParameter);
            }
        });

        static class Controller
        {
            static dynamic Controllers;
            static Controller()
            {
                dynamic model = Application.Current.MainWindow.DataContext;
                Controllers = model.App.GameController.Controllers;
            }
            static int GetControllesStateHash()
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
            public static void LongPressCommand(ICommand shortCommand, ICommand longCommand, object shortParameter, object longParameter)
            {
                var startState = GetControllesStateHash();
                DateTime startTime = DateTime.Now;
                System.Timers.Timer timer = new System.Timers.Timer(50)
                {
                    AutoReset = false,
                    Enabled = true
                };
                timer.Elapsed += (sender, e) =>
                {

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (startState != GetControllesStateHash())
                        {
                            timer.Stop();
                            timer.Dispose();
                            if (shortCommand?.CanExecute(shortParameter) == true)
                            {
                                shortCommand?.Execute(shortParameter);
                            }
                        }
                        else if ((DateTime.Now - startTime).TotalMilliseconds > 500)
                        {
                            timer.Stop();
                            timer.Dispose();
                            if (longCommand?.CanExecute(longParameter) == true)
                            {
                                longCommand?.Execute(longParameter);
                            }

                        }
                        else
                        {
                            timer.Start();
                        }
                    });
                };
                timer.Start();

            }
        }
    }
}