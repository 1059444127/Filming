using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UIH.Mcsf.Filming.Converters
{
    public class SelectedStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isSelected = value is bool && (bool) value;
            return isSelected ? Brushes.Aqua : Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}