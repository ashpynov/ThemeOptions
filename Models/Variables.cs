using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Globalization;


namespace ThemeOptions.Models
{
    public class Variables : DictNoCase<Variable>{}

    public class VariablesValues : DictNoCase<VariableValue>
    {
        public VariablesValues() : base() {}
        public VariablesValues(IEnumerable<KeyValuePair<string, Variable>> pairs) : base()
        {
            if (pairs == null) return;

            foreach(var p in pairs)
            {
                this[p.Key] = new VariableValue() { Type = p.Value.Type, Value = p.Value.Value };
            }
        }
        public VariablesValues(IEnumerable<KeyValuePair<string, VariableValue>> pairs) : base()
        {
            if (pairs == null) return;

            foreach(var p in pairs)
            {
                this[p.Key] = p.Value;
            }
        }

        public VariablesValues Merge(IEnumerable<KeyValuePair<string, VariableValue>> pairs)
        {
            if (pairs == null) return this;

            foreach(var p in pairs)
            {
                this[p.Key] = p.Value;
            }
            return this;
        }

        public string FormatResourceDictionary()
        {
            List<string> nss = new List<string>()
            {
                "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"",
                "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"",
                "xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\""
            };

            List<string> items = new List<string>();

            const string ns_sys = "xmlns:sys=\"clr-namespace:System;assembly=mscorlib\"";

            foreach (var v in this)
            {
                if (
                    v.Value.Type == "String"
                 || v.Value.Type == "Boolean"
                 || v.Value.Type == "Int32"
                 || v.Value.Type == "Double"
                 || v.Value.Type == "TimeSpan"
                )
                {
                    items.Add($"<sys:{v.Value.Type} x:Key=\"{v.Key}\">{v.Value.Value}</sys:{v.Value.Type}>");
                    nss.AddMissing(ns_sys);
                }
                else if (
                    v.Value.Type == "Color"
                 || v.Value.Type == "Thickness"
                 || v.Value.Type == "Duration"
                 || v.Value.Type == "CornerRadius"
                 || v.Value.Type == "Visibility"
                )
                {
                    items.Add($"<{v.Value.Type} x:Key=\"{v.Key}\">{v.Value.Value}</{v.Value.Type}>");
                }
                else if (
                    v.Value.Type == "SolidColorBrush"
                )
                {
                    items.Add($"<{v.Value.Type} x:Key=\"{v.Key}\" Color=\"{v.Value.Value}\" />");
                }
            }

            return "<ResourceDictionary "
                + string.Join("\n    ", nss)
                + " >\n    "
                + string.Join("\n    ", items)
                + "\n</ResourceDictionary>\n";
        }

        public static Thickness ConvertToThickness(string thicknessString)
        {
            // Split the string by commas
            string[] values = thicknessString.Split(',');

            // Parse the values to integers
            if (values.Length == 4 &&
                double.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double left) &&
                double.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double top) &&
                double.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double right) &&
                double.TryParse(values[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double bottom))
            {
                return new Thickness(left, top, right, bottom);
            }
            else if (values.Length == 1 && double.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture,out double uniform))
            {
                return new Thickness(uniform);
            }
            else
            {
                return new Thickness();
            }
        }

        public static CornerRadius ConvertToCornerRadius(string cornerRadiusString)
        {
            // Split the string by commas
            string[] values = cornerRadiusString.Split(',');

            if (values.Length == 4 &&
                double.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double left) &&
                double.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double top) &&
                double.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double right) &&
                double.TryParse(values[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double bottom))
            {
                return new CornerRadius(left, top, right, bottom);
            }
            else if (values.Length == 1 && double.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double uniform))
            {
                return new CornerRadius(uniform);
            }
            else
            {
                return new CornerRadius();
            }
        }

        public static object CastToObject(VariableValue variable)
        {
            object result = null;
            switch (variable.Type.ToLower())
                {
                    case "string":
                        result = variable.Value;
                        break;
                    case "boolean":
                        result = variable.Value.Equals("True", StringComparison.OrdinalIgnoreCase);
                        break;
                    case "int32":
                        result = int.TryParse(variable.Value, out int _int) ? _int : 0;
                        break;
                    case "double":
                        result = double.TryParse(variable.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double _double) ? _double : double.NaN;
                        break;
                    case "visibility":
                        result = variable.Value.Equals("Visible", StringComparison.OrdinalIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "timespan":
                        result = TimeSpan.TryParse(variable.Value, out TimeSpan _timespan) ? _timespan : TimeSpan.Zero;
                        break;
                    case "color":
                        result = ColorConverter.ConvertFromString(variable.Value);
                        break;
                    case "thickness":
                        result = ConvertToThickness(variable.Value);
                        break;
                    case "duration":
                        result = new DurationConverter().ConvertFromString(variable.Value);
                        break;
                    case "cornerradius":
                        result =  ConvertToCornerRadius(variable.Value);
                        break;
                    case "solidcolorbrush":
                        result = new SolidColorBrush((Color)ColorConverter.ConvertFromString(variable.Value));
                        break;
                    default:
                        // Handle unknown types if necessary
                        break;
                }
            return result;
        }

        public DynamicProperties ToDynamicProperties()
        {
            DynamicProperties properties = new DynamicProperties();
            List<string> existed = new List<string>();
            foreach (var v in this)
            {
                object value = CastToObject(v.Value);
                if (value != null)
                {
                    existed.Add(v.Key);
                    if (properties[v.Key] != value)
                    {
                        properties[v.Key] = value;
                    }
                }
            }
            return properties;
        }

        static public string CastToValue(object obj)
        {
            if (obj is double d) return d.ToString(CultureInfo.InvariantCulture);
            if (obj is Color c) return (c.A == 255) ? $"#{c.R:X2}{c.G:X2}{c.B:X2}" : c.ToString();
            if (obj is SolidColorBrush b) return (b.Color.A == 255) ? $"#{b.Color.R:X2}{b.Color.G:X2}{b.Color.B:X2}" : b.Color.ToString();
            return obj.ToString();
        }
        public VariablesValues FromDynamicProperties(DynamicProperties properties, bool updateOnly=false)
        {
            foreach (var v in properties)
            {
                if (ContainsKey(v.Key))
                {
                    this[v.Key].Value = CastToValue(v.Value);
                }
                else if (!updateOnly)
                {
                    Add(v.Key, new Variable() {
                         Type = v.Value.GetType().Name,
                         Value = CastToValue(v.Value)
                    });
                }
            }
            return this;
        }
    }
}