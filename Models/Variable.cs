using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Automation;
using Playnite.SDK.Data;

namespace ThemeOptions.Models
{
    public class VariableValue: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _value;

        public string Type { get; set; }
        public string Value { get => _value;
        set
        {
            _value = value;
            OnPropertyChanged();
        }}
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Slider
    {

        static private double ToDouble(string str) =>  str.Contains(":") ? TimeSpan.Parse(str).TotalSeconds : double.Parse(str, CultureInfo.InvariantCulture);
        static private string ToStr(double val) => val.ToString(CultureInfo.InvariantCulture);

        private double min;
        public string Min { get => ToStr(min); set { min = ToDouble(value); } }

        private double max;
        public string Max { get => ToStr(max); set { max = ToDouble(value); } }

        private double step;
        public string Step { get => ToStr(step); set { step = ToDouble(value); } }

        private double small;
        public string SmallChange { get => ToStr(small!= 0 ? small : step); set {  small = ToDouble(value); } }

        private double large;
        public string LargeChange
        {
            get
            {
                if (large != 0) return ToStr(large);
                var count = (max - min) / step;
                var scale = (int)(count / 10) + (count%10 > 0 ? 1 : 0);
                return ToStr(step * scale);
            }
            set
            {
                large = ToDouble(value);
            }
        }
    }

    public class Variable : VariableValue
    {
        [DontSerialize]
        public string Title { get; set; }
        [DontSerialize]
        public string LocKey { get; set; }
        [DontSerialize]
        public string Default { get; set; }
        [DontSerialize]
        public string Description { get; set; }
        [DontSerialize]
        public string Preview{ get; set; }
        [DontSerialize]
        public Slider Slider { get; set; }
    }
}