using System;
using System.Globalization;
using System.Windows.Data;

namespace UIH.Mcsf.Filming.Converters
{
    public class DefaultEmptyItemConvertor : IValueConverter
    {

        private static readonly string DefaultNlsEmptyItem = string.Empty;

        static DefaultEmptyItemConvertor()
        {
            DefaultNlsEmptyItem = "<"+FilmingViewerContainee.FilmingResourceDict["UID_Filming_Default_Empty_Item"] +">";
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var modelString = value as string;
            return string.IsNullOrWhiteSpace(modelString) ? DefaultNlsEmptyItem : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewString = value as string;
            return viewString == DefaultNlsEmptyItem ? string.Empty : value;
        }
    }
}
