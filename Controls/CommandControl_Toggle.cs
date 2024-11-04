
using Playnite.SDK;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ThemeOptions.Controls
{
    public partial class CommandControl
    {
        /********************************************************************************************************
          Toggle Command implementation
         ********************************************************
            @Name: Content.Toggle
            @Parameter: FrameworkElement or NameOf on which button press/toggle will be called

            Currently it will:
               call specified control command (if command exist)
            or toggle IsChecked of specified control (if command exists)

        ********************************************************************************************************/

        public RelayCommand<object> Toggle => new RelayCommand<object>(o =>
        {
            if (((o as FrameworkElement) ?? FindName(o as string)) is FrameworkElement control)
            {
                var type = control.GetType();
                if (type.GetProperty("Command") is PropertyInfo cp && cp.GetValue(control) is ICommand command )
                {
                    var parameter = type.GetProperty("Command")?.GetValue(control);
                    if (command.CanExecute(parameter))
                    {
                        command.Execute(parameter);
                    }
                }
                else if (type.GetProperty("IsChecked") is PropertyInfo isCheckedProp && isCheckedProp.GetValue(control) is bool isChecked)
                {
                    isCheckedProp.SetValue(control, !isChecked);
                }
            }
        });
    }
}