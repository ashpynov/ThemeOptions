using System;
using System.IO;
using System.Collections.Generic;
using Playnite.SDK;
using Playnite.SDK.Data;
using System.Linq;
using System.CodeDom;
using System.Runtime.InteropServices;
using System.Windows.Controls;


namespace ThemeOptions.Models
{
    public class Variables : DictNoCase<Variable>{}

    public class VariablesValues : DictNoCase<VariableValue>
    {
        public VariablesValues() : base() {}
        public VariablesValues(IEnumerable<KeyValuePair<string, Variable>> pairs) : base()
        {
            foreach(var p in pairs)
            {
                this[p.Key] = p.Value;
            }
        }
        public VariablesValues(IEnumerable<KeyValuePair<string, VariableValue>> pairs) : base()
        {
            foreach(var p in pairs)
            {
                this[p.Key] = p.Value;
            }
        }

        public VariablesValues Merge(IEnumerable<KeyValuePair<string, VariableValue>> pairs)
        {
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
                + " >\n"
                + string.Join("\n    ", items)
                + "</ResourceDictionary>\n";
        }
    }
}