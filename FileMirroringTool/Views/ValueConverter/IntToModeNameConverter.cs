using System;
using System.Globalization;
using System.Windows.Data;

namespace FileMirroringTool.Views.ValueConverter
{
    internal class IntToModeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (int)value;
            return val == 0 ? "Original" :
                val == 1 ? "Encrypt" :
                val == 2 ? "Decrypt" :
                string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
