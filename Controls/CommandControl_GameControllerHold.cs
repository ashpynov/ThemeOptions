using Playnite.SDK;
using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ThemeOptions.Tools;

namespace ThemeOptions.Controls
{
    public partial class CommandControl
    {
        /********************************************************************************************************
          Command to Assign action on Gamepad longPress button
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
                new Gamepad().LongPressCommand(buttons[0]?.Command, buttons[1]?.Command, buttons[0]?.CommandParameter, buttons[1]?.CommandParameter);
            }
        });

        /********************************************************************************************************
          Command to Assign action on Gamepad short press (on release) button
          Action occurs only on short click (on release) button
          it is required if you are going to use some button as 'modifier' on hold and  same time to keep ability
          to use it also on short press
          e.g. 'Options' button to open menu, but 'Options + View' as Switch to
         ********************************************************
            @Name: Content.GameControllerShortCommand
            @Parameter: Command to be called on Short press or Button object as Command and CommandParameter holder

            <Grid.Resources>
                <p:BindingProxy x:Key="MAIN_Commands" Data="{Binding}" />
                <p:BindingProxy x:Key="THEMEOPTIONS_Commands" Data="{Binding Content, ElementName=ThemeOptions_Command}" />
            </Grid.Resources>
            <Grid.InputBindings>
                <pin:GameControllerInputBinding
                    Button="Back"
                    Command="{Binding Data.OpenMainMenuCommand, Source={StaticResource MAIN_Commands}}"
                    CommandParameter="{Binding Data.TouchTag, Source={StaticResource THEMEOPTIONS_Commands}}" />
                </pin:GameControllerInputBinding>
            <Grid.InputBindings>

        ********************************************************************************************************/
        public RelayCommand<object> GameControllerAltCommand => new RelayCommand<object>(o =>
        {
            if (o is ICommand command )
            {
                new Gamepad().LongPressCommand(command, null, null, null);
            }
            else if (o is Button button)
            {
                new Gamepad().LongPressCommand(button.Command, null, button.CommandParameter, null);
            }
        });
    }
}