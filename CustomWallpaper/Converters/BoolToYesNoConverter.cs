using System;
using Windows.UI.Xaml.Data;

namespace CustomWallpaper.Converters
{
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => (value is bool b && b) ? "Yes" : "No";

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => (value is string s && s.Equals("Yes", StringComparison.OrdinalIgnoreCase));
    }
}
