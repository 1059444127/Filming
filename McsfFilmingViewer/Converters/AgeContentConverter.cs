using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
namespace UIH.Mcsf.Filming.Converters
{
    public class AgeContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var age = value as string;

            return AgeNLS_Converter(age);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        /// <summary>
        /// replace the suffix of DICOM files to the NLS format.
        /// </summary>
        /// <param name="patientAge"></param>
        /// <returns></returns>
        private string AgeNLS_Converter(string patientAge)
        {
            string newAgeWithSuffix = patientAge;
            if (string.IsNullOrEmpty(patientAge))
            {
                return "";
            }

            if (FilmingViewerContainee.FilmingResourceDict == null)
            {
                return "";
            }

            if (patientAge.Contains("Y"))
            {
                string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Year"] as string;
                newAgeWithSuffix = patientAge.Replace("Y", yearSuffix);
            }
            else if (patientAge.Contains("M"))
            {
                string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Month"] as string;
                newAgeWithSuffix = patientAge.Replace("M", yearSuffix);
            }
            else if (patientAge.Contains("W"))
            {
                string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Week"] as string;
                newAgeWithSuffix = patientAge.Replace("W", yearSuffix);
            }
            else if (patientAge.Contains("D"))
            {
                string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Day"] as string;
                newAgeWithSuffix = patientAge.Replace("D", yearSuffix);
            }

            return newAgeWithSuffix;
        }
    }
}
