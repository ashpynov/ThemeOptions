
using Playnite.SDK;
using System.Windows;
using System.Windows.Data;

namespace ThemeOptions.Controls
{
    public partial class CommandControl
    {
        /********************************************************************************************************
          TouchTag Command implementation
         ********************************************************
            @Name: Content.TouchTag
            @Parameter: FrameworkElement or NameOf on which TargetUpdated will be called

            To use it  - you have refer to object with 'Tag' field binded to something and NotifyOnTargetUpdated=true
            If it does not - Tag field binding will be created or update
            During triggering - value of tag is not chaned

            Latter you may handle TargetUpdated Event using Behaviors:

            xmlns:pin="clr-namespace:Playnite.Input;assembly=Playnite"
            xmlns:Behaviors="clr-namespace:Microsoft.Xaml.Behaviors;assembly=Microsoft.Xaml.Behaviors"
            xmlns:BehaviorsCore="clr-namespace:Microsoft.Xaml.Behaviors.Core;assembly=Microsoft.Xaml.Behaviors"
            xmlns:BehaviorsMedia="clr-namespace:Microsoft.Xaml.Behaviors.Media;assembly=Microsoft.Xaml.Behaviors"

            ...

            <Grid.InputBindings>
                <pin:GameControllerInputBinding
                    Button="LeftShoulder"
                    Command="{Binding Content.TouchTag, ElementName=ThemeOptions_Command}"
                    CommandParameter="{Binding ElementName=FILTER_Left}"/>
            </Grid.InputBindings>

            ...

            <ContentControl x:Name="FILTER_Left" Visibility="Collapsed">
                <Behaviors:Interaction.Triggers>
                    <Behaviors:EventTrigger EventName="TargetUpdated" >
                        <BehaviorsCore:ChangePropertyAction
                            TargetObject="{Binding ElementName=FILTER_PreviousName}"
                            PropertyName="Text" Value="{Binding ActiveFilterPreset.Name}"
                        />
                        <BehaviorsMedia:ControlStoryboardAction
                            Storyboard="{StaticResource SwipeLeft}"
                            ControlStoryboardOption="Play"
                        />
                        <Behaviors:InvokeCommandAction Command="{Binding PrevFilterViewCommand}" />
                    </Behaviors:EventTrigger>
                </Behaviors:Interaction.Triggers>
            </ContentControl>


        ********************************************************************************************************/

        public RelayCommand<object> TouchTag => new RelayCommand<object>(o =>
        {
            if (((o as FrameworkElement) ?? FindName(o as string)) is FrameworkElement obj)
            {

                BindingExpression bindingExpression = BindingOperations.GetBindingExpression(obj, TagProperty);

                if (bindingExpression == null )
                {
                    Binding binding = new Binding()
                    {
                        NotifyOnTargetUpdated = true
                    };
                    bindingExpression = BindingOperations.SetBinding(obj, TagProperty, binding) as BindingExpression;
                }
                else if (bindingExpression.ParentBinding.NotifyOnTargetUpdated == false)
                {
                    bindingExpression.ParentBinding.NotifyOnTargetUpdated = true;
                }
                bindingExpression.UpdateTarget();
            }
        });
    }
}