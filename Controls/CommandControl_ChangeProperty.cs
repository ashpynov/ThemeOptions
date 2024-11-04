
using Playnite.SDK;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

namespace ThemeOptions.Controls
{

    public partial class CommandControl
    {
         /********************************************************************************************************
          ChangeProperty Command implementation

          CommandParameter: [Setter ElementName=element_x_name, Property=property_name, Value=value]

         ********************************************************/

        public RelayCommand<object> ChangeProperty => new RelayCommand<object>((o) => ChangePropertyCommand(o));
        public class CustomSetter
        {
            static public bool TryParse(string value, out CustomSetter result)
            {
                result = null;
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }

                var regex = new Regex(@"\[\s*(?<CommandName>\w+)(?:\s+ElementName\s*=\s*(?<ElementName>\w+))?(?:,\s*Property\s*=\s*(?<Property>\w+))?(?:,\s*Value\s*=\s*(?<Value>\w+))?\s*\]");
                var match = regex.Match(value);
                if (match.Success && match.Groups["CommandName"].Value == "Setter")
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

            public CustomSetter(string elementName, string property, string value)
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
                if (FindName(setter.ElementName) is DependencyObject element)
                {
                    if (element.GetType().GetProperty(setter.Property, BindingFlags.Public | BindingFlags.Instance) is PropertyInfo property)
                    {
                        try
                        {
                            var converter = TypeDescriptor.GetConverter(property.PropertyType);
                            if (converter != null && converter.CanConvertFrom(typeof(string)))
                            {
                                property.SetValue(element, converter.ConvertFromString(setter.Value));
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
            else
            {
                Logger.Error($"ChangeProperty CommandParameter should match format: [Setter ElementName=element_x_name, Property=property_name, Value=value]");
            }
        }

    }
}