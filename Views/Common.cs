using System.Windows;
using System.Windows.Controls;

using Playnite.SDK;

using ThemeOptions.Models;

namespace ThemeOptions.Views
{

    public class VariablePanelItemsDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is Variable && (item as Variable).Type != null )
            {
                try
                {
                    return element.FindResource((item as Variable).Type.ToUpper() + "_PanelTemplate") as DataTemplate;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
    public static class FocusHelper
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(FocusHelper), new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var preset = (d as ComboBoxItem).DataContext as Preset;
            ThemeOptions.Settings.UpdatePreview(preset?.Preview, (bool)e.NewValue);
        }
    }
    public class Commands
    {
        public static RelayCommand<object> ResetToDefault { get; } = new RelayCommand<object>((sender) =>
        {
            Variable v = sender as Variable;
            v.Value = v.Default;
        });

        public static RelayCommand<object> TextInputCommand { get; } = new RelayCommand<object>((sender) => FullscreenSettingsView.TextInput(sender as Variable));
    }
}