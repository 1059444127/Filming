using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Filming.Utility;

namespace UIH.Mcsf.Filming
{
    public class UIDToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var uid = (string)value;
            return FilmingHelper.TryTranslateFilmingResource(uid);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
