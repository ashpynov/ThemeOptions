
using Playnite.SDK;
using Playnite.SDK.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ThemeOptions.Controls
{

    public partial class CommandControl : PluginUserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly ILogger Logger = LogManager.GetLogger();

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        static CommandControl()
        {
        }

        public RelayCommand<object> ChangeProperty => new RelayCommand<object>((o)=> ChangePropertyCommand(o));
        public new RelayCommand<object> BeginStoryboard => new RelayCommand<object>((o)=>
        {
            if (o is Storyboard storyboard)
            {
                storyboard.Begin();
            }
        });


        public class CustomSetter
        {
            static public bool TryParse( string value, out CustomSetter result)
            {
                result = null;
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }

                var regex = new Regex(@"\[(?<CommandName>\w+)(?:\s+ElementName\s*=\s*(?<ElementName>\w+))?(?:,\s*Property\s*=\s*(?<Property>\w+))?(?:,\s*Value\s*=\s*(?<Value>\w+))?\]");
                var match = regex.Match(value);
                if (match.Success && match.Groups["CommandName"].Value == "Setter" )
                {
                    result = new CustomSetter(
                        match.Groups["ElementName"].Value,
                        match.Groups["Property"].Value,
                        match.Groups["Value"].Value
                    );
                    return true;
                }
                return false;
            }
            public string ElementName;
            public string Property;
            public string Value;

            public CustomSetter( string elementName, string property, string value)
            {
                ElementName = elementName;
                Property = property;
                Value = value;
            }

        }
        void ChangePropertyCommand(object o)
        {
            if (CustomSetter.TryParse(o as string, out CustomSetter setter))
            {
                if (FindName(setter.ElementName) is DependencyObject target)
                {
                    if (target.GetType().GetProperty(setter.Property, BindingFlags.Public | BindingFlags.Instance) is PropertyInfo property)
                    {
                        try
                        {
                            var converter = TypeDescriptor.GetConverter(property.PropertyType);
                            if (converter != null && converter.CanConvertFrom(typeof(string)))
                            {
                                property.SetValue(target, converter.ConvertFromString(setter.Value));
                            }
                            else
                            {
                                Logger.Error($"Cant convert Element {setter.ElementName} {setter.Property} from string");
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"Cant set Element {setter.ElementName} {setter.Property} property: {e.InnerException?.Message ?? e.Message}");
                        }
                    }
                    else
                    {
                        Logger.Error($"Element {setter.ElementName} has no {setter.Property} property");
                    }
                }
                else
                {
                    Logger.Error($"Cant find Element with name {setter.ElementName}");
                }
            }
        }
    }
}