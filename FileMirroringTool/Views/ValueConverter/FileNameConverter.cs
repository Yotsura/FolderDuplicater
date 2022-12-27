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
    internal class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value.ToString();
            var test = new System.Text.RegularExpressions.Regex(@"!Backup.*?\\").Split(path);
            var dir = test.ElementAtOrDefault(1);
            return dir ?? path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
