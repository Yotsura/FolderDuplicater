using System;
using System.Globalization;
using System.Windows.Data;

namespace FileMirroringTool.Views.ValueConverter
{
    [ValueConversion(typeof(Boolean), typeof(String))]
    internal class BoolToOnOffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;
            return val ? "ON" : "OFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value.ToString();
            return strValue == "ON";
        }
    }
}
