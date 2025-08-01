using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace CustomWallpaper.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string str &&
                DateTime.TryParseExact(str, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            {
                return dt.ToString("dd/MM/yyyy HH:mm:ss");
            }

            if (value is DateTime dt2)
            {
                return dt2.ToString("dd/MM/yyyy HH:mm:ss");
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
