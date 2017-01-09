using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UIH.Mcsf.Filming.Converters
{
    public class FocusedStatusToColorConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isFocused = (value is bool) && (bool) value;
            return isFocused ? Brushes.GreenYellow : Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
