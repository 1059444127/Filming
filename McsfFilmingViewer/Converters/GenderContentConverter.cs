using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
namespace UIH.Mcsf.Filming.Converters
{
    public class GenderContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var gender = value as string;
            return GenderNLS_Converter(gender);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        private string GenderNLS_Converter(string genderDICOM)
        {
            if (genderDICOM == "Mixed" || genderDICOM==string.Empty) return genderDICOM;

            string nlsGender = genderDICOM;

            if (genderDICOM == "F" || genderDICOM == "Female")
            {
                nlsGender = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Gender_Female"] as string;
            }
            else if (genderDICOM == "M" || genderDICOM == "Male")
            {
                nlsGender = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Gender_Male"] as string;
            }
            else if (genderDICOM == "O" || genderDICOM == "Other" )
            {
                nlsGender = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Gender_Other"] as string;
            }
            else
            {
                nlsGender = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Gender_Unknown"] as string;
            }

            return nlsGender;
        }
    }

}
