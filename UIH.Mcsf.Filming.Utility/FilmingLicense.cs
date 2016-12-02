using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIH.Mcsf.License;

namespace UIH.Mcsf.Filming
{
    public class FilmingLicense
    {
        private static McsfLicense _license;
        private static string _isLicenseDBT = "";
        public static bool IsLicenseDBT
        {
            get
            {
                if (_license == null)
                    _license = new McsfLicense();
                if (string.IsNullOrEmpty(_isLicenseDBT))
                {
                    _isLicenseDBT = _license.CheckExpirationTime("DBT").ToString();
                }
                return _isLicenseDBT == "0";
            }
        }
    }
}
