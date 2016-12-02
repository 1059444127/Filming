using System;
using System.Globalization;
using System.Windows.Data;

namespace UIH.Mcsf.Filming.Converters
{
    /// <summary>
    /// Assume that CurrentFilmSizeRatioOfPortrait unit is "IN" or "CM"
    /// </summary>
    public class FilmSizeConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var filmSize = value as string;
                if (filmSize == null) return value;//throw new NullReferenceException("Invalid Film Size");
                var unitIndex = filmSize.IndexOf("IN", StringComparison.InvariantCultureIgnoreCase);
                if (unitIndex == -1) unitIndex = filmSize.IndexOf("CM", StringComparison.InvariantCultureIgnoreCase);
                if (unitIndex == -1) return value;
                return filmSize.Remove(unitIndex, 2);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.Message);
            }
            return value;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var displayFilmSize = value as string;
                if(displayFilmSize == null) throw new NullReferenceException("Invalid display Film Size");
                var unit = displayFilmSize.Substring(displayFilmSize.Length - 3, 2);
                var multiplicationSignIndex = displayFilmSize.IndexOf("X", StringComparison.InvariantCultureIgnoreCase);
                return displayFilmSize.Insert(multiplicationSignIndex - 1, unit);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.Message);
            }
            return value;
        }

        #endregion
    }
}
