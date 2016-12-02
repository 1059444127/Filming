using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Forms;

namespace UIH.Mcsf.Filming.Converters
{
    public class LoadingConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
            {
                return value;
            }

            //按照百分比显示
            //float temp;
            //if (float.TryParse(value.ToString(), out temp))
            //{
            //    string process = string.Format("{0:s}: {1:#}%", (string)FilmingViewerContainee.FilmingResourceDict[
            //            "UID_Filming_Loading_ProcessStep"], temp);
            //    return process;
            //}

            //显示进度数字
            var sValue = value as string;
            if (sValue != null)
            {
                string process = string.Format("{0:s}: {1:s}", (string)FilmingViewerContainee.FilmingResourceDict[
                        "UID_Filming_Loading_ProcessStep"], sValue);
                return process;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
