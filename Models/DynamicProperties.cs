using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace ThemeOptions.Models
{
    public class DynamicProperties : Dictionary<string, object>, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public DynamicProperties() {}

        public new object this[string propertyName]
        {
            get
            {
                TryGetValue(propertyName, out var value);
                return value;
            }
            set
            {
                base[propertyName] = value;
                OnPropertyChanged(propertyName);
            }
        }

        public DynamicProperties Update(VariablesValues variables) => Update(variables.ToDynamicProperties());

        public DynamicProperties Update(DynamicProperties properties)
        {
            foreach(var v in properties)
            {
                if (this[v.Key] != v.Value)
                {
                    this[v.Key] = v.Value;
                }
            }

            var toRemove = Keys.Where( k => !properties.Keys.Contains(k)).ToList();

            foreach (var k in toRemove)
            {
                Remove(k);
                OnPropertyChanged(k);
            }

            return this;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}