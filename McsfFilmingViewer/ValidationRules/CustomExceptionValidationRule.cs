using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace UIH.Mcsf.Filming.ValidationRules
{
    public class CustomExceptionValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (null == value)
            {
                string tip = (string)FilmingViewerContainee.FilmingResourceDict[
                        "UID_Filming_Validation_Exception"];

                return new ValidationResult(false, tip);
            }
            return ValidationResult.ValidResult;
        }
    }
}
