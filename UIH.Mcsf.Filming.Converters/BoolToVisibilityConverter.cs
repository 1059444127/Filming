using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UIH.Mcsf.Filming.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = value is bool && (bool) value;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}