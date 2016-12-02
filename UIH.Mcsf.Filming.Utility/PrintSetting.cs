using System;

namespace UIH.Mcsf.Filming
{
    public class PrintSetting
    {
        #region Properties

        public uint Copies
        {
            get { return _copies; }
            set { _copies = value; }
        }

        public PRINT_PRIORITY Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public DateTime FilmingDateTime
        {
            get { return _filmingDateTime; }
            set { _filmingDateTime = value; }
        }

        public bool IfSaveElectronicFilm
        {
            get { return _ifSaveElectronicFilm; }
            set { _ifSaveElectronicFilm = value; }
        }

        public string MediaType
        {
            get { return _mediaType; }
            set { _mediaType = value; }
        }
        public string FilmDestination
        {
            get { return _filmDestination; }
            set { _filmDestination = value; }
        }

        public bool IfColorPrint
        {
            get { return _ifColorPrint; }
            set { _ifColorPrint = value; }
        }

        #endregion  //Properties

        #region Fields

        private uint _copies = 1;
        private PRINT_PRIORITY _priority = PRINT_PRIORITY.MEDIUM;
        private DateTime _filmingDateTime = new DateTime();
        private bool _ifSaveElectronicFilm = false;
        private string _mediaType = string.Empty;
        private string _filmDestination = string.Empty;
        private bool _ifColorPrint = false;
        #endregion  //Fields
    }

    public enum PRINT_PRIORITY
    {
       HIGH = 1,
       MEDIUM = 2,
       LOW = 3
	}
}
