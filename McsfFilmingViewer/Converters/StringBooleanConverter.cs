using System;
using System.Globalization;
using System.Windows.Data;

namespace UIH.Mcsf.Filming.Converters
{
    public class StringBooleanConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            var stringValue = value as string;
            return stringValue != null && !string.IsNullOrWhiteSpace(stringValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}