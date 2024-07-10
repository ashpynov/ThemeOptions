using Playnite.SDK;
using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;
using System.Windows;

namespace ThemeOptions.Converters
{
    public class DurationToString : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan timeSpan;
            if (value is Duration durationValue && durationValue.HasTimeSpan)
            {
                timeSpan = durationValue.TimeSpan;
            }
            else if (value is string strValue)
            {
                timeSpan = TimeSpan.Parse(strValue);
            }
            else if (value is TimeSpan tsValue)
            {
                timeSpan = tsValue;
            }
            else
            {
                throw new NotSupportedException();
            }

            int totalHours = (int)timeSpan.TotalHours;
            string hours = totalHours > 0 ? totalHours.ToString() + ":" : "";
            string ms = timeSpan.Milliseconds > 0 ? timeSpan.ToString(@"\.FFFFFF") : "";
            return hours + timeSpan.ToString(@"mm\:ss") + ms;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string strValue)
            {
                return new Duration(TimeSpan.FromSeconds(double.Parse(strValue, CultureInfo.InvariantCulture)));
            }
            else
            {
                throw new NotSupportedException();
            }

        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
