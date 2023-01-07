using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileMirroringTool.Views.ValueConverter
{
    [ValueConversion(typeof(FileInfo), typeof(String))]
    internal class FileInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (FileInfo)value;
            return val?.FullName ?? string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value.ToString();
            return string.IsNullOrEmpty(val) ? null : new FileInfo(val);
        }
    }
}
