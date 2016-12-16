using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace UIH.Mcsf.Filming.Converters
{
    public class ElementIndexConverter : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(values.Length == 2);
            return string.Format("{0}/{1}", values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
