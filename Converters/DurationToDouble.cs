using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;
using System.Windows;

namespace ThemeOptions.Converters
{
    public class DurationToDouble : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan timeSpan;
            if (value is string strValue)
            {
                timeSpan = TimeSpan.Parse(strValue);
            }
            else if (value is Duration durationValue && durationValue.HasTimeSpan)
            {
                timeSpan = durationValue.TimeSpan;
            }
            else if (value is TimeSpan tsValue)
            {
                timeSpan = tsValue;
            }
            else
            {
                throw new NotSupportedException();
            }
            return timeSpan.TotalSeconds;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            double seconds = 0;
            if (value is string strValue)
            {
                seconds = double.Parse(strValue, CultureInfo.InvariantCulture);
            }
            else if (value is double doubleValue)
            {
                seconds = doubleValue;
            }
            else
            {
                throw new NotSupportedException();
            }
            return new Duration(TimeSpan.FromSeconds(seconds));
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
