using System.ComponentModel;
using System.Runtime.CompilerServices;

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

    public class Variable : VariableValue
    {
        public string Title { get; set; }
        public string LocKey { get; set; }
        public string Default { get; set; }
        public string Description { get; set; }
        public string Preview{ get; set; }
    }
}