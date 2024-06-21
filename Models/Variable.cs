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
    public class VariableValue
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class Variable : VariableValue
    {
        public string Title { get; set; }
        public string Default { get; set; }
        public string Description { get; set; }
        public string Preview{ get; set; }
    }
}