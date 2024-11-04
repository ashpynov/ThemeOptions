
using Playnite.SDK;
using System.Windows.Media.Animation;

namespace ThemeOptions.Controls
{

    public partial class CommandControl
    {
         /********************************************************************************************************
          BeginStoryboard Command implementation
         ********************************************************
            @Name: Content.BeginStoryboard
            @Parameter: Storyboard to be triggered

            Just trigger Storyboard

            xmlns:pin="clr-namespace:Playnite.Input;assembly=Playnite"

            ...

            <Grid.InputBindings>
                <pin:GameControllerInputBinding
                    Button="LeftShoulder"
                    Command="{Binding Content.TouchTag, ElementName=ThemeOptions_Command}"
                    CommandParameter="{StaticResource SwipeLeft}"/>
            </Grid.InputBindings>

        ********************************************************************************************************/


        public new RelayCommand<object> BeginStoryboard => new RelayCommand<object>((o) => (o as Storyboard)?.Begin());
    }
}