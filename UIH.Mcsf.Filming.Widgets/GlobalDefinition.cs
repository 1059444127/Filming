
using System.Collections;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Widgets
{
    public static class GlobalDefinition
    {
        public const string Mixed = "Mixed";

        public const string StarsString = "****";

        #region [--Tag Name Displayed in Job Manager--]

        public const string AccessionNumber = "AccessionNumber";

        #endregion

        public const string MixedEFilmModality = "OT";


        #region [--UI Definition--]

        public const int DefaultDisplayMode = 1;

        public const int MaxDisplayMode = 8;

        public static readonly IList<int> DisplayModes = new List<int>{ 1, 2, 3, 4, 6, 8 };

        #endregion //[--UI Definition--]

        
    }
}
